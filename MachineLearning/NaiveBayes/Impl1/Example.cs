using System;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * An object to hold training or test examples for categorization.
     * Stores the name, category and HashMapVector representation of
     * the example.
     *
     * @author Sugato Basu, Prem Melville, and Ray Mooney
     */

    public class Example
    {
        /**
         * Name of the example
         */

        /**
         * Category index of the example
         */
        protected int category;

        /**
         * Representation of the example as a vector of (feature -> weight) mappings
         */

        /**
         * fileDocument object for the example
         */
        protected AFileDocument m_document;
        protected HashMapVector hashVector;
        public string name;

        public Example(HashMapVector input, int cat, string id, AFileDocument doc)
        {
            hashVector = input;
            category = cat;
            name = id;
            m_document = doc;
        }

        /**
         * Sets the name of the example
         */

        public void setName(string id)
        {
            name = id;
        }

        /**
         * Returns the name of the example
         */

        public string getName()
        {
            return name;
        }

        /**
         * Sets the category of the example
         */

        public void setCategory(int cat)
        {
            category = cat;
        }

        /**
         * Returns the category of the example
         */

        public int getCategory()
        {
            return category;
        }

        /**
         * Sets the hashVector of the example
         */

        public void setHashMapVector(HashMapVector v)
        {
            hashVector = v;
        }

        /**
         * Returns the hashVector of the example
         */

        public HashMapVector getHashMapVector()
        {
            return hashVector;
        }

        /**
         * Sets the document of the example
         */

        public void setDocument(AFileDocument doc)
        {
            m_document = doc;
        }

        /**
         * Returns the document of the example
         */

        public AFileDocument getDocument()
        {
            return m_document;
        }

        /**
         * Returns the string representation of the example object
         */

        public string toString()
        {
            string str;
            str = "Name:" + name + ", Category: " + category + Environment.NewLine;
            return str;
        }
    }
}
