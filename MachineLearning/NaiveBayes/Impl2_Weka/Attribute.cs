using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
     *    Attribute.java
     *    Copyright (C) 1999 Eibe Frank
     *
     */

    //package weka.core;

    //import java.io.Serializable;
    //import java.text.ParseException;
    //import java.text.SimpleDateFormat;
    //import java.util.DateWrapper;
    //import java.util.IEnumerator;
    //import java.util.Hashtable;
    //import java.util.Properties;
    //import java.io.StreamTokenizer;
    //import java.io.StringReader;
    //import java.io.IOException;

    /** 
     * Class for handling an attribute. Once an attribute has been created,
     * it can't be changed. <p>
     *
     * The following attribute types are supported:
     * <ul>
     *    <li> numeric: <br/>
     *         This type of attribute represents a floating-point number.
     *    </li>
     *    <li> nominal: <br/>
     *         This type of attribute represents a fixed set of nominal values.
     *    </li>
     *    <li> string: <br/>
     *         This type of attribute represents a dynamically expanding set of
     *         nominal values. Usually used in text classification.
     *    </li>
     *    <li> date: <br/>
     *         This type of attribute represents a date, internally represented as 
     *         floating-point number storing the milliseconds since January 1, 
     *         1970, 00:00:00 GMT. The string representation of the date must be
     *         <a href="http://www.iso.org/iso/en/prods-services/popstds/datesandtime.html" target="_blank">
     *         ISO-8601</a> compliant, the default is <code>yyyy-MM-dd'T'HH:mm:ss</code>.
     *    </li>
     * </ul>
     * 
     * Typical usage (code from the main() method of this class): <p>
     *
     * <code>
     * ... <br>
     *
     * // Create numeric attributes "Length" and "weight" <br>
     * Attribute Length = new Attribute("Length"); <br>
     * Attribute weight = new Attribute("weight"); <br><br>
     * 
     * // Create vector to hold nominal values "first", "second", "third" <br>
     * FastVector my_nominal_values = new FastVector(3); <br>
     * my_nominal_values.Add("first"); <br>
     * my_nominal_values.Add("second"); <br>
     * my_nominal_values.Add("third"); <br><br>
     *
     * // Create nominal attribute "position" <br>
     * Attribute position = new Attribute("position", my_nominal_values);<br>
     *
     * ... <br>
     * </code><p>
     *
     * @author Eibe Frank (eibe@cs.waikato.ac.nz)
     * @version $Revision: 1.32.2.3 $
     */

    public class Attribute
    {

        /** Constant set for numeric attributes. */
        public const int NUMERIC = 0;

        /** Constant set for nominal attributes. */
        public const int NOMINAL = 1;

        /** Constant set for attributes with string values. */
        public const int STRING = 2;

        /** Constant set for attributes with date values. */
        public const int DATE = 3;

        /** Constant set for symbolic attributes. */
        public const int ORDERING_SYMBOLIC = 0;

        /** Constant set for ordered attributes. */
        public const int ORDERING_ORDERED = 1;

        /** Constant set for modulo-ordered attributes. */
        public const int ORDERING_MODULO = 2;

        /** The keyword used to denote the start of an arff attribute declaration */
        static string ARFF_ATTRIBUTE = "@attribute";

        /** A keyword used to denote a numeric attribute */
        static string ARFF_ATTRIBUTE_INTEGER = "integer";

        /** A keyword used to denote a numeric attribute */
        static string ARFF_ATTRIBUTE_REAL = "real";

        /** A keyword used to denote a numeric attribute */
        static string ARFF_ATTRIBUTE_NUMERIC = "numeric";

        /** The keyword used to denote a string attribute */
        static string ARFF_ATTRIBUTE_STRING = "string";

        /** The keyword used to denote a date attribute */
        static string ARFF_ATTRIBUTE_DATE = "date";

        /** Strings longer than this will be stored compressed. */
        private static int STRING_COMPRESS_THRESHOLD = 200;

        /** The attribute's name. */
        private /*@ spec_public non_null @*/ string m_Name;

        /** The attribute's type. */
        private /*@ spec_public @*/ int m_Type;
        /*@ invariant m_Type == NUMERIC || 
                      m_Type == DATE || 
                      m_Type == STRING || 
                      m_Type == NOMINAL;
        */

        /** The attribute's values (if nominal or string). */
        private /*@ spec_public @*/ FastVector m_Values;

        /** Mapping of values to indices (if nominal or string). */
        private Hashtable m_Hashtable;

        /** DateWrapper format specification for date attributes */
        //private SimpleDateFormat m_DateFormat;

        /** The attribute's index. */
        private /*@ spec_public @*/ int m_Index;

        /** The attribute's metadata. */
        private ProtectedProperties m_Metadata;

        /** The attribute's ordering. */
        private int m_Ordering;

        /** Whether the attribute is regular. */
        private bool m_IsRegular;

        /** Whether the attribute is averagable. */
        private bool m_IsAveragable;

        /** Whether the attribute has a zeropoint. */
        private bool m_HasZeropoint;

        /** The attribute's weight. */
        private double m_Weight;

        /** The attribute's lower numeric bound. */
        private double m_LowerBound;

        /** Whether the lower bound is open. */
        private bool m_LowerBoundIsOpen;

        /** The attribute's upper numeric bound. */
        private double m_UpperBound;

        /** Whether the upper bound is open */
        private bool m_UpperBoundIsOpen;

        /**
         * Constructor for a numeric attribute.
         *
         * @param attributeName the name for the attribute
         */
        //@ requires attributeName != null;
        //@ ensures  m_Name == attributeName;
        public Attribute(string attributeName) :
            this(attributeName, new ProtectedProperties(new Dictionary<object, object>()))
        {
        }

        /**
         * Constructor for a numeric attribute, where metadata is supplied.
         *
         * @param attributeName the name for the attribute
         * @param metadata the attribute's properties
         */
        //@ requires attributeName != null;
        //@ requires metadata != null;
        //@ ensures  m_Name == attributeName;
        public Attribute(string attributeName, ProtectedProperties metadata)
        {

            m_Name = attributeName;
            m_Index = -1;
            m_Values = null;
            m_Hashtable = null;
            m_Type = NUMERIC;
            setMetadata(metadata);
        }

        /**
         * Constructor for a date attribute.
         *
         * @param attributeName the name for the attribute
         * @param dateFormat a string suitable for use with
         * SimpleDateFormatter for parsing dates.
         */
        //@ requires attributeName != null;
        //@ requires dateFormat != null;
        //@ ensures  m_Name == attributeName;
        public Attribute(string attributeName, string dateFormat) :
            this(attributeName, dateFormat,
           new ProtectedProperties(new Dictionary<object, object>()))
        {
        }

        /**
         * Constructor for a date attribute, where metadata is supplied.
         *
         * @param attributeName the name for the attribute
         * @param dateFormat a string suitable for use with
         * SimpleDateFormatter for parsing dates.
         * @param metadata the attribute's properties
         */
        //@ requires attributeName != null;
        //@ requires dateFormat != null;
        //@ requires metadata != null;
        //@ ensures  m_Name == attributeName;
        public Attribute(string attributeName, string dateFormat,
                 ProtectedProperties metadata)
        {

            m_Name = attributeName;
            m_Index = -1;
            m_Values = null;
            m_Hashtable = null;
            m_Type = DATE;
            //if (dateFormat != null) {
            //  m_DateFormat = new SimpleDateFormat(dateFormat);
            //} else {
            //  m_DateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            //}
            //m_DateFormat.setLenient(false);
            setMetadata(metadata);
        }

        /**
         * Constructor for nominal attributes and string attributes.
         * If a null vector of attribute values is passed to the method,
         * the attribute is assumed to be a string.
         *
         * @param attributeName the name for the attribute
         * @param attributeValues a vector of strings denoting the 
         * attribute values. Null if the attribute is a string attribute.
         */
        //@ requires attributeName != null;
        //@ ensures  m_Name == attributeName;
        public Attribute(string attributeName,
                 FastVector attributeValues) :
            this(attributeName, attributeValues,
           new ProtectedProperties(new Dictionary<object, object>()))
        {
        }

        /**
         * Constructor for nominal attributes and string attributes, where
         * metadata is supplied. If a null vector of attribute values is passed
         * to the method, the attribute is assumed to be a string.
         *
         * @param attributeName the name for the attribute
         * @param attributeValues a vector of strings denoting the 
         * attribute values. Null if the attribute is a string attribute.
         * @param metadata the attribute's properties
         */
        //@ requires attributeName != null;
        //@ requires metadata != null;
        /*@ ensures  m_Name == attributeName;
            ensures  m_Index == -1;
            ensures  attributeValues == null && m_Type == STRING
                  || attributeValues != null && m_Type == NOMINAL 
                        && m_Values.size() == attributeValues.size();
            signals (Exception ex) 
                       (* if duplicate strings in attributeValues *);
        */
        public Attribute(string attributeName,
                 FastVector attributeValues,
                 ProtectedProperties metadata)
        {

            m_Name = attributeName;
            m_Index = -1;
            if (attributeValues == null)
            {
                m_Values = new FastVector();
                m_Hashtable = new Hashtable();
                m_Type = STRING;
            }
            else
            {
                m_Values = new FastVector(attributeValues.size());
                m_Hashtable = new Hashtable(attributeValues.size());
                for (int i = 0; i < attributeValues.size(); i++)
                {
                    Object store = attributeValues.elementAt(i);
                    if (((string)store).Length > STRING_COMPRESS_THRESHOLD)
                    {
                        //try {
                        //  store = new SerializedObject(attributeValues.elementAt(i), true);
                        //} catch (Exception ex) {
                        //  PrintToScreen.WriteLine("Couldn't compress nominal attribute value -"
                        //             + " storing uncompressed.");
                        //}
                    }
                    if (m_Hashtable.ContainsKey(store))
                    {
                        throw new Exception("A nominal attribute (" +
                                           attributeName + ") cannot"
                                           + " have duplicate labels (" + store + ").");
                    }
                    m_Values.Add(store);
                    m_Hashtable.Add(store, i);
                }
                m_Type = NOMINAL;
            }
            setMetadata(metadata);
        }

        /**
         * Produces a shallow copy of this attribute.
         *
         * @return a copy of this attribute with the same index
         */
        //@ also ensures \result is Attribute;
        public /*@ pure non_null @*/ Object copy()
        {

            Attribute copy = new Attribute(m_Name);

            copy.m_Index = m_Index;
            copy.m_Type = m_Type;
            copy.m_Values = m_Values;
            copy.m_Hashtable = m_Hashtable;
            //copy.m_DateFormat = m_DateFormat;
            copy.setMetadata(m_Metadata);

            return copy;
        }

        /**
         * Returns an enumeration of all the attribute's values if
         * the attribute is nominal or a string, null otherwise. 
         *
         * @return enumeration of all the attribute's values
         */
        public  /*@ pure @*/ FastVector.FastVectorEnumeration enumerateValues()
        {

            if (isNominal() || isString())
            {
                return m_Values.elements();
            }
            return null;
        }

        /**
         * Tests if given attribute is equal to this attribute.
         *
         * @param other the Object to be compared to this attribute
         * @return true if the given attribute is equal to this attribute
         */
        public  /*@ pure @*/ bool Equals(Object other)
        {

            if ((other == null) ||
                !(other.GetType().Equals(this.GetType())))
            {
                return false;
            }
            Attribute att = (Attribute)other;
            if (!m_Name.Equals(att.m_Name))
            {
                return false;
            }
            if (isNominal() && att.isNominal())
            {
                if (m_Values.size() != att.m_Values.size())
                {
                    return false;
                }
                for (int i = 0; i < m_Values.size(); i++)
                {
                    if (!m_Values.elementAt(i).Equals(att.m_Values.elementAt(i)))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return (type() == att.type());
            }
        }

        /**
         * Returns the index of this attribute.
         *
         * @return the index of this attribute
         */
        //@ ensures \result == m_Index;
        public  /*@ pure @*/ int index()
        {

            return m_Index;
        }

        /**
         * Returns the index of a given attribute value. (The index of
         * the first occurence of this value.)
         *
         * @param value the value for which the index is to be returned
         * @return the index of the given attribute value if attribute
         * is nominal or a string, -1 if it is numeric or the value 
         * can't be found
         */
        public int indexOfValue(string value)
        {

            if (!isNominal() && !isString())
                return -1;
            Object store = value;
            int val = (int)m_Hashtable[store];
            if (val == null)
                return -1;
            else
                return val;
        }

        /**
         * UnitTest if the attribute is nominal.
         *
         * @return true if the attribute is nominal
         */
        //@ ensures \result <==> (m_Type == NOMINAL);
        public  /*@ pure @*/ bool isNominal()
        {

            return (m_Type == NOMINAL);
        }

        /**
         * Tests if the attribute is numeric.
         *
         * @return true if the attribute is numeric
         */
        //@ ensures \result <==> ((m_Type == NUMERIC) || (m_Type == DATE));
        public  /*@ pure @*/ bool isNumeric()
        {

            return ((m_Type == NUMERIC) || (m_Type == DATE));
        }

        /**
         * Tests if the attribute is a string.
         *
         * @return true if the attribute is a string
         */
        //@ ensures \result <==> (m_Type == STRING);
        public  /*@ pure @*/ bool isString()
        {

            return (m_Type == STRING);
        }

        /**
         * Tests if the attribute is a date type.
         *
         * @return true if the attribute is a date type
         */
        //@ ensures \result <==> (m_Type == DATE);
        public  /*@ pure @*/ bool isDate()
        {

            return (m_Type == DATE);
        }

        /**
         * Returns the attribute's name.
         *
         * @return the attribute's name as a string
         */
        //@ ensures \result == m_Name;
        public  /*@ pure @*/ string name()
        {

            return m_Name;
        }

        /**
         * Returns the number of attribute values. Returns 0 for numeric attributes.
         *
         * @return the number of attribute values
         */
        public  /*@ pure @*/ int numValues()
        {

            if (!isNominal() && !isString())
            {
                return 0;
            }
            else
            {
                return m_Values.size();
            }
        }

        /**
         * Returns a description of this attribute in ARFF format. Quotes
         * strings if they contain whitespace characters, or if they
         * are a question mark.
         *
         * @return a description of this attribute as a string
         */
        public string ToString()
        {

            StringBuilder text = new StringBuilder();

            text.Append(ARFF_ATTRIBUTE).Append(" ").Append(
                m_Name).Append(" ");
            switch (m_Type)
            {
                case NOMINAL:
                    text.Append('{');
                    FastVector.FastVectorEnumeration enu = enumerateValues();
                    while (enu.hasMoreElements())
                    {
                        text.Append((string)enu.nextElement());
                        if (enu.hasMoreElements())
                            text.Append(',');
                    }
                    text.Append('}');
                    break;
                case NUMERIC:
                    text.Append(ARFF_ATTRIBUTE_NUMERIC);
                    break;
                case STRING:
                    text.Append(ARFF_ATTRIBUTE_STRING);
                    break;
                case DATE:
                    text.Append(ARFF_ATTRIBUTE_DATE).Append(" ").Append(
                        "");
                    break;
                default:
                    text.Append("UNKNOWN");
                    break;
            }
            return text.ToString();
        }

        /**
         * Returns the attribute's type as an integer.
         *
         * @return the attribute's type.
         */
        //@ ensures \result == m_Type;
        public  /*@ pure @*/ int type()
        {

            return m_Type;
        }

        /**
         * Returns a value of a nominal or string attribute. 
         * Returns an empty string if the attribute is neither
         * nominal nor a string attribute.
         *
         * @param valIndex the value's index
         * @return the attribute's value as a string
         */
        public  /*@ non_null pure @*/ string value(int valIndex)
        {

            if (!isNominal() && !isString())
            {
                return "";
            }
            else
            {
                Object val = m_Values.elementAt(valIndex);

                // If we're storing strings compressed, uncompress it.
                return (string)val;
            }
        }

        /**
         * Constructor for a numeric attribute with a particular index.
         *
         * @param attributeName the name for the attribute
         * @param index the attribute's index
         */
        //@ requires attributeName != null;
        //@ requires index >= 0;
        //@ ensures  m_Name == attributeName;
        //@ ensures  m_Index == index;
        Attribute(string attributeName, int index) :
            this(attributeName)
        {
            m_Index = index;
        }

        /**
         * Constructor for date attributes with a particular index.
         *
         * @param attributeName the name for the attribute
         * @param dateFormat a string suitable for use with
         * SimpleDateFormatter for parsing dates.  Null for a default format
         * string.
         * @param index the attribute's index
         */
        //@ requires attributeName != null;
        //@ requires index >= 0;
        //@ ensures  m_Name == attributeName;
        //@ ensures  m_Index == index;
        Attribute(string attributeName, string dateFormat,
              int index)
            : this(attributeName, dateFormat)
        {
            m_Index = index;
        }

        /**
         * Constructor for nominal attributes and string attributes with
         * a particular index.
         * If a null vector of attribute values is passed to the method,
         * the attribute is assumed to be a string.
         *
         * @param attributeName the name for the attribute
         * @param attributeValues a vector of strings denoting the attribute values.
         * Null if the attribute is a string attribute.
         * @param index the attribute's index
         */
        //@ requires attributeName != null;
        //@ requires index >= 0;
        //@ ensures  m_Name == attributeName;
        //@ ensures  m_Index == index;
        Attribute(string attributeName, FastVector attributeValues,
              int index)
            : this(attributeName, attributeValues)
        {
            m_Index = index;
        }

        /**
         * Adds a string value to the list of valid strings for attributes
         * of type STRING and returns the index of the string.
         *
         * @param value The string value to add
         * @return the index assigned to the string, or -1 if the attribute is not
         * of type Attribute.STRING 
         */
        /*@ requires value != null;
            ensures  isString() && 0 <= \result && \result < m_Values.size() ||
                   ! isString() && \result == -1;
        */
        public int addStringValue(string value)
        {

            if (!isString())
            {
                return -1;
            }
            Object store = value;

            int index = (int)m_Hashtable[store];
            if (index != null)
            {
                return index;
            }
            else
            {
                int intIndex = m_Values.size();
                m_Values.Add(store);
                m_Hashtable.Add(store, intIndex);
                return intIndex;
            }
        }

        /**
         * Adds a string value to the list of valid strings for attributes
         * of type STRING and returns the index of the string. This method is
         * more efficient than addStringValue(string) for long strings.
         *
         * @param src The Attribute containing the string value to add.
         * @param int index the index of the string value in the source attribute.
         * @return the index assigned to the string, or -1 if the attribute is not
         * of type Attribute.STRING 
         */
        /*@ requires src != null;
            requires 0 <= index && index < src.m_Values.size();
            ensures  isString() && 0 <= \result && \result < m_Values.size() ||
                   ! isString() && \result == -1;
        */
        public int addStringValue(Attribute src, int index)
        {

            if (!isString())
            {
                return -1;
            }
            Object store = src.m_Values.elementAt(index);
            int oldIndex = (int)m_Hashtable[store];
            if (oldIndex != null)
            {
                return oldIndex;
            }
            else
            {
                int intIndex = m_Values.size();
                m_Values.Add(store);
                m_Hashtable.Add(store, intIndex);
                return intIndex;
            }
        }

        /**
         * Adds an attribute value. Creates a fresh list of attribute
         * values before adding it.
         *
         * @param value the attribute value
         */
        void addValue(string value)
        {

            m_Values = (FastVector)m_Values.copy();
            m_Hashtable = (Hashtable)m_Hashtable.Clone();
            forceAddValue(value);
        }

        /**
         * Produces a shallow copy of this attribute with a new name.
         *
         * @param newName the name of the new attribute
         * @return a copy of this attribute with the same index
         */
        //@ requires newName != null;
        //@ ensures \result.m_Name  == newName;
        //@ ensures \result.m_Index == m_Index;
        //@ ensures \result.m_Type  == m_Type;
        /*@ pure non_null @*/
        Attribute copy(string newName)
        {

            Attribute copy = new Attribute(newName);

            copy.m_Index = m_Index;
            copy.m_Type = m_Type;
            copy.m_Values = m_Values;
            copy.m_Hashtable = m_Hashtable;
            copy.setMetadata(m_Metadata);

            return copy;
        }

        /**
         * Removes a value of a nominal or string attribute. Creates a 
         * fresh list of attribute values before removing it.
         *
         * @param index the value's index
         * @exception Exception if the attribute is not nominal
         */
        //@ requires isNominal() || isString();
        //@ requires 0 <= index && index < m_Values.size();
        void delete(int index)
        {

            if (!isNominal() && !isString())
                throw new Exception("Can only remove value of" +
                                                   "nominal or string attribute!");
            else
            {
                m_Values = (FastVector)m_Values.copy();
                m_Values.removeElementAt(index);
                Hashtable hash = new Hashtable(m_Hashtable.Count);
                IEnumerator enu = m_Hashtable.Keys.GetEnumerator();
                while (enu.MoveNext())
                {
                    Object string_ = enu.Current;
                    int valIndexObject = (int)m_Hashtable[string_];
                    int valIndex = valIndexObject;
                    if (valIndex > index)
                    {
                        hash.Add(string_, valIndex - 1);
                    }
                    else if (valIndex < index)
                    {
                        hash.Add(string_, valIndexObject);
                    }
                }
                m_Hashtable = hash;
            }
        }

        /**
         * Adds an attribute value.
         *
         * @param value the attribute value
         */
        //@ requires value != null;
        //@ ensures  m_Values.size() == \old(m_Values.size()) + 1;
        public void forceAddValue(string value)
        {

            Object store = value;
            m_Values.Add(store);
            m_Hashtable.Add(store, m_Values.size() - 1);
        }

        /**
         * Sets the index of this attribute.
         *
         * @param the index of this attribute
         */
        //@ requires 0 <= index;
        //@ assignable m_Index;
        //@ ensures m_Index == index;
        public void setIndex(int index)
        {

            m_Index = index;
        }

        /**
         * Sets a value of a nominal attribute or string attribute.
         * Creates a fresh list of attribute values before it is set.
         *
         * @param index the value's index
         * @param string the value
         * @exception Exception if the attribute is not nominal or 
         * string.
         */
        //@ requires string != null;
        //@ requires isNominal() || isString();
        //@ requires 0 <= index && index < m_Values.size();
        public void setValue(int index, string string_)
        {

            switch (m_Type)
            {
                case NOMINAL:
                case STRING:
                    m_Values = (FastVector)m_Values.copy();
                    m_Hashtable = (Hashtable)m_Hashtable.Clone();
                    Object store = string_;
                    m_Hashtable.Remove(m_Values.elementAt(index));
                    m_Values.setElementAt(store, index);
                    m_Hashtable.Add(store, index);
                    break;
                default:
                    throw new Exception("Can only set values for nominal"
                                                       + " or string attributes!");
            }
        }

        //@ requires isDate();
        public /*@pure@*/ string formatDate(double date)
        {
            switch (m_Type)
            {
                case DATE:
                    return new DateTime((long)date).ToShortDateString();
                default:
                    throw new Exception("Can only format date values for date"
                                                       + " attributes!");
            }
        }

        //@ requires isDate();
        //@ requires string != null;
        public double parseDate(string string_)
        {
            switch (m_Type)
            {
                case DATE:
                    long time = DateTime.Parse(string_).Ticks;
                    // TODO put in a safety check here if we can't store the value in a double.
                    return (double)time;
                default:
                    throw new Exception("Can only parse date values for date"
                                                       + " attributes!");
            }
        }

        /**
         * Returns the properties supplied for this attribute.
         *
         * @return metadata for this attribute
         */
        public  /*@ pure @*/ ProtectedProperties getMetadata()
        {

            return m_Metadata;
        }

        /**
         * Returns the ordering of the attribute. One of the following:
         * 
         * ORDERING_SYMBOLIC - attribute values should be treated as symbols.
         * ORDERING_ORDERED  - attribute values have a global ordering.
         * ORDERING_MODULO   - attribute values have an ordering which wraps.
         *
         * @return the ordering type of the attribute
         */
        public  /*@ pure @*/ int ordering()
        {

            return m_Ordering;
        }

        /**
         * Returns whether the attribute values are equally spaced.
         *
         * @return whether the attribute is regular or not
         */
        public  /*@ pure @*/ bool isRegular()
        {

            return m_IsRegular;
        }

        /**
         * Returns whether the attribute can be averaged meaningfully.
         *
         * @return whether the attribute can be averaged or not
         */
        public  /*@ pure @*/ bool isAveragable()
        {

            return m_IsAveragable;
        }

        /**
         * Returns whether the attribute has a zeropoint and may be
         * added meaningfully.
         *
         * @return whether the attribute has a zeropoint or not
         */
        public  /*@ pure @*/ bool hasZeropoint()
        {

            return m_HasZeropoint;
        }

        /**
         * Returns the attribute's weight.
         *
         * @return the attribute's weight as a double
         */
        public  /*@ pure @*/ double weight()
        {

            return m_Weight;
        }

        /**
         * Returns the lower bound of a numeric attribute.
         *
         * @return the lower bound of the specified numeric range
         */
        public  /*@ pure @*/ double getLowerNumericBound()
        {

            return m_LowerBound;
        }

        /**
         * Returns whether the lower numeric bound of the attribute is open.
         *
         * @return whether the lower numeric bound is open or not (closed)
         */
        public  /*@ pure @*/ bool lowerNumericBoundIsOpen()
        {

            return m_LowerBoundIsOpen;
        }

        /**
         * Returns the upper bound of a numeric attribute.
         *
         * @return the upper bound of the specified numeric range
         */
        public  /*@ pure @*/ double getUpperNumericBound()
        {

            return m_UpperBound;
        }

        /**
         * Returns whether the upper numeric bound of the attribute is open.
         *
         * @return whether the upper numeric bound is open or not (closed)
         */
        public  /*@ pure @*/ bool upperNumericBoundIsOpen()
        {

            return m_UpperBoundIsOpen;
        }

        /**
         * Determines whether a value lies within the bounds of the attribute.
         *
         * @return whether the value is in range
         */
        public  /*@ pure @*/ bool isInRange(double value)
        {

            // dates and missing values are a special case 
            if (m_Type == DATE || value == Instance.missingValue()) return true;
            if (m_Type != NUMERIC)
            {
                // do label range check
                int intVal = (int)value;
                if (intVal < 0 || intVal >= m_Hashtable.Count) return false;
            }
            else
            {
                // do numeric bounds check
                if (m_LowerBoundIsOpen)
                {
                    if (value <= m_LowerBound) return false;
                }
                else
                {
                    if (value < m_LowerBound) return false;
                }
                if (m_UpperBoundIsOpen)
                {
                    if (value >= m_UpperBound) return false;
                }
                else
                {
                    if (value > m_UpperBound) return false;
                }
            }
            return true;
        }

        /**
         * Sets the metadata for the attribute. Processes the strings stored in the
         * metadata of the attribute so that the properties can be set up for the
         * easy-access metadata methods. Any strings sought that are omitted will
         * cause default values to be set.
         * 
         * The following properties are recognised:
         * ordering, averageable, zeropoint, regular, weight, and range.
         *
         * All other properties can be queried and handled appropriately by classes
         * calling the getMetadata() method.
         *
         * @param metadata the metadata
         * @exception Exception if the properties are not consistent
         */
        //@ requires metadata != null;
        private void setMetadata(ProtectedProperties metadata)
        {

            m_Metadata = metadata;

            if (m_Type == DATE)
            {
                m_Ordering = ORDERING_ORDERED;
                m_IsRegular = true;
                m_IsAveragable = false;
                m_HasZeropoint = false;
            }
            else
            {

                // get ordering
                string orderString = m_Metadata.getProperty("ordering", "");

                // numeric ordered attributes are averagable and zeropoint by default
                string def;
                if (m_Type == NUMERIC
                && orderString.CompareTo("modulo") != 0
                && orderString.CompareTo("symbolic") != 0)
                    def = "true";
                else def = "false";

                // determine bool states
                m_IsAveragable =
              (m_Metadata.getProperty("averageable", def).CompareTo("true") == 0);
                m_HasZeropoint =
              (m_Metadata.getProperty("zeropoint", def).CompareTo("true") == 0);
                // averagable or zeropoint implies regular
                if (m_IsAveragable || m_HasZeropoint) def = "true";
                m_IsRegular =
              (m_Metadata.getProperty("regular", def).CompareTo("true") == 0);

                // determine ordering
                if (orderString.CompareTo("symbolic") == 0)
                    m_Ordering = ORDERING_SYMBOLIC;
                else if (orderString.CompareTo("ordered") == 0)
                    m_Ordering = ORDERING_ORDERED;
                else if (orderString.CompareTo("modulo") == 0)
                    m_Ordering = ORDERING_MODULO;
                else
                {
                    if (m_Type == NUMERIC || m_IsAveragable || m_HasZeropoint)
                        m_Ordering = ORDERING_ORDERED;
                    else m_Ordering = ORDERING_SYMBOLIC;
                }
            }

            // consistency checks
            if (m_IsAveragable && !m_IsRegular)
                throw new Exception("An averagable attribute must be"
                               + " regular");
            if (m_HasZeropoint && !m_IsRegular)
                throw new Exception("A zeropoint attribute must be"
                               + " regular");
            if (m_IsRegular && m_Ordering == ORDERING_SYMBOLIC)
                throw new Exception("A symbolic attribute cannot be"
                               + " regular");
            if (m_IsAveragable && m_Ordering != ORDERING_ORDERED)
                throw new Exception("An averagable attribute must be"
                               + " ordered");
            if (m_HasZeropoint && m_Ordering != ORDERING_ORDERED)
                throw new Exception("A zeropoint attribute must be"
                               + " ordered");

            // determine weight
            m_Weight = 1.0;
            string weightString = m_Metadata.getProperty("weight");
            if (weightString != null)
            {
                try
                {
                    m_Weight = double.Parse(weightString);
                }
                catch (Exception e)
                {
                    // Check if value is really a number
                    throw new Exception("Not a valid attribute weight: '"
                                       + weightString + "'");
                }
            }

            // determine numeric range
            if (m_Type == NUMERIC) setNumericRange(m_Metadata.getProperty("range"));
        }

        /**
         * Sets the numeric range based on a string. If the string is null the range
         * will default to [-inf,+inf]. A square brace represents a closed interval, a
         * curved brace represents an open interval, and 'inf' represents infinity.
         * Examples of valid range strings: "[-inf,20)","(-13.5,-5.2)","(5,inf]"
         *
         * @param rangeString the string to parse as the attribute's numeric range
         * @exception Exception if the range is not valid
         */
        //@ requires rangeString != null;
        private void setNumericRange(string rangeString)
        {
            // set defaults
            m_LowerBound = double.NegativeInfinity;
            m_LowerBoundIsOpen = false;
            m_UpperBound = double.PositiveInfinity;
            m_UpperBoundIsOpen = false;

            if (rangeString == null) return;

            //// set up a tokenzier to parse the string
            //StreamTokenizer tokenizer =
            //  new StreamTokenizer(new StringReader(rangeString));
            //tokenizer.resetSyntax();         
            //tokenizer.whitespaceChars(0, ' ');    
            //tokenizer.wordChars(' '+1,'\u00FF');
            //tokenizer.ordinaryChar('[');
            //tokenizer.ordinaryChar('(');
            //tokenizer.ordinaryChar(',');
            //tokenizer.ordinaryChar(']');
            //tokenizer.ordinaryChar(')');

            //try {

            //  // get opening brace
            //  tokenizer.nextToken();

            //  if (tokenizer.ttype == '[') m_LowerBoundIsOpen = false;
            //  else if (tokenizer.ttype == '(') m_LowerBoundIsOpen = true;
            //  else throw new Exception("Expected opening brace on range,"
            //                      + " found: "
            //                      + tokenizer.ToString());

            //  // get lower bound
            //  tokenizer.nextToken();
            //  if (tokenizer.ttype != tokenizer.TT_WORD)
            //throw new Exception("Expected lower bound in range,"
            //                   + " found: "
            //                   + tokenizer.ToString());
            //  if (tokenizer.sval.compareToIgnoreCase("-inf") == 0)
            //m_LowerBound = double.NegativeInfinity;
            //  else if (tokenizer.sval.compareToIgnoreCase("+inf") == 0)
            //m_LowerBound = double.PositiveInfinity;
            //  else if (tokenizer.sval.compareToIgnoreCase("inf") == 0)
            //m_LowerBound = double.NegativeInfinity;
            //  else try {
            //m_LowerBound = double.valueOf(tokenizer.sval).doubleValue();
            //  } catch (NumberFormatException e) {
            //throw new Exception("Expected lower bound in range,"
            //                   + " found: '" + tokenizer.sval + "'");
            //  }

            //  // get separating comma
            //  if (tokenizer.nextToken() != ',')
            //throw new Exception("Expected comma in range,"
            //                   + " found: "
            //                   + tokenizer.ToString());

            //  // get upper bound
            //  tokenizer.nextToken();
            //  if (tokenizer.ttype != tokenizer.TT_WORD)
            //throw new Exception("Expected upper bound in range,"
            //                   + " found: "
            //                   + tokenizer.ToString());
            //  if (tokenizer.sval.compareToIgnoreCase("-inf") == 0)
            //m_UpperBound = double.NegativeInfinity;
            //  else if (tokenizer.sval.compareToIgnoreCase("+inf") == 0)
            //m_UpperBound = double.PositiveInfinity;
            //  else if (tokenizer.sval.compareToIgnoreCase("inf") == 0)
            //m_UpperBound = double.PositiveInfinity;
            //  else try {
            //m_UpperBound = double.valueOf(tokenizer.sval).doubleValue();
            //  } catch (NumberFormatException e) {
            //throw new Exception("Expected upper bound in range,"
            //                   + " found: '" + tokenizer.sval + "'");
            //  }

            //  // get closing brace
            //  tokenizer.nextToken();

            //  if (tokenizer.ttype == ']') m_UpperBoundIsOpen = false;
            //  else if (tokenizer.ttype == ')') m_UpperBoundIsOpen = true;
            //  else throw new Exception("Expected closing brace on range,"
            //                      + " found: "
            //                      + tokenizer.ToString());

            //  // check for rubbish on end
            //  if (tokenizer.nextToken() != tokenizer.TT_EOF)
            //throw new Exception("Expected end of range string,"
            //                   + " found: "
            //                   + tokenizer.ToString());

            //} catch (IOException e) {
            //  throw new Exception("IOException reading attribute range"
            //                 + " string: " + e.getMessage());
            //}

            //if (m_UpperBound < m_LowerBound)
            //  throw new Exception("Upper bound (" + m_UpperBound
            //                 + ") on numeric range is"
            //                 + " less than lower bound ("
            //                 + m_LowerBound + ")!");
        }

        /**
         * Simple main method for testing this class.
         */
        //@ requires ops != null;
        //@ requires \nonnullelements(ops);
        public static void main(string[] ops)
        {

            try
            {

                // Create numeric attributes "Length" and "weight"
                Attribute Length = new Attribute("Length");
                Attribute weight = new Attribute("weight");

                // Create date attribute "date"
                Attribute date = new Attribute("date", "yyyy-MM-dd HH:mm:ss");

                PrintToScreen.WriteLine(date);
                double dd = date.parseDate("2001-04-04 14:13:55");
                PrintToScreen.WriteLine("UnitTest date = " + dd);
                PrintToScreen.WriteLine(date.formatDate(dd));

                dd = DateTime.Now.Ticks;
                PrintToScreen.WriteLine("DateWrapper now = " + dd);
                PrintToScreen.WriteLine(date.formatDate(dd));

                // Create vector to hold nominal values "first", "second", "third" 
                FastVector my_nominal_values = new FastVector(3);
                my_nominal_values.Add("first");
                my_nominal_values.Add("second");
                my_nominal_values.Add("third");

                // Create nominal attribute "position" 
                Attribute position = new Attribute("position", my_nominal_values);

                // Print the name of "position"
                PrintToScreen.WriteLine("Name of \"position\": " + position.name());

                // Print the values of "position"
                FastVector.FastVectorEnumeration attValues = position.enumerateValues();
                while (attValues.hasMoreElements())
                {
                    string string_ = (string)attValues.nextElement();
                    PrintToScreen.WriteLine("Value of \"position\": " + string_);
                }

                // Shallow copy attribute "position"
                Attribute copy = (Attribute)position.copy();

                // UnitTest if attributes are the same
                PrintToScreen.WriteLine("Copy is the same as original: " + copy.Equals(position));

                // Print index of attribute "weight" (should be unset: -1)
                PrintToScreen.WriteLine("Index of attribute \"weight\" (should be -1): " +
                       weight.index());

                // Print index of value "first" of attribute "position"
                PrintToScreen.WriteLine("Index of value \"first\" of \"position\" (should be 0): " +
                       position.indexOfValue("first"));

                // Tests type of attribute "position"
                PrintToScreen.WriteLine("\"position\" is numeric: " + position.isNumeric());
                PrintToScreen.WriteLine("\"position\" is nominal: " + position.isNominal());
                PrintToScreen.WriteLine("\"position\" is string: " + position.isString());

                // Prints name of attribute "position"
                PrintToScreen.WriteLine("Name of \"position\": " + position.name());

                // Prints number of values of attribute "position"
                PrintToScreen.WriteLine("Number of values for \"position\": " + position.numValues());

                // Prints the values (againg)
                for (int i = 0; i < position.numValues(); i++)
                {
                    PrintToScreen.WriteLine("Value " + i + ": " + position.value(i));
                }

                // Prints the attribute "position" in ARFF format
                PrintToScreen.WriteLine(position);

                // Checks type of attribute "position" using constants
                switch (position.type())
                {
                    case Attribute.NUMERIC:
                        PrintToScreen.WriteLine("\"position\" is numeric");
                        break;
                    case Attribute.NOMINAL:
                        PrintToScreen.WriteLine("\"position\" is nominal");
                        break;
                    case Attribute.STRING:
                        PrintToScreen.WriteLine("\"position\" is string");
                        break;
                    case Attribute.DATE:
                        PrintToScreen.WriteLine("\"position\" is date");
                        break;
                    default:
                        PrintToScreen.WriteLine("\"position\" has unknown type");
                        break;
                }
            }
            catch (Exception e)
            {
                PrintToScreen.WriteLine(e.StackTrace);
            }
        }
    }

}
