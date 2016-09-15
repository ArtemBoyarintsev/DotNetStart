using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace Fabric
{
    delegate void TaskExecute();

    class BlockingQueue<T> : Queue<T>
    {
        public BlockingQueue() : this(100)
        {
            
        }
        
        public BlockingQueue(int Limit)
        {
            this.Limit = Limit;
            FreeingExpecting = new object();
            ItemExpecting = new object();
        }

        public void Put(T t)
        {
            lock(FreeingExpecting)
            {
                while(Count == Limit)
                {
                    Monitor.Wait(FreeingExpecting);
                }
                lock(this)
                {
                    Enqueue(t);
                }
            }
            lock(ItemExpecting)
            {
                Monitor.Pulse(ItemExpecting);
            }
        }

        public T Take()
        {
            T Ret;
            lock (ItemExpecting)
            {
                while (Count == 0)
                {
                    Monitor.Wait(ItemExpecting);
                }
                lock (this)
                { 
                    Ret = Dequeue();
                }
            }
            lock (FreeingExpecting)
            {
                Monitor.Pulse(FreeingExpecting);   
            }
            return Ret;
        }

        private int Limit;
        private object FreeingExpecting;
        private object ItemExpecting;
    }
    class ThreadPool
    {
        public ThreadPool(int NoThread, TextWriter Log)
        {
            this.Log = Log;
            this.NoThread = NoThread;
            TaskQueue = new BlockingQueue<TaskExecute>();
            Threads = new Thread[NoThread];
            for (int i = 0; i < NoThread; ++i)
            {
                Threads[i] = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            TaskQueue.Take().Invoke();
                        }
                    }
                    catch(ThreadAbortException)
                    {
                        Log.WriteLine(Thread.CurrentThread.Name + " is Stopped!");
                    }
                });
                Threads[i].Name = "Worker #" + i;
                Threads[i].Start();
            }
        }
        
        public void PutTask(TaskExecute Task)
        {
            TaskQueue.Put(Task);
        }

        public void Stop()
        {
            foreach(Thread T in Threads)
            {
                T.Abort();
                T.Join();
            }
        }

        private BlockingQueue<TaskExecute> TaskQueue;
        private int NoThread;
        private Thread[] Threads;
        private TextWriter Log;
    }
}