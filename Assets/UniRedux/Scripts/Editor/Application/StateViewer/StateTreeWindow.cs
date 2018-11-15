using UniRedux.Editor;
using UnityEditor;

namespace UniRedux.Application.Editor
{
    public class StateTreeWindow : StateTreeWindowBase<StateTreeWindow>
    {
        private IReduxApplication _application;

        [MenuItem("UniRedux/Application/StateTreeWindow open")]
        private static void Open()
        {
            GetWindow<StateTreeWindow>();
        }

        protected override IStore GetStore()
        {
            if (!UnityEngine.Application.isPlaying) return null;
            _application = UniReduxApplication.GetApplication();
            return _application.CurrentStore;
        }
    }
}