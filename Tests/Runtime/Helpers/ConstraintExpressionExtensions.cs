using NUnit.Framework.Constraints;
using UnityEngine;

namespace Overefactor.DI.Tests.Tests.Runtime.Helpers
{
    internal class DestroyedConstraint : Constraint
    {
        public override string Description { get; protected set; } = "Destroyed object";

        public override ConstraintResult ApplyTo(object actual)
        {
            if (actual == null) return new ConstraintResult(this, null, true);
            if (actual is not Object o) return new ConstraintResult(this, actual, false);
            
            return new ConstraintResult(this, actual, !o);
        }
    }
}