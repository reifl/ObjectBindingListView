using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
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
