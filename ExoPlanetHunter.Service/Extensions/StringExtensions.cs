namespace System.ComponentModel
{
    public static class StringExtensions
    {
        public static T? ToNullable<T>(this string s) where T : struct
        {
            T? result = new T?();

            if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
            {
                TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                result = (T)conv.ConvertFrom(s);
            }

            return result;
        }
    }
}