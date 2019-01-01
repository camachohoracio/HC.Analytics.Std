#region

using System;
using System.Text;
using HC.Analytics.Colt.stat;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package quantile;

    /**
     * A set of buffers holding <tt>double</tt> elements; internally used for computing approximate quantiles.
     */

    [Serializable]
    public class DoubleBufferSet : BufferSet
    {
        public DoubleBufferStat[] m_buffers;
        private bool m_nextTriggerCalculationState; //tmp var only
        /**
         * Constructs a buffer set with b buffers, each having k elements
         * @param b the number of buffers
         * @param k the number of elements per buffer
         */

        public DoubleBufferSet(int b, int k)
        {
            m_buffers = new DoubleBufferStat[b];
            clear(k);
        }

        /**
         * Returns an empty buffer if at least one exists.
         * Preferably returns a buffer which has already been used,
         * i.e. a buffer which has already been allocated.
         */

        public DoubleBufferStat _getFirstEmptyBuffer()
        {
            DoubleBufferStat emptyBuffer = null;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (m_buffers[i].isEmpty())
                {
                    if (m_buffers[i].isAllocated())
                    {
                        return m_buffers[i];
                    }
                    emptyBuffer = m_buffers[i];
                }
            }

            return emptyBuffer;
        }

        /**
         * Returns all full or partial buffers.
         */

        public DoubleBufferStat[] _getFullOrPartialBuffers()
        {
            //count buffers
            int count = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (!m_buffers[i].isEmpty())
                {
                    count++;
                }
            }

            //collect buffers
            DoubleBufferStat[] collectedBuffers = new DoubleBufferStat[count];
            int j = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (!m_buffers[i].isEmpty())
                {
                    collectedBuffers[j++] = m_buffers[i];
                }
            }

            return collectedBuffers;
        }

        /**
         * Determines all full buffers having the specified level.
         * @return all full buffers having the specified level
         */

        public DoubleBufferStat[] _getFullOrPartialBuffersWithLevel(int level)
        {
            //count buffers
            int count = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if ((!m_buffers[i].isEmpty()) && m_buffers[i].level() == level)
                {
                    count++;
                }
            }

            //collect buffers
            DoubleBufferStat[] collectedBuffers = new DoubleBufferStat[count];
            int j = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if ((!m_buffers[i].isEmpty()) && m_buffers[i].level() == level)
                {
                    collectedBuffers[j++] = m_buffers[i];
                }
            }

            return collectedBuffers;
        }

        /**
         * @return The minimum level of all buffers which are full.
         */

        public int _getMinLevelOfFullOrPartialBuffers()
        {
            int intB = b();
            int minLevel = int.MaxValue;
            DoubleBufferStat buffer;

            for (int i = 0; i < intB; i++)
            {
                buffer = m_buffers[i];
                if ((!buffer.isEmpty()) && (buffer.level() < minLevel))
                {
                    minLevel = buffer.level();
                }
            }
            return minLevel;
        }

        /**
         * Returns the number of empty buffers.
         */

        public int _getNumberOfEmptyBuffers()
        {
            int count = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (m_buffers[i].isEmpty())
                {
                    count++;
                }
            }

            return count;
        }

        /**
         * Returns all empty buffers.
         */

        public DoubleBufferStat _getPartialBuffer()
        {
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (m_buffers[i].isPartial())
                {
                    return m_buffers[i];
                }
            }
            return null;
        }

        /**
         * @return the number of buffers
         */

        public int b()
        {
            return m_buffers.Length;
        }

        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns, and its memory requirements will be close to zero.
         */

        public void Clear()
        {
            clear(k());
        }

        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns, and its memory requirements will be close to zero.
         */

        public void clear(int k)
        {
            for (int i = b(); --i >= 0;)
            {
                m_buffers[i] = new DoubleBufferStat(k);
            }
            m_nextTriggerCalculationState = true;
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public new Object Clone()
        {
            DoubleBufferSet copy = (DoubleBufferSet) base.Clone();

            copy.m_buffers = (DoubleBufferStat[]) copy.m_buffers.Clone();
            for (int i = m_buffers.Length; --i >= 0;)
            {
                copy.m_buffers[i] = (DoubleBufferStat) copy.m_buffers[i].Clone();
            }
            return copy;
        }

        /**
         * Collapses the specified full buffers (must not include partial buffer).
         * @return a full buffer containing the collapsed values. The buffer has accumulated weight.
         * @param buffers the buffers to be collapsed (all of them must be full or partially full)
         */

        public DoubleBufferStat collapse(DoubleBufferStat[] buffers)
        {
            //determine W
            int W = 0; //sum of all weights
            for (int i = 0; i < buffers.Length; i++)
            {
                W += buffers[i].weight();
            }

            //determine outputTriggerPositions
            int k_ = k();
            long[] triggerPositions = new long[k_];
            for (int j = 0; j < k_; j++)
            {
                triggerPositions[j] = nextTriggerPosition(j, W);
            }

            //do the main work: determine values at given positions in sorted sequence
            double[] outputValues = getValuesAtPositions(buffers, triggerPositions);

            //mark all full buffers as empty, except the first, which will contain the output
            for (int b = 1; b < buffers.Length; b++)
            {
                buffers[b].Clear();
            }

            DoubleBufferStat outputBuffer = buffers[0];
            outputBuffer.values.elements(outputValues);
            outputBuffer.weight(W);

            return outputBuffer;
        }

        /**
         * Returns whether the specified element is contained in the receiver.
         */

        public bool contains(double element)
        {
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if ((!m_buffers[i].isEmpty()) && m_buffers[i].contains(element))
                {
                    return true;
                }
            }

            return false;
        }

        /**
         * Applies a procedure to each element of the receiver, if any.
         * Iterates over the receiver in no particular order.
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         */

        public bool forEach(DoubleProcedure procedure)
        {
            for (int i = m_buffers.Length; --i >= 0;)
            {
                for (int w = m_buffers[i].weight(); --w >= 0;)
                {
                    if (!(m_buffers[i].values.forEach(procedure)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Determines all values of the specified buffers positioned at the specified triggerPositions within the sorted sequence and fills them into outputValues.
         * @param buffers the buffers to be searched (all must be full or partial) 
         * @param triggerPositions the positions of elements within the sorted sequence to be retrieved
         * @return outputValues a list filled with the values at triggerPositions
         */

        public double[] getValuesAtPositions(
            DoubleBufferStat[] buffers,
            long[] triggerPositions)
        {
            //if (buffers.Length==0) 
            //{
            //	throw new ArgumentException("Oops! buffer.Length==0.");
            //}

            //PrintToScreen.WriteLine("triggers="+cern.it.util.Arrays.ToString(positions));

            //new DoubleArrayList(outputValues).fillFromToWith(0, outputValues.Length-1, 0.0f);
            //delte the above line, it is only for testing

            //cern.it.util.Log.println("\nEntering getValuesAtPositions...");
            //cern.it.util.Log.println("hitPositions="+cern.it.util.Arrays.ToString(positions));

            // sort buffers.
            for (int i = buffers.Length; --i >= 0;)
            {
                buffers[i].Sort();
            }

            // collect some infos into fast cache; for tuning purposes only.
            int[] bufferSizes = new int[buffers.Length];
            double[,] bufferValues = new double[buffers.Length,buffers[0].Size()];
            int totalBuffersSize = 0;
            for (int i = buffers.Length; --i >= 0;)
            {
                bufferSizes[i] = buffers[i].Size();
                ArrayHelper.SetRow(
                    bufferValues,
                    buffers[i].values.elements(),
                    i);
                totalBuffersSize += bufferSizes[i];
                //cern.it.util.Log.println("buffer["+i+"]="+buffers[i].values);
            }

            // prepare merge of equi-distant elements within buffers into output values

            // first collect some infos into fast cache; for tuning purposes only.
            int buffersSize = buffers.Length;
            int triggerPositionsLength = triggerPositions.Length;

            // now prepare the important things.
            int j = 0; //current position in collapsed values
            int[] cursors = new int[buffers.Length]; //current position in each buffer; init with zeroes
            long counter = 0; //current position in sorted sequence
            long nextHit = triggerPositions[j]; //next position in sorted sequence to trigger output population
            double[] outputValues = new double[triggerPositionsLength];

            if (totalBuffersSize == 0)
            {
                // nothing to output, because no elements have been filled (we are empty).
                // return meaningless values
                for (int i = 0; i < triggerPositions.Length; i++)
                {
                    outputValues[i] = double.NaN;
                }
                return outputValues;
            }

            // fill all output values with equi-distant elements.
            while (j < triggerPositionsLength)
            {
                //PrintToScreen.WriteLine("\nj="+j);
                //PrintToScreen.WriteLine("counter="+counter);
                //PrintToScreen.WriteLine("nextHit="+nextHit);

                // determine buffer with smallest value at cursor position.
                double minValue = Double.PositiveInfinity;
                int minBufferIndex = -1;

                for (int b = buffersSize; --b >= 0;)
                {
                    //DoubleBuffer buffer = buffers[b];
                    //if (cursors[b] < buffer.Length) { 
                    if (cursors[b] < bufferSizes[b])
                    {
                        ///double value = buffer.values[cursors[b]];
                        double value = bufferValues[b, cursors[b]];
                        if (value <= minValue)
                        {
                            minValue = value;
                            minBufferIndex = b;
                        }
                    }
                }

                DoubleBufferStat minBuffer = buffers[minBufferIndex];

                // trigger copies into output sequence, if necessary.
                counter += minBuffer.weight();
                while (counter > nextHit && j < triggerPositionsLength)
                {
                    outputValues[j++] = minValue;
                    //PrintToScreen.WriteLine("adding to output="+minValue);
                    if (j < triggerPositionsLength)
                    {
                        nextHit = triggerPositions[j];
                    }
                }


                // that element has now been treated, move further.
                cursors[minBufferIndex]++;
                //PrintToScreen.WriteLine("cursors="+cern.it.util.Arrays.ToString(cursors));
            } //end while (j<k)

            //cern.it.util.Log.println("returning output="+cern.it.util.Arrays.ToString(outputValues));
            return outputValues;
        }

        /**
         * @return the number of elements within a buffer.
         */

        public int k()
        {
            return m_buffers[0].m_k;
        }

        /**
         * Returns the number of elements currently needed to store all contained elements.
         */

        public long memory()
        {
            long memory = 0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                memory = memory + m_buffers[i].memory();
            }
            return memory;
        }

        /**
         * Computes the next triggerPosition for collapse
         * @return the next triggerPosition for collapse
         * @param j specifies that the j-th trigger position is to be computed
         * @param W the accumulated weights
         */

        public long nextTriggerPosition(int j, long W)
        {
            long nextTriggerPosition;

            if (W%2L != 0)
            {
                //is W odd?
                nextTriggerPosition = j*W + (W + 1)/2;
            }

            else
            {
                //W is even
                //alternate between both possible next hit positions upon successive invocations
                if (m_nextTriggerCalculationState)
                {
                    nextTriggerPosition = j*W + W/2;
                }
                else
                {
                    nextTriggerPosition = j*W + (W + 2)/2;
                }
            }

            return nextTriggerPosition;
        }

        /**
         * Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
         * Does linear interpolation if the element is not contained but lies in between two contained elements.
         *
         * @param the element to search for.
         * @return the percentage <tt>p</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= p &lt;=1.0)</tt>.
         */

        public double phi(double element)
        {
            double elementsLessThanOrEqualToElement = 0.0;
            for (int i = m_buffers.Length; --i >= 0;)
            {
                if (!m_buffers[i].isEmpty())
                {
                    elementsLessThanOrEqualToElement +=
                        m_buffers[i].m_weight*m_buffers[i].rank(element);
                }
            }

            return elementsLessThanOrEqualToElement/totalSize();
        }

        /**
         * @return a string representation of the receiver
         */

        public new string ToString()
        {
            StringBuilder buf = new StringBuilder();
            for (int intB = 0; intB < b(); intB++)
            {
                if (!m_buffers[intB].isEmpty())
                {
                    buf.Append("buffer#" + intB + " = ");
                    buf.Append(m_buffers[intB] + Environment.NewLine);
                }
            }
            return buf.ToString();
        }

        /**
         * Returns the number of elements in all buffers.
         */

        public long totalSize()
        {
            DoubleBufferStat[] fullBuffers = _getFullOrPartialBuffers();
            long totalSize = 0;
            for (int i = fullBuffers.Length; --i >= 0;)
            {
                totalSize += fullBuffers[i].Size()*fullBuffers[i].weight();
            }

            return totalSize;
        }
    }
}
