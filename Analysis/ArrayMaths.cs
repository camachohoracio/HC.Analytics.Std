#region

using System;
using System.Collections.Generic;
using HC.Analytics.ConvertClasses;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Complex;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Class   ArrayMaths
    *
    *   USAGE:  One dimensional arrays:  mathematical manipulations and inter-conversions
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:       April 2008
    *   AMENDED:    22-30 May 2008, 4 June 2008, 27-28 June 2007, 2-4 July 2008,
    *               8 July 2008, 25 July 2008, 4 September 2008, 13 December 2008
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web pages:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/ArrayMaths.html
    *
    *   Copyright (c) 2008
    *
    *   PERMISSION TO COPY:
    *
    *   Redistributions of this source code, or parts of, must retain the above
    *   copyright notice, this list of conditions and the following disclaimer.
    *
    *   Redistribution in binary form of all or parts of this class, must reproduce
    *   the above copyright, this list of conditions and the following disclaimer in
    *   the documentation and/or other materials provided with the distribution.
    *
    *   Permission to use, copy and modify this software and its documentation for
    *   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
    *   to the author, Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all
    *   copies and associated documentation or publications.
    *
    *   Dr Michael Thomas Flanagan makes no representations about the suitability
    *   or fitness of the software for any or for a particular purpose.
    *   Michael Thomas Flanagan shall not be liable for any damages suffered
    *   as a result of using, modifying or distributing this software or its derivatives.
    *
    ***************************************************************************************/

    public class ArrayMaths
    {
        protected bool blnSuppressMessages; // = true when suppress 'possible loss of precision' messages has been set
        protected int intLength; // array Length
        protected List<Object> m_array; // internal array

        protected List<Object> m_minmax = new List<Object>(2); // element at 0 -  maximum value
        // element at 1 -  minimum value
        protected int maxIndex = -1; // index of the maximum value array element
        protected int minIndex = -1; // index of the minimum value array element
        protected int[] originalTypes; // list of entered types in the array

        protected bool productDone; // = true whem array product has been found

        protected bool productlongToDouble;
        // = true whem long has been converted to double to avoid overflow in multiplication

        protected List<Object> productt = new List<Object>(1); // product of all elements

        protected int[] sortedIndices; // sorted indices
        protected bool sumDone; // = true whem array sum has been found
        protected bool sumlongToDouble; // = true whem long has been converted to double to avoid overflow in summation
        protected List<Object> summ = new List<Object>(1); // sum of all elements

        protected int type = -1;
        // 0 double, 1 double, 2 float, 3 float, 4 long, 5 long, 6 int, 7 int, 8 short, 9 short, 10 byte, 11 byte

        protected string[] typeName = {
                                          "double", "double", "float", "float", "long", "long", "int", "int", "short",
                                          "short", "byte", "byte", "decimal", "long", "ComplexClass", "char", "char",
                                          "string"
                                      };

        public ArrayMaths()
        {
        }


        public ArrayMaths(double[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 0;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(long[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 4;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(float[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 2;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(int[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 6;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(short[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 8;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(byte[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 10;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add((array[i]));
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(decimal[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 12;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(ComplexClass[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 14;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
        }

        public ArrayMaths(char[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 16;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add((array[i]));
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(string[] array)
        {
            intLength = array.Length;
            m_array = new List<Object>(intLength);
            type = 18;
            for (int i = 0; i < intLength; i++)
            {
                m_array.Add(array[i]);
            }
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
        }

        public ArrayMaths(Object[] array)
        {
            intLength = array.Length;
            originalTypes = new int[intLength];
            List<Object> arrayl = new List<Object>(intLength);
            for (int i = 0; i < intLength; i++)
            {
                arrayl.Add(array[i]);
            }
            ArrayMaths am = new ArrayMaths(arrayl);
            m_array = am.getArray_as_ArrayList();
            m_minmax = am.m_minmax;
            minIndex = am.minIndex;
            maxIndex = am.maxIndex;
            originalTypes = am.originalTypes;
        }

        public ArrayMaths(Stat arrayst)
        {
            m_array = arrayst.getArray_as_ArrayList();
            intLength = m_array.Count;
            type = arrayst.typeIndex();
            originalTypes = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = type;
            }
            minmax();
        }

        public ArrayMaths(List<Object> arrayl)
        {
            intLength = arrayl.Count;
            originalTypes = new int[intLength];
            m_array = new List<Object>(intLength);

            for (int i = 0; i < intLength; i++)
            {
                originalTypes[i] = -1;
                if (arrayl[i] is double)
                {
                    originalTypes[i] = 1;
                }
                if (arrayl[i] is float)
                {
                    originalTypes[i] = 3;
                }
                if (arrayl[i] is long)
                {
                    originalTypes[i] = 5;
                }
                if (arrayl[i] is int)
                {
                    originalTypes[i] = 7;
                }
                if (arrayl[i] is short)
                {
                    originalTypes[i] = 9;
                }
                if (arrayl[i] is byte)
                {
                    originalTypes[i] = 11;
                }
                if (arrayl[i] is decimal)
                {
                    originalTypes[i] = 12;
                }
                if (arrayl[i] is long)
                {
                    originalTypes[i] = 13;
                }
                if (arrayl[i] is ComplexClass)
                {
                    originalTypes[i] = 14;
                }
                if (arrayl[i] is char)
                {
                    originalTypes[i] = 17;
                }
                if (arrayl[i] is string)
                {
                    originalTypes[i] = 18;
                }
                if (originalTypes[i] == -1)
                {
                    throw new ArgumentException("Object at " + i + " not recognised as one allowed by this class");
                }
            }

            int testType = -1;
            for (int i = 0; i < intLength; i++)
            {
                if (originalTypes[i] == 18)
                {
                    testType = 0;
                }
            }
            for (int i = 0; i < intLength; i++)
            {
                if (originalTypes[i] == 14)
                {
                    testType = 1;
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] == 15)
                    {
                        testType = 2;
                    }
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] == 12)
                    {
                        testType = 3;
                    }
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] == 13)
                    {
                        testType = 4;
                    }
                }
            }
            if (testType == 4)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] <= 3)
                    {
                        testType = 3;
                    }
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] <= 3)
                    {
                        testType = 5;
                    }
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] > 3 && originalTypes[i] < 12)
                    {
                        testType = 6;
                    }
                }
            }
            if (testType == -1)
            {
                for (int i = 0; i < intLength; i++)
                {
                    if (originalTypes[i] == 17)
                    {
                        testType = 7;
                    }
                }
            }
            if (testType == -1)
            {
                throw new ArgumentException(
                    "It should not be possible to reach this exception - main Object type not identified");
            }
            switch (testType)
            {
                case 0:
                    type = 18;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 1:
                                double hold1 = (double) arrayl[i];
                                m_array.Add(hold1);
                                break;
                            case 3:
                                float hold3 = (float) arrayl[i];
                                m_array.Add(hold3);
                                break;
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(hold5);
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(hold7);
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(hold9.ToString());
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(hold11.ToString());
                                break;
                            case 12:
                                decimal hold12 = (decimal) arrayl[i];
                                m_array.Add(hold12.ToString());
                                break;
                            case 13:
                                long hold13 = (long) arrayl[i];
                                m_array.Add(hold13.ToString());
                                break;
                            case 14:
                                ComplexClass hold14 = (ComplexClass) arrayl[i];
                                m_array.Add(hold14.ToString());
                                break;
                            case 15:
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add(hold17.ToString());
                                break;
                            case 18:
                                string hold18 = (string) arrayl[i];
                                m_array.Add(hold18);
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 1:
                    type = 14;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 1:
                                double hold1 = (double) arrayl[i];
                                m_array.Add(new ComplexClass(hold1));
                                break;
                            case 3:
                                float hold3 = (float) arrayl[i];
                                m_array.Add(new ComplexClass(hold3));
                                break;
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(new ComplexClass(hold5));
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(new ComplexClass(hold7));
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(new ComplexClass(hold9));
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(new ComplexClass(hold11));
                                break;
                            case 12:
                                decimal hold12 = (decimal) arrayl[i];
                                m_array.Add(new ComplexClass((double) hold12));
                                break;
                            case 13:
                                long hold13 = (long) arrayl[i];
                                m_array.Add(new ComplexClass(hold13));
                                break;
                            case 14:
                                ComplexClass hold14 = (ComplexClass) arrayl[i];
                                m_array.Add(hold14);
                                break;
                            case 15:
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add(new ComplexClass((double) (hold17)));
                                break;
                            case 18:
                                string hold18 = (string) arrayl[i];
                                m_array.Add(new ComplexClass(double.Parse(hold18)));
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 2:
                    type = 15;
                    //for (int i = 0; i < intLength; i++)
                    //{
                    throw new ArgumentException("Data type not identified by this method");
                    //}
                    //break;
                case 3:
                    type = 12;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 1:
                                double hold1 = (double) arrayl[i];
                                m_array.Add(Converter.convert_double_to_decimal(hold1));
                                break;
                            case 3:
                                float hold3 = (float) arrayl[i];
                                m_array.Add(Converter.convert_float_to_decimal(hold3));
                                break;
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(hold5);
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(hold7);
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(hold9);
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(hold11);
                                break;
                            case 12:
                                decimal hold12 = (decimal) arrayl[i];
                                m_array.Add(hold12);
                                break;
                            case 13:
                                long hold13 = (long) arrayl[i];
                                m_array.Add(hold13);
                                break;
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add(hold17);
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 4:
                    type = 13;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 1:
                                double hold1 = (double) arrayl[i];
                                m_array.Add(Converter.convert_double_to_int(hold1));
                                break;
                            case 3:
                                float hold3 = (float) arrayl[i];
                                m_array.Add(Converter.convert_float_to_int(hold3));
                                break;
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(Converter.convert_long_to_int(hold5));
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(hold7);
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(Converter.convert_short_to_int(hold9));
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(Converter.convert_byte_to_int(hold11));
                                break;
                            case 12:
                                decimal hold12 = (decimal) arrayl[i];
                                m_array.Add(Converter.convert_decimal_to_long(hold12));
                                break;
                            case 13:
                                long hold13 = (long) arrayl[i];
                                m_array.Add(hold13);
                                break;
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add((hold17.ToString()));
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 5:
                    type = 1;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 1:
                                double hold1 = (double) arrayl[i];
                                m_array.Add(hold1);
                                break;
                            case 3:
                                float hold3 = (float) arrayl[i];
                                m_array.Add(Converter.convert_float_to_double(hold3));
                                break;
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(Converter.convert_long_to_double(hold5));
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(Converter.convert_int_to_double(hold7));
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(Converter.convert_short_to_double(hold9));
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(Converter.convert_byte_to_double(hold11));
                                break;
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add(double.Parse(hold17.ToString()));
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 6:
                    type = 7;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 5:
                                long hold5 = (long) arrayl[i];
                                m_array.Add(hold5);
                                break;
                            case 7:
                                int hold7 = (int) arrayl[i];
                                m_array.Add(hold7);
                                break;
                            case 9:
                                short hold9 = (short) arrayl[i];
                                m_array.Add(Converter.convert_short_to_long(hold9));
                                break;
                            case 11:
                                byte hold11 = (byte) arrayl[i];
                                m_array.Add(Converter.convert_byte_to_long(hold11));
                                break;
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add((long) hold17);
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                case 7:
                    type = 7;
                    for (int i = 0; i < intLength; i++)
                    {
                        switch (originalTypes[i])
                        {
                            case 17:
                                char hold17 = (char) arrayl[i];
                                m_array.Add(hold17);
                                break;
                            default:
                                throw new ArgumentException("Data type not identified by this method");
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("Dominant array data type not identified by this method");
            }

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    minmax();
                    break;
            }
        }

        // ARRAY LENGTH
        // retuns array Length
        public int Length()
        {
            return intLength;
        }


        // ARRAY TYPE
        // retuns array type as the index:
        // 0 double, 1 double, 2 float, 3 float, 4 long, 5 long, 6 int, 7 int, 8 short, 9 short, 10 byte, 11 byte
        // 12 decimal, 13 long, 14 ComplexClass, 15 , 16 char, 17 char or 18 string
        public int typeIndex()
        {
            return type;
        }

        // retuns array type as the name:
        // 0 double, 1 double, 2 float, 3 float, 4 long, 5 long, 6 int, 7 int, 8 short, 9 short, 10 byte, 11 byte
        // 12 decimal, 13 long, 14 ComplexClass, 15 , 16 char, 17 char or 18 string
        public string arrayType()
        {
            return (typeName[type] + "[]");
        }

        // retuns original array types, before conversion to common type if array entered as mixed types, as the names:
        // 0 double, 1 double, 2 float, 3 float, 4 long, 5 long, 6 int, 7 int, 8 short, 9 short, 10 byte, 11 byte
        // 12 decimal, 13 long, 14 ComplexClass, 15 , 16 char, 17 char or 18 string
        public string[] originalArrayTypes()
        {
            string[] ss = new string[intLength];
            for (int i = 0; i < intLength; i++)
            {
                ss[i] = typeName[originalTypes[i]];
            }
            return ss;
        }

        // DEEP COPY
        // Copy to a new instance of ArrayMaths
        public ArrayMaths Copy()
        {
            ArrayMaths am = new ArrayMaths();

            am.intLength = intLength;
            am.maxIndex = maxIndex;
            am.minIndex = minIndex;
            am.sumDone = sumDone;
            am.productDone = productDone;
            am.sumlongToDouble = sumlongToDouble;
            am.productlongToDouble = productlongToDouble;
            am.type = type;
            if (originalTypes == null)
            {
                am.originalTypes = null;
            }
            else
            {
                am.originalTypes = (int[]) originalTypes.Clone();
            }
            if (sortedIndices == null)
            {
                am.sortedIndices = null;
            }
            else
            {
                am.sortedIndices = (int[]) sortedIndices.Clone();
            }
            am.blnSuppressMessages = blnSuppressMessages;
            am.m_minmax = new List<Object>();
            if (m_minmax.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                        double dd = ((double) m_minmax[0]);
                        am.m_minmax.Add(dd);
                        dd = ((double) m_minmax[1]);
                        am.m_minmax.Add(dd);
                        break;
                    case 4:
                    case 5:
                        long ll = ((long) m_minmax[0]);
                        am.m_minmax.Add(ll);
                        ll = ((long) m_minmax[1]);
                        am.m_minmax.Add((ll));
                        break;
                    case 2:
                    case 3:
                        float ff = ((float) m_minmax[0]);
                        am.m_minmax.Add(ff);
                        ff = ((float) m_minmax[1]);
                        am.m_minmax.Add(ff);
                        break;
                    case 6:
                    case 7:
                        int ii = ((int) m_minmax[0]);
                        am.m_minmax.Add(ii);
                        ii = ((int) m_minmax[1]);
                        am.m_minmax.Add(ii);
                        break;
                    case 8:
                    case 9:
                        short ss = ((short) m_minmax[0]);
                        am.m_minmax.Add(ss);
                        ss = ((short) m_minmax[1]);
                        am.m_minmax.Add(ss);
                        break;
                    case 10:
                    case 11:
                        byte bb = ((byte) m_minmax[0]);
                        am.m_minmax.Add(bb);
                        ss = ((byte) m_minmax[1]);
                        am.m_minmax.Add(bb);
                        break;
                    case 12:
                        decimal bd = (decimal) m_minmax[0];
                        am.m_minmax.Add(bd);
                        bd = (decimal) m_minmax[1];
                        am.m_minmax.Add(bd);
                        bd = 0;
                        break;
                    case 13:
                        long bi = (long) m_minmax[0];
                        am.m_minmax.Add(bi);
                        bi = (long) m_minmax[1];
                        am.m_minmax.Add(bi);
                        bi = 0;
                        break;
                    case 16:
                    case 17:
                        int iii = ((int) m_minmax[0]);
                        am.m_minmax.Add(iii);
                        iii = (int) m_minmax[1];
                        am.m_minmax.Add(iii);
                        break;
                }
            }

            am.summ = new List<Object>();
            if (summ.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double) summ[0]);
                        am.summ.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double) summ[0]);
                            am.summ.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) summ[0]);
                            am.summ.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd = (decimal) summ[0];
                        am.summ.Add(bd);
                        break;
                    case 13:
                        long bi = (long) summ[0];
                        am.summ.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) summ[0];
                        am.summ.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }

            am.productt = new List<Object>();
            if (productt.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double) productt[0]);
                        am.productt.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double) productt[0]);
                            am.productt.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) productt[0]);
                            am.productt.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd = (decimal) productt[0];
                        am.productt.Add(bd);
                        break;
                    case 13:
                        long bi = (long) productt[0];
                        am.productt.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) productt[0];
                        am.productt.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }


            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = (double[]) getArray_as_double().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]);
                    }
                    break;
                case 2:
                case 3:
                    float[] ff = (float[]) getArray_as_float().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ff[i]);
                    }
                    break;
                case 4:
                case 5:
                    long[] ll = (long[]) getArray_as_long().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]);
                    }
                    break;
                case 6:
                case 7:
                    int[] ii = (int[]) getArray_as_int().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ii[i]);
                    }
                    break;
                case 8:
                case 9:
                    short[] ss = (short[]) getArray_as_short().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    break;
                case 10:
                case 11:
                    byte[] bb = (byte[]) getArray_as_byte().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    break;
                case 12:
                    decimal[] bd = (decimal[]) getArray_as_decimal().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i]);
                    }
                    break;
                case 13:
                    break;
                case 14:
                    ComplexClass[] ccc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ccc[i].Copy());
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    char[] cc = (char[]) getArray_as_char().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((cc[i]));
                    }
                    break;
                case 18:
                    string[] sss = (string[]) getArray_as_string().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(sss[i]);
                    }
                    break;
            }

            return am;
        }

        // Copy to a new instance of Stat
        public Stat statCopy()
        {
            Stat am = new Stat();
            am.intLength = intLength;
            am.maxIndex = maxIndex;
            am.minIndex = minIndex;
            am.sumDone = sumDone;
            am.productDone = productDone;
            am.sumlongToDouble = sumlongToDouble;
            am.productlongToDouble = productlongToDouble;
            am.type = type;
            if (originalTypes == null)
            {
                am.originalTypes = null;
            }
            else
            {
                am.originalTypes = (int[]) originalTypes.Clone();
            }
            if (sortedIndices == null)
            {
                am.sortedIndices = null;
            }
            else
            {
                am.sortedIndices = (int[]) sortedIndices.Clone();
            }
            am.blnSuppressMessages = blnSuppressMessages;
            am.m_minmax = new List<Object>();
            if (m_minmax.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                        double dd = ((double) m_minmax[0]);
                        am.m_minmax.Add(dd);
                        dd = ((double) m_minmax[1]);
                        am.m_minmax.Add(dd);
                        break;
                    case 4:
                    case 5:
                        long ll = ((long) m_minmax[0]);
                        am.m_minmax.Add(ll);
                        ll = ((long) m_minmax[1]);
                        am.m_minmax.Add((ll));
                        break;
                    case 2:
                    case 3:
                        float ff = ((float) m_minmax[0]);
                        am.m_minmax.Add(ff);
                        ff = ((float) m_minmax[1]);
                        am.m_minmax.Add(ff);
                        break;
                    case 6:
                    case 7:
                        int ii = ((int) m_minmax[0]);
                        am.m_minmax.Add(ii);
                        ii = ((int) m_minmax[1]);
                        am.m_minmax.Add(ii);
                        break;
                    case 8:
                    case 9:
                        short ss = ((short) m_minmax[0]);
                        am.m_minmax.Add(ss);
                        ss = ((short) m_minmax[1]);
                        am.m_minmax.Add(ss);
                        break;
                    case 10:
                    case 11:
                        byte bb = ((byte) m_minmax[0]);
                        am.m_minmax.Add(bb);
                        ss = ((byte) m_minmax[1]);
                        am.m_minmax.Add(bb);
                        break;
                    case 12:
                        decimal bd = (decimal) m_minmax[0];
                        am.m_minmax.Add(bd);
                        bd = (decimal) m_minmax[1];
                        am.m_minmax.Add(bd);
                        bd = 0;
                        break;
                    case 13:
                        long bi = (long) m_minmax[0];
                        am.m_minmax.Add(bi);
                        bi = (long) m_minmax[1];
                        am.m_minmax.Add(bi);
                        bi = 0;
                        break;
                    case 16:
                    case 17:
                        int iii = ((int) m_minmax[0]);
                        am.m_minmax.Add(iii);
                        iii = ((int) m_minmax[1]);
                        am.m_minmax.Add(iii);
                        break;
                }
            }

            am.summ = new List<Object>();
            if (summ.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double) summ[0]);
                        am.summ.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double) summ[0]);
                            am.summ.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) summ[0]);
                            am.summ.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd = (decimal) summ[0];
                        am.summ.Add(bd);
                        break;
                    case 13:
                        long bi = (long) summ[0];
                        am.summ.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) summ[0];
                        am.summ.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }

            am.productt = new List<Object>();
            if (productt.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double) productt[0]);
                        am.productt.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double) productt[0]);
                            am.productt.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) productt[0]);
                            am.productt.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd = (decimal) productt[0];
                        am.productt.Add(bd);
                        break;
                    case 13:
                        long bi = (long) productt[0];
                        am.productt.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) productt[0];
                        am.productt.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }


            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = (double[]) getArray_as_double().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]);
                    }
                    break;
                case 2:
                case 3:
                    float[] ff = (float[]) getArray_as_float().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ff[i]);
                    }
                    break;
                case 4:
                case 5:
                    long[] ll = (long[]) getArray_as_long().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ll[i]));
                    }
                    break;
                case 6:
                case 7:
                    int[] ii = (int[]) getArray_as_int().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ii[i]);
                    }
                    break;
                case 8:
                case 9:
                    short[] ss = (short[]) getArray_as_short().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    break;
                case 10:
                case 11:
                    byte[] bb = (byte[]) getArray_as_byte().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    break;
                case 12:
                    decimal[] bd = (decimal[]) getArray_as_decimal().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i]);
                    }
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    break;
                case 14:
                    ComplexClass[] ccc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ccc[i].Copy());
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    char[] cc = (char[]) getArray_as_char().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((cc[i]));
                    }
                    break;
                case 18:
                    string[] sss = (string[]) getArray_as_string().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(sss[i]);
                    }
                    break;
            }

            return am;
        }

        // POSSIBLE LOSS OF PRECISION MESSAGE
        // Suppress possible loss of precisicion messages in an instance of ArrayMaths
        public void suppressMessages()
        {
            blnSuppressMessages = true;
        }

        // Restore possible loss of precisicion messages in an instance of ArrayMaths
        public void restoreMessages()
        {
            blnSuppressMessages = false;
        }

        // Suppress possible loss of precisicion messages for all instances in an application
        public static void suppressMessagesTotal()
        {
            Converter.suppressMessagesAM();
        }

        // Restore possible loss of precisicion messages
        public static void restoreMessagesTotal()
        {
            Converter.restoreMessagesAM();
        }

        // INTERNAL ARRAY
        // return internal array as double
        public double[] array()
        {
            return getArray_as_double();
        }

        public double[] array_as_double()
        {
            return getArray_as_double();
        }

        public double[] getArray_as_double()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            double[] retArray = new double[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_double((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_long_to_double((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_double((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_short_to_double((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_byte_to_double((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_double((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_double((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (double) m_array[i];
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_double(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to double is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as float
        public float[] array_as_float()
        {
            return getArray_as_float();
        }

        public float[] getArray_as_float()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            float[] retArray = new float[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_float((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_long_to_float((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_float((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_short_to_float((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_byte_to_float((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_float((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_float((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((float) m_array[i]);
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_float(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to float is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as long
        public long[] array_as_long()
        {
            return getArray_as_long();
        }

        public long[] getArray_as_long()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            long[] retArray = new long[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_long((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_long((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_long((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_short_to_long((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_byte_to_long((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_long((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_long((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (((long) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_long(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to long is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as int
        public int[] array_as_int()
        {
            return getArray_as_int();
        }

        public int[] getArray_as_int()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] retArray = new int[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_int((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_int((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_long_to_int((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_short_to_int((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_byte_to_int((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_int((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (int) m_array[i];
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((int) m_array[i]);
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((char) m_array[i]);
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to int is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as short
        public short[] array_as_short()
        {
            return getArray_as_short();
        }

        public short[] getArray_as_short()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            short[] retArray = new short[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_short((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_short((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_long_to_short((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_short((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_byte_to_short((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_short((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_short((long) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (short) m_array[i];
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_short(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to short is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as byte
        public byte[] array_as_byte()
        {
            return getArray_as_byte();
        }

        public byte[] getArray_as_byte()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            byte[] retArray = new byte[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_byte((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_byte((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_long_to_byte((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_byte((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_short_to_byte((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (byte) m_array[i];
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_decimal_to_byte((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_byte((long) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((byte) m_array[i]);
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_int_to_byte(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to byte is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as decimal
        public decimal[] array_as_decimal()
        {
            return getArray_as_decimal();
        }

        public decimal[] getArray_as_decimal()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            decimal[] retArray = new decimal[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_double_to_decimal((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = Converter.convert_float_to_decimal((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (decimal) m_array[i];
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to decimal is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as ComplexClass
        public ComplexClass[] array_as_Complex()
        {
            return getArray_as_Complex();
        }

        public ComplexClass[] getArray_as_Complex()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ComplexClass[] retArray = ComplexClass.oneDarray(intLength);
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(((double) m_array[i]));
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(((float) m_array[i]));
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_long_to_double((long) m_array[i]));
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_int_to_double((int) m_array[i]));
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_short_to_double((short) m_array[i]));
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_byte_to_double((byte) m_array[i]));
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_decimal_to_double((decimal) m_array[i]));
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_int_to_double((int) m_array[i]));
                    }
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (ComplexClass) m_array[i];
                    }
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        string ss = (string) m_array[i];
                        //if (ss['i'] != -1 || ss['j'] != -1)
                        //{
                        retArray[i] = ComplexClass.valueOf(ss);
                        //}
                        //else
                        //{
                        //    retArray[i] = new ComplexClass(
                        //        Convert.ToDouble(
                        //            ss));
                        //}
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = new ComplexClass(Converter.convert_int_to_double(((char) m_array[i])));
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as char
        public char[] array_as_char()
        {
            return getArray_as_char();
        }

        public char[] getArray_as_char()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            char[] retArray = new char[intLength];
            switch (type)
            {
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (char) (((int) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (((char) m_array[i]));
                    }
                    break;
                case 18:
                    bool test = true;
                    string[] ss = new string[intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ss[i] = ((string) m_array[i]).Trim();
                        if (ss[1].Length > 1)
                        {
                            test = false;
                            break;
                        }
                    }
                    if (test)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            retArray[i] = (ss[i][0]);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The string array elements are too long to be converted to char");
                    }
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to char is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as string
        public string[] array_as_string()
        {
            return getArray_as_string();
        }

        public string[] getArray_as_string()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            string[] retArray = new string[intLength];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((double) m_array[i]).ToString();
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((float) m_array[i]).ToString();
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((long) m_array[i]).ToString();
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((int) m_array[i]).ToString();
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((short) m_array[i]).ToString();
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((byte) m_array[i]).ToString();
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((decimal) m_array[i]).ToString();
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((long) m_array[i]).ToString();
                    }
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (m_array[i]).ToString();
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = ((char) m_array[i]).ToString();
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        retArray[i] = (string) m_array[i];
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return internal array as Object
        public Object[] array_as_Object()
        {
            return getArray_as_Object();
        }

        public Object[] getArray_as_Object()
        {
            Object[] arrayo = new Object[intLength];
            for (int i = 0; i < intLength; i++)
            {
                arrayo[i] = m_array[i];
            }
            return arrayo;
        }

        // return internal array as List
        public List<object> array_as_ArrayList()
        {
            return getArray_as_ArrayList();
        }

        public List<Object> getArray_as_ArrayList()
        {
            List<Object> arrayl = new List<Object>(intLength);
            for (int i = 0; i < intLength; i++)
            {
                arrayl.Add(m_array[i]);
            }
            return arrayl;
        }

        // return internal array as a Row Matrix_non_stable, Matrix_non_stable.rowMatrix
        public MatrixExtended array_as_Matrix_rowMatrix()
        {
            return getArray_as_Matrix_rowMatrix();
        }

        public MatrixExtended getArray_as_Matrix_rowMatrix()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            MatrixExtended mat = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 18:
                case 17:
                    double[] dd = getArray_as_double();
                    mat = MatrixExtended.rowMatrix(dd);
                    break;
                case 14:
                    throw new ArgumentException(
                        "ComplexClass array cannot be converted to Matrix_non_stable.rowMatrix - use method getArray_as_Complex_rowMatrix");
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return mat;
        }

        // return internal array as a Column Matrix_non_stable, Matrix_non_stable.columnMatrix
        public MatrixExtended array_as_Matrix_columnMatrix()
        {
            return getArray_as_Matrix_columnMatrix();
        }

        public MatrixExtended getArray_as_Matrix_columnMatrix()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            MatrixExtended mat = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 18:
                case 17:
                    double[] dd = getArray_as_double();
                    mat = MatrixExtended.columnMatrix(dd);
                    break;
                case 14:
                    throw new ArgumentException(
                        "ComplexClass array cannot be converted to Matrix_non_stable.columnMatrix - use method getArray_as_Complex_columnMatrix");
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return mat;
        }

        // return internal array as a ComplexClass Row Matix, ComplexClass.rowMatrix
        public ComplexMatrix array_as_Complex_rowMatrix()
        {
            return getArray_as_Complex_rowMatrix();
        }

        public ComplexMatrix getArray_as_Complex_rowMatrix()
        {
            ComplexClass[] cc = getArray_as_Complex();
            ComplexMatrix mat = ComplexMatrix.rowMatrix(cc);
            return mat;
        }

        // return internal array as a ComplexClass Column Matrix_non_stable, ComplexClass.columnMatrix
        public ComplexMatrix array_as_Complex_columnMatrix()
        {
            return getArray_as_Complex_columnMatrix();
        }

        public ComplexMatrix getArray_as_Complex_columnMatrix()
        {
            ComplexClass[] cc = getArray_as_Complex();
            ComplexMatrix mat = ComplexMatrix.columnMatrix(cc);
            return mat;
        }

        // return array of moduli of a ComplexClass internal array
        public double[] array_as_modulus_of_Complex()
        {
            ComplexClass[] cc = getArray_as_Complex();
            double[] mod = new double[intLength];
            for (int i = 0; i < intLength; i++)
            {
                mod[i] = cc[i].Abs();
            }
            return mod;
        }

        // return array of real parts of a ComplexClass internal array
        public double[] array_as_real_part_of_Complex()
        {
            return getArray_as_real_part_of_Complex();
        }

        public double[] getArray_as_real_part_of_Complex()
        {
            ComplexClass[] cc = getArray_as_Complex();
            double[] real = new double[intLength];
            for (int i = 0; i < intLength; i++)
            {
                real[i] = cc[i].getReal();
            }
            return real;
        }

        // return array of imaginary parts of a ComplexClass internal array
        public double[] array_as_imaginary_part_of_Complex()
        {
            return getArray_as_imaginay_part_of_Complex();
        }

        public double[] getArray_as_imaginay_part_of_Complex()
        {
            ComplexClass[] cc = getArray_as_Complex();
            double[] imag = new double[intLength];
            for (int i = 0; i < intLength; i++)
            {
                imag[i] = cc[i].getImag();
            }
            return imag;
        }

        // GET A SUB-ARRAY
        // first index of sub-array = start, last index of sub-array = end
        // return sub-array as double
        public double[] subarray_as_double(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            double[] retArray = new double[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_double((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_long_to_double((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_double((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_short_to_double((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_byte_to_double((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_double((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_double((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (((double) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_double(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to double is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as float
        public float[] subarray_as_float(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            float[] retArray = new float[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_float((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_long_to_float((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_float((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_short_to_float((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_byte_to_float((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_float((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_float((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((float) m_array[i]);
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_float(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to float is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as long
        public long[] subarray_as_long(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            long[] retArray = new long[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_long((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_long((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_long((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_short_to_long((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_byte_to_long((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_long((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_long((int) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (((long) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_long(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to long is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as int
        public int[] subarray_as_int(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] retArray = new int[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_int((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_int((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_long_to_int((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_short_to_int((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_byte_to_int((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_int((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (int) m_array[i];
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((int) m_array[i]);
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((char) m_array[i]);
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to int is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as short
        public short[] subarray_as_short(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            short[] retArray = new short[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_short((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_short((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_long_to_short((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_short((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_byte_to_short((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_short((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_short((long) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (short) m_array[i];
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_short(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to short is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as byte
        public byte[] subarray_as_byte(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            byte[] retArray = new byte[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_byte((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_byte((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_long_to_byte((long) m_array[i]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_byte((int) m_array[i]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_short_to_byte((short) m_array[i]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((byte) m_array[i]);
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_decimal_to_byte((decimal) m_array[i]);
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_byte((long) m_array[i]);
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (((byte) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_int_to_byte(((char) m_array[i]));
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to byte is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as decimal
        public decimal[] subarray_as_decimal(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            decimal[] retArray = new decimal[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_double_to_decimal((double) m_array[i]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = Converter.convert_float_to_decimal((float) m_array[i]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (decimal) m_array[i];
                    }
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to decimal is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as ComplexClass
        public ComplexClass[] subarray_as_Complex(int start, int end)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ComplexClass[] retArray = ComplexClass.oneDarray(intLength);
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(((double) m_array[i]));
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(((float) m_array[i]));
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_long_to_double((long) m_array[i]));
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_int_to_double((int) m_array[i]));
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_short_to_double((short) m_array[i]));
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_byte_to_double((byte) m_array[i]));
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_decimal_to_double((decimal) m_array[i]));
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass((double) m_array[i]);
                    }
                    break;
                case 14:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (ComplexClass) m_array[i];
                    }
                    break;
                case 15:
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        string ss = (string) m_array[i];
                        //if (ss['i'] != -1 || ss['j'] != -1)
                        //{
                        retArray[i - start] = ComplexClass.valueOf(ss);
                        //}
                        //else
                        //{
                        //    retArray[i - start] = new ComplexClass(Convert.ToDouble(ss));
                        //}
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = new ComplexClass(Converter.convert_int_to_double(((char) m_array[i])));
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as char
        public char[] subarray_as_char(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            char[] retArray = new char[end - start + 1];
            switch (type)
            {
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (char) (((int) m_array[i]));
                    }
                    break;
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (((char) m_array[i]));
                    }
                    break;
                case 18:
                    bool test = true;
                    string[] ss = new string[end - start + 1];
                    for (int i = start; i <= end; i++)
                    {
                        ss[i - start] = ((string) m_array[i]).Trim();
                        if (ss[i - start].Length > 1)
                        {
                            test = false;
                            break;
                        }
                    }
                    if (test)
                    {
                        for (int i = start; i <= end; i++)
                        {
                            retArray[i - start] = (ss[i - start][0]);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The string array elements are too long to be converted to char");
                    }
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a conversion to char is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as string
        public string[] subarray_as_string(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            string[] retArray = new string[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((double) m_array[i]).ToString();
                    }
                    break;
                case 2:
                case 3:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((float) m_array[i]).ToString();
                    }
                    break;
                case 4:
                case 5:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((long) m_array[i]).ToString();
                    }
                    break;
                case 6:
                case 7:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((int) m_array[i]).ToString();
                    }
                    break;
                case 8:
                case 9:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((short) m_array[i]).ToString();
                    }
                    break;
                case 10:
                case 11:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((byte) m_array[i]).ToString();
                    }
                    break;
                case 12:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((decimal) m_array[i]).ToString();
                    }
                    break;
                case 13:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((long) m_array[i]).ToString();
                    }
                    break;
                case 14:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (m_array[i]).ToString();
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = ((char) m_array[i]).ToString();
                    }
                    break;
                case 18:
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = (string) m_array[i];
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return retArray;
        }

        // return sub-array as Object
        public Object[] subarray_as_Object(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            Object[] arrayo = new Object[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                arrayo[i - start] = m_array[i];
            }
            return arrayo;
        }

        // return sub-array as List
        public List<Object> subarray_as_array_list(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            List<Object> vec = new List<Object>(end - start + 1);
            for (int i = start; i <= end; i++)
            {
                vec.Add(m_array[i]);
            }
            return vec;
        }

        // return sub-array as a Row Matrix_non_stable, Matrix_non_stable.rowMatrix
        public MatrixExtended subarray_as_Matrix_rowMatrix(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            MatrixExtended mat = null;
            double[] retArray = new double[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 18:
                case 17:
                    double[] dd = getArray_as_double();
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = dd[i];
                    }
                    mat = MatrixExtended.rowMatrix(retArray);
                    break;
                case 14:
                    throw new ArgumentException(
                        "ComplexClass array cannot be converted to Matrix_non_stable.rowMatrix - use method subarray_as_Complex_rowMatrix");
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return mat;
        }

        // return sub-array as a Column Matrix_non_stable, Matrix_non_stable.columnMatrix
        public MatrixExtended subarray_as_Matrix_columnMatrix(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            MatrixExtended mat = null;
            double[] retArray = new double[end - start + 1];
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 18:
                case 17:
                    double[] dd = getArray_as_double();
                    for (int i = start; i <= end; i++)
                    {
                        retArray[i - start] = dd[i];
                    }
                    mat = MatrixExtended.columnMatrix(retArray);
                    break;
                case 14:
                    throw new ArgumentException(
                        "ComplexClass array cannot be converted to Matrix_non_stable.columnMatrix - use method subarray_as_Complex_columnMatrix");
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return mat;
        }

        // return sub-array as a ComplexClass Row Matix, ComplexClass.rowMatrix
        public ComplexMatrix subarray_as_Complex_rowMatrix(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            ComplexClass[] cc = getArray_as_Complex();
            ComplexClass[] retArray = new ComplexClass[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                retArray[i - start] = cc[i];
            }
            ComplexMatrix mat = ComplexMatrix.rowMatrix(retArray);
            return mat;
        }

        // return sub-array as a ComplexClass Column Matrix_non_stable, ComplexClass.columnMatrix
        public ComplexMatrix subarray_as_Complex_columnMatrix(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            ComplexClass[] cc = getArray_as_Complex();
            ComplexClass[] retArray = new ComplexClass[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                retArray[i - start] = cc[i];
            }
            ComplexMatrix mat = ComplexMatrix.columnMatrix(retArray);
            return mat;
        }

        // return array of the moduli of a ComplexClass sub-array
        public double[] subarray_as_modulus_of_Complex(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            ComplexClass[] cc = getArray_as_Complex();
            double[] real = new double[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                real[i - start] = cc[i].Abs();
            }
            return real;
        }

        // return array of real parts of a ComplexClass sub-array
        public double[] subarray_as_real_part_of_Complex(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            ComplexClass[] cc = getArray_as_Complex();
            double[] real = new double[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                real[i - start] = cc[i].getReal();
            }
            return real;
        }

        // return array of imaginary parts of a ComplexClass sub-array
        public double[] subarray_as_imaginay_part_of_Complex(int start, int end)
        {
            if (end >= intLength)
            {
                throw new ArgumentException("end, " + end + ", is greater than the highest index, " + (intLength - 1));
            }
            ComplexClass[] cc = getArray_as_Complex();
            double[] imag = new double[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                imag[i - start] = cc[i].getImag();
            }
            return imag;
        }


        // MAXIMUM AND MINIMUM
        // protected method to call search method for maximum and minimum values
        // called by public methods
        protected void minmax()
        {
            int[] maxminIndices = new int[2];
            findMinMax(getArray_as_Object(), m_minmax, maxminIndices, typeName, type);
            maxIndex = maxminIndices[0];
            minIndex = maxminIndices[1];
        }

        // protected method that finds the maximum and minimum values
        // called by protected method minmax which is called by public methods
        protected static void findMinMax(Object[] arrayo, List<Object> minmaxx, int[] maxminIndices, string[] aTypeName,
                                         int aType)
        {
            int maxIndexx = 0;
            int minIndexx = 0;
            int arraylength = arrayo.Length;
            switch (aType)
            {
                case 0:
                case 1:
                    double[] arrayD = new double[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayD[i] = ((double) arrayo[i]);
                    }
                    double amaxD = arrayD[0];
                    double aminD = arrayD[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayD[i] > amaxD)
                        {
                            amaxD = arrayD[i];
                            maxIndexx = i;
                        }
                        if (arrayD[i] < aminD)
                        {
                            aminD = arrayD[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxD);
                    minmaxx.Add(aminD);
                    break;
                case 4:
                case 5:
                    long[] arrayL = new long[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayL[i] = ((long) arrayo[i]);
                    }
                    long amaxL = arrayL[0];
                    long aminL = arrayL[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayL[i] > amaxL)
                        {
                            amaxL = arrayL[i];
                            maxIndexx = i;
                        }
                        if (arrayL[i] < aminL)
                        {
                            aminL = arrayL[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add((amaxL));
                    minmaxx.Add((aminL));
                    break;
                case 2:
                case 3:
                    float[] arrayF = new float[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayF[i] = ((float) arrayo[i]);
                    }
                    float amaxF = arrayF[0];
                    float aminF = arrayF[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayF[i] > amaxF)
                        {
                            amaxF = arrayF[i];
                            maxIndexx = i;
                        }
                        if (arrayF[i] < aminF)
                        {
                            aminF = arrayF[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxF);
                    minmaxx.Add(aminF);
                    break;
                case 6:
                case 7:
                    int[] arrayI = new int[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayI[i] = ((int) arrayo[i]);
                    }
                    int amaxI = arrayI[0];
                    int aminI = arrayI[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayI[i] > amaxI)
                        {
                            amaxI = arrayI[i];
                            maxIndexx = i;
                        }
                        if (arrayI[i] < aminI)
                        {
                            aminI = arrayI[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxI);
                    minmaxx.Add(aminI);
                    break;
                case 8:
                case 9:
                    short[] arrayS = new short[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayS[i] = ((short) arrayo[i]);
                    }
                    short amaxS = arrayS[0];
                    short aminS = arrayS[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayS[i] > amaxS)
                        {
                            amaxS = arrayS[i];
                            maxIndexx = i;
                        }
                        if (arrayS[i] < aminS)
                        {
                            aminS = arrayS[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxS);
                    minmaxx.Add(aminS);
                    break;
                case 10:
                case 11:
                    byte[] arrayB = new byte[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayB[i] = ((byte) arrayo[i]);
                    }
                    byte amaxB = arrayB[0];
                    byte aminB = arrayB[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayB[i] > amaxB)
                        {
                            amaxB = arrayB[i];
                            maxIndexx = i;
                        }
                        if (arrayB[i] < aminB)
                        {
                            aminB = arrayB[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add((amaxB));
                    minmaxx.Add((aminB));
                    break;
                case 12:
                    decimal[] arrayBD = new decimal[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayBD[i] = (decimal) arrayo[i];
                    }
                    decimal amaxBD = arrayBD[0];
                    decimal aminBD = arrayBD[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayBD[i].CompareTo(amaxBD) == 1)
                        {
                            amaxBD = arrayBD[i];
                            maxIndexx = i;
                        }
                        if (arrayBD[i].CompareTo(aminBD) == -1)
                        {
                            aminBD = arrayBD[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxBD);
                    minmaxx.Add(aminBD);
                    break;
                case 13:
                    long[] arrayBI = new long[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayBI[i] = (long) arrayo[i];
                    }
                    long amaxBI = arrayBI[0];
                    long aminBI = arrayBI[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayBI[i].CompareTo(amaxBI) == 1)
                        {
                            amaxBI = arrayBI[i];
                            maxIndexx = i;
                        }
                        if (arrayBI[i].CompareTo(aminBI) == -1)
                        {
                            aminBI = arrayBI[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(amaxBI);
                    minmaxx.Add(aminBI);
                    break;
                case 16:
                case 17:
                    int[] arrayInt = new int[arraylength];
                    for (int i = 0; i < arraylength; i++)
                    {
                        arrayInt[i] = (((char) arrayo[i]));
                    }
                    int amaxInt = arrayInt[0];
                    int aminInt = arrayInt[0];
                    maxIndexx = 0;
                    minIndexx = 0;
                    for (int i = 1; i < arraylength; i++)
                    {
                        if (arrayInt[i] > amaxInt)
                        {
                            amaxInt = arrayInt[i];
                            maxIndexx = i;
                        }
                        if (arrayInt[i] < aminInt)
                        {
                            aminInt = arrayInt[i];
                            minIndexx = i;
                        }
                    }
                    minmaxx.Add(((char) amaxInt));
                    minmaxx.Add(((char) aminInt));
                    break;
                case 14:
                case 15:
                case 18:
                    PrintToScreen.WriteLine("ArrayMaths:  getMaximum_... or getMinimum_... (findMinMax): the " +
                                            aTypeName[aType] +
                                            " is not a numerical type for which a maximum or a minimum is meaningful/supported");
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            maxminIndices[0] = maxIndexx;
            maxminIndices[1] = minIndexx;
        }

        // Return maximum
        public double maximum()
        {
            return getMaximum_as_double();
        }

        public double maximum_as_double()
        {
            return getMaximum_as_double();
        }

        public double getMaximum()
        {
            return getMaximum_as_double();
        }

        public double getMaximum_as_double()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            double max = 0.0D;
            switch (type)
            {
                case 0:
                case 1:
                    max = ((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_double((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = Converter.convert_long_to_double((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = Converter.convert_int_to_double((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = Converter.convert_short_to_double((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = Converter.convert_byte_to_double((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_double((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = Converter.convert_int_to_double((int) m_minmax[0]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public float maximum_as_float()
        {
            return getMaximum_as_float();
        }

        public float getMaximum_as_float()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            float max = 0.0F;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_float((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = ((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = Converter.convert_long_to_float((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = Converter.convert_int_to_float((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = Converter.convert_short_to_float((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = Converter.convert_byte_to_float((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_float((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = Converter.convert_int_to_float((int) m_minmax[0]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public long maximum_as_long()
        {
            return getMaximum_as_long();
        }

        public long getMaximum_as_long()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            long max = 0L;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_long((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_long((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = ((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = Converter.convert_int_to_long((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = Converter.convert_short_to_long((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = Converter.convert_byte_to_long((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_long((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = Converter.convert_int_to_long((int) m_minmax[0]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public int maximum_as_int()
        {
            return getMaximum_as_int();
        }

        public int getMaximum_as_int()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int max = 0;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_int((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_int((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = Converter.convert_long_to_int((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = ((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = Converter.convert_short_to_int((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = Converter.convert_byte_to_int((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_int((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = (int) m_minmax[0];
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public short maximum_as_short()
        {
            return getMaximum_as_short();
        }

        public short getMaximum_as_short()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            short max = 0;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_short((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_short((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = Converter.convert_long_to_short((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = Converter.convert_int_to_short((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = ((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = Converter.convert_byte_to_short((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_short((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = Converter.convert_int_to_short((long) m_minmax[0]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public byte maximum_as_byte()
        {
            return getMaximum_as_byte();
        }

        public byte getMaximum_as_byte()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            byte max = 0;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_byte((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_byte((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = Converter.convert_long_to_byte((long) m_minmax[0]);
                    break;
                case 6:
                case 7:
                    max = Converter.convert_int_to_byte((int) m_minmax[0]);
                    break;
                case 8:
                case 9:
                    max = Converter.convert_short_to_byte((short) m_minmax[0]);
                    break;
                case 10:
                case 11:
                    max = ((byte) m_minmax[0]);
                    break;
                case 12:
                    max = Converter.convert_decimal_to_byte((decimal) m_minmax[0]);
                    break;
                case 13:
                    max = Converter.convert_int_to_byte((long) m_minmax[0]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public decimal maximum_as_decimal()
        {
            return getMaximum_as_decimal();
        }

        public decimal getMaximum_as_decimal()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            decimal max = 0;
            switch (type)
            {
                case 0:
                case 1:
                    max = Converter.convert_double_to_decimal((double) m_minmax[0]);
                    break;
                case 2:
                case 3:
                    max = Converter.convert_float_to_decimal((float) m_minmax[0]);
                    break;
                case 4:
                case 5:
                    max = (decimal) m_minmax[0];
                    break;
                case 6:
                case 7:
                    max = (decimal) m_minmax[0];
                    break;
                case 8:
                case 9:
                    max = (decimal) m_minmax[0];
                    break;
                case 10:
                case 11:
                    max = (decimal) m_minmax[0];
                    break;
                case 12:
                    max = (decimal) m_minmax[0];
                    break;
                case 13:
                    max = (decimal) m_minmax[0];
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return maximum
        public char maximum_as_char()
        {
            return getMaximum_as_char();
        }

        public char getMaximum_as_char()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            char max = '\u0000';
            switch (type)
            {
                case 6:
                case 7:
                    max = (char) ((int) m_minmax[1]);
                    break;
                case 16:
                case 17:
                    max = ((char) m_minmax[1]);
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a char type maximum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return max;
        }

        // Return minimum
        public double minimum()
        {
            return getMinimum_as_double();
        }

        public double minimum_as_double()
        {
            return getMinimum_as_double();
        }

        public double getMinimum()
        {
            return getMinimum_as_double();
        }

        public double getMinimum_as_double()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            double min = 0.0D;
            switch (type)
            {
                case 0:
                case 1:
                    min = ((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_double((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = Converter.convert_long_to_double((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = Converter.convert_int_to_double((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = Converter.convert_short_to_double((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = Converter.convert_byte_to_double((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_double((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = Converter.convert_int_to_double((int) m_minmax[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public float minimum_as_float()
        {
            return getMinimum_as_float();
        }

        public float getMinimum_as_float()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            float min = 0.0F;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_float((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = ((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = Converter.convert_long_to_float((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = Converter.convert_int_to_float((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = Converter.convert_short_to_float((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = Converter.convert_byte_to_float((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_float((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = Converter.convert_int_to_float((int) m_minmax[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public long minimum_as_long()
        {
            return getMinimum_as_long();
        }

        public long getMinimum_as_long()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            long min = 0L;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_long((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_long((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = ((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = Converter.convert_int_to_long((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = Converter.convert_short_to_long((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = Converter.convert_byte_to_long((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_long((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = Converter.convert_int_to_long((int) m_minmax[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public int minimum_as_int()
        {
            return getMinimum_as_int();
        }

        public int getMinimum_as_int()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int min = 0;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_int((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_int((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = Converter.convert_long_to_int((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = ((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = Converter.convert_short_to_int((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = Converter.convert_byte_to_int((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_int((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = (int) m_minmax[1];
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public short minimum_as_short()
        {
            return getMinimum_as_short();
        }

        public short getMinimum_as_short()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            short min = 0;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_short((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_short((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = Converter.convert_long_to_short((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = Converter.convert_int_to_short((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = ((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = Converter.convert_byte_to_short((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_short((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = Converter.convert_int_to_short((long) m_minmax[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public byte minimum_as_byte()
        {
            return getMinimum_as_byte();
        }

        public byte getMinimum_as_byte()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            byte min = 0;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_byte((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_byte((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = Converter.convert_long_to_byte((long) m_minmax[1]);
                    break;
                case 6:
                case 7:
                    min = Converter.convert_int_to_byte((int) m_minmax[1]);
                    break;
                case 8:
                case 9:
                    min = Converter.convert_short_to_byte((short) m_minmax[1]);
                    break;
                case 10:
                case 11:
                    min = ((byte) m_minmax[1]);
                    break;
                case 12:
                    min = Converter.convert_decimal_to_byte((decimal) m_minmax[1]);
                    break;
                case 13:
                    min = Converter.convert_int_to_byte((long) m_minmax[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public decimal minimum_as_decimal()
        {
            return getMinimum_as_decimal();
        }

        public decimal getMinimum_as_decimal()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            decimal min = 0;
            switch (type)
            {
                case 0:
                case 1:
                    min = Converter.convert_double_to_decimal((double) m_minmax[1]);
                    break;
                case 2:
                case 3:
                    min = Converter.convert_float_to_decimal((float) m_minmax[1]);
                    break;
                case 4:
                case 5:
                    min = (decimal) m_minmax[1];
                    break;
                case 6:
                case 7:
                    min = (decimal) m_minmax[1];
                    break;
                case 8:
                case 9:
                    min = (decimal) m_minmax[1];
                    break;
                case 10:
                case 11:
                    min = (decimal) m_minmax[1];
                    break;
                case 12:
                    min = (decimal) m_minmax[1];
                    break;
                case 13:
                    min = (decimal) m_minmax[1];
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return minimum
        public char minimum_as_char()
        {
            return getMinimum_as_char();
        }

        public char getMinimum_as_char()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            char min = '\u0000';
            switch (type)
            {
                case 6:
                case 7:
                    min = (char) ((int) m_minmax[1]);
                    break;
                case 16:
                case 17:
                    min = ((char) m_minmax[1]);
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 18:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a char type minimum is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return min;
        }

        // Return index of the maximum
        public int maximumIndex()
        {
            return maxIndex;
        }

        public int getMaximumIndex()
        {
            return maxIndex;
        }

        // Return index of the minimum
        public int minimumIndex()
        {
            return minIndex;
        }

        public int getMinimumIndex()
        {
            return minIndex;
        }

        // Returns true if all array elements are, arithmetically, integers,
        // returns false if any array element is not, arithmetically, an integer
        public bool isInteger()
        {
            bool test = false;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    double[] arrayd = getArray_as_double();
                    test = Fmath.isInteger(arrayd);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 13:
                case 16:
                case 17:
                    test = true;
                    break;
                case 12:
                    decimal[] arraybd = getArray_as_decimal();
                    test = Fmath.isInteger(arraybd);
                    break;
                case 14:
                case 15:
                    test = false;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            return test;
        }


        // ADDITION
        // add a constant to the elements of the internal array
        public ArrayMaths plus(double constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + constant);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + (decimal) constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = hold1 + (decimal) constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a double or float cannot be added to a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(double[] arrayD)
        {
            if (intLength != arrayD.Length)
            {
                throw new ArgumentException("The Length of the argument array, " + arrayD.Length +
                                            ", and the Length of this instance internal array, " + intLength +
                                            ", must be equal");
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + arrayD[i]);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + (decimal) arrayD[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = hold1 + (decimal) arrayD[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(arrayD[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayD[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a double or float cannot be added to a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(float constant)
        {
            double constantd = constant;
            return plus(constantd);
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(float[] arrayF)
        {
            if (intLength != arrayF.Length)
            {
                throw new ArgumentException("The Length of the argument array, " + arrayF.Length +
                                            ", and the Length of this instance internal array, " + intLength +
                                            ", must be equal");
            }
            double[] arrayD = new double[intLength];
            for (int i = 0; i < intLength; i++)
            {
                arrayD[i] = arrayF[i];
            }
            return plus(arrayD);
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(long constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a long cannot be added to a char");
                case 17:
                    throw new ArgumentException("a long cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(long[] arrayL)
        {
            int nArg = arrayL.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + arrayL[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max1 = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayL);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max1) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + arrayL[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) arrayL[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + arrayL[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + arrayL[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(arrayL[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayL[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a long cannot be added to a char");
                case 17:
                    throw new ArgumentException("a long cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(int constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("an int cannot be added to a char");
                case 17:
                    throw new ArgumentException("an int cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();

            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(int[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    int maxi2 = am22.getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + arrayI[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayI[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("an int cannot be added to a char");
                case 17:
                    throw new ArgumentException("an int cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        // add a constant to the elements of the internal array
        public ArrayMaths plus(short constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a short cannot be added to a char");
                case 17:
                    throw new ArgumentException("a short cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(short[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    short maxi2 = am22.getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + arrayI[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayI[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a short cannot be added to a char");
                case 17:
                    throw new ArgumentException("a short cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // add a constant to the elements of the internal array
        public ArrayMaths plus(decimal constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i] + (constant));
                    }
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].plus((double) constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a decimal cannot be added to a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(byte constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a byte cannot be added to a char");
                case 17:
                    throw new ArgumentException("a byte cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(byte[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] + arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] + arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    byte maxi2 = am22.getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] + (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 + arrayI[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 + arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).plus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayI[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a byte cannot be added to a char");
                case 17:
                    throw new ArgumentException("a byte cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(decimal[] arrayBD)
        {
            int nArg = arrayBD.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i] + (arrayBD[i]));
                    }
                    Converter.restoreMessages();
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].plus((double) arrayBD[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayBD[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a decimal cannot be added to a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(ComplexClass constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].plus(constant));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + constant);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be added to a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(ComplexClass[] arrayC)
        {
            int nArg = arrayC.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].plus(arrayC[i]));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((string) m_array[i] + arrayC[i]);
                    }
                    am.type = type;
                    break;
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be added to a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be added to a char");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }


        // add a constant to the elements of the internal array
        public ArrayMaths plus(string constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    string[] ss = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i] + constant);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }


        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(string[] arraySt)
        {
            int nArg = arraySt.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    string[] ss = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i] + arraySt[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // add a constant to the elements of the internal array
        public ArrayMaths plus(char constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    string[] ss = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i] + constant);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(char[] arrayCh)
        {
            int nArg = arrayCh.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    string[] ss = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i] + arrayCh[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(List<Object> vec)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am1 = new ArrayMaths();
            ArrayMaths am2 = new ArrayMaths(vec);

            switch (am2.type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = am2.getArray_as_double();
                    am1 = plus(dd);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long[] ll = am2.getArray_as_long();
                    am1 = plus(ll);
                    break;
                case 12:
                    decimal[] bd = am2.getArray_as_decimal();
                    am1 = plus(bd);
                    break;
                case 13:
                    long[] bi = am2.getArray_as_long();
                    am1 = plus(bi);
                    break;
                case 14:
                    ComplexClass[] cc = am2.getArray_as_Complex();
                    am1 = plus(cc);
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ct = am2.getArray_as_char();
                    am1 = plus(ct);
                    break;
                case 18:
                    string[] st = am2.getArray_as_string();
                    am1 = plus(st);
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am1.getArray_as_Object(), am1.m_minmax, maxminIndices, am1.typeName, am1.type);
            am1.maxIndex = maxminIndices[0];
            am1.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am1;
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(ArrayMaths arrayM)
        {
            List<Object> arrayl = arrayM.getArray_as_ArrayList();
            return plus(arrayl);
        }

        // add the elements of an array to the elements of the internal array
        public ArrayMaths plus(Stat arrayS)
        {
            List<Object> arrayl = arrayS.getArray_as_ArrayList();
            return plus(arrayl);
        }

        // SUBTRACTION
        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(double constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - constant);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - (decimal) constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = (decimal) (hold1 - constant);
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a double or float cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a double or float cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(double[] arrayD)
        {
            if (intLength != arrayD.Length)
            {
                throw new ArgumentException("The Length of the argument array, " + arrayD.Length +
                                            ", and the Length of this instance internal array, " + intLength +
                                            ", must be equal");
            }
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - arrayD[i]);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - (decimal) (arrayD[i]);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = (decimal) (hold1 - arrayD[i]);
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(arrayD[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a double or float cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a double or float cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(float constant)
        {
            double constantd = constant;
            return minus(constantd);
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(float[] arrayF)
        {
            if (intLength != arrayF.Length)
            {
                throw new ArgumentException("The Length of the argument array, " + arrayF.Length +
                                            ", and the Length of this instance internal array, " + intLength +
                                            ", must be equal");
            }
            double[] arrayD = new double[intLength];
            for (int i = 0; i < intLength; i++)
            {
                arrayD[i] = arrayF[i];
            }
            return minus(arrayD);
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(long constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a long cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(long[] arrayL)
        {
            int nArg = arrayL.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - arrayL[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max1 = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayL);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max1) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - arrayL[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) arrayL[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - arrayL[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - arrayL[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(arrayL[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a long cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(int constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("an int cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("an int cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("an int cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(int[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    int maxi2 = am22.getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - (arrayI[i]);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("an int cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("an int cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("an int cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(short constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a short cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a short cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a short cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(short[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    short maxi2 = am22.getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - (arrayI[i]);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a long cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a short cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(decimal constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i] - (constant));
                    }
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].minus((double) constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a decimal cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a decimal cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(byte constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a byte cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a byte cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a byte cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(byte[] arrayI)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i] - arrayI[i]);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    ArrayMaths am2 = new ArrayMaths(arrayI);
                    long max2 = am2.getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= max2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i] - arrayI[i]));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    ArrayMaths am22 = new ArrayMaths(arrayI);
                    byte maxi2 = am22.getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    if ((int.MaxValue - maxi) >= maxi2)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - arrayI[i]);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i] - (double) arrayI[i]);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1 - arrayI[i];
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1 - arrayI[i];
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).minus(arrayI[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a byte cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a byte cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a byte cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(decimal[] arrayBD)
        {
            int nArg = arrayBD.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i] + (arrayBD[i]));
                    }
                    Converter.restoreMessages();
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].minus((double) arrayBD[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a decimal cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a decimalcannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // subtract a constant from the elements of the internal array
        public ArrayMaths minus(ComplexClass constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].minus(constant));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(ComplexClass[] arrayC)
        {
            int nArg = arrayC.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].minus(arrayC[i]));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a char");
                case 18:
                    throw new ArgumentException("a ComplexClass cannot be subtracted from a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(List<Object> vec)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am1 = new ArrayMaths();
            ArrayMaths am2 = new ArrayMaths(vec);

            switch (am2.type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = am2.getArray_as_double();
                    am1 = minus(dd);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long[] ll = am2.getArray_as_long();
                    am1 = minus(ll);
                    break;
                case 12:
                    decimal[] bd = am2.getArray_as_decimal();
                    am1 = minus(bd);
                    break;
                case 13:
                    long[] bi = am2.getArray_as_long();
                    am1 = minus(bi);
                    break;
                case 14:
                    ComplexClass[] cc = am2.getArray_as_Complex();
                    am1 = minus(cc);
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("List/char subtraction not allowed");
                case 17:
                    throw new ArgumentException("List/char subtraction not allowed");
                case 18:
                    throw new ArgumentException("List/string subtraction not allowed");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am1.getArray_as_Object(), am1.m_minmax, maxminIndices, am1.typeName, am1.type);
            am1.maxIndex = maxminIndices[0];
            am1.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am1;
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(ArrayMaths arrayM)
        {
            List<Object> arrayl = arrayM.getArray_as_ArrayList();
            return minus(arrayl);
        }

        // Subtract the elements of an array from the elements of the internal array
        public ArrayMaths minus(Stat arrayS)
        {
            List<Object> arrayl = arrayS.getArray_as_ArrayList();
            return minus(arrayl);
        }

        // MULTIPLICATION
        // multiply the elements of the internal array by a constant
        public ArrayMaths times(double constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*constant);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1*(decimal) constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = (decimal) (hold1*constant);
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).times(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a double or float cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a double or float cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(float constant)
        {
            double constantd = constant;
            return times(constantd);
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(long constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i]*constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1*constant;
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1*constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).times(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a long cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a long cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a long cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(int constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i]*constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1*(constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1*constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).times(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("an int cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("an int cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("an int cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(short constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i]*constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1*(constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1*constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).times(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a short cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a short cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a short cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(decimal constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i]*(constant));
                    }
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].times((double) constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a decimal cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a decimal cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(byte constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i]*constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    if ((int.MaxValue - maxi) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*constant);
                        }
                        am.type = 6;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(lll[i]*(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1*(constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1*constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).times(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a byte cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a byte cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a byte cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // multiply the elements of the internal array by a constant
        public ArrayMaths times(ComplexClass constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].times(constant));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be multiplied by a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be multiplied by a char");
                case 18:
                    throw new ArgumentException("a ComplexClass cannot be multiplied by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }


        // DIVISION
        // divide the elements of the internal array by a constant
        public ArrayMaths over(double constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1/(decimal) (constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = type;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        decimal hold2 = (hold1)/((decimal) constant);
                        am.m_array.Add(hold2);
                    }
                    am.type = 12;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).over(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a double or float cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a double or float cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a double or float cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(float constant)
        {
            double constantd = constant;
            return over(constantd);
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(long constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    if ((long.MaxValue - max) >= constant)
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add((ll[i]/constant));
                        }
                        am.type = 4;
                    }
                    else
                    {
                        for (int i = 0; i < intLength; i++)
                        {
                            am.m_array.Add(ll[i]/(double) constant);
                        }
                        am.type = 0;
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1/(constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1/constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).over(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a long cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a long cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a long cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(int constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]/constant);
                    }
                    am.type = 4;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    int maxi = getMaximum_as_int();
                    int[] lll = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(lll[i]/constant);
                    }
                    am.type = 6;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1/(constant);
                        am.m_array.Add(hold1);
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1/constant;
                        am.m_array.Add(hold2);
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).over(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("an int cannot be divided by a char");
                case 17:
                    throw new ArgumentException("an int cannot be divided by a char");
                case 18:
                    throw new ArgumentException("an int cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(short constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]/constant);
                    }
                    am.type = 4;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    short maxi = getMaximum_as_short();
                    short[] lll = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(lll[i]/constant);
                    }
                    am.type = 6;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1/(constant);
                        am.m_array.Add(hold1);
                        hold1 = 0;
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1/constant;
                        am.m_array.Add(hold2);
                        hold1 = 0;
                        hold2 = 0;
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).over(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a short cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a short cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a short cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(decimal constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i]/(constant));
                    }
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].over((double) constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a decimal cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a decimal cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a decimal cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(byte constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;

            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 4:
                    long max = getMaximum_as_long();
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]/constant);
                    }
                    am.type = 4;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    byte maxi = getMaximum_as_byte();
                    byte[] lll = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(lll[i]/constant);
                    }
                    am.type = 0;
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        decimal hold1 = (decimal) (m_array[i]);
                        hold1 = hold1/(constant);
                        am.m_array.Add(hold1);
                    }
                    am.type = 12;
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        long hold1 = (long) (m_array[i]);
                        long hold2 = hold1/constant;
                        am.m_array.Add(hold2);
                    }
                    am.type = 13;
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((ComplexClass) m_array[i]).over(constant));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a byte cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a byte cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a byte cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // divide the elements of the internal array by a constant
        public ArrayMaths over(ComplexClass constant)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].over(constant));
                    }
                    am.type = 14;
                    break;
                case 15:
                case 16:
                    throw new ArgumentException("a ComplexClass cannot be divided by a char");
                case 17:
                    throw new ArgumentException("a ComplexClass cannot be divided by a char");
                case 18:
                    throw new ArgumentException("a ComplexClass cannot be divided by a string");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // TRUNCATION AND ROUNDING OF ARRAY ELEMENTS
        // Returns new ArrayMaths with array elements truncated to n decimal places
        public ArrayMaths truncate(int n)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Fmath.truncate(dd[i], n));
                    }
                    am.type = type;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Fmath.truncate(ff[i], n));
                    }
                    am.type = type;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]);
                    }
                    am.type = type;
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ii[i]));
                    }
                    am.type = type;
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    am.type = type;
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    am.type = type;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Round(bd[i], n));
                    }
                    am.type = type;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    am.type = type;
                    break;
                case 14:
                    ComplexClass[] co = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.truncate(co[i], n));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ch = getArray_as_char();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ch[i]));
                    }
                    am.type = type;
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(st[i]);
                    }
                    am.type = type;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }


        // Returns new ArrayMaths with array elements rounded down to the nearest lower integer
        public ArrayMaths Floor()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Floor(dd[i]));
                    }
                    am.type = type;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Floor(ff[i]));
                    }
                    am.type = type;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]);
                    }
                    am.type = type;
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ii[i]));
                    }
                    am.type = type;
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    am.type = type;
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    am.type = type;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Floor(bd[i]));
                    }
                    am.type = type;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    am.type = type;
                    break;
                case 14:
                    ComplexClass[] co = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(new ComplexClass(Math.Floor(co[i].getReal()), Math.Floor(co[i].getImag())));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ch = getArray_as_char();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ch[i]));
                    }
                    am.type = type;
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(st[i]);
                    }
                    am.type = type;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }


        // Returns new ArrayMaths with array elements rounded up to the nearest higher integer
        public ArrayMaths Ceiling()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Ceiling(dd[i]));
                    }
                    am.type = type;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Ceiling(ff[i]));
                    }
                    am.type = type;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]);
                    }
                    am.type = type;
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ii[i]));
                    }
                    am.type = type;
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    am.type = type;
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    am.type = type;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Ceiling(bd[i]));
                    }
                    am.type = type;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    am.type = type;
                    break;
                case 14:
                    ComplexClass[] co = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(new ComplexClass(Math.Ceiling(co[i].getReal()), Math.Ceiling(co[i].getImag())));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ch = getArray_as_char();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ch[i]));
                    }
                    am.type = type;
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(st[i]);
                    }
                    am.type = type;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }


        // Returns new ArrayMaths with array elements rounded to a value that is closest in value to the element and is equal to a mathematical integer.
        public ArrayMaths rint()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Converter.rint(dd[i]));
                    }
                    am.type = type;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Converter.rint(ff[i]));
                    }
                    am.type = type;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ll[i]);
                    }
                    am.type = type;
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ii[i]));
                    }
                    am.type = type;
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    am.type = type;
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    am.type = type;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Round(bd[i], 0));
                    }
                    am.type = type;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    am.type = type;
                    break;
                case 14:
                    ComplexClass[] co = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(new ComplexClass(Converter.rint(co[i].getReal()), Converter.rint(co[i].getImag())));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ch = getArray_as_char();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ch[i]));
                    }
                    am.type = type;
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(st[i]);
                    }
                    am.type = type;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }


        // ARRAY REVERSAL
        // Returns new ArrayMaths with array elements reversed
        public ArrayMaths reverse()
        {
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            am.type = type;
            am.sortedIndices = new int[intLength];

            for (int i = 0; i < intLength; i++)
            {
                am.m_array.Add(m_array[intLength - i - 1]);
                am.sortedIndices[i] = intLength - i - 1;
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            return am;
        }


        // ARRAY LOGARITHMS
        // Returns new ArrayMaths with array elements converted to their natural logs
        public ArrayMaths Log()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Log(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Log(cc[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements converted to log to the base 2
        public ArrayMaths log2()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Fmath.log2(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Log(cc[i].over(Math.Log(2.0))));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements converted to Log10(array element)
        public ArrayMaths Log10()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Log10(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Log(cc[i].over(Math.Log(10.0))));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements converted to antilog10(element)
        public ArrayMaths antilog10()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Pow(10.0, dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Pow(10.0, cc[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements, x, converted to x.log2(x)
        public ArrayMaths xLog2x()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*Fmath.log2(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].times(ComplexClass.Log(cc[i].over(Math.Log(2)))));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements, x, converted to x.loge(x)
        public ArrayMaths xLogEx()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*Math.Log(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].times(ComplexClass.Log(cc[i])));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements, x, converted to x.Log10(x)
        public ArrayMaths xLog10x()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]*Math.Log10(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].times(ComplexClass.Log(cc[i].over(Math.Log(10)))));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements, x, converted to -x.log2(x)
        public ArrayMaths minusxLog2x()
        {
            ArrayMaths am = xLog2x();
            return am.negate();
        }

        // Returns new ArrayMaths with array elements, x, converted to -x.loge(x)
        public ArrayMaths minusxLogEx()
        {
            ArrayMaths am = xLogEx();
            return am.negate();
        }

        // Returns new ArrayMaths with array elements, x, converted to -x.Log10(x)
        public ArrayMaths minusxLog10x()
        {
            ArrayMaths am = xLog10x();
            return am.negate();
        }

        // SQUARE ROOTS
        // Returns new ArrayMaths with array elements converted to their square roots
        public ArrayMaths sqrt()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Sqrt(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Sqrt(cc[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements converted to 1.0/sqrt(element)
        public ArrayMaths oneOverSqrt()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(1.0D/Math.Sqrt(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ComplexClass.Sqrt(cc[i])).inverse());
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // VARIOUS TRANSFORMATION OF ARRAY ELEMENTS
        // Returns new ArrayMaths with array elements converted to their absolute values
        public ArrayMaths abs()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(dd[i]));
                    }
                    am.type = type;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(ff[i]));
                    }
                    am.type = type;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((Math.Abs(ll[i])));
                    }
                    am.type = type;
                    break;
                case 6:
                case 7:
                    int[] ii1 = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(ii1[i]));
                    }
                    am.type = type;
                    break;
                case 8:
                case 9:
                    int[] ii2 = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(ii2[i]));
                    }
                    am.type = type;
                    break;
                case 10:
                case 11:
                    int[] ii3 = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((byte) Math.Abs(ii3[i])));
                    }
                    am.type = type;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(bd[i]));
                    }
                    am.type = type;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(bi[i]));
                    }
                    am.type = type;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Abs(cc[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                case 16:
                case 17:
                    int[] ii4 = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(ii4[i]));
                    }
                    am.type = type;
                    break;
                case 18:
                    double[] dd2 = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Abs(dd2[i]));
                    }
                    am.type = 1;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // Returns new ArrayMaths with array elements converted to Exp(element)
        public ArrayMaths Exp()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Exp(dd[i]));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Exp(cc[i]));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements converted to 1.0/element
        public ArrayMaths invert()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(1.0D/dd[i]);
                    }
                    am.type = 1;
                    break;
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((decimal.One)/(bd[i]));
                    }
                    am.type = 12;
                    bd = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ComplexClass.plusOne()).over(cc[i]));
                    }
                    am.type = 14;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(int n)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Pow(dd[i], n));
                    }
                    am.type = 1;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal bdpow = decimal.One;
                    for (int i = 0; i < intLength; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            bdpow = bdpow*(bd[i]);
                        }
                        am.m_array.Add(bdpow);
                    }
                    am.type = 12;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Pow(bi[i], n));
                    }
                    am.type = 13;
                    bi = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Pow(cc[i], n));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(double n)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }

            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Pow(dd[i], n));
                    }
                    am.type = 1;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Pow(cc[i], n));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(float n)
        {
            double nn = n;
            return Pow(nn);
        }

        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(long n)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(Math.Pow(dd[i], n));
                    }
                    am.type = 1;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal bdpow = decimal.One;
                    for (int i = 0; i < intLength; i++)
                    {
                        long j = 0L;
                        while (j < n)
                        {
                            bdpow = bdpow*(bd[i]);
                        }
                        am.m_array.Add(bdpow);
                    }
                    am.type = 12;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    long bipow = 1;
                    for (int i = 0; i < intLength; i++)
                    {
                        long j = 0L;
                        while (j < n)
                        {
                            bipow = bipow*(bi[i]);
                        }
                        am.m_array.Add(bipow);
                    }
                    am.type = 13;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ComplexClass.Pow(cc[i], n));
                    }
                    am.type = type;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }

        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(short n)
        {
            int ii = n;
            return Pow(ii);
        }

        // Returns new ArrayMaths with array elements raised to a power n
        public ArrayMaths Pow(byte n)
        {
            int ii = n;
            return Pow(ii);
        }

        // Returns new ArrayMaths with array elements converted to -element
        public ArrayMaths negate()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            switch (type)
            {
                case 0:
                case 1:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-dd[i]);
                    }
                    am.type = 1;
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-ff[i]);
                    }
                    am.type = 3;
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((-ll[i]));
                    }
                    am.type = 5;
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-ii[i]);
                    }
                    am.type = 7;
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-ss[i]);
                    }
                    am.type = 9;
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(((byte) (-bb[i])));
                    }
                    am.type = 11;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-bd[i]);
                    }
                    am.type = 12;
                    bd = null;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(-bi[i]);
                    }
                    am.type = 13;
                    bi = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(cc[i].negate());
                    }
                    am.type = 14;
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];
            Converter.restoreMessages();
            return am;
        }


        // protected method for calcuating the sum of the array elements
        // called by the public methods
        protected void calcSum()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    double[] dd = getArray_as_double();
                    double sum = 0.0D;
                    for (int i = 0; i < intLength; i++)
                    {
                        sum += dd[i];
                    }
                    summ.Add(sum);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    long[] ll = getArray_as_long();
                    long suml = 0L;
                    bool test = false;
                    for (int i = 0; i < intLength; i++)
                    {
                        if (long.MaxValue - suml < ll[i])
                        {
                            test = true;
                        }
                        suml += ll[i];
                    }
                    if (test)
                    {
                        double[] dd2 = getArray_as_double();
                        double sum2 = 0.0D;
                        for (int i = 0; i < intLength; i++)
                        {
                            sum2 += dd2[i];
                        }
                        summ.Add(sum2);
                        sumlongToDouble = true;
                    }
                    else
                    {
                        summ.Add((suml));
                    }
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal sumbd = 0.0M;
                    for (int i = 0; i < intLength; i++)
                    {
                        sumbd += (bd[i]);
                    }
                    summ.Add(sumbd);
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    long sumbi = 0;
                    for (int i = 0; i < intLength; i++)
                    {
                        sumbi += (bi[i]);
                    }
                    summ.Add(sumbi);
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    ComplexClass sumcc = ComplexClass.zero();
                    for (int i = 0; i < intLength; i++)
                    {
                        sumcc.plus(cc[i]);
                    }
                    summ.Add(sumcc);
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            sumDone = true;
            Converter.restoreMessages();
        }


        // returns the sum of the array elements as a double
        public double sum()
        {
            return getSum_as_double();
        }

        public double sum_as_double()
        {
            return getSum_as_double();
        }

        public double getSum()
        {
            return getSum_as_double();
        }

        public double getSum_as_double()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            double sum = 0.0D;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = ((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = ((double) summ[0]);
                    }
                    else
                    {
                        sum = Converter.convert_long_to_double((long) summ[0]);
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_double((decimal) summ[0]);
                    break;
                case 13:
                    sum = Converter.convert_int_to_double((int) summ[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as double is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a float
        public float sum_as_float()
        {
            return getSum_as_float();
        }

        public float getSum_as_float()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            float sum = 0.0F;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_float((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_float((double) summ[0]);
                    }
                    else
                    {
                        sum = Converter.convert_long_to_float((long) summ[0]);
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_float((decimal) summ[0]);
                    break;
                case 13:
                    sum = Converter.convert_int_to_float((int) summ[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as float is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a long
        public long sum_as_long()
        {
            return getSum_as_long();
        }

        public long getSum_as_long()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            long sum = 0L;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_long((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_long((double) summ[0]);
                    }
                    else
                    {
                        sum = (long) summ[0];
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_long((decimal) summ[0]);
                    break;
                case 13:
                    sum = Converter.convert_int_to_long((int) summ[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as long is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as an int
        public int sum_as_int()
        {
            return getSum_as_int();
        }

        public int getSum_as_int()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            int sum = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_int((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_int((double) summ[0]);
                    }
                    else
                    {
                        sum = Converter.convert_long_to_int((long) summ[0]);
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_int((decimal) summ[0]);
                    break;
                case 13:
                    sum = (int) summ[0];
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as int is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a short
        public short sum_as_short()
        {
            return getSum_as_short();
        }

        public short getSum_as_short()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            short sum = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_short((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_short((double) summ[0]);
                    }
                    else
                    {
                        sum = Converter.convert_long_to_short((long) summ[0]);
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_short((decimal) summ[0]);
                    break;
                case 13:
                    sum = Converter.convert_int_to_short((long) summ[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as short is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a byte
        public byte sum_as_byte()
        {
            return getSum_as_byte();
        }

        public byte getSum_as_byte()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            byte sum = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_byte((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_byte((double) summ[0]);
                    }
                    else
                    {
                        sum = Converter.convert_long_to_byte((long) summ[0]);
                    }
                    break;
                case 12:
                    sum = Converter.convert_decimal_to_byte((decimal) summ[0]);
                    break;
                case 13:
                    sum = Converter.convert_int_to_byte((long) summ[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as byte is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a decimal
        public decimal sum_as_decimal()
        {
            return getSum_as_decimal();
        }

        public decimal getSum_as_decimal()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            decimal sum = 0.0M;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = Converter.convert_double_to_decimal((double) summ[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = Converter.convert_double_to_decimal((double) summ[0]);
                    }
                    else
                    {
                        sum = (decimal) summ[0];
                    }
                    break;
                case 12:
                    sum = (decimal) summ[0];
                    break;
                case 13:
                    sum = (decimal) summ[0];
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as decimal is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // returns the sum of the array elements as a ComplexClass
        public ComplexClass sum_as_Complex()
        {
            return getSum_as_Complex();
        }

        public ComplexClass getSum_as_Complex()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            ComplexClass sum = ComplexClass.zero();
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = new ComplexClass(((double) summ[0]));
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = new ComplexClass(((double) summ[0]));
                    }
                    else
                    {
                        sum = new ComplexClass(((long) summ[0]));
                    }
                    break;
                case 12:
                    sum = new ComplexClass(((decimal) summ[0]));
                    break;
                case 13:
                    sum = new ComplexClass(((long) summ[0]));
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as ComplexClass is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }


        // returns the sum of the array elements as a string
        public string sum_as_string()
        {
            return getSum_as_string();
        }

        public string getSum_as_string()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!sumDone)
            {
                calcSum();
            }
            string sum = " ";
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    sum = (string) summ[0];
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (sumlongToDouble)
                    {
                        sum = (string) summ[0];
                    }
                    else
                    {
                        sum = (string) summ[0];
                    }
                    break;
                case 12:
                    sum = ((decimal) summ[0]).ToString();
                    break;
                case 13:
                    sum = ((long) summ[0]).ToString();
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a sum as string is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return sum;
        }

        // protected method for calcuating the product of the array elements
        // called by the public methods
        protected void calcProduct()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    double[] dd = getArray_as_double();
                    double product = 1.0D;
                    for (int i = 0; i < intLength; i++)
                    {
                        product *= dd[i];
                    }
                    productt.Add(product);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    long[] ll = getArray_as_long();
                    long productl = 1L;
                    bool test = false;
                    for (int i = 0; i < intLength; i++)
                    {
                        if (long.MaxValue/productl < ll[i])
                        {
                            test = true;
                        }
                        productl += ll[i];
                    }
                    if (test)
                    {
                        double[] dd2 = getArray_as_double();
                        double product2 = 1.0D;
                        for (int i = 0; i < intLength; i++)
                        {
                            product2 *= dd2[i];
                        }
                        productt.Add(product2);
                        sumlongToDouble = true;
                    }
                    else
                    {
                        productt.Add((productl));
                    }
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal productbd = 1.0M;
                    for (int i = 0; i < intLength; i++)
                    {
                        productbd *= (bd[i]);
                    }
                    productt.Add(productbd);
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    long productbi = 1;
                    for (int i = 0; i < intLength; i++)
                    {
                        productbi *= (bi[i]);
                    }
                    productt.Add(productbi);
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    ComplexClass productcc = ComplexClass.plusOne();
                    for (int i = 0; i < intLength; i++)
                    {
                        productcc.times(cc[i]);
                    }
                    productt.Add(productcc);
                    break;
                case 15:
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            productDone = true;
            Converter.restoreMessages();
        }

        // returns the product of the array elements as a double
        public double product()
        {
            return getProduct_as_double();
        }

        public double product_as_double()
        {
            return getProduct_as_double();
        }

        public double getProduct()
        {
            return getProduct_as_double();
        }

        public double getProduct_as_double()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            double product = 0.0D;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = ((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = ((double) productt[0]);
                    }
                    else
                    {
                        product = Converter.convert_long_to_double((long) productt[0]);
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_double((decimal) productt[0]);
                    break;
                case 13:
                    product = Converter.convert_int_to_double((int) productt[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas double is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a float
        public float product_as_float()
        {
            return getProduct_as_float();
        }

        public float getProduct_as_float()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            float product = 0.0F;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_float((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_float((double) productt[0]);
                    }
                    else
                    {
                        product = Converter.convert_long_to_float((long) productt[0]);
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_float((decimal) productt[0]);
                    break;
                case 13:
                    product = Converter.convert_int_to_float((int) productt[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas float is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a long
        public long product_as_long()
        {
            return getProduct_as_long();
        }

        public long getProduct_as_long()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            long product = 0L;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_long((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_long((double) productt[0]);
                    }
                    else
                    {
                        product = (long) productt[0];
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_long((decimal) productt[0]);
                    break;
                case 13:
                    product = Converter.convert_int_to_long((int) productt[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas long is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as an int
        public int product_as_int()
        {
            return getProduct_as_int();
        }

        public int getProduct_as_int()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            int product = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_int((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_int((double) productt[0]);
                    }
                    else
                    {
                        product = Converter.convert_long_to_int((long) productt[0]);
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_int((decimal) productt[0]);
                    break;
                case 13:
                    product = (int) productt[0];
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas int is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a short
        public short product_as_short()
        {
            return getProduct_as_short();
        }

        public short getProduct_as_short()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            short product = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_short((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_short((double) productt[0]);
                    }
                    else
                    {
                        product = Converter.convert_long_to_short((long) productt[0]);
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_short((decimal) productt[0]);
                    break;
                case 13:
                    product = Converter.convert_int_to_short((long) productt[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas short is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a byte
        public byte product_as_byte()
        {
            return getProduct_as_byte();
        }

        public byte getProduct_as_byte()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            byte product = 0;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_byte((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_byte((double) productt[0]);
                    }
                    else
                    {
                        product = Converter.convert_long_to_byte((long) productt[0]);
                    }
                    break;
                case 12:
                    product = Converter.convert_decimal_to_byte((decimal) productt[0]);
                    break;
                case 13:
                    product = Converter.convert_int_to_byte((long) productt[0]);
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas byte is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a decimal
        public decimal product_as_decimal()
        {
            return getProduct_as_decimal();
        }

        public decimal getProduct_as_decimal()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            decimal product = 0.0M;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = Converter.convert_double_to_decimal((double) productt[0]);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = Converter.convert_double_to_decimal((double) productt[0]);
                    }
                    else
                    {
                        product = (decimal) productt[0];
                    }
                    break;
                case 12:
                    product = (decimal) productt[0];
                    break;
                case 13:
                    product = (decimal) productt[0];
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas decimal is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a ComplexClass
        public ComplexClass product_as_Complex()
        {
            return getProduct_as_Complex();
        }

        public ComplexClass getProduct_as_Complex()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            ComplexClass product = ComplexClass.zero();
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = new ComplexClass(((double) productt[0]));
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = new ComplexClass(((double) productt[0]));
                    }
                    else
                    {
                        product = new ComplexClass(((long) productt[0]));
                    }
                    break;
                case 12:
                    product = new ComplexClass(((decimal) productt[0]));
                    break;
                case 13:
                    product = new ComplexClass(((long) productt[0]));
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas ComplexClass is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // returns the product of the array elements as a string
        public string product_as_string()
        {
            return getProduct_as_string();
        }

        public string getProduct_as_string()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            if (!productDone)
            {
                calcProduct();
            }
            string product = " ";
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 18:
                    product = (string) productt[0];
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    if (productlongToDouble)
                    {
                        product = (string) productt[0];
                    }
                    else
                    {
                        product = (string) productt[0];
                    }
                    break;
                case 12:
                    product = ((decimal) productt[0]).ToString();
                    break;
                case 13:
                    product = ((long) productt[0]).ToString();
                    break;
                case 14:
                case 15:
                    throw new ArgumentException("The " + typeName[type] +
                                                " is not a numerical type for which a productas string is meaningful/supported");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }

            Converter.restoreMessages();
            return product;
        }

        // randomize the order of the elements in the internal array
        public ArrayMaths randomize()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            am.type = type;
            RngWrapper ran = new RngWrapper();
            am.sortedIndices = ran.uniqueIntegerArray(intLength - 1);

            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((double) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((float) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((int) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((short) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((byte) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((decimal) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((char) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(m_array[am.sortedIndices[i]]);
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // randomize the order of the elements in the internal array
        public ArrayMaths randomise()
        {
            return randomize();
        }


        // sort the array into ascending order
        public ArrayMaths sort()
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            am.type = type;
            am.sortedIndices = new int[intLength];

            switch (type)
            {
                case 0:
                case 1:
                    double[] dd1 = getArray_as_double();
                    am.sortedIndices = sortWithIndices(dd1);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((double) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 2:
                case 3:
                    double[] dd2 = getArray_as_double();
                    am.sortedIndices = sortWithIndices(dd2);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((float) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 4:
                case 5:
                    long[] ll1 = getArray_as_long();
                    am.sortedIndices = sortWithIndices(ll1);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 6:
                case 7:
                    long[] ll2 = getArray_as_long();
                    am.sortedIndices = sortWithIndices(ll2);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((int) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 8:
                case 9:
                    long[] ll3 = getArray_as_long();
                    am.sortedIndices = sortWithIndices(ll3);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((short) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 10:
                case 11:
                    long[] ll4 = getArray_as_long();
                    am.sortedIndices = sortWithIndices(ll4);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((byte) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    am.sortedIndices = sortWithIndices(bd);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((decimal) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    am.sortedIndices = sortWithIndices(bi);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 14:
                    ArrayMaths am2 = abs();
                    double[] cc = am2.getArray_as_double();
                    am.sortedIndices = sortWithIndices(cc);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    long[] ii = getArray_as_long();
                    am.sortedIndices = sortWithIndices(ii);
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((char) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 18:
                    throw new ArgumentException("Alphabetic sorting is not supported by this method");
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // order an array to a given sequence of indices
        public ArrayMaths sort(int[] indices)
        {
            int nArg = indices.Length;
            if (intLength != nArg)
            {
                throw new ArgumentException("The argument array [Length = " + nArg +
                                            "], must be of the same Length as this instance array [Length = " +
                                            intLength + "]");
            }

            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = intLength;
            am.type = type;
            am.sortedIndices = indices;
            switch (type)
            {
                case 0:
                case 1:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((double) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 2:
                case 3:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((float) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 4:
                case 5:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 6:
                case 7:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((int) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 8:
                case 9:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((short) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 10:
                case 11:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((byte) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 12:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((decimal) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 13:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((long) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 14:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((char) m_array[am.sortedIndices[i]]);
                    }
                    break;
                case 18:
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(m_array[am.sortedIndices[i]]);
                    }
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            if (type != 18)
            {
                findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            }
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // sort elements in an array into ascending order
        // using selection sort method
        // returns indices of the sorted array
        protected int[] sortWithIndices(double[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            double holdb = 0.0D;
            int holdi = 0;
            double[] bb = new double[intLength];
            int[] indices = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != intLength - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < intLength; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
            return indices;
        }

        // protected method for obtaining original indices of a sorted array
        // called by public sort methods
        protected int[] sortWithIndices(long[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            long holdb = 0L;
            int holdi = 0;
            long[] bb = new long[intLength];
            int[] indices = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != intLength - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < intLength; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
            return indices;
        }

        // protected method for obtaining original indices of a sorted array
        // called by public sort methods
        protected int[] sortWithIndices(decimal[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            decimal holdb = decimal.Zero;
            int holdi = 0;
            decimal[] bb = new decimal[intLength];
            int[] indices = new int[intLength];
            for (int i = 0; i < intLength; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != intLength - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < intLength; i++)
                {
                    if (bb[i].CompareTo(bb[index]) == -1)
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
            return indices;
        }

        // return original indices of sorted array
        public int[] originalIndices()
        {
            if (sortedIndices == null)
            {
                PrintToScreen.WriteLine("method: originalIndices: array has not been sorted: null returned");
            }
            return sortedIndices;
        }


        // concatenates two arrays
        public ArrayMaths concatenate(double[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    double[] yy = getArray_as_double();
                    double[] zz = new double[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        zz[i] = yy[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        zz[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(zz[i]);
                    }
                    am.type = 1;
                    break;
                case 12:
                case 13:
                    decimal[] bd1 = getArray_as_decimal();
                    am2 = new ArrayMaths(xx);
                    decimal[] bd2 = am2.getArray_as_decimal();
                    decimal[] bda = new decimal[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bda[i] = bd1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bda[i + intLength] = bd2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bda[i]);
                    }
                    bd1 = null;
                    bd2 = null;
                    bda = null;
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(float[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] yy = getArray_as_double();
                    double[] zz = new double[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        zz[i] = yy[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        zz[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(zz[i]);
                    }
                    am.type = 1;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    float[] ff = getArray_as_float();
                    float[] gg = new float[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        gg[i] = ff[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        gg[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(gg[i]);
                    }
                    am.type = 3;
                    break;
                case 12:
                case 13:
                    decimal[] bd1 = getArray_as_decimal();
                    am2 = new ArrayMaths(xx);
                    decimal[] bd2 = am2.getArray_as_decimal();
                    decimal[] bda = new decimal[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bda[i] = bd1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bda[i + intLength] = bd2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bda[i]);
                    }
                    bd1 = null;
                    bd2 = null;
                    bda = null;
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(long[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] yy = getArray_as_double();
                    double[] zz = new double[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        zz[i] = yy[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        zz[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(zz[i]);
                    }
                    am.type = 1;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                    long[] ll = getArray_as_long();
                    long[] mm = new long[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        mm[i] = ll[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        mm[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add((mm[i]));
                    }
                    am.type = 3;
                    break;
                case 12:
                    decimal[] bd1 = getArray_as_decimal();
                    am2 = new ArrayMaths(xx);
                    decimal[] bd2 = am2.getArray_as_decimal();
                    decimal[] bda = new decimal[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bda[i] = bd1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bda[i + intLength] = bd2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bda[i]);
                    }
                    bd1 = null;
                    bd2 = null;
                    bda = null;
                    am.type = 12;
                    break;
                case 13:
                    long[] bi1 = getArray_as_long();
                    am2 = new ArrayMaths(xx);
                    long[] bi2 = am2.getArray_as_long();
                    long[] bia = new long[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bia[i] = bi1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bia[i + intLength] = bi2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bia[i]);
                    }
                    bi1 = null;
                    bi2 = null;
                    bia = null;
                    am.type = 13;
                    break;
                case 14:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(int[] xx)
        {
            long[] dd = new long[xx.Length];
            for (int i = 0; i < xx.Length; i++)
            {
                dd[i] = xx[i];
            }
            return concatenate(dd);
        }

        // concatenates two arrays
        public ArrayMaths concatenate(short[] xx)
        {
            long[] dd = new long[xx.Length];
            for (int i = 0; i < xx.Length; i++)
            {
                dd[i] = xx[i];
            }
            return concatenate(dd);
        }

        // concatenates two arrays
        public ArrayMaths concatenate(byte[] xx)
        {
            long[] dd = new long[xx.Length];
            for (int i = 0; i < xx.Length; i++)
            {
                dd[i] = xx[i];
            }
            return concatenate(dd);
        }

        // concatenates two arrays
        public ArrayMaths concatenate(decimal[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 16:
                case 17:
                    decimal[] bd1 = getArray_as_decimal();
                    am2 = new ArrayMaths(xx);
                    decimal[] bd2 = am2.getArray_as_decimal();
                    decimal[] bda = new decimal[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bda[i] = bd1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bda[i + intLength] = bd2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bda[i]);
                    }
                    bd1 = null;
                    bd2 = null;
                    bda = null;
                    am.type = 12;
                    break;
                case 14:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 15:
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(ComplexClass[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(string[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(char[] xx)
        {
            int xlength = xx.Length;
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            ArrayMaths am = new ArrayMaths();
            am.m_array = new List<Object>();
            am.intLength = xx.Length + intLength;
            ArrayMaths am2 = null;
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    double[] yy = getArray_as_double();
                    double[] zz = new double[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        zz[i] = yy[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        zz[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(zz[i]);
                    }
                    am.type = 1;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    long[] ll = getArray_as_long();
                    long[] mm = new long[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        mm[i] = ll[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        mm[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add((mm[i]));
                    }
                    am.type = 3;
                    break;
                case 12:
                    decimal[] bd1 = getArray_as_decimal();
                    am2 = new ArrayMaths(xx);
                    decimal[] bd2 = am2.getArray_as_decimal();
                    decimal[] bda = new decimal[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bda[i] = bd1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bda[i + intLength] = bd2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bda[i]);
                    }
                    bd1 = null;
                    bd2 = null;
                    bda = null;
                    am.type = 12;
                    break;
                case 13:
                    long[] bi1 = getArray_as_long();
                    am2 = new ArrayMaths(xx);
                    long[] bi2 = am2.getArray_as_long();
                    long[] bia = new long[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        bia[i] = bi1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        bia[i + intLength] = bi2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(bia[i]);
                    }
                    bi1 = null;
                    bi2 = null;
                    bia = null;
                    am.type = 13;
                    break;
                case 14:
                    ComplexClass[] cc1 = getArray_as_Complex();
                    am2 = new ArrayMaths(xx);
                    ComplexClass[] cc2 = am2.getArray_as_Complex();
                    ComplexClass[] cca = new ComplexClass[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        cca[i] = cc1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        cca[i + intLength] = cc2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(cca[i]);
                    }
                    am.type = 14;
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ch = getArray_as_char();
                    char[] dh = new char[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        dh[i] = ch[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        dh[i + intLength] = xx[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add((dh[i]));
                    }
                    am.type = 1;
                    break;
                case 18:
                    string[] ss1 = getArray_as_string();
                    am2 = new ArrayMaths(xx);
                    string[] ss2 = am2.getArray_as_string();
                    string[] ssa = new string[am.intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ssa[i] = ss1[i];
                    }
                    for (int i = 0; i < xlength; i++)
                    {
                        ssa[i + intLength] = ss2[i];
                    }
                    for (int i = 0; i < am.intLength; i++)
                    {
                        am.m_array.Add(ssa[i]);
                    }
                    am.type = 18;
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(ArrayMaths xx)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int type = xx.type;
            ArrayMaths am = new ArrayMaths();
            switch (xx.type)
            {
                case 0:
                case 1:
                    double[] dd = xx.getArray_as_double();
                    am = concatenate(dd);
                    break;
                case 2:
                case 3:
                    float[] ff = xx.getArray_as_float();
                    am = concatenate(ff);
                    break;
                case 4:
                case 5:
                    long[] ll = xx.getArray_as_long();
                    am = concatenate(ll);
                    break;
                case 6:
                case 7:
                    int[] ii = xx.getArray_as_int();
                    am = concatenate(ii);
                    break;
                case 8:
                case 9:
                    short[] ss = xx.getArray_as_short();
                    am = concatenate(ss);
                    break;
                case 10:
                case 11:
                    byte[] bb = xx.getArray_as_byte();
                    am = concatenate(bb);
                    break;
                case 12:
                    decimal[] bd = xx.getArray_as_decimal();
                    am = concatenate(bd);
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    am = concatenate(bi);
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    am = concatenate(cc);
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ct = getArray_as_char();
                    am = concatenate(ct);
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    am = concatenate(st);
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }

        // concatenates two arrays
        public ArrayMaths concatenate(Stat xx)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int type = xx.type;
            ArrayMaths am = new ArrayMaths();
            switch (xx.type)
            {
                case 0:
                case 1:
                    double[] dd = xx.getArray_as_double();
                    am = concatenate(dd);
                    break;
                case 2:
                case 3:
                    float[] ff = xx.getArray_as_float();
                    am = concatenate(ff);
                    break;
                case 4:
                case 5:
                    long[] ll = xx.getArray_as_long();
                    am = concatenate(ll);
                    break;
                case 6:
                case 7:
                    int[] ii = xx.getArray_as_int();
                    am = concatenate(ii);
                    break;
                case 8:
                case 9:
                    short[] ss = xx.getArray_as_short();
                    am = concatenate(ss);
                    break;
                case 10:
                case 11:
                    byte[] bb = xx.getArray_as_byte();
                    am = concatenate(bb);
                    break;
                case 12:
                    decimal[] bd = xx.getArray_as_decimal();
                    am = concatenate(bd);
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    am = concatenate(bi);
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    am = concatenate(cc);
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ct = getArray_as_char();
                    am = concatenate(ct);
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    am = concatenate(st);
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
            int[] maxminIndices = new int[2];
            findMinMax(am.getArray_as_Object(), am.m_minmax, maxminIndices, am.typeName, am.type);
            am.maxIndex = maxminIndices[0];
            am.minIndex = maxminIndices[1];

            Converter.restoreMessages();
            return am;
        }


        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(double value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 0 || type == 1)
            {
                double[] arrayc = getArray_as_double();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare double or double with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(float value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 2 || type == 3)
            {
                float[] arrayc = getArray_as_float();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare float or float with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(long value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 4 || type == 5)
            {
                long[] arrayc = getArray_as_long();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare long or long with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(int value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 6 || type == 7)
            {
                int[] arrayc = getArray_as_int();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare int or int with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(short value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 8 || type == 9)
            {
                short[] arrayc = getArray_as_short();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare short or short with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(byte value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 10 || type == 11)
            {
                byte[] arrayc = getArray_as_byte();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare byte or byte with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(char value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 16 || type == 17)
            {
                char[] arrayc = getArray_as_char();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter] == value)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare char or char with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(string value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 18)
            {
                string[] arrayc = getArray_as_string();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter].Equals(value))
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare string with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(ComplexClass value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 14)
            {
                ComplexClass[] arrayc = getArray_as_Complex();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter].Equals(value))
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare ComplexClass with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }


        // finds the index of the first occurence of the element equal to a given value in an array
        // returns -1 if none found
        public int indexOf(decimal value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = -1;
            if (type == 12)
            {
                decimal[] arrayc = getArray_as_decimal();
                bool test = true;
                int counter = 0;
                while (test)
                {
                    if (arrayc[counter].CompareTo(value) == 0)
                    {
                        index = counter;
                        test = false;
                    }
                    else
                    {
                        counter++;
                        if (counter >= arrayc.Length)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare decimal with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(double value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 0 || type == 1)
            {
                double[] arrayc = getArray_as_double();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare double or double with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(float value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 2 || type == 3)
            {
                float[] arrayc = getArray_as_float();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare float or float with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(long value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 4 || type == 5)
            {
                long[] arrayc = getArray_as_long();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare long or long with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(int value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 6 || type == 7)
            {
                int[] arrayc = getArray_as_int();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare int or int with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(short value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 8 || type == 9)
            {
                short[] arrayc = getArray_as_short();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare short or short with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(byte value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 10 || type == 11)
            {
                byte[] arrayc = getArray_as_byte();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare byte or byte with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(char value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 16 || type == 17)
            {
                char[] arrayc = getArray_as_char();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i] == value)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare char or char with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(string value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 18)
            {
                string[] arrayc = getArray_as_string();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i].Equals(value))
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare string with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(ComplexClass value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 14)
            {
                ComplexClass[] arrayc = getArray_as_Complex();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i].Equals(value))
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare ComplexClass with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds all indices of the occurences of the element equal to a given value in an array
        // returns null if none found
        public int[] indicesOf(decimal value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int[] indices = null;
            int numberOfIndices = 0;
            if (type == 12)
            {
                decimal[] arrayc = getArray_as_decimal();
                List<int> arrayl = new List<int>();
                for (int i = 0; i < intLength; i++)
                {
                    if (arrayc[i].CompareTo(value) == 0)
                    {
                        numberOfIndices++;
                        arrayl.Add(i);
                    }
                }
                if (numberOfIndices != 0)
                {
                    indices = new int[numberOfIndices];
                    for (int i = 0; i < numberOfIndices; i++)
                    {
                        indices[i] = (arrayl[i]);
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare decimal with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return indices;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(double value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 0 || type == 1)
            {
                double[] arrayc = getArray_as_double();
                double diff = Math.Abs(arrayc[0] - value);
                double nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare double or double with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(float value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 2 || type == 3)
            {
                float[] arrayc = getArray_as_float();
                float diff = Math.Abs(arrayc[0] - value);
                float nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare float or float with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(long value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 4 || type == 5)
            {
                long[] arrayc = getArray_as_long();
                long diff = Math.Abs(arrayc[0] - value);
                long nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare long or long with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(int value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 6 || type == 7)
            {
                int[] arrayc = getArray_as_int();
                int diff = Math.Abs(arrayc[0] - value);
                int nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare int or int with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }


        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(short value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 8 || type == 9)
            {
                short[] arrayc = getArray_as_short();
                short diff = (short) Math.Abs(arrayc[0] - value);
                short nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = (short) Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare short or short with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(byte value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 10 || type == 11)
            {
                byte[] arrayc = getArray_as_byte();
                byte diff = (byte) Math.Abs(arrayc[0] - value);
                byte nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = (byte) Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare byte or byte with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(char value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 16 || type == 17)
            {
                int[] arrayc = getArray_as_int();
                int diff = Math.Abs(arrayc[0] - value);
                int nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - value) < diff)
                    {
                        diff = Math.Abs(arrayc[i] - value);
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare char or char with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the index of the first occurence of the nearest element to a given value in an array
        public int nearestIndex(decimal value)
        {
            if (blnSuppressMessages)
            {
                Converter.suppressMessages();
            }
            int index = 0;
            if (type == 12)
            {
                decimal[] arrayc = getArray_as_decimal();
                decimal diff = Math.Abs(arrayc[0] - (value));
                decimal nearest = arrayc[0];
                for (int i = 1; i < arrayc.Length; i++)
                {
                    if (Math.Abs(arrayc[i] - (value)).CompareTo(diff) == -1)
                    {
                        diff = Math.Abs(arrayc[i] - (value));
                        index = i;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Only comparisons between the same data types are supported - you are attempting to compare decimal with " +
                    typeName[type]);
            }
            Converter.restoreMessages();
            return index;
        }

        // finds the value of the nearest element to a given value in an array
        public double nearestValue(double value)
        {
            int index = nearestIndex(value);
            double ret = ((double) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public float nearestValue(float value)
        {
            int index = nearestIndex(value);
            float ret = ((float) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public long nearestValue(long value)
        {
            int index = nearestIndex(value);
            long ret = ((long) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public int nearestValue(int value)
        {
            int index = nearestIndex(value);
            int ret = ((int) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public short nearestValue(short value)
        {
            int index = nearestIndex(value);
            short ret = ((short) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public byte nearestValue(byte value)
        {
            int index = nearestIndex(value);
            byte ret = ((byte) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public char nearestValue(char value)
        {
            int index = nearestIndex(value);
            char ret = ((char) (m_array[index]));
            return ret;
        }

        // finds the value of the nearest element to a given value in an array
        public decimal nearestValue(decimal value)
        {
            int index = nearestIndex(value);
            decimal ret = (decimal) m_array[index];
            return ret;
        }

        // return maximum difference, i.e. range
        public double maximumDifference()
        {
            return getMaximumDifference_as_double();
        }

        public double maximumDifference_as_double()
        {
            return getMaximumDifference_as_double();
        }

        public double getMaximumDifference()
        {
            return getMaximumDifference_as_double();
        }

        public double getMaximumDifference_as_double()
        {
            double diff = 0.0D;
            if (type == 0 || type == 1)
            {
                double max = getMaximum_as_double();
                double min = getMinimum_as_double();
                diff = max - min;
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as double or double the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public float maximumDifference_as_float()
        {
            return getMaximumDifference_as_float();
        }

        public float getMaximumDifference_as_float()
        {
            float diff = 0.0F;
            if (type == 2 || type == 3)
            {
                float max = getMaximum_as_float();
                float min = getMinimum_as_float();
                diff = max - min;
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as float or float the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public long maximumDifference_as_long()
        {
            return getMaximumDifference_as_long();
        }

        public long getMaximumDifference_as_long()
        {
            long diff = 0L;
            if (type == 4 || type == 5)
            {
                long max = getMaximum_as_long();
                long min = getMinimum_as_long();
                diff = max - min;
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as long or long the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public int maximumDifference_as_int()
        {
            return getMaximumDifference_as_int();
        }

        public int getMaximumDifference_as_int()
        {
            int diff = 0;
            if (type == 6 || type == 7)
            {
                int max = getMaximum_as_int();
                int min = getMinimum_as_int();
                diff = max - min;
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as int or int the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public short maximumDifference_as_short()
        {
            return getMaximumDifference_as_short();
        }

        public short getMaximumDifference_as_short()
        {
            short diff = 0;
            if (type == 8 || type == 9)
            {
                short max = getMaximum_as_short();
                short min = getMinimum_as_short();
                diff = (short) (max - min);
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as short or short the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public byte maximumDifference_as_byte()
        {
            return getMaximumDifference_as_byte();
        }

        public byte getMaximumDifference_as_byte()
        {
            byte diff = 0;
            if (type == 10 || type == 11)
            {
                byte max = getMaximum_as_byte();
                byte min = getMinimum_as_byte();
                diff = (byte) (max - min);
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as byte or byte the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return maximum difference, i.e. range
        public decimal maximumDifference_as_decimal()
        {
            return getMaximumDifference_as_decimal();
        }

        public decimal getMaximumDifference_as_decimal()
        {
            decimal diff = decimal.Zero;
            if (type == 12)
            {
                decimal max = getMaximum_as_decimal();
                decimal min = getMinimum_as_decimal();
                diff = max - (min);
            }
            else
            {
                throw new ArgumentException(
                    "Maximum difference may only be returned as the same type as the type of the internal array - you are trying to return as decimal the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public double minimumDifference()
        {
            return getMinimumDifference_as_double();
        }

        public double minimumDifference_as_double()
        {
            return getMinimumDifference_as_double();
        }

        public double getMinimumDifference()
        {
            return getMinimumDifference_as_double();
        }

        public double getMinimumDifference_as_double()
        {
            double diff = 0.0D;
            if (type == 0 || type == 1)
            {
                ArrayMaths am = sort();
                double[] sorted = am.getArray_as_double();
                diff = sorted[1] - sorted[0];
                double minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = sorted[i + 1] - sorted[i];
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as double or double the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public float minimumDifference_as_float()
        {
            return getMinimumDifference_as_float();
        }

        public float getMinimumDifference_as_float()
        {
            float diff = 0.0F;
            if (type == 2 || type == 3)
            {
                ArrayMaths am = sort();
                float[] sorted = am.getArray_as_float();
                diff = sorted[1] - sorted[0];
                float minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = sorted[i + 1] - sorted[i];
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as float or float the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public long minimumDifference_as_long()
        {
            return getMinimumDifference_as_long();
        }

        public long getMinimumDifference_as_long()
        {
            long diff = 0L;
            if (type == 4 || type == 5)
            {
                ArrayMaths am = sort();
                long[] sorted = am.getArray_as_long();
                diff = sorted[1] - sorted[0];
                long minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = sorted[i + 1] - sorted[i];
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as long or long the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public int minimumDifference_as_int()
        {
            return getMinimumDifference_as_int();
        }

        public int getMinimumDifference_as_int()
        {
            int diff = 0;
            if (type == 6 || type == 7)
            {
                ArrayMaths am = sort();
                int[] sorted = am.getArray_as_int();
                diff = sorted[1] - sorted[0];
                int minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = sorted[i + 1] - sorted[i];
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as int or int the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public short minimumDifference_as_short()
        {
            return getMinimumDifference_as_short();
        }

        public short getMinimumDifference_as_short()
        {
            short diff = 0;
            if (type == 8 || type == 9)
            {
                ArrayMaths am = sort();
                short[] sorted = am.getArray_as_short();
                diff = (short) (sorted[1] - sorted[0]);
                short minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = (short) (sorted[i + 1] - sorted[i]);
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as short or short the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public byte minimumDifference_as_byte()
        {
            return getMinimumDifference_as_byte();
        }

        public byte getMinimumDifference_as_byte()
        {
            byte diff = 0;
            if (type == 10 || type == 11)
            {
                ArrayMaths am = sort();
                byte[] sorted = am.getArray_as_byte();
                diff = (byte) (sorted[1] - sorted[0]);
                byte minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = (byte) (sorted[i + 1] - sorted[i]);
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as byte or byte the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // return minimum difference
        public decimal minimumDifference_as_decimal()
        {
            return getMinimumDifference_as_decimal();
        }

        public decimal getMinimumDifference_as_decimal()
        {
            decimal diff = decimal.Zero;
            if (type == 12)
            {
                ArrayMaths am = sort();
                decimal[] sorted = am.getArray_as_decimal();
                diff = sorted[1] - (sorted[0]);
                decimal minDiff = diff;
                for (int i = 1; i < intLength - 1; i++)
                {
                    diff = (sorted[i + 1] - (sorted[i]));
                    if (diff.CompareTo(minDiff) == -1)
                    {
                        minDiff = diff;
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Minimum difference may only be returned as the same type as the type of the internal array - you are trying to return as decimal the difference for a " +
                    typeName[type] + "[] array");
            }
            return diff;
        }

        // Print array to screen with no line returns
        public void print()
        {
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    PrintToScreen.print(dd);
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    PrintToScreen.print(ff);
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    PrintToScreen.print(ll);
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    PrintToScreen.print(ii);
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    PrintToScreen.print(ss);
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    PrintToScreen.print(bb);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    PrintToScreen.print(bd);
                    bd = null;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    PrintToScreen.print(bi);
                    bi = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    PrintToScreen.print(cc);
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ct = getArray_as_char();
                    PrintToScreen.print(ct);
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    PrintToScreen.print(st);
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
        }

        // Print array to screen with  line returns
        public void println()
        {
            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = getArray_as_double();
                    PrintToScreen.println(dd);
                    break;
                case 2:
                case 3:
                    float[] ff = getArray_as_float();
                    PrintToScreen.println(ff);
                    break;
                case 4:
                case 5:
                    long[] ll = getArray_as_long();
                    PrintToScreen.println(ll);
                    break;
                case 6:
                case 7:
                    int[] ii = getArray_as_int();
                    PrintToScreen.println(ii);
                    break;
                case 8:
                case 9:
                    short[] ss = getArray_as_short();
                    PrintToScreen.println(ss);
                    break;
                case 10:
                case 11:
                    byte[] bb = getArray_as_byte();
                    PrintToScreen.println(bb);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    PrintToScreen.println(bd);
                    bd = null;
                    break;
                case 13:
                    long[] bi = getArray_as_long();
                    PrintToScreen.println(bi);
                    bi = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    PrintToScreen.println(cc);
                    break;
                case 15:
                case 16:
                case 17:
                    char[] ct = getArray_as_char();
                    PrintToScreen.println(ct);
                    break;
                case 18:
                    string[] st = getArray_as_string();
                    PrintToScreen.println(st);
                    break;
                default:
                    throw new ArgumentException("Data type not identified by this method");
            }
        }

        // Convert to decimal if long
        public void convertToHighest()
        {
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(dd[i]);
                    }
                    type = 1;
                    break;
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(bd[i]);
                    }
                    type = 12;
                    bd = null;
                    break;
                case 14:
                case 15:
                    ComplexClass[] cc = getArray_as_Complex();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(cc[i]);
                    }
                    type = 14;
                    break;
            }
        }

        // Returns an instance of Stat in which all the instance variable common to the sub-class Stat and the base-class ArrayMaths
        // are set to the values of this instance of ArrayMaths after conversion:
        //  convert array to double if not ComplexClass, ,decimal or long
        //  convert to decimal if long
        public Stat toStat()
        {
            convertToHighest();
            return statCopy();
        }

        // plot the array
        public void plot(int n)
        {
            //if (n > 2) throw new ArgumentException("Argument n, " + n + ", must be less than 3");

            //double[] xAxis = new double[intLength];
            //for (int i = 0; i < intLength; i++) xAxis[i] = i;
            //double[] yAxis = getArray_as_double();


            //PlotGraph pg = new PlotGraph(xAxis, yAxis);
            //pg.setGraphTitle("ArrayMaths plot method");
            //pg.setXaxisLegend("Array element index");
            //pg.setYaxisLegend("Array element value");
            //pg.setPoint(1);
            //switch (n)
            //{
            //    case 0: pg.setLine(0);
            //        pg.setGraphTitle2("Points only - no line");
            //        break;
            //    case 1: pg.setLine(3);
            //        pg.setGraphTitle2("Points joined by straight lines");
            //        break;
            //    case 2: pg.setLine(1);
            //        pg.setGraphTitle2("Points joined by cubic spline interpolated line");
            //        break;
            //    default: throw new ArgumentException("Should not be possible to Get here!!!");
            //}
            //pg.plot();
        }
    }
}
