using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace SwitchManagment.API.Extensions
{
    public static class QueryableExtensions
    {

        private readonly static MethodInfo _expressionLambdaGeneric;

        private readonly static MethodInfo _orderByGeneric;

        private readonly static MethodInfo _orderByDescendingGeneric;

        static QueryableExtensions()
        {
            _expressionLambdaGeneric = typeof(Expression).GetMethods().Single(methInf => methInf.Name == "Lambda" && methInf.GetGenericArguments().Length == 1 && methInf.GetParameters().Length == 2 &&
                methInf.GetParameters()[0].ParameterType.Name == typeof(Expression).Name && methInf.GetParameters()[1].ParameterType.Name == typeof(ParameterExpression[]).Name);

            _orderByGeneric = typeof(Queryable).GetMethods()
                .Single(methInf => methInf.Name == "OrderBy" && methInf.GetParameters().Length == 2);

            _orderByDescendingGeneric = typeof(Queryable).GetMethods()
                .Single(methInf => methInf.Name == "OrderByDescending" && methInf.GetParameters().Length == 2);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyOrFieldName, bool ascending = true)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyOrFieldName);

            Type type = typeof(T);

            ParameterExpression parameter = Expression.Parameter(type);

            (Expression getFieldOrPropertyExpression, Type fieldOrPropertyType) = GetFieldOrPropertyExpression(type, parameter, propertyOrFieldName);

            Type funcType = typeof(Func<,>).MakeGenericType(type, fieldOrPropertyType);

            MethodInfo lambda = _expressionLambdaGeneric.MakeGenericMethod(funcType);
            MethodInfo orderMethod = (ascending ? _orderByGeneric : _orderByDescendingGeneric).MakeGenericMethod(type, fieldOrPropertyType);

            object keySelector = lambda.Invoke(null, [getFieldOrPropertyExpression, new ParameterExpression[] { parameter }]);

            return (IOrderedQueryable<T>) orderMethod.Invoke(null, [queryable, keySelector]);
        }


        private static (Expression memberExpression, Type propertyOrFieldType) GetFieldOrPropertyExpression(Type type, Expression expression, string propertyOrFieldName)
        {
            string[] propsName = propertyOrFieldName.Split('.');

            Type lastType = type;

            Expression lastExpression = expression;

            foreach (string prop in propsName)
            {
                MemberInfo memberInfo = type.GetMember(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField).SingleOrDefault() ??
                        type.GetMember(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).SingleOrDefault() ??
                        throw new ArgumentException("Not exitst property or field.", nameof(propertyOrFieldName));


                lastExpression = Expression.MakeMemberAccess(lastExpression, memberInfo);

                lastType = memberInfo.MemberType switch 
                { 
                    MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType, 
                    MemberTypes.Field => ((FieldInfo)memberInfo).FieldType, 
                    _ => throw new Exception("If this execute, you algoritm bruhen..") 
                };
            }

            return (lastExpression, lastType);
        }



        /*
        public static IQueryable<T> Like<T>(this IQueryable<T> queryable, Dictionary<string, string> propertyAndPatterns)
        {

            

            EF.Functions.Like()

            return queryable;
        }
        */
    }
}
