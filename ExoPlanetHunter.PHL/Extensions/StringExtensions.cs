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
            return (int.TryParse(value, out val) && val == 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int? ConvertYearToIntToNullable(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) {
                    return null;
                }
                return Convert.ToInt32(value.Split(",")[0])

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
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));

                 

                    result = (T)conv.ConvertFrom(s.Replace(".", ","));
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }
    }
}