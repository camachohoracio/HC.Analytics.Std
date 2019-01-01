#region

using System;

#endregion

namespace HC.Analytics.Probability.Distributions.LossDistributions
{
    /// <summary>
    /// Risk Variable.
    /// Stores the marginal and contributorial risks for
    /// a given distribution index.
    /// Note: This class is not threadsafe.
    /// </summary>
    public class RiskVariable : IEquatable<RiskVariable>
    {
        #region Properties

        /// <summary>
        /// Marginal Risk.
        /// Risk increasement by adding a new distribution to an 
        /// existing one
        /// </summary>
        public double MarginalRisk { get; set; }

        /// <summary>
        /// Constributorial risk.
        /// Is the stand-alone risk from a given distribution.
        /// </summary>
        public double ContributorialRisk { get; set; }

        /// <summary>
        /// Distribution index
        /// </summary>
        public int Index { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Risk variable constructor
        /// </summary>
        /// <param name="intIndex">
        /// Distribution index
        /// </param>
        /// <param name="dblMarginalRisk">
        /// Marginal Risk
        /// </param>
        /// <param name="dblContributorialRisk">
        /// Constributorial risk
        /// </param>
        public RiskVariable(
            int intIndex,
            double dblMarginalRisk,
            double dblContributorialRisk)
        {
            Index = intIndex;
            ContributorialRisk = dblContributorialRisk;
            MarginalRisk = dblMarginalRisk;
        }

        #endregion

        #region Public

        /// <summary>
        /// Check if the two variables are equal by
        /// comparing the distribution index.
        /// </summary>
        /// <param name="riskVariable">
        /// Risk variable
        /// </param>
        /// <returns></returns>
        public bool Equals(RiskVariable riskVariable)
        {
            return riskVariable.Index == Index;
        }

        #endregion
    }
}
