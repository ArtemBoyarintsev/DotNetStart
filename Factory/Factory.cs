using System.Threading;
using System.IO;

namespace Fabric
{
    class AssemblerController
    {
        private ThreadPool Workers;
        private AssemblerFunction Worker;

        public AssemblerController(int NoThread, AssemblerFunction Worker,TextWriter Log)
        {
            Workers = new ThreadPool(NoThread,Log);
            this.Worker = Worker;
        }

        public void Assemble()
        {
            Workers.PutTask(Worker.Assemble);
        }
        public void Stop()
        {
            Workers.Stop();
        }
    }

    class Factory
    {
        
        public Factory(Storage<Car> CarStorage,SignalMonitor Request, Parametrs Params,TextWriter Log)
        {
            this.Request = Request;
            this.CarStorage = CarStorage;
            this.Log = Log;

            BodyStorage = new Storage<Body>(Params.BodiesStorageCapacity);
            EngineStorage = new Storage<Engine>(Params.EnginesStorageCapacity);

            AssemblerController = new AssemblerController(Params.NoAssemblers, 
                                            new AssemblerFunction(EngineStorage, BodyStorage, CarStorage),Log);

            EngineProducers = new Producer<Engine>[Params.NoEngineProducers];
            BodyProducers = new Producer<Body>[Params.NoBodyProducers];
            

            for (int i = 0; i < Params.NoEngineProducers;++i)
            {
                EngineProducers[i] = new Producer<Engine>(Params.TimeOfEngineProducing, 
                                                                EngineStorage, "Engine Producers#" + i,Log);
            }
            for (int i = 0; i < Params.NoBodyProducers; ++i)
            {
                BodyProducers[i] = new Producer<Body>(Params.TimeOfBodyProducing,
                                                                BodyStorage, "Body Producers#" + i,Log);
            }

            FactoryThread = new Thread(this.Run);
        }

        public void Run()
        {
            try
            {
                foreach (Producer<Body> P in BodyProducers)
                {
                    P.Start();
                }
                foreach (Producer<Engine> P in EngineProducers)
                {
                    P.Start();
                }
                Work();
            }
            catch(ThreadAbortException)
            {
                Log.WriteLine("ThreadFactory is Stopped!");
            }
        }

        public void StartFactory()
        {
            FactoryThread.Start();
        }

        public void StopFactory()
        {
            Log.WriteLine("Factory is Stopping...");
            foreach (Producer<Body> P in BodyProducers)
            {
                P.Stop();
            }
            foreach (Producer<Engine> P in EngineProducers)
            {
                P.Stop();
            }
            AssemblerController.Stop();
            FactoryThread.Abort();
            FactoryThread.Join();
            Log.WriteLine("Factory is Stopped");
        }

        public void PutInfo(CarrierObject CarOb)
        {
            CarOb.BodiesOnStorage = BodyStorage.CountOnStorage;
            CarOb.EnginesOnStorage = EngineStorage.CountOnStorage;
            CarOb.CarOnStorage = CarStorage.CountOnStorage;
            int WasProductedEngines = 0;
            for (int i = 0; i < EngineProducers.Length; ++i)
            {
                WasProductedEngines += EngineProducers[i].CountOfProdiction;
            }
            int WasProductedBodies = 0;
            for (int i = 0; i < BodyProducers.Length; ++i)
            {
                WasProductedBodies += BodyProducers[i].CountOfProdiction;
            }
            CarOb.WasBodiesProduced = WasProductedBodies;
            CarOb.WasEnginesProduced = WasProductedEngines;
        }

        private void Work() 
        {
            while (true)
            {
                lock (Request)
                {
                    while (!Request.Require())
                    {
                        Monitor.Wait(Request);
                    }
                    AssemblerController.Assemble();
                    Request.Supply();
                }
            }
        }

        private SignalMonitor Request;

        private Storage<Body> BodyStorage;
        private Storage<Engine> EngineStorage;
        private Storage<Car> CarStorage;
        private AssemblerController AssemblerController;

        private Producer<Engine>[] EngineProducers;
        private Producer<Body>[] BodyProducers;

        private TextWriter Log;

        private Thread FactoryThread;
    }
}
