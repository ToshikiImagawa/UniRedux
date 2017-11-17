using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor
{
    public class UniReduxTreeView : TreeView
    {
        public const string SortedColumnIndexStateKey = "UniReduxTreeView_SearchString";
        private TreeViewItem RootItem { get; set; }
        private int _id = 1;

        private int Id => _id++;

        public void SetSerializableStateElement(params SerializableStateElement[] rootElement)
        {
            _id = 1;
            RootItem = new TreeViewItem
            {
                id = 0,
                depth = -1
            };

            var items = rootElement.Select(CreateTreeViewItem).ToList();
            RootItem.children = items;

            Reload();
        }

        public UniReduxTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
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
                   | uniReduxTreeModel.Element.StateValue.Contains(search);
        }

        private void CellGUI(Rect cellRect, UniReduxTreeModel uniReduxTreeModel, ColumnIndex columnIndex,
            ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (columnIndex)
            {
                case ColumnIndex.Id:
                    EditorGUI.LabelField(cellRect, args.item.id.ToString(), EditorStyles.label);
                    break;
                case ColumnIndex.StateName:
                    var spaceRect = cellRect;
                    spaceRect.x += uniReduxTreeModel.Element.StateDepth * 18f + 20;
                    EditorGUI.LabelField(spaceRect, uniReduxTreeModel.Element.StateName, EditorStyles.largeLabel);
                    break;
                case ColumnIndex.StateTypeName:
                    EditorGUI.LabelField(cellRect, uniReduxTreeModel.Element.StateTypeName, EditorStyles.label);
                    break;
                case ColumnIndex.StateValue:
                    EditorGUI.LabelField(cellRect, uniReduxTreeModel.Element.StateValue, EditorStyles.textField);
                    break;
                case ColumnIndex.StateType:
                    EditorGUI.LabelField(cellRect, uniReduxTreeModel.Element.StateType.ToString(), EditorStyles.toolbarPopup);
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

            for (var index = 0; index < args.GetNumVisibleColumns(); index++)
            {
                CellGUI(args.GetCellRect(index), uniReduxTreeModel, (ColumnIndex) args.GetColumn(index), ref args);
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
            var item = new UniReduxTreeModel(Id, element.Name, element.TypeName, element.Value, element.Type, depth);
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
                case ColumnIndex.StateType:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateType);
                    break;
                case ColumnIndex.StateValue:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateValue);
                    break;
                case ColumnIndex.StateTypeName:
                    orderedEnumerable = items.OrderBy(item => item.Element.StateTypeName);
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
        StateTypeName,
        StateValue,
        StateType
    }
}