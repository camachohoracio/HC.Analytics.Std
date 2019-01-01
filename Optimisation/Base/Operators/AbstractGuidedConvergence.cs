#region

using System;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators
{
    [Serializable]
    public abstract class AbstractGuidedConvergence : IDisposable
    {
        #region Members

        protected readonly int m_intVariableCount;
        protected double[] m_gcProbabilityArray;
        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public AbstractGuidedConvergence(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_heuristicProblem = heuristicProblem;
            m_intVariableCount = m_heuristicProblem.VariableCount;
            InitializeGcProbabilities();
        }

        #endregion

        #region Protected Methods

        protected bool CheckPopulation()
        {
            if (m_heuristicProblem.Population.LargePopulationSize == 0)
            {
                return false;
            }
            return m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                m_heuristicProblem,
                m_heuristicProblem.Population.LargePopulationSize - 1) != null;
        }

        public double GetGcProb(int intIndex)
        {
            if (m_gcProbabilityArray == null)
            {
                InitializeGcProbabilities();
            }

            if (m_gcProbabilityArray == null)
            {
                return 0.0;
            }

            return m_gcProbabilityArray[intIndex];
        }

        #endregion

        #region Abstract Methods

        public abstract void UpdateGcProbabilities(HeuristicProblem heuristicProblem);

        /// <summary>
        ///   This method generates a random number value according 
        ///   to the guided convergence probabilities
        /// </summary>
        /// <param name = "intIndex"></param>
        /// <returns></returns>
        public abstract double DrawGuidedConvergenceValue(
            int intIndex,
            RngWrapper rng);

        protected abstract void InitializeGcProbabilities();

        #endregion

        public void Dispose()
        {
            m_gcProbabilityArray = null;
        }

        ~AbstractGuidedConvergence()
        {
            Dispose();
        }
    }
}
