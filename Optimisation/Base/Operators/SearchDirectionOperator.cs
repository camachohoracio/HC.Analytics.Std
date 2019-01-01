#region

using System;
using System.Linq;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators
{
    /// <summary>
    ///   This class provides the likelihood of the search direction the algorithm
    ///   should aim for.
    /// 
    ///   The search direction, depending on the constraints and the objective function,
    ///   allows the algorithm to find promissing solutions in the neighbourhood or
    ///   repair solutions for contrained problems.
    /// 
    ///   The algorithm counts the number of time an improvement has been found
    ///   in a certain direction. The frequency is used for the calculation of
    ///   a likelihood value. The method then draws a random number which is used 
    ///   for the decision.
    /// </summary>
    public class SearchDirectionOperator
    {
        #region Members

        private readonly int[] m_intBackwardCounterArr;

        /// <summary>
        ///   Forward counter. Add weights to chormosomes in order to improve the
        ///   fitness
        /// </summary>
        private readonly int[] m_intForwardCounterArr;

        protected double m_dblSearchDirectionLikelihood;

        private HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public SearchDirectionOperator(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_dblSearchDirectionLikelihood = OptimisationConstants.SEARCH_DIRECTION_LIKELIHOOD;
            m_intForwardCounterArr = new int[heuristicProblem.VariableCount];
            m_intBackwardCounterArr = new int[heuristicProblem.VariableCount];
        }

        #endregion

        #region Public

        public bool CheckGoForward(
            int intIndex,
            RngWrapper rng)
        {
            double dblTotalWeight =
                m_intBackwardCounterArr[intIndex] + m_intForwardCounterArr[intIndex];
            double dblForwardCount = m_intForwardCounterArr[intIndex];
            double dblBackwardCount = m_intBackwardCounterArr[intIndex];

            // if there are not enough statistics, 
            // then check the overall likelihood
            if (dblForwardCount == 0 && dblBackwardCount == 0)
            {
                return CheckGoForward(rng);
            }

            return CheckGoForward0(
                rng,
                dblTotalWeight,
                dblForwardCount);
        }

        public bool CheckGoForward(
            RngWrapper rng)
        {
            double dblForwardCount = m_intForwardCounterArr.Sum();
            var dblTotalWeight =
                m_intBackwardCounterArr.Sum() + dblForwardCount;

            return CheckGoForward0(
                rng,
                dblTotalWeight,
                dblForwardCount);
        }

        public void SetImprovementCounter(
            int intIndex,
            bool blnGoForward)
        {
            //
            // set global improvement counters
            //

            if (blnGoForward)
            {
                m_intForwardCounterArr[intIndex]++;
            }
            else
            {
                m_intBackwardCounterArr[intIndex]++;
            }
        }

        #endregion

        #region Private

        private bool CheckGoForward0(
            RngWrapper rng,
            double dblTotalWeight,
            double dblForwardCount)
        {
            if (dblTotalWeight == 0)
            {
                //
                // select randomly 50/50 chance to go forward or backward
                //
                return rng.NextBln();
            }
            var dblRandom = rng.NextDouble();
            //
            // calculate forward likelihood
            //
            var dblForwardLikelihood = dblForwardCount/dblTotalWeight;

            dblForwardLikelihood =
                Math.Min(
                    m_dblSearchDirectionLikelihood,
                    dblForwardLikelihood);
            dblForwardLikelihood =
                Math.Max(
                    1.0 - m_dblSearchDirectionLikelihood,
                    dblForwardLikelihood);

            return dblRandom < dblForwardLikelihood;
        }

        #endregion
    }
}
