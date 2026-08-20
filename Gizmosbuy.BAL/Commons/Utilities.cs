using Microsoft.AspNetCore.Http;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Gizmosbuy.BAL.Commons
{
    public class Utilities
    {
        public static Func<T, bool> GetSearchValue<T>(string value, string dateFormat)
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

                            if (obj2 != null)
                            {
                                string strValue;

                                // Special handling for DateTime
                                if (obj2 is DateTime dt)
                                {
                                    // Format as dd-MM-yyyy (or any format you need)
                                    strValue = dt.ToString(dateFormat, CultureInfo.InvariantCulture);
                                }
                                else if (obj2 is DateTime?)
                                {
                                    var dtNullable = (DateTime?)obj2;
                                    strValue = dtNullable.HasValue
                                        ? dtNullable.Value.ToString(dateFormat, CultureInfo.InvariantCulture)
                                        : string.Empty;
                                }
                                else
                                {
                                    strValue = Convert.ToString(obj2) ?? string.Empty;
                                }

                                if (strValue.ToLower().Contains(value.ToLower()))
                                {
                                    return true;
                                }
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

        public static string GenerateStoreTransferBillNo(string preFixValue, string value, string postFixValue)
        {
            try
            {
                string output = "";
                string prefix = string.IsNullOrWhiteSpace(preFixValue) ? "" : preFixValue;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return prefix + "0001" + postFixValue;
                }
                else
                {
                    string stringNumber = value.Replace(prefix, "").Replace(postFixValue, "");
                    int num = Convert.ToInt32(stringNumber);

                    string str = new string('0', stringNumber.Length);
                    output = num > 0 ? (++num).ToString(str) : (++num).ToString();
                }

                output = prefix + output + postFixValue;

                return output;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static DataTable CreateDataTable<T>(IList<T> item, string tableName = "")
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable(tableName);
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in item)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static string ConvertToIndianCurrencyWords(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);

            string rupeeWords = NumberToWords(rupees) + " Rupees";
            string paiseWords = paise > 0 ? " and " + NumberToWords(paise) + " Paise" : "";

            return rupeeWords + paiseWords + " Only";
        }

        internal static string NumberToWords(long number)
        {
            if (number == 0)
                return "Zero";

            string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen",
                       "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] thousands = { "", "Thousand", "Lakh", "Crore" };

            List<string> parts = new List<string>();

            if (number < 0)
            {
                parts.Add("Minus");
                number = -number;
            }

            long[] numParts = new long[4];
            numParts[0] = number % 1000; // units
            numParts[1] = (number / 1000) % 100; // thousands
            numParts[2] = (number / 100000) % 100; // lakhs
            numParts[3] = (number / 10000000); // crores

            for (int i = 3; i >= 0; i--)
            {
                if (numParts[i] == 0) continue;

                int h = (int)(numParts[i] / 100);
                int t = (int)(numParts[i] % 100);
                int u = t % 10;

                if (h > 0)
                    parts.Add(units[h] + " Hundred");

                if (t > 0)
                {
                    if (h > 0) parts.Add("and");

                    if (t < 10)
                        parts.Add(units[t]);
                    else if (t < 20)
                        parts.Add(teens[t - 10]);
                    else
                        parts.Add(tens[t / 10] + (u > 0 ? " " + units[u] : ""));
                }

                if (i > 0)
                    parts.Add(thousands[i]);
            }

            return string.Join(" ", parts).Trim();
        }

        public static string GetLast10Characters(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 10)
            {
                return input;
            }
            // Starts 10 characters from the end and continues to the end of the string.
            return input.Substring(input.Length - 10);
        }

        public static Dictionary<string, int> GetMonthList()
        {
            try
            {
                List<string> stringValues = new List<string> { "January", "February", "March", "April", "May", "Jun", "July", "August", "September", "October", "November", "December" };

                // Convert to Dictionary (Key: Month Name, Value: Month Number)
                Dictionary<string, int> monthDictionary = stringValues
                    .Select((month, index) => new { Name = month, Number = index + 1 })
                    .ToDictionary(m => m.Name, m => m.Number);

                return monthDictionary;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
