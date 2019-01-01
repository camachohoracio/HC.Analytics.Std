#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Core;

#endregion

namespace HC.Analytics.Optimisation.Base.Constraints
{
    /// <summary>
    ///   ConstraintsClass:
    ///   This class hold reference to the constraints 
    ///   considered in an optimisation problem.
    ///   Check that the set of constraints are in bounds.
    /// </summary>
    [Serializable]
    public class ConstraintClass : IDisposable
    {
        #region Properties

        /// <summary>
        ///   list of constraints
        /// </summary>
        public List<IConstraint> ListConstraints { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        public ConstraintClass()
        {
            // initialize constraint list
            ListConstraints = new List<IConstraint>();
        }

        #endregion

        #region Public

        /// <summary>
        ///   Add new constraint
        /// </summary>
        /// <param name = "constraint">
        ///   Constraint
        /// </param>
        public void AddConstraint(IConstraint constraint)
        {
            ListConstraints.Add(constraint);
        }

        /// <summary>
        ///   Evaluate constraints
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns></returns>
        public bool CheckConstraints(
            Individual individual)
        {
            // evaluate constraints
            foreach (IConstraint constraint in ListConstraints)
            {
                if (!constraint.CheckConstraint(individual))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Evaluate constraints.
        ///   Sum up constraint values
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns>
        ///   Constraint value
        /// </returns>
        public double EvaluateConstraints(Individual individual)
        {
            var dblTotalValue = 0.0;
            // evaluate constraints
            foreach (IConstraint constraint in ListConstraints)
            {
                dblTotalValue += constraint.EvaluateConstraint(individual);
            }
            return dblTotalValue;
        }

        #endregion

        ~ConstraintClass()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if(ListConstraints != null)
            {
                for (int i = 0; i < ListConstraints.Count; i++)
                {
                    ListConstraints[i].Dispose();
                }
            }
        }
    }
}
