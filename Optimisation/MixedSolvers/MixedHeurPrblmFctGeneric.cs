#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.Repair;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Binary.Operators;
using HC.Analytics.Optimisation.Continuous.Operators;
using HC.Analytics.Optimisation.Integer.Operators;
using HC.Analytics.Optimisation.MixedSolvers.DummyObjectiveFunctions;
using HC.Analytics.Optimisation.MixedSolvers.DummyProblemFactories;
using HC.Analytics.Optimisation.MixedSolvers.Operators;
using HC.Analytics.Optimisation.ProblemFactories;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers
{
    public class MixedHeurPrblmFctGeneric : AHeuristicProblemFactory
    {
        #region Members

        private readonly ConstraintClass m_constraintClass;
        private readonly List<HeuristicProblem> m_dummyProblems;

        #endregion

        #region Constructors

        public MixedHeurPrblmFctGeneric(
            IHeuristicObjectiveFunction objectiveFunction,
            int intVarCountInt,
            int intVarCountDbl,
            int intVarCountBln,
            ConstraintClass constraintClass,
            double[] dblVariableRangesIntegerProbl) :
                this(objectiveFunction,
                     GetDummyProblems(
                         intVarCountInt,
                         intVarCountDbl,
                         intVarCountBln,
                         dblVariableRangesIntegerProbl),
                     constraintClass)
        {
        }

        public MixedHeurPrblmFctGeneric(
            IHeuristicObjectiveFunction objectiveFunction,
            List<HeuristicProblem> problFactDummies,
            ConstraintClass constraintClass) :
                this(objectiveFunction,
                     problFactDummies,
                     constraintClass,
                     null,
            false)
        {
        }

        public MixedHeurPrblmFctGeneric(
            IHeuristicObjectiveFunction objectiveFunction,
            List<HeuristicProblem> problFactDummies,
            ConstraintClass constraintClass,
            GpOperatorsContainer gpOperatorsContainer,
            bool bonDoPublish) :
                base(EnumOptimimisationPoblemType.MIXED,
                     objectiveFunction)
        {
            m_constraintClass = constraintClass;
            m_dummyProblems = problFactDummies;
            m_gpOperatorsContainer = gpOperatorsContainer;
            DoPublish = bonDoPublish;
        }

        #endregion

        #region Public

        public static HeuristicProblem BuildDummyProblemDouble(
            int intVarCounts)
        {
            var heuristicProblFactDummy =
                new HeuristicProblFactDummy(
                    EnumOptimimisationPoblemType.CONTINUOUS,
                    intVarCounts);
            return heuristicProblFactDummy.BuildProblem();
        }

        public static HeuristicProblem BuildDummyProblemInteger(int intVarCounts)
        {
            var heuristicProblFactDummyInt =
                new HeuristicProblFactDummy(
                    EnumOptimimisationPoblemType.INTEGER,
                    intVarCounts);
            return heuristicProblFactDummyInt.BuildProblem();
        }

        public static HeuristicProblem BuildDummyProblemBln(int intVarCounts)
        {
            var heuristicProblFactDummyInt =
                new HeuristicProblFactDummy(
                    EnumOptimimisationPoblemType.BINARY,
                    intVarCounts);
            return heuristicProblFactDummyInt.BuildProblem();
        }

        public override HeuristicProblem BuildProblem()
        {
            return BuildProblem(
                string.Empty,
                string.Empty);
        }

        public override HeuristicProblem BuildProblem(
            string strProblemName,
            string strGuiNodeName)
        {
            //
            // This method creates dummy subproblems for integer 
            // and continuous cases.
            // 
            // The subproblems are linked to the main mixed problem:
            //     - Load main solver operators for these subproblems.
            // 
            // The fitness function evaluation is done by the main problem
            // The reproduction, repair and local search opeations
            // are done by the dummy problems
            // 

            HeuristicProblem mixedProblem = base.BuildProblem(
                strProblemName, 
                strGuiNodeName);
            //
            // invalidate guided convergence operator
            //
            mixedProblem.GuidedConvergence = null;

            mixedProblem.InnerProblemList = m_dummyProblems;


            foreach (HeuristicProblem dummyHeuristicProbl in m_dummyProblems)
            {
                //
                // Register objective functions
                // The objective function is evaluated by the main problem. 
                // Communication is done via event trigger.
                //
                SetObjectiveFunction(
                    mixedProblem,
                    dummyHeuristicProbl);

                //
                // Register Guided Convergence operator
                // The operator is executed by the main problem.
                // Comunication is done via event trigger.
                //
                SetGcOperators(
                    mixedProblem,
                    dummyHeuristicProbl);

                //
                // Register population.
                // The population of the subproblems is the same as the populaion
                // of the main problem
                // 
                SetPopulation(
                    mixedProblem,
                    dummyHeuristicProbl);

                //if(dummyHeuristicProbl.OptimimisationPoblemType ==
                //    OptimimisationPoblemType.INTEGER)
                //{
                //    if(mixedProblem.VariableRangesIntegerProbl != null)
                //    {
                //        throw new HCException("Multiple integer problems not allowed");
                //    }
                //    dummyHeuristicProbl.VariableRangesIntegerProbl =
                //        m_dblVariableRangesIntegerProbl;
                //    mixedProblem.VariableRangesIntegerProbl =
                //        m_dblVariableRangesIntegerProbl;
                //}
            }

            //
            // load reproduction operators
            //
            SetReproduction(mixedProblem);

            //
            // load individual factory
            //
            mixedProblem.IndividualFactory =
                new MixedIndividualFactoryGeneric(
                    mixedProblem,
                    m_dummyProblems);

            //
            // load local search operators
            //
            mixedProblem.LocalSearch =
                new MixedLocalSearch(
                    mixedProblem,
                    m_dummyProblems);

            //
            // load constraints
            //
            LoadConstraints(mixedProblem);

            //
            // set gc operators
            //
            SetGpOperators(mixedProblem);

            return mixedProblem;
        }

        public static HeuristicProblem BuildDummyProblem(
            EnumOptimimisationPoblemType enumOptimimisationPoblemType,
            int intVarCount)
        {
            var heuristicProblFactDummy =
                new HeuristicProblFactDummy(
                    enumOptimimisationPoblemType,
                    intVarCount);
            return heuristicProblFactDummy.BuildProblem();
        }

        #endregion

        #region Private

        private static List<HeuristicProblem> GetDummyProblems(
            int intVarCountInt,
            int intVarCountDbl,
            int intVarCountBln,
            double[] dblVariableRangesIntegerProbl)
        {
            var dummyHeuristicProblems =
                new List<HeuristicProblem>();
            //
            // load dummy problems
            //
            // dummy continous problem
            if (intVarCountDbl > 0)
            {
                var heuristicProblDbl =
                    BuildDummyProblem(
                        EnumOptimimisationPoblemType.CONTINUOUS,
                        intVarCountDbl);
                dummyHeuristicProblems.Add(
                    heuristicProblDbl);
            }

            // dummy integer problem
            if (intVarCountInt > 0)
            {
                var heuristicProblDummyInt =
                    BuildDummyProblem(
                        EnumOptimimisationPoblemType.INTEGER,
                        intVarCountInt);

                heuristicProblDummyInt.VariableRangesIntegerProbl =
                    dblVariableRangesIntegerProbl;

                dummyHeuristicProblems.Add(heuristicProblDummyInt);
            }

            // dummy binary problem
            if (intVarCountBln > 0)
            {
                var heuristicProblDummyBln =
                    BuildDummyProblem(
                        EnumOptimimisationPoblemType.BINARY,
                        intVarCountBln);
                dummyHeuristicProblems.Add(heuristicProblDummyBln);
            }
            if (dummyHeuristicProblems.Count == 0)
            {
                throw new HCException("No dummy problems");
            }

            return dummyHeuristicProblems;
        }

        private void SetGpOperators(
            HeuristicProblem mixedProblem)
        {
            //
            // set gp operators
            //
            foreach (HeuristicProblem problem in m_dummyProblems)
            {
                SetGpOperatorRecursively(problem);
                //
                // set params for gp problem
                //
                mixedProblem.GpOperatorsContainer =
                    m_gpOperatorsContainer;
                mixedProblem.InitialPopulation.DoLocalSearch = false;
                mixedProblem.LocalSearchProb = 0.1;
            }
        }

        private void SetGpOperatorRecursively(
            HeuristicProblem dummyHeuristicProbl)
        {
            if (dummyHeuristicProbl.EnumOptimimisationPoblemType ==
                EnumOptimimisationPoblemType.GENETIC_PROGRAMMING ||
                dummyHeuristicProbl.EnumOptimimisationPoblemType ==
                EnumOptimimisationPoblemType.MIXED)
            {
                dummyHeuristicProbl.GpOperatorsContainer =
                    m_gpOperatorsContainer;
            }

            //
            // set population for nested problems
            //
            if (dummyHeuristicProbl.InnerProblemList != null &&
                dummyHeuristicProbl.InnerProblemList.Count > 0)
            {
                foreach (HeuristicProblem nestedHeuristicProblem in 
                    dummyHeuristicProbl.InnerProblemList)
                {
                    SetGpOperatorRecursively(
                        nestedHeuristicProblem);
                }
            }
        }

        private void SetReproduction(
            HeuristicProblem mixedProblem)
        {
            mixedProblem.Reproduction = new MixedReproductionGeneric(
                mixedProblem,
                m_dummyProblems,
                m_gpOperatorsContainer);

            //
            // set reproduction recursively
            //
            foreach (HeuristicProblem dummyHeuristicProb in
                m_dummyProblems)
            {
                SetReproductionOperatorRecursively(
                    mixedProblem,
                    dummyHeuristicProb);
            }
        }

        private static void SetReproductionOperatorRecursively(
            HeuristicProblem mixedProblem,
            HeuristicProblem dummyHeuristicProblem)
        {
            dummyHeuristicProblem.Reproduction =
                mixedProblem.Reproduction;

            //
            // recursively set reproduction for nested problems
            //
            if (dummyHeuristicProblem.InnerProblemList != null &&
                dummyHeuristicProblem.InnerProblemList.Count > 0)
            {
                foreach (HeuristicProblem nestedHeuristicProblem in
                    dummyHeuristicProblem.InnerProblemList)
                {
                    SetReproductionOperatorRecursively(
                        mixedProblem,
                        nestedHeuristicProblem);
                }
            }
        }

        private void LoadConstraints(
            HeuristicProblem mixedProblem)
        {
            mixedProblem.Constraints = m_constraintClass;

            if (m_constraintClass != null)
            {
                //
                // set repair operator
                //
                mixedProblem.RepairIndividual =
                    new RepairClass(mixedProblem);

                foreach (HeuristicProblem dummyProblem in m_dummyProblems)
                {
                    //
                    // add constraints
                    // The same cosntraints apply for main problem
                    // and for the subproblems
                    //
                    LoadConstraint(mixedProblem, dummyProblem);
                }
            }
        }

        private void LoadConstraint(
            HeuristicProblem mixedProblem,
            HeuristicProblem dummyProblem)
        {
            if (dummyProblem != null)
            {
                //
                // Create repair operator
                // 
                var repairOperator =
                    new RepairClass(dummyProblem);
                dummyProblem.RepairIndividual = repairOperator;
                dummyProblem.Constraints = m_constraintClass;

                if (dummyProblem.EnumOptimimisationPoblemType ==
                    EnumOptimimisationPoblemType.INTEGER)
                {
                    var repairConstraintsInt =
                        new RepairConstraintsInt(
                            dummyProblem);

                    dummyProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsInt);

                    mixedProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsInt);
                }
                else if (dummyProblem.EnumOptimimisationPoblemType ==
                         EnumOptimimisationPoblemType.CONTINUOUS)
                {
                    var repairConstraintsDbl =
                        new RepairConstraintsDbl(
                            dummyProblem);

                    dummyProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsDbl);

                    mixedProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsDbl);
                }
                else if (dummyProblem.EnumOptimimisationPoblemType ==
                         EnumOptimimisationPoblemType.BINARY)
                {
                    var repairConstraintsBln =
                        new RepairConstraintsBln(
                            dummyProblem);

                    dummyProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsBln);

                    mixedProblem.RepairIndividual.AddRepairOperator(
                        repairConstraintsBln);
                }

                //
                // load constraint to nested problems recursively
                //
                if (dummyProblem.InnerProblemList != null &&
                    dummyProblem.InnerProblemList.Count > 0)
                {
                    foreach (HeuristicProblem innerHeuristicProblem in
                        dummyProblem.InnerProblemList)
                    {
                        LoadConstraint(
                            mixedProblem,
                            innerHeuristicProblem);
                    }
                }
            }
        }

        private static void SetPopulation(
            HeuristicProblem mixedProblem,
            HeuristicProblem dummyHeuristicProb)
        {
            if (dummyHeuristicProb != null)
            {
                dummyHeuristicProb.Population = mixedProblem.Population;
                dummyHeuristicProb.PopulationSize = mixedProblem.PopulationSize;

                //
                // set population for nested problems
                //
                if (dummyHeuristicProb.InnerProblemList != null &&
                    dummyHeuristicProb.InnerProblemList.Count > 0)
                {
                    foreach (HeuristicProblem nestedHeuristicProblem in
                        dummyHeuristicProb.InnerProblemList)
                    {
                        SetPopulation(
                            mixedProblem,
                            nestedHeuristicProblem);
                    }
                }
            }
        }

        private static void SetObjectiveFunction(
            HeuristicProblem mixedHeuristicProb,
            HeuristicProblem dummyHeuristicProb)
        {
            if (dummyHeuristicProb != null)
            {
                var mixedObjectiveFunctionDummy =
                    (ObjectiveFunctionDummy) dummyHeuristicProb.ObjectiveFunction;
                mixedObjectiveFunctionDummy.OnEvaluateObjective +=
                    mixedHeuristicProb.ObjectiveFunction.Evaluate;

                //
                // set objective function for nested problems
                //
                if (dummyHeuristicProb.InnerProblemList != null &&
                    dummyHeuristicProb.InnerProblemList.Count > 0)
                {
                    foreach (HeuristicProblem nestedHeuristicProblem in 
                        dummyHeuristicProb.InnerProblemList)
                    {
                        SetObjectiveFunction(
                            mixedHeuristicProb,
                            nestedHeuristicProblem);
                    }
                }
            }
        }

        private static void SetGcOperators(
            HeuristicProblem mixedHeuristicProb,
            HeuristicProblem dummyHeuristicProb)
        {
            if (dummyHeuristicProb != null &&
                dummyHeuristicProb.GuidedConvergence != null)
            {
                mixedHeuristicProb.Solver.OnCompletedGeneration +=
                    dummyHeuristicProb.GuidedConvergence.UpdateGcProbabilities;

                //
                // load inner gc operators recursively
                //
                if (dummyHeuristicProb.InnerProblemList != null &&
                    dummyHeuristicProb.InnerProblemList.Count > 0)
                {
                    foreach (HeuristicProblem nestedHeuristicProblem in 
                        dummyHeuristicProb.InnerProblemList)
                    {
                        SetGcOperators(
                            mixedHeuristicProb,
                            nestedHeuristicProblem);
                    }
                }
            }
        }

        #endregion
    }
}
