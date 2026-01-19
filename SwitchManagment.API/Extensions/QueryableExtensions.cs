using System.Linq.Expressions;
using System.Reflection;

namespace SwitchManagment.API.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool ascending = true)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Single(property => property.Name == propertyName);

            Type funcType = typeof(Func<,>).MakeGenericType(type, property.PropertyType);

            ParameterExpression parameter = Expression.Parameter(type);

            MemberExpression getPropExpression = Expression.PropertyOrField(parameter, property.Name);

            MethodInfo lambda = typeof(Expression).GetMethods().Single(methInf => methInf.Name == "Lambda" && methInf.GetGenericArguments().Length == 1 && methInf.GetParameters().Length == 2 &&
                methInf.GetParameters()[0].ParameterType.Name == typeof(Expression).Name && methInf.GetParameters()[1].ParameterType.Name == typeof(ParameterExpression[]).Name)
                .MakeGenericMethod(funcType);

            object keySelector = lambda.Invoke(null, [getPropExpression, new ParameterExpression[] { parameter }]);

            MethodInfo orderMethod = typeof(Queryable).GetMethods()
                .Single(methInf => methInf.Name == (ascending ? "OrderBy" : "OrderByDescending") && methInf.GetParameters().Length == 2)
                .MakeGenericMethod(type, property.PropertyType);
            
            return (IOrderedQueryable<T>) orderMethod.Invoke(null, [queryable, keySelector]);
        }
    }
}
