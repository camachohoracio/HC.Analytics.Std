#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures
{
    /// <summary>
    ///   Solver results are stored in this class.
    ///   Note: This class in not threadsafe.
    /// </summary>
    public class ResultRow : IComparable<ResultRow>
    {
        #region Properties

        /// <summary>
        ///   Chromosome
        /// </summary>
        public double[] ChromosomeArray { get; set; }

        /// <summary>
        ///   Average annual loss (Aal)
        /// </summary>
        public double Aal { get; set; }

        /// <summary>
        ///   Tail conditional expectation (Tce)
        /// </summary>
        public double Tce { get; set; }

        /// <summary>
        ///   Return period loss (Rpl)
        /// </summary>
        public double Rpl { get; set; }

        /// <summary>
        ///   years to consider for a certain loss
        /// </summary>
        public double ReturnPeriod { get; set; }

        /// <summary>
        ///   return
        /// </summary>
        public double Return { get; set; }

        /// <summary>
        ///   list of exposures
        /// </summary>
        public List<string> ExposureList { get; set; }

        /// <summary>
        ///   initial risk
        /// </summary>
        public double BaseRisk { get; set; }

        /// <summary>
        ///   Standalone risk
        /// </summary>
        public double StandaloneRisk { get; set; }

        /// <summary>
        ///   Marginal risk
        /// </summary>
        public double MarginalRisk { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "dblAal">
        ///   Average Annual Loss value
        /// </param>
        /// <param name = "dblTce">
        ///   Tail Conditional Expectation value
        /// </param>
        /// <param name = "dblRpl">
        ///   Return Period Loss
        /// </param>
        /// <param name = "dblReturnPeriod">
        ///   Return Period (years)
        /// </param>
        /// <param name = "dblFitness">
        ///   Return
        /// </param>
        /// <param name = "policyList">
        ///   List of policies
        /// </param>
        /// <param name = "dblBaseRisk">
        ///   Base risk
        /// </param>
        /// <param name = "dblStandaloneRisk">
        ///   Risk alone for current policy
        /// </param>
        /// <param name = "dblMarginalRisk">
        ///   Marginal Risk
        /// </param>
        /// <param name = "dblChromosomeArray">
        ///   Chromosome
        /// </param>
        public ResultRow(
            double dblAal,
            double dblTce,
            double dblRpl,
            double dblReturnPeriod,
            double dblFitness,
            List<string> policyList,
            double dblBaseRisk,
            double dblStandaloneRisk,
            double dblMarginalRisk,
            double[] dblChromosomeArray)
        {
            Aal = dblAal;
            Tce = dblTce;
            Rpl = dblRpl;
            ReturnPeriod = dblReturnPeriod;
            Return = dblFitness;
            ExposureList = policyList;
            BaseRisk = dblBaseRisk;
            StandaloneRisk = dblStandaloneRisk;
            MarginalRisk = dblMarginalRisk;
            ChromosomeArray = dblChromosomeArray;
        }

        #endregion

        #region Public

        /// <summary>
        ///   compare method used for sorting the results
        /// </summary>
        /// <param name = "o">
        ///   Result object to compare with
        /// </param>
        /// <returns>
        ///   Compare value
        /// </returns>
        public int CompareTo(ResultRow o)
        {
            var difference = Return - o.Return;
            if (difference < 0)
            {
                return 1;
            }
            if (difference > 0)
            {
                return -1;
            }
            return 0;
        }

        #endregion
    }
}
