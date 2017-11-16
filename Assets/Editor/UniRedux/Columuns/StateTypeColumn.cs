using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor.UniRedux
{
    public class StateTypeColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateType;

        public StateTypeColumn()
        {
            width = minWidth = maxWidth = 100;
            autoResize = true;
            headerContent = new GUIContent("StateType");
            sortingArrowAlignment = TextAlignment.Left;
            allowToggleVisibility = true;
        }
    }
}
