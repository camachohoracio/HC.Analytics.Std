namespace HC.Analytics.Colt
{
    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package impl;

    /**
     * Formats a double into a string (like sprintf in C).
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 21/07/00
     * @see IComparer
     * @see cern.colt
     * @see Sorting
     */

    public interface Former
    {
        /** 
         * Formats a double into a string (like sprintf in C).
         * @param x the number to format
         * @return the formatted string 
         * @exception ArgumentException if bad argument
         */
        string form(double value);
    }
}
