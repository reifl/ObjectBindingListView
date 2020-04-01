using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.Tokenizer
{
    public enum TokenType
    {
        NotDefined,
        And,
        Application,
        Between,
        CloseParenthesis,
        Comma,
        DateTimeValue,
        Equals,
        ExceptionType,
        Fingerprint,
        In,
        Invalid,
        Like,
        Limit,
        Match,
        GreaterThan,
        LowerThan,
        GreaterOrEqual,
        LowerOrEqual,
        Message,
        NotEquals,
        NotIn,
        NotLike,
        Invert,
        Number,
        Or,
        True,
        False,
        Null,
        Is,
        OpenParenthesis,
        StackFrame,
        StringValue,
        SequenceTerminator,
        Function,
        Variable
    }
}
