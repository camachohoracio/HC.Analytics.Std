using System;
using System.Collections.Generic;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public static class RecursiveNnetTrainerWrapper
    {
        public static double Predict(
            double[] feature,
            BasicNetwork network)
        {
            try
            {
                IMLData input = new BasicNeuralData(feature);
                IMLData predictData = network.Compute(input);
                return predictData[0];
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static BasicNetwork DoTrain(
            List<double[]> xData,
            List<double> yData,
            double[] importanceWeights)
        {
            try
            {
                var basicMlDataSet =
                    new BasicMLDataSet(
                        xData.ToArray(),
                        (from n in yData select new[] { n }).ToArray());
                int i = 0;
                if (importanceWeights != null)
                {
                    foreach (IMLDataPair mlDataPair in basicMlDataSet)
                    {
                        mlDataPair.Significance = importanceWeights[i];
                        i++;
                    }
                }
                var network =
                    (BasicNetwork)CreateElmanNetwork(
                    basicMlDataSet.InputSize);

                TrainNetwork(
                    "Elman",
                    network,
                    basicMlDataSet,
                    "Leven");

                return network;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static IMLMethod CreateElmanNetwork(int input)
        {
            try
            {
                // construct an Elman type network
                var pattern = new ElmanPattern
                                  {
                                      ActivationFunction = new ActivationSigmoid(),
                                      InputNeurons = input
                                  };
                pattern.AddHiddenLayer(6);
                pattern.OutputNeurons = 1;
                return pattern.Generate();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double TrainNetwork(
            String what,
            BasicNetwork network,
            IMLDataSet trainingSet,
            string strMethod)
        {
            try
            {
                // train the neural network
                ICalculateScore score = new TrainingSetScore(trainingSet);
                IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
                IMLTrain trainMain;
                if (strMethod.Equals("Leven"))
                {
                    trainMain = new LevenbergMarquardtTraining(network, trainingSet);
                }
                else
                {
                    trainMain = new Backpropagation(network, trainingSet);
                }

                var stop = new StopTrainingStrategy();
                trainMain.AddStrategy(new Greedy());
                trainMain.AddStrategy(new HybridStrategy(trainAlt));
                trainMain.AddStrategy(stop);

                int epoch = 0;
                while (!stop.ShouldStop())
                {
                    trainMain.Iteration();
                    epoch++;
                }
                return trainMain.Error;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

    }
}
