using System;
using System.Runtime.Serialization;

namespace ObjectBoundBindingList.Tokenizer
{
    [Serializable]
    internal class DslParserException : Exception
    {
        public DslParserException()
        {
        }

        public DslParserException(string message) : base(message)
        {
        }

        public DslParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DslParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}