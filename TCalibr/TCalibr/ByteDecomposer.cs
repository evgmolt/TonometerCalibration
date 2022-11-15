namespace TCalibr
{
    internal class ByteDecomposer
    {
        private uint DataArrSize = 0x1000;

        protected const byte marker1 = 0x19;

        public event EventHandler<PacketEventArgs>? OnDecomposePacketEvent;

        public uint MainIndex = 0;
        public int PacketCounter = 0;

        public int Zero;

        protected int tmpValue;

        protected int noDataCounter;

        protected int byteNum;

        public int SamplingFrequency => 250;
        public int BaudRate => 115200;
        public int BytesInPacket => 3;
        public int MaxNoDataCounter => 10;

        //Очереди для усреднения скользящим окном
        protected Queue<int> QueueForZero;
        protected Queue<int> QueueForValue;

        protected int sizeQForZero = 100;
        protected int sizeQForValue = 20;

        public bool RemoveZeroMode = true;
        private int ZeroCountInterval = 120;

        public ByteDecomposer()
        {
            QueueForZero = new Queue<int>(sizeQForZero);
            QueueForValue = new Queue<int>(sizeQForValue);
        }
        protected void OnDecomposeLineEvent()
        {
            if (RemoveZeroMode) return;

            OnDecomposePacketEvent?.Invoke(
                this,
                new PacketEventArgs
                {
                    RealTimeValue = (int)QueueForValue.Average(),
                });
        }

        public int Decompos(USBSerialPort usbport)
        {
            int bytes = usbport.BytesRead;
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
                        QueueForZero.Enqueue(tmpValue);
                        if (QueueForZero.Count > sizeQForZero)
                        {
                            QueueForZero.Dequeue();
                        }

                        QueueForValue.Enqueue(tmpValue - Zero);
                        if (QueueForValue.Count > sizeQForValue)
                        {
                            QueueForValue.Dequeue();
                        }

                        //Массив переменной составляющей
                        byteNum = 0;

                        PacketCounter++;
                        if (RemoveZeroMode)
                        {
                            if (PacketCounter > ZeroCountInterval)
                            {
                                RemoveZeroMode = false;
                                PacketCounter = 0;
                                MainIndex = 0;
                                Zero = (int)QueueForZero.Average();
                            }
                        }
                        
                        if (PacketCounter > ZeroCountInterval * 2)
                        {
                            OnDecomposeLineEvent();
                        }

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
