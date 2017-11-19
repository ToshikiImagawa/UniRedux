using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor
{
    public class StateObjectTypeColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateObjectType;

        public StateObjectTypeColumn()
        {
            width = minWidth = maxWidth = 32;
            autoResize = true;
            headerContent = new GUIContent("");
            sortingArrowAlignment = TextAlignment.Left;
            allowToggleVisibility = true;
        }
    }
}
