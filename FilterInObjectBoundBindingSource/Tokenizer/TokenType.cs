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
        Between,
        CloseParenthesis,
        Comma,
        DateTimeValue,
        Equals,
        In,
        Like,
        Limit,
        Match,
        GreaterThan,
        LowerThan,
        GreaterOrEqual,
        LowerOrEqual,
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
        StringValue,
        SequenceTerminator,
        Function,
        Variable,
        Invalid
    }
}
