using ObjectBoundBindingList.DataRepresentation;
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
            if (_lookaheadFirst.TokenType == TokenType.Function)
            {
                _currentMatchCondition.Value1 = GetIValueFromDslToken();
                if (IsLogialOperator(_lookaheadFirst))
                    BooleanFunctionMatchCondition();
                if (IsEqualityOperator(_lookaheadFirst))
                {
                    EqualityFunctionMatchCondition();
                }
                if(_lookaheadFirst.TokenType != TokenType.SequenceTerminator)
                    MatchConditionNext();
                return;
            }
            if (IsObject(_lookaheadFirst))
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
            else
            {
                throw new DslParserException(ExpectedObjectErrorText + _lookaheadFirst.Value);
            }
        }

        private void EqualityFunctionMatchCondition()
        {
            IValue Value2;
            DslOperator Operator;

            Operator = GetOperator(_lookaheadFirst);
            DiscardToken();
            Value2 = GetIValueFromDslToken();
            _currentMatchCondition.Operator = Operator;
            _currentMatchCondition.Value2 = Value2;
        }

        private void BooleanFunctionMatchCondition()
        {
            var Operator = DslOperator.Equals;

            var Value2 = new FixedValue();
            (Value2 as FixedValue).Value = "true";
            _currentMatchCondition.Value2 = Value2;
            _currentMatchCondition.Operator = Operator;
        }

        private bool IsInverter(DslToken token)
        {
            return token.TokenType == TokenType.Invert;
        }

        private IValue GetIValueFromDslToken()
        {
            IValue returnValue = null;
            DslObject obj = GetObject(_lookaheadFirst);
            if(obj == DslObject.Variable)
            {
                returnValue = new VariableValue();
                (returnValue as VariableValue).VariableName = _lookaheadFirst.Value;
                DiscardToken();
            } else if(obj == DslObject.FixedValue)
            {
                returnValue = new FixedValue();
                (returnValue as FixedValue).Value = _lookaheadFirst.Value;
                DiscardToken();
            } else if(obj == DslObject.NULL)
            {
                returnValue = new NullValue();
                DiscardToken();
            } else if(obj == DslObject.Function)
            {
                returnValue = new FunctionCallValue();
                var x = returnValue as FunctionCallValue;
                x.FunctionName = _lookaheadFirst.Value.Remove(_lookaheadFirst.Value.Length - 1);
                DiscardToken();
                while(_lookaheadFirst.TokenType != TokenType.CloseParenthesis)
                {
                    x.Parameters.Add(GetIValueFromDslToken());
                    if(!(_lookaheadFirst.TokenType == TokenType.Comma || _lookaheadFirst.TokenType == TokenType.CloseParenthesis))
                    {
                        throw new ArgumentException(", or ) expected " + _lookaheadFirst.Value + " found");
                    }
                    if (_lookaheadFirst.TokenType == TokenType.Comma)
                        DiscardToken();
                }
                DiscardToken();
            }
            
            return returnValue;
        }

        private void EqualityMatchCondition()
        {
            IValue Value1;
            IValue Value2;
            DslOperator Operator;

            Value1 = GetIValueFromDslToken();

            Operator = GetOperator(_lookaheadFirst);
            DiscardToken();
            Value2 = GetIValueFromDslToken();
 

            _currentMatchCondition.Value1 = Value1;
            _currentMatchCondition.Operator = Operator;
            _currentMatchCondition.Value2 = Value2;
        }

        private DslObject GetObject(DslToken token)
        {
            switch (token.TokenType)
            {
                case TokenType.Variable:
                    return DslObject.Variable;
                case TokenType.Null:
                    return DslObject.NULL;
                case TokenType.Function:
                    return DslObject.Function;
                case TokenType.Number:
                case TokenType.StringValue:
                case TokenType.DateTimeValue:
                    return DslObject.FixedValue;
                default:
                    throw new DslParserException(ExpectedObjectErrorText + token.Value);
            }
        }

        private bool IsLogialOperator(DslToken token)
        {
            if (token.TokenType == TokenType.And || token.TokenType == TokenType.Or || token.TokenType == TokenType.SequenceTerminator)
                return true;
            return false;
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
                    throw new DslParserException("Expected =, !=, <>, >, >=, <, <=, LIKE, IN, IS, a Function Call but found: " + token.Value);
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

            IValue value1 = GetIValueFromDslToken();
            _currentMatchCondition.Value1 = value1;

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
            _currentMatchCondition.Value2 = new ListValue();

            (_currentMatchCondition.Value2 as ListValue).Values.Add(ReadToken(TokenType.StringValue).Value);
            DiscardToken(TokenType.StringValue);
            StringLiteralListNext();
        }

        private void StringLiteralListNext()
        {
            if (_lookaheadFirst.TokenType == TokenType.Comma)
            {
                DiscardToken(TokenType.Comma);
                (_currentMatchCondition.Value2 as ListValue).Values.Add(ReadToken(TokenType.StringValue).Value);
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
            _currentMatchCondition.NextLogOperator = DslLogicalOperator.And;
            DiscardToken(TokenType.And);
            MatchCondition();
        }

        private void OrMatchCondition()
        {
            _currentMatchCondition.NextLogOperator = DslLogicalOperator.Or;
            DiscardToken(TokenType.Or);
            MatchCondition();
        }

        private bool IsObject(DslToken token)
        {
            return token.TokenType == TokenType.Variable
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
