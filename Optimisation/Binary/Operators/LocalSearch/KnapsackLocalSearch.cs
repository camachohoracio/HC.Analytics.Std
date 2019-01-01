#region

using System;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.LocalSearch
{
    /*************************************************************************
     *  Compilation:  javac Knapsack.java
     *  Execution:    java Knapsack N W
     *
     *  Generates an instance of the 0/1 knapsack problem with N items
     *  and maximum weight W and solves it in time and space proportional
     *  to N * W using dynamic programming.
     *
     *  For testing, the inputs are generated at random with weights between 0
     *  and W, and profits between 0 and 1000.
     *
     *  %  java Knapsack 6 2000 
     *  item    profit  weight  take
     *  1       874     580     true
     *  2       620     1616    false
     *  3       345     1906    false
     *  4       369     1942    false
     *  5       360     50      true
     *  6       470     294     true
     *
     *************************************************************************/

    public class KnapsackLocalSearch
    {
        public void Solve(
            int W, // maximum weight of knapsack
            double[] profit,
            int[] weight)
        {
            var N = profit.Length; // number of items

            //
            // get the profit and weight arrays in the correct
            // format
            //
            var profitTmp = new double[N + 1];
            var weightTmp = new int[N + 1];
            for (var n = 1; n <= N; n++)
            {
                profitTmp[n] = profit[n - 1];
                weightTmp[n] = weight[n - 1];
            }
            profit = profitTmp;
            weight = weightTmp;

            //
            // opt[n,w] = Max profit of packing items 1..n with weight limit w
            // sol[n,w] = does opt solution to pack items 1..n with weight limit w include item n?
            //
            var opt = new double[N + 1,W + 1];
            var sol = new bool[N + 1,W + 1];

            for (var n = 1; n <= N; n++)
            {
                for (var w = 1; w <= W; w++)
                {
                    // don't take item n
                    var option1 = opt[n - 1, w];

                    // take item n
                    var option2 = double.MinValue;
                    if (weight[n] <= w) option2 = profit[n] + opt[n - 1, w - weight[n]];

                    // select better of two options
                    opt[n, w] = Math.Max(option1, option2);
                    sol[n, w] = (option2 > option1);
                }
            }

            // determine which items to take
            var take = new bool[N + 1];
            for (int n = N, w = W; n > 0; n--)
            {
                if (sol[n, w])
                {
                    take[n] = true;
                    w = w - weight[n];
                }
                else
                {
                    take[n] = false;
                }
            }

            // print results
            PrintToScreen.WriteLine("item" + "\t" + "profit" + "\t" + "weight" + "\t" + "take");
            for (var n = 1; n <= N; n++)
            {
                PrintToScreen.WriteLine(n + "\t" + profit[n] + "\t" + weight[n] + "\t" + take[n]);
            }
        }
    }
}
