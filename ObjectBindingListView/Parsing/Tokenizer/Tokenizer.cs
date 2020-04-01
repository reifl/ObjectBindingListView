using ObjectBindingListView.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectBindingListView.Parsing.Tokenizer
{
    public class Tokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        public Tokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.Function, "([A-Za-z_][A-Za-z0-9_]*\\()", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "and", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Between, "between", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, "\\)", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, ",", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "=", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotIn, "not in", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.In, "in", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Like, "lke", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Limit, "^limit", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Match, "match", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "!=|<>", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterOrEqual, ">=", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterThan, ">", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LowerOrEqual, "<=", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LowerThan, "<", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotLike, "not like", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Invert, "not", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "\\(", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "or", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.True, "true", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.False, "false", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Null, "null", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Is, "is", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DateTimeValue, "\\d\\d\\d\\d-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d", 3));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "'(?:[^'\\\\]|\\\\.)*'", 3));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "-?\\d+\\.\\d+|-?\\d+", 3));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Variable, "([A-Za-z_][A-Za-z0-9_]*)", 3));
        }

        public IEnumerable<DslToken> Tokenize(string lqlText)
        {
            var tokenMatches = FindTokenMatches(lqlText);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new DslToken(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new DslToken(TokenType.SequenceTerminator);
        }

        private List<TokenMatch> FindTokenMatches(string lqlText)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(lqlText).ToList());

            return tokenMatches;
        }
    }
}
