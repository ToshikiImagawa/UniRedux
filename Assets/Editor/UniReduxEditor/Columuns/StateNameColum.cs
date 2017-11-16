using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniReduxEditor
{
    public class StateNameColum : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateName;

        public StateNameColum()
        {
            width = minWidth = 150;
            maxWidth = 500;
            autoResize = true;
            headerContent = new GUIContent("StateName");
            sortingArrowAlignment = TextAlignment.Center;
            allowToggleVisibility = false;
        }
    }
}
