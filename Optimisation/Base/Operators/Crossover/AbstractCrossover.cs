#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Crossover
{
    [Serializable]
    public abstract class AbstractCrossover : ICrossover
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public AbstractCrossover(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region ICrossover Members

        public abstract Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals);

        #endregion
    }
}
