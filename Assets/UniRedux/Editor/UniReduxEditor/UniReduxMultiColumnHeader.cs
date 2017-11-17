using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace UniReduxEditor
{
    public class UniReduxMultiColumnHeader : MultiColumnHeader
    {
        public UniReduxMultiColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            var columns = new List<MultiColumnHeaderState.Column>();

            foreach (ColumnIndex value in Enum.GetValues(typeof(ColumnIndex)))
            {
                switch (value)
                {
                    case ColumnIndex.Id:
                        columns.Add(new IdColum());
                        break;
                    case ColumnIndex.StateName:
                        columns.Add(new StateNameColum());
                        break;
                    case ColumnIndex.StateObjectType:
                        columns.Add(new StateObjectTypeColumn());
                        break;
                    case ColumnIndex.StateValue:
                        columns.Add(new StateValueColumn());
                        break;
                    case ColumnIndex.StateType:
                        columns.Add(new StateTypeColumn());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            this.state = new MultiColumnHeaderState(columns.ToArray());
        }
    }

    public interface IHasColumnIndex
    {
        ColumnIndex Index { get; }
    }
}