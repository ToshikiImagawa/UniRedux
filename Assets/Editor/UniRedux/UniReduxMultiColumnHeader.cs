using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor.UniRedux
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
                    case ColumnIndex.StateType:
                        columns.Add(new StateTypeColumn());
                        break;
                    case ColumnIndex.StateValue:
                        columns.Add(new StateValueColumn());
                        break;
                    case ColumnIndex.StateTypeName:
                        columns.Add(new StateTypeNameColumn());
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