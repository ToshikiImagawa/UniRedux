using UniRedux;
using UnityEditor.IMGUI.Controls;

namespace Editor.UniRedux
{
    public class UniReduxTreeModel : TreeViewItem
    {
        public UniReduxTreeElement Element { get; set; }

        public UniReduxTreeModel(int id, string name, string typeName, string value, StateType type, int depth)
            : base(id, depth, name)
        {
            Element = new UniReduxTreeElement(id, name, typeName, value, type, depth);
        }
    }
}