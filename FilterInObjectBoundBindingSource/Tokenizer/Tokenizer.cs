using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.Tokenizer
{

    public class Tokenizer
    {
        IList<TokenDefinition> _tokenDefinitions;
        public Tokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.Function, "^([A-Za-z_][A-Za-z0-9_]*\\()"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "^and"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Between, "^between"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotIn, "^not in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.In, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Like, "^like"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Limit, "^limit"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Match, "^match"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "^!=|^<>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterOrEqual, "^>="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterThan, "^>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LowerOrEqual, "^<="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LowerThan, "^<"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotLike, "^not like"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Invert, "^not"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "^or"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.True, "^true"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.False, "^false"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Null, "^null"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Is, "^is"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DateTimeValue, "^\\d\\d\\d\\d-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "^'(?:[^'\\\\]|\\\\.)*'"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "^-?\\d+\\.\\d+|^-?\\d+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Variable, "^([A-Za-z_][A-Za-z0-9_]*)"));
        }

        public List<DslToken> Tokenize(string lqlText)
        {
            var tokens = new List<DslToken>();

            string remainingText = lqlText;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    tokens.Add(new DslToken(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                }
                else
                {
                    if (IsWhitespace(remainingText))
                    {
                        remainingText = remainingText.Substring(1);
                    }
                    else
                    {
                        var invalidTokenMatch = CreateInvalidTokenMatch(remainingText);
                        tokens.Add(new DslToken(invalidTokenMatch.TokenType, invalidTokenMatch.Value));
                        remainingText = invalidTokenMatch.RemainingText;
                    }
                }
            }

            tokens.Add(new DslToken(TokenType.SequenceTerminator, string.Empty));

            return tokens;
        }

        private TokenMatch FindMatch(string lqlText)
        {
            foreach (var tokenDefinition in _tokenDefinitions)
            {
                var match = tokenDefinition.Match(lqlText);
                if (match.IsMatch)
                {
                    if (tokenDefinition.TokenType == TokenType.StringValue)
                    {
                        if (match.Value.EndsWith("\\"))
                        {

                        }
                    }
                    return match;
                }

            }

            return new TokenMatch() { IsMatch = false };
        }

        private bool IsWhitespace(string lqlText)
        {
            return Regex.IsMatch(lqlText, "^\\s+");
        }

        private TokenMatch CreateInvalidTokenMatch(string lqlText)
        {
            var match = Regex.Match(lqlText, "(^\\S+\\s)|^\\S+");
            if (match.Success)
            {
                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = lqlText.Substring(match.Length),
                    TokenType = TokenType.Invalid,
                    Value = match.Value.Trim()
                };
            }

            throw new DslParserException("Failed to generate invalid token");
        }
    }
}
