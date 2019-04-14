using Zenject;
using System.Linq;

namespace UniRedux.Zenject.Examples.Signal.Installers
{
    public class SignalToDoElementInstaller : MonoInstaller
    {
        private ToDoElement _toDoElement;
        private ToDoElement ToDoElement => _toDoElement ?? (_toDoElement = GetComponent<ToDoElement>());
        public override void InstallBindings()
        {
            Container.Bind<ToDoElement>().FromComponentOnRoot().AsSingle();
            Container.DeclareUniReduxSignal<ToDo, ToDo[]>(Filter).SetParent<ToDo[]>();
        }

        private ToDo Filter(ToDo[] toDos)
        {
            return toDos.FirstOrDefault(todo => todo.Id == (ToDoElement?.ToDoId ?? -1));
        }
    }
}
