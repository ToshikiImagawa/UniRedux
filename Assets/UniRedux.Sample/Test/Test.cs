using UnityEngine;

namespace UniRedux.Sample.Test
{
    public class Test : MonoBehaviour
    {
        public ScriptableObject.Json.ToDoScriptableStore JsonScriptableStore;
        public ScriptableObject.Binary.ToDoScriptableStore BinaryScriptableStore;

        public int m_element_num = 1000;

        public float m_noSerializer_dispatch_time = 0.0f;
        public float m_jsonSerializer_dispatch_time = 0.0f;
        public float m_binarySerializer_dispatch_time = 0.0f;

        public void OnClick()
        {
            var addToDoAction = new AddToDoAction
            {
                Text = "TestTestTestTestTestTestTestTestTestTestTestTestTestTest"
            };

            m_noSerializer_dispatch_time = Time.realtimeSinceStartup;
            for (int i = 0; i < m_element_num; i++)
            {
                Application.ToDoApplication.CurrentStore.Dispatch(addToDoAction);
            }
            m_noSerializer_dispatch_time = Time.realtimeSinceStartup - m_noSerializer_dispatch_time;

            m_jsonSerializer_dispatch_time = Time.realtimeSinceStartup;
            for (int i = 0; i < m_element_num; i++)
            {
                JsonScriptableStore.Dispatch(addToDoAction);
            }
            m_jsonSerializer_dispatch_time = Time.realtimeSinceStartup - m_jsonSerializer_dispatch_time;

            m_binarySerializer_dispatch_time = Time.realtimeSinceStartup;
            for (int i = 0; i < m_element_num; i++)
            {
                BinaryScriptableStore.Dispatch(addToDoAction);
            }
            m_binarySerializer_dispatch_time = Time.realtimeSinceStartup - m_binarySerializer_dispatch_time;
        }
    }
}