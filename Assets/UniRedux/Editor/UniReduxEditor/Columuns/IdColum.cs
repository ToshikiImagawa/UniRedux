using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor
{
    public class IdColum : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.Id;

        public IdColum()
        {
            width = minWidth = maxWidth = 32;
            headerContent = new GUIContent("Id");
            sortingArrowAlignment = TextAlignment.Right;
            allowToggleVisibility = true;
        }
    }
}