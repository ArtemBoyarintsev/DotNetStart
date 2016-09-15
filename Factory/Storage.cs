using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Fabric
{
    class Storage<T> where T : class
    {
        public Storage(int Limit)
        {
            Details = new List<T>();
            this.Limit = Limit;
        }

        public int CountOnStorage
        {
            get
            {
                lock (this)
                {
                    return Details.Count;
                }
            }
        }

        public virtual void PutItem(T t)
        {
            lock(ExpectPlaceMonitor)
            {
                while (CountOnStorage == Limit)
                {
                    Monitor.Wait(ExpectPlaceMonitor);
                }
                lock(this)
                {
                    Details.Add(t);
                }
            }
            lock(ExpectItemMonitor)
            {
                Monitor.Pulse(ExpectItemMonitor);
            }
        }

        public virtual T GetItem()
        {
            T Detail = TryToGetItem();
            if (Detail != null)
            {
                return Detail;
            }
            lock (ExpectItemMonitor)
            {
                while (CountOnStorage == 0)
                {
                    Monitor.Wait(ExpectItemMonitor); //sleep if storage is empty
                }

                lock(this) {
                    Detail = Details.ElementAt(0);
                    Details.Remove(Detail); // удалит используя Equals,а не ссылочное сравнение.
                }
            }
            lock(ExpectPlaceMonitor) {
                Monitor.Pulse(ExpectPlaceMonitor); //if anybody,who expect place sleep, awake him.
            }
            return Detail;
        }

        public virtual T TryToGetItem()
        {
            T Detail;
            lock(this)
            {
                if (CountOnStorage == 0)
                {
                    return null;
                }
                Detail = Details.ElementAt(0);
                Details.Remove(Detail);
            }
            lock (ExpectPlaceMonitor)
            {
                Monitor.Pulse(ExpectPlaceMonitor); //if anybody,who expect place sleep, awake him.
            }
            return Detail;
        }

        private List<T> Details;
        private int Limit;

        private object ExpectItemMonitor = new object();
        private object ExpectPlaceMonitor = new object();
    }
}
