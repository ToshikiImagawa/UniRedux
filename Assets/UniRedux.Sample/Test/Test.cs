using UnityEngine;

namespace UniRedux.Sample.Test
{
    public class Test : MonoBehaviour
    {
        public ScriptableObjectToDoApplication SimpleToDoApplication;
        public ScriptableObjectJsonToDoApplication JsonToDoApplication;
        public ScriptableObjectBinaryToDoApplication BinaryToDoApplication;

        public int m_element_num = 1000;

        public float scriptableObjectNoSerializerDispatchTime = 0.0f;
        public float scriptableObjectJsonSerializerDispatchTime = 0.0f;
        public float scriptableObjectBinarySerializerDispatchTime = 0.0f;
        public float singletonNoSerializerDispatchTime = 0.0f;
        public float singletonJsonSerializerDispatchTime = 0.0f;
        public float singletonBinarySerializerDispatchTime = 0.0f;

        private AddToDoAction addToDoAction = new AddToDoAction
        {
            Text = "TestTestTestTestTestTestTestTestTestTestTestTestTestTest"
        };

        public void OnClick()
        {
            StartCoroutine(Run());
        }

        private System.Collections.IEnumerator Run()
        {
            float sonsdt1 = 0f, sonsdt2 = 0f, sonsdt3 = 0f,
                sojsdt1 = 0f, sojsdt2 = 0f, sojsdt3 = 0f,
                sobsdt1 = 0f, sobsdt2 = 0f, sobsdt3 = 0f,
                snsdt1 = 0f, snsdt2 = 0f, snsdt3 = 0f,
                sjsdt1 = 0f, sjsdt2 = 0f, sjsdt3 = 0f,
                sbsdt1 = 0f, sbsdt2 = 0f, sbsdt3 = 0f;

            #region 1
            yield return Run(SimpleToDoApplication.CurrentStore, time =>
            {
                sonsdt1 = time;
            });
            yield return null;
            yield return Run(JsonToDoApplication.CurrentStore, time =>
            {
                sojsdt1 = time;
            });
            yield return null;
            yield return Run(BinaryToDoApplication.CurrentStore, time =>
            {
                sobsdt1 = time;
            });
            yield return null;
            yield return Run(SingletonToDoApplication.CurrentStore, time =>
            {
                snsdt1 = time;
            });
            yield return null;
            yield return Run(SingletonJsonToDoApplication.CurrentStore, time =>
            {
                sjsdt1 = time;
            });
            yield return null;
            yield return Run(SingletonBinaryToDoApplication.CurrentStore, time =>
            {
                sbsdt1 = time;
            });
            #endregion
            #region 2
            yield return Run(JsonToDoApplication.CurrentStore, time =>
            {
                sojsdt2 = time;
            });
            yield return null;
            yield return Run(BinaryToDoApplication.CurrentStore, time =>
            {
                sobsdt2 = time;
            });
            yield return null;
            yield return Run(SimpleToDoApplication.CurrentStore, time =>
            {
                sonsdt2 = time;
            });
            yield return null;
            yield return Run(SingletonJsonToDoApplication.CurrentStore, time =>
            {
                sjsdt2 = time;
            });
            yield return null;
            yield return Run(SingletonBinaryToDoApplication.CurrentStore, time =>
            {
                sbsdt2 = time;
            });
            yield return null;
            yield return Run(SingletonToDoApplication.CurrentStore, time =>
            {
                snsdt2 = time;
            });
            #endregion
            #region 3
            yield return Run(BinaryToDoApplication.CurrentStore, time =>
            {
                sobsdt3 = time;
            });
            yield return null;
            yield return Run(SimpleToDoApplication.CurrentStore, time =>
            {
                sonsdt3 = time;
            });
            yield return null;
            yield return Run(JsonToDoApplication.CurrentStore, time =>
            {
                sojsdt3 = time;
            });
            yield return null;
            yield return Run(SingletonBinaryToDoApplication.CurrentStore, time =>
            {
                sbsdt3 = time;
            });
            yield return null;
            yield return Run(SingletonToDoApplication.CurrentStore, time =>
            {
                snsdt3 = time;
            });
            yield return null;
            yield return Run(SingletonJsonToDoApplication.CurrentStore, time =>
            {
                sjsdt3 = time;
            });
            #endregion

            scriptableObjectNoSerializerDispatchTime = (sonsdt1 + sonsdt2 + sonsdt3) / 3;
            scriptableObjectJsonSerializerDispatchTime = (sojsdt1 + sojsdt2 + sojsdt3) / 3;
            scriptableObjectBinarySerializerDispatchTime = (sobsdt1 + sobsdt2 + sobsdt3) / 3;
            singletonNoSerializerDispatchTime = (snsdt1 + snsdt2 + snsdt3) / 3;
            singletonJsonSerializerDispatchTime = (sjsdt1 + sjsdt2 + sjsdt3) / 3;
            singletonBinarySerializerDispatchTime = (sbsdt1 + sbsdt2 + sbsdt3) / 3;
        }
        private System.Collections.IEnumerator Run(IStore<ToDoState> store, System.Action<float> setTimer)
        {
            var time = Time.realtimeSinceStartup;
            for (int i = 0; i < m_element_num; i++)
            {
                store.Dispatch(addToDoAction);
            }
            setTimer(Time.realtimeSinceStartup - time);
            yield break;
        }
    }
}