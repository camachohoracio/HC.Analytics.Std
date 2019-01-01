using System;
using System.Collections;
using System.Collections.Generic;
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
 *    ProtectedProperties.java
 *    Copyright (C) 2001 Richard Kirkby
 *
 */

//package weka.core;

//import java.util.Properties;
//import java.util.Map;
//import java.util.IEnumerator;
//import java.io.InputStream;

/**
 * Simple class that extends the Properties class so that the properties are
 * unable to be modified.
 *
 * @author Richard Kirkby (rkirkby@cs.waikato.ac.nz)
 * @version $Revision: 1.2 $
 */

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl2_Weka
{

    public class ProtectedProperties
    {

        // the properties need to be open during construction of the object
        private bool closed = false;
        private Dictionary<object, object> m_props;
        /**
         * Creates a set of protected properties from a set of normal ones.
         *
         * @param props the properties to be stored and protected.
         */
        public ProtectedProperties(Dictionary<object, object> props)
        {
            m_props = new Dictionary<object, object>();
            IEnumerator propEnum = props.Keys.GetEnumerator();
            while (propEnum.MoveNext())
            {
                string propName = (string)propEnum.Current;
                string propValue = (string)props[propName];
                m_props.Add(propName, propValue);
            }
            closed = true; // no modifications allowed from now on
        }

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        public Object setProperty(string key, string value)
        {

            if (closed)
                throw new
              Exception("ProtectedProperties cannot be modified!");
            else
            {
                m_props.Add(key, value);
                return null;
            }
        }

        public string getProperty(string key, string defaultValue)
        {
            string val = getProperty(key);
            return (val == null) ? defaultValue : val;
        }


        public string getProperty(string key)
        {
            Object oval = m_props[key];
            string sval = (oval is string) ? (string)oval : null;
            return sval;
        }

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        //public void load(InputStream inStream) {

        //  throw new
        //    Exception("ProtectedProperties cannot be modified!");
        //}

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        public void clear()
        {

            throw new
              Exception("ProtectedProperties cannot be modified!");
        }

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        public Object put(Object key,
                  Object value)
        {

            if (closed)
                throw new
              Exception("ProtectedProperties cannot be modified!");
            else
            {
                m_props.Add(key, value);
                return null;
            }
        }

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        public void putAll(object t)
        {

            throw new
              Exception("ProtectedProperties cannot be modified!");
        }

        /**
         * Overrides a method to prevent the properties from being modified.
         *
         * @return never returns without throwing an exception.
         * @exception UnsupportedOperationException always.
         */
        public Object remove(Object key)
        {

            throw new
              Exception("ProtectedProperties cannot be modified!");
        }

    }

}
