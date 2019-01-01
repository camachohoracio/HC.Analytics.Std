#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Operators.MultiObjective;
using HC.Analytics.Optimisation.Base.Operators.PopulationClasses;
using HC.Analytics.Optimisation.Base.Operators.Repair;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Analytics.Optimisation.MixedSolvers.DummyObjectiveFunctions;
using HC.Analytics.Probability.Random;
using HC.Core;
using HC.Core.Events;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Problem
{
    public class HeuristicProblem : IDisposable
    {
        #region Members

        /// <summary>
        ///   Optimisation problem type
        /// </summary>
        private readonly EnumOptimimisationPoblemType m_enumOptimimisationPoblemType;

        /// <summary>
        ///   Constraints
        /// </summary>
        private ConstraintClass m_constraints;

        /// <summary>
        ///   The size of the population. Very large populations tend to converge slower 
        ///   but it is more likely to find the "global solution" if the number of 
        ///   generations is very large.
        ///   Very small populations converge too quickly to a local solution.
        ///   The size of the population is also dictated by the machine phisical memory
        /// </summary>
        private int m_intPopulationSize;

        private int m_intThreads;

        /// <summary>
        ///   Ranks objective functions for multi-objective problems
        /// </summary>
        private MultiObjectiveRanking m_multiObjectiveRanking;

        public double[] VariableRangesIntegerProbl { get; set; }

        #endregion

        #region Properties

        public int CurrentLocalSearchInstances { get; set; }

        public double RepairProb { get; set; }

        public double LocalSearchProb { get; set; }

        public bool Verbose { get; set; }

        public IIndividualFactory IndividualFactory { get; set; }

        public IInitialPopulation InitialPopulation { get; set; }

        public EvolutionarySolver Solver { get; set; }

        public List<HeuristicProblem> InnerProblemList { get; set; }

        public MultiObjectiveRanking MultiObjectiveRanking
        {
            get
            {
                if (m_multiObjectiveRanking == null && ObjectiveCount > 1)
                {
                    m_multiObjectiveRanking = new MultiObjectiveRanking(
                        this);
                }
                return m_multiObjectiveRanking;
            }
        }

        public SelfDescribingTsEvent ProblemStats { get; private set; }

        public int Iterations { get; set; }

        public string ProblemName { get; set; }

        public int Convergence { get; set; }

        public int VariableCount
        {
            get { return ObjectiveFunction == null ? 0 : ObjectiveFunction.VariableCount; }
        }

        public int ObjectiveCount
        {
            get
            {
                if (!IsMultiObjective())
                {
                    return 1;
                }
                return ((HeuristicMultiObjectiveFunction) ObjectiveFunction).ObjectiveCount;
            }
        }

        public IHeuristicObjectiveFunction ObjectiveFunction { get; set; }

        public bool DoCheckConstraints
        {
            get { return m_constraints != null; }
        }

        public ConstraintClass Constraints
        {
            get
            {
                if (m_constraints == null)
                {
                    //
                    // load dummy constraints
                    //
                    m_constraints = new ConstraintClass();
                }
                return m_constraints;
            }
            set { m_constraints = value; }
        }

        public Population Population { get; set; }

        public IReproduction Reproduction { get; set; }

        public IRepair RepairIndividual { get; set; }

        public ILocalSearch LocalSearch { get; set; }

        public AbstractGuidedConvergence GuidedConvergence { get; set; }

        public bool DoLocalSearch { get; set; }

        public bool DoRepairSolution
        {
            get { return RepairIndividual != null; }
        }

        public bool DoClusterSolution { get; set; }

        public int PopulationSize
        {
            get { return m_intPopulationSize; }
            set
            {
                m_intPopulationSize = value;
                if (Population != null)
                {
                    Population.SetPopulationSize();
                }
                // set population to nested problems
                SetPopulationToNestedProblems(
                    value,
                    this);
            }
        }

        public EnumOptimimisationPoblemType EnumOptimimisationPoblemType
        {
            get
            {
                return m_enumOptimimisationPoblemType;
            }
        }

        public int Threads
        {
            get { return m_intThreads; }
            set
            {
                m_intThreads = value;
                if(InnerProblemList != null)
                {
                    foreach (HeuristicProblem heuristicProblem in InnerProblemList)
                    {
                        heuristicProblem.Threads = value;
                    }
                }
            }
        }
        
        public AbstractGpBridge GpBridge { get; set; }

        public GpOperatorsContainer GpOperatorsContainer { get; set; }

        public bool DoPublish { get; set; }

        public string GuiNodeName { get; set; }
        
        #endregion

        #region Constructor

        public HeuristicProblem(
            EnumOptimimisationPoblemType problemType) :
                this(problemType,
                     GetRandomProblemName(problemType),
                    "Optimiser")
        {
        }

        private static string GetRandomProblemName(EnumOptimimisationPoblemType problemType)
        {
            return Guid.NewGuid() + "_" +
                   problemType;
        }

        public HeuristicProblem(
            EnumOptimimisationPoblemType problemType,
            string strProblemName,
            string strGuiNodeName)
        {
            GuiNodeName = strGuiNodeName;
            if (string.IsNullOrEmpty(strProblemName))
            {
                strProblemName = GetRandomProblemName(problemType);
            }
            ProblemName = strProblemName;
            m_enumOptimimisationPoblemType = problemType;
            Verbose = true;
            //
            // cluster class opeartor: Saves explored solutions.
            // This option is enabled by default
            //
            DoClusterSolution = true;

            var strClassName =
                "name_" +
                ProblemName
                    .Replace(";", "_")
                    .Replace(",", "_")
                    .Replace(".", "_")
                    .Replace(":", "_")
                    .Replace("-", "_");
            ProblemStats = new SelfDescribingTsEvent(
                strClassName);

            ProblemStats.SetStrValue(
                EnumHeuristicProblem.ProblemName,
                ProblemName);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return ProblemName;
        }

        ~HeuristicProblem()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                EventHandlerHelper.RemoveAllEventHandlers(this);
                if (Population != null)
                {
                    Population.Dispose();
                }
                if (Solver != null)
                {
                    Solver.Dispose();
                }
                if (InnerProblemList != null)
                {
                    for (int i = 0; i < InnerProblemList.Count; i++)
                    {
                        InnerProblemList[i].Dispose();
                    }
                    InnerProblemList.Clear();
                }
                if (ObjectiveFunction != null)
                {
                    ObjectiveFunction.Dispose();
                }
                if (Constraints != null)
                {
                    Constraints.Dispose();
                }
                if (Reproduction != null)
                {
                    Reproduction.Dispose();
                }
                if (RepairIndividual != null)
                {
                    RepairIndividual.Dispose();
                }
                if (LocalSearch != null)
                {
                    LocalSearch.Dispose();
                }
                if (GuidedConvergence != null)
                {
                    GuidedConvergence.Dispose();
                }
                if (GpBridge != null)
                {
                    GpBridge.Dispose();
                }

                if (GpOperatorsContainer != null)
                {
                    GpOperatorsContainer.Dispose();
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static RngWrapper CreateRandomGenerator()
        {
            return new RngWrapper();
        }

        public bool IsMultiObjective()
        {
            return ObjectiveFunction is HeuristicMultiObjectiveFunction;
        }

        public bool CheckConstraints(
            Individual individual)
        {
            return (m_constraints == null || m_constraints.CheckConstraints(individual));
        }

        public bool ContainsIntegerVariables()
        {
            if (m_enumOptimimisationPoblemType == EnumOptimimisationPoblemType.INTEGER)
            {
                return true;
            }
            return false;
        }

        public bool ContainsContinuousVariables()
        {
            if (m_enumOptimimisationPoblemType == EnumOptimimisationPoblemType.CONTINUOUS)
            {
                return true;
            }
            return false;
        }


        public bool ValidateIntegerProblem()
        {
            if (m_enumOptimimisationPoblemType == EnumOptimimisationPoblemType.INTEGER)
            {
                if (VariableRangesIntegerProbl == null)
                {
                    return false;
                }
                if (VariableRangesIntegerProbl.Length != VariableCount)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Private

        public void PublishGridStats()
        {
            if(!DoPublish)
            {
                return;
            }
            var strProblemName =
                ProblemStats.GetStrValue(
                    EnumHeuristicProblem.ProblemName);
            ProblemStats.Time = DateTime.Now;
            if (!(ObjectiveFunction is ObjectiveFunctionDummy))
            {
                LiveGuiPublisherEvent.PublishGrid(
                    GuiNodeName,
                    strProblemName,
                    EnumHeuristicProblem.SolverParams.ToString(),
                    ProblemStats.GetClassName(),
                    ProblemStats,
                    2,
                    true);
            }
        }

        private static void SetPopulationToNestedProblems(
            int intPopulationSize,
            HeuristicProblem heuristicProblem)
        {
            if (heuristicProblem.InnerProblemList != null &&
                heuristicProblem.InnerProblemList.Count > 0)
            {
                foreach (HeuristicProblem currentHeuristicProblem in 
                    heuristicProblem.InnerProblemList)
                {
                    currentHeuristicProblem.PopulationSize = intPopulationSize;
                }
            }
        }

        #endregion


    }
}
