namespace Fabric
{
    class Body
    {
        private static int BodyCount = 0;
        private static object Mon = new object();
        private int SerialNum
        {
            get;
        }
        public Body()
        {
            lock (Mon)
            {
                SerialNum = ++BodyCount;
            }
        }
    }
}
