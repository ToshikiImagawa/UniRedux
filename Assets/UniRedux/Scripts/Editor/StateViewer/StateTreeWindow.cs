using UnityEditor;

namespace UniRedux.Editor
{
    public class StateTreeWindow : StateTreeWindowBase<StateTreeWindow>
    {
        [MenuItem("UniRedux/StateTreeWindow open")]
        private static void Open()
        {
            GetWindow<StateTreeWindow>();
        }

        protected override IStore GetStore()
        {
            if (!UnityEngine.Application.isPlaying) return null;
            return UniReduxProvider.Store;
        }
    }
}