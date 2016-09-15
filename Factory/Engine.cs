namespace Fabric
{
    class Engine
    { 
        public Engine()
        {
            lock (Mon)
            {
                SerialNumber = EngineCount++;
            }
        }

        private static int EngineCount = 0;
        private static object Mon = new object();
        private int SerialNumber
        {
            get;
        }
    }
}
