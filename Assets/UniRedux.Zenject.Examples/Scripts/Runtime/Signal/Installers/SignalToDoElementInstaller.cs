using Zenject;
using System.Linq;

namespace UniRedux.Zenject.Examples.Signal.Installers
{
    public class SignalToDoElementInstaller : MonoInstaller
    {
        private ToDoElement _toDoElement;
        private ToDoElement ToDoElement => _toDoElement != null ? _toDoElement : _toDoElement = GetComponent<ToDoElement>();
        public override void InstallBindings()
        {
            Container.BindInstance(ToDoElement);
            Container.DeclareUniReduxSignal<ToDo, ToDo[]>(
                toDos => toDos.FirstOrDefault(todo => todo.Id == ToDoElement.ToDoId)
                ).SetParent<ToDo[]>();
        }
    }
}
