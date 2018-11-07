﻿using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniRedux.Editor
{
    public class StateValueColumn : MultiColumnHeaderState.Column, IHasColumnIndex
    {
        public ColumnIndex Index { get; } = ColumnIndex.StateValue;

        public StateValueColumn()
        {
            width = minWidth = 100;
            maxWidth = 500;
            autoResize = true;
            headerContent = new GUIContent("StateValue");
            sortingArrowAlignment = TextAlignment.Left;
        }
    }
}