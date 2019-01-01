#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Reproduction
{
    /// <summary>
    ///   Reproduction interface
    /// </summary>
    public interface IReproduction : IDisposable
    {
        #region Properties

        /// <summary>
        ///   Likelihood of selecting current reproduction operator
        /// </summary>
        double ReproductionProb { get; set; }

        HeuristicProblem HeuristicProblem { get; }

        #endregion

        #region Methods

        /// <summary>
        ///   Reproducte individual
        /// </summary>
        /// <returns>
        ///   New individual
        /// </returns>
        Individual DoReproduction();

        /// <summary>
        ///   Cluster individual
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        void ClusterInstance(
            Individual individual);

        #endregion
    }
}
