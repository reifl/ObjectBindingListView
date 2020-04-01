using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentationNew
{
    class FunctionCallValue
    {
        public string FunctionName;
        public IList<IValue> Parameters = new List<IValue>();
    }
}
