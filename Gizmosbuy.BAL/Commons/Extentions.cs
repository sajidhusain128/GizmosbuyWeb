using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Gizmosbuy.BAL.Commons
{
    public static class Extentions
    {
        public static async Task<(List<T1>, List<T2>)> ExecuteStoredProc<T1, T2>(this DbContext context, string storedProcName,
                                                                    List<SqlParameter> sqlParameterList,
                                                                    Func<DbDataReader, T1> mapFirst,
                                                                    Func<DbDataReader, T2> mapSecond)
        {
            var result1 = new List<T1>();
            var result2 = new List<T2>();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(sqlParameterList);

                context.Database.OpenConnection();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        result1.Add(mapFirst(reader));

                    reader.NextResult();

                    while (reader.Read())
                        result2.Add(mapSecond(reader));
                }
            }

            return (result1, result2);
        }

        public static IEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> source, string orderByMember, string sortDirection)
        {
            var property = typeof(T).GetProperty(orderByMember);
            if (property == null) return source;

            return sortDirection == "asc"
                ? source.OrderBy(x => property.GetValue(x, null))
                : source.OrderByDescending(x => property.GetValue(x, null));

        }

        public static IQueryable<T> SearchAllProperties<T>(this IQueryable<T> query, string searchTerm, string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return query;

            // Force the search term to lowercase
            string lowerSearchTerm = searchTerm.ToLower();

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression orExpression = null;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                Expression propAccess = Expression.Property(parameter, prop);

                Expression containsExpression = null;

                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    if (DateTime.TryParseExact(searchTerm, dateFormat,
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        // Compare only the Date part
                        var dateConst = Expression.Constant(parsedDate.Date, typeof(DateTime));
                        var dateProp = Expression.Property(propAccess, "Date");
                        containsExpression = Expression.Equal(dateProp, dateConst);
                    }
                }
                else
                {
                    // Default string Contains
                    var toStringMethod = typeof(object).GetMethod("ToString");
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    Expression toStringExpr = Expression.Call(propAccess, toStringMethod);
                    Expression toLowerExpr = Expression.Call(toStringExpr, toLowerMethod);
                    var searchExpression = Expression.Constant(searchTerm.ToLower());
                    containsExpression = Expression.Call(toLowerExpr, containsMethod, searchExpression);
                }

                if (containsExpression != null)
                {
                    orExpression = orExpression == null
                        ? containsExpression
                        : Expression.OrElse(orExpression, containsExpression);
                }
            }

            if (orExpression == null) return query;

            // 6. Build the final lambda: x => x.Prop1.ToLower().Contains(...) || x.Prop2.ToLower().Contains(...)
            var lambda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);

            return query.Where(lambda);
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (var item in source)
            {
                yield return (item, index);
                index++;
            }
        }
    }
}
