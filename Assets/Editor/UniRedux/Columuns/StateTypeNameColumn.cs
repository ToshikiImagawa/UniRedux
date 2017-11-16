using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor.UniRedux
{
    public class StateTypeNameColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateTypeName;

        public StateTypeNameColumn()
        {
            width = minWidth = 100;
            maxWidth = 500;
            autoResize = true;
            headerContent = new GUIContent("StateTypeName");
            sortingArrowAlignment = TextAlignment.Left;
            allowToggleVisibility = true;
        }
    }
}
