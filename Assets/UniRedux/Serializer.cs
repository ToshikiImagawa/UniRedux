using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace UniRedux
{
    public static class BinarySerializer
    {
#if UNITY_IPHONE
        private static bool IsFirst;
#endif

        public static byte[] Serialize(object target)
        {
#if UNITY_IPHONE
            InitSetEnvironmentVariable();
#endif
            byte[] result;
            var b = new BinaryFormatter();
            var mem = new MemoryStream();
            try
            {
                b.Serialize(mem, target);
                mem.Position = 0;
                result = mem.ToArray();
            }
            finally
            {
                mem.Close();
            }
            return result;
        }

        public static T Deserialize<T>(byte[] target)
        {
#if UNITY_IPHONE
            InitSetEnvironmentVariable();
#endif
            T result;
            var b = new BinaryFormatter();
            var mem = new MemoryStream(target);
            try
            {
                result = (T) b.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }
            return result;
        }

#if UNITY_IPHONE
        private static void InitSetEnvironmentVariable()
        {
            if (!IsFirst)
            {
                IsFirst = true;
                Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            }
        }
#endif
    }
}