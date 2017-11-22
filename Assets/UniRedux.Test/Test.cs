using UniRedux.Sample;
using UnityEngine;

namespace UniRedux.Test
{
    public class Test : MonoBehaviour
    {
        public Sample2.ToDoScriptableStore JsonScriptableStore;
        public Sample3.ToDoScriptableStore BinaryScriptableStore;

        public int m_element_num = 1000;

        public float m_jsonSerializer_dispatch_time = 0.0f;
        public float m_binarySerializer_dispatch_time = 0.0f;

        public void OnClick()
        {
            var addToDoAction = new AddToDoAction
            {
                Text = "TestTestTestTestTestTestTestTestTestTestTestTestTestTest"
            };

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