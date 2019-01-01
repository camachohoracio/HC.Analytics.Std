#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;
using HC.Core;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers.NelderMead
{
    /// <summary>
    ///   This class is used for local search purposes.
    /// 
    ///   The Nelder-Mead algorithm requires n+1 solutions. The number
    ///   of required solutions is selected in this class.
    /// 
    ///   Since the same solver may be called
    ///   multiple times (for each local search call), 
    ///   the solver should select a different starting point each
    ///   time it is executed (Otherwise the same solution may be found).
    /// </summary>
    public abstract class AbstractNmPopulationGenerator : IDisposable
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;
        private readonly int m_intNmDimensions;
        private object m_lockObjectAddValidation = new object();
        private Dictionary<string, object> m_solutionValidator;

        #endregion

        #region Constructor

        protected AbstractNmPopulationGenerator(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_solutionValidator = new Dictionary<string, object>();
            m_intNmDimensions = m_heuristicProblem.VariableCount + 1;
        }

        #endregion

        #region Public

        public AbstractNmVertex[] GenerateVertexArray()
        {
            var vertexArray = new AbstractNmVertex[m_intNmDimensions];
            int intIndex = GetLowerIndividual(vertexArray);
            LoadVertexList(vertexArray, intIndex);
            return vertexArray;
        }

        #endregion

        #region Private

        protected virtual void LoadVertexList(
            AbstractNmVertex[] vertexArray, 
            int intIndex)
        {
            //
            // create a new validator dictionary in order to avoid
            // adding the same vertex to the vertex array
            //
            var validationDictionary =
                new Dictionary<string, object>(m_intNmDimensions + 1);

            while (intIndex < m_intNmDimensions)
            {
                var currentIndividual = m_heuristicProblem.
                    Reproduction.DoReproduction();

                currentIndividual.Evaluate(m_heuristicProblem);
                var strIndividualDescr =
                    ToStringIndividual(currentIndividual);
                if (!validationDictionary.ContainsKey(strIndividualDescr) &&
                    !m_solutionValidator.ContainsKey(strIndividualDescr) &&
                    m_heuristicProblem.Constraints.CheckConstraints(currentIndividual))
                {
                    LoadVertexItem(vertexArray, intIndex, currentIndividual);
                    validationDictionary.Add(strIndividualDescr, null);
                    intIndex++;
                }
            }
        }

        private int GetLowerIndividual(AbstractNmVertex[] vertexArray)
        {
            //
            // Get lower bound individual.
            // Ensure that the lower bound has not been included in a previous 
            // call to the solver
            //
            var intIndex = 0;
            for (var i = 0; i < m_heuristicProblem.Population.LargePopulationSize; i++)
            {
                Individual currentIndividual =
                    m_heuristicProblem.
                        Population.GetIndividualFromLargePopulation(
                            null,
                            i);

                if (currentIndividual != null)
                {
                    var strIndividualDescr = ToStringIndividual(currentIndividual);
                    lock (m_lockObjectAddValidation)
                    {
                        if (!m_solutionValidator.ContainsKey(strIndividualDescr) &&
                            m_heuristicProblem.Constraints.CheckConstraints(currentIndividual))
                        {
                            m_solutionValidator.Add(strIndividualDescr, null);
                            LoadVertexItem(vertexArray, intIndex, currentIndividual);
                            intIndex++;
                            break;
                        }
                    }
                }
            }
            return intIndex;
        }

        /// <summary>
        ///   Add a new vertex to vertex arr
        /// </summary>
        /// <param name = "vertexArray"></param>
        /// <param name = "intIndex"></param>
        /// <param name = "currentIndividual"></param>
        protected void LoadVertexItem(
            AbstractNmVertex[] vertexArray,
            int intIndex,
            Individual currentIndividual)
        {
            vertexArray[intIndex] =
                CreateNmVertex(
                    m_heuristicProblem.VariableCount,
                    currentIndividual);
            vertexArray[intIndex].Value = -currentIndividual.Fitness;
        }

        #endregion

        #region Abstract Methods

        public abstract string ToStringIndividual(Individual individual);
        protected abstract double[] GetChromosomeCopy(Individual individual);

        public abstract AbstractNmVertex CreateNmVertex(
            int intDimensions,
            Individual individual);

        #endregion

        ~AbstractNmPopulationGenerator()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_solutionValidator != null)
            {
                m_solutionValidator.Clear();
            }
            m_solutionValidator = null;
            m_heuristicProblem = null;
            m_lockObjectAddValidation = null;
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }
    }
}
