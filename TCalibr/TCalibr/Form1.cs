using System.Collections.ObjectModel;

namespace TCalibr
{
    public partial class Form1 : Form, IMessageHandler
    {
        static double P0_015 = 112.5092524; //мм.рт.ст для давления P0_0XX
        static double P0_025 = 187.5154207;
        static double P0_035 = 262.5215889;
        static double SumP = (P0_015 + P0_025 + P0_035);
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
            Status = CalibrationStep.Step015;
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
        }

        private void timerRead_Tick(object sender, EventArgs e)
        {
            if (USBPort?.PortHandle?.IsOpen == true)
            {
                Decomposer?.Decompos(USBPort);
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            panValue.Visible = (Status != CalibrationStep.NoConnected) && 
                               (Status != CalibrationStep.Completed);
            tbWarning.Visible = Status == CalibrationStep.Step015;
            labValve.Visible = Status == CalibrationStep.Step015;

            labMessage.Text = Status switch
            {
                CalibrationStep.NoConnected => MessagesStrings.Connect,
                CalibrationStep.Step015 => MessagesStrings.SetPressure,
                CalibrationStep.Step025 => MessagesStrings.SetPressure,
                CalibrationStep.Step035 => MessagesStrings.SetPressure,
                CalibrationStep.ReadyToRecord => "",
                CalibrationStep.Completed => MessagesStrings.Completed,
                _ => MessagesStrings.Connect
            };            

            labTargetPressure.Text = Status switch
            {
                CalibrationStep.Step015 => MessagesStrings.Step015,
                CalibrationStep.Step025 => MessagesStrings.Step025,
                CalibrationStep.Step035 => MessagesStrings.Step035,
                _ => ""
            };

            labPressButton.Text = Status switch
            {
                CalibrationStep.Step015 => MessagesStrings.PressContinue,
                CalibrationStep.Step025 => MessagesStrings.PressContinue,
                CalibrationStep.Step035 => MessagesStrings.PressContinue,
                CalibrationStep.ReadyToRecord => MessagesStrings.PressWrite,
                CalibrationStep.Completed => MessagesStrings.CloseValve,
                _ => ""
            };

            labADCValue.Visible = (Status == CalibrationStep.Step015) ||
                                  (Status == CalibrationStep.Step025) ||
                                  (Status == CalibrationStep.Step035);

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
                CalibrationStep.Step015 => "0,015",
                CalibrationStep.Step025 => "0,025",
                CalibrationStep.Step035 => "0,035",
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
            CalibrationCoeff = 18.69;
            byte coommandWriteCoeff = 11;
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

        private void button1_Click(object sender, EventArgs e)
        {
            butWrite_Click(null, EventArgs.Empty);
        }
    }
}