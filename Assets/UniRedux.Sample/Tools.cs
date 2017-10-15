using System.Linq;

namespace UniRedux.Sample
{
    public static class Util
    {
        public static T[] Empty<T>()
        {
            return (T[])Enumerable.Empty<T>();
        }
    }
}