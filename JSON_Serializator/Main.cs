using System;
using System.IO;
using System.Reflection.Emit;

namespace JSON_Parser
{ 
    class Program
    {
        public static int Main()
        {
            try
            {
                using (TextWriter Out = new StreamWriter("Out.txt"))
                {
                    Type T = TypeBuilder.GetType("JSON_Parser.SomeTestClass");
                    new JSON_Serializator(Activator.CreateInstance(T), Out).SerializeStart();
                }
            }
            catch (BadArgumentException)
            {
                Console.Error.WriteLine("Bad Argument!");
            }
            Console.WriteLine("Press Any Key!");
            Console.ReadKey();
            return 0;
        }
    }


    [SerializableClass]
    public class AnotherTestClass
    {
        public int FirstAnotherValue = 35;

        public int SecondAnotherValue = 47;

        public string[] TestArray = { "Hi", "Robert", "How Are You" };

    }

    [SerializableClass]
    public class SomeTestClass
    {
        public int FirstTestValue = 5;

        [NonSerializableField]
        public int SecondTestValue = 7;

        public int ThirdTestValue = 12;

        public int[][] TestArray = {  new int[] { 3, 4, 5, 6, 7, 8, 9 }, new int[] { 5, 6, 7, 6, 7, 8, 9 } };

        public string TestString = "Hello World!";

        public AnotherTestClass TestClass = new AnotherTestClass();

        public AnotherTestClass[] TestDifficultArray = { new AnotherTestClass(), new AnotherTestClass() };
    }
    
}
