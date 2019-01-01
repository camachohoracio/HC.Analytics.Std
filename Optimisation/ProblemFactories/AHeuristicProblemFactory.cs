#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Helpers;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Operators.PopulationClasses;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Analytics.Optimisation.Binary.Operators;
using HC.Analytics.Optimisation.Binary.Operators.LocalSearch;
using HC.Analytics.Optimisation.Binary.Operators.Reproduction;
using HC.Analytics.Optimisation.Continuous.Operators;
using HC.Analytics.Optimisation.Continuous.Operators.LocalSearch;
using HC.Analytics.Optimisation.Integer.Operators;
using HC.Analytics.Optimisation.Integer.Operators.LocalSearch;
using HC.Analytics.Optimisation.MixedSolvers;
using HC.Core.Exceptions;
using HC.Core.Logging;
using Constants = HC.Analytics.Optimisation.Base.OptimisationConstants;

#endregion

namespace HC.Analytics.Optimisation.ProblemFactories
{
    public abstract class AHeuristicProblemFactory : IHeuristicProblemFactory
    {
        #region Members

        protected IHeuristicObjectiveFunction m_objectiveFunction;
        private readonly EnumOptimimisationPoblemType m_problemType;
        protected AbstractGpBridge m_gpBridge;
        protected GpOperatorsContainer m_gpOperatorsContainer;

        #endregion

        #region Constructor
        
        public bool DoPublish { get; set; }

        public AHeuristicProblemFactory(
            EnumOptimimisationPoblemType problemType,
            IHeuristicObjectiveFunction objectiveFunction) : this(problemType,
            objectiveFunction,
            null,
            null,
            false)
        {
        }

        public AHeuristicProblemFactory(
            EnumOptimimisationPoblemType problemType,
            IHeuristicObjectiveFunction objectiveFunction,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpBridge gpBridge,
            bool blnDoPublish)
        {
            m_problemType = problemType;
            m_objectiveFunction = objectiveFunction;
            m_gpOperatorsContainer = gpOperatorsContainer;
            m_gpBridge = gpBridge;
            DoPublish = blnDoPublish;
        }

        #endregion

        #region IHeuristicProblemFactory Members

        public virtual HeuristicProblem BuildProblem()
        {
            return BuildProblem(
                string.Empty,
                string.Empty);
        }

        #endregion

        public virtual HeuristicProblem BuildProblem(
            string strProblemName,
            string strGuiNodeName)
        {
            try
            {
                var heuristicProblem =
                    new HeuristicProblem(
                        m_problemType,
                        strProblemName,
                        strGuiNodeName)
                        {
                            ObjectiveFunction = m_objectiveFunction,
                            DoPublish = DoPublish
                        };

                //
                // crete population
                //
                var population = new Population(heuristicProblem);
                heuristicProblem.Population = population;

                //
                // Create reproduction operator
                //
                BuildReproduction(heuristicProblem);

                //
                // Build guided convergence
                //
                BuildGuidedConvergence(heuristicProblem);

                //
                // create initial population
                //
                IInitialPopulation initialPopulation = new RandomInitialPopulation(
                    heuristicProblem);
                heuristicProblem.InitialPopulation = initialPopulation;
                if(!string.IsNullOrEmpty(strProblemName))
                {
                    heuristicProblem.ProblemName = strProblemName;
                }

                //
                // create solver
                //
                var solver = new EvolutionarySolver(heuristicProblem);

                heuristicProblem.Solver = solver;

                if (m_problemType == EnumOptimimisationPoblemType.CONTINUOUS)
                {
                    SetContinuousDefaultParams(heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.INTEGER)
                {
                    SetIntegerDefaultParams(heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.BINARY)
                {
                    SetBinaryDefaultParams(heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.MIXED)
                {
                    SetMixedDefaultParams(heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.GENETIC_PROGRAMMING)
                {
                    //
                    // set bridge
                    //
                    heuristicProblem.GpBridge = m_gpBridge;

                    SetGpDefaultParams(heuristicProblem);
                }

                //
                // set local searh operator
                //
                LoadLocalSearch(heuristicProblem);

                if (m_problemType != EnumOptimimisationPoblemType.MIXED)
                {
                    //
                    // Set default individual factory
                    //
                    heuristicProblem.IndividualFactory =
                        new IndividualFactory(
                            heuristicProblem);
                }


                //
                // set number of threads
                //
                heuristicProblem.Threads = Constants.INT_THREADS;

                heuristicProblem.DoClusterSolution = true;


                return heuristicProblem;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private void BuildGuidedConvergence(
            HeuristicProblem heuristicProblem)
        {
            //
            // create guided convergence
            //
            AbstractGuidedConvergence guidedConvergence = null;
            if (m_problemType == EnumOptimimisationPoblemType.CONTINUOUS)
            {
                guidedConvergence = new GuidedConvergenceDbl(heuristicProblem);
            }
            else if (m_problemType == EnumOptimimisationPoblemType.INTEGER)
            {
                guidedConvergence = new GuidedConvergenceInt(heuristicProblem);
            }
            else if (m_problemType == EnumOptimimisationPoblemType.BINARY)
            {
                guidedConvergence = new GuidedConvergenceBln(heuristicProblem);
            }
            heuristicProblem.GuidedConvergence = guidedConvergence;
        }

        private void LoadLocalSearch(
            HeuristicProblem heuristicProblem)
        {
            //
            // Set local search for non multi objective problems
            //
            ILocalSearch localSearch = null;
            if (m_objectiveFunction.ObjectiveFunctionType !=
                ObjectiveFunctionType.MULTI_OBJECTIVE_FUNCT &&
                m_objectiveFunction.ObjectiveFunctionType !=
                ObjectiveFunctionType.MIXED)
            {
                if (m_problemType == EnumOptimimisationPoblemType.CONTINUOUS)
                {
                    localSearch = new LocalSearchStdDbl(
                        heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.BINARY)
                {
                    localSearch = new LocalSearchStdBln(
                        heuristicProblem);
                }
                else if (m_problemType == EnumOptimimisationPoblemType.INTEGER)
                {
                    localSearch = new LocalSearchStdInt(
                        heuristicProblem);
                }

                heuristicProblem.LocalSearch = localSearch;
                heuristicProblem.DoLocalSearch = true;
                heuristicProblem.LocalSearchProb =
                    OptimisationConstants.DBL_LOCAL_SEARCH;
            }
        }

        private void BuildReproduction(
            HeuristicProblem heuristicProblem)
        {
            //
            // create reproduction
            //
            IReproduction reproduction = null;
            if (m_problemType == EnumOptimimisationPoblemType.CONTINUOUS)
            {
                reproduction = new ReproductionDblStd(
                    heuristicProblem);
            }
            else if (m_problemType == EnumOptimimisationPoblemType.INTEGER)
            {
                reproduction = ReproductionFactoryInt.BuildReproductionInt(
                    heuristicProblem);
            }
            else if (m_problemType == EnumOptimimisationPoblemType.BINARY)
            {
                reproduction = ReproductionFactoryBln.BuildReproductionBln(
                    heuristicProblem);
            }
            else if (m_problemType == EnumOptimimisationPoblemType.MIXED)
            {
                //
                // Do nothing
                // Reproduction should be loaded in a later stage
                //
            }
            else if (m_problemType == EnumOptimimisationPoblemType.GENETIC_PROGRAMMING)
            {
                //
                // Do nothing
                // Reproduction should be loaded in a later stage
                //
            }
            else
            {
                throw new HCException("Error. Problem type not implemented");
            }
            heuristicProblem.Reproduction = reproduction;
        }

        private static void SetContinuousDefaultParams(
            HeuristicProblem heuristicProblem)
        {
            ValidateIterations(heuristicProblem);

            //
            // set convergence
            //
            if (heuristicProblem.ObjectiveFunction.VariableCount <=
                Continuous.Constants.INT_SMALL_PROBLEM_DE &&
                heuristicProblem.Convergence == 0 &&
                heuristicProblem.ObjectiveCount == 1)
            {
                heuristicProblem.Convergence =
                    Continuous.Constants.INT_DE_SMALL_CONVERGENCE;
            }
            else
            {
                heuristicProblem.Convergence =
                    Continuous.Constants.INT_DE_CONVERGENCE;
            }

            ValidatePopulation(heuristicProblem);

            heuristicProblem.Solver.SetSolverName("Continuous Genetic Algorithm Solver");
        }

        private static void SetBinaryDefaultParams(
            HeuristicProblem heuristicProblem)
        {
            ValidateIterations(heuristicProblem);

            //
            // set convergence
            //
            if (heuristicProblem.ObjectiveFunction.VariableCount <=
                Constants.INT_SMALL_PROBLEM_GA)
            {
                heuristicProblem.Convergence =
                    Constants.INT_GA_SMALL_CONVERGENCE;
            }
            else
            {
                heuristicProblem.Convergence =
                    Constants.INT_GA_CONVERGENCE;
            }

            ValidatePopulation(heuristicProblem);

            heuristicProblem.Solver.SetSolverName("Binary Genetic Algorithm Solver");
        }

        private static void SetIntegerDefaultParams(
            HeuristicProblem heuristicProblem)
        {
            ValidateIterations(heuristicProblem);

            //
            // set convergence
            //
            if (heuristicProblem.ObjectiveFunction.VariableCount <=
                Constants.INT_SMALL_PROBLEM_GA)
            {
                heuristicProblem.Convergence =
                    Constants.INT_GA_SMALL_CONVERGENCE;
            }
            else
            {
                heuristicProblem.Convergence =
                    Constants.INT_GA_CONVERGENCE;
            }

            ValidatePopulation(heuristicProblem);

            heuristicProblem.Solver.SetSolverName("Integer Genetic Algorithm Solver");
        }

        private static void SetMixedDefaultParams(
            HeuristicProblem heuristicProblem)
        {
            ValidateIterations(heuristicProblem);
            //
            // set convergence
            //
            if (heuristicProblem.ObjectiveFunction.VariableCount <=
                Constants.INT_SMALL_PROBLEM_GA)
            {
                heuristicProblem.Convergence =
                    MixedSolversConstants.INT_GA_SMALL_CONVERGENCE;
            }
            else
            {
                heuristicProblem.Convergence =
                    MixedSolversConstants.INT_GA_CONVERGENCE;
            }

            ValidatePopulation(heuristicProblem);

            heuristicProblem.Solver.SetSolverName("Mixed Genetic Algorithm Solver");
        }

        private static void SetGpDefaultParams(
            HeuristicProblem heuristicProblem)
        {
            //
            // set iterations
            //
            heuristicProblem.Iterations =
                Gp.Constants.INT_GP_ITERATIONS;
            //
            // set convergence
            //
            heuristicProblem.Convergence =
                Gp.Constants.INT_GP_CONVERGENCE;

            ValidatePopulation(heuristicProblem);

            heuristicProblem.Solver.SetSolverName("Genetic Programming Algorithm");
        }

        private static void ValidatePopulation(
            HeuristicProblem heuristicProblem)
        {
            //
            // set population
            //
            if (heuristicProblem.PopulationSize == 0)
            {
                heuristicProblem.PopulationSize =
                    Constants.INT_POPULATION_SIZE;
            }
        }

        private static void ValidateIterations(
            HeuristicProblem heuristicProblem)
        {
            //
            // set iterations
            //
            if (heuristicProblem.Iterations == 0)
            {
                heuristicProblem.Iterations =
                    OptimizationHelper.GetHeuristicSolverIterations(
                        heuristicProblem.VariableCount);
            }
        }

    }
}
