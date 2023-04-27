namespace TCalibr
{
    public partial class Form1 : Form, IMessageHandler
    {
        const byte coommandWriteCoeff = 11;
        const byte coommandGetNumber = 12;
        string LogFileName = "hisory.txt";
        static double P_Step1 = 100;
        static double P_Step2 = 200;
        static double P_Step3 = 250;
        static double SumP = (P_Step1 + P_Step2 + P_Step3);
        //static double P_Step1 = 116.2; //мм.рт.ст для давления P0_0XX
        //static double P_Step2 = 191.2;
        //static double P_Step3 = 266.6;
        //static double SumP = (P_Step1 + P_Step2 + P_Step3);
        double CalibrationCoeff;

        USBSerialPort USBPort;
        ByteDecomposer Decomposer;
        CalibrationStep Status = CalibrationStep.NoConnected;
        double CurrentPressure;
        List<double> Values = new();

        public Form1()
        {
            InitializeComponent();
            string ConnectionString = "USBSER";
            Decomposer = new ByteDecomposer();
            Decomposer.OnDecomposePacketEvent += OnPacketReceived;
            USBPort = new USBSerialPort(this, Decomposer.BaudRate, ConnectionString);
            USBPort.ConnectionOk += OnConnectionOk;
            USBPort.Connect();
            labMessage.Text = MessagesStrings.Connect;
            listView1.View = View.Details;
            labCoeff.Text = "";
            labTargetPressure.Text = "";
            SetColors();
        }

        public event Action<Message> WindowsMessageHandler;
        protected override void WndProc(ref Message m)
        {
            const int WM_DEVICECHANGE = 0x0219;
            if (m.Msg == WM_DEVICECHANGE)
            {
                WindowsMessageHandler?.Invoke(m);
            }
            base.WndProc(ref m);
        }

        private void SetColors()
        {
            Color panColor = Color.AliceBlue;
            //Color butColor = SystemColors.Control;
            //butSetZero.BackColor = butColor;
            //butContinue.BackColor = butColor;
            //butRepeat.BackColor = butColor;
            //butWrite.BackColor = butColor;
            panConnect.BackColor = panColor;
            panMessages.BackColor = panColor;
            panValue.BackColor = panColor;
            tbWarning.BackColor = panColor;
            tableLayoutPanel1.BackColor = panColor;
        }

        private void ResetState()
        {
            Status = CalibrationStep.GetNumber;
            USBPort.WriteByte(coommandGetNumber);
            USBPort.PortHandle.BaseStream.Flush();
            USBPort.BytesRead = 0;
            Thread.Sleep(1000);
            butRepeat.Enabled = false;
            butWrite.Enabled = false;
            butContinue.Enabled = true;
            butSetZero.Enabled = true;
            listView1.Items.Clear();
            labCoeff.Text = "";
            panValue.Visible = true;
        }

        private void OnPacketReceived(object? sender, PacketEventArgs e)
        {
            CurrentPressure = e.RealTimeValue;
            labADCValue.Text = e.RealTimeValue.ToString();
        }

        private void OnConnectionOk()
        {
            ResetState();
        }

        private void OnDisconnected()
        {
            butRepeat.Enabled = false;
            butWrite.Enabled = false;
            butContinue.Enabled = false;
            panValue.Visible = false;
            Text = "Калибровка тонометра ";
        }

        private void timerRead_Tick(object sender, EventArgs e)
        {
            if (USBPort?.PortHandle?.IsOpen == true)
            {
                Decomposer?.Decompos(USBPort);
                if (!Decomposer.WaitNumberMode)
                {
                    Status = CalibrationStep.Step01;
                    Text = "Калибровка тонометра " + Decomposer.SerialNumStr;
                }
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            panValue.Visible = (Status != CalibrationStep.NoConnected) && 
                               (Status != CalibrationStep.Completed);
            tbWarning.Visible = Status == CalibrationStep.Step01;
            labValve.Visible = Status == CalibrationStep.Step01;

            labMessage.Text = Status switch
            {
                CalibrationStep.NoConnected => MessagesStrings.Connect,
                CalibrationStep.Step01 => MessagesStrings.SetPressure,
                CalibrationStep.Step02 => MessagesStrings.SetPressure,
                CalibrationStep.Step03 => MessagesStrings.SetPressure,
                CalibrationStep.ReadyToRecord => "",
                CalibrationStep.Completed => MessagesStrings.Completed,
                _ => MessagesStrings.Connect
            };            

            labTargetPressure.Text = Status switch
            {
                CalibrationStep.Step01 => MessagesStrings.Step01,
                CalibrationStep.Step02 => MessagesStrings.Step02,
                CalibrationStep.Step03 => MessagesStrings.Step03,
                _ => ""
            };

            labPressButton.Text = Status switch
            {
                CalibrationStep.Step01 => MessagesStrings.PressContinue,
                CalibrationStep.Step02 => MessagesStrings.PressContinue,
                CalibrationStep.Step03 => MessagesStrings.PressContinue,
                CalibrationStep.ReadyToRecord => MessagesStrings.PressWrite,
                CalibrationStep.Completed => MessagesStrings.CloseValve,
                _ => ""
            };

            labADCValue.Visible = (Status == CalibrationStep.Step01) ||
                                  (Status == CalibrationStep.Step02) ||
                                  (Status == CalibrationStep.Step03);

            if (USBPort == null)
            {
                labPort.Text = MessagesStrings.NoConnection;
                OnDisconnected();
                Status = CalibrationStep.NoConnected;
                return;
            }
            if (USBPort.PortHandle == null)
            {
                labPort.Text = MessagesStrings.NoConnection;
                OnDisconnected();
                Status = CalibrationStep.NoConnected;
                return;
            }
            if (USBPort.PortHandle.IsOpen)
            {
                labPort.Text = "Тонометр подключен к порту " + USBPort.PortNames[USBPort.CurrentPort];
            }
            else
            {
                labPort.Text = MessagesStrings.NoConnection;
                OnDisconnected();
                Status = CalibrationStep.NoConnected;
            }
        }

        private void butContinue_Click(object sender, EventArgs e)
        {
            tbWarning.Visible = false;
            butSetZero.Enabled = false;
            string pressure = Status switch
            {
                CalibrationStep.Step01 => MessagesStrings.Step01,
                CalibrationStep.Step02 => MessagesStrings.Step02,
                CalibrationStep.Step03 => MessagesStrings.Step03,
                _ => ""
            };
            listView1.Items.Add(new ListViewItem(new String[] { pressure, CurrentPressure.ToString() }));
            Values.Add(CurrentPressure);
            Status++;
            if (Status == CalibrationStep.ReadyToRecord)
            {
                butContinue.Enabled = false;
                butRepeat.Enabled = true;
                butWrite.Enabled = true;
                CalibrationCoeff = Values.Sum() / SumP;
                labCoeff.Text = "Калибровочный коэффициент : " + CalibrationCoeff.ToString("0.##");
            }
        }

        private void butWrite_Click(object sender, EventArgs e)
        {
            Status = CalibrationStep.Completed;
            butWrite.Enabled = false;
            labCoeff.Text = "";
            double RoundedCoeff = Math.Round(CalibrationCoeff, 2);
            double floor = Math.Floor(RoundedCoeff);
            double fract = Math.Floor((RoundedCoeff - floor) * 100);
            byte first = (byte)floor;
            byte second = (byte)fract;
            byte[] buf = { coommandWriteCoeff, first, second };
            USBPort.WriteBuf(buf);
            File.WriteAllText(LogFileName, DateTime.Now.ToString("dd.MM.yy hh:mm") + " " + Decomposer.SerialNumStr + " " + RoundedCoeff.ToString());
        }

        private void butSetZero_Click(object sender, EventArgs e)
        {
            Decomposer.RemoveZeroMode = true;
        }

        private void butRepeat_Click(object sender, EventArgs e)
        {
            ResetState();
            Decomposer.RemoveZeroMode = true;
        }
    }
}