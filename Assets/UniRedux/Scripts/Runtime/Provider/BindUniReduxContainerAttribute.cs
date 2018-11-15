using System;

namespace UniRedux.Provider
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BindUniReduxContainerAttribute : Attribute
    {
        public readonly string[] ContainerNames;

        public BindUniReduxContainerAttribute(params string[] containerNames)
        {
            ContainerNames = containerNames;
        }
    }
}