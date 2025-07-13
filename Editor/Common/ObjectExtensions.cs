namespace Overefactor.DI.Editor.Editor.Common
{
    internal static class ObjectExtensions
    {
        private static string ToBase62(uint value)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var result = "";
            
            do
            {
                result = chars[(int)(value % 62)] + result;
                value /= 62;
            } while (value > 0);

            return result;
        }
        
        
        
        public static string GetShortHashCode(this object obj) => ToBase62((uint)obj.GetHashCode());
    }
}