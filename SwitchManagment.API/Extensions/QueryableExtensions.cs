using System.Linq.Expressions;
using System.Reflection;

namespace SwitchManagment.API.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> queryable, string propertyName, bool ascending = true)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Single(property => property.Name == propertyName);

            Type funcType = typeof(Func<,>).MakeGenericType(type, property.PropertyType);

            ParameterExpression parameter = Expression.Parameter(type);

            MemberExpression getPropExpression = Expression.PropertyOrField(parameter, property.Name);

            Expression<Func<T, TKey>> keySelector = Expression.Lambda<Func<T, TKey>>(getPropExpression, parameter);

            //var t = typeof(Queryable);
            //new Binder().

            Type keySelectorType = typeof(Expression<>).MakeGenericType(funcType);


            MethodInfo orderMethod = typeof(Queryable).GetMethods()
                .Single(methInf => methInf.Name == (ascending ? "OrderBy" : "OrderByDescending") && methInf.GetParameters().Length == 2)
                .MakeGenericMethod(type, property.PropertyType);
            


            //MethodInfo orderMethod = typeof(Queryable).GetMethod(ascending ? "OrderBy" : "OrderByDescending", 2, [typeof(IQueryable<>), typeof(Expression<>).MakeGenericType(typeof(Func<,>))]);


            return ascending ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
        }
    }
}
