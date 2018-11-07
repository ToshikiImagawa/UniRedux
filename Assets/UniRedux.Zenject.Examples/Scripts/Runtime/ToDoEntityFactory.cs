using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples
{
    public class ToDoEntityFactory : IFactory<Transform, ToDoElement>
    {
        private DiContainer _container;
        private ToDoElement _prefab;

        [Inject]
        public void Construct(ToDoElement prefab, DiContainer container)
        {
            _container = container;
            _prefab = prefab;
        }

        public ToDoElement Create(Transform parentTransform)
        {
            var element = _container.InstantiatePrefabForComponent<ToDoElement>(_prefab);
            element.transform.SetParent(parentTransform, false);
            return element;
        }
    }
}