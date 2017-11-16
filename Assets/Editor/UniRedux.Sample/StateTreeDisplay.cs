using System;
using UniRedux;
using UniRedux.Sample;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor.UniRedux.Sample
{
    public class StateTreeDisplay : EditorWindow
    {
        private TreeViewState _state;
        private UniReduxTreeView _treeView;
        private UniReduxMultiColumnHeader _header;
        [SerializeField] MultiColumnHeaderState _mMultiColumnHeaderState;
        private SearchField _searchField;

        [MenuItem("UniRedux/StateTreeDisplay open")]
        private static void Open()
        {
            GetWindow<StateTreeDisplay>();
        }

        private void OnEnable()
        {
            _state = new TreeViewState();
            _header = new UniReduxMultiColumnHeader(null);
            _treeView = new UniReduxTreeView(_state, _header)
            {
                searchString = SessionState.GetString(UniReduxTreeView.SortedColumnIndexStateKey, "")
            };
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            _treeView.Reload();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UniReduxで管理されているStoreのStateを表示します。");
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    Reload();
                }

                using (var checkScope = new EditorGUI.ChangeCheckScope())
                {
                    if (_searchField == null) return;
                    var searchString = _searchField.OnToolbarGUI(_treeView.searchString);

                    if (checkScope.changed)
                    {
                        SessionState.SetString(UniReduxTreeView.SortedColumnIndexStateKey, searchString);
                        _treeView.searchString = searchString;
                    }
                }
            }

            if (_treeView != null && _treeView.GetRows() != null)
            {
                _treeView.OnGUI(new Rect(0, EditorGUIUtility.singleLineHeight * 2.3f, position.width,
                    position.height - EditorGUIUtility.singleLineHeight * 2.3f));
            }
        }

        private void Reload()
        {
            try
            {
                var toDoState = ToDoApplication.CurrentStore.GetState();
                if (toDoState != null)
                {
                    _treeView.SetSerializableStateElement(toDoState.ToSerialize());
                }
            }
            catch (Exception e)
            {
                _treeView = new UniReduxTreeView(_state, _header);
                Debug.LogError(e);
            }
            _treeView.Reload();
        }
    }
}