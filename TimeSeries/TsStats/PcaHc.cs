using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class PcaHc
    {
        #region Properties

        public double[,] Basis { get; private set; }
        public double[] Variance { get; private set; }

        #endregion

        #region Constructors

        public PcaHc(double[,] data)
        {
            GetBasis(data);
        }

        #endregion

        public static void DoTest()
        {
            var data = new[]
                           {
                               "2.5,2.4",
                               "0.5,0.7",
                               "2.2,2.9",
                               "1.9,2.2",
                               "3.1,3",
                               "2.3,2.7",
                               "2,1.6",
                               "1,1.1",
                               "1.5,1.6",
                               "1.1,0.9"
                           };

            const int intVars = 2;
            const int intNumofComponents = 1;

            var parsedData = new double[data.Length, intVars];
            int intSamples0 = data.Length;
            var means = new double[intVars];
            for (int i = 0; i < intSamples0; i++)
            {
                string[] toks = data[i].Split(',');
                parsedData[i, 0] = double.Parse(toks[0]);
                parsedData[i, 1] = double.Parse(toks[1]);
                means[0] += parsedData[i, 0];
                means[1] += parsedData[i, 1];
            }
            means[0] /= intSamples0;
            means[1] /= intSamples0;

            for (int i = 0; i < intSamples0; i++)
            {
                parsedData[i, 0] -= means[0];
                parsedData[i, 1] -= means[1];
            }
            var testPca = new PcaHc(parsedData);
            double[,] selectedBasis;
            double[,] newData = testPca.GetPcaData(
                parsedData, 
                intNumofComponents,
                out selectedBasis);

            Console.WriteLine(newData);
        }

        private void GetBasis(double[,] data)
        {
            int intVars = data.GetLength(1);
            int intSamples = data.GetLength(0);
            int intInfo = 0;
            var variance = new double[0];
            var basis = new double[0, 0];
            alglib.pca.pcabuildbasis(data, intSamples, intVars, ref intInfo, ref variance, ref basis);
            Variance = variance;
            Basis = basis;
            if (intInfo != 1)
            {
                throw new Exception("Invalid pca");
            }
        }

        public double[,] GetPcaData(
            double[,] data,
            int intNumofComponents,
            out double[,] selectedBasis)
        {
            selectedBasis = GetPcaBasis(intNumofComponents);
            double[,] newData = ApplyPca(data, selectedBasis);
            return newData;
        }

        public double[,] GetPcaBasis(int intNumofComponents)
        {
            int intVars = Basis.GetLength(0);
            double[,] selectedBasis = GetSelectedBasis(
                intNumofComponents,
                intVars);
            return selectedBasis;
        }

        private double[,] GetSelectedBasis(int intNumofComponents, int intVars)
        {
            var selectedBasis = new double[intNumofComponents, intVars];
            for (int i = 0; i < intNumofComponents; i++)
            {
                for (int j = 0; j < intVars; j++)
                {
                    selectedBasis[i, j] = Basis[j, i];
                }
            }
            return selectedBasis;
        }

        public static double[,] ApplyPca(
            double[,] data, 
            double[,] selectedBasis)
        {
            int intVars = data.GetLength(1);
            int intSamples = data.GetLength(0);
            int intNumofComponents = selectedBasis.GetLength(0);
            var newData = new double[intSamples,intNumofComponents];
            for (int i = 0; i < intSamples; i++)
            {
                var currRow = new double[intVars];
                for (int k = 0; k < intVars; k++)
                {
                    currRow[k] = data[i, k];
                }
                for (int j = 0; j < intNumofComponents; j++)
                {
                    double dblSum = 0;
                    for (int k = 0; k < intVars; k++)
                    {
                        dblSum += data[i, k] * selectedBasis[j,k];
                    }
                    newData[i, j] = dblSum;
                }
            }
            return newData;
        }

        public static double[] ConvertRow(
            double[] data,
            double[,] selectedBasis)
        {
            int intNumofComponents = selectedBasis.GetLength(0);
            int intVars = data.Length;
            var result = new double[intNumofComponents];
            for (int j = 0; j < intNumofComponents; j++)
            {
                double dblSum = 0;
                for (int k = 0; k < intVars; k++)
                {
                    dblSum += data[k] * selectedBasis[j, k];
                }
                result[j] = dblSum;
            }
            return result;
        }
    }
}

