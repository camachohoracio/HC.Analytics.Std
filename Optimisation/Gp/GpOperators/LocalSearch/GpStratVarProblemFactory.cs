#region

using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch
{
    public class GpStratVarProblemFactory : MixedHeurPrblmFctGeneric
    {
        #region Members

        private readonly int m_intIterations;
        private readonly int m_intPopulationSize;
        private readonly int m_intThreads;

        #endregion

        #region Constructor

        public GpStratVarProblemFactory(
            int intThreads,
            int intPopulationSize,
            int intIterations,
            int intVarCountInt,
            int intVarCountContinuous,
            AbstractGpVarObjFunction gpVarObjFunction) :
                base(gpVarObjFunction,
                     intVarCountInt,
                     intVarCountContinuous,
                     0,
                     null,
                     null)
        {
            m_intThreads = intThreads;
            m_intPopulationSize = intPopulationSize;
            m_intIterations = intIterations;
        }

        #endregion

        public override HeuristicProblem BuildProblem()
        {
            var heuristicProblem = base.BuildProblem();

            heuristicProblem.Threads = m_intThreads;
            heuristicProblem.PopulationSize = m_intPopulationSize;
            heuristicProblem.Iterations = m_intIterations;

            //
            // set problem parameters
            //
            //SetSubProblParams(
            //    HeuristicProblInt_);
            //SetSubProblParams(
            //    HeuristicProblDbl_);

            return heuristicProblem;
        }

        //private void SetSubProblParams(
        //    HeuristicProblem heuristicProblem)
        //{
        //    if (heuristicProblem != null)
        //    {
        //        heuristicProblem.PopulationSize = m_intPopulationSize;
        //        heuristicProblem.Threads = m_intThreads;
        //        heuristicProblem.Iterations = m_intIterations;
        //    }
        //}
    }
}
