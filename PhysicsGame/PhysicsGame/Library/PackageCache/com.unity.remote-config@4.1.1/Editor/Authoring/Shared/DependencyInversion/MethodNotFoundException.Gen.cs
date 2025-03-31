// WARNING: Auto generated code. Modifications will be lost!
// Original source 'com.unity.services.shared' @0.0.11.
using System;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Shared.DependencyInversion
{
    class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(Type type, string methodName)
            : base($"Type {type.Name} must have a single public method called {methodName}")
        {
        }
    }
}
