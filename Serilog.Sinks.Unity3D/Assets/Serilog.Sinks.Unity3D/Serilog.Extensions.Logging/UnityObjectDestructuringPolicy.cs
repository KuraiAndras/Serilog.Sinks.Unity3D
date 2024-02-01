#nullable enable
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace Serilog.Sinks.Unity3D
{
    /// <summary>
    /// Makes sure we destructure <see cref="UnityEngine.Object"/>s as scalars, i.e. keeping their reference.
    /// </summary>
    internal class UnityObjectDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
        {
            result = null;

            if (value is UnityEngine.Object unityObject)
            {
                result = new ScalarValue(unityObject);
                return true;
            }

            return false;
        }
    }
}
