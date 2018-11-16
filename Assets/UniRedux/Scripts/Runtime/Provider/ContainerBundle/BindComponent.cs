using System;
using UnityEngine;

namespace UniRedux.Provider
{
#if UNITY_EDITOR
    [Serializable]
    public struct BindComponent
    {
        [HideInInspector] public string ContainerName;
        public Component Component;
    }
#endif
}