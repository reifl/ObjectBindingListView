using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ObjectBindingListView.Exceptions
{
    public class DslParserException : Exception
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
