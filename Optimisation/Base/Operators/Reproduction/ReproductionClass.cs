#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Reproduction
{
    [Serializable]
    public class ReproductionClass : AbstractReproduction
    {
        #region Members

        /// <summary>
        ///   A list containing all the reproduction operators
        /// </summary>
        protected List<IReproduction> m_reproductionList;

        #endregion

        #region Constructor

        protected ReproductionClass(
            HeuristicProblem heuristicProblem) :
                this(heuristicProblem,
                     null)
        {
        }

        public ReproductionClass(
            HeuristicProblem heuristicProblem,
            List<IReproduction> reproductionList) :
                base(heuristicProblem)
        {
            m_reproductionList = reproductionList;
            ValidateReproductionList();
        }

        #endregion

        public override Individual DoReproduction()
        {
            //
            // turn the wheel, select a reproduction 
            // operator and reproduce
            //
            var rng =
                HeuristicProblem.CreateRandomGenerator();
            var dblRandom = rng.NextDouble();
            double dblTotalProb = 0;
            foreach (IReproduction reproduction in m_reproductionList)
            {
                dblTotalProb += reproduction.ReproductionProb;
                if (dblRandom <= dblTotalProb)
                {
                    var newIndividual = reproduction.DoReproduction();
                    if (newIndividual.IsEvaluated)
                    {
                        //Debugger.Break();
                        throw new HCException("Individual already evaluated.");
                    }

                    return newIndividual;
                }
            }
            throw new HCException("Error. Reproduction not selected");
        }

        public override void ClusterInstance(Individual individual)
        {
        }

        protected void ValidateReproductionList()
        {
            if (m_reproductionList == null)
            {
                return;
            }

            double dblTotalProb = 0;
            foreach (IReproduction reproduction in m_reproductionList)
            {
                dblTotalProb += reproduction.ReproductionProb;
            }
            if (Math.Abs(dblTotalProb - 1.0) > 1.0e-4)
            {
                throw new HCException("Error. Probability is not = 1");
            }
        }
    }
}
