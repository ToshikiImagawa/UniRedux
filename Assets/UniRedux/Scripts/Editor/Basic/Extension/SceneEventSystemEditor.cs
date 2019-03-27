using UniRedux.EventSystems;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Editor.Application
{
    [CustomEditor(typeof(SceneEventSystem))]
    public class SceneEventSystemEditor : UnityEditor.Editor
    {
        private bool _foldoutStatus = true;

        public override void OnInspectorGUI()
        {
            var rxEventSystem = target as SceneEventSystem;
            _foldoutStatus = EditorGUILayout.Foldout(_foldoutStatus, "TargetGameObjects");
            if (!_foldoutStatus || rxEventSystem == null) return;
            foreach (var targetObject in rxEventSystem.TargetObjects)
            {
                EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true);
            }
        }
    }
}