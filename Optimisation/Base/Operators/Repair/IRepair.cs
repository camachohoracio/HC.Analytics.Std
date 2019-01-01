#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Repair
{
    /// <summary>
    ///   Repair solution iterface
    /// </summary>
    public interface IRepair : IDisposable
    {
        #region Interface methods

        /// <summary>
        ///   Do repair
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual to be repaired
        /// </param>
        bool DoRepair(Individual individual);

        void AddRepairOperator(IRepair repair);

        #endregion
    }
}
