using System;
using System.Reflection;
using System.IO;

namespace JSON_Parser
{
    class BadArgumentException:Exception
    {

    }
    class JSON_Serializator
    {
        public JSON_Serializator(object Object,TextWriter Out)
        {
            this.Object = Object;
            this.Out = Out;
            if (null == Object)
            {
                throw new BadArgumentException();
            }
        }

        public void SerializeStart()
        {
            Type Type = Object.GetType();
            if (TypeIsPrimitive(Type))
            {
                SerializePrimitive(Type,Object);
                return;
            }
            Out.WriteLine("{");
            if (!CanSerializeObject())
            {
                Out.WriteLine("Not Serialized class!");
                return;
            }
            
            FieldInfo[] Fields = Type.GetFields();
            foreach(FieldInfo Field in Fields)
            {
                if (CanSerializeField(Type.GetCustomAttributes(false)))
                {
                    SerializeField(Field);
                }
            }
            Out.WriteLine("}");
        }

        private bool CanSerializeField(object[] Attributes)
        {
            foreach (object A in  Attributes)
            {
                if (A.GetType() == typeof(NonSerializableField))
                {
                    return false;
                }
            }
            return true;
        }
        private bool CanSerializeObject()
        {
            Type Type = Object.GetType();
            object[] Attributes = Type.GetCustomAttributes(false);
            foreach(object A  in Attributes)
            {
                if (A.GetType() == typeof(SerializableClass))
                {
                    return true;
                }
            }
            return false;
        }

        private void SerializePrimitive(Type Type,object Obj)
        {

            if (Type == typeof(string))
            {
                Out.Write("\"{0}\"", Obj);
                return;
            }
            if (Type.IsArray)
            {
                Out.Write("[ ");
                Array A = (Array)Obj;
                foreach (object O in A)
                {
                    new JSON_Serializator(O,Out).SerializeStart();
                    Out.Write(" ");
                }
                Out.Write("]");
                return;
            }
            Out.Write("{0}", Obj);
        }
        
        private bool TypeIsPrimitive(Type Type)
        {
            return Type.IsPrimitive || Type.IsArray || Type == typeof(string);
        }

        private void SerializeField(FieldInfo Field)
        {
            Out.Write("\"{0}\":", Field.Name);
            if (!TypeIsPrimitive(Field.FieldType))
            { 
                new JSON_Serializator(Field.GetValue(Object),Out).SerializeStart();
                return;
            }
            SerializePrimitive(Field.FieldType,Field.GetValue(Object));
            Out.WriteLine();
        }
        

        private object Object;
        private TextWriter Out;
    }
}
