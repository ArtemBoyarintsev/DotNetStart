using System.IO;

namespace Fabric
{
    class Parametrs
    {
        public int BodiesStorageCapacity = 20;
        public int EnginesStorageCapacity = 10;
        public int CarStorageCapacity = 10;
        public int NoAssemblers = 10; // число сборщиков.
        public int NoEngineProducers = 10;
        public int NoBodyProducers = 8;
        public int TimeOfEngineProducing = 1;
        public int TimeOfBodyProducing = 5;
        public int ManagerSleepTime = 3000;
    }


    class ProductionLine
    {
        public static void Main(string[] args)
        {
            using (TextWriter Log = TextWriter.Synchronized(new StreamWriter(new FileStream("Log.txt", FileMode.Create))))
            {
                ProductionLine Line = new ProductionLine(Log);
                Line.Start();
                UI ui = new UI(Line);
                ui.Drawing();
            }
        }

        public ProductionLine(TextWriter Log)
        {
            this.Log = Log;
            SignalMonitor RequestToFactory = new SignalMonitor();
            Parametrs Params = new Parametrs();
            CarStorage = new CarShopStorage(Params.CarStorageCapacity);
            Shop = new Shop(CarStorage, RequestToFactory,Log,Params);
            Factory = new Factory(CarStorage, RequestToFactory, Params,Log);
            BuyerCreator = new BuyerCreator(Shop,Log);
            Logger = new InfoLogger(Log, this);
        }

        public void Start()
        {
            Factory.StartFactory();
            BuyerCreator.Start();
            Logger.Start();
            Shop.StartPeriodicOrder();
        }

        public void Stop()
        {
            Factory.StopFactory();
            BuyerCreator.Stop();
            Shop.StopPeriodicOrder();
            Logger.Stop();
        }

        public CarrierObject GetInfo()
        {
            CarrierObject CarOb = new CarrierObject();
            Factory.PutInfo(CarOb);
            CarOb.WasBought = Shop.BoughtCount;
            return CarOb;
        }

        private TextWriter Log;

        private Storage<Car> CarStorage;
        private Factory Factory;
        private BuyerCreator BuyerCreator;
        private InfoLogger Logger;

        private Shop Shop
        {
            get;
        }

    }
}
