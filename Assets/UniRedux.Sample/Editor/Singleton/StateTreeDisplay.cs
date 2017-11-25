using System;
using UniRedux;
using UniRedux.Sample;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor.Sample.Singleton
{
    public abstract class StateTreeDisplay : EditorWindow
    {
        private TreeViewState _state;
        private UniReduxTreeView _treeView;
        private UniReduxMultiColumnHeader _header;
        private SearchField _searchField;
        private IDisposable _disposable;
        private int RootId = 0;

        private void OnEnable()
        {
            _state = new TreeViewState();
            _header = new UniReduxMultiColumnHeader(null);
            _treeView = new UniReduxTreeView(_state, _header, NewOpenWindow)
            {
                searchString = SessionState.GetString(UniReduxTreeView.SortedColumnIndexStateKey, "")
            };
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            _treeView.Reload();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"UniReduxで管理されているStoreのStateを表示します。{(RootId > 0 ? $"Id:{RootId}を表示しています." : "")}");
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

        public void Reload()
        {
            try
            {
                var toDoState = CurrentStore.GetState();
                if (toDoState != null)
                {
                    _treeView.SetSerializableStateElement(RootId, toDoState.ToSerialize(false));
                }
                _disposable?.Dispose();
                var treeViewSelector = new UpdateTreeViewSelector(_treeView, RootId);
                _disposable = CurrentStore.Subscribe(treeViewSelector);
            }
            catch (Exception e)
            {
                _treeView = new UniReduxTreeView(_state, _header, NewOpenWindow);
                Debug.LogError(e);
            }
            _treeView.Reload();
        }

        private class UpdateTreeViewSelector : IObserver<ToDoState>
        {
            private readonly UniReduxTreeView _treeView;
            private readonly int _rootId = 0;

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
                    _treeView.SetSerializableStateElement(_rootId, _toDoState.ToSerialize(false));
                }
            }

            public UpdateTreeViewSelector(UniReduxTreeView treeView, int rootId = 0)
            {
                _treeView = treeView;
                _rootId = rootId;
            }
        }

        private void NewOpenWindow(int id)
        {
            var toDoTreeWindow = CreateInstance<StateTreeDisplay>();

            toDoTreeWindow.RootId = id;
            toDoTreeWindow.Show();
            toDoTreeWindow.Reload();
        }

        protected abstract IStore<ToDoState> CurrentStore { get; }
    }
}
