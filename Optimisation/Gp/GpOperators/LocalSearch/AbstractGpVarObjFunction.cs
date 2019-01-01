#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch
{
    public abstract class AbstractGpVarObjFunction : AbstractHeuristicObjectiveFunction
    {
        #region Properties

        public Individual BestIndividual { get; set; }

        public AbstractGpNode Root
        {
            get { return m_root; }
        }

        public GpOperatorsContainer GpOperatorsContainer
        {
            get { return m_gpOperatorsContainer; }
        }

        public HeuristicProblem BaseHeuristicProblem
        {
            get { return m_baseHeuristicProblem; }
        }

        #endregion

        #region Members

        protected readonly HeuristicProblem m_baseHeuristicProblem;
        protected readonly GpOperatorsContainer m_gpOperatorsContainer;
        protected readonly int m_intVariableCount;
        protected readonly AbstractGpNode m_root;
        private double m_dblBestFiness;
        protected List<AbstractGpNode> m_varNodeList;

        #endregion

        #region Constructors

        protected AbstractGpVarObjFunction(
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            int intVariableCount,
            List<AbstractGpNode> varNodeList) :
                base(null)
        {
            m_dblBestFiness = -double.MaxValue;
            m_baseHeuristicProblem = baseHeuristicProblem;
            m_gpOperatorsContainer = gpOperatorsContainer;
            m_root = root;
            m_intVariableCount = intVariableCount;
            m_varNodeList = varNodeList;
        }

        #endregion

        #region Abstract Methods

        public abstract Individual CreateIndividualForParentProblem(
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            int[] intChromosomeArr,
            double[] dblChromosomeArr);

        #endregion

        #region Public

        public override int VariableCount
        {
            get { return m_intVariableCount; }
        }

        public override double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            int[] intChromosomeArr = null;
            double[] dblChromosomeArr = null;

            if (individual.IndividualList == null ||
                individual.IndividualList.Count == 0)
            {
                if (individual.ContainsChromosomeInt())
                {
                    intChromosomeArr = individual.GetChromosomeCopyInt();
                }

                if (individual.ContainsChromosomeDbl())
                {
                    dblChromosomeArr = individual.GetChromosomeCopyDbl();
                }
            }
            else
            {
                //
                // load chromosomes from inner individuals
                //
                foreach (Individual innerIndividual in individual.IndividualList)
                {
                    if (innerIndividual.ContainsChromosomeInt())
                    {
                        intChromosomeArr = innerIndividual.GetChromosomeCopyInt();
                    }

                    if (innerIndividual.ContainsChromosomeDbl())
                    {
                        dblChromosomeArr = innerIndividual.GetChromosomeCopyDbl();
                    }
                }
            }

            var newIndividual = CreateIndividualForParentProblem(
                m_root,
                m_gpOperatorsContainer,
                m_baseHeuristicProblem,
                intChromosomeArr,
                dblChromosomeArr);

            var dblFitness = m_baseHeuristicProblem.
                GpBridge.GetRegressionFit(newIndividual, heuristicProblem);

            if (dblFitness > m_dblBestFiness)
            {
                m_dblBestFiness = dblFitness;
                BestIndividual = newIndividual;
            }

            return dblFitness;
        }

        public override double Evaluate()
        {
            throw new NotImplementedException();
        }

        public override string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
