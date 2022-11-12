namespace TCalibr
{
    internal class ByteDecomposer
    {
        public const int DataArrSize = 0x100000;

        protected const byte marker1 = 0x19; // Если маркер - 1 байт, используется этот. Если больше, то объявлять свои в наследнике

        protected DataArrays Data;

        public event EventHandler<PacketEventArgs>? OnDecomposePacketEvent;

        public uint MainIndex = 0;
        public int PacketCounter = 0;

        public bool RecordStarted;
        public bool DeviceTurnedOn;

        public int tmpZero;

        protected int tmpValue;

        protected int noDataCounter;

        protected int byteNum;

        public int SamplingFrequency => 250;
        public int BaudRate => 115200;
        public int BytesInPacket => 3;
        public int MaxNoDataCounter => 10;

        private const int _queueForACSize = 6;
        private const int _queueForDCSize = 60;
        //Очереди для усреднения скользящим окном
        protected Queue<int> QueueForDC;
        protected Queue<int> QueueForAC;
        protected Queue<int> QueueForZero;

        protected int sizeQForDC = 100;


        public ByteDecomposer(DataArrays data)
        {
            Data = data;
            QueueForDC = new Queue<int>(sizeQForDC);
        }
        protected void OnDecomposeLineEvent()
        {
            OnDecomposePacketEvent?.Invoke(
                this,
                new PacketEventArgs
                {
                    DCValue = Data.DCArray[MainIndex],
                    RealTimeValue = Data.RealTimeArray[MainIndex],
                    PacketCounter = PacketCounter,
                    MainIndex = MainIndex
                });
        }

        public int Decompos(USBSerialPort usbport, Stream saveFileStream, StreamWriter txtFileStream)
        {
            int bytes = usbport.BytesRead;
            if (bytes == 0)
            {
                noDataCounter++;
                if (noDataCounter > MaxNoDataCounter)
                {
                    DeviceTurnedOn = false;
                }
                return 0;
            }
            DeviceTurnedOn = true;
            if (saveFileStream != null && RecordStarted)
            {
                try
                {
                    saveFileStream.Write(usbport.PortBuf, 0, bytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Save file stream error" + ex.Message);
                }
            }
            for (int i = 0; i < bytes; i++)
            {
                switch (byteNum)
                {
                    case 0:// Marker
                        if (usbport.PortBuf[i] == marker1)
                        {
                            byteNum = 1;
                        }
                        break;
                    case 1:
                        tmpValue = usbport.PortBuf[i];
                        byteNum = 2;
                        break;
                    case 2:
                        tmpValue += 0x100 * usbport.PortBuf[i];
                        if ((tmpValue & 0x8000) != 0)
                        {
                            tmpValue -= 0x10000;
                        }

                        //Очередь для выделения постоянной составляющей
                        QueueForDC.Enqueue(tmpValue);
                        if (QueueForDC.Count > _queueForDCSize)
                        {
                            QueueForDC.Dequeue();
                        }

                        //Массив исходных данный - смещение
                        Data.RealTimeArray[MainIndex] = tmpValue;
                        //Массив постоянной составляющей
                        Data.DCArray[MainIndex] = (int)QueueForDC.Average();

                        //Очередь - переменная составляющая

                        //Массив переменной составляющей
                        byteNum = 0;

                        OnDecomposeLineEvent();
                        PacketCounter++;
                        MainIndex++;
                        MainIndex &= DataArrSize - 1;
                        break;
                }
            }
            usbport.BytesRead = 0;
            return bytes;
        }
    }
}
