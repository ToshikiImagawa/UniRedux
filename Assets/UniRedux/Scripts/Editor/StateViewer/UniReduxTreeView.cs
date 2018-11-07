using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public class UniReduxTreeView : TreeView
    {
        public const string SortedColumnIndexStateKey = "UniReduxTreeView_SearchString";
        private TreeViewItem RootItem { get; set; }
        private int _id = 1;
        private readonly Action<int> _openNewTreeViewAction;

        private int Id => _id++;

        public void SetSerializableStateElement(int rootId = 0, params SerializableStateElement[] rootElement)
        {
            _id = 1;
            RootItem = new TreeViewItem
            {
                id = 0,
                depth = -1
            };

            var items = rootElement.Select(CreateTreeViewItem).ToList();
            if (rootId > 0)
            {
                TreeViewItem childRoot = null;
                var children = items;
                do
                {
                    if (children == null)
                    {
                        childRoot = null;
                        break;
                    }

                    childRoot = null;
                    foreach (var item in children)
                    {
                        if (item.id < rootId)
                        {
                            childRoot = item;
                        }
                        else if (item.id == rootId)
                        {
                            childRoot = item;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    children = childRoot?.children ?? null;
                } while (childRoot != null && childRoot.id != rootId);

                if (childRoot != null) items = children;
            }

            RootItem.children = items;
            Reload();
        }

        public UniReduxTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader,
            Action<int> openNewTreeViewAction)
            : base(state, multiColumnHeader)
        {
            rowHeight = 32;
            columnIndexForTreeFoldouts = 1;
            showBorder = true;
            customFoldoutYOffset = (rowHeight - EditorGUIUtility.singleLineHeight) * 0.5f;
            extraSpaceBeforeIconAndLabel = 20;
            showAlternatingRowBackgrounds = true;
            multiColumnHeader.sortingChanged += SortItems;

            multiColumnHeader.ResizeToFit();
            if (RootItem != null) Reload();
            multiColumnHeader.sortedColumnIndex = SessionState.GetInt(SortedColumnIndexStateKey, -1);
            _openNewTreeViewAction = openNewTreeViewAction;
        }

        protected override TreeViewItem BuildRoot()
        {
            return RootItem ?? (RootItem = new TreeViewItem
            {
                depth = -1,
                children = new List<TreeViewItem>
                {
                    new TreeViewItem
                    {
                        id = 1,
                        depth = 0,
                        displayName = "No Store"
                    }
                }
            });
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            var uniReduxTreeModel = item as UniReduxTreeModel;
            if (uniReduxTreeModel == null) return false;
            if (uniReduxTreeModel.Element.StateName.Contains(search))
            {
                return true;
            }

            return uniReduxTreeModel.Element.StateName.Contains(search)
                   | $"{uniReduxTreeModel.Element.StateValue}".Contains(search);
        }

        private void CellGUI(Rect cellRect, UniReduxTreeModel uniReduxTreeModel, ColumnIndex columnIndex,
            ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (columnIndex)
            {
                case ColumnIndex.Id:
                    EditorGUI.LabelField(cellRect, $"{uniReduxTreeModel.Element.StateId}", EditorStyles.label);
                    break;
                case ColumnIndex.StateName:
                    var spaceRect = cellRect;
                    spaceRect.x += uniReduxTreeModel.Element.StateDepth * 18f + 20;
                    var pass = $"UniRedux/icon_{uniReduxTreeModel.Element.ObjectType.ToString()}.png";
                    var texture = EditorGUIUtility.Load(pass) as Texture;
                    var textureRect = spaceRect;
                    textureRect.width = textureRect.height;
                    spaceRect.x += textureRect.width + 5;
                    GUI.DrawTexture(textureRect, texture, ScaleMode.ScaleToFit);
                    EditorGUI.LabelField(spaceRect, uniReduxTreeModel.Element.StateName, EditorStyles.largeLabel);
                    break;
                case ColumnIndex.StateType:

                    var curEvent = Event.current;
                    var typeName = uniReduxTreeModel.Element.StateType.Name;
                    if (cellRect.Contains(curEvent.mousePosition))
                        typeName = uniReduxTreeModel.Element.StateType.FullName;

                    EditorGUI.LabelField(cellRect, typeName, EditorStyles.label);
                    break;
                case ColumnIndex.StateValue:
                    if (uniReduxTreeModel.Element.ObjectType == ObjectType.Array)
                    {
                        EditorGUI.LabelField(cellRect, $"{uniReduxTreeModel.Element.StateValue}", EditorStyles.label);
                    }
                    else if (uniReduxTreeModel.Element.ObjectType == ObjectType.Value)
                    {
                        if (uniReduxTreeModel.Element.StateType.IsEnum)
                        {
                            EditorGUI.SelectableLabel(cellRect, $"{uniReduxTreeModel.Element.StateValue}",
                                EditorStyles.popup);
                        }
                        else
                        {
                            switch (Type.GetTypeCode(uniReduxTreeModel.Element.StateType))
                            {
                                case TypeCode.Int32:
                                    var intValue = Convert.ToInt32(uniReduxTreeModel.Element.StateValue);
                                    EditorGUI.SelectableLabel(cellRect, $"{intValue}", EditorStyles.textField);
                                    break;
                                case TypeCode.String:
                                    EditorGUI.SelectableLabel(cellRect, $"{uniReduxTreeModel.Element.StateValue}",
                                        EditorStyles.textField);
                                    break;
                                case TypeCode.Boolean:
                                    var flag = Convert.ToBoolean(uniReduxTreeModel.Element.StateValue);
                                    Rect toggleRect = cellRect;
                                    toggleRect.width = 18;
                                    using (new BackgroundColorScope(flag ? Color.green : Color.red))
                                    {
                                        EditorGUI.Toggle(toggleRect, flag, EditorStyles.radioButton);
                                    }

                                    break;
                                case TypeCode.Double:
                                    var doubleValue = Convert.ToDouble(uniReduxTreeModel.Element.StateValue);
                                    EditorGUI.SelectableLabel(cellRect, $"{doubleValue}", EditorStyles.textField);
                                    break;
                                case TypeCode.DateTime:
                                default:
                                    EditorGUI.SelectableLabel(cellRect, $"{uniReduxTreeModel.Element.StateValue}",
                                        EditorStyles.textField);
                                    break;
                            }
                        }
                    }

                    break;
                case ColumnIndex.Button:
                    if (uniReduxTreeModel.Element.ObjectType != ObjectType.Value &&
                        uniReduxTreeModel.children != null && uniReduxTreeModel.children.Count > 0)
                    {
                        if (GUI.Button(cellRect, "Open"))
                        {
                            _openNewTreeViewAction(uniReduxTreeModel.Element.StateId);
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var uniReduxTreeModel = args.item as UniReduxTreeModel;

            if (uniReduxTreeModel == null)
            {
                base.RowGUI(args);
                return;
            }

            var backgroundColor = GUI.backgroundColor;
            switch (uniReduxTreeModel.Element.ObjectType)
            {
                case ObjectType.Array:
                    backgroundColor = Color.blue;
                    break;
                case ObjectType.Object:
                    backgroundColor = Color.yellow;
                    break;
                default:
                    break;
            }

            using (new BackgroundColorScope(backgroundColor))
            {
                for (var index = 0; index < args.GetNumVisibleColumns(); index++)
                {
                    CellGUI(args.GetCellRect(index), uniReduxTreeModel, (ColumnIndex) args.GetColumn(index), ref args);
                }
            }
        }

        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            var cellRect = GetCellRectForTreeFoldouts(rowRect);
            CenterRectUsingSingleLineHeight(ref cellRect);
            return base.GetRenameRect(cellRect, row, item);
        }

        private TreeViewItem CreateTreeViewItem(SerializableStateElement element, int depth = 0)
        {
            var item = new UniReduxTreeModel(Id, element.Name, element.Type, element.Value, element.ObjectType, depth);
            var items =
                element.Children.Select(childrenElement => CreateTreeViewItem(childrenElement, depth + 1)).ToList();
            item.children = items;
            return item;
        }

        private void SortItems(MultiColumnHeader multiColumnHeader)
        {
            SessionState.SetInt(SortedColumnIndexStateKey, multiColumnHeader.sortedColumnIndex);
            var index = (ColumnIndex) multiColumnHeader.sortedColumnIndex;
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);

            if (rootItem != null)
            {
                rootItem.children = SortItems(rootItem.children, index, ascending);
                BuildRows(rootItem);
            }
        }

        private List<TreeViewItem> SortItems(List<TreeViewItem> children, ColumnIndex index, bool ascending)
        {
            var items = children.Cast<UniReduxTreeModel>();
            foreach (var item in items)
            {
                if (item.children != null)
                {
                    item.children = SortItems(item.children, index, ascending);
                }
            }

            IOrderedEnumerable<UniReduxTreeModel> orderedEnumerable;
            switch (index)
            {
                case ColumnIndex.Id:
                    orderedEnumerable = items.OrderBy(item => item.id);
                    break;
                case ColumnIndex.StateName:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateName);
                    break;
                case ColumnIndex.StateValue:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateValue);
                    break;
                case ColumnIndex.StateType:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            items = orderedEnumerable.AsEnumerable();
            if (!ascending)
            {
                items = items.Reverse();
            }

            return items.Cast<TreeViewItem>().ToList();
        }
    }

    public enum ColumnIndex
    {
        Id,
        StateName,
        StateValue,
        StateType,
        Button
    }

    public class BackgroundColorScope : GUI.Scope
    {
        private readonly Color color;

        public BackgroundColorScope(Color color)
        {
            this.color = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        protected override void CloseScope()
        {
            GUI.backgroundColor = color;
        }
    }
}