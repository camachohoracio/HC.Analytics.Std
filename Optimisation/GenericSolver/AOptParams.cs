#region

using System;
using System.Collections.Generic;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public abstract class AOptParams : IDisposable
    {
        #region Properties

        public ASelfDescribingClass OptimisationParams { get; set; }
        public List<string> IntParams { get; private set; }
        public List<string> DblParams { get; private set; }
        public List<string> BlnParams { get; private set; }
        public SortedDictionary<string, double[]> ParamsNormalizerDbl { get; private set; }
        public SortedDictionary<string, int[]> ParamsNormalizerInt { get; private set; }

        #endregion

        protected void GetVariableParams()
        {
            var variables = OptimisationParams.GetStringArr(
                EnumGenericSolver.OptVariables);
            IntParams =
                new List<string>();
            DblParams =
                new List<string>();
            BlnParams =
                new List<string>();
            ParamsNormalizerDbl =
                new SortedDictionary<string, double[]>();
            ParamsNormalizerInt =
                new SortedDictionary<string, int[]>();

            foreach (string strParam in variables)
            {
                var tokens = strParam.Split('|');
                tokens[0] = tokens[0].Trim();
                tokens[1] = tokens[1].Trim();
                tokens[2] = tokens[2].Trim();
                tokens[3] = tokens[3].Trim();

                if (tokens[3].Equals("double"))
                {
                    DblParams.Add(
                        tokens[0]);
                    ParamsNormalizerDbl.Add(
                        tokens[0],
                        new[]
                            {
                                double.Parse(tokens[1]),
                                double.Parse(tokens[2])
                            });
                }
                else if (tokens[3].Equals("int"))
                {
                    IntParams.Add(
                        tokens[0]);
                    ParamsNormalizerInt.Add(
                        tokens[0],
                        new[]
                            {
                                int.Parse(tokens[1]),
                                int.Parse(tokens[2])
                            });
                }
                else if (tokens[3].Equals("bool"))
                {
                    BlnParams.Add(
                        tokens[0]);
                }
                else
                {
                    HCException.Throw("Value type not supported");
                }
            }
        }

        public double DeNormalizeParamDbl(
            string strParamName,
            double dblValue)
        {
            var dblMin = ParamsNormalizerDbl[strParamName][0];
            var dblMax = ParamsNormalizerDbl[strParamName][1];
            var dblDelta = dblMax - dblMin;
            return (dblValue * dblDelta) + dblMin;
        }

        public double NormalizeParamDbl(
            string strParamName,
            double dblValue)
        {
            var dblMin = ParamsNormalizerDbl[strParamName][0];
            var dblMax = ParamsNormalizerDbl[strParamName][1];
            var dblDelta = dblMax - dblMin;
            var dblOut = (dblValue - dblMin) / dblDelta;

            HCException.ThrowIfTrue(dblOut < 0,
                "Wrong initial param value or param out of bounds");

            return dblOut;
        }

        public int NormalizeParamInt(
            string strParamName,
            int intValue)
        {
            HCException.ThrowIfTrue(intValue < ParamsNormalizerInt[strParamName][0] ||
                intValue > ParamsNormalizerInt[strParamName][1],
                "Param out of range");

            int intReturnValue = intValue - ParamsNormalizerInt[strParamName][0];
            return intReturnValue;
        }

        public int DeNormalizeParamInt(
            string strParamName,
            int intValue)
        {
            int intReturnValue = intValue + ParamsNormalizerInt[strParamName][0];
            return intReturnValue;
        }


        public double[] GetIntScaleFactors()
        {
            var dblScaledChromosome = new List<double>();
            for (var i = 0; i < IntParams.Count; i++)
            {
                var strVar = IntParams[i];
                var intMinValue = ParamsNormalizerInt[strVar][0];
                var intMaxValue = ParamsNormalizerInt[strVar][1];
                dblScaledChromosome.Add(
                    intMaxValue - intMinValue);
            }
            return dblScaledChromosome.ToArray();
        }


        ~AOptParams()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            OptimisationParams.Dispose();
            IntParams.Clear();
            DblParams.Clear();
            BlnParams.Clear();
            ParamsNormalizerDbl.Clear();
            ParamsNormalizerInt.Clear();
        }
    }
}

