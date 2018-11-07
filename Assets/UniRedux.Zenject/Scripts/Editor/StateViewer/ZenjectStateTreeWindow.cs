using UniRedux.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace UniRedux.Zenject.Editor
{
    public class ZenjectStateTreeWindow : StateTreeWindowBase<ZenjectStateTreeWindow>
    {
        [MenuItem("UniRedux/Zenject/ZenjectStateTreeWindow open")]
        private static void Open()
        {
            GetWindow<ZenjectStateTreeWindow>();
        }

        [FormerlySerializedAs("_sceneContext")] [SerializeField]
        private RunnableContext _runnableContext;

        [Inject] private IStore _store;

        protected override IStore GetStore()
        {
            return _store;
        }

        protected override bool Ready => _store != null;

        protected override void OnCustomGui()
        {
            EditorGUILayout.LabelField("Store Object", GUILayout.Width(80));
            _runnableContext =
                EditorGUILayout.ObjectField(_runnableContext, typeof(RunnableContext), true,
                        GUILayout.Width(150)) as
                    RunnableContext;
        }

        protected override void InitBeforeReady()
        {
            if (_runnableContext == null || _runnableContext.Container == null)
                throw Assert.CreateException("Found null pointer when value was expected");

            _runnableContext.Container.Inject(this);
        }

        protected override void OnOpenNewWindow(ZenjectStateTreeWindow newWindow)
        {
            newWindow._runnableContext = _runnableContext;
        }
    }
}