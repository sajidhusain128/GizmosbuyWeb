using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Gizmosbuy.BAL.Commons
{
    public class Utility
    {
        public static Func<T, bool> GetSearchValue<T>(string value)
        {
            Type temp = typeof(T);

            return (T obj) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        try
                        {
                            object? obj2 = pro.GetValue(obj);

                            if (Convert.ToString(obj2).ToLower().Contains(value.ToLower()))
                            {
                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    return false;
                }
                else
                    return true;
            };
        }


        public static string GetSessionValue(string key, HttpContext httpContext)
        {
            try
            {
                httpContext.Session.TryGetValue("UserName", out byte[] values);
                string stringData = Encoding.UTF8.GetString(values);

                return stringData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
