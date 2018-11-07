using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public class ButtonColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.Button;

        public ButtonColumn()
        {
            width = minWidth = maxWidth = 100;
            headerContent = new GUIContent("Option");
            sortingArrowAlignment = TextAlignment.Right;
            allowToggleVisibility = false;
        }
    }
}
