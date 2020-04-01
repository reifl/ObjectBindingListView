using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.DataRepresentation
{
    public class FunctionCallValue : IValue
    {
        public string FunctionName;
        public IList<IValue> Parameters = new List<IValue>();
    }
}
