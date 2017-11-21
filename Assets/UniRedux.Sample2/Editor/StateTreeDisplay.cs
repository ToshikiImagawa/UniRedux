using System;
using UniRedux;
using UniRedux.Sample2;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor.Sample2
{
    public class StateTreeDisplay : EditorWindow
    {
        private TreeViewState _state;
        private UniReduxTreeView _treeView;
        private UniReduxMultiColumnHeader _header;
        private SearchField _searchField;
        private IDisposable _disposable;

        private ToDoScriptableStore _scriptableStore;

        [MenuItem("UniRedux/ToDoList_Try/StateTreeDisplay open")]
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
                EditorGUILayout.LabelField("Store Object", GUILayout.Width(80));
                _scriptableStore =
                    EditorGUILayout.ObjectField(_scriptableStore, typeof(ToDoScriptableStore), true, GUILayout.Width(150))
                        as ToDoScriptableStore;
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
                var toDoState = _scriptableStore.GetState();
                if (toDoState != null)
                {
                    _treeView.SetSerializableStateElement(toDoState.ToSerialize(false));
                }
                _disposable?.Dispose();
                var treeViewSelector = new UpdateTreeViewSelector(_treeView);
                _disposable = _scriptableStore.Subscribe(treeViewSelector);
            }
            catch (Exception e)
            {
                _treeView = new UniReduxTreeView(_state, _header);
                Debug.LogError(e);
            }
            _treeView.Reload();
        }

        private class UpdateTreeViewSelector: IObserver<ToDoState>
        {
            private readonly UniReduxTreeView _treeView;

            private ToDoState _toDoState;
            public void OnNext(ToDoState value)
            {
                _toDoState = value;
            }

            public void OnError(Exception error)
            {
                Debug.LogError(error);
            }

            public void OnCompleted()
            {
                if (_toDoState != null)
                {
                    _treeView.SetSerializableStateElement(_toDoState.ToSerialize(false));
                }
            }

            public UpdateTreeViewSelector(UniReduxTreeView treeView)
            {
                _treeView = treeView;
            }
        }
    }
}