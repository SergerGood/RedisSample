using System.Text;

namespace Redis.Sample
{
    public static class Extensions
    {
        public static string GetString(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static byte[] GetBytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}