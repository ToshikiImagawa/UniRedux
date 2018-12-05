using System;

namespace UniRedux.Provider
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class UniReduxInjectAttribute : Attribute
    {
        public string PropertyName { get; set; }
    }
}