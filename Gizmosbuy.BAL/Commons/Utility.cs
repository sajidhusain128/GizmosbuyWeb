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
                httpContext.Session.TryGetValue(key, out byte[] values);
                string stringData = Encoding.UTF8.GetString(values);

                return stringData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetPrefixByLocation(string Location)
        {
            try
            {
                switch (Location)
                {
                    case "Jogeshwari":
                        return "JGS";

                    case "Nalasopara":
                        return "NSP";

                    case "Chhapi":
                        return "CHP";

                    case "Ahmedabad":
                        return "AMD";

                    default:
                        return "JGS";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GenerateBillNo(string prefixValue, string value)
        {
            try
            {
                string output = "";
                string prefix = string.IsNullOrWhiteSpace(prefixValue) ? "" : prefixValue;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return prefix + "0001";
                }
                else
                {
                    string stringNumber = value.Replace(prefix, "");
                    int num = Convert.ToInt32(stringNumber);

                    switch (stringNumber.Length)
                    {
                        case 4:
                            output = (++num).ToString("0000");
                            break;
                        case 5:
                            output = (++num).ToString("00000");
                            break;
                        default:
                            output = (++num).ToString();
                            break;
                    }
                }

                output = prefix + output;

                return output;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
