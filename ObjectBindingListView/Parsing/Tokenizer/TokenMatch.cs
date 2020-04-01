using ObjectBindingListView.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.Parsing.Tokenizer
{
    public class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
    }
}
