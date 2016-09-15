namespace Fabric
{
    class SignalMonitor
    {
        public void Request()
        {
            SignalCount++;
        }
        public void Supply()
        {
            SignalCount--;
        }
        public bool Require()
        {
            return SignalCount > 0;
        }

        private int SignalCount = 0;
    }
}
