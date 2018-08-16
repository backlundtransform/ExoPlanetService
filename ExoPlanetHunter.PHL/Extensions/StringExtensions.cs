
using System.Globalization;
namespace System.ComponentModel
{
    public static class StringExtensions
    {
        public static bool? ConvertToBoolToNullable(this string value)
        {
            try
            {

                if (string.IsNullOrEmpty(value)) {
                return null;
            }
            int val = 0;
            return (int.TryParse(value, out val) && val == 1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int? ConvertYearIntToNullable(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Trim())) {
                    return null;
                }
                return Convert.ToInt32(value.Split(".")[0]);

            }
            catch (Exception)
            {
                return null;
            }
        }



        public static T? ToNullable<T>(this string s) where T : struct
        {
            T? result = new T?();
            try
            {
                if (string.IsNullOrEmpty(s) || s.Trim().Length == 0)
                {
                 
                 return result;
                  
                }

                return (T) Convert.ChangeType(s, typeof (T),CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return result;
            }
           
        }
    }
}