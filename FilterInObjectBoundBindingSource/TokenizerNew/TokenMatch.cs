using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.TokenizerNew
{
    public class TokenMatch
    {
        public ObjectBoundBindingList.Tokenizer.TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
    }
}
