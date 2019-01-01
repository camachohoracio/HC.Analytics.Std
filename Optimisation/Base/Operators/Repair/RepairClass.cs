#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Repair
{
    [Serializable]
    public class RepairClass : AbstractRepair
    {
        #region Members

        private readonly List<IRepair> m_repairList;

        #endregion

        #region Properties

        public HeuristicProblem HeuristicOptmizationProblem_
        {
            get { return m_heuristicProblem; }
        }

        #endregion

        #region Constructors

        public RepairClass(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_repairList = new List<IRepair>();
        }

        #endregion

        #region Public Methods

        public override void AddRepairOperator(IRepair repair)
        {
            m_repairList.Add(repair);
        }

        public override bool DoRepair(Individual individual)
        {
            if (m_repairList == null || m_repairList.Count == 0)
            {
                throw new HCException("Repair list is empty");
            }

            //
            // shuffle repair list
            //
            var repairList = new List<IRepair>(m_repairList);
            RngWrapper rng = HeuristicProblem.CreateRandomGenerator();
            rng.ShuffleList(repairList);

            var blnRepair = true;
            foreach (IRepair repairOperator in repairList)
            {
                if (!repairOperator.DoRepair(individual))
                {
                    blnRepair = false;
                }
            }

            //if (blnRepair &&
            //    m_heuristicProblem.DoCheckConstraints &&
            //    !m_heuristicProblem.Constraints.CheckConstraints(individual))
            //{
            //    throw new HCException("Repair individual failed.");
            //}

            return blnRepair;
        }

        #endregion
    }
}
