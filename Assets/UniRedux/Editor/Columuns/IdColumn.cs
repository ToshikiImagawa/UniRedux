using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor
{
    public class IdColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.Id;

        public IdColumn()
        {
            width = minWidth = maxWidth = 70;
            headerContent = new GUIContent("Id");
            sortingArrowAlignment = TextAlignment.Right;
            allowToggleVisibility = true;
        }
    }
}