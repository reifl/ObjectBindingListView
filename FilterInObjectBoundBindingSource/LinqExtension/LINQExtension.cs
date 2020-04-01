using ObjectBoundBindingList.DataRepresentation;
using ObjectBoundBindingList.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.LinqExtension
{
    public static class LINQExtension
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string filter)
        {
            var tokenizer = new Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize(filter);

            var dslParser = new DslParser();
            var dsl = dslParser.Parse(tokens);

            return source.WhereClause<T>(dsl.MatchConditions);
        }


        private static object GetStringAsObject(string ObjectValue, Type t)
        {
            if(ObjectValue.Substring(0, 1) == "'")
            {
                ObjectValue = ObjectValue.Substring(1);
                ObjectValue = ObjectValue.Remove(ObjectValue.Length - 1);
            }
            if (t == typeof(int))
                return int.Parse(ObjectValue);
            else if (t == typeof(float))
                return float.Parse(ObjectValue, CultureInfo.InvariantCulture);
            else if (t == typeof(string)) 
                return ObjectValue;
            else if (t == typeof(double))
                return double.Parse(ObjectValue, CultureInfo.InvariantCulture);
            else if (t == typeof(bool))
                return bool.Parse(ObjectValue);
            else if (t == typeof(decimal))
                return decimal.Parse(ObjectValue, CultureInfo.InvariantCulture);
            else if (t == typeof(DateTime))
                return DateTime.Parse(ObjectValue);
            else
            {
                //TODO: Custom GetStringAsObject via Func<string, Type, object>
                throw new ArgumentException("Don't know this Type " + t.ToString());
            }
        }

        public static Expression<Func<T, bool>> BuildExpression<T>(this IEnumerable<T> source, IList<MatchCondition> conditions, ParameterExpression pe = null)
        {

            Expression<Func<T, bool>> returnCondition = null;
            Expression cond1 = null;
            Expression currentCond = null;
            DslLogicalOperator currentOperator = DslLogicalOperator.NotDefined;
            if (pe == null)
                pe = Expression.Parameter(typeof(T), "s");



            foreach (var x in conditions)
            {
                switch (x.Operator)
                {
                    case DslOperator.Like:
                        currentCond = Expression.Call(typeof(LINQExtension).GetMethod("RegexMatch", new Type[] { typeof(string), typeof(string) }), new Expression[] {
                            Expression.Property(pe, x.VariableName),
                            Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T))))
                        });
                        break;
                    case DslOperator.Is:
                        if (x.Value.ToLower() != "null")
                            throw new ArgumentException("IS Operator only Supports NULL");
                        currentCond = Expression.Equal(Expression.Property(pe, x.VariableName), Expression.Constant(null));
                        break;
                    case DslOperator.NotEquals:
                        currentCond = Expression.NotEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Equals:
                        currentCond = Expression.Equal(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Greater:
                        currentCond = Expression.GreaterThan(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.GreaterOrEqual:
                        currentCond = Expression.GreaterThanOrEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Lower:
                        currentCond = Expression.LessThan(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.LowerOrEqual:
                        currentCond = Expression.LessThanOrEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.OpenParenthesis:
                        currentCond = BuildExpression(source, x.SubConditions, pe).Body;
                        break;
                }
                if (x.Not)
                {
                    currentCond = Expression.Not(currentCond);
                }
                if (currentOperator == DslLogicalOperator.And)
                {
                    currentCond = Expression.AndAlso(cond1, currentCond);
                }
                else if (currentOperator == DslLogicalOperator.Or)
                {
                    currentCond = Expression.OrElse(cond1, currentCond);
                }

                cond1 = currentCond;
                currentOperator = x.LogOpToNextCondition;
            }



            returnCondition = Expression.Lambda<Func<T, bool>>(currentCond, new[] { pe });
            return returnCondition;
        }

        public static bool RegexMatch(string m1, string match)
        {
            var m2 = match.Substring(1);

            m2 = m2.Remove(m2.Length - 1);
            m2 = match;
            if (m2.Substring(0, 1) == "%" && m2.Substring(m2.Length - 1) == "%")
            {
                m2 = m2.Substring(1);
                m2 = m2.Remove(m2.Length - 1);
                return m1.Contains(m2);
            }
            else if (m2.Substring(0, 1) == "%")
            {
                m2 = m2.Substring(1);
                return m1.EndsWith(m2);
            }
            else if (m2.Substring(m2.Length - 1) == "%")
            {
                m2 = m2.Remove(m2.Length - 1);
                return m1.StartsWith(m2);
            }
            return m1 == m2;
        }

        private static string BuildCSharpCode<T>(this IEnumerable<T> source, IList<MatchCondition> conditions)
        {

            string csharpCode = "";
            foreach (var x in conditions)
            {
                if (x.Not)
                    csharpCode += "!(";
                if (x.Object == DslObject.Variable)
                {
                    csharpCode += "x." + x.VariableName;
                }
                else if (x.Object == DslObject.FixedValue)
                {
                    var compareValue = x.LeftSideValue.ToString();
                    if (compareValue.StartsWith("'"))
                    {
                        compareValue = compareValue.Substring(1, compareValue.Length - 2);
                        compareValue = compareValue.Replace("\\'", "'");
                    }
                    csharpCode += compareValue;
                }
                switch (x.Operator)
                {
                    case DslOperator.Equals:
                        csharpCode += "==";
                        break;
                    case DslOperator.Greater:
                        csharpCode += ">";
                        break;
                    case DslOperator.GreaterOrEqual:
                        csharpCode += ">=";
                        break;
                    case DslOperator.Lower:
                        csharpCode += "<";
                        break;
                    case DslOperator.LowerOrEqual:
                        csharpCode += "<=";
                        break;
                    case DslOperator.OpenParenthesis:
                        csharpCode += "(" + BuildCSharpCode<T>(source, x.SubConditions) + ")";
                        break;
                }

                csharpCode += x.Value;

                if (x.Not)
                    csharpCode += ")";
                if (x.LogOpToNextCondition == DslLogicalOperator.And)
                    csharpCode += " && ";
                if (x.LogOpToNextCondition == DslLogicalOperator.Or)
                    csharpCode += " || ";
            }
            return csharpCode;
        }

        private static IEnumerable<T> WhereClause<T>(this IEnumerable<T> source, IList<MatchCondition> MatchConditions)
        {
            var expr = source.BuildExpression(MatchConditions);
            return source.Where(expr.Compile());
        }

        static object GetFixedValue(MatchCondition match, object t)
        {
            return match.LeftSideValue;
        }

        static object GetVariableValue(MatchCondition match, object t)
        {
            return GetPropertyValue(match.VariableName, t);
        }

        private static Type GetPropertType(string value, Type t)
        {
            var type = t;
            var properties = type.GetProperties();
            var prop = properties.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (prop == null)
                throw new ArgumentOutOfRangeException("Cannot find " + value + " Property.");
            return prop.PropertyType;
        }

        private static object GetPropertyValue(string value, object t)
        {
            var type = t.GetType();

            var properties = type.GetProperties();
            var prop = properties.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (prop == null)
                throw new ArgumentOutOfRangeException("Cannot find " + value + " Property.");
            return prop.GetValue(t);
        }
    }
}
