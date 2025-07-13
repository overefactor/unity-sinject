using System;

namespace Overefactor.DI.Runtime.Attributes
{
    /// <summary>
    /// The CInjectAttribute attribute is used to mark fields that
    /// should be injected with components on the same object.
    /// </summary>
    /// <example>
    /// This sample shows how to use the CInjectAttribute attribute.
    /// <code>
    /// public class MyClass : MonoBehaviour
    /// {
    ///     [CInject] private MyComponent _myComponent;
    /// }
    /// </code>
    /// This will be equivalent to:
    /// <code>
    /// public class MyClass : MonoBehaviour
    /// {
    ///     private MyComponent _myComponent;
    ///     <br/>
    ///     private void Awake()
    ///     {
    ///         _myComponent = GetComponent&lt;MyComponent&gt;();
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CInjectAttribute : Attribute
    {
        
    }
}