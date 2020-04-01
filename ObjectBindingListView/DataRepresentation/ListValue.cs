using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
{
    class ListValue : IValue
    {
        public IList<string> Values = new List<string>();
    }
}
