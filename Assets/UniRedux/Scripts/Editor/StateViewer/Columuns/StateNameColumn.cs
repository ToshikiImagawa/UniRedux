using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public class StateNameColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateName;

        public StateNameColumn()
        {
            width = minWidth = 100;
            maxWidth = 500;
            autoResize = true;
            headerContent = new GUIContent("StateName");
            sortingArrowAlignment = TextAlignment.Center;
            allowToggleVisibility = false;
        }
    }
}
