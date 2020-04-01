using ObjectBindingListView.DataRepresentation;
using ObjectBindingListView.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectBindingListView
{
    public static class LinqExtension
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string filter)
        {

            var tokenizer = new Parsing.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize(filter);

            var dslParser = new Parsing.Parser();
            var dsl = dslParser.Parse(tokens.ToList());

            return source.WhereClause<T>(dsl.MatchConditions);
        }

        public static IDictionary<string, MethodInfo> MethodInfos
        {
            get
            {
                if (methodInfos == null)
                    initMethodInfos();
                return methodInfos;
            }
        }

        private static void initMethodInfos()
        {
            if (methodInfos == null)
            {

                methodInfos = new Dictionary<string, MethodInfo>();
                methodInfos.Add("ISNULL".ToLower(), typeof(InternalFunctions).GetMethod("IsNull"));
                methodInfos.Add("SUBSTRING".ToLower(), typeof(InternalFunctions).GetMethod("substring"));
                methodInfos.Add("TRIM".ToLower(), typeof(InternalFunctions).GetMethod("Trim"));
            }
        }

        private static object GetStringAsObject(string ObjectValue, Type t)
        {
            if (ObjectValue.Substring(0, 1) == "'")
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

        private static Expression GetExpressionFromIValue(Expression instance, IValue value, Type destTyp)
        {
            if (value is FixedValue fixedValue)
            {
                string val1 = fixedValue.Value;
                if (fixedValue.Value.StartsWith("'") && fixedValue.Value.EndsWith("'"))
                {
                    val1 = val1.Substring(1);
                    val1 = val1.Remove(val1.Length - 1);
                }
                return Expression.Constant(GetStringAsObject(val1, destTyp));
            }
            else if (value is VariableValue variableValue)
            {
                return Expression.PropertyOrField(instance, variableValue.VariableName);
            }
            else if (value is NullValue)
            {
                return Expression.Constant(null);
            }
            else if (value is FunctionCallValue fCall)
            {
                var expList = new List<Expression>();
                var mInfo = MethodInfos[fCall.FunctionName.ToLower()];
                var parameters = mInfo.GetParameters();
                int i = 0;
                foreach (var x in fCall.Parameters)
                {
                    expList.Add(GetExpressionFromIValue(instance, x, parameters[i].ParameterType));
                    i++;
                }
                return Expression.Call(mInfo, expList);
            }

            return null;
        }

        private static IDictionary<string, MethodInfo> methodInfos;



        private static Type GetTargetType(MatchCondition cond, Type baseType)
        {
            if (cond.Value1 is VariableValue || cond.Value2 is VariableValue)
            {
                if (cond.Value1 is VariableValue parameterGiven)
                    return GetPropertType(parameterGiven.VariableName, baseType);
                if (cond.Value2 is VariableValue parameterGiven2)
                    return GetPropertType(parameterGiven2.VariableName, baseType);
            }
            if (cond.Value1 is FixedValue || cond.Value2 is FixedValue)
            {
                FixedValue v1 = cond.Value1 as FixedValue;
                if (v1 == null)
                    v1 = cond.Value2 as FixedValue;
                if (v1.Value.StartsWith("'") && v1.Value.EndsWith("'"))
                    return typeof(string);
                if (v1.Value.ToLower() == "true" || v1.Value.ToLower() == "false")
                    return typeof(bool);
            }

            return typeof(string);
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
                        currentCond = Expression.Call(typeof(LinqExtension).GetMethod("RegexMatch", new Type[] { typeof(string), typeof(string) }), new Expression[] {
                            GetExpressionFromIValue(pe, x.Value1, typeof(string)),
                            GetExpressionFromIValue(pe, x.Value2, typeof(string))
                        });
                        break;
                    case DslOperator.Is:
                        if (!(x.Value1 is VariableValue))
                            throw new ArgumentException("Left Side Value of IS must be a Variable");
                        if ((x.Value2 as FixedValue).Value.ToLower() != "null")
                            throw new ArgumentException("IS Operator only Supports NULL");
                        currentCond = Expression.Equal(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), Expression.Constant(null));
                        break;
                    case DslOperator.NotEquals:
                        currentCond = Expression.NotEqual(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.NotEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Equals:
                        currentCond = Expression.Equal(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.Equal(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Greater:
                        currentCond = Expression.GreaterThan(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.GreaterThan(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.GreaterOrEqual:
                        currentCond = Expression.GreaterThanOrEqual(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.GreaterThanOrEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.Lower:
                        currentCond = Expression.LessThan(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.LessThan(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
                        break;
                    case DslOperator.LowerOrEqual:
                        currentCond = Expression.LessThanOrEqual(GetExpressionFromIValue(pe, x.Value1, GetTargetType(x, typeof(T))), GetExpressionFromIValue(pe, x.Value2, GetTargetType(x, typeof(T))));
                        //currentCond = Expression.LessThanOrEqual(Expression.Property(pe, x.VariableName), Expression.Constant(GetStringAsObject(x.Value, GetPropertType(x.VariableName, typeof(T)))));
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
                currentOperator = x.NextLogOperator;
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


        private static IEnumerable<T> WhereClause<T>(this IEnumerable<T> source, IList<MatchCondition> MatchConditions)
        {
            var expr = source.BuildExpression(MatchConditions);
            return source.Where(expr.Compile());
        }

        private static Type GetPropertType(string value, Type t)
        {
            var type = t;
            var properties = type.GetProperties();
            var prop = properties.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (prop != null)
                return prop.PropertyType;

            var fields = type.GetFields();
            var field = fields.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (field != null)
                return field.FieldType;

            throw new ArgumentOutOfRangeException("Cannot find " + value + " Property or Field.");

        }
    }
}
