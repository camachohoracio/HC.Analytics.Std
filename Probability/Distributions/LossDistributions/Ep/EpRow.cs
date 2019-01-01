#region

using System;
using HC.Analytics.TimeSeries;

#endregion

namespace HC.Analytics.Probability.Distributions.LossDistributions.Ep
{
    /// <summary>
    /// EpRow:
    /// Contains a point from the exceedance probability (EP) distribution
    /// </summary>
    public class EpRow : IComparable<EpRow>
    {
        #region Properties

        /// <summary>
        /// Get / Set loss threshold
        /// </summary>
        public double LossThreshold { get; set; }

        /// <summary>
        /// Get / Set probability
        /// </summary>
        public double ExceedenceProbability { get; set; }

        #endregion

        #region Private

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dblThreshold">
        /// Loss threshold
        /// </param>
        /// <param name="dblProbability">
        /// Probability
        /// </param>
        public EpRow(double dblThreshold, double dblProbability)
        {
            LossThreshold = dblThreshold;
            ExceedenceProbability = dblProbability;
        }

        #endregion

        #region Public

        /// <summary>
        /// EP rows are sorted by loss threshold
        /// </summary>
        /// <param name="epRow"> EpRow to compare with</param>
        /// <returns></returns>
        public int CompareTo(EpRow epRow)
        {
            double difference =
                LossThreshold - epRow.LossThreshold;
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

        /// <summary>
        /// Returns string description of the current EP row.
        /// </summary>
        /// <returns>
        /// String description of current EP row.
        /// </returns>
        public override string ToString()
        {
            return
                "Loss threshold: " + LossThreshold + Environment.NewLine +
                "ExceedenceProbability: " + ExceedenceProbability;
        }

        #endregion

        public string ToCsvString()
        {
            return TsEventHelper.ToCsvString(this, GetType());
        }
    }
}
