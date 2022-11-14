namespace TCalibr
{
    public partial class Form1 : Form, IMessageHandler
    {
        static double P0_015 = 112.5092524;
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
            tbWarning.Visible = true;
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
            };
            listView1.Items.Add(new ListViewItem(new String[] { pressure, CurrentPressure.ToString() }));
            Values.Add(CurrentPressure);
            CalibrStep++;
            if (CalibrStep == CalibrationStep.ReadyToRecord)
            {
                CalibrationCoeff = Values.Sum() / SumP;
                labCoeff.Text = CalibrationCoeff.ToString("0.##");
            }
        }

        private void butWrite_Click(object sender, EventArgs e)
        {
            CalibrStep = CalibrationStep.Completed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Decomposer.RemoveZeroMode = true;
        }
    }
}