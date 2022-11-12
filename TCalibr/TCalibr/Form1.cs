namespace TCalibr
{
    public partial class Form1 : Form, IMessageHandler
    {
        USBSerialPort USBPort;
        DataArrays? DataA;
        ByteDecomposer Decomposer;
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
        }

        private void OnPacketReceived(object? sender, PacketEventArgs e)
        {
           labADCValue.Text = e.DCValue.ToString();
        }

        private void OnConnectionOk()
        {
            labPort.Text = USBPort.PortNames[USBPort.CurrentPort];
        }

        public event Action<Message> WindowsMessageHandler;

        private void timerRead_Tick(object sender, EventArgs e)
        {
                if (USBPort?.PortHandle?.IsOpen == true)
                {
                    Decomposer?.Decompos(USBPort, null, null);
                }
        }
    }
}