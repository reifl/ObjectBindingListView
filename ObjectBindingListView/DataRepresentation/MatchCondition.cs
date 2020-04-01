using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
{
    public class MatchCondition
    {
        public IValue Value1;
        public IValue Value2;
        public DslOperator Operator;
        public IList<MatchCondition> SubConditions = new List<MatchCondition>();
        public bool Not;
        public DslLogicalOperator NextLogOperator;
    }
}
