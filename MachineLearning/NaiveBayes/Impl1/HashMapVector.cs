#region

using System;
using System.Collections.Generic;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * A data structure for a term vector for a document stored
     * as a Dictionary that maps tokens to Weight's that store the
     * weight of that token in the document.
     * <p/>
     * Needed as an efficient, indexed representation of sparse
     * document vectors.
     *
     * @author Ray Mooney
     */

    public class HashMapVector
    {
        #region Members

        /**
         * The Dictionary that stores the mapping of tokens to Weights
         */
        public Dictionary<string, Weight> Map { get; private set;}

        #endregion

        #region Constructors

        public HashMapVector()
        {
            Map = new Dictionary<string, Weight>();
        }

        #endregion

        /**
         * Returns the number of tokens in the vector.
         */

        public int size()
        {
            return Map.Count;
        }

        /**
         * Clears the vector back to all zeros
         */

        public void Clear()
        {
            Map.Clear();
        }

        /**
         * Returns the Set of MapEntries in the hashMap
         */
        //public Set<Map.Entry<string, Weight>> entrySet() {
        //  return hashMap.entrySet();
        //}

        /**
         * Increment the weight for the given token in the vector by the given amount.
         */

        public double Increment(string strToken, double dblAmount)
        {
            Weight weight;
            // If there is no current Weight for this token, create one
            if (!Map.TryGetValue(
                strToken,
                out weight))
            {
                weight = new Weight();
                Map[strToken] = weight;
            }
            // Increment the weight of this token in the bag.
            weight.increment(dblAmount);
            return weight.GetValue();
        }

        /**
         * Return the weight of the given token in the vector
         */

        public double getWeight(string token)
        {
            Weight weight = Map[token];
            if (weight == null)
                return 0.0;
            else
                return weight.GetValue();
        }

        /**
         * Increment the weight for the given token in the vector by 1.
         */

        public double Increment(string strToken)
        {
            return Increment(strToken, 1.0);
        }

        /**
         * Increment the weight for the given token in the vector by the given int
         */

        public double Increment(string strToken, int amount)
        {
            return Increment(strToken, (double) amount);
        }

        /**
         * Destructively Add the given vector to the current vector
         */

        public void Add(HashMapVector vector)
        {
            foreach (KeyValuePair<string, Weight> entry in vector.Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                Increment(token, weight);
            }
        }

        /**
         * Destructively Add a scaled version of the given vector to the current vector
         */

        public void addScaled(HashMapVector vector, double scalingFactor)
        {
            foreach (KeyValuePair<string, Weight> entry in vector.Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                Increment(token, scalingFactor*weight);
            }
        }

        /**
         * Destructively subtract the given vector from the current vector
         */

        public void subtract(HashMapVector vector)
        {
            foreach (KeyValuePair<string, Weight> entry in vector.Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                Increment(token, -weight);
            }
        }


        /**
         * Destructively multiply the vector by a constant
         */

        public void multiply(double factor)
        {
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                Weight weight = entry.Value;
                weight.setValue(factor*weight.GetValue());
            }
        }


        /**
         * Produce a copy of this HashMapVector with a new Dictionary and new
         * Weight's
         */

        public HashMapVector copy()
        {
            HashMapVector result = new HashMapVector();
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                result.Increment(token, weight);
            }
            return result;
        }

        /**
         * Returns the maximum weight of any token in the vector.
         */

        public double maxWeight()
        {
            double maxWeight = Double.NegativeInfinity;
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                if (weight > maxWeight)
                    maxWeight = weight;
            }
            return maxWeight;
        }


        /**
         * Print out the vector showing the tokens and their weights
         */

        public void Print()
        {
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // Print the term and its weight, where the value of the map entry is a Weight
                // and then you need to get the value of the Weight as the weight.
                PrintToScreen.WriteLine(entry.Key + ":" + entry.Value.GetValue());
            }
        }

        /**
         * Return string of the vector showing the tokens and their weights
         */

        public string toString()
        {
            string ret = "";
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // Print the term and its weight, where the value of the map entry is a Weight
                // and then you need to get the value of the Weight as the weight.
                ret += entry.Key + ": " + entry.Value.GetValue() + " ";
            }
            return ret;
        }

        /**
         * Computes cosine of angle to otherVector.
         */

        public double cosineTo(HashMapVector otherVector)
        {
            return cosineTo(otherVector, otherVector.Length());
        }

        /**
         * Computes cosine of angle to otherVector when also given otherVector's Euclidian Length
         * (Allows saving computation if Length already known.  more efficient when
         * current vector is shorter than otherVector)
         */

        public double cosineTo(HashMapVector otherVector, double Length)
        {
            // Stores sum of squares of current vector elements
            double sum = 0;
            // Stores running sum for dot product of two vectors
            double dotProd = 0;
            // iterate through elements in current vector
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The weight for the token is in the value of the Weight
                double weight = entry.Value.GetValue();
                double otherWeight = otherVector.getWeight(token);
                // Update dot product sum and sum of squares
                dotProd += weight*otherWeight;
                sum += weight*weight;
            }
            // cosine is dot product over product of lengths
            return (dotProd/(Math.Sqrt(sum)*Length));
        }

        /**
         * Compute Euclidian Length (sqrt of sum of squares) of vector
         */

        public double Length()
        {
            // Stores running sum of squares
            double sum = 0;
            foreach (KeyValuePair<string, Weight> entry in Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                double weight = entry.Value.GetValue();
                sum += weight*weight;
            }
            return Math.Sqrt(sum);
        }
    }
}
