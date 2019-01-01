#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    [Serializable]
    public abstract class AbstractLocalSearch : ILocalSearch
    {
        #region Properties

        public double LocalSearchProb { get; set; }

        #endregion

        #region Members

        protected double m_dblBestFitness;
        protected HeuristicProblem m_heuristicProblem;
        protected int m_intLocaSearchIterations;

        #endregion

        #region Events

        public event UpdateLsProgressEventHandler UpdateProgressEvent;

        public event LsImprovementFound ImprovementFoundEvent;

        #endregion

        #region Constructors

        public AbstractLocalSearch(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region Abstract Methods

        public abstract void DoLocalSearch(Individual individual);

        #endregion

        #region Protected

        /// <summary>
        ///   Update progress bar
        /// </summary>
        /// <param name = "intProgress">
        ///   Progress value
        /// </param>
        /// <param name = "strMessage">
        ///   Progress message
        /// </param>
        public void UpdateProgressBar(int intProgress, string strMessage)
        {
            if (UpdateProgressEvent != null)
            {
                if (UpdateProgressEvent.GetInvocationList().Length > 0)
                {
                    UpdateProgressEvent(intProgress, strMessage);
                }
            }
        }

        #endregion

        #region Invoke Events

        /// <summary>
        ///   Call this method if an improvent is found
        /// </summary>
        protected void InvokeImprovementFound()
        {
            if (ImprovementFoundEvent != null)
            {
                if (ImprovementFoundEvent.GetInvocationList().Length > 0)
                {
                    ImprovementFoundEvent();
                }
            }
        }

        #endregion

        public virtual void Dispose()
        {
        }
    }
}
