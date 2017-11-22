using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UniRedux.Sample.ScriptableObject.Binary
{
    [CreateAssetMenu(fileName = "ToDoStore", menuName = "UniRedux/ByteToDoStore", order = 1)]
    public class ToDoScriptableStore : ScriptableStore<ToDoState, byte[]>
    {
#if UNITY_IPHONE
        private static bool IsFirst;
#endif
        private BinaryFormatter binaryFormatter = new BinaryFormatter();
        protected override byte[] Serialize(ToDoState state)
        {
#if UNITY_IPHONE
            InitSetEnvironmentVariable();
#endif
            byte[] result;
            var mem = new MemoryStream();
            try
            {
                binaryFormatter.Serialize(mem, state);
                mem.Position = 0;
                result = mem.ToArray();
            }
            finally
            {
                mem.Close();
            }
            return result;
        }

        protected override ToDoState Deserialize(byte[] serializeState)
        {
#if UNITY_IPHONE
            InitSetEnvironmentVariable();
#endif
            ToDoState result;
            var mem = new MemoryStream(serializeState);
            try
            {
                result = (ToDoState)binaryFormatter.Deserialize(mem);
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
        protected override Reducer<ToDoState> InitReducer => ToDoReducer.Execute;
        protected override Middleware<ToDoState>[] InitMiddlewares { get; } = { UniReduxMiddleware.Logger };
    }
}
