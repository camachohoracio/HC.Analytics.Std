#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Reproduction
{
    [Serializable]
    public abstract class AbstractReproduction : IReproduction
    {
        #region Properties

        public double ReproductionProb
        {
            get { return m_dblReproductionProb; }
            set { m_dblReproductionProb = value; }
        }

        public HeuristicProblem HeuristicProblem
        {
            get { return m_heuristicProblem; }
        }

        #endregion

        #region Members

        protected double m_dblReproductionProb;
        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public AbstractReproduction(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region IReproduction Members

        public abstract Individual DoReproduction();

        public abstract void ClusterInstance(Individual individual);

        #endregion

        public virtual void Dispose()
        {
        }
    }
}
