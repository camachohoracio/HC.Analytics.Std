//#region

//using System;
//using System.Collections.Generic;
//using System.Text;
//using HC.Analytics.Mathematics;
//using HC.Analytics.Probability.Distributions.LossDistributions;
//using HC.Analytics.Probability.Portfolio.Base;
//using HC.Core;
//using HC.Core.Exceptions;

//#endregion

//namespace HC.Analytics.Probability.Distributions.Portfolio
//{
//    public abstract class AbstractPortfolio
//    {
//        #region Properties

//        public double[] DistributionValidationArray { get; set; }

//        public IDistribution BaseDistribution { get; set; }

//        public IDistribution Distribution { get; set; }

//        #endregion

//        #region Members

//        protected bool m_blnGenerateDistributions;

//        protected IDistribution m_distributionRemove;

//        protected int m_intProblemSize;

//        protected Dictionary<int, double> m_listAddAnalysisQueue;

//        protected Dictionary<int, double> m_listRemoveAnalysisQueue;

//        protected IRepository m_repository;

//        #endregion

//        #region Constructor

//        protected AbstractPortfolio(
//            IRepository repository,
//            IDistribution baseDistribution)
//        {
//            m_blnGenerateDistributions = true;
//            m_intProblemSize = repository.Capacity;
//            m_repository = repository;
//            m_listAddAnalysisQueue = new Dictionary<int, double>();
//            m_listRemoveAnalysisQueue = new Dictionary<int, double>();
//            BaseDistribution = baseDistribution;
//            InitializePortfolio();
//        }

//        #endregion

//        #region Public

//        public string ToStringPortfolioSummary()
//        {
//            // load distribution
//            GetDistribution();
//            var sb = new StringBuilder();

//            //
//            // get risk metrics at a portfolio level
//            //
//            IDistribution standAloneDistribution = Distribution.Clone();
//            standAloneDistribution.RemoveDistribution(BaseDistribution);

//            double dblAal = Distribution.GetAal();
//            double dblStdDev = Distribution.GetStdDev();
//            int intPortfolioSize = DistributionValidationArray.Length;

//            sb.Append(Environment.NewLine + "Portfolio summary:");
//            sb.Append(Environment.NewLine + "AAL = " + dblAal.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Stddev = " + dblStdDev.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Size = " + intPortfolioSize);

//            return sb.ToString();
//        }

//        public string ToStringMarginalRisks(
//            EnumEpType enumEpType,
//            double dblReturnPeriod)
//        {
//            // load distribution
//            GetDistribution();
//            var sb = new StringBuilder();

//            //
//            // get risk metrics at a portfolio level
//            //
//            IDistribution standAloneDistribution = Distribution.Clone();
//            standAloneDistribution.RemoveDistribution(BaseDistribution);

//            double dblBaseRisk = BaseDistribution.GetPnl(dblReturnPeriod, enumEpType);
//            double dblTotalRisk = Distribution.GetPnl(dblReturnPeriod, enumEpType);
//            double dblStandaloneRisk = standAloneDistribution.GetPnl(dblReturnPeriod, enumEpType);

//            double dblMarginalRisk = dblTotalRisk - dblBaseRisk;

//            sb.Append(Environment.NewLine + "Overall marginal Risks:");
//            sb.Append(Environment.NewLine + "Base risk = " + dblBaseRisk.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Total risk = " + dblTotalRisk.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Standalone risk = " + dblStandaloneRisk.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Marginal risk = " + dblMarginalRisk.ToString("#,#"));
//            sb.Append(Environment.NewLine + "Portfolio Risks metrics:" + Environment.NewLine);
//            sb.Append(Environment.NewLine + "Risk concentrations:" + Environment.NewLine);
//            return sb.ToString();
//        }

//        public IDistribution GetDistribution()
//        {
//            if (m_listAddAnalysisQueue.Count > 0 ||
//                m_listRemoveAnalysisQueue.Count > 0)
//            {
//                RunAnalysis();
//            }
//            return Distribution;
//        }

//        public void RemoveAll()
//        {
//            for (int i = 0; i < DistributionValidationArray.Length; i++)
//            {
//                if (DistributionValidationArray[i] > 0.0)
//                {
//                    RemoveDistributionIndexQueue(i);
//                }
//            }
//            RunAnalysis();
//        }

//        public void AddDistributionIndexQueue(int intDistributionIndex)
//        {
//            AddDistributionIndexQueue(
//                intDistributionIndex,
//                1,
//                DistributionValidationArray,
//                ref m_listAddAnalysisQueue);
//        }

//        public void AddDistributionIndexQueue(int intDistributionIndex, double dblWeight)
//        {
//            if (dblWeight != 0)
//            {
//                AddDistributionIndexQueue(
//                    intDistributionIndex,
//                    dblWeight,
//                    DistributionValidationArray,
//                    ref m_listAddAnalysisQueue);
//            }
//            if (dblWeight < 0)
//            {
//                throw new HCException("Negative weight.");
//            }
//        }

//        public void RemoveDistributionIndexQueue(int intDistributionIndex)
//        {
//            RemoveDistributionIndexQueue(
//                intDistributionIndex,
//                1,
//                DistributionValidationArray,
//                ref m_listRemoveAnalysisQueue);
//        }

//        public void RemoveDistributionIndexQueue(
//            int intDistributionIndex,
//            double dblWeight)
//        {
//            if (dblWeight > 0)
//            {
//                RemoveDistributionIndexQueue(
//                    intDistributionIndex,
//                    dblWeight,
//                    DistributionValidationArray,
//                    ref m_listRemoveAnalysisQueue);
//            }
//            else
//            {
//                throw new HCException("Negative weight.");
//            }
//        }

//        public void RemoveFromBasePortfolio(
//            IDistribution baseDistribution,
//            IDistribution deleteDistribution)
//        {
//            InitializeAnalysis();
//            Distribution.RemoveDistribution(deleteDistribution);
//            BaseDistribution = baseDistribution;
//        }

//        public void AddToBasePortfolio(IDistribution baseDistribution, IDistribution addDistribution)
//        {
//            InitializeAnalysis();
//            BaseDistribution = baseDistribution;
//            Distribution.MergeDistribution(addDistribution, 1.0);
//        }

//        public bool AddNewDistributions()
//        {
//            // initialize list of distributions
//            InitializeAnalysis();
//            if (m_repository.GetSize() > 0)
//            {
//                var dblNewDistributionValidation = new double[m_repository.GetSize()];
//                if (m_repository != null)
//                {
//                    for (int i = 0; i < DistributionValidationArray.Length; i++)
//                    {
//                        dblNewDistributionValidation[i] = DistributionValidationArray[i];
//                    }
//                    DistributionValidationArray = dblNewDistributionValidation;
//                }
//                else
//                {
//                    DistributionValidationArray = dblNewDistributionValidation;
//                }
//            }
//            else if (m_repository != null)
//            {
//                PrintToScreen.WriteLine("Warning. New risks were not added.");
//                return false;
//            }


//            return true;
//        }

//        public void RemoveDistributions(int intDistributionIndex)
//        {
//            // initialize list of distributions
//            InitializeAnalysis();
//            if (intDistributionIndex != -1)
//            {
//                // remove distribution from portfolio
//                RemoveDistributionIndexQueue(intDistributionIndex);
//                RunAnalysis();

//                var dblNewDistributionValidation = new double[m_repository.GetSize() - 1];
//                for (int i = 0, j = 0; i < DistributionValidationArray.Length; i++)
//                {
//                    if (i != intDistributionIndex)
//                    {
//                        dblNewDistributionValidation[j] = DistributionValidationArray[i];
//                        j++;
//                    }
//                }
//                DistributionValidationArray = dblNewDistributionValidation;
//                m_repository.DeleteDistribution(intDistributionIndex);
//            }
//        }

//        public IPortfolio Clone()
//        {
//            IPortfolio analysisPortfolio =
//                m_repository.PortfolioFactory.BuildPortfolio(
//                    m_repository,
//                    BaseDistribution == null
//                        ?
//                            null
//                        : BaseDistribution.Clone());
//            if (DistributionValidationArray != null)
//            {
//                analysisPortfolio.DistributionValidationArray =
//                    (double[]) DistributionValidationArray.Clone();
//            }
//            analysisPortfolio.Distribution = Distribution.Clone();
//            return analysisPortfolio;
//        }

//        #endregion

//        #region Private

//        private void AddDistributionIndexQueue(
//            int intDistributionIndex,
//            double dblWeight,
//            double[] dblPortfolioValidationArray,
//            ref Dictionary<int, double> listAddAnalysisQueue)
//        {
//            if (dblPortfolioValidationArray[intDistributionIndex] + dblWeight > 1.0)
//            {
//                if (dblPortfolioValidationArray[intDistributionIndex] + dblWeight > 1.000000000000001)
//                {
//                    throw new HCException("Distribution already added.");
//                }
//                // rounding errors
//                dblWeight = 1.0 - dblPortfolioValidationArray[intDistributionIndex];
//            }
//            dblPortfolioValidationArray[intDistributionIndex] += dblWeight;
//            listAddAnalysisQueue.Add(intDistributionIndex, dblWeight);
//        }

//        private void RemoveDistributionIndexQueue(
//            int intDistributionIndex,
//            double dblWeight,
//            double[] dblPortfolioValidationArray,
//            ref Dictionary<int, double> listRemoveAnalysisQueue)
//        {
//            if (dblPortfolioValidationArray[intDistributionIndex] -
//                dblWeight < 0.0)
//            {
//                if (dblWeight - dblPortfolioValidationArray[intDistributionIndex] >
//                    MathConstants.DBL_ROUNDING_FACTOR)
//                {
//                    //Debugger.Break();
//                    throw new HCException("Distribution not included in this portfolio.");
//                }
//                // round error
//                dblWeight = dblPortfolioValidationArray[intDistributionIndex];
//            }
//            else if (
//                dblPortfolioValidationArray[intDistributionIndex] -
//                dblWeight < MathConstants.DBL_ROUNDING_FACTOR)
//            {
//                // round error
//                dblWeight = dblPortfolioValidationArray[intDistributionIndex];
//            }
//            dblPortfolioValidationArray[intDistributionIndex] -= dblWeight;
//            listRemoveAnalysisQueue.Add(intDistributionIndex, dblWeight);
//        }

//        private void GetInitialDistribution()
//        {
//            if (BaseDistribution != null)
//            {
//                Distribution.MergeDistribution(BaseDistribution, 1);
//            }
//        }

//        protected void InitializePortfolio()
//        {
//            if (m_repository != null)
//            {
//                DistributionValidationArray =
//                    new double[m_repository.GetSize()];
//            }

//            Distribution = m_repository.DistributionFactory.BuildDistribution();
//            m_distributionRemove = m_repository.DistributionFactory.BuildDistribution();
//            GetInitialDistribution();
//        }

//        #endregion

//        #region Protected

//        protected void InitializeAnalysis()
//        {
//            m_listAddAnalysisQueue = new Dictionary<int, double>();
//            m_listRemoveAnalysisQueue = new Dictionary<int, double>();
//        }

//        #endregion

//        #region Abstract

//        public abstract void RunAnalysis();

//        #endregion
//    }
//}
