#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    /// <summary>
    ///   Nelder-Mead Local search
    /// </summary>
    [Serializable]
    public abstract class AbstractLocalSearchNm : AbstractLocalSearch
    {
        #region Members

        private readonly object m_nmCounterLockObject = new object();

        /// <summary>
        ///   Number of Nelder-Mead solvers
        /// </summary>
        private int m_intNmCounter;

        protected AbstractNmPopulationGenerator m_nmPopulationGenerator;
        private int m_intNmLimit;

        #endregion

        #region Constructors

        protected AbstractLocalSearchNm(
            HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            LoadNmLimit();
        }

        #endregion

        private  void LoadNmLimit()
        {
            int intThreads;
            if (m_heuristicProblem.Threads == 0)
            {
                intThreads = int.MaxValue;
            }
            else
            {
                intThreads = m_heuristicProblem.Threads;
                intThreads = intThreads == 1 ? 2 : intThreads;
            }
            m_intNmLimit = Math.Min(intThreads, NmConstants.NM_INSTANCES);
        }

        public override void DoLocalSearch(Individual individual)
        {
            if (!ValidateNmInstances())
            {
                return;
            }

            lock (m_nmCounterLockObject)
            {
                m_intNmCounter++;
            }

            m_intLocaSearchIterations = 0;

            //
            // run Nelder-Mead algorithm
            //
            var nmSolver = LoadNmSolver();

            nmSolver.UpdateProgress +=
                UpdateProgressBar;
            nmSolver.OnImprovementFound +=
                NmSolverOnOnImprovementFound;
            nmSolver.Solve();

            var bestNmSolution = nmSolver.GetBestSolution();
            //
            // if imrovement found, the cluster solution
            //
            if (m_heuristicProblem.Population.GetIndividualFromPopulation(
                m_heuristicProblem,
                0).Fitness <
                bestNmSolution.Fitness)
            {
                bestNmSolution.Clone(m_heuristicProblem).Evaluate(
                    false,
                    false,
                    true,
                    m_heuristicProblem);
            }
            lock (m_nmCounterLockObject)
            {
                m_intNmCounter--;
            }
            nmSolver.Dispose();
        }

        /// <summary>
        ///   Improvement found by Nelder-Mead
        /// </summary>
        private void NmSolverOnOnImprovementFound(NmSolver nmSolver, Individual bestIndividual)
        {
            m_intLocaSearchIterations = 0;
            InvokeImprovementFound();
        }

        /// <summary>
        ///   Validate conditions for NM the solver
        /// </summary>
        public bool ValidateNmSolver(int intLocaSearchIterations)
        {
            if (!ValidateNmInstances())
            {
                return false;
            }

            if (intLocaSearchIterations < 10)
            {
                return false;
            }

            return true;
        }

        private bool ValidateNmInstances()
        {
            if (m_intNmCounter > m_intNmLimit)
            {
                return false;
            }

            lock (m_nmCounterLockObject)
            {
                if (m_intNmCounter > m_intNmLimit)
                {
                    return false;
                }
                return true;
            }
        }

        #region Abstract Methods

        protected abstract NmSolver LoadNmSolver();

        protected abstract double GetChromosomeValue(
            Individual individual,
            int intIndex);

        protected abstract void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract double[] GetChromosomeCopy(
            Individual individual);

        protected abstract double GetMaxChromosomeValue(int intIndex);

        protected abstract Individual BuildIndividual(double[] dblChromosomeArr, double dblFitness);

        #endregion
    }
}
