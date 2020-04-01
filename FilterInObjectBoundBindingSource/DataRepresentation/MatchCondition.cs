using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
{
    public class MatchCondition
    {
        public MatchCondition()
        {
            SubConditions = new List<MatchCondition>();
        }

        public DslObject Object { get; set; }

        public string VariableName { get; set; }

        public string LeftSideValue { get; set; }

        public DslOperator Operator { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        public bool Not { get; set; }

        public DslLogicalOperator LogOpToNextCondition { get; set; }

        public IList<MatchCondition> SubConditions { get; set; }
    }
}
