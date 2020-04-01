using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
{
    public class DslQueryModel
    {
        public DslQueryModel()
        {
            MatchConditions = new List<MatchCondition>();
        }
        public IList<MatchCondition> MatchConditions { get; set; }
    }
}
