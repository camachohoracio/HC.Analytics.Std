#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers;
using HC.Analytics.Optimisation.MixedSolvers.DummyProblemFactories;
using HC.Core.Events;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public abstract class AOptPrblmFctry
    {
        #region Properties

        public IOptInstFctry OptInstanceFactory { get; set; }

        #endregion

        #region Members

        protected OptChromosomeFactory m_optChromosomeFactory;
        protected AOptParams m_optParams;

        #endregion

        private ConstraintClass LoadConstraints(
            IOptInstFctry instFctry)
        {
            string[] constraints;
            if (!m_optParams.OptimisationParams.TryGetStringArr(
                EnumGenericSolver.OptConstraints,
                out constraints))
            {
                return null;
            }

            ConstraintClass constraintClass = null;
            foreach (string strConstraint in constraints)
            {
                if (string.IsNullOrEmpty(strConstraint))
                {
                    continue;
                }

                if (constraintClass == null)
                {
                    constraintClass = new ConstraintClass();
                }

                var inequalityType =
                    MathHelper.ParseInequalitySymbol(strConstraint);
                var strTokens = strConstraint.Split(@"<=>".ToCharArray());
                var strParam1 = strTokens[0];
                var strParam2 = string.Empty;

                double dblBoundary;
                if (!double.TryParse(
                    strTokens.Last(),
                    out dblBoundary))
                {
                    //
                    // get param boundary
                    //
                    strParam2 = strTokens.Last();
                    dblBoundary = double.NaN;
                }

                var optConstraint = new OptConstraint(
                    instFctry,
                    m_optChromosomeFactory,
                    strParam1,
                    strParam2,
                    inequalityType,
                    dblBoundary);
                constraintClass.AddConstraint(optConstraint);
            }

            return constraintClass;
        }

        public HeuristicProblem BuildProblem(
            string strProblemName,
            string strGuiNodeName)
        {
            int intPopulation;
            if (!m_optParams.OptimisationParams.TryGetIntValue(
                EnumGenericSolver.OptPopulation,
                out intPopulation))
            {
                intPopulation = OptConstants.POPULATION_SIZE;
            }

            //
            // load dummy problems
            //
            var dummyProblems = LoadDummyProblems(intPopulation);

            var constraints = LoadConstraints(
                OptInstanceFactory);

            //
            // create optimisation problem
            //
            MixedHeurPrblmFctGeneric mixedHeurPrblmFctGeneric;
            HeuristicProblem heuristicProblem;
            if (constraints != null)
            {
                //
                // load objective
                //
                var objectives =
                    LoadAllObjectives(OptInstanceFactory,
                                      true);

                HCException.ThrowIfTrue(objectives.Count > 1,
                        "Multiple objectives not supported for a constrained problem");


                //
                // load a single-objective problem
                //
                mixedHeurPrblmFctGeneric =
                    new MixedHeurPrblmFctGeneric(
                        objectives[0],
                        dummyProblems,
                        constraints);
                heuristicProblem = mixedHeurPrblmFctGeneric.BuildProblem();
            }
            else
            {
                //
                // load multiple objectives
                //
                var objectives =
                    LoadAllObjectives(OptInstanceFactory,
                                      false);

                if (objectives == null ||
                    objectives.Count == 0)
                {
                    throw new Exception("No objectives found");
                }

                var heuristicMultiObjectiveFunction =
                    new HeuristicMultiObjectiveFunction(objectives);

                mixedHeurPrblmFctGeneric =
                    new MixedHeurPrblmFctGeneric(
                        heuristicMultiObjectiveFunction,
                        m_optParams.IntParams.Count,
                        m_optParams.DblParams.Count,
                        m_optParams.BlnParams.Count,
                        constraints,
                        m_optParams.GetIntScaleFactors());
                heuristicProblem = mixedHeurPrblmFctGeneric.BuildProblem(
                    strProblemName,
                    strGuiNodeName);
            }

            //
            // do not do a local search for the initial population
            //
            heuristicProblem.InitialPopulation.DoLocalSearch = false;

            HCException.ThrowIfTrue(m_optParams.IntParams.Count == 0 &&
                m_optParams.DblParams.Count == 0 &&
                m_optParams.BlnParams.Count == 0,
                "Invalid variable count");


            //
            // get optimisation params
            //

            int intThreads = m_optParams.OptimisationParams.GetIntValue(
                EnumGenericSolver.OptThreads);
            int intIterations = m_optParams.OptimisationParams.GetIntValue(
                EnumGenericSolver.OptIterations);
            int intConvergence = m_optParams.OptimisationParams.GetIntValue(
                EnumGenericSolver.OptConvergence);
            string strOptimisationEngine = m_optParams.OptimisationParams.GetStrValue(
                EnumGenericSolver.OptEngineName);


            if (strOptimisationEngine.Equals("Random"))
            {
                heuristicProblem.Reproduction = new RandomReproduction(
                    heuristicProblem);
            }

            //
            // set local search value between 0 and 1 (small value runs quicker)
            //
            heuristicProblem.LocalSearchProb = 0.2;
            heuristicProblem.RepairProb = 0.2;
            heuristicProblem.Threads = intThreads;
            heuristicProblem.Iterations = intIterations;
            heuristicProblem.PopulationSize = intPopulation;
            heuristicProblem.Convergence = intConvergence;
            heuristicProblem.Solver.OnUpdateProgress +=
                SolverOnUpdateProgress;

            heuristicProblem.Reproduction = new OptReproduction(
                heuristicProblem,
                heuristicProblem.InnerProblemList,
                heuristicProblem.GpOperatorsContainer,
                m_optChromosomeFactory);
            heuristicProblem.IndividualFactory = new OptIndividualFactory(
                heuristicProblem,
                heuristicProblem.InnerProblemList,
                m_optChromosomeFactory);

            OptInstanceFactory.HeuristicProblem = heuristicProblem;
            return heuristicProblem;
        }

        private List<IHeuristicObjectiveFunction> LoadAllObjectives(
            IOptInstFctry optInstFctry,
            bool blnMaximiseModel)
        {
            var objectiveList = m_optParams.OptimisationParams.GetStringArr(
                EnumGenericSolver.OptObjectives);

            var objectiveFunctions =
                new List<IHeuristicObjectiveFunction>();

            double dblMaximiseFactor = blnMaximiseModel ? -1 : 1;


            foreach (string strObjective in objectiveList)
            {
                var objectiveTokens =
                    strObjective.Split('|');

                double dblMultiplier;
                if (objectiveTokens[1].Equals("Maximise"))
                {
                    dblMultiplier = -1 * dblMaximiseFactor;
                }
                else if (objectiveTokens[1].Equals("Minimise"))
                {
                    dblMultiplier = dblMaximiseFactor;
                }
                else
                {
                    HCException.Throw("Objective type not found");
                    dblMultiplier = double.NaN;
                }

                var objFunction =
                    new OptObjFunct(
                        optInstFctry,
                        m_optChromosomeFactory,
                        objectiveTokens[0],
                        dblMultiplier,
                        m_optParams);
                objectiveFunctions.Add(objFunction);
            }

            return objectiveFunctions;
        }

        protected List<HeuristicProblem> LoadDummyProblems(
            int intPopulation)
        {
            var dummyHeuristicProblems =
                new List<HeuristicProblem>();
            if (m_optParams.IntParams != null && m_optParams.IntParams.Count > 0)
            {
                var heuristicProblFactDummy =
                    new HeuristicProblFactDummy(
                        EnumOptimimisationPoblemType.INTEGER,
                        m_optParams.IntParams.Count);

                var heuristicProblem =
                    heuristicProblFactDummy.BuildProblem();
                heuristicProblem.VariableRangesIntegerProbl =
                    m_optParams.GetIntScaleFactors();
                heuristicProblem.PopulationSize = intPopulation;
                dummyHeuristicProblems.Add(heuristicProblem);
            }
            if (m_optParams.DblParams != null && m_optParams.DblParams.Count > 0)
            {
                var heuristicProblem =
                    new HeuristicProblFactDummy(
                        EnumOptimimisationPoblemType.CONTINUOUS,
                        m_optParams.DblParams.Count).BuildProblem();
                heuristicProblem.PopulationSize = intPopulation;
                dummyHeuristicProblems.Add(heuristicProblem);
            }
            if (m_optParams.BlnParams != null && m_optParams.BlnParams.Count > 0)
            {
                var heuristicProblem =
                    new HeuristicProblFactDummy(
                        EnumOptimimisationPoblemType.BINARY,
                        m_optParams.BlnParams.Count).BuildProblem();
                heuristicProblem.PopulationSize = intPopulation;
                dummyHeuristicProblems.Add(heuristicProblem);
            }
            return dummyHeuristicProblems;
        }

        private static void SolverOnUpdateProgress(string strMessage, int intProgress)
        {
            Logger.Log(strMessage);
            SendMessageEvent.OnSendMessage(strMessage, intProgress);
        }
    }
}

