using UniRedux.Sample;
using UniRedux;
using UnityEditor;
using UniRedux.Sample.Singleton.Binary;

namespace UniReduxEditor.Sample.Singleton.Binary
{
    public class SimpleStateTreeDisplay : StateTreeDisplay
    {
        [MenuItem("UniRedux/Singleton_ToDoList/BinaryStateTreeDisplay open")]
        private static void Open()
        {
            GetWindow<SimpleStateTreeDisplay>();
        }
        protected override IStore<ToDoState> CurrentStore
        {
            get
            {
                return ToDoApplication.CurrentStore;
            }
        }

        protected override StateTreeDisplay NewInstance
        {
            get
            {
                return CreateInstance<SimpleStateTreeDisplay>();
            }
        }
    }
}