using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public class StateTypeColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateType;

        public StateTypeColumn()
        {
            width = minWidth = 100;
            maxWidth = 500;
            autoResize = true;
            headerContent = new GUIContent("StateType");
            sortingArrowAlignment = TextAlignment.Left;
            allowToggleVisibility = true;
        }
    }
}
