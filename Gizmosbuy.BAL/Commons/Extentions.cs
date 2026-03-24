using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
    }
}
