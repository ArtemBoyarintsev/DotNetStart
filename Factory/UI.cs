using System;
using System.Threading;

namespace Fabric
{ 
    class UI
    {
        ProductionLine ProductionLine;
        Thread StList;
        public UI(ProductionLine ProductionLine)
        {
            this.ProductionLine = ProductionLine;
            StList  = new Thread(this.StopListener);
            StList.Start();
        }

        public void Drawing()
        {
            while (IsDraw)
            {
                Thread.Sleep(1000);
                Draw();
            }
           
        }

        public void Draw()
        {
            CarrierObject CarOb = ProductionLine.GetInfo();

            Console.WriteLine("Bodies On Storage {0}", CarOb.BodiesOnStorage);
            Console.WriteLine("Engines On Storage {0}", CarOb.EnginesOnStorage);
            Console.WriteLine("Cars On Storage {0}", CarOb.CarOnStorage);

            Console.WriteLine("Was Bodies Produced {0}", CarOb.WasBodiesProduced);
            Console.WriteLine("Was Engines Produced {0}", CarOb.WasEnginesProduced);
            Console.WriteLine("Was Bought {0}", CarOb.WasBought);
        }

        public void StopListener()
        {
            try
            {
                while (true)
                {
                    ConsoleKeyInfo Info = Console.ReadKey();
                    if (Info.KeyChar == 's')
                    {
                        ProductionLine.Stop();
                        IsDraw = false;
                        return;
                    }
                }
            }
            catch(ThreadAbortException)
            {
                Console.Error.WriteLine("Unexpected Abort");
                ProductionLine.Stop();
            }
        }

        private bool IsDraw = true;
    }
}
