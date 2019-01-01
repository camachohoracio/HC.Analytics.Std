#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Binary.Operators.Reproduction;
using HC.Analytics.Optimisation.Continuous.Operators;
using HC.Analytics.Optimisation.Gp.GpOperators;
using HC.Analytics.Optimisation.Integer.Operators;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.Operators
{
    /// <summary>
    ///   Combines double and integer reproduction operators
    /// </summary>
    [Serializable]
    public class MixedReproductionGeneric : ReproductionClass
    {
        #region Members

        private readonly GpOperatorsContainer m_gpOperatorsContainer;

        #endregion

        #region Constructors

        public MixedReproductionGeneric(
            HeuristicProblem heuristicProblem,
            List<HeuristicProblem> heuristicProblems,
            GpOperatorsContainer gpOperatorsContainer) :
                base(heuristicProblem)
        {
            m_gpOperatorsContainer = gpOperatorsContainer;
            LoadReproductionOperators(heuristicProblems);
        }

        #endregion

        #region Public

        public override Individual DoReproduction()
        {
            try
            {
                var reproductionList = new List<IReproduction>(m_reproductionList);
                //
                // create new individual
                //
                var finalIndividual = new Individual(
                    null,
                    null,
                    null,
                    0,
                    m_heuristicProblem)
                                          {
                                              IndividualList = new List<Individual>()
                                          };

                foreach (IReproduction reproduction in reproductionList)
                {
                    Individual individual = reproduction.DoReproduction();
                    individual.ProblemName =
                        reproduction.HeuristicProblem.ProblemName;

                    finalIndividual.IndividualList.Add(individual);
                }

                if (finalIndividual.IndividualList.Count == 0)
                {
                    throw new HCException("No individuals found");
                }

                return finalIndividual;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        #endregion

        #region Private

        private void LoadReproductionOperators(List<HeuristicProblem> heuristicProblems)
        {
            m_reproductionList = new List<IReproduction>();


            foreach (HeuristicProblem problem in heuristicProblems)
            {
                if (problem != null)
                {
                    IReproduction currentReproduction;
                    if (problem.EnumOptimimisationPoblemType == EnumOptimimisationPoblemType.INTEGER)
                    {
                        currentReproduction = new ReproductionIntStd(problem);
                    }
                    else if (problem.EnumOptimimisationPoblemType == EnumOptimimisationPoblemType.BINARY)
                    {
                        currentReproduction = new ReproductionBlnStd(problem);
                    }
                    else if (problem.EnumOptimimisationPoblemType == EnumOptimimisationPoblemType.CONTINUOUS)
                    {
                        currentReproduction = new ReproductionDblStd(problem);
                    }
                    else if (problem.EnumOptimimisationPoblemType == EnumOptimimisationPoblemType.GENETIC_PROGRAMMING)
                    {
                        currentReproduction = new GpReproduction(
                            problem,
                            m_gpOperatorsContainer.CrossoverProbability,
                            m_gpOperatorsContainer.MaxTreeDepthMutation,
                            m_gpOperatorsContainer.MaxTreeSize,
                            m_gpOperatorsContainer.TournamentSize,
                            m_gpOperatorsContainer);
                    }
                    else if (problem.EnumOptimimisationPoblemType ==
                             EnumOptimimisationPoblemType.MIXED)
                    {
                        currentReproduction = new MixedReproductionGeneric(
                            problem,
                            problem.InnerProblemList,
                            m_gpOperatorsContainer);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    m_reproductionList.Add(currentReproduction);
                }
            }
        }

        #endregion
    }
}
