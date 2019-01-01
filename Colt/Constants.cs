#region

using System;

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
//package cern.jet.math;

/**
 * Defines some useful constants.
 */

    [Serializable]
    public class Constants
    {
        /*
   * machine constants
   */
        public static double big = 4.503599627370496e15;
        public static double biginv = 2.22044604925031308085e-16;
        public static double LOGPI = 1.14472988584940017414;
        public static double MACHEP = 1.11022302462515654042E-16;
        public static double MAXGAM = 171.624376956302725;
        public static double MAXLOG = 7.09782712893383996732E2;
        public static double MINLOG = -7.451332191019412076235E2;

        /**
     * Constant specifying the overflow bin (can be passed to any method expecting a bin number).
     */
        public static int OVERFLOW = -1;
        /**
     * Constant specifying the underflow bin (can be passed to any method expecting a bin number).
     */

        public static long serialVersionUID = 1020;
        public static double SQRTH = 7.07106781186547524401E-1;
        public static double SQTPI = 2.50662827463100050242E0;
        public static int UNDERFLOW = -2;

        /*
 * MACHEP =  1.38777878078144567553E-17       2**-56
 * MAXLOG =  8.8029691931113054295988E1       log(2**127)
 * MINLOG = -8.872283911167299960540E1        log(2**-128)
 * MAXNUM =  1.701411834604692317316873e38    2**127
 *
 * For IEEE arithmetic (IBMPC):
 * MACHEP =  1.11022302462515654042E-16       2**-53
 * MAXLOG =  7.09782712893383996843E2         log(2**1024)
 * MINLOG = -7.08396418532264106224E2         log(2**-1022)
 * MAXNUM =  1.7976931348623158E308           2**1024
 *
 * The global symbols for mathematical constants are
 * PI     =  3.14159265358979323846           pi
 * PIO2   =  1.57079632679489661923           pi/2
 * PIO4   =  7.85398163397448309616E-1        pi/4
 * SQRT2  =  1.41421356237309504880           sqrt(2)
 * SQRTH  =  7.07106781186547524401E-1        sqrt(2)/2
 * LOG2E  =  1.4426950408889634073599         1/log(2)
 * SQ2OPI =  7.9788456080286535587989E-1      sqrt( 2/pi )
 * LOGE2  =  6.93147180559945309417E-1        log(2)
 * LOGSQ2 =  3.46573590279972654709E-1        log(2)/2
 * THPIO4 =  2.35619449019234492885           3*pi/4
 * TWOOPI =  6.36619772367581343075535E-1     2/pi
 */

/**
 * Makes this class non instantiable, but still let's others inherit from it.
 */
    }
}
