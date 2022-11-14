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
        CalibrationStep CalibrStep = CalibrationStep.NoConnected;
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
        }

        private void OnPacketReceived(object? sender, PacketEventArgs e)
        {
            CurrentPressure = e.RealTimeValue;
            labADCValue.Text = e.RealTimeValue.ToString();
        }

        private void OnConnectionOk()
        {
            CalibrStep = CalibrationStep.Step015;
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
            panValue.Visible = (CalibrStep != CalibrationStep.NoConnected) && 
                               (CalibrStep != CalibrationStep.ReadyToRecord) && 
                               (CalibrStep != CalibrationStep.Completed);
            tbWarning.Visible = CalibrStep == CalibrationStep.Step015;
            butRepeat.Enabled = (CalibrStep == CalibrationStep.ReadyToRecord) || (CalibrStep == CalibrationStep.Completed);
            butSetZero.Enabled = CalibrStep == CalibrationStep.Step015;

            labMessage.Text = CalibrStep switch
            {
                CalibrationStep.NoConnected => Messages.Connect,
                CalibrationStep.Step015 => Messages.SetPressure,
                CalibrationStep.Step025 => Messages.SetPressure,
                CalibrationStep.Step035 => Messages.SetPressure,
                CalibrationStep.ReadyToRecord => "",
                CalibrationStep.Completed => Messages.Completed,
                _ => Messages.Connect
            };
            

            labTargetPressure.Text = CalibrStep switch
            {
                CalibrationStep.Step015 => Messages.Step015,
                CalibrationStep.Step025 => Messages.Step025,
                CalibrationStep.Step035 => Messages.Step035,
                _ => ""
            };

            labPressButton.Text = CalibrStep switch
            {
                CalibrationStep.Step015 => Messages.PressContinue,
                CalibrationStep.Step025 => Messages.PressContinue,
                CalibrationStep.Step035 => Messages.PressContinue,
                CalibrationStep.ReadyToRecord => Messages.PressWrite,
                _ => ""
            };

            butContinue.Enabled = (CalibrStep == CalibrationStep.Step015) ||
                                  (CalibrStep == CalibrationStep.Step025) ||
                                  (CalibrStep == CalibrationStep.Step035);
            butWrite.Enabled = CalibrStep == CalibrationStep.ReadyToRecord;

            labADCValue.Visible = (CalibrStep == CalibrationStep.Step015) ||
                                  (CalibrStep == CalibrationStep.Step025) ||
                                  (CalibrStep == CalibrationStep.Step035);

            if (USBPort == null)
            {
                labPort.Text = "Тонометр не подключен";
                CalibrStep = CalibrationStep.NoConnected;
                return;
            }
            if (USBPort.PortHandle == null)
            {
                labPort.Text = "Тонометр не подключен";
                CalibrStep = CalibrationStep.NoConnected;
                return;
            }
            if (USBPort.PortHandle.IsOpen)
            {
                labPort.Text = "Тонометр подключен к порту " + USBPort.PortNames[USBPort.CurrentPort];
            }
            else
            {
                labPort.Text = "Тонометр не подключен";
                CalibrStep = CalibrationStep.NoConnected;
            }
        }

        private void butContinue_Click(object sender, EventArgs e)
        {
            tbWarning.Visible = false;
            string pressure = CalibrStep switch
            {
                CalibrationStep.Step015 => "0,015",
                CalibrationStep.Step025 => "0,025",
                CalibrationStep.Step035 => "0,035",
                _ => ""
            };
            listView1.Items.Add(new ListViewItem(new String[] { pressure, CurrentPressure.ToString() }));
            Values.Add(CurrentPressure);
            CalibrStep++;
            if (CalibrStep == CalibrationStep.ReadyToRecord)
            {
                butWrite.Focus();
                CalibrationCoeff = Values.Sum() / SumP;
                labCoeff.Text = "Калибровочный коэффициент : " + CalibrationCoeff.ToString("0.##");
            }
        }

        private void butWrite_Click(object sender, EventArgs e)
        {
            CalibrStep = CalibrationStep.Completed;
            labCoeff.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Decomposer.RemoveZeroMode = true;
        }

        private void butRepeat_Click(object sender, EventArgs e)
        {
            Decomposer.RemoveZeroMode = true;
            CalibrStep = CalibrationStep.Step015;
            listView1.Items.Clear();
            labCoeff.Text = "";
        }
    }
}