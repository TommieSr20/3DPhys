// WARNING: Auto generated code. Modifications will be lost!
// Original source 'com.unity.services.shared' @0.0.11.
using System;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Shared.DependencyInversion
{
    class TypeAlreadyRegisteredException : Exception
    {
        public TypeAlreadyRegisteredException(Type type)
            : base($"A factory for type {type.Name} has already been registered")
        {
        }
    }
}
