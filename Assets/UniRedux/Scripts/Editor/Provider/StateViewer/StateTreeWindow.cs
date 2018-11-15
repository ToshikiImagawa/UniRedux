using UniRedux.Editor;
using UnityEditor;

namespace UniRedux.Provider.Editor
{
    public class StateTreeWindow : StateTreeWindowBase<StateTreeWindow>
    {
        [MenuItem("UniRedux/Provider/StateTreeWindow open")]
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