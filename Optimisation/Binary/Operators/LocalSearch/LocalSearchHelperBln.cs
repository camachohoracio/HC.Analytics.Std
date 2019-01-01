#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.LocalSearch
{
    public static class LocalSearchHelperBln
    {
        public static void GetRankLists(
            Individual individual0,
            RngWrapper rng,
            int intChromosomeLength,
            bool blnGoForward,
            HeuristicProblem heuristicProblem,
            out List<int> indexList,
            out List<VariableContribution> selectedRankedList)
        {
            var localIndividual = individual0;
            if (individual0.IndividualList != null &&
                individual0.IndividualList.Count > 0)
            {
                localIndividual = individual0.GetIndividual(
                    heuristicProblem.ProblemName);
            }

            indexList = new List<int>(intChromosomeLength + 1);

            var oneList = new List<int>(intChromosomeLength + 1);
            var zeroList = new List<int>(intChromosomeLength + 1);
            for (var i = 0; i < intChromosomeLength; i++)
            {
                if (localIndividual.GetChromosomeValueBln(i))
                {
                    oneList.Add(i);
                }
                else
                {
                    zeroList.Add(i);
                }
            }

            //
            // rank one list
            //
            var oneListRanked =
                new List<VariableContribution>();
            for (var i = 0; i < oneList.Count; i++)
            {
                var intCurrentIndex = oneList[i];
                oneListRanked.Add(
                    new VariableContribution(
                        intCurrentIndex,
                        (heuristicProblem.Reproduction == null
                             ? 1.0
                             : heuristicProblem.GuidedConvergence.GetGcProb(
                                 intCurrentIndex))*
                        rng.NextDouble()));
            }
            oneListRanked.Sort();


            //
            // rank zero list
            //
            var zeroListRanked =
                new List<VariableContribution>();
            for (var i = 0; i < zeroList.Count; i++)
            {
                var intCurrentIndex = zeroList[i];
                zeroListRanked.Add(
                    new VariableContribution(
                        intCurrentIndex,
                        (
                            heuristicProblem.Reproduction == null
                                ? 1.0
                                : heuristicProblem.GuidedConvergence.GetGcProb(
                                    intCurrentIndex))*
                        rng.NextDouble()));
            }
            zeroListRanked.Sort();

            //
            // reverse the list in order to 
            // allow the most likely zeros to be converted into ones
            //
            selectedRankedList = null;
            List<VariableContribution> nonRankedList = null;

            if (blnGoForward)
            {
                zeroListRanked.Reverse();
                selectedRankedList = zeroListRanked;
                nonRankedList = oneListRanked;
            }
            else
            {
                oneListRanked.Reverse();
                selectedRankedList = oneListRanked;
                nonRankedList = zeroListRanked;
            }

            //
            // load index list
            //
            foreach (VariableContribution variableContribution in
                nonRankedList)
            {
                indexList.Add(variableContribution.Index);
            }
        }
    }
}
