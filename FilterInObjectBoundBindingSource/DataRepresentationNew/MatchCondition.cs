using ObjectBoundBindingList.DataRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentationNew
{
    public class MatchCondition
    {
        private IValue Value1;
        private IValue Value2;
        private DslOperator Operator;
        private MatchCondition SubConditions;
        private bool Not;
        private DslLogicalOperator NextLogOperator;
    }
}
