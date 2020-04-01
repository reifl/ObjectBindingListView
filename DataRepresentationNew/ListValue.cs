using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.DataRepresentation
{
    public class ListValue : IValue
    {
        public IList<string> Values = new List<string>();
    }
}
