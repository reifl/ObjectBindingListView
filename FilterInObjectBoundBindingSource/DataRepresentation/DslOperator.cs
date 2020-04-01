using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
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
