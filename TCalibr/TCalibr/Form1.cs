using System.Collections.ObjectModel;

namespace TCalibr
{
    public partial class Form1 : Form, IMessageHandler
    {
        static double P0_015 = 112.5092524; //мм.рт.ст для давления P0_0XX
        static double P0_025 = 187.5154207;
        static double P0_035 = 262.5215889;
        static double SumP = (P0_015 + P0_025 + P0_035);

        USBSerialPort USBPort;
        DataArrays? DataA;
        ByteDecomposer Decomposer;
        CalibrationStep Status = CalibrationStep.NoConnected;
        double CurrentPressure;
        double CalibrationCoeff;
        List<double> Values = new();

        public Form1()
        {
            InitializeComponent();
            string ConnectionString = "USBSER";
            DataA = new DataArrays(ByteDecomposer.DataArrSize);
            Decomposer = new ByteDecomposer(DataA);
            Decomposer.OnDecomposePacketEvent += OnPacketReceived;
            USBPort = new USBSerialPort(this, Decomposer.BaudRate, ConnectionString);
            USBPort.ConnectionOk += OnConnectionOk;
            USBPort.Connect();
            labMessage.Text = Messages.Connect;
            listView1.View = View.Details;
            labCoeff.Text = "";
            labTargetPressure.Text = "";
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

        public event Action<Message> WindowsMessageHandler;

        private void timerRead_Tick(object sender, EventArgs e)
        {
            if (USBPort?.PortHandle?.IsOpen == true)
            {
                Decomposer?.Decompos(USBPort, null, null);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_DEVICECHANGE = 0x0219;
            if (m.Msg == WM_DEVICECHANGE)
            {
                WindowsMessageHandler?.Invoke(m);
            }
            base.WndProc(ref m);
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            panValue.Visible = (Status != CalibrationStep.NoConnected) && 
                               (Status != CalibrationStep.Completed);
            tbWarning.Visible = Status == CalibrationStep.Step015;
            labValve.Visible = Status == CalibrationStep.Step015;

            labMessage.Text = Status switch
            {
                CalibrationStep.NoConnected => Messages.Connect,
                CalibrationStep.Step015 => Messages.SetPressure,
                CalibrationStep.Step025 => Messages.SetPressure,
                CalibrationStep.Step035 => Messages.SetPressure,
                CalibrationStep.ReadyToRecord => "",
                CalibrationStep.Completed => Messages.Completed,
                _ => Messages.Connect
            };
            

            labTargetPressure.Text = Status switch
            {
                CalibrationStep.Step015 => Messages.Step015,
                CalibrationStep.Step025 => Messages.Step025,
                CalibrationStep.Step035 => Messages.Step035,
                _ => ""
            };

            labPressButton.Text = Status switch
            {
                CalibrationStep.Step015 => Messages.PressContinue,
                CalibrationStep.Step025 => Messages.PressContinue,
                CalibrationStep.Step035 => Messages.PressContinue,
                CalibrationStep.ReadyToRecord => Messages.PressWrite,
                CalibrationStep.Completed => Messages.CloseValve,
                _ => ""
            };


            labADCValue.Visible = (Status == CalibrationStep.Step015) ||
                                  (Status == CalibrationStep.Step025) ||
                                  (Status == CalibrationStep.Step035);

            if (USBPort == null)
            {
                labPort.Text = "Тонометр не подключен";
                OnDisconnected();
                Status = CalibrationStep.NoConnected;
                return;
            }
            if (USBPort.PortHandle == null)
            {
                labPort.Text = "Тонометр не подключен";
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
                labPort.Text = "Тонометр не подключен";
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
            Status = CalibrationStep.Completed;
            butWrite.Enabled = false;
            labCoeff.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
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