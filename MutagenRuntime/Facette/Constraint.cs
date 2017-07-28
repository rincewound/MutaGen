using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MutagenRuntime
{
    public class Constraint
    {
        public delegate bool ConstraintGuard(List<object> activeValues);

        ConstraintGuard guard;
        public Facette constraintSource { get; private set; }
        public Facette constraintTarget { get; private set; }
        List<object> valuesToConstrainTo;

        public Constraint(Facette constraintSource,Facette constraintTarget, ConstraintGuard guard, List<object> valuesToConstrainTo)
        {
            if (guard == null)
                throw new NoConstraintGuardException();

            if (constraintSource == null)
                throw new NoSuchFacetteException();

            if (constraintTarget == null)
                throw new NoSuchFacetteException();

            this.guard = guard;
            this.constraintSource = constraintSource;
            this.valuesToConstrainTo = valuesToConstrainTo;
            this.constraintTarget = constraintTarget;
        }

        public bool IsActive(Facette theFacette, BitArray val)
        {
            if (theFacette != constraintSource)
                return false;
            var activeValues = constraintSource.GetValues(val);
            return guard(activeValues);            
        }

        public List<object> GetConstrainedValues()
        {
            return valuesToConstrainTo;
        }

        public bool ValueSetFullfillsConstraint(Facette sourceFacette, List<object> valueSet)
        {
            if (sourceFacette != constraintTarget)
                return true;
            return valueSet.All(x => valuesToConstrainTo.Contains(x));
        }

        internal bool ConstrainsFacette(Facette theFacette)
        {
            return theFacette == constraintTarget;
        }
    }
}