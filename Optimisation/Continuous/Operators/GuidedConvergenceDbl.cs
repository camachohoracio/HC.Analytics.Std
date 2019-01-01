#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators
{
    /// <summary>
    ///   Guided convergence probabilities. New solutions are
    ///   generated
    ///   based on the probability array. A probability of
    ///   one means that the variable should be included in 
    ///   the solution. Zero means no likelihood to be included.
    /// </summary>
    [Serializable]
    public class GuidedConvergenceDbl : AbstractGuidedConvergence
    {
        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        public GuidedConvergenceDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        #region Public

        /// <summary>
        ///   Update guided convergence probabilities
        /// </summary>
        public override void UpdateGcProbabilities(HeuristicProblem heuristicProblem)
        {
            if (CheckInitialProbabilities())
            {
                var dblGcProbabilityArray =
                    new double[m_intVariableCount];
                for (var i = 0;
                     i < Math.Min(
                         OptimisationConstants.INT_GM_POPULATION*
                         2,
                         m_heuristicProblem.Population.LargePopulationSize);
                     i++)
                {
                    for (var j = 0; j < m_intVariableCount; j++)
                    {
                        dblGcProbabilityArray[j] +=
                            m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                                m_heuristicProblem,
                                i).GetChromosomeValueDbl(j);
                    }
                }
                for (var j = 0; j < m_intVariableCount; j++)
                {
                    m_gcProbabilityArray[j] =
                        ((1.0 - OptimisationConstants.DBL_GM_LAMBDA)*
                         m_gcProbabilityArray[j]) +
                        (OptimisationConstants.DBL_GM_LAMBDA*
                         dblGcProbabilityArray[j]/
                         ((double) OptimisationConstants.INT_GM_POPULATION*2));
                }
            }
        }

        public override double DrawGuidedConvergenceValue(
            int intIndex,
            RngWrapper rng)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        /// <summary>
        ///   Initialize Guided Mutation probabilities
        /// </summary>
        protected override void InitializeGcProbabilities()
        {
            if (CheckPopulation())
            {
                m_gcProbabilityArray = new double[m_intVariableCount];
                for (var i = 0; i < m_heuristicProblem.Population.LargePopulationSize; i++)
                {
                    for (var j = 0; j < m_intVariableCount; j++)
                    {
                        m_gcProbabilityArray[j] +=
                            m_heuristicProblem.Population.
                                GetIndividualFromLargePopulation(
                                    m_heuristicProblem,
                                    i).
                                GetChromosomeValueDbl(j);
                    }
                }
                for (var j = 0; j < m_intVariableCount; j++)
                {
                    m_gcProbabilityArray[j] /= m_heuristicProblem.Population.LargePopulationSize;
                }
            }
        }

        private bool CheckInitialProbabilities()
        {
            if (m_gcProbabilityArray == null)
            {
                InitializeGcProbabilities();
                return m_gcProbabilityArray != null;
            }

            return true;
        }

        #endregion
    }
}
