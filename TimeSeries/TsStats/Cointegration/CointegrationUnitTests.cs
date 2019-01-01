using System.Collections.Generic;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Core.ConfigClasses;
using NUnit.Framework;

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public static class CointegrationUnitTests
    {
        [SetUp]
        public static void SetupTests()
        {
            HCConfig.SetConfigDir(@"C:\HC\Config");
            //AssemblyCache.Initialize();
            //KnownTypesCache.LoadKnownTypes();
            //TsDataProviderHelper.LoadDataProvidersTypes();
        }
        
        [Test]
        public static void TestRandom()
        {
            var randomNormal = new List<double>();
            for (int i = 0; i < 10000; i++)
            {
                randomNormal.Add(UnivNormalDistStd.NextDouble_static());
            }
            bool blnIsCointegrated = AllCointegrationTests.IsCointegrated(randomNormal.ToArray());
            Assert.IsTrue(blnIsCointegrated);
            bool blnIsNormallyDistr = NormalDistrHypTest.IsNormallyDistributed(randomNormal);
            Assert.IsTrue(blnIsNormallyDistr);
        }

        [Test]
        public static void Test1()
        {
            var xValues = new[]
                              {
                                  6109.58,
                                  6157.84,
                                  5850.22,
                                  5976.63,
                                  6382.12,
                                  6437.74,
                                  6877.68,
                                  6611.79,
                                  7040.23,
                                  6842.36,
                                  6512.78,
                                  6699.44,
                                  6700.2,
                                  7092.49,
                                  7558.5,
                                  7664.99,
                                  7589.78,
                                  7366.89,
                                  6931.43,
                                  5530.71,
                                  5611.9,
                                  6208.28,
                                  6343.87,
                                  6485.94
                              };

            double dblTStat;
            double dblTestValue;
            bool blnIsItStationary = DurbinWatsonTest.CheckIsStationary(
                xValues,
                0.05,
                out dblTStat,
                out dblTestValue);

            Assert.IsFalse(blnIsItStationary);
        }

        [Test]
        public static void Test2()
        {
            var xVector = new List<double[]>(
                new[]
                    {
                        new double[] {75},
                        new double[] {78},
                        new double[] {80},
                        new double[] {82},
                        new double[] {84},
                        new double[] {88},
                        new double[] {93},
                        new double[] {97},
                        new double[] {99},
                        new double[] {104},
                        new double[] {109},
                        new double[] {115},
                        new double[] {120},
                        new double[] {127},
                        new double[] {135},
                        new double[] {144},
                        new double[] {153},
                        new double[] {161},
                        new double[] {170},
                        new double[] {182}
                    });
            var yVector = new List<double>(
                new double[]
                    {
                        3083,
                        3149,
                        3218,
                        3239,
                        3295,
                        3374,
                        3475,
                        3569,
                        3597,
                        3725,
                        3794,
                        3959,
                        4043,
                        4194,
                        4318,
                        4493,
                        4683,
                        4850,
                        5005,
                        5236
                    });

            double dblTStat;
            double dblTestValue;
            bool blnIsItStationary = DurbinWatsonTest.CheckIsStationary(
                xVector,
                yVector,
                0.05,
                out dblTStat,
                out dblTestValue);

            Assert.IsFalse(blnIsItStationary);
        }

        [Test]
        public static void Test3()
        {
            //
            // a normally distributed sample
            //
            var data = new List<double>(
                new[]
                    {
                        0.719947533,
                        -1.545123215,
                        -0.956236174,
                        -0.639734809,
                        -1.039268439,
                        0.128152801,
                        -1.882061269,
                        0.142223823,
                        0.724911492,
                        0.95250443,
                        0.946320709,
                        1.439309523,
                        -0.456805714,
                        -0.112248741,
                        -0.171543809,
                        -1.103160816,
                        -2.170246182,
                        0.348489555,
                        1.463836117,
                        -0.199022168,
                        -0.453967289,
                        -0.254109917,
                        -0.359901242,
                        -0.09359429,
                        0.507699463,
                        -1.041751001,
                        0.454296695,
                        0.534336636,
                        -2.407686485,
                        0.685393354,
                        -0.512814093,
                        -0.476692512,
                        -2.053449748,
                        -1.732586569,
                        -0.446085656,
                        0.820625072,
                        0.929072888,
                        -0.924572037,
                        0.914757217,
                        -1.529149934,
                        -0.737237727,
                        -1.505739988,
                        0.447215237,
                        0.316776636,
                        1.423529564,
                        1.020196672,
                        -0.059650528,
                        0.634696765,
                        -1.548346323,
                        -1.006437582,
                        0.058132875,
                        0.584486095,
                        0.626691966,
                        -0.364050458,
                        -0.090166255,
                        -1.146114549,
                        -1.674999038,
                        -0.067708604,
                        -0.547294403,
                        0.572289471,
                        0.339011329,
                        0.459702568,
                        0.146038045,
                        0.270095495,
                        -0.532342713,
                        -1.308791089,
                        -0.048867174,
                        -0.434992412,
                        -0.021952746,
                        -0.75797764,
                        0.519166863,
                        -0.03098529,
                        -0.651898002,
                        -1.631068304,
                        -0.744699661,
                        -0.930281457,
                        -0.416332268,
                        0.598575445,
                        1.502993525,
                        1.343629054,
                        -0.717283865,
                        -0.719833219,
                        -2.182356992,
                        0.675415114,
                        0.818029792,
                        -1.193197886,
                        0.675786803,
                        0.491378384,
                        0.436093707,
                        0.310117893,
                        -0.193664047,
                        -0.768882735,
                        -0.758704714,
                        -0.583804575,
                        -1.131413054,
                        0.618737958,
                        1.800089996,
                        -0.608123452,
                        0.059132189,
                        -1.397019415,
                        -0.705033969,
                        -0.317276267,
                        -0.645086985,
                        0.200293703,
                        -0.807995851,
                        -2.640168165,
                        -1.514286233,
                        -1.706689678,
                        0.317596181,
                        1.066799134,
                        -0.283793831,
                        0.350940496,
                        -2.030542988,
                        1.523507299,
                        -0.613941175,
                        1.006076477,
                        0.337635259,
                        -0.555453926,
                        -0.277163052,
                        -0.18048447,
                        0.438927079,
                        -0.891043413,
                        1.554575721,
                        -1.474964032,
                        0.721856611,
                        0.737464394,
                        0.13149633,
                        0.7587363,
                        -0.183189329,
                        0.75472954,
                        -0.255817263,
                        -0.736027048,
                        1.474780671,
                        1.152868234,
                        -0.428443526,
                        -0.874939079,
                        0.844495446,
                        -0.332878908,
                        1.17180452,
                        -1.614192024,
                        -1.068418888,
                        -0.935821047,
                        -0.326381383,
                        1.56490679,
                        3.235340255,
                        0.172466172,
                        -1.927790618,
                        1.143043018,
                        -0.97149285,
                        0.773939431,
                        0.066711196,
                        -0.262838506,
                        -0.378679746,
                        0.11580841,
                        0.371061924,
                        -2.008794301,
                        -0.934879698,
                        0.431370494,
                        -0.290920286,
                        0.324319046,
                        0.093262904,
                        0.366649688,
                        1.090242112,
                        0.987815221,
                        -1.047082695,
                        0.967600305,
                        2.54593722,
                        -1.278038344,
                        0.416981789,
                        0.062707441
                    });
            double dblTStat;
            const double dblConfidence = 0.05;
            double dblTestValue;
            bool blnCheck = AllCointegrationTests.IsAnyCointegrated(
                                           data.ToArray(),
                                           dblConfidence,
                                           out dblTStat,
                                           out dblTestValue);

            Assert.IsTrue(blnCheck);
        }
    }
}
