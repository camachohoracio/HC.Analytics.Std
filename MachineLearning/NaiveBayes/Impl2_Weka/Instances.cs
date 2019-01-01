using System;
using System.IO;
using System.Linq;
using System.Text;
using HC.Core.Helpers;

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
 *    Instances.java
 *    Copyright (C) 1999 Eibe Frank
 *
 */

//package weka.core;

//import java.io.*;
//import java.text.ParseException;
//import java.util.*;

/**
 * Class for handling an ordered set of weighted instances. <p>
 *
 * Typical usage (code from the main() method of this class): <p>
 *
 * <code>
 * ... <br>
 * 
 * // Read all the instances in the file <br>
 * reader = new FileReader(filename); <br>
 * instances = new Instances(reader); <br><br>
 *
 * // Make the last attribute be the class <br>
 * instances.setClassIndex(instances.numAttributes() - 1); <br><br>
 * 
 * // Print header and instances. <br>
 * PrintToScreen.WriteLine("\nDataset:\n"); <br> 
 * PrintToScreen.WriteLine(instances); <br><br>
 *
 * ... <br>
 * </code><p>
 *
 * All methods that change a set of instances are safe, ie. a change
 * of a set of instances does not affect any other sets of
 * instances. All methods that change a datasets's attribute
 * information Clone the dataset before it is changed.
 *
 * @author Eibe Frank (eibe@cs.waikato.ac.nz)
 * @author Len Trigg (trigg@cs.waikato.ac.nz)
 * @version $Revision: 1.58.2.6 $ 
 */

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl2_Weka
{

    public class Instances
    {

        /** The filename extension that should be used for arff files */
        public static string FILE_EXTENSION = ".arff";

        /** The filename extension that should be used for bin. serialized instances files */
        public static string SERIALIZED_OBJ_FILE_EXTENSION = ".bsi";

        /** The keyword used to denote the start of an arff header */
        static string ARFF_RELATION = "@relation";

        /** The keyword used to denote the start of the arff data section */
        static string ARFF_DATA = "@data";

        /** The dataset's name. */
        protected /*@spec_public non_null@*/ string m_RelationName;

        /** The attribute information. */
        protected /*@spec_public non_null@*/ FastVector m_Attributes;
        /*  public invariant (\forall int i; 0 <= i && i < m_Attributes.size(); 
                          m_Attributes.elementAt(i) != null);
        */

        /** The instances. */
        protected /*@spec_public non_null@*/ FastVector m_Instances;

        /** The class attribute's index */
        protected int m_ClassIndex;
        //@ protected invariant classIndex() == m_ClassIndex;

        /** Buffer of values for sparse instance */
        protected double[] m_ValueBuffer;

        /** Buffer of indices for sparse instance */
        protected int[] m_IndicesBuffer;

        /**
         * Reads an ARFF file from a reader, and assigns a weight of
         * one to each instance. Lets the index of the class 
         * attribute be undefined (negative).
         *
         * @param reader the reader
         * @exception IOException if the ARFF file is not read 
         * successfully
         */
        public Instances(/*@non_null@*/
            StreamReader reader)
        {

            //StreamTokenizer tokenizer;

            //tokenizer = new StreamTokenizer(reader);
            //initTokenizer(tokenizer);
            //readHeader(tokenizer);
            //m_ClassIndex = -1;
            //m_Instances = new FastVector(1000);
            //while (getInstance(tokenizer, true)) { };
            //compactify();
        }

        /**
         * Reads the header of an ARFF file from a reader and 
         * reserves space for the given number of instances. Lets
         * the class index be undefined (negative).
         *
         * @param reader the reader
         * @param capacity the capacity
         * @exception Exception if the header is not read successfully
         * or the capacity is negative.
         * @exception IOException if there is a problem with the reader.
         */
        //@ requires capacity >= 0;
        //@ ensures classIndex() == -1;
        public Instances(/*@non_null@*/StreamReader reader, int capacity)
        {

            //StreamTokenizer tokenizer;

            if (capacity < 0)
            {
                throw new Exception("Capacity has to be positive!");
            }
            //tokenizer = new StreamTokenizer(reader);
            //initTokenizer(tokenizer);
            //readHeader(tokenizer);
            m_ClassIndex = -1;
            m_Instances = new FastVector(capacity);
        }

        /**
         * Constructor copying all instances and references to
         * the header information from the given set of instances.
         *
         * @param instances the set to be copied
         */
        public Instances(/*@non_null@*/Instances dataset) :
            this(dataset, dataset.numInstances())
        {

            dataset.copyInstances(0, this, dataset.numInstances());
        }

        /**
         * Constructor creating an empty set of instances. Copies references
         * to the header information from the given set of instances. Sets
         * the capacity of the set of instances to 0 if its negative.
         *
         * @param instances the instances from which the header 
         * information is to be taken
         * @param capacity the capacity of the new dataset 
         */
        public Instances(/*@non_null@*/Instances dataset, int capacity)
        {

            if (capacity < 0)
            {
                capacity = 0;
            }

            // Strings only have to be "shallow" copied because
            // they can't be modified.
            m_ClassIndex = dataset.m_ClassIndex;
            m_RelationName = dataset.m_RelationName;
            m_Attributes = dataset.m_Attributes;
            m_Instances = new FastVector(capacity);
        }

        /**
         * Creates a new set of instances by copying a 
         * subset of another set.
         *
         * @param source the set of instances from which a subset 
         * is to be created
         * @param first the index of the first instance to be copied
         * @param toCopy the number of instances to be copied
         * @exception Exception if first and toCopy are out of range
         */
        //@ requires 0 <= first;
        //@ requires 0 <= toCopy;
        //@ requires first + toCopy <= source.numInstances();
        public Instances(/*@non_null@*/Instances source, int first, int toCopy) :
            this(source, toCopy)
        {

            if ((first < 0) || ((first + toCopy) > source.numInstances()))
            {
                throw new Exception("Parameters first and/or toCopy out " +
                                                   "of range");
            }
            source.copyInstances(first, this, toCopy);
        }

        /**
         * Creates an empty set of instances. Uses the given
         * attribute information. Sets the capacity of the set of 
         * instances to 0 if its negative. Given attribute information
         * must not be changed after this constructor has been used.
         *
         * @param name the name of the relation
         * @param attInfo the attribute information
         * @param capacity the capacity of the set
         */
        public Instances(/*@non_null@*/string name,
            /*@non_null@*/FastVector attInfo, int capacity)
        {

            m_RelationName = name;
            m_ClassIndex = -1;
            m_Attributes = attInfo;
            for (int i = 0; i < numAttributes(); i++)
            {
                attribute(i).setIndex(i);
            }
            m_Instances = new FastVector(capacity);
        }

        /**
         * Create a copy of the structure, but "cleanse" string types (i.e.
         * doesn't contain references to the strings seen in the past).
         *
         * @return a copy of the instance structure.
         */
        public Instances stringFreeStructure()
        {

            FastVector atts = (FastVector)m_Attributes.copy();
            for (int i = 0; i < atts.size(); i++)
            {
                Attribute att = (Attribute)atts.elementAt(i);
                if (att.type() == Attribute.STRING)
                {
                    atts.setElementAt(new Attribute(att.name(), (FastVector)null), i);
                }
            }
            Instances result = new Instances(relationName(), atts, 0);
            result.m_ClassIndex = m_ClassIndex;
            return result;
        }

        /**
         * Adds one instance to the end of the set. 
         * Shallow copies instance before it is added. Increases the
         * size of the dataset if it is not large enough. Does not
         * check if the instance is compatible with the dataset.
         * Note: string values are not transferred.
         *
         * @param instance the instance to be added
         */
        public void add(/*@non_null@*/ Instance instance)
        {

            Instance newInstance = (Instance)instance.copy();

            newInstance.setDataset(this);
            m_Instances.Add(newInstance);
        }

        /**
         * Returns an attribute.
         *
         * @param index the attribute's index
         * @return the attribute at the given position
         */
        //@ requires 0 <= index;
        //@ requires index < m_Attributes.size();
        //@ ensures \result != null;
        public /*@pure@*/ Attribute attribute(int index)
        {

            return (Attribute)m_Attributes.elementAt(index);
        }

        /**
         * Returns an attribute given its name. If there is more than
         * one attribute with the same name, it returns the first one.
         * Returns null if the attribute can't be found.
         *
         * @param name the attribute's name
         * @return the attribute with the given name, null if the
         * attribute can't be found
         */
        public /*@pure@*/ Attribute attribute(string name)
        {

            for (int i = 0; i < numAttributes(); i++)
            {
                if (attribute(i).name().Equals(name))
                {
                    return attribute(i);
                }
            }
            return null;
        }

        /**
         * Checks for string attributes in the dataset
         *
         * @return true if string attributes are present, false otherwise
         */
        public /*@pure@*/ bool checkForStringAttributes()
        {

            int i = 0;

            while (i < m_Attributes.size())
            {
                if (attribute(i++).isString())
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Checks if the given instance is compatible
         * with this dataset. Only looks at the size of
         * the instance and the ranges of the values for 
         * nominal and string attributes.
         *
         * @return true if the instance is compatible with the dataset 
         */
        public /*@pure@*/ bool checkInstance(Instance instance)
        {

            if (instance.numAttributes() != numAttributes())
            {
                return false;
            }
            for (int i = 0; i < numAttributes(); i++)
            {
                if (instance.isMissing(i))
                {
                    continue;
                }
                else if (attribute(i).isNominal() ||
                   attribute(i).isString())
                {
                    //if (!(Utils.eq(instance.value(i),
                    //           (double)(int)instance.value(i))))
                    //{
                    //    return false;
                    //}
                    //else if (Utils.sm(instance.value(i), 0) ||
                    //       Utils.gr(instance.value(i),
                    //            attribute(i).numValues()))
                    //{
                    //    return false;
                    //}
                }
            }
            return true;
        }

        /**
         * Returns the class attribute.
         *
         * @return the class attribute
         * @exception Exception if the class is not set
         */
        //@ requires classIndex() >= 0;
        public /*@pure@*/ Attribute classAttribute()
        {

            if (m_ClassIndex < 0)
            {
                throw new Exception("Class index is negative (not set)!");
            }
            return attribute(m_ClassIndex);
        }

        /**
         * Returns the class attribute's index. Returns negative number
         * if it's undefined.
         *
         * @return the class index as an integer
         */
        // ensures \result == m_ClassIndex;
        public /*@pure@*/ int classIndex()
        {

            return m_ClassIndex;
        }

        /**
         * Compactifies the set of instances. Decreases the capacity of
         * the set so that it matches the number of instances in the set.
         */
        public void compactify()
        {

            m_Instances.trimToSize();
        }

        /**
         * Removes all instances from the set.
         */
        public void delete()
        {

            m_Instances = new FastVector();
        }

        /**
         * Removes an instance at the given position from the set.
         *
         * @param index the instance's position
         */
        //@ requires 0 <= index && index < numInstances();
        public void delete(int index)
        {

            m_Instances.removeElementAt(index);
        }

        /**
         * Deletes an attribute at the given position 
         * (0 to numAttributes() - 1). A deep copy of the attribute
         * information is performed before the attribute is deleted.
         *
         * @param pos the attribute's position
         * @exception Exception if the given index is out of range 
         *            or the class attribute is being deleted
         */
        //@ requires 0 <= position && position < numAttributes();
        //@ requires position != classIndex();
        public void deleteAttributeAt(int position)
        {

            if ((position < 0) || (position >= m_Attributes.size()))
            {
                throw new Exception("Index out of range");
            }
            if (position == m_ClassIndex)
            {
                throw new Exception("Can't delete class attribute");
            }
            freshAttributeInfo();
            if (m_ClassIndex > position)
            {
                m_ClassIndex--;
            }
            m_Attributes.removeElementAt(position);
            for (int i = position; i < m_Attributes.size(); i++)
            {
                Attribute current = (Attribute)m_Attributes.elementAt(i);
                current.setIndex(current.index() - 1);
            }
            for (int i = 0; i < numInstances(); i++)
            {
                //instance(i).forceDeleteAttributeAt(position);
            }
        }

        /**
         * Deletes all string attributes in the dataset. A deep copy of the attribute
         * information is performed before an attribute is deleted.
         *
         * @exception Exception if string attribute couldn't be 
         * successfully deleted (probably because it is the class attribute).
         */
        public void deleteStringAttributes()
        {

            int i = 0;
            while (i < m_Attributes.size())
            {
                if (attribute(i).isString())
                {
                    deleteAttributeAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        /**
         * Removes all instances with missing values for a particular
         * attribute from the dataset.
         *
         * @param attIndex the attribute's index
         */
        //@ requires 0 <= attIndex && attIndex < numAttributes();
        public void deleteWithMissing(int attIndex)
        {

            FastVector newInstances = new FastVector(numInstances());

            for (int i = 0; i < numInstances(); i++)
            {
                if (!instance(i).isMissing(attIndex))
                {
                    newInstances.Add(instance(i));
                }
            }
            m_Instances = newInstances;
        }

        /**
         * Removes all instances with missing values for a particular
         * attribute from the dataset.
         *
         * @param att the attribute
         */
        public void deleteWithMissing(/*@non_null@*/ Attribute att)
        {

            deleteWithMissing(att.index());
        }

        /**
         * Removes all instances with a missing class value
         * from the dataset.
         *
         * @exception Exception if class is not set
         */
        public void deleteWithMissingClass()
        {

            if (m_ClassIndex < 0)
            {
                throw new Exception("Class index is negative (not set)!");
            }
            deleteWithMissing(m_ClassIndex);
        }

        /**
         * Returns an enumeration of all the attributes.
         *
         * @return enumeration of all the attributes.
         */
        public /*@non_null pure@*/ FastVector.FastVectorEnumeration enumerateAttributes()
        {

            return m_Attributes.elements(m_ClassIndex);
        }

        /**
         * Returns an enumeration of all instances in the dataset.
         *
         * @return enumeration of all instances in the dataset
         */
        public /*@non_null pure@*/ FastVector.FastVectorEnumeration enumerateInstances()
        {

            return m_Instances.elements();
        }

        /**
         * Checks if two headers are equivalent.
         *
         * @param dataset another dataset
         * @return true if the header of the given dataset is equivalent 
         * to this header
         */
        public /*@pure@*/ bool equalHeaders(Instances dataset)
        {

            // Check class and all attributes
            if (m_ClassIndex != dataset.m_ClassIndex)
            {
                return false;
            }
            if (m_Attributes.size() != dataset.m_Attributes.size())
            {
                return false;
            }
            for (int i = 0; i < m_Attributes.size(); i++)
            {
                if (!(attribute(i).Equals(dataset.attribute(i))))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns the first instance in the set.
         *
         * @return the first instance in the set
         */
        //@ requires numInstances() > 0;
        public /*@non_null pure@*/ Instance firstInstance()
        {

            return (Instance)m_Instances.firstElement();
        }

        /**
         * Returns a random number generator. The initial seed of the random
         * number generator depends on the given seed and the hash code of
         * a string representation of a instances chosen based on the given
         * seed. 
         *
         * @param seed the given seed
         * @return the random number generator
         */
        public Random getRandomNumberGenerator(long seed)
        {

            Random r = new Random((int)seed);
            //r.setSeed(instance(r.nextInt(numInstances())).ToString().hashCode() + seed);
            return r;
        }

        /**
         * Inserts an attribute at the given position (0 to 
         * numAttributes()) and sets all values to be missing.
         * Shallow copies the attribute before it is inserted, and performs
         * a deep copy of the existing attribute information.
         *
         * @param att the attribute to be inserted
         * @param pos the attribute's position
         * @exception Exception if the given index is out of range
         */
        //@ requires 0 <= position;
        //@ requires position <= numAttributes();
        public void insertAttributeAt(/*@non_null@*/ Attribute att, int position)
        {

            if ((position < 0) ||
            (position > m_Attributes.size()))
            {
                throw new Exception("Index out of range");
            }
            att = (Attribute)att.copy();
            freshAttributeInfo();
            att.setIndex(position);
            m_Attributes.insertElementAt(att, position);
            for (int i = position + 1; i < m_Attributes.size(); i++)
            {
                Attribute current = (Attribute)m_Attributes.elementAt(i);
                current.setIndex(current.index() + 1);
            }
            for (int i = 0; i < numInstances(); i++)
            {
                //instance(i).forceInsertAttributeAt(position);
            }
            if (m_ClassIndex >= position)
            {
                m_ClassIndex++;
            }
        }

        /**
         * Returns the instance at the given position.
         *
         * @param index the instance's index
         * @return the instance at the given position
         */
        //@ requires 0 <= index;
        //@ requires index < numInstances();
        public /*@non_null pure@*/ Instance instance(int index)
        {

            return (Instance)m_Instances.elementAt(index);
        }

        /**
         * Returns the kth-smallest attribute value of a numeric attribute.
         * Note that calling this method will change the order of the data!
         *
         * @param att the Attribute object
         * @param k the value of k
         * @return the kth-smallest value
         */
        public double kthSmallestValue(Attribute att, int k)
        {

            return kthSmallestValue(att.index(), k);
        }

        /**
         * Returns the kth-smallest attribute value of a numeric attribute.
         * Note that calling this method will change the order of the data!
         * The number of non-missing values in the data must be as least
         * as last as k for this to work.
         *
         * @param attIndex the attribute's index
         * @param k the value of k
         * @return the kth-smallest value
         */
        public double kthSmallestValue(int attIndex, int k)
        {

            if (!attribute(attIndex).isNumeric())
            {
                throw new Exception("Instances: attribute must be numeric to compute kth-smallest value.");
            }

            int i, j;

            // move all instances with missing values to end
            j = numInstances() - 1;
            i = 0;
            while (i <= j)
            {
                if (instance(j).isMissing(attIndex))
                {
                    j--;
                }
                else
                {
                    if (instance(i).isMissing(attIndex))
                    {
                        swap(i, j);
                        j--;
                    }
                    i++;
                }
            }

            if ((k < 1) || (k > j + 1))
            {
                throw new Exception("Instances: value for k for computing kth-smallest value too large.");
            }

            return instance(select(attIndex, 0, j, k)).value(attIndex);
        }

        /**
         * Returns the last instance in the set.
         *
         * @return the last instance in the set
         */
        //@ requires numInstances() > 0;
        public /*@non_null pure@*/ Instance lastInstance()
        {

            return (Instance)m_Instances.lastElement();
        }

        /**
         * Returns the mean (mode) for a numeric (nominal) attribute as
         * a floating-point value. Returns 0 if the attribute is neither nominal nor 
         * numeric. If all values are missing it returns zero.
         *
         * @param attIndex the attribute's index
         * @return the mean or the mode
         */
        public /*@pure@*/ double meanOrMode(int attIndex)
        {

            double result, found;
            int[] counts;

            if (attribute(attIndex).isNumeric())
            {
                result = found = 0;
                for (int j = 0; j < numInstances(); j++)
                {
                    if (!instance(j).isMissing(attIndex))
                    {
                        found += instance(j).weight();
                        result += instance(j).weight() * instance(j).value(attIndex);
                    }
                }
                if (found <= 0)
                {
                    return 0;
                }
                else
                {
                    return result / found;
                }
            }
            else if (attribute(attIndex).isNominal())
            {
                counts = new int[attribute(attIndex).numValues()];
                for (int j = 0; j < numInstances(); j++)
                {
                    if (!instance(j).isMissing(attIndex))
                    {
                        //counts[(int)instance(j).value(attIndex)] += instance(j).weight();
                    }
                }
                //return (double)Utils.maxIndex(counts);
            }
            else
            {
                return 0;
            }
            return 0;
        }

        /**
         * Returns the mean (mode) for a numeric (nominal) attribute as a
         * floating-point value.  Returns 0 if the attribute is neither
         * nominal nor numeric.  If all values are missing it returns zero.
         *
         * @param att the attribute
         * @return the mean or the mode 
         */
        public /*@pure@*/ double meanOrMode(Attribute att)
        {

            return meanOrMode(att.index());
        }

        /**
         * Returns the number of attributes.
         *
         * @return the number of attributes as an integer
         */
        //@ ensures \result == m_Attributes.size();
        public /*@pure@*/ int numAttributes()
        {

            return m_Attributes.size();
        }

        /**
         * Returns the number of class labels.
         *
         * @return the number of class labels as an integer if the class 
         * attribute is nominal, 1 otherwise.
         * @exception Exception if the class is not set
         */
        //@ requires classIndex() >= 0;
        public /*@pure@*/ int numClasses()
        {

            if (m_ClassIndex < 0)
            {
                throw new Exception("Class index is negative (not set)!");
            }
            if (!classAttribute().isNominal())
            {
                return 1;
            }
            else
            {
                return classAttribute().numValues();
            }
        }

        /**
         * Returns the number of distinct values of a given attribute.
         * Returns the number of instances if the attribute is a
         * string attribute. The value 'missing' is not counted.
         *
         * @param attIndex the attribute
         * @return the number of distinct values of a given attribute
         */
        //@ requires 0 <= attIndex;
        //@ requires attIndex < numAttributes();
        public /*@pure@*/ int numDistinctValues(int attIndex)
        {

            if (attribute(attIndex).isNumeric())
            {
                double[] attVals = attributeTodoubleArray(attIndex);
                int[] sorted = null; //= attVals;
                double prev = 0;
                int counter = 0;
                for (int i = 0; i < sorted.Length; i++)
                {
                    Instance current = instance(sorted[i]);
                    if (current.isMissing(attIndex))
                    {
                        break;
                    }
                    if ((i == 0) ||
                        (current.value(attIndex) > prev))
                    {
                        prev = current.value(attIndex);
                        counter++;
                    }
                }
                return counter;
            }
            else
            {
                return attribute(attIndex).numValues();
            }
        }

        /**
         * Returns the number of distinct values of a given attribute.
         * Returns the number of instances if the attribute is a
         * string attribute. The value 'missing' is not counted.
         *
         * @param att the attribute
         * @return the number of distinct values of a given attribute
         */
        public /*@pure@*/ int numDistinctValues(/*@non_null@*/Attribute att)
        {

            return numDistinctValues(att.index());
        }

        /**
         * Returns the number of instances in the dataset.
         *
         * @return the number of instances in the dataset as an integer
         */
        //@ ensures \result == m_Instances.size();
        public /*@pure@*/ int numInstances()
        {

            return m_Instances.size();
        }

        /**
         * Shuffles the instances in the set so that they are ordered 
         * randomly.
         *
         * @param random a random number generator
         */
        public void randomize(Random random)
        {

            for (int j = numInstances() - 1; j > 0; j--)
                swap(j, random.Next(j + 1));
        }

        /**
         * Reads a single instance from the reader and appends it
         * to the dataset.  Automatically expands the dataset if it
         * is not large enough to hold the instance. This method does
         * not check for carriage return at the end of the line.
         *
         * @param reader the reader 
         * @return false if end of file has been reached
         * @exception IOException if the information is not read 
         * successfully
         */
        public bool readInstance(StreamReader reader)
        {

            //StreamTokenizer tokenizer = new StreamTokenizer(reader);

            //initTokenizer(tokenizer);
            //return getInstance(tokenizer, false);
            return false;
        }

        /**
         * Returns the relation's name.
         *
         * @return the relation's name as a string
         */
        //@ ensures \result == m_RelationName;
        public /*@pure@*/ string relationName()
        {

            return m_RelationName;
        }

        /**
         * Renames an attribute. This change only affects this
         * dataset.
         *
         * @param att the attribute's index
         * @param name the new name
         */
        public void renameAttribute(int att, string name)
        {

            Attribute newAtt = null;//= attribute(att).copy(name);
            FastVector newVec = new FastVector(numAttributes());

            for (int i = 0; i < numAttributes(); i++)
            {
                if (i == att)
                {
                    newVec.Add(newAtt);
                }
                else
                {
                    newVec.Add(attribute(i));
                }
            }
            m_Attributes = newVec;
        }

        /**
         * Renames an attribute. This change only affects this
         * dataset.
         *
         * @param att the attribute
         * @param name the new name
         */
        public void renameAttribute(Attribute att, string name)
        {

            renameAttribute(att.index(), name);
        }

        /**
         * Renames the value of a nominal (or string) attribute value. This
         * change only affects this dataset.
         *
         * @param att the attribute's index
         * @param val the value's index
         * @param name the new name 
         */
        public void renameAttributeValue(int att, int val, string name)
        {

            Attribute newAtt = (Attribute)attribute(att).copy();
            FastVector newVec = new FastVector(numAttributes());

            newAtt.setValue(val, name);
            for (int i = 0; i < numAttributes(); i++)
            {
                if (i == att)
                {
                    newVec.Add(newAtt);
                }
                else
                {
                    newVec.Add(attribute(i));
                }
            }
            m_Attributes = newVec;
        }

        /**
         * Renames the value of a nominal (or string) attribute value. This
         * change only affects this dataset.
         *
         * @param att the attribute
         * @param val the value
         * @param name the new name
         */
        public void renameAttributeValue(Attribute att, string val,
                                               string name)
        {

            int v = att.indexOfValue(val);
            if (v == -1) throw new Exception(val + " not found");
            renameAttributeValue(att.index(), v, name);
        }

        /**
         * Creates a new dataset of the same size using random sampling
         * with replacement.
         *
         * @param random a random number generator
         * @return the new dataset
         */
        public Instances resample(Random random)
        {

            Instances newData = new Instances(this, numInstances());
            while (newData.numInstances() < numInstances())
            {
                newData.add(instance(random.Next(numInstances())));
            }
            return newData;
        }

        /**
         * Creates a new dataset of the same size using random sampling
         * with replacement according to the current instance weights. The
         * weights of the instances in the new dataset are set to one.
         *
         * @param random a random number generator
         * @return the new dataset
         */
        public Instances resampleWithWeights(Random random)
        {

            double[] weights = new double[numInstances()];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = instance(i).weight();
            }
            return resampleWithWeights(random, weights);
        }


        /**
         * Creates a new dataset of the same size using random sampling
         * with replacement according to the given weight vector. The
         * weights of the instances in the new dataset are set to one.
         * The Length of the weight vector has to be the same as the
         * number of instances in the dataset, and all weights have to
         * be positive.
         *
         * @param random a random number generator
         * @param weights the weight vector
         * @return the new dataset
         * @exception Exception if the weights array is of the wrong
         * Length or contains negative weights.
         */
        public Instances resampleWithWeights(Random random,
                               double[] weights)
        {

            if (weights.Length != numInstances())
            {
                throw new Exception("weights.Length != numInstances.");
            }
            Instances newData = new Instances(this, numInstances());
            if (numInstances() == 0)
            {
                return newData;
            }
            double[] probabilities = new double[numInstances()];
            double sumProbs = 0, sumOfWeights = weights.Sum();
            for (int i = 0; i < numInstances(); i++)
            {
                sumProbs += random.NextDouble();
                probabilities[i] = sumProbs;
            }
            Utils.normalize(probabilities, sumProbs / sumOfWeights);

            // Make sure that rounding errors don't mess things up
            probabilities[numInstances() - 1] = sumOfWeights;
            int k = 0; int l = 0;
            sumProbs = 0;
            while ((k < numInstances() && (l < numInstances())))
            {
                if (weights[l] < 0)
                {
                    throw new Exception("Weights have to be positive.");
                }
                sumProbs += weights[l];
                while ((k < numInstances()) &&
                   (probabilities[k] <= sumProbs))
                {
                    newData.add(instance(l));
                    newData.instance(k).setWeight(1);
                    k++;
                }
                l++;
            }
            return newData;
        }

        /** 
         * Sets the class attribute.
         *
         * @param att attribute to be the class
         */
        public void setClass(Attribute att)
        {

            m_ClassIndex = att.index();
        }

        /** 
         * Sets the class index of the set.
         * If the class index is negative there is assumed to be no class.
         * (ie. it is undefined)
         *
         * @param classIndex the new class index
         * @exception Exception if the class index is too big or < 0
         */
        public void setClassIndex(int classIndex)
        {

            if (classIndex >= numAttributes())
            {
                throw new Exception("Invalid class index: " + classIndex);
            }
            m_ClassIndex = classIndex;
        }

        /**
         * Sets the relation's name.
         *
         * @param newName the new relation name.
         */
        public void setRelationName(/*@non_null@*/string newName)
        {

            m_RelationName = newName;
        }

        /**
         * Sorts the instances based on an attribute. For numeric attributes, 
         * instances are sorted in ascending order. For nominal attributes, 
         * instances are sorted based on the attribute label ordering 
         * specified in the header. Instances with missing values for the 
         * attribute are placed at the end of the dataset.
         *
         * @param attIndex the attribute's index
         */
        public void sort(int attIndex)
        {

            int i, j;

            // move all instances with missing values to end
            j = numInstances() - 1;
            i = 0;
            while (i <= j)
            {
                if (instance(j).isMissing(attIndex))
                {
                    j--;
                }
                else
                {
                    if (instance(i).isMissing(attIndex))
                    {
                        swap(i, j);
                        j--;
                    }
                    i++;
                }
            }
            quickSort(attIndex, 0, j);
        }

        /**
         * Sorts the instances based on an attribute. For numeric attributes, 
         * instances are sorted into ascending order. For nominal attributes, 
         * instances are sorted based on the attribute label ordering 
         * specified in the header. Instances with missing values for the 
         * attribute are placed at the end of the dataset.
         *
         * @param att the attribute
         */
        public void sort(Attribute att)
        {

            sort(att.index());
        }

        /**
         * Stratifies a set of instances according to its class values 
         * if the class attribute is nominal (so that afterwards a 
         * stratified cross-validation can be performed).
         *
         * @param numFolds the number of folds in the cross-validation
         * @exception Exception if the class is not set
         */
        public void stratify(int numFolds)
        {

            if (numFolds <= 0)
            {
                throw new Exception("Number of folds must be greater than 1");
            }
            if (m_ClassIndex < 0)
            {
                throw new Exception("Class index is negative (not set)!");
            }
            if (classAttribute().isNominal())
            {

                // sort by class
                int index = 1;
                while (index < numInstances())
                {
                    Instance instance1 = instance(index - 1);
                    for (int j = index; j < numInstances(); j++)
                    {
                        Instance instance2 = instance(j);
                        if ((instance1.classValue() == instance2.classValue()) ||
                            (instance1.classIsMissing() &&
                             instance2.classIsMissing()))
                        {
                            swap(index, j);
                            index++;
                        }
                    }
                    index++;
                }
                stratStep(numFolds);
            }
        }

        /**
         * Computes the sum of all the instances' weights.
         *
         * @return the sum of all the instances' weights as a double
         */
        public /*@pure@*/ double sumOfWeights()
        {

            double sum = 0;

            for (int i = 0; i < numInstances(); i++)
            {
                sum += instance(i).weight();
            }
            return sum;
        }

        /**
         * Creates the test set for one fold of a cross-validation on 
         * the dataset.
         *
         * @param numFolds the number of folds in the cross-validation. Must
         * be greater than 1.
         * @param numFold 0 for the first fold, 1 for the second, ...
         * @return the test set as a set of weighted instances
         * @exception Exception if the number of folds is less than 2
         * or greater than the number of instances.
         */
        //@ requires 2 <= numFolds && numFolds < numInstances();
        //@ requires 0 <= numFold && numFold < numFolds;
        public Instances testCV(int numFolds, int numFold)
        {

            int numInstForFold, first, offset;
            Instances test;

            if (numFolds < 2)
            {
                throw new Exception("Number of folds must be at least 2!");
            }
            if (numFolds > numInstances())
            {
                throw new Exception("Can't have more folds than instances!");
            }
            numInstForFold = numInstances() / numFolds;
            if (numFold < numInstances() % numFolds)
            {
                numInstForFold++;
                offset = numFold;
            }
            else
                offset = numInstances() % numFolds;
            test = new Instances(this, numInstForFold);
            first = numFold * (numInstances() / numFolds) + offset;
            copyInstances(first, test, numInstForFold);
            return test;
        }

        /**
         * Returns the dataset as a string in ARFF format. Strings
         * are quoted if they contain whitespace characters, or if they
         * are a question mark.
         *
         * @return the dataset in ARFF format as a string
         */
        public string ToString()
        {

            StringBuilder text = new StringBuilder();

            text.Append(ARFF_RELATION).Append(" ").
              Append(m_RelationName).Append("\n\n");
            for (int i = 0; i < numAttributes(); i++)
            {
                text.Append(attribute(i)).Append(Environment.NewLine);
            }
            text.Append(Environment.NewLine).Append(ARFF_DATA).Append(Environment.NewLine);
            for (int i = 0; i < numInstances(); i++)
            {
                text.Append(instance(i));
                if (i < numInstances() - 1)
                {
                    text.Append('\n');
                }
            }
            return text.ToString();
        }

        /**
         * Creates the training set for one fold of a cross-validation 
         * on the dataset. 
         *
         * @param numFolds the number of folds in the cross-validation. Must
         * be greater than 1.
         * @param numFold 0 for the first fold, 1 for the second, ...
         * @return the training set 
         * @exception Exception if the number of folds is less than 2
         * or greater than the number of instances.
         */
        //@ requires 2 <= numFolds && numFolds < numInstances();
        //@ requires 0 <= numFold && numFold < numFolds;
        public Instances trainCV(int numFolds, int numFold)
        {

            int numInstForFold, first, offset;
            Instances train;

            if (numFolds < 2)
            {
                throw new Exception("Number of folds must be at least 2!");
            }
            if (numFolds > numInstances())
            {
                throw new Exception("Can't have more folds than instances!");
            }
            numInstForFold = numInstances() / numFolds;
            if (numFold < numInstances() % numFolds)
            {
                numInstForFold++;
                offset = numFold;
            }
            else
                offset = numInstances() % numFolds;
            train = new Instances(this, numInstances() - numInstForFold);
            first = numFold * (numInstances() / numFolds) + offset;
            copyInstances(0, train, first);
            copyInstances(first + numInstForFold, train,
                  numInstances() - first - numInstForFold);

            return train;
        }

        /**
         * Creates the training set for one fold of a cross-validation 
         * on the dataset. The data is subsequently randomized based
         * on the given random number generator.
         *
         * @param numFolds the number of folds in the cross-validation. Must
         * be greater than 1.
         * @param numFold 0 for the first fold, 1 for the second, ...
         * @param random the random number generator
         * @return the training set 
         * @exception Exception if the number of folds is less than 2
         * or greater than the number of instances.
         */
        //@ requires 2 <= numFolds && numFolds < numInstances();
        //@ requires 0 <= numFold && numFold < numFolds;
        public Instances trainCV(int numFolds, int numFold, Random random)
        {

            Instances train = trainCV(numFolds, numFold);
            train.randomize(random);
            return train;
        }

        /**
         * Computes the variance for a numeric attribute.
         *
         * @param attIndex the numeric attribute
         * @return the variance if the attribute is numeric
         * @exception Exception if the attribute is not numeric
         */
        public /*@pure@*/ double variance(int attIndex)
        {

            double sum = 0, sumSquared = 0, sumOfWeights = 0;

            if (!attribute(attIndex).isNumeric())
            {
                throw new Exception("Can't compute variance because attribute is " +
                        "not numeric!");
            }
            for (int i = 0; i < numInstances(); i++)
            {
                if (!instance(i).isMissing(attIndex))
                {
                    sum += instance(i).weight() *
                      instance(i).value(attIndex);
                    sumSquared += instance(i).weight() *
                      instance(i).value(attIndex) *
                      instance(i).value(attIndex);
                    sumOfWeights += instance(i).weight();
                }
            }
            if (sumOfWeights <= 1)
            {
                return 0;
            }
            double result = (sumSquared - (sum * sum / sumOfWeights)) /
              (sumOfWeights - 1);

            // We don't like negative variance
            if (result < 0)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        /**
         * Computes the variance for a numeric attribute.
         *
         * @param att the numeric attribute
         * @return the variance if the attribute is numeric
         * @exception Exception if the attribute is not numeric
         */
        public /*@pure@*/ double variance(Attribute att)
        {

            return variance(att.index());
        }

        /**
         * Calculates summary statistics on the values that appear in this
         * set of instances for a specified attribute.
         *
         * @param index the index of the attribute to summarize.
         * @return an AttributeStats object with it's fields calculated.
         */
        //@ requires 0 <= index && index < numAttributes();
        //public AttributeStats attributeStats(int index)
        //{

        //    AttributeStats result = new AttributeStats();
        //    if (attribute(index).isNominal())
        //    {
        //        result.nominalCounts = new int[attribute(index).numValues()];
        //    }
        //    if (attribute(index).isNumeric())
        //    {
        //        result.numericStats = new weka.experiment.Stats();
        //    }
        //    result.totalCount = numInstances();

        //    double[] attVals = attributeTodoubleArray(index);
        //    int[] sorted = Utils.sort(attVals);
        //    int currentCount = 0;
        //    double prev = Instance.missingValue();
        //    for (int j = 0; j < numInstances(); j++)
        //    {
        //        Instance current = instance(sorted[j]);
        //        if (current.isMissing(index))
        //        {
        //            result.missingCount = numInstances() - j;
        //            break;
        //        }
        //        if (current.value(index) == prev)
        //        {
        //            currentCount++;
        //        }
        //        else
        //        {
        //            result.addDistinct(prev, currentCount);
        //            currentCount = 1;
        //            prev = current.value(index);
        //        }
        //    }
        //    result.addDistinct(prev, currentCount);
        //    result.distinctCount--; // So we don't count "missing" as a value 
        //    return result;
        //}

        /**
         * Gets the value of all instances in this dataset for a particular
         * attribute. Useful in conjunction with Utils.sort to allow iterating
         * through the dataset in sorted order for some attribute.
         *
         * @param index the index of the attribute.
         * @return an array containing the value of the desired attribute for
         * each instance in the dataset. 
         */
        //@ requires 0 <= index && index < numAttributes();
        public /*@pure@*/ double[] attributeTodoubleArray(int index)
        {

            double[] result = new double[numInstances()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = instance(i).value(index);
            }
            return result;
        }

        /**
         * Generates a string summarizing the set of instances. Gives a breakdown
         * for each attribute indicating the number of missing/discrete/unique
         * values and other information.
         *
         * @return a string summarizing the dataset
         */
        public string toSummaryString()
        {
            return "";
        }

        /**
         * Reads a single instance using the tokenizer and appends it
         * to the dataset. Automatically expands the dataset if it
         * is not large enough to hold the instance.
         *
         * @param tokenizer the tokenizer to be used
         * @param flag if method should test for carriage return after 
         * each instance
         * @return false if end of file has been reached
         * @exception IOException if the information is not read 
         * successfully
         */
        //protected bool getInstance(StreamTokenizer tokenizer,
        //              bool flag)
        //{

        //    // Check if any attributes have been declared.
        //    if (m_Attributes.size() == 0)
        //    {
        //        errms(tokenizer, "no header information available");
        //    }

        //    // Check if end of file reached.
        //    getFirstToken(tokenizer);
        //    if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //    {
        //        return false;
        //    }

        //    // Parse instance
        //    if (tokenizer.ttype == '{')
        //    {
        //        return getInstanceSparse(tokenizer, flag);
        //    }
        //    else
        //    {
        //        return getInstanceFull(tokenizer, flag);
        //    }
        //}

        /**
         * Reads a single instance using the tokenizer and appends it
         * to the dataset. Automatically expands the dataset if it
         * is not large enough to hold the instance.
         *
         * @param tokenizer the tokenizer to be used
         * @param flag if method should test for carriage return after 
         * each instance
         * @return false if end of file has been reached
         * @exception IOException if the information is not read 
         * successfully
         */
        //protected bool getInstanceSparse(StreamTokenizer tokenizer,
        //                    bool flag)
        //{

        //    int valIndex, numValues = 0, maxIndex = -1;

        //    // Get values
        //    do
        //    {

        //        // Get index
        //        getIndex(tokenizer);
        //        if (tokenizer.ttype == '}')
        //        {
        //            break;
        //        }

        //        // Is index valid?
        //        try
        //        {
        //            m_IndicesBuffer[numValues] = int.valueOf(tokenizer.sval).intValue();
        //        }
        //        catch (NumberFormatException e)
        //        {
        //            errms(tokenizer, "index number expected");
        //        }
        //        if (m_IndicesBuffer[numValues] <= maxIndex)
        //        {
        //            errms(tokenizer, "indices have to be ordered");
        //        }
        //        if ((m_IndicesBuffer[numValues] < 0) ||
        //        (m_IndicesBuffer[numValues] >= numAttributes()))
        //        {
        //            errms(tokenizer, "index out of bounds");
        //        }
        //        maxIndex = m_IndicesBuffer[numValues];

        //        // Get value;
        //        getNextToken(tokenizer);

        //        // Check if value is missing.
        //        if (tokenizer.ttype == '?')
        //        {
        //            m_ValueBuffer[numValues] = Instance.missingValue();
        //        }
        //        else
        //        {

        //            // Check if token is valid.
        //            if (tokenizer.ttype != StreamTokenizer.TT_WORD)
        //            {
        //                errms(tokenizer, "not a valid value");
        //            }
        //            switch (attribute(m_IndicesBuffer[numValues]).type())
        //            {
        //                case Attribute.NOMINAL:
        //                    // Check if value appears in header.
        //                    valIndex =
        //                      attribute(m_IndicesBuffer[numValues]).indexOfValue(tokenizer.sval);
        //                    if (valIndex == -1)
        //                    {
        //                        errms(tokenizer, "nominal value not declared in header");
        //                    }
        //                    m_ValueBuffer[numValues] = (double)valIndex;
        //                    break;
        //                case Attribute.NUMERIC:
        //                    // Check if value is really a number.
        //                    try
        //                    {
        //                        m_ValueBuffer[numValues] = double.valueOf(tokenizer.sval).
        //                          doubleValue();
        //                    }
        //                    catch (NumberFormatException e)
        //                    {
        //                        errms(tokenizer, "number expected");
        //                    }
        //                    break;
        //                case Attribute.STRING:
        //                    m_ValueBuffer[numValues] =
        //                      attribute(m_IndicesBuffer[numValues]).addStringValue(tokenizer.sval);
        //                    break;
        //                case Attribute.DATE:
        //                    try
        //                    {
        //                        m_ValueBuffer[numValues] =
        //                          attribute(m_IndicesBuffer[numValues]).parseDate(tokenizer.sval);
        //                    }
        //                    catch (ParseException e)
        //                    {
        //                        errms(tokenizer, "unparseable date: " + tokenizer.sval);
        //                    }
        //                    break;
        //                default:
        //                    errms(tokenizer, "unknown attribute type in column " + m_IndicesBuffer[numValues]);
        //            }
        //        }
        //        numValues++;
        //    } while (true);
        //    if (flag)
        //    {
        //        getLastToken(tokenizer, true);
        //    }

        //    // Add instance to dataset
        //    double[] tempValues = new double[numValues];
        //    int[] tempIndices = new int[numValues];
        //    System.arraycopy(m_ValueBuffer, 0, tempValues, 0, numValues);
        //    System.arraycopy(m_IndicesBuffer, 0, tempIndices, 0, numValues);
        //    add(new SparseInstance(1, tempValues, tempIndices, numAttributes()));
        //    return true;
        //}

        /**
         * Reads a single instance using the tokenizer and appends it
         * to the dataset. Automatically expands the dataset if it
         * is not large enough to hold the instance.
         *
         * @param tokenizer the tokenizer to be used
         * @param flag if method should test for carriage return after 
         * each instance
         * @return false if end of file has been reached
         * @exception IOException if the information is not read 
         * successfully
         */
        //protected bool getInstanceFull(StreamTokenizer tokenizer,
        //                  bool flag)
        //{

        //    double[] instance = new double[numAttributes()];
        //    int index;

        //    // Get values for all attributes.
        //    for (int i = 0; i < numAttributes(); i++)
        //    {

        //        // Get next token
        //        if (i > 0)
        //        {
        //            getNextToken(tokenizer);
        //        }

        //        // Check if value is missing.
        //        if (tokenizer.ttype == '?')
        //        {
        //            instance[i] = Instance.missingValue();
        //        }
        //        else
        //        {

        //            // Check if token is valid.
        //            if (tokenizer.ttype != StreamTokenizer.TT_WORD)
        //            {
        //                errms(tokenizer, "not a valid value");
        //            }
        //            switch (attribute(i).type())
        //            {
        //                case Attribute.NOMINAL:
        //                    // Check if value appears in header.
        //                    index = attribute(i).indexOfValue(tokenizer.sval);
        //                    if (index == -1)
        //                    {
        //                        errms(tokenizer, "nominal value not declared in header");
        //                    }
        //                    instance[i] = (double)index;
        //                    break;
        //                case Attribute.NUMERIC:
        //                    // Check if value is really a number.
        //                    try
        //                    {
        //                        instance[i] = double.valueOf(tokenizer.sval).
        //                          doubleValue();
        //                    }
        //                    catch (NumberFormatException e)
        //                    {
        //                        errms(tokenizer, "number expected");
        //                    }
        //                    break;
        //                case Attribute.STRING:
        //                    instance[i] = attribute(i).addStringValue(tokenizer.sval);
        //                    break;
        //                case Attribute.DATE:
        //                    try
        //                    {
        //                        instance[i] = attribute(i).parseDate(tokenizer.sval);
        //                    }
        //                    catch (ParseException e)
        //                    {
        //                        errms(tokenizer, "unparseable date: " + tokenizer.sval);
        //                    }
        //                    break;
        //                default:
        //                    errms(tokenizer, "unknown attribute type in column " + i);
        //            }
        //        }
        //    }
        //    if (flag)
        //    {
        //        getLastToken(tokenizer, true);
        //    }

        //    // Add instance to dataset
        //    add(new Instance(1, instance));
        //    return true;
        //}

        /**
         * Reads and stores header of an ARFF file.
         *
         * @param tokenizer the stream tokenizer
         * @exception IOException if the information is not read 
         * successfully
         */
        //protected void readHeader(StreamTokenizer tokenizer)
        //{

        //    string attributeName;
        //    FastVector attributeValues;
        //    int i;

        //    // Get name of relation.
        //    getFirstToken(tokenizer);
        //    if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //    {
        //        errms(tokenizer, "premature end of file");
        //    }
        //    if (ARFF_RELATION.equalsIgnoreCase(tokenizer.sval))
        //    {
        //        getNextToken(tokenizer);
        //        m_RelationName = tokenizer.sval;
        //        getLastToken(tokenizer, false);
        //    }
        //    else
        //    {
        //        errms(tokenizer, "keyword " + ARFF_RELATION + " expected");
        //    }

        //    // Create vectors to hold information temporarily.
        //    m_Attributes = new FastVector();

        //    // Get attribute declarations.
        //    getFirstToken(tokenizer);
        //    if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //    {
        //        errms(tokenizer, "premature end of file");
        //    }

        //    while (Attribute.ARFF_ATTRIBUTE.equalsIgnoreCase(tokenizer.sval))
        //    {

        //        // Get attribute name.
        //        getNextToken(tokenizer);
        //        attributeName = tokenizer.sval;
        //        getNextToken(tokenizer);

        //        // Check if attribute is nominal.
        //        if (tokenizer.ttype == StreamTokenizer.TT_WORD)
        //        {

        //            // Attribute is real, integer, or string.
        //            if (tokenizer.sval.equalsIgnoreCase(Attribute.ARFF_ATTRIBUTE_REAL) ||
        //                tokenizer.sval.equalsIgnoreCase(Attribute.ARFF_ATTRIBUTE_INTEGER) ||
        //                tokenizer.sval.equalsIgnoreCase(Attribute.ARFF_ATTRIBUTE_NUMERIC))
        //            {
        //                m_Attributes.Add(new Attribute(attributeName, numAttributes()));
        //                readTillEOL(tokenizer);
        //            }
        //            else if (tokenizer.sval.equalsIgnoreCase(Attribute.ARFF_ATTRIBUTE_STRING))
        //            {
        //                m_Attributes.
        //                  Add(new Attribute(attributeName, (FastVector)null,
        //                               numAttributes()));
        //                readTillEOL(tokenizer);
        //            }
        //            else if (tokenizer.sval.equalsIgnoreCase(Attribute.ARFF_ATTRIBUTE_DATE))
        //            {
        //                string format = null;
        //                if (tokenizer.nextToken() != StreamTokenizer.TT_EOL)
        //                {
        //                    if ((tokenizer.ttype != StreamTokenizer.TT_WORD) &&
        //                        (tokenizer.ttype != '\'') &&
        //                        (tokenizer.ttype != '\"'))
        //                    {
        //                        errms(tokenizer, "not a valid date format");
        //                    }
        //                    format = tokenizer.sval;
        //                    readTillEOL(tokenizer);
        //                }
        //                else
        //                {
        //                    tokenizer.pushBack();
        //                }
        //                m_Attributes.Add(new Attribute(attributeName, format,
        //                                                          numAttributes()));

        //            }
        //            else
        //            {
        //                errms(tokenizer, "no valid attribute type or invalid " +
        //                  "enumeration");
        //            }
        //        }
        //        else
        //        {

        //            // Attribute is nominal.
        //            attributeValues = new FastVector();
        //            tokenizer.pushBack();

        //            // Get values for nominal attribute.
        //            if (tokenizer.nextToken() != '{')
        //            {
        //                errms(tokenizer, "{ expected at beginning of enumeration");
        //            }
        //            while (tokenizer.nextToken() != '}')
        //            {
        //                if (tokenizer.ttype == StreamTokenizer.TT_EOL)
        //                {
        //                    errms(tokenizer, "} expected at end of enumeration");
        //                }
        //                else
        //                {
        //                    attributeValues.Add(tokenizer.sval);
        //                }
        //            }
        //            m_Attributes.
        //              Add(new Attribute(attributeName, attributeValues,
        //                           numAttributes()));
        //        }
        //        getLastToken(tokenizer, false);
        //        getFirstToken(tokenizer);
        //        if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //            errms(tokenizer, "premature end of file");
        //    }

        //    // Check if data part follows. We can't easily check for EOL.
        //    if (!ARFF_DATA.equalsIgnoreCase(tokenizer.sval))
        //    {
        //        errms(tokenizer, "keyword " + ARFF_DATA + " expected");
        //    }

        //    // Check if any attributes have been declared.
        //    if (m_Attributes.size() == 0)
        //    {
        //        errms(tokenizer, "no attributes declared");
        //    }

        //    // Allocate buffers in case sparse instances have to be read
        //    m_ValueBuffer = new double[numAttributes()];
        //    m_IndicesBuffer = new int[numAttributes()];
        //}

        /**
         * Copies instances from one set to the end of another 
         * one.
         *
         * @param source the source of the instances
         * @param from the position of the first instance to be copied
         * @param dest the destination for the instances
         * @param num the number of instances to be copied
         */
        //@ requires 0 <= from && from <= numInstances() - num;
        //@ requires 0 <= num;
        protected void copyInstances(int from, /*@non_null@*/ Instances dest, int num)
        {

            for (int i = 0; i < num; i++)
            {
                dest.add(instance(from + i));
            }
        }

        /**
         * Throws error message with line number and last token read.
         *
         * @param theMsg the error message to be thrown
         * @param tokenizer the stream tokenizer
         * @throws IOExcpetion containing the error message
         */
        //protected void errms(StreamTokenizer tokenizer, string theMsg)
        //{

        //    throw new IOException(theMsg + ", read " + tokenizer.ToString());
        //}

        /**
         * Replaces the attribute information by a Clone of
         * itself.
         */
        protected void freshAttributeInfo()
        {

            m_Attributes = (FastVector)m_Attributes.copyElements();
        }

        /**
         * Gets next token, skipping empty lines.
         *
         * @param tokenizer the stream tokenizer
         * @exception IOException if reading the next token fails
         */
        //protected void getFirstToken(StreamTokenizer tokenizer)
        //{

        //    while (tokenizer.nextToken() == StreamTokenizer.TT_EOL) { };
        //    if ((tokenizer.ttype == '\'') ||
        //    (tokenizer.ttype == '"'))
        //    {
        //        tokenizer.ttype = StreamTokenizer.TT_WORD;
        //    }
        //    else if ((tokenizer.ttype == StreamTokenizer.TT_WORD) &&
        //           (tokenizer.sval.Equals("?")))
        //    {
        //        tokenizer.ttype = '?';
        //    }
        //}

        /**
         * Gets index, checking for a premature and of line.
         *
         * @param tokenizer the stream tokenizer
         * @exception IOException if it finds a premature end of line
         */
        //protected void getIndex(StreamTokenizer tokenizer)
        //{

        //    if (tokenizer.nextToken() == StreamTokenizer.TT_EOL)
        //    {
        //        errms(tokenizer, "premature end of line");
        //    }
        //    if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //    {
        //        errms(tokenizer, "premature end of file");
        //    }
        //}

        /**
         * Gets token and checks if its end of line.
         *
         * @param tokenizer the stream tokenizer
         * @exception IOException if it doesn't find an end of line
         */
        //protected void getLastToken(StreamTokenizer tokenizer, bool endOfFileOk)
        //{

        //    if ((tokenizer.nextToken() != StreamTokenizer.TT_EOL) &&
        //    ((tokenizer.ttype != StreamTokenizer.TT_EOF) || !endOfFileOk))
        //    {
        //        errms(tokenizer, "end of line expected");
        //    }
        //}

        /**
         * Gets next token, checking for a premature and of line.
         *
         * @param tokenizer the stream tokenizer
         * @exception IOException if it finds a premature end of line
         */
        //protected void getNextToken(StreamTokenizer tokenizer)
        //{

        //    if (tokenizer.nextToken() == StreamTokenizer.TT_EOL)
        //    {
        //        errms(tokenizer, "premature end of line");
        //    }
        //    if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        //    {
        //        errms(tokenizer, "premature end of file");
        //    }
        //    else if ((tokenizer.ttype == '\'') ||
        //           (tokenizer.ttype == '"'))
        //    {
        //        tokenizer.ttype = StreamTokenizer.TT_WORD;
        //    }
        //    else if ((tokenizer.ttype == StreamTokenizer.TT_WORD) &&
        //         (tokenizer.sval.Equals("?")))
        //    {
        //        tokenizer.ttype = '?';
        //    }
        //}

        /**
         * Initializes the StreamTokenizer used for reading the ARFF file.
         *
         * @param tokenizer the stream tokenizer
         */
        //protected void initTokenizer(StreamTokenizer tokenizer)
        //{

        //    tokenizer.resetSyntax();
        //    tokenizer.whitespaceChars(0, ' ');
        //    tokenizer.wordChars(' ' + 1, '\u00FF');
        //    tokenizer.whitespaceChars(',', ',');
        //    tokenizer.commentChar('%');
        //    tokenizer.quoteChar('"');
        //    tokenizer.quoteChar('\'');
        //    tokenizer.ordinaryChar('{');
        //    tokenizer.ordinaryChar('}');
        //    tokenizer.eolIsSignificant(true);
        //}

        /**
         * Returns string including all instances, their weights and
         * their indices in the original dataset.
         *
         * @return description of instance and its weight as a string
         */
        protected /*@pure@*/ string instancesAndWeights()
        {

            StringBuilder text = new StringBuilder();

            for (int i = 0; i < numInstances(); i++)
            {
                text.Append(instance(i) + " " + instance(i).weight());
                if (i < numInstances() - 1)
                {
                    text.Append(Environment.NewLine);
                }
            }
            return text.ToString();
        }

        /**
         * Partitions the instances around a pivot. Used by quicksort and
         * kthSmallestValue.
         *
         * @param attIndex the attribute's index
         * @param left the first index of the subset 
         * @param right the last index of the subset 
         *
         * @return the index of the middle element
         */
        //@ requires 0 <= attIndex && attIndex < numAttributes();
        //@ requires 0 <= left && left <= right && right < numInstances();
        protected int partition(int attIndex, int l, int r)
        {

            double pivot = instance((l + r) / 2).value(attIndex);

            while (l < r)
            {
                while ((instance(l).value(attIndex) < pivot) && (l < r))
                {
                    l++;
                }
                while ((instance(r).value(attIndex) > pivot) && (l < r))
                {
                    r--;
                }
                if (l < r)
                {
                    swap(l, r);
                    l++;
                    r--;
                }
            }
            if ((l == r) && (instance(r).value(attIndex) > pivot))
            {
                r--;
            }

            return r;
        }

        /**
         * Implements quicksort according to Manber's "Introduction to
         * Algorithms".
         *
         * @param attIndex the attribute's index
         * @param left the first index of the subset to be sorted
         * @param right the last index of the subset to be sorted
         */
        //@ requires 0 <= attIndex && attIndex < numAttributes();
        //@ requires 0 <= first && first <= right && right < numInstances();
        protected void quickSort(int attIndex, int left, int right)
        {

            if (left < right)
            {
                int middle = partition(attIndex, left, right);
                quickSort(attIndex, left, middle);
                quickSort(attIndex, middle + 1, right);
            }
        }

        /**
         * Reads and skips all tokens before next end of line token.
         *
         * @param tokenizer the stream tokenizer
         */
        //protected void readTillEOL(StreamTokenizer tokenizer)
        //{

        //    while (tokenizer.nextToken() != StreamTokenizer.TT_EOL) { };
        //    tokenizer.pushBack();
        //}

        /**
         * Implements computation of the kth-smallest element according
         * to Manber's "Introduction to Algorithms".
         *
         * @param attIndex the attribute's index
         * @param left the first index of the subset 
         * @param right the last index of the subset 
         * @param k the value of k
         *
         * @return the index of the kth-smallest element
         */
        //@ requires 0 <= attIndex && attIndex < numAttributes();
        //@ requires 0 <= first && first <= right && right < numInstances();
        protected int select(int attIndex, int left, int right, int k)
        {

            if (left == right)
            {
                return left;
            }
            else
            {
                int middle = partition(attIndex, left, right);
                if ((middle - left + 1) >= k)
                {
                    return select(attIndex, left, middle, k);
                }
                else
                {
                    return select(attIndex, middle + 1, right, k - (middle - left + 1));
                }
            }
        }

        /**
         * Help function needed for stratification of set.
         *
         * @param numFolds the number of folds for the stratification
         */
        protected void stratStep(int numFolds)
        {

            FastVector newVec = new FastVector(m_Instances.capacity());
            int start = 0, j;

            // create stratified batch
            while (newVec.size() < numInstances())
            {
                j = start;
                while (j < numInstances())
                {
                    newVec.Add(instance(j));
                    j = j + numFolds;
                }
                start++;
            }
            m_Instances = newVec;
        }

        /**
         * Swaps two instances in the set.
         *
         * @param i the first instance's index
         * @param j the second instance's index
         */
        //@ requires 0 <= i && i < numInstances();
        //@ requires 0 <= j && j < numInstances();
        public void swap(int i, int j)
        {

            m_Instances.swap(i, j);
        }

        /**
         * Merges two sets of Instances together. The resulting set will have
         * all the attributes of the first set plus all the attributes of the 
         * second set. The number of instances in both sets must be the same.
         *
         * @param first the first set of Instances
         * @param second the second set of Instances
         * @return the merged set of Instances
         * @exception Exception if the datasets are not the same size
         */
        public static Instances mergeInstances(Instances first, Instances second)
        {

            if (first.numInstances() != second.numInstances())
            {
                throw new Exception("Instance sets must be of the same size");
            }

            // Create the vector of merged attributes
            FastVector newAttributes = new FastVector();
            for (int i = 0; i < first.numAttributes(); i++)
            {
                newAttributes.Add(first.attribute(i));
            }
            for (int i = 0; i < second.numAttributes(); i++)
            {
                newAttributes.Add(second.attribute(i));
            }

            // Create the set of Instances
            Instances merged = new Instances(first.relationName() + '_'
                             + second.relationName(),
                             newAttributes,
                             first.numInstances());
            // Merge each instance
            for (int i = 0; i < first.numInstances(); i++)
            {
                merged.add(first.instance(i).mergeInstance(second.instance(i)));
            }
            return merged;
        }

        /**
         * Method for testing this class.
         *
         * @param argv should contain one element: the name of an ARFF file
         */
        //@ requires argv != null;
        //@ requires argv.Length == 1;
        //@ requires argv[0] != null;
        public static void test(string[] argv)
        {

            Instances instances, secondInstances, train, test, transformed, empty;
            Instance instance;
            Random random = new Random(2);
            StreamReader reader;
            int start, num;
            double newWeight;
            FastVector testAtts, testVals;
            int i, j;

            try
            {
                if (argv.Length > 1)
                {
                    throw (new Exception("Usage: Instances [<filename>]"));
                }

                // Creating set of instances from scratch
                testVals = new FastVector(2);
                testVals.Add("first_value");
                testVals.Add("second_value");
                testAtts = new FastVector(2);
                testAtts.Add(new Attribute("nominal_attribute", testVals));
                testAtts.Add(new Attribute("numeric_attribute"));
                instances = new Instances("test_set", testAtts, 10);
                instances.add(new Instance(instances.numAttributes()));
                instances.add(new Instance(instances.numAttributes()));
                instances.add(new Instance(instances.numAttributes()));
                instances.setClassIndex(0);
                PrintToScreen.WriteLine("\nSet of instances created from scratch:\n");
                PrintToScreen.WriteLine(instances);

                if (argv.Length == 1)
                {
                    string filename = argv[0];
                    reader = new StreamReader(filename);

                    // Read first five instances and print them
                    PrintToScreen.WriteLine("\nFirst five instances from file:\n");
                    instances = new Instances(reader, 1);
                    instances.setClassIndex(instances.numAttributes() - 1);
                    i = 0;
                    while ((i < 5) && (instances.readInstance(reader)))
                    {
                        i++;
                    }
                    PrintToScreen.WriteLine(instances);

                    // Read all the instances in the file
                    reader = new StreamReader(filename);
                    instances = new Instances(reader);

                    // Make the last attribute be the class 
                    instances.setClassIndex(instances.numAttributes() - 1);

                    // Print header and instances.
                    PrintToScreen.WriteLine("\nDataset:\n");
                    PrintToScreen.WriteLine(instances);
                    PrintToScreen.WriteLine("\nClass index: " + instances.classIndex());
                }

                // UnitTest basic methods based on class index.
                PrintToScreen.WriteLine("\nClass name: " + instances.classAttribute().name());
                PrintToScreen.WriteLine("\nClass index: " + instances.classIndex());
                PrintToScreen.WriteLine("\nClass is nominal: " +
                       instances.classAttribute().isNominal());
                PrintToScreen.WriteLine("\nClass is numeric: " +
                       instances.classAttribute().isNumeric());
                PrintToScreen.WriteLine("\nClasses:\n");
                for (i = 0; i < instances.numClasses(); i++)
                {
                    PrintToScreen.WriteLine(instances.classAttribute().value(i));
                }
                PrintToScreen.WriteLine("\nClass values and labels of instances:\n");
                for (i = 0; i < instances.numInstances(); i++)
                {
                    Instance inst = instances.instance(i);
                    Console.Write(inst.classValue() + "\t");
                    Console.Write(inst.ToString(inst.classIndex()));
                    if (instances.instance(i).classIsMissing())
                    {
                        PrintToScreen.WriteLine("\tis missing");
                    }
                    else
                    {
                        PrintToScreen.WriteLine();
                    }
                }

                // Create random weights.
                PrintToScreen.WriteLine("\nCreating random weights for instances.");
                for (i = 0; i < instances.numInstances(); i++)
                {
                    instances.instance(i).setWeight(random.NextDouble());
                }

                // Print all instances and their weights (and the sum of weights).
                PrintToScreen.WriteLine("\nInstances and their weights:\n");
                PrintToScreen.WriteLine(instances.instancesAndWeights());
                Console.Write("\nSum of weights: ");
                PrintToScreen.WriteLine(instances.sumOfWeights());

                // Insert an attribute
                secondInstances = new Instances(instances);
                Attribute testAtt = new Attribute("Inserted");
                secondInstances.insertAttributeAt(testAtt, 0);
                PrintToScreen.WriteLine("\nSet with inserted attribute:\n");
                PrintToScreen.WriteLine(secondInstances);
                PrintToScreen.WriteLine("\nClass name: "
                       + secondInstances.classAttribute().name());

                // Delete the attribute
                secondInstances.deleteAttributeAt(0);
                PrintToScreen.WriteLine("\nSet with attribute deleted:\n");
                PrintToScreen.WriteLine(secondInstances);
                PrintToScreen.WriteLine("\nClass name: "
                       + secondInstances.classAttribute().name());

                // UnitTest if headers are equal
                PrintToScreen.WriteLine("\nHeaders equal: " +
                       instances.equalHeaders(secondInstances) + Environment.NewLine);

                // Print data in internal format.
                PrintToScreen.WriteLine("\nData (internal values):\n");
                for (i = 0; i < instances.numInstances(); i++)
                {
                    for (j = 0; j < instances.numAttributes(); j++)
                    {
                        if (instances.instance(i).isMissing(j))
                        {
                            Console.Write("? ");
                        }
                        else
                        {
                            Console.Write(instances.instance(i).value(j) + " ");
                        }
                    }
                    PrintToScreen.WriteLine();
                }

                // Just print header
                PrintToScreen.WriteLine("\nEmpty dataset:\n");
                empty = new Instances(instances, 0);
                PrintToScreen.WriteLine(empty);
                PrintToScreen.WriteLine("\nClass name: " + empty.classAttribute().name());

                // Create copy and rename an attribute and a value (if possible)
                if (empty.classAttribute().isNominal())
                {
                    Instances copy = new Instances(empty, 0);
                    copy.renameAttribute(copy.classAttribute(), "new_name");
                    copy.renameAttributeValue(copy.classAttribute(),
                                  copy.classAttribute().value(0),
                                  "new_val_name");
                    PrintToScreen.WriteLine("\nDataset with names changed:\n" + copy);
                    PrintToScreen.WriteLine("\nOriginal dataset:\n" + empty);
                }

                // Create and prints subset of instances.
                start = instances.numInstances() / 4;
                num = instances.numInstances() / 2;
                Console.Write("\nSubset of dataset: ");
                PrintToScreen.WriteLine(num + " instances from " + (start + 1)
                       + ". instance");
                secondInstances = new Instances(instances, start, num);
                PrintToScreen.WriteLine("\nClass name: "
                       + secondInstances.classAttribute().name());

                // Print all instances and their weights (and the sum of weights).
                PrintToScreen.WriteLine("\nInstances and their weights:\n");
                PrintToScreen.WriteLine(secondInstances.instancesAndWeights());
                Console.Write("\nSum of weights: ");
                PrintToScreen.WriteLine(secondInstances.sumOfWeights());

                // Create and print training and test sets for 3-fold
                // cross-validation.
                PrintToScreen.WriteLine("\nTrain and test folds for 3-fold CV:");
                if (instances.classAttribute().isNominal())
                {
                    instances.stratify(3);
                }
                for (j = 0; j < 3; j++)
                {
                    train = instances.trainCV(3, j, new Random(1));
                    test = instances.testCV(3, j);

                    // Print all instances and their weights (and the sum of weights).
                    PrintToScreen.WriteLine("\nTrain: ");
                    PrintToScreen.WriteLine("\nInstances and their weights:\n");
                    PrintToScreen.WriteLine(train.instancesAndWeights());
                    Console.Write("\nSum of weights: ");
                    PrintToScreen.WriteLine(train.sumOfWeights());
                    PrintToScreen.WriteLine("\nClass name: " + train.classAttribute().name());
                    PrintToScreen.WriteLine("\nTest: ");
                    PrintToScreen.WriteLine("\nInstances and their weights:\n");
                    PrintToScreen.WriteLine(test.instancesAndWeights());
                    Console.Write("\nSum of weights: ");
                    PrintToScreen.WriteLine(test.sumOfWeights());
                    PrintToScreen.WriteLine("\nClass name: " + test.classAttribute().name());
                }

                // Randomize instances and print them.
                PrintToScreen.WriteLine("\nRandomized dataset:");
                instances.randomize(random);

                // Print all instances and their weights (and the sum of weights).
                PrintToScreen.WriteLine("\nInstances and their weights:\n");
                PrintToScreen.WriteLine(instances.instancesAndWeights());
                Console.Write("\nSum of weights: ");
                PrintToScreen.WriteLine(instances.sumOfWeights());

                // Sort instances according to first attribute and
                // print them.
                Console.Write("\nInstances sorted according to first attribute:\n ");
                instances.sort(0);

                // Print all instances and their weights (and the sum of weights).
                PrintToScreen.WriteLine("\nInstances and their weights:\n");
                PrintToScreen.WriteLine(instances.instancesAndWeights());
                Console.Write("\nSum of weights: ");
                PrintToScreen.WriteLine(instances.sumOfWeights());
            }
            catch (Exception e)
            {
                PrintToScreen.WriteLine(e.StackTrace);
            }
        }

        /**
         * Main method for this class. The following calls are possible:
         * <ul>
         *   <li>
         *     <code>weka.core.Instances</code> &lt;filename&gt;<br/>
         *     prints a summary of a set of instances.
         *   </li>
         *   <li>
         *     <code>weka.core.Instances</code> merge &lt;filename1&gt; &lt;filename2&gt;<br/>
         *     merges the two datasets (must have same number of instances) and
         *     outputs the results on stdout.
         *   </li>
         * </ul>
         *
         * @param args 	the commandline parameters
         */
        //  public static void main(string[] args) {

        //  try {
        //    Instances i;
        //    // read from stdin and print statistics
        //    if (args.Length == 0) {
        //  i = new Instances(new StreamReader(System.in));
        //  PrintToScreen.WriteLine(i.toSummaryString());
        //    }
        //    // read file and print statistics
        //    else if (args.Length == 1) {
        //  i = new Instances(new StreamReader(args[0]));
        //  PrintToScreen.WriteLine(i.toSummaryString());
        //    }
        //    // read two files, merge them and print result to stdout
        //    else if ((args.Length == 3) && (args[0].ToLower().Equals("merge"))) {
        //  i = Instances.mergeInstances(
        //          new Instances(new StreamReader((args[1]))),
        //          new Instances(new StreamReader((args[2]))));
        //  PrintToScreen.WriteLine(i);
        //    }
        //    // wrong parameters
        //    else {
        //  PrintToScreen.WriteLine(
        //      "\nUsage:\n"
        //      + "\tweka.core.Instances <filename>\n"
        //      + "\tweka.core.Instances merge <filename1> <filename2>\n");
        //  System.exit(1);
        //    }
        //  }
        //  catch (Exception ex) {
        //    ex.printStackTrace();
        //    PrintToScreen.WriteLine(ex.getMessage());
        //  }
        //}
    }

}
