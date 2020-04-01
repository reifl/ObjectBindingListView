﻿using ObjectBoundBindingList.DataRepresentation;
using ObjectBoundBindingList.Tokenizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.Parser
{
    public class DslParser
    {
        private Stack<DslToken> _tokenSequence;
        private DslToken _lookaheadFirst;
        private DslToken _lookaheadSecond;

        private DslQueryModel _queryModel;
        private MatchCondition _currentMatchCondition;

        private Stack<IList<MatchCondition>> matchCollections;
        private Stack<MatchCondition> conditions;

        private IList<MatchCondition> MatchConditions
        {
            get
            {
                return matchCollections.Peek();
            }
        }

        private const string ExpectedObjectErrorText = "Expected =, !=, LIKE, NOT LIKE, IN or NOT IN but found: ";

        public DslQueryModel Parse(List<DslToken> tokens)
        {
            matchCollections = new Stack<IList<MatchCondition>>();
            conditions = new Stack<MatchCondition>();
            LoadSequenceStack(tokens);
            PrepareLookaheads();
            _queryModel = new DslQueryModel();
            matchCollections.Push(_queryModel.MatchConditions);
            Match();

            DiscardToken(TokenType.SequenceTerminator);

            return _queryModel;
        }

        private void LoadSequenceStack(List<DslToken> tokens)
        {
            _tokenSequence = new Stack<DslToken>();
            int count = tokens.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                _tokenSequence.Push(tokens[i]);
            }
        }

        private void PrepareLookaheads()
        {
            _lookaheadFirst = _tokenSequence.Pop();
            _lookaheadSecond = _tokenSequence.Pop();
        }

        private DslToken ReadToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new DslParserException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            return _lookaheadFirst;
        }

        private void DiscardToken()
        {
            _lookaheadFirst = _lookaheadSecond.Clone();

            if (_tokenSequence.Any())
                _lookaheadSecond = _tokenSequence.Pop();
            else
                _lookaheadSecond = new DslToken(TokenType.SequenceTerminator, string.Empty);
        }

        private void DiscardToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new DslParserException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            DiscardToken();
        }

        private void Match()
        {
            //DiscardToken(TokenType.Match);
            MatchCondition();
        }

        private void MatchCondition()
        {
            CreateNewMatchCondition();
            if (IsInverter(_lookaheadFirst))
            {
                _currentMatchCondition.Not = true;
                DiscardToken(TokenType.Invert);
            }
            if(_lookaheadFirst.TokenType == TokenType.OpenParenthesis)
            {
                _currentMatchCondition.Operator = DslOperator.OpenParenthesis;
                matchCollections.Push(_currentMatchCondition.SubConditions);
                conditions.Push(_currentMatchCondition);
                DiscardToken(TokenType.OpenParenthesis);
                MatchCondition();
                return;
            } 
            else if (IsObject(_lookaheadFirst))
            {
                if (IsEqualityOperator(_lookaheadSecond))
                {
                    EqualityMatchCondition();
                }
                else if (_lookaheadSecond.TokenType == TokenType.In)
                {
                    InCondition();
                }
                else if (_lookaheadSecond.TokenType == TokenType.NotIn)
                {
                    NotInCondition();
                }
                else
                {
                    throw new DslParserException(ExpectedObjectErrorText + " " + _lookaheadSecond.Value);
                }

                MatchConditionNext();
            }
            else if(_lookaheadFirst.TokenType == TokenType.Function)
            {
            }
            else
            {
                throw new DslParserException(ExpectedObjectErrorText + _lookaheadFirst.Value);
            }
        }

        private bool IsInverter(DslToken token)
        {
            return token.TokenType == TokenType.Invert;
        }

        private void EqualityMatchCondition()
        {
            _currentMatchCondition.Object = GetObject(_lookaheadFirst);
            DiscardToken();
            _currentMatchCondition.Operator = GetOperator(_lookaheadFirst);
            DiscardToken();
            _currentMatchCondition.Value = _lookaheadFirst.Value;
            DiscardToken();
        }

        private DslObject GetObject(DslToken token)
        {
            switch (token.TokenType)
            {
                case TokenType.Application:
                    return DslObject.Application;
                case TokenType.ExceptionType:
                    return DslObject.ExceptionType;
                case TokenType.Fingerprint:
                    return DslObject.Fingerprint;
                case TokenType.Message:
                    return DslObject.Message;
                case TokenType.StackFrame:
                    return DslObject.StackFrame;
                case TokenType.Variable:
                    return DslObject.Variable;
                case TokenType.Number:
                case TokenType.StringValue:
                case TokenType.DateTimeValue:
                    return DslObject.FixedValue;
                default:
                    throw new DslParserException(ExpectedObjectErrorText + token.Value);
            }
        }

        private DslOperator GetOperator(DslToken token)
        {
            switch (token.TokenType)
            {
                case TokenType.Equals:
                    return DslOperator.Equals;
                case TokenType.NotEquals:
                    return DslOperator.NotEquals;
                case TokenType.Like:
                    return DslOperator.Like;
                case TokenType.NotLike:
                    return DslOperator.NotLike;
                case TokenType.In:
                    return DslOperator.In;
                case TokenType.NotIn:
                    return DslOperator.NotIn;
                case TokenType.GreaterThan:
                    return DslOperator.Greater;
                case TokenType.GreaterOrEqual:
                    return DslOperator.GreaterOrEqual;
                case TokenType.LowerThan:
                    return DslOperator.Lower;
                case TokenType.LowerOrEqual:
                    return DslOperator.LowerOrEqual;
                case TokenType.Is:
                    return DslOperator.Is;
                case TokenType.Function:
                    return DslOperator.Function;
                default:
                    throw new DslParserException("Expected =, !=, LIKE, NOT LIKE, IN, NOT IN but found: " + token.Value);
            }
        }

        private void NotInCondition()
        {
            ParseInCondition(DslOperator.NotIn);
        }

        private void InCondition()
        {
            ParseInCondition(DslOperator.In);
        }

        private void ParseInCondition(DslOperator inOperator)
        {
            _currentMatchCondition.Operator = inOperator;
            _currentMatchCondition.Values = new List<string>();
            _currentMatchCondition.Object = GetObject(_lookaheadFirst);
            DiscardToken();

            if (inOperator == DslOperator.In)
                DiscardToken(TokenType.In);
            else if (inOperator == DslOperator.NotIn)
                DiscardToken(TokenType.NotIn);

            DiscardToken(TokenType.OpenParenthesis);
            StringLiteralList();
            DiscardToken(TokenType.CloseParenthesis);
        }

        private void StringLiteralList()
        {
            _currentMatchCondition.Values.Add(ReadToken(TokenType.StringValue).Value);
            DiscardToken(TokenType.StringValue);
            StringLiteralListNext();
        }

        private void StringLiteralListNext()
        {
            if (_lookaheadFirst.TokenType == TokenType.Comma)
            {
                DiscardToken(TokenType.Comma);
                _currentMatchCondition.Values.Add(ReadToken(TokenType.StringValue).Value);
                DiscardToken(TokenType.StringValue);
                StringLiteralListNext();
            }
            else
            {
                // nothing
            }
        }

        private void MatchConditionNext()
        {
            if (_lookaheadFirst.TokenType == TokenType.And)
            {
                AndMatchCondition();
            }
            else if (_lookaheadFirst.TokenType == TokenType.Or)
            {
                OrMatchCondition();
            }
            else if (_lookaheadFirst.TokenType == TokenType.Between)
            {
                DateCondition();
            }
            else if(_lookaheadFirst.TokenType == TokenType.CloseParenthesis)
            {
                DiscardToken(TokenType.CloseParenthesis);
                matchCollections.Pop();
                _currentMatchCondition = conditions.Pop();
                MatchConditionNext();
            }
            else if (_lookaheadFirst.TokenType == TokenType.SequenceTerminator)
            {
                return;
            }
            else
            {
                throw new DslParserException("Expected AND, OR or BETWEEN but found: " + _lookaheadFirst.Value);
            }
        }

        private void AndMatchCondition()
        {
            _currentMatchCondition.LogOpToNextCondition = DslLogicalOperator.And;
            DiscardToken(TokenType.And);
            MatchCondition();
        }

        private void OrMatchCondition()
        {
            _currentMatchCondition.LogOpToNextCondition = DslLogicalOperator.Or;
            DiscardToken(TokenType.Or);
            MatchCondition();
        }

        private void DateCondition()
        {
            DiscardToken(TokenType.Between);

            _queryModel.DateRange = new DateRange();
            _queryModel.DateRange.From = DateTime.ParseExact(ReadToken(TokenType.DateTimeValue).Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DiscardToken(TokenType.DateTimeValue);
            DiscardToken(TokenType.And);
            _queryModel.DateRange.To = DateTime.ParseExact(ReadToken(TokenType.DateTimeValue).Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DiscardToken(TokenType.DateTimeValue);
            DateConditionNext();
        }

        private void DateConditionNext()
        {
            if (_lookaheadFirst.TokenType == TokenType.Limit)
            {
                Limit();
            }
            else if (_lookaheadFirst.TokenType == TokenType.SequenceTerminator)
            {
                // nothing
            }
            else
            {
                throw new DslParserException("Expected LIMIT or the end of the query but found: " + _lookaheadFirst.Value);
            }

        }

        private void Limit()
        {
            DiscardToken(TokenType.Limit);
            int limit = 0;
            bool success = int.TryParse(ReadToken(TokenType.Number).Value, out limit);
            if (success)
                _queryModel.Limit = limit;
            else
                throw new DslParserException("Expected an integer number but found " + ReadToken(TokenType.Number).Value);

            DiscardToken(TokenType.Number);
        }

        private bool IsObject(DslToken token)
        {
            if (token.TokenType == TokenType.Variable)
                _currentMatchCondition.VariableName = token.Value;
            if (token.TokenType == TokenType.Number
                   || token.TokenType == TokenType.StringValue
                   || token.TokenType == TokenType.DateTimeValue)
            {
                _currentMatchCondition.LeftSideValue = token.Value;
            }
            return token.TokenType == TokenType.Application
                   || token.TokenType == TokenType.ExceptionType
                   || token.TokenType == TokenType.Fingerprint
                   || token.TokenType == TokenType.Message
                   || token.TokenType == TokenType.StackFrame
                   || token.TokenType == TokenType.Variable
                   || token.TokenType == TokenType.Number
                   || token.TokenType == TokenType.StringValue
                   || token.TokenType == TokenType.DateTimeValue;
        }

        private bool IsEqualityOperator(DslToken token)
        {
            return token.TokenType == TokenType.Equals
                   || token.TokenType == TokenType.NotEquals
                   || token.TokenType == TokenType.Like
                   || token.TokenType == TokenType.NotLike
                   || token.TokenType == TokenType.LowerOrEqual
                   || token.TokenType == TokenType.LowerThan
                   || token.TokenType == TokenType.GreaterOrEqual
                   || token.TokenType == TokenType.GreaterThan
                   || token.TokenType == TokenType.Is;
        }

        private void CreateNewMatchCondition()
        {
            _currentMatchCondition = new MatchCondition();
            MatchConditions.Add(_currentMatchCondition);
        }
    }
}
