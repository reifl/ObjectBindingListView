﻿using ObjectBoundBindingList.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.TokenizerNew
{
    public class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly int _precedence;

        public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence)
        {
            _regex = new Regex(regexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
            _precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = _returnsToken,
                    Value = matches[i].Value,
                    Precedence = _precedence
                };
            }
        }
    }
}
