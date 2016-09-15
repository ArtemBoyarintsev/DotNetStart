using System;
using System.IO;
using System.Threading;
using System.Text;


namespace Fabric
{
    class Buyer
    {
        private const int NameSize = 5;
        private Shop Shop;
        private TextWriter Log;
        private string Name;
        private Thread BuyerThread;
        public Buyer(Shop Shop,TextWriter Log)
        {
            this.Shop = Shop;
            this.Log = Log;
            BuyerThread = new Thread(this.Buy);
            BuyerThread.IsBackground = true;
            byte[] N = new byte[NameSize];
            Random R = new Random();
            for (int i = 0; i < NameSize; ++i)
            {
                N[i] = (byte)(R.Next() % 26 + 'A');
            }
            Name = "Buyer " + Encoding.ASCII.GetString(N);
        }

        public void Start()
        {
            BuyerThread.Start();
        }

        public void Buy()
        {
            try
            {
                Log.WriteLine(Name + " wanna buy car.");
                Car car = Shop.TryToGetItem();
                if (car != null)
                {
                    Log.WriteLine(Name + " have bought car");
                    return;
                }
                car = Shop.GetItem(Name);
                Log.WriteLine(Name + " have bought car");
            }
            catch (Exception er)
            {
                Console.WriteLine("Buyer Error!");
                Console.Error.WriteLine(er.StackTrace);
            }
        }
    }

    class BuyerCreator
    {
        private TextWriter Log;
        private Shop Shop;
        private Thread ThreadCreator;

        public BuyerCreator(Shop Shop, TextWriter Log)
        {
            this.Log = Log;
            this.Shop = Shop;
            ThreadCreator = new Thread(Create);
        }

        public void Start()
        {
            if (ThreadCreator.ThreadState == ThreadState.Unstarted)
            {
                ThreadCreator.Start();
            }
        }

        public void Create()
        {
            try
            {
                Random Random = new Random();
                int t = 0;
                while (true)
                {
                    Buyer Buyer = new Buyer(Shop, Log);
                    Buyer.Start();
                    t = Random.Next() % 300;
                    Thread.Sleep(t);
                }
            }
            catch(ThreadAbortException)
            {
                Console.WriteLine("Stop Buyer Generation");
            }
        }

        public void Stop()
        {
            ThreadCreator.Abort();
            ThreadCreator.Join();
        }
    }
}
