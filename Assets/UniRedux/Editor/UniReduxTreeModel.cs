using System;
using UniRedux;
using UnityEditor.IMGUI.Controls;

namespace UniReduxEditor
{
    public class UniReduxTreeModel : TreeViewItem
    {
        public UniReduxTreeElement Element { get; set; }

        public UniReduxTreeModel(int id, string name, Type type, object value, ObjectType objectType, int depth) : base(id, depth, name)
        {
            Element = new UniReduxTreeElement(id, name, type, value, objectType, depth);
        }
    }
}