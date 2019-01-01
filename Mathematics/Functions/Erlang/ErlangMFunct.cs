#region

using System;

#endregion

namespace HC.Analytics.Mathematics.Functions.Erlang
{
    public class ErlangMFunct
    {
        // ERLANG CONNECTIONS BUSY, B AND C EQUATIONS

        // returns the probablility that m resources (connections) are busy
        // totalTraffic:    total traffic in Erlangs
        // totalResouces:   total number of resources in the system
        public static double erlangMprobability(double totalTraffic, double totalResources, double em)
        {
            double prob = 0.0D;
            if (totalTraffic > 0.0D)
            {
                double numer = totalResources*Math.Log(em) - Fmath.logFactorial(em);
                double denom = 1.0D;
                double lastTerm = 1.0D;
                for (int i = 1; i <= totalResources; i++)
                {
                    lastTerm = lastTerm*totalTraffic/i;
                    denom += lastTerm;
                }
                denom = Math.Log(denom);
                prob = numer - denom;
                prob = Math.Exp(prob);
            }
            return prob;
        }

        public static double erlangMprobability(double totalTraffic, long totalResources, long em)
        {
            return erlangMprobability(totalTraffic, totalResources, (double) em);
        }

        public static double erlangMprobability(double totalTraffic, int totalResources, int em)
        {
            return erlangMprobability(totalTraffic, totalResources, (double) em);
        }
    }
}
