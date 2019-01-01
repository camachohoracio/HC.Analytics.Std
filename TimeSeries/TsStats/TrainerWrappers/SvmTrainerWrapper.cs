using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class SvmTrainerWrapper : ATrainerWrapper
    {
        private List<MulticlassSupportVectorMachine<Gaussian>> m_ensembleList;

        public SvmTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses,
            bool blnVerbose = true,
            int intThreads = 40,
            int intEnsembleSize = ENSEMBLE_SIZE) :
            base(xData,
                yData,
                intNumClasses,
                blnVerbose,
                intThreads,
                intEnsembleSize)
        {
            LoadEnsemble();
        }

        private void LoadEnsemble()
        {
            try
            {
                m_ensembleList = new List<MulticlassSupportVectorMachine<Gaussian>>(m_intEnsembleSize);
                if (m_intEnsembleSize == 1)
                {
                    m_ensembleList.Add(
                        DoTraining(
                        m_xData,
                        m_yData,
                        m_intNumClasses));
                }
                else
                {
                    for (int i = 0; i < m_intEnsembleSize; i++)
                    {
                        //
                        // sample with replacement. Even if sample size is 1, it is good to have different sample sets for each training instance
                        //
                        List<double[]> xSample;
                        List<double> ySample;
                        TimeSeriesHelper.SelectRandomSampleData(
                            m_xData,
                            m_yData,
                            1,
                            out xSample,
                            out ySample);
                        m_ensembleList.Add(
                            DoTraining(
                                xSample,
                                ySample,
                                m_intNumClasses));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static MulticlassSupportVectorMachine<Gaussian> DoTraining(
                        List<double[]> xData,
                        List<double> yData,
            int intNumClasses)
        {
            try
            {
                double[][] inputs = xData.ToArray();
                //ISupportVectorMachine svm;
                double dblError= 0;

                //if (intNumClasses == 2)
                //{
                //    svm = new SupportVectorMachine(xData.First().Length);
                //    if ((from n in yData
                //         where n >= intNumClasses
                //         select n).Any())
                //    {
                //        throw new HCException("Zero class is not allowed");
                //    }
                //    int[] outputs = (from n in yData select n == 0 ? -1 : 1).ToArray();
                //    // TODO, check that this class translation makes sense
                //    // Create a new linear-SVM for two inputs (a and b)

                //    // Create a L2-regularized L2-loss support vector classification
                //    var teacher = new LinearDualCoordinateDescent((SupportVectorMachine) svm, inputs, outputs)
                //    {
                //        Loss = Loss.L2,
                //        Complexity = 1000,
                //        Tolerance = 1e-5
                //    };
                //    // Run the learning algorithm
                //    dblError = teacher.Run(computeError: true);
                //}
                //else if(intNumClasses > 2)
                //{
                    int[] outputs = (from n in yData select (int)n).ToArray();
                    // Create the multi-class learning algorithm for the machine
                    var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                    {
                        // Configure the learning algorithm to use SMO to train the
                        //  underlying SVMs in each of the binary class subproblems.
                        Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                        {
                            // Estimate a suitable guess for the Gaussian kernel's parameters.
                            // This estimate can serve as a starting point for a grid search.
                            UseKernelEstimation = true
                        }
                    };

                    // Learn a machine
                    MulticlassSupportVectorMachine<Gaussian> machine = 
                        teacher.Learn(inputs, outputs);


                    // Create the multi-class learning algorithm for the machine
                    var calibration = new MulticlassSupportVectorLearning<Gaussian>()
                    {
                        Model = machine, // We will start with an existing machine

                        // Configure the learning algorithm to use SMO to train the
                        //  underlying SVMs in each of the binary class subproblems.
                        Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                        {
                            Model = param.Model // Start with an existing machine
                        }
                    };


                    // Configure parallel execution options
                    calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

                    // Learn a machine
                    calibration.Learn(inputs, outputs);

                //}
                //else
                //{
                //    svm = new SupportVectorMachine(xData.First().Length);
                //    double[] outputs = yData.ToArray();

                //    // Create the linear regression coordinate descent teacher
                //    var learn = new LinearRegressionCoordinateDescent((SupportVectorMachine)svm, inputs, outputs)
                //    {
                //        Complexity = 10000000,
                //        Epsilon = 1e-10
                //    };

                //    // Run the learning algorithm
                //    dblError = learn.Run();
                //}
                Console.WriteLine("Finish training svn with error [" + dblError + "]");
                return machine;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public override double Forecast(double[] dblX)
        {
            try
            {
                if (m_ensembleList == null)
                {
                    return 0;
                }

               {
                    var votes = new List<int>(m_ensembleList.Count + 2);
                    for (int i = 0; i < m_ensembleList.Count; i++)
                    {
                        int intClass = m_ensembleList[i].Decide(
                            dblX);
                        votes.Add(intClass < 0 ? 0 : 1);
                    }
                    return TimeSeriesHelper.GetMostVotedClass(votes);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
