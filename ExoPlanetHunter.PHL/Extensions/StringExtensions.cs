namespace System.ComponentModel
{
    public static class StringExtensions
    {
        public static bool? ConvertToBoolToNullable(this string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            int val = 0;
            return (int.TryParse(value, out val) && val == 0);
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
            catch (Exception e)
            {
                return null;
            }
            return result;
        }
    }
}