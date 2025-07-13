using NUnit.Framework.Constraints;

namespace Overefactor.DI.Tests.Tests.Runtime.Helpers
{
    internal abstract class Is : NUnit.Framework.Is
    {
        public static DestroyedConstraint Destroyed => new DestroyedConstraint();
    }
    
    internal static class ConstraintExpressionExtensions
    {
        public static DestroyedConstraint Destroyed(this ConstraintExpression expression)
        {
            var constraint = new DestroyedConstraint();
            expression.Append(constraint);
            return constraint;
        }
    }
}