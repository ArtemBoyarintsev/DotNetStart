using System.Threading;
using System.IO;

namespace Fabric
{
    class Shop
    {
        public int BoughtCount
        {
            get
            {
                return WasBought;
            }
        }
        public Shop(Storage<Car> CarStorage,SignalMonitor RequestToFactory,TextWriter Log,Parametrs Params)
        {
            this.CarStorage = CarStorage;
            this.Log = Log;
            this.RequestToFactory = RequestToFactory;
            ManagerSleepTime = Params.ManagerSleepTime;
            CarRequireCount = Params.CarStorageCapacity;
            ShopManagerThread = new Thread(this.PeriodicOrder);
        }
        
        public Car TryToGetItem()
        {
            Car car = CarStorage.TryToGetItem();
            if (car!=null)
            {
                IncBoughtCount();
            }
            return car;
        }
        
        public Car GetItem(string Name)
        {
            Log.WriteLine(Name + " have requested a Car");
            Car car = CarStorage.TryToGetItem();
            if (car != null)
            {
                IncBoughtCount();
                return car;
            }
            lock (RequestToFactory)
            {
                RequestToFactory.Request();
                Monitor.Pulse(RequestToFactory);
            }
            car = CarStorage.GetItem();
            if (car != null)
            {
                IncBoughtCount();
            }
            return car;
        }

   
        public void StopPeriodicOrder()
        {
            ShopManagerThread.Abort();
            ShopManagerThread.Join();
        }

        public void StartPeriodicOrder()
        {
            ShopManagerThread.Start();
        }
        private void PeriodicOrder()
        {
            try
            {
                int Count = CarStorage.CountOnStorage;
                while (true)
                {
                    if (Count < CarRequireCount)
                    {
                        lock (RequestToFactory)
                        {
                            for (int i = 0; i < CarRequireCount - Count; ++i)
                            {
                                RequestToFactory.Request();
                            }
                            Monitor.Pulse(RequestToFactory);
                        }
                    }
                    Thread.Sleep(ManagerSleepTime);
                }
            }
            catch(ThreadAbortException)
            {
                Log.WriteLine("Shop Manager is Stopping");
            }
        }

        private void IncBoughtCount()
        {
            lock (MonitorObject)
            {
                WasBought++;
            }
        }

        private TextWriter Log;
        private Storage<Car> CarStorage;
        private Thread ShopManagerThread;
        private SignalMonitor RequestToFactory;
        private int WasBought = 0;
        private object MonitorObject = new object();

        private int CarRequireCount;
        private int ManagerSleepTime;

    }
}
