
namespace Fabric
{
    class Car
    {
        public Car(Engine Engine, Body Body)
        {
            lock (Mon)
            {
                SerialNumber = CarCount;
                this.Engine = Engine;
                this.Body = Body;
            }
        }

        private Engine Engine
        {
            get;
        }

        private Body Body
        {
            get;
        }

        private int SerialNumber
        {
            get;
        }

        private static int CarCount = 0;
        private static object Mon = new object();
    }
}
