using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Probability;
using HC.Analytics.Statistics;
using HC.Analytics.TimeSeries.TsStats;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries
{
    public static class GenericForwardBasedVariableSelector
    {
        public static TrainerWrapperFactory DefaultTrainerWrapperFactory { get; private set; }

        static GenericForwardBasedVariableSelector()
        {
            try
            {
                DefaultTrainerWrapperFactory = new TrainerWrapperFactory(EnumTrainerWrapper.LinearRegression);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static List<int> SelectBestVariablesByCorrelationLocal(
            List<double[]> xMap,
            List<double> yMap,
            int intVarsToSelect,
            List<string> featuresNames,
            int intLowerBoundVariable,
            List<int> ignoreList,
            bool blnUseAbsCorrelation = false,
            bool blnUseReturns = true)
        {
            try
            {
                if (featuresNames == null ||
                    featuresNames.Count == 0)
                {
                    return new List<int>();
                }

                //
                // validate
                //
                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid vector size");
                }

                if (xMap.First().Length != featuresNames.Count)
                {
                    throw new HCException("Invalid feature names");
                }

                var yVarsOri0 = yMap.ToList();

                if (xMap.Count == 0 ||
                    yMap.Count == 0)
                {
                    return new List<int>();
                }

                //var missingDates = (from n in xMap
                //                    where !yMap.ContainsKey(n.Key)
                //                    select n).ToList();
                //if (missingDates.Any())
                //{
                //    throw new HCException("Dates do not match");
                //}

                int intVars = xMap.First().Length;
                if (intVarsToSelect > intVars)
                {
                    Logger.Log("Invalid number of vars to select");
                    var results = new List<int>();
                    for (int i = 0; i < intVars; i++)
                    {
                        results.Add(i);
                    }
                    return results;
                }

                if (featuresNames.Count != intVars)
                {
                    throw new HCException("Invalid number of variables names");
                }

                foreach (var kvp in xMap)
                {
                    if ((from n in kvp
                         where double.IsNaN(n) ||
                               double.IsInfinity(n)
                         select n).Any())
                    {
                        throw new HCException("Invalid x feature value!");
                    }
                }
                if ((from n in yMap
                     where double.IsNaN(n) ||
                           double.IsInfinity(n)
                     select n).Any())
                {
                    throw new HCException("Invalid y value!");
                }
                List<double[]> xMapReturn;
                if (blnUseReturns)
                {
                    //
                    // load returns
                    //
                    xMapReturn = TimeSeriesHelper.GetReturns(
                        xMap,
                        1,
                        true);
                }
                else
                {
                    xMapReturn = xMap.ToList();
                }
                List<double[]> xVarsReturns = xMapReturn.ToList();
                List<double> yVarsReturns;
                if (blnUseReturns)
                {
                    var returnY = TimeSeriesHelper.GetReturns(yVarsOri0, 1, true);
                    yVarsReturns =
                        returnY.ToList();
                }
                else
                {
                    yVarsReturns = yVarsOri0.ToList();
                }
                //
                // get var set
                //
                var varSet = new HashSet<int>();
                for (int i = 0; i < intVars; i++)
                {
                    if (ignoreList == null ||
                        !ignoreList.Contains(i))
                    {
                        varSet.Add(i);
                    }
                }

                //
                // initialize with default residuals as y
                //
                var selectedVariablesFinal = new List<int>();

                //
                // add lower bound variable
                //
                if (intLowerBoundVariable >= 0)
                {
                    selectedVariablesFinal.Add(intLowerBoundVariable);
                    if (!varSet.Remove(intLowerBoundVariable))
                    {
                        throw new HCException("Variable not found");
                    }
                } // lower bound variable

                var corrRankings = new List<
                    KeyValuePair<int, double>>();
                foreach (int intCurrVar in varSet)
                {
                    double dblCorr =
                        Correlation.GetCorrelation(
                            yVarsReturns,
                            TimeSeriesHelper.SelectVariable(
                                xVarsReturns,
                                intCurrVar));
                    if (blnUseAbsCorrelation)
                    {
                        dblCorr = Math.Abs(dblCorr);
                    }
                    corrRankings.Add(
                        new KeyValuePair<int, double>(
                            intCurrVar,
                            dblCorr));
                }

                corrRankings.Sort((a, b) => -a.Value.CompareTo(b.Value));

                //
                // add by correlation rankings
                //

                for (int i = 0; i < corrRankings.Count; i++)
                {
                    int intVar = corrRankings[i].Key;
                    if (!selectedVariablesFinal.Contains(intVar))
                    {
                        selectedVariablesFinal.Add(intVar);
                    }
                    if (selectedVariablesFinal.Count >= intVarsToSelect)
                    {
                        break;
                    }
                }
                return selectedVariablesFinal;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<int> SelectBestVariablesByCorrelationLocal<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double> yMap,
            int intVarsToSelect,
            List<string> featuresNames,
            int intLowerBoundVariable,
            List<int> ignoreList,
            bool blnUseAbsCorrelation = false,
            bool blnUseReturns=true)
        {
            try
            {
                if (featuresNames == null ||
                    featuresNames.Count == 0)
                {
                    return new List<int>();
                }

                //
                // validate
                //
                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid vector size");
                }

                if (xMap.First().Value.Length != featuresNames.Count)
                {
                    throw new HCException("Invalid feature names");
                }

                var yVarsOri0 = TimeSeriesHelper.CloneMap(yMap);

                if (xMap.Count == 0 ||
                    yMap.Count == 0)
                {
                    return new List<int>();
                }

                var missingDates = (from n in xMap
                    where !yMap.ContainsKey(n.Key)
                    select n).ToList();
                if (missingDates.Any())
                {
                    throw new HCException("Dates do not match");
                }

                int intVars = xMap.First().Value.Length;
                if (intVarsToSelect > intVars)
                {
                    Logger.Log("Invalid number of vars to select");
                    var results = new List<int>();
                    for (int i = 0; i < intVars; i++)
                    {
                        results.Add(i);
                    }
                    return results;
                }

                if (featuresNames.Count != intVars)
                {
                    throw new HCException("Invalid number of variables names");
                }

                foreach (var kvp in xMap)
                {
                    if ((from n in kvp.Value
                        where double.IsNaN(n) ||
                              double.IsInfinity(n)
                        select n).Any())
                    {
                        throw new HCException("Invalid x feature value!");
                    }
                }
                if ((from n in yMap.Values
                    where double.IsNaN(n) ||
                          double.IsInfinity(n)
                    select n).Any())
                {
                    throw new HCException("Invalid y value!");
                }
                SortedDictionary<T, double[]> xMapReturn;
                if (blnUseReturns)
                {
                    //
                    // load returns
                    //
                    xMapReturn = TimeSeriesHelper.GetReturns(
                        xMap,
                        1,
                        true);
                }
                else
                {
                    xMapReturn = new SortedDictionary<T, double[]>(xMap);
                }
                List<double[]> xVarsReturns = xMapReturn.Values.ToList();
                List<double> yVarsReturns;
                if (blnUseReturns)
                {
                    var returnY = TimeSeriesHelper.GetReturns(yVarsOri0, 1, true);
                    yVarsReturns =
                        returnY.Values.ToList();
                }
                else
                {
                    yVarsReturns = yVarsOri0.Values.ToList();
                }
                //
                // get var set
                //
                var varSet = new HashSet<int>();
                for (int i = 0; i < intVars; i++)
                {
                    if (ignoreList == null ||
                        !ignoreList.Contains(i))
                    {
                        varSet.Add(i);
                    }
                }

                //
                // initialize with default residuals as y
                //
                var selectedVariablesFinal = new List<int>();

                //
                // add lower bound variable
                //
                if (intLowerBoundVariable >= 0)
                {
                    selectedVariablesFinal.Add(intLowerBoundVariable);
                    if (!varSet.Remove(intLowerBoundVariable))
                    {
                        throw new HCException("Variable not found");
                    }
                } // lower bound variable

                var corrRankings = new List<
                    KeyValuePair<int, double>>();
                foreach (int intCurrVar in varSet)
                {
                    double dblCorr =
                        Correlation.GetCorrelation(
                            yVarsReturns,
                            TimeSeriesHelper.SelectVariable(
                                xVarsReturns,
                                intCurrVar));
                    if (blnUseAbsCorrelation)
                    {
                        dblCorr = Math.Abs(dblCorr);
                    }
                    corrRankings.Add(
                        new KeyValuePair<int, double>(
                            intCurrVar,
                            dblCorr));
                }

                corrRankings.Sort((a, b) => -a.Value.CompareTo(b.Value));

                //
                // add by correlation rankings
                //

                for (int i = 0; i < corrRankings.Count; i++)
                {
                    int intVar = corrRankings[i].Key;
                    if (!selectedVariablesFinal.Contains(intVar))
                    {
                        selectedVariablesFinal.Add(intVar);
                    }
                    if (selectedVariablesFinal.Count >= intVarsToSelect)
                    {
                        break;
                    }
                }
                return selectedVariablesFinal;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<int> SelectBestVariablesLocalGeneric<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double> yMap,
            int intVarsToSelect,
            List<string> featuresNames,
            int intLowerBoundVariable,
            List<int> ignoreList,
            bool blnVerbose,
            bool blnUseAbsCorrelation,
            TrainerWrapperFactory trainerWrapperFactory = null,
            bool blnAddMissingVariables = true,
            int intNumClasses = 0)
        {
            try
            {
                if (featuresNames == null ||
                    featuresNames.Count == 0)
                {
                    return new List<int>();
                }

                if(trainerWrapperFactory == null)
                {
                    trainerWrapperFactory = DefaultTrainerWrapperFactory;
                }

                //
                // validate
                //
                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid vector size");
                }

                if (xMap.First().Value.Length != featuresNames.Count)
                {
                    throw new HCException("Invalid feature names");
                }

                var yVarsOri0 = TimeSeriesHelper.CloneMap(yMap);

                if (xMap.Count == 0 ||
                   yMap.Count == 0)
                {
                    return new List<int>();
                }

                var missingDates = (from n in xMap
                                    where !yMap.ContainsKey(n.Key)
                                    select n).ToList();
                if (missingDates.Any())
                {
                    throw new HCException("Dates do not match");
                }

                int intVars = xMap.First().Value.Length;
                if (intVarsToSelect > intVars)
                {
                    Logger.Log("Invalid number of vars to select");
                    var results = new List<int>();
                    for (int i = 0; i < intVars; i++)
                    {
                        results.Add(i);
                    }
                    return results;
                }

                if (featuresNames.Count != intVars)
                {
                    throw new HCException("Invalid number of variables names");
                }

                foreach (var kvp in xMap)
                {
                    if((from n in kvp.Value
                    where double.IsNaN(n) ||
                    double.IsInfinity(n)
                    select n).Any())
                    {
                        throw new HCException("Invalid x feature value!");
                    }
                }
                if ((from n in yMap.Values
                     where double.IsNaN(n) ||
                     double.IsInfinity(n)
                     select n).Any())
                {
                    throw new HCException("Invalid y value!");
                }


                List<double[]> xVars = xMap.Values.ToList();
                var xVarsTmp = new List<double[]>(xVars);
                List<double> yVars = yMap.Values.ToList();
                List<double> yVarsOri = yVarsOri0.Values.ToList();

                //
                // get var set
                //
                var varSet = new HashSet<int>();
                for (int i = 0; i < intVars; i++)
                {
                    if (ignoreList == null ||
                        !ignoreList.Contains(i))
                    {
                        varSet.Add(i);
                    }
                }

                //
                // initialize with default residuals as y
                //
                var prevResiduals = new List<double>(yVars);
                var selectedVariablesFinal = new List<int>();

                //
                // add lower bound variable
                //
                List<double> prevErrorsSquare = null;
                ITrainerWrapper prevTrainer = null;
                double dblPrevAdjustedCoeffDeterm = double.NaN;
                if (intLowerBoundVariable >= 0)
                {
                    selectedVariablesFinal.Add(intLowerBoundVariable);
                    if (!varSet.Remove(intLowerBoundVariable))
                    {
                        throw new HCException("Variable not found");
                    }

                    List<double[]> xVarsSubset = TimeSeriesHelper.SelectVariables(
                        xVars,
                        selectedVariablesFinal);
                    prevTrainer = trainerWrapperFactory.Build(
                        xVarsSubset, 
                        yVars,
                        intNumClasses,
                        false);
                    prevResiduals = prevTrainer.GetErrors();
                    prevErrorsSquare = (from n in prevResiduals select n * n).ToList();
                    dblPrevAdjustedCoeffDeterm = TimeSeriesHelper.GetAdjustedCoeffDeterm(
                        yVarsOri,
                        prevResiduals,
                        prevTrainer.Dimesions - 1);
                } // lower bound variable

                //
                // iterate variables, until x num of variables are found
                //
                var rankingsMap = new Dictionary<int, double>();
                string strIgnoreReason = string.Empty;
                var correlationRanking = new List<int>();
                while (varSet.Count > 0)
                {
                    //
                    // get correlation with residuals
                    //
                    int intSelectedCorrelVar = GetMostCorrelatedSymbol(
                        varSet, 
                        xVarsTmp, 
                        intNumClasses < 2 ?
                            prevResiduals :
                            yVars.ToList(),
                        DefaultTrainerWrapperFactory, // hack, we use the generic because it is faster!
                        blnUseAbsCorrelation);

                    correlationRanking.Add(intSelectedCorrelVar);

                    if(!varSet.Remove(intSelectedCorrelVar))
                    {
                        throw new HCException("var could not be removed");
                    }

                    //
                    // get current model error
                    //
                    List<double[]> xVarsSubset = TimeSeriesHelper.SelectVariables(
                        xVars,
                        selectedVariablesFinal.Union(
                            new[]
                            {
                                intSelectedCorrelVar
                            }).ToList());

                    ITrainerWrapper currTrainerWrapper = trainerWrapperFactory.Build(
                        xVarsSubset, 
                        yVars,
                        intNumClasses,
                        false);

                    List<double> currResiduals = currTrainerWrapper.GetErrors();

                    List<double> errorsSquare = (from n in currResiduals
                                                 select n * n).ToList();

                    double dblCurrAdjustedCoeffDeterm = TimeSeriesHelper.GetAdjustedCoeffDeterm(
                        yVarsOri,
                        currResiduals,
                        currTrainerWrapper.Dimesions - 1);
                    string strMessage;
                    if (prevErrorsSquare != null)
                    {
                        double dblAvgErrSquare = errorsSquare.Average();
                        double dblPrevAvgErrSquare = prevErrorsSquare.Average();

                        bool blnAddVariable;
                        if (dblAvgErrSquare > dblPrevAvgErrSquare)
                        {
                            strIgnoreReason = "dblAvgErr[" +
                                              dblAvgErrSquare + "] > dblPrevAvgErr[" +
                                              dblPrevAvgErrSquare + "]";
                            blnAddVariable = false;
                        }
                        else if (HypothesisTests.IsMeanDiffEqualTo(
                            0.05,
                            0,
                            prevErrorsSquare,
                            errorsSquare))
                        {
                            double dblF;
                            double dblCriticalValue;
                            if (!HypothesisTests.IsModel2BetterThanModel1(
                                prevResiduals.Count,
                                dblPrevAvgErrSquare,
                                dblAvgErrSquare,
                                prevTrainer.Dimesions - 1,
                                currTrainerWrapper.Dimesions - 1,
                                out dblF,
                                out dblCriticalValue))
                            {
                                // here is the current error lower than the previous, consider a rank
                                double dblImprovement = (dblPrevAvgErrSquare - dblAvgErrSquare) / dblPrevAvgErrSquare; // the greater the better
                                rankingsMap[intSelectedCorrelVar] = dblImprovement;
                                strIgnoreReason = "Old model is better than new";
                                blnAddVariable = false;
                            }
                            else
                            {
                                blnAddVariable = true;
                            }

                        }
                        else
                        {
                            blnAddVariable = true;
                        }


                        if (blnAddVariable)
                        {
                            if (dblCurrAdjustedCoeffDeterm < dblPrevAdjustedCoeffDeterm)
                            {
                                strIgnoreReason = "dblCurrAdjustedCoeffDeterm[" + dblCurrAdjustedCoeffDeterm +
                                    "] < dblPrevAdjustedCoeffDeterm[" + dblPrevAdjustedCoeffDeterm + "]";
                                blnAddVariable = false;
                            }
                        }

                        if (blnVerbose)
                        {
                            strMessage = typeof(GenericForwardBasedVariableSelector).Name +  
                                " mean error diff [" + 
                                (dblPrevAvgErrSquare - dblAvgErrSquare) + "]";
                            Console.WriteLine(strMessage);
                            Logger.Log(strMessage);
                        }

                        if (blnAddVariable)
                        {
                            selectedVariablesFinal.Add(intSelectedCorrelVar);
                            prevErrorsSquare = errorsSquare;
                            prevTrainer = currTrainerWrapper;
                            dblPrevAdjustedCoeffDeterm = dblCurrAdjustedCoeffDeterm;
                            //List<double> selectedVarVector = TimeSeriesHelper.GetVector(
                            //    xVarsTmp, 
                            //    intSelectedCorrelVar);
                            //var trainerWrapper = trainerWrapperFactory.Build(
                            //    selectedVarVector, 
                            //    intNumClasses < 2 ? 
                            //        prevResiduals :
                            //        yVars.ToList(),
                            //    intNumClasses);

                            //prevResiduals = trainerWrapper.GetErrors();
                            prevResiduals = currResiduals;

                            if (blnVerbose)
                            {
                                double dblAvgError = errorsSquare.Average();
                                if(double.IsNaN(dblAvgError))
                                {
                                    throw new HCException("Invalid avg error");
                                }
                                strMessage = typeof(GenericForwardBasedVariableSelector).Name +  
                                    " added variable [" + intSelectedCorrelVar + "][" +
                                             featuresNames[intSelectedCorrelVar] +
                                             "]. Avg error = " + dblAvgError;
                                Console.WriteLine(strMessage);
                                Logger.Log(strMessage);
                            }
                        }
                        else
                        {
                            if (blnVerbose)
                            {
                                strMessage = typeof(GenericForwardBasedVariableSelector).Name +
                                    " ignored variable [" + intSelectedCorrelVar + "]" + "[" +
                                             featuresNames[intSelectedCorrelVar] + "]";
                                Console.WriteLine(strMessage);
                                Logger.Log(strMessage);
                                strMessage = typeof(GenericForwardBasedVariableSelector).Name +  
                                    " ignore reason [" + strIgnoreReason + "]";
                                Console.WriteLine(strMessage);
                                Logger.Log(strMessage);
                            }
                        }
                    }
                    else
                    {
                        //
                        // lower bound variable
                        //
                        selectedVariablesFinal.Add(intSelectedCorrelVar);
                        prevErrorsSquare = errorsSquare;
                        prevTrainer = currTrainerWrapper;
                        dblPrevAdjustedCoeffDeterm = dblCurrAdjustedCoeffDeterm;
                        //List<double> selectedVarVector = TimeSeriesHelper.GetVector(
                        //    xVarsTmp, 
                        //    intSelectedCorrelVar);
                        //ITrainerWrapper trainerWrapper = trainerWrapperFactory.Build(
                        //    selectedVarVector, 
                        //    intNumClasses < 2 ?
                        //    prevResiduals :
                        //    yVars.ToList(),
                        //    intNumClasses);
                        //prevResiduals = trainerWrapper.GetErrors();
                        prevResiduals = currResiduals;

                        if (blnVerbose)
                        {
                            strMessage = typeof(GenericForwardBasedVariableSelector).Name +  
                                " added variable [" + intSelectedCorrelVar + "][" +
                                         featuresNames[intSelectedCorrelVar] + "]" +
                                         ". Avg error [" + errorsSquare.Average() + "]";
                            Console.WriteLine(strMessage);
                            Logger.Log(strMessage);
                        }
                    }

                    if (selectedVariablesFinal.Count >= intVarsToSelect)
                    {
                        break;
                    }
                    int intPerc = selectedVariablesFinal.Count * 100 / intVarsToSelect;
                    if (blnVerbose)
                    {
                        strMessage = typeof(GenericForwardBasedVariableSelector).Name +  
                            " jobsDone [" + intPerc + "]% [" + selectedVariablesFinal.Count + "/" +
                                      intVarsToSelect + "] .  [" + selectedVariablesFinal.Count +
                                     "] variables selected. [" + varSet.Count + "] variables to select [" +
                                     Math.Round(prevErrorsSquare.Average(), 4) + "] error";

                        Console.WriteLine(strMessage);
                        Logger.Log(strMessage);
                    }
                }
                int intTotalVars = selectedVariablesFinal.Count;
                List<KeyValuePair<int, double>> rankingsList = rankingsMap.ToList();
                rankingsList.Sort((a, b) => -a.Value.CompareTo(b.Value));
                for (int i = intTotalVars, intCounter = 0; i < intVarsToSelect; i++, intCounter++)
                {
                    if (intCounter < rankingsList.Count)
                    {
                        selectedVariablesFinal.Add(rankingsList[intCounter].Key);
                    }
                }

                //
                // add by correlation rankings
                //
                if (blnAddMissingVariables)
                {
                    if (selectedVariablesFinal.Count < intVarsToSelect)
                    {
                        for (int i = 0; i < correlationRanking.Count; i++)
                        {
                            int intVar = correlationRanking[i];
                            if (!selectedVariablesFinal.Contains(intVar))
                            {
                                selectedVariablesFinal.Add(intVar);
                            }
                            if (selectedVariablesFinal.Count >= intVarsToSelect)
                            {
                                break;
                            }
                        }
                    }
                }
                return selectedVariablesFinal;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static int GetMostCorrelatedSymbol(
            HashSet<int> varSet, 
            List<double[]> xVarsTmp, 
            List<double> prevResiduals,
            TrainerWrapperFactory trainerWrapperFactory,
            bool blnUseAbsCorrelation)
        {
            try
            {
                int intSelectedCorrelVar = TimeSeriesHelper.GetMostCorrelatedSymbol(
                    varSet,
                    xVarsTmp,
                    prevResiduals,
                    trainerWrapperFactory,
                    blnUseAbsCorrelation);
                return intSelectedCorrelVar;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }
    }
}
