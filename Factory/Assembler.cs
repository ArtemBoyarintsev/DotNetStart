
namespace Fabric
{
    class AssemblerFunction
    {
        private Storage<Engine> EngineStorage;
        private Storage<Body> BodyStorage;
        private Storage<Car> CarStorage;

        public AssemblerFunction(AssemblerFunction A)
        {
            EngineStorage = A.EngineStorage;
            BodyStorage = A.BodyStorage;
            CarStorage = A.CarStorage;
        }
        public AssemblerFunction(Storage<Engine> SE,Storage<Body> SB, Storage<Car> SC)
        {
            EngineStorage = SE;
            BodyStorage = SB;
            CarStorage = SC;
        }
        
        public void Assemble()
        {
            Engine Engine = EngineStorage.GetItem();
            Body Body = BodyStorage.GetItem();
            CarStorage.PutItem(new Car(Engine, Body));
        }
    }
}
