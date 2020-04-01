using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
{
    public enum DslObject
    {
        Application,
        ExceptionType,
        Message,
        StackFrame,
        Fingerprint,
        Variable,
        FixedValue,
        NULL,
        Function,
        OpenParenthesis,
        CloseParenthesis
    }
}
