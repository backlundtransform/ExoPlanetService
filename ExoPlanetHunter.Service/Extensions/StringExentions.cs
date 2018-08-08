

namespace System
{
    public static class StringExtensions
    {
        public static T ToEnum<T>(this string s) 
        {
            
 
            try
            {
                return (T)Enum.Parse(typeof(T), s.Replace("-","_").Replace(" ",string.Empty));
            }
            catch
            {
             return (T)Enum.Parse(typeof(T), "Nodata");
             }
        }
    }
}