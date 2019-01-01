#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptChromosomeFactory : IDisposable
    {
        #region Properties

        public AOptParams OptParams { get; private set; }

        #endregion

        #region Constructors

        public OptChromosomeFactory(
            AOptParams optParams)
        {
            OptParams = optParams;
        }

        #endregion

        #region Public

        public static OptStatsCache GetOptStatsCache(
            Individual individual,
            IOptInstFctry optInstFctry,
            OptChromosomeFactory optChromosomeFactory)
        {
            ASelfDescribingClass constantsToOptimise;
            OptChromosomeWrapper chromosomeWrapper;

            //
            // build a new chromosome and retrieve the data in a thread-safe manner
            //
            optChromosomeFactory.GetMixedChromosome(
                individual,
                out chromosomeWrapper,
                out constantsToOptimise);

            OptStatsCache optStatsCache;
            if (!optInstFctry.ContainsStatsCache(
                chromosomeWrapper,
                out optStatsCache))
            {
                //
                // run trading model
                //
                optStatsCache = optInstFctry.GetOptStatsCache(
                    chromosomeWrapper,
                    constantsToOptimise);
            }
            return optStatsCache;
        }

        #endregion

        #region Private

        private void GetMixedChromosome(
            Individual individual,
            out OptChromosomeWrapper optChromosomeWrapper,
            out ASelfDescribingClass constantsToOptimise)
        {
            constantsToOptimise = new SelfDescribingClass();
            constantsToOptimise.SetClassName(GetType().Name + "_ConstantsToOptimise");
            var dblChromosome = new List<double>();
            var intChromosome = new List<int>();
            var blnChromosome = new List<bool>();

            Individual intIndividual;
            Individual blnIndividual;
            Individual dblIndividual;
            OptHelper.GetIndividuals(
                individual, 
                out intIndividual, 
                out blnIndividual, 
                out dblIndividual);

            //
            // get integer variables
            //
            for (var i = 0; i < OptParams.IntParams.Count; i++)
            {
                var strVar = OptParams.IntParams[i];
                var intChromosomeValue =
                    intIndividual.GetChromosomeValueInt(i);
                var intScaledChromosome =
                    ScaleParamInt(
                        OptParams.ParamsNormalizerInt,
                        strVar,
                        intChromosomeValue);
                constantsToOptimise.SetIntValue(strVar, intScaledChromosome);
                intChromosome.Add(intScaledChromosome);
            }

            //
            // get continuous variables
            //
            for (var i = 0; i < OptParams.DblParams.Count; i++)
            {
                var strVar = OptParams.DblParams[i];
                var dblChromosomeValue =
                    dblIndividual.GetChromosomeValueDbl(i);
                var dblScaledChromosome =
                    ScaleParamDbl(
                        OptParams.ParamsNormalizerDbl,
                        strVar,
                        dblChromosomeValue);
                constantsToOptimise.SetDblValue(strVar, dblScaledChromosome);
                dblChromosome.Add(dblScaledChromosome);
            }

            //
            // get binary variables
            //
            for (var i = 0; i < OptParams.BlnParams.Count; i++)
            {
                var strVar = OptParams.BlnParams[i];
                var blnChromosomeValue =
                    blnIndividual.GetChromosomeValueBln(i);
                constantsToOptimise.SetBlnValue(strVar, blnChromosomeValue);
                blnChromosome.Add(blnChromosomeValue);
            }

            optChromosomeWrapper = new OptChromosomeWrapper(
                dblChromosome.ToArray(),
                intChromosome.ToArray(),
                blnChromosome.ToArray());
        }

        public static int ScaleParamInt(
            SortedDictionary<string, int[]> paramsNormalizer,
            string strParamName,
            int intParam)
        {
            var intMinValue = paramsNormalizer[strParamName][0];
            return intMinValue + intParam;
        }

        public static double ScaleParamDbl(
            SortedDictionary<string, double[]> paramsNormalizer,
            string strParamName,
            double dblParam)
        {
            var dblMin = paramsNormalizer[strParamName][0];
            var dblMax = paramsNormalizer[strParamName][1];
            HCException.ThrowIfTrue(dblMax < dblMin,
                "Invald min/max values");
            
            var dblDelta = dblMax - dblMin;
            return dblMin + dblParam*dblDelta;
        }

        #endregion

        ~OptChromosomeFactory()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            OptParams.Dispose();
        }
    }
}
