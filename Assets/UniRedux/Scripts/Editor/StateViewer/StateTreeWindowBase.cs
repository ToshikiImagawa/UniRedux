using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public abstract class StateTreeWindowBase<TStateTreeWindow> : EditorWindow
        where TStateTreeWindow : StateTreeWindowBase<TStateTreeWindow>
    {
        private TreeViewState _state;
        private UniReduxTreeView _treeView;
        private UniReduxMultiColumnHeader _header;
        private SearchField _searchField;
        private IDisposable _disposable;
        private int _rootId;

        private IStore _store;

        /// <summary>
        /// get store
        /// </summary>
        /// <returns></returns>
        protected abstract IStore GetStore();

        /// <summary>
        /// reload StateTreeWindow
        /// </summary>
        public void Reload()
        {
            if (!Ready) InitBeforeReady();

            _store = GetStore();
            if (_store == null) return;

            _disposable?.Dispose();
            _disposable = _store.Subscribe(e =>
            {
                if (_store.GetStateForce() != null)
                    _treeView.SetSerializableStateElement(_rootId, _store.GetStateForce().ToSerialize());
            });

            try
            {
                var toDoState = _store.GetStateForce();
                if (toDoState != null)
                {
                    _treeView.SetSerializableStateElement(_rootId, toDoState.ToSerialize());
                }
            }
            catch (Exception e)
            {
                _treeView = new UniReduxTreeView(_state, _header, NewOpenWindow);
                Debug.LogError(e);
            }

            _treeView.Reload();
        }

        /// <summary>
        /// Ready to draw
        /// </summary>
        protected virtual bool Ready => true;

        /// <summary>
        /// Add to ToolBar GUI
        /// </summary>
        protected virtual void OnCustomGui()
        {
        }

        /// <summary>
        /// Initialization done before Ready
        /// </summary>
        protected virtual void InitBeforeReady()
        {
        }

        /// <summary>
        /// Called when opening a new window
        /// </summary>
        /// <param name="newWindow"></param>
        protected virtual void OnOpenNewWindow(TStateTreeWindow newWindow)
        {
        }

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
            EditorGUILayout.LabelField(
                $"Display the State of Store managed by UniRedux.{(_rootId > 0 ? $" Showing Id:{_rootId}." : string.Empty)}"
            );
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

                OnCustomGui();
            }

            if (_treeView?.GetRows() != null)
            {
                _treeView.OnGUI(new Rect(0, EditorGUIUtility.singleLineHeight * 2.3f, position.width,
                    position.height - EditorGUIUtility.singleLineHeight * 2.3f));
            }
        }

        private void NewOpenWindow(int id)
        {
            var toDoTreeWindow = CreateInstance<TStateTreeWindow>();

            OnOpenNewWindow(toDoTreeWindow);
            toDoTreeWindow._rootId = id;
            toDoTreeWindow.Show();
            toDoTreeWindow.Reload();
        }
    }
}