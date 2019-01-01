#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.DynamicCompilation;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public abstract class AOptInstFctry : IOptInstFctry
    {
        #region Properties

        public bool AmIUnitTesting { get; set; }
        public HeuristicProblem HeuristicProblem { get; set; }

        #endregion

        #region Members

        private readonly object m_lockObject = new object();
        protected SortedDictionary<OptChromosomeWrapper, OptChromosomeWrapper> m_chromosomes;

        #endregion

        public bool ContainsStatsCache(
            OptChromosomeWrapper chromosomeWrapper,
            out OptStatsCache optStatsCache)
        {
            try
            {
                OptChromosomeWrapper outChromosomeWrapper;
                var blnContainsKey = m_chromosomes.TryGetValue(
                    chromosomeWrapper,
                    out outChromosomeWrapper);
                optStatsCache = null;
                if (blnContainsKey)
                {
                    optStatsCache = outChromosomeWrapper.OptStatsCache;
                    if (optStatsCache == null)
                    {
                        //
                        // the stats cache is in process in another thread
                        //
                        blnContainsKey = false;
                    }
                }
                return blnContainsKey;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            optStatsCache = null;
            return false;
        }

        public OptStatsCache GetOptStatsCache(
            OptChromosomeWrapper chromosomeWrapper,
            ASelfDescribingClass constantsToOptimise)
        {
            try
            {
                lock (m_lockObject)
                {
                    if (!m_chromosomes.ContainsKey(chromosomeWrapper))
                    {
                        m_chromosomes.Add(chromosomeWrapper, chromosomeWrapper);
                    }
                    chromosomeWrapper = m_chromosomes[chromosomeWrapper];
                }
                lock (chromosomeWrapper)
                {
                    var optStatsCache = GetOptStatsCache(
                        constantsToOptimise);
                    chromosomeWrapper.OptStatsCache = optStatsCache;
                    return optStatsCache;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        protected abstract OptStatsCache GetOptStatsCache(ASelfDescribingClass constantsToOptimise);
    }
}

