using System;

namespace JSON_Parser
{
    [AttributeUsage(AttributeTargets.Class)]
    class SerializableClass : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class NonSerializableField : Attribute
    {

    }
}
