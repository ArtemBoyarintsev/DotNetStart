using System.IO;
using System;
using System.Threading;

namespace Fabric
{
   
    class InfoLogger
    {
        public InfoLogger(TextWriter Log,ProductionLine ProductionLine)
        {
            this.Log = Log;
            this.ProductionLine = ProductionLine;
            LogThread = new Thread(LogWork);
        }
        
        public void LogWork()
        {
            try
            {
                while(true)
                {
                    Thread.Sleep(1000);
                    CarrierObject CarOb = ProductionLine.GetInfo();
                    Log.WriteLine("Bodies On Storage {0}", CarOb.BodiesOnStorage);
                    Log.WriteLine("Engines On Storage {0}", CarOb.EnginesOnStorage);
                    Log.WriteLine("Cars On Storage {0}", CarOb.CarOnStorage);

                    Log.WriteLine("Was Bodies Produced {0}", CarOb.WasBodiesProduced);
                    Log.WriteLine("Was Engines Produced {0}", CarOb.WasEnginesProduced);
                    Log.WriteLine("Was Bought {0}", CarOb.WasBought);
                }
            }
            catch(ThreadAbortException)
            {
                Console.Error.WriteLine("Log Thread Is Aborted!");
            }
        }

        public void Start()
        {
            LogThread.Start();
        }

        public void Stop()
        {
            LogThread.Abort();
            LogThread.Join();
        }

        private ProductionLine ProductionLine;
        private TextWriter Log;
        private Thread LogThread;
    }
}
