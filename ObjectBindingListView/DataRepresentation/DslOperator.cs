using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
{
    public enum DslOperator
    {
        NotDefined,
        Equals,
        NotEquals,
        Like,
        NotLike,
        In,
        NotIn,
        Greater,
        GreaterOrEqual,
        Lower,
        LowerOrEqual,
        Is,
        OpenParenthesis,
        Function
    }
}
