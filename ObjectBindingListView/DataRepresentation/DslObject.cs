using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
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
