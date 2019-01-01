#region

using System.Collections.Generic;
using HC.Analytics.Probability.Distributions.LossDistributions;
using HC.Core.Events;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.Portfolio
{
    public abstract class AbstractRepository
    {
        #region Events

        public event LoadDistributionReadyEventHandler LoadReady;

        #endregion

        #region Properties

        private int m_intCapacity;

        public int Capacity
        {
            get { return m_intCapacity; }
            set
            {
                m_intCapacity =
                    m_intCapacity == int.MaxValue
                        ?
                            int.MaxValue - 1
                        : value;
            }
        }

        public List<IDistribution> ListDistribution { get; set; }

        public IDistributionFactory DistributionFactory { get; set; }
        
        public IPortfolioFactory PortfolioFactory { get; set; }

        #endregion

        #region Members

        private IDistribution m_DistributionTable;

        protected List<IJob> m_jobList;

        protected List<RiskVariable> m_marginaRisksList;

        #endregion

        #region Constructors

        protected AbstractRepository(
            int intCapacity,
            IDistributionFactory distributionFactory,
            IPortfolioFactory portfolioFactory)
        {
            ListDistribution = new List<IDistribution>();
            Capacity = intCapacity;
            m_jobList = new List<IJob>();
            Capacity = intCapacity;
            DistributionFactory = distributionFactory;
            PortfolioFactory = portfolioFactory;
        }

        #endregion

        #region Public

        public void AddDistributionJob(IJob jobAnalysis)
        {
            m_jobList.Add(jobAnalysis);
            ResetValues();
        }

        public void AddDistribution(IDistribution distribution)
        {
            ListDistribution.Add(distribution);
            ResetValues();
        }

        public void DeleteDistribution(int intDistributionIndex)
        {
            ResetValues();
            ListDistribution.RemoveAt(intDistributionIndex);
        }

        public IDistribution GetDistributionTable()
        {
            if (m_DistributionTable == null)
            {
                IDistribution distributionTable =
                    DistributionFactory.BuildDistribution();
                for (int i = 0; i < ListDistribution.Count; i++)
                {
                    distributionTable.MergeDistribution(ListDistribution[i], 1.0);
                }
                m_DistributionTable = distributionTable;
            }
            return m_DistributionTable;
        }

        public int CountActiveDistributions()
        {
            int intCounter = 0;
            for (int i = 0; i < ListDistribution.Count; i++)
            {
                if (ListDistribution[i].ContainsDistribution())
                {
                    intCounter++;
                }
            }
            return intCounter;
        }

        public List<RiskVariable> GetMarginalRiskList(
            double dblReturnPeriod,
            EnumEpType enumEpType,
            int intCapacity)
        {
            if (m_marginaRisksList == null)
            {
                m_marginaRisksList = new List<RiskVariable>();
                int intRepositorySize = ListDistribution.Count;
                // get average diversification
                IDistribution distributionAll =
                    DistributionFactory.BuildDistribution();
                for (int i = 0; i < intRepositorySize; i++)
                {
                    distributionAll.MergeDistribution(ListDistribution[i], 1.0);
                }
                double dblAllRisk = distributionAll.GetPnl(dblReturnPeriod, enumEpType);
                double dblMarginalRisk;
                double dblContributorialRisk;
                string strMessage;
                for (int i = 0; i < intRepositorySize; i++)
                {
                    // remove distribution
                    distributionAll.MergeDistribution(ListDistribution[i], -1.0);
                    dblMarginalRisk = dblAllRisk - distributionAll.GetPnl(dblReturnPeriod, enumEpType);
                    dblContributorialRisk = ListDistribution[i].GetPnl(dblReturnPeriod, enumEpType);
                    dblMarginalRisk = dblMarginalRisk < 0 ? 0 : dblMarginalRisk;
                    m_marginaRisksList.Add(new RiskVariable(i, dblMarginalRisk, dblContributorialRisk));
                    // back to initial state
                    distributionAll.MergeDistribution(ListDistribution[i], 1.0);
                    strMessage = "Calculating portfolio's marginal risks " + (i + 1) +
                                 " of " + intRepositorySize + ". Please wait...";
                    PrintToScreen.WriteLine(strMessage);
                    SendMessageEvent.OnSendMessage(this, strMessage);
                }
            }
            return m_marginaRisksList;
        }

        public IDistribution GetDistribution(int intDistribuionIndex)
        {
            if (!ListDistribution[intDistribuionIndex].ContainsDistribution())
            {
                LoadDistributionData(intDistribuionIndex);
            }
            return ListDistribution[intDistribuionIndex];
        }

        public int GetSize()
        {
            return ListDistribution.Count;
        }

        #endregion

        #region Private

        private void LoadDistributionData(int intDistributionIndex)
        {
            IDistribution distribution = ListDistribution[intDistributionIndex];
        }

        #endregion

        #region Protected

        protected void RepositoryLoadReady(int state, int intWorkerId, string strMessage)
        {
            InvokeDistributionReadyEventHandler(strMessage);
        }

        private void InvokeDistributionReadyEventHandler(string strMessage)
        {
            if (LoadReady != null)
            {
                if (LoadReady.GetInvocationList().Length > 0)
                {
                    LoadReady.Invoke(strMessage);
                }
            }
        }

        #endregion

        #region Abstract

        protected abstract void ResetValues();

        #endregion
    }
}
