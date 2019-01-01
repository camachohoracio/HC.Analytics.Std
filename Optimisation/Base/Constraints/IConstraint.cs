#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Constraints
{
    public interface IConstraint : IDisposable
    {
        #region Properties

        /// <summary>
        ///   The type of inequality
        /// </summary>
        InequalityType Inequality { get; set; }

        /// <summary>
        ///   Cosntraint boundary (limit)
        /// </summary>
        double Boundary { get; set; }

        #endregion

        #region Interface Methods

        /// <summary>
        ///   Check if the constraint is in bounds
        ///   Returns true if the constraint is in bound. 
        ///   False otherwise
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns>
        ///   Returns true if the constraint is in bound. 
        ///   False otherwise
        /// </returns>
        bool CheckConstraint(Individual individual);

        /// <summary>
        ///   Evaluate constraint value.
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns>
        ///   Constraint value
        /// </returns>
        double EvaluateConstraint(Individual individual);

        #endregion
    }
}
