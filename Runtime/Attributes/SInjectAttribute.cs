using System;

namespace Overefactor.DI.Runtime.Attributes
{
    /// <summary>
    /// The SInjectAttribute attribute is used to mark fields that should be injected.
    /// </summary>
    /// <example>
    /// This sample shows how to use the SInjectAttribute attribute.
    /// <code>
    /// public class MyClass
    /// {
    ///     [SInject] private MyDependency _myDependency;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SInjectAttribute : Attribute
    {
        
    }
}