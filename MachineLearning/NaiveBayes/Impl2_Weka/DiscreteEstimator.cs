using System;
using HC.Core.Helpers;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl2_Weka
{
    /*
     *    This program is free software; you can redistribute it and/or modify
     *    it under the terms of the GNU General Public License as published by
     *    the Free Software Foundation; either version 2 of the License, or
     *    (at your option) any later version.
     *
     *    This program is distributed in the hope that it will be useful,
     *    but WITHOUT ANY WARRANTY; without even the implied warranty of
     *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     *    GNU General Public License for more details.
     *
     *    You should have received a copy of the GNU General Public License
     *    along with this program; if not, write to the Free Software
     *    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
     */

    /*
     *    DiscreteEstimator.java
     *    Copyright (C) 1999 Len Trigg
     *
     */

    //package weka.estimators;

    //import weka.core.Utils;


    /** 
     * Simple symbolic probability estimator based on symbol counts.
     *
     * @author Len Trigg (trigg@cs.waikato.ac.nz)
     * @version $Revision: 1.6 $
     */
    public class DiscreteEstimator : Estimator
    {

        /** Hold the counts */
        private double[] m_Counts;

        /** Hold the sum of counts */
        private double m_SumOfCounts;


        /**
         * Constructor
         *
         * @param numSymbols the number of possible symbols (remember to include 0)
         * @param laplace if true, counts will be initialised to 1
         */
        public DiscreteEstimator(int numSymbols, bool laplace)
        {

            m_Counts = new double[numSymbols];
            m_SumOfCounts = 0;
            if (laplace)
            {
                for (int i = 0; i < numSymbols; i++)
                {
                    m_Counts[i] = 1;
                }
                m_SumOfCounts = (double)numSymbols;
            }
        }

        /**
         * Constructor
         *
         * @param nSymbols the number of possible symbols (remember to include 0)
         * @param fPrior value with which counts will be initialised
         */
        public DiscreteEstimator(int nSymbols, double fPrior)
        {

            m_Counts = new double[nSymbols];
            for (int iSymbol = 0; iSymbol < nSymbols; iSymbol++)
            {
                m_Counts[iSymbol] = fPrior;
            }
            m_SumOfCounts = fPrior * (double)nSymbols;
        }

        /**
         * Add a new data value to the current estimator.
         *
         * @param data the new data value 
         * @param weight the weight assigned to the data value 
         */
        public void addValue(double data, double weight)
        {

            m_Counts[(int)data] += weight;
            m_SumOfCounts += weight;
        }

        /**
         * Get a probability estimate for a value
         *
         * @param data the value to estimate the probability of
         * @return the estimated probability of the supplied value
         */
        public double getProbability(double data)
        {

            if (m_SumOfCounts == 0)
            {
                return 0;
            }
            return (double)m_Counts[(int)data] / m_SumOfCounts;
        }

        /**
         * Gets the number of symbols this estimator operates with
         *
         * @return the number of estimator symbols
         */
        public int getNumSymbols()
        {

            return (m_Counts == null) ? 0 : m_Counts.Length;
        }


        /**
         * Get the count for a value
         *
         * @param data the value to get the count of
         * @return the count of the supplied value
         */
        public double getCount(double data)
        {

            if (m_SumOfCounts == 0)
            {
                return 0;
            }
            return m_Counts[(int)data];
        }


        /**
         * Get the sum of all the counts
         *
         * @return the total sum of counts
         */
        public double getSumOfCounts()
        {

            return m_SumOfCounts;
        }


        /**
         * Display a representation of this estimator
         */
        public String toString()
        {

            String result = "Discrete Estimator. Counts = ";
            if (m_SumOfCounts > 1)
            {
                for (int i = 0; i < m_Counts.Length; i++)
                {
                    result += " " + m_Counts[i];
                }
                result += "  (Total = " + m_SumOfCounts
              + ")\n";
            }
            else
            {
                for (int i = 0; i < m_Counts.Length; i++)
                {
                    result += " " + m_Counts[i];
                }
                result += "  (Total = " + m_SumOfCounts + ")\n";
            }
            return result;
        }

        /**
         * Main method for testing this class.
         *
         * @param argv should contain a sequence of integers which
         * will be treated as symbolic.
         */
        public static void main(String[] argv)
        {

            try
            {
                if (argv.Length == 0)
                {
                    PrintToScreen.WriteLine("Please specify a set of instances.");
                    return;
                }
                int current = int.Parse(argv[0]);
                int max = current;
                for (int i = 1; i < argv.Length; i++)
                {
                    current = int.Parse(argv[i]);
                    if (current > max)
                    {
                        max = current;
                    }
                }
                DiscreteEstimator newEst = new DiscreteEstimator(max + 1, true);
                for (int i = 0; i < argv.Length; i++)
                {
                    current = int.Parse(argv[i]);
                    PrintToScreen.WriteLine(newEst);
                    PrintToScreen.WriteLine("Prediction for " + current
                               + " = " + newEst.getProbability(current));
                    newEst.addValue(current, 1);
                }
            }
            catch (Exception e)
            {
                PrintToScreen.WriteLine(e.Message);
            }
        }
    }
}

