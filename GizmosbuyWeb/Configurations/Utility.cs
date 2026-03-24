using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace GizmosbuyWeb.Configurations
{
    public class Utility
    {
        public static string ExtractText(string html)
        {
            try
            {
                string s = "";
                Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                s = reg.Replace(html, " ");
                s = HttpUtility.HtmlDecode(s);
                return s;
            }
            catch (Exception)
            {
                return "";
            }

        }

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
    }
}
