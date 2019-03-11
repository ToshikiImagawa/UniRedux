using System.Collections.Generic;
using Zenject;

namespace UniRedux
{
    [NoReflectionBaking]
    public class UniReduxSignalCopyBinder
    {
        private readonly List<BindInfo> _bindInfos;

        protected UniReduxSignalDeclarationBindInfo SignalBindInfo
        {
            get; private set;
        }

        public UniReduxSignalCopyBinder()
        {
            _bindInfos = new List<BindInfo>();
        }

        public UniReduxSignalCopyBinder(BindInfo bindInfo)
        {
            _bindInfos = new List<BindInfo>
            {
                bindInfo
            };
        }
        public UniReduxSignalCopyBinder(
            UniReduxSignalDeclarationBindInfo signalBindInfo)
        {
            _bindInfos = new List<BindInfo>();
            SignalBindInfo = signalBindInfo;
        }

        public void AddCopyBindInfo(BindInfo bindInfo)
        {
            _bindInfos.Add(bindInfo);
        }

        public void CopyIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyIntoAll);
        }

        public void CopyIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyDirectOnly);
        }

        public void MoveIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveIntoAll);
        }

        public void MoveIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveDirectOnly);
        }

        private void SetInheritanceMethod(BindingInheritanceMethods method)
        {
            for (int i = 0; i < _bindInfos.Count; i++)
            {
                _bindInfos[i].BindingInheritanceMethod = method;
            }
        }
    }
}
