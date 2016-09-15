using System;
using System.IO;
using System.Threading;

namespace Fabric
{

    class Producer<T> where T : class, new()
    {
        public int TimeOfProduction
        {
            get
            {
                return ProductionTime;
            }

            set
            {
                if (value > 0)
                {
                    ProductionTime = value;
                }
            }
        }
        
        public int CountOfProdiction
        {
            get
            {
                return ProductionCount;
            }
        }

        public Producer(int ProductionTime, Storage<T> SD, String Name,TextWriter Log)
        {
            this.ProductionTime = ProductionTime;
            WorkThread = new Thread(Produce);
            this.Name = WorkThread.Name = Name;
            DetailsStorage = SD;
            this.Log = Log;
        }

        public void Start()
        { 
            WorkThread.Start();  
        }

        public void Produce()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeOfProduction); //detail production.
                    DetailsStorage.PutItem(Create());
                    ProductionCount++;
                }
            }
            catch (ThreadAbortException)
            {
                Finish();
            }
        }

        public void Stop()
        {
            WorkThread.Abort();
            WorkThread.Join();
        }

        private string Name;
        private int ProductionTime;
        private int ProductionCount;

        private Storage<T> DetailsStorage;
        TextWriter Log;
        private Thread WorkThread;


        private T Create()
        {
            return new T();
        }

        private void Finish()
        {
            Log.WriteLine("Producer {0} finished.", Name);
        }
    }
}
