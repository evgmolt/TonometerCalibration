namespace TCalibr
{
    internal class DataArrays
    {
        private readonly int _size;

        public double[] RealTimeArray;
        public double[] DCArray;

        public int Size { get { return _size; } }
        
        public DataArrays(int size)
        {
            _size = size;
            RealTimeArray = new double[_size];
            DCArray = new double[_size];
        }
    }
}
