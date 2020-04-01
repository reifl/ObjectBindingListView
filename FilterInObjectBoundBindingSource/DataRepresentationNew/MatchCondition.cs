using ObjectBoundBindingList.DataRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
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
