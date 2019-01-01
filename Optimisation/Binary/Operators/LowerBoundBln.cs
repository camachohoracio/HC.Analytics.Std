#region

using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LowerBound;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Events;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators
{
    /// <summary>
    ///   Lower bound class.
    ///   Calculates a solution which serves as a base point to a given solver
    /// </summary>
    public class LowerBoundBln : AbstractLowerBound
    {
        #region Members

        private new readonly HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public LowerBoundBln(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region Public

        /// <summary>
        ///   Standard lower bound method. Loads a list of variables 
        ///   ranked by reward/risk ratio.
        ///   Adds one by one of the ranked variables 
        ///   to the solution until the solution is out of bounds.
        /// </summary>
        /// </param>
        /// <returns>
        ///   Lower bound individual
        /// </returns>
        public override Individual GetLowerBound()
        {
            var listVariables = GetLowerBoundList();
            listVariables.Sort();
            var individual =
                m_heuristicProblem.IndividualFactory.BuildRandomIndividual();

            string strMessage;
            // add variables
            foreach (VariableContribution variable in listVariables)
            {
                individual.SetChromosomeValueDbl(variable.Index, 1.0);

                if (!m_heuristicProblem.CheckConstraints(individual))
                {
                    // back to original state
                    individual.SetChromosomeValueDbl(variable.Index, 0.0);
                    strMessage = "Greedy account not included: \t" +
                                 m_heuristicProblem.ObjectiveFunction.GetVariableDescription(variable.Index);
                    SendMessageEvent.OnSendMessage(this, strMessage);
                    PrintToScreen.WriteLine(strMessage);
                }
                else
                {
                    strMessage = "Greedy account: \t" +
                                 m_heuristicProblem.ObjectiveFunction.GetVariableDescription(variable.Index);
                    PrintToScreen.WriteLine(strMessage);
                    SendMessageEvent.OnSendMessage(this, strMessage);
                }
            }

            var dblChromosomeArray = new double[
                m_heuristicProblem.ObjectiveFunction.VariableCount];
            for (var i = 0;
                 i <
                 m_heuristicProblem.ObjectiveFunction.VariableCount;
                 i++)
            {
                if (individual.GetChromosomeValueDbl(i) >= 1.0 -
                    MathConstants.DBL_ROUNDING_FACTOR)
                {
                    dblChromosomeArray[i] = 1;
                }
            }

            var individual2 =
                new Individual(
                    (double[]) dblChromosomeArray.Clone(),
                    m_heuristicProblem);

            individual2.Evaluate(true,
                                 true,
                                 true,
                                 m_heuristicProblem);

            var m_dblLowerBound = individual2.Fitness;
            PrintToScreen.WriteLine("Lower bound = " + m_dblLowerBound);

            return new Individual(
                dblChromosomeArray,
                m_heuristicProblem);
        }

        /// <summary>
        ///   Naive lower bound operator.
        ///   Remove distribution until the constraints are satisfied.
        /// </summary>
        /// <returns></returns>
        public Individual GetLowerBound_naive()
        {
            var listVariables =
                GetLowerBoundList();
            listVariables.Sort();
            listVariables.Reverse();

            var individual =
                m_heuristicProblem.IndividualFactory.BuildRandomIndividual();

            string strMessage;

            // add all variables
            foreach (VariableContribution variable in listVariables)
            {
                individual.SetChromosomeValueDbl(variable.Index, 1.0);
            }

            // add variables
            foreach (VariableContribution variable in listVariables)
            {
                if (m_heuristicProblem.CheckConstraints(individual))
                {
                    break;
                }
                individual.SetChromosomeValueDbl(variable.Index, 0);

                // back to original state
                individual.SetChromosomeValueDbl(variable.Index, 0);
                strMessage = "Naive approach. Greedy account not included: \t" +
                             m_heuristicProblem.ObjectiveFunction.GetVariableDescription(variable.Index);
                SendMessageEvent.OnSendMessage(this, strMessage);
                PrintToScreen.WriteLine(strMessage);
            }

            var dblChromosomeArray =
                new double[m_heuristicProblem.VariableCount];
            for (var i = 0;
                 i <
                 m_heuristicProblem.VariableCount;
                 i++)
            {
                if (individual.GetChromosomeValueDbl(i) >= 1.0 - MathConstants.DBL_ROUNDING_FACTOR)
                {
                    dblChromosomeArray[i] = 1;
                }
            }

            var individual2 =
                new Individual(
                    (double[]) dblChromosomeArray.Clone(),
                    m_heuristicProblem);

            individual2.Evaluate(false,
                                 false,
                                 true,
                                 m_heuristicProblem);
            var m_dblLowerBound = individual2.Fitness;
            PrintToScreen.WriteLine("Lower bound naive premium = " + m_dblLowerBound);

            return new Individual(
                dblChromosomeArray,
                m_heuristicProblem);
        }

        #endregion
    }
}
