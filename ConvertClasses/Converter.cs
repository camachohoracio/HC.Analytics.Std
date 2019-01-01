#region

using System;
using HC.Analytics.Mathematics;
using HC.Core.Helpers;
using HC.Core.Time;

#endregion

namespace HC.Analytics.ConvertClasses
{
    public class Converter
    {
        private static bool m_blnSuppressMessage; // if true lack of precision messages are suppressed

        private static bool m_blnSuppressMessageAM;
        // for use with ArrayMaths - allows suppression for all instances of ArrayMaths

        private static double max_byte_as_double = byte.MaxValue;
        private static double max_byte_as_float = byte.MaxValue;
        private static double max_byte_as_int = byte.MaxValue;
        private static double max_byte_as_long = byte.MaxValue;
        private static double max_byte_as_short = byte.MaxValue;

        //private static int type = -1;      // 0 double, 1 Double, 2 long, 3 long, 4 float, 5 Float, 6 int, 7 int, 8 short, 9 Short, 10 byte, 11 Byte
        // 12 BigDecimal, 13 BigInteger, 14 Complex, 15 Phasor, 16 char, 17 Character, 18 string

        private static double max_float_as_double = float.MaxValue;
        private static double max_int_as_double = int.MaxValue;
        private static double max_int_as_float = int.MaxValue;
        private static double max_int_as_long = int.MaxValue;
        private static double max_long_as_double = long.MaxValue;
        private static double max_long_as_float = long.MaxValue;
        private static double max_short_as_double = short.MaxValue;
        private static double max_short_as_float = short.MaxValue;
        private static double max_short_as_int = short.MaxValue;
        private static double max_short_as_long = short.MaxValue;

        private static string[] typeName = {
                                               "double", "Double", "long", "long", "float", "Float", "int", "int",
                                               "short",
                                               "Short", "byte", "Byte", "BigDecimal", "BigInteger", "Complex", "Phasor",
                                               "char", "Character", "string"
                                           };

        #region Constructors

        // CONSTRUCTORS

        #endregion

        public static decimal convert_float_to_decimal(float hold3)
        {
            return (decimal) hold3;
        }

        public static long convert_decimal_to_long(decimal hold12)
        {
            return (long) hold12;
        }

        public static double convert_decimal_to_double(decimal p)
        {
            return (double) p;
        }

        public static byte convert_int_to_byte(long p)
        {
            return (byte) p;
        }

        public static float convert_decimal_to_float(decimal p)
        {
            return (float) p;
        }

        public static int convert_double_to_integer(double p)
        {
            return (int) p;
        }

        public static int convert_decimal_to_int(decimal p)
        {
            return (int) p;
        }

        public static short convert_decimal_to_short(decimal p)
        {
            return (short) p;
        }

        public static short convert_int_to_short(long p)
        {
            return (short) p;
        }

        public static byte convert_decimal_to_byte(decimal p)
        {
            return (byte) p;
        }

        // RECAST
        // double and Double -> . . .
        public static float convert_double_to_float(double x)
        {
            if (x > max_float_as_double)
            {
                throw new ArgumentException("double is too large to be recast as float");
            }
            if (!m_blnSuppressMessage)
            {
                PrintToScreen.WriteLine("Class Conv: method convert_double_to_float: possible loss of precision");
            }
            return (float) ((x));
        }

        // float and Float -> . . .
        public static double convert_float_to_double(float x)
        {
            return ((x));
        }

        public static long convert_float_to_long(float x)
        {
            if (x > max_long_as_float)
            {
                throw new ArgumentException("float is too large to be recast as long");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("float is not, arithmetically, an integer");
            }
            return (long) ((x));
        }

        public static int convert_float_to_int(float x)
        {
            if (x > max_int_as_float)
            {
                throw new ArgumentException("double is too large to be recast as int");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("double is not, arithmetically, an integer");
            }
            return (int) ((x));
        }

        public static short convert_float_to_short(float x)
        {
            if (x > max_short_as_float)
            {
                throw new ArgumentException("float is too large to be recast as short");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("float is not, arithmetically, an integer");
            }
            return (short) ((x));
        }

        public static byte convert_float_to_byte(float x)
        {
            if (x > max_byte_as_float)
            {
                throw new ArgumentException("float is too large to be recast as byte");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("float is not, arithmetically, an integer");
            }
            return (byte) ((x));
        }

        // long and long -> . . .
        public static double convert_long_to_double(long x)
        {
            if (!m_blnSuppressMessage)
            {
                PrintToScreen.WriteLine("Class Conv: method convert_long_to_double: possible loss of precision");
            }
            return ((x));
        }

        public static float convert_long_to_float(long x)
        {
            if (!m_blnSuppressMessage)
            {
                PrintToScreen.WriteLine("Class Conv: method convert_long_to_float: possible loss of precision");
            }
            return ((x));
        }

        public static int convert_long_to_int(long x)
        {
            if (x > max_int_as_long)
            {
                throw new ArgumentException("long is too large to be recast as int");
            }
            return (int) ((x));
        }

        public static short convert_long_to_short(long x)
        {
            if (x > max_short_as_long)
            {
                throw new ArgumentException("long is too large to be recast as short");
            }
            return (short) ((x));
        }

        public static byte convert_long_to_byte(long x)
        {
            if (x > max_byte_as_long)
            {
                throw new ArgumentException("long is too large to be recast as byte");
            }
            return (byte) ((x));
        }

        // int and int -> . . .
        public static double convert_int_to_double(int x)
        {
            return ((x));
        }

        public static float convert_int_to_float(int x)
        {
            if (!m_blnSuppressMessage)
            {
                PrintToScreen.WriteLine("Class Conv: method convert_int_to_float: possible loss of precision");
            }
            return ((x));
        }

        public static long convert_int_to_long(int x)
        {
            return ((x));
        }

        public static short convert_int_to_short(int x)
        {
            if (x > max_short_as_int)
            {
                throw new ArgumentException("int is too large to be recast as short");
            }
            return (short) ((x));
        }

        public static byte convert_int_to_byte(int x)
        {
            if (x > max_byte_as_int)
            {
                throw new ArgumentException("int is too large to be recast as byte");
            }
            return (byte) ((x));
        }

        // short and Short -> . . .
        public static double convert_short_to_double(short x)
        {
            return ((x));
        }

        public static float convert_short_to_float(short x)
        {
            return ((x));
        }

        public static long convert_short_to_long(short x)
        {
            return ((x));
        }

        public static int convert_short_to_int(short x)
        {
            return ((x));
        }

        public static byte convert_short_to_byte(short x)
        {
            if (x > max_byte_as_short)
            {
                throw new ArgumentException("short is too large to be recast as byte");
            }
            return (byte) ((x));
        }

        // byte and Byte -> . . .
        public static double convert_byte_to_double(byte x)
        {
            return ((x));
        }

        public static float convert_byte_to_float(byte x)
        {
            return ((x));
        }

        public static long convert_byte_to_long(byte x)
        {
            return ((x));
        }

        public static int convert_byte_to_int(byte x)
        {
            return ((x));
        }

        public static short convert_byte_to_short(byte x)
        {
            return ((x));
        }


        // LOSS OF PRECISION MESSAGE
        // Suppress loss of precision messages
        public static void suppressMessages()
        {
            m_blnSuppressMessage = true;
        }

        // Restore loss of precision messages
        public static void restoreMessages()
        {
            if (!m_blnSuppressMessageAM)
            {
                m_blnSuppressMessage = false;
            }
        }

        //  For use of ArrayMaths - suppression for all ArrayMaths instances
        public static void suppressMessagesAM()
        {
            m_blnSuppressMessageAM = true;
        }

        // For use of ArrayMaths - restore total loss of precision messages
        public static void restoreMessagesAM()
        {
            m_blnSuppressMessageAM = false;
        }


        // UNIT CONVERSIONS

        // Converts radians to degrees
        public static double radToDeg(double rad)
        {
            return rad*180.0D/Math.PI;
        }

        // Converts degrees to radians
        public static double degToRad(double deg)
        {
            return deg*Math.PI/180.0D;
        }

        // Converts frequency (Hz) to radial frequency
        public static double frequencyToRadialFrequency(double frequency)
        {
            return 2.0D*Math.PI*frequency;
        }

        // Converts radial frequency to frequency (Hz)
        public static double radialFrequencyToFrequency(double radial)
        {
            return radial/(2.0D*Math.PI);
        }

        // Converts electron volts(eV) to corresponding wavelength in nm
        public static double evToNm(double ev)
        {
            return 1e+9*Fmath.C_LIGHT/(-ev*Fmath.Q_ELECTRON/Fmath.H_PLANCK);
        }

        // Converts wavelength in nm to matching energy in eV
        public static double nmToEv(double nm)
        {
            return Fmath.C_LIGHT/(-nm*1e-9)*Fmath.H_PLANCK/Fmath.Q_ELECTRON;
        }

        // Converts moles per litre to percentage weight by volume
        public static double molarToPercentWeightByVol(double molar, double molWeight)
        {
            return molar*molWeight/10.0D;
        }

        // Converts percentage weight by volume to moles per litre
        public static double percentWeightByVolToMolar(double perCent, double molWeight)
        {
            return perCent*10.0D/molWeight;
        }

        // Converts Celsius to Kelvin
        public static double celsiusToKelvin(double cels)
        {
            return cels - Fmath.T_ABS;
        }

        // Converts Kelvin to Celsius
        public static double kelvinToCelsius(double kelv)
        {
            return kelv + Fmath.T_ABS;
        }

        // Converts Celsius to Fahrenheit
        public static double celsiusToFahren(double cels)
        {
            return cels*(9.0/5.0) + 32.0;
        }

        // Converts Fahrenheit to Celsius
        public static double fahrenToCelsius(double fahr)
        {
            return (fahr - 32.0)*5.0/9.0;
        }

        // Converts calories to Joules
        public static double calorieToJoule(double cal)
        {
            return cal*4.1868;
        }

        // Converts Joules to calories
        public static double jouleToCalorie(double joule)
        {
            return joule*0.23884;
        }

        // Converts grams to ounces
        public static double gramToOunce(double gm)
        {
            return gm/28.3459;
        }

        // Converts ounces to grams
        public static double ounceToGram(double oz)
        {
            return oz*28.3459;
        }

        // Converts kilograms to pounds
        public static double kgToPound(double kg)
        {
            return kg/0.4536;
        }

        // Converts pounds to kilograms
        public static double poundToKg(double pds)
        {
            return pds*0.4536;
        }

        // Converts kilograms to tons
        public static double kgToTon(double kg)
        {
            return kg/1016.05;
        }

        /**
         * Converts an angle measured in radians to an approximately
         * equivalent angle measured in degrees.  The conversion from
         * radians to degrees is generally inexact; users should
         * <i>not</i> expect <code>cos(toRadians(90.0))</code> to exactly
         * equal <code>0.0</code>.
         *
         * @param   angrad   an angle, in radians
         * @return  the measurement of the angle <code>angrad</code>
         *          in degrees.
         * @since   1.2
         */

        public static double toDegrees(double angrad)
        {
            return angrad*180.0/Math.PI;
        }

        // Converts tons to kilograms
        public static double tonToKg(double tons)
        {
            return tons*1016.05;
        }

        // Converts millimetres to inches
        public static double millimetreToInch(double mm)
        {
            return mm/25.4;
        }

        // Converts inches to millimetres
        public static double inchToMillimetre(double inches)
        {
            return inches*25.4;
        }

        // Converts feet to metres
        public static double footToMetre(double ft)
        {
            return ft*0.3048;
        }

        // Converts metres to feet
        public static double metreToFoot(double metre)
        {
            return metre/0.3048;
        }

        // Converts yards to metres
        public static double yardToMetre(double yd)
        {
            return yd*0.9144;
        }

        // Converts metres to yards
        public static double metreToYard(double metre)
        {
            return metre/0.9144;
        }

        // Converts miles to kilometres
        public static double mileToKm(double mile)
        {
            return mile*1.6093;
        }

        // Converts kilometres to miles
        public static double kmToMile(double km)
        {
            return km/1.6093;
        }

        // Converts UK gallons to litres
        public static double gallonToLitre(double gall)
        {
            return gall*4.546;
        }

        // Converts litres to UK gallons
        public static double litreToGallon(double litre)
        {
            return litre/4.546;
        }

        // Converts UK quarts to litres
        public static double quartToLitre(double quart)
        {
            return quart*1.137;
        }

        // Converts litres to UK quarts
        public static double litreToQuart(double litre)
        {
            return litre/1.137;
        }

        // Converts UK pints to litres
        public static double pintToLitre(double pint)
        {
            return pint*0.568;
        }

        // Converts litres to UK pints
        public static double litreToPint(double litre)
        {
            return litre/0.568;
        }

        // Converts UK gallons per mile to litres per kilometre
        public static double gallonPerMileToLitrePerKm(double gallPmile)
        {
            return gallPmile*2.825;
        }

        // Converts litres per kilometre to UK gallons per mile
        public static double litrePerKmToGallonPerMile(double litrePkm)
        {
            return litrePkm/2.825;
        }

        // Converts miles per UK gallons to kilometres per litre
        public static double milePerGallonToKmPerLitre(double milePgall)
        {
            return milePgall*0.354;
        }

        // Converts kilometres per litre to miles per UK gallons
        public static double kmPerLitreToMilePerGallon(double kmPlitre)
        {
            return kmPlitre/0.354;
        }

        // Converts UK fluid ounce to American fluid ounce
        public static double fluidOunceUKtoUS(double flOzUK)
        {
            return flOzUK*0.961;
        }

        // Converts American fluid ounce to UK fluid ounce
        public static double fluidOunceUStoUK(double flOzUS)
        {
            return flOzUS*1.041;
        }

        // Converts UK pint to American liquid pint
        public static double pintUKtoUS(double pintUK)
        {
            return pintUK*1.201;
        }

        // Converts American liquid pint to UK pint
        public static double pintUStoUK(double pintUS)
        {
            return pintUS*0.833;
        }

        // Converts UK quart to American liquid quart
        public static double quartUKtoUS(double quartUK)
        {
            return quartUK*1.201;
        }

        // Converts American liquid quart to UK quart
        public static double quartUStoUK(double quartUS)
        {
            return quartUS*0.833;
        }

        // Converts UK gallon to American gallon
        public static double gallonUKtoUS(double gallonUK)
        {
            return gallonUK*1.201;
        }

        // Converts American gallon to UK gallon
        public static double gallonUStoUK(double gallonUS)
        {
            return gallonUS*0.833;
        }

        // Converts UK pint to American cup
        public static double pintUKtoCupUS(double pintUK)
        {
            return pintUK/0.417;
        }

        // Converts American cup to UK pint
        public static double cupUStoPintUK(double cupUS)
        {
            return cupUS*0.417;
        }

        // Calculates body mass index (BMI) from height (m) and weight (kg)
        public static double calcBMImetric(double height, double weight)
        {
            return weight/(height*height);
        }

        // Calculates body mass index (BMI) from height (ft) and weight (lbs)
        public static double calcBMIimperial(double height, double weight)
        {
            height = Fmath.footToMetre(height);
            weight = Fmath.poundToKg(weight);
            return weight/(height*height);
        }

        // Calculates weight (kg) to give a specified BMI for a given height (m)
        public static double calcWeightFromBMImetric(double bmi, double height)
        {
            return bmi*height*height;
        }

        // Calculates weight (lbs) to give a specified BMI for a given height (ft)
        public static double calcWeightFromBMIimperial(double bmi, double height)
        {
            height = Fmath.footToMetre(height);
            double weight = bmi*height*height;
            weight = Fmath.kgToPound(weight);
            return weight;
        }

        // Returns milliseconds since 0 hours 0 minutes 0 seconds on 1 Jan 1970
        public static long dateToJavaMilliSecondsUK(int year, int month, int day, string dayOfTheWeek, int hour, int min,
                                                    int sec, int millisec)
        {
            TimeAndDate tad = new TimeAndDate();
            long ms = tad.dateToJavaMilliSecondsUK(year, month, day, dayOfTheWeek, hour, min, sec, millisec);

            return ms;
        }


        /**
         * Converts an angle measured in degrees to an approximately
         * equivalent angle measured in radians.  The conversion from
         * degrees to radians is generally inexact.
         *
         * @param   angdeg   an angle, in degrees
         * @return  the measurement of the angle <code>angdeg</code>
         *          in radians.
         * @since   1.2
         */

        public static double toRadians(double angdeg)
        {
            return angdeg/180.0*Math.PI;
        }

        /**
         * Returns the <code>double</code> value that is closest in value
         * to the argument and is equal to a mathematical integer. If two
         * <code>double</code> values that are mathematical integers are
         * equally close to the value of the argument, the result is the
         * integer value that is even. Special cases:
         * <ul><li>If the argument value is already equal to a mathematical 
         * integer, then the result is the same as the argument. 
         * <li>If the argument is NaN or an infinity or positive zero or negative 
         * zero, then the result is the same as the argument.</ul>
         *
         * @param   a   a value.
         * @return  the closest floating-point value to <code>a</code> that is
         *          equal to a mathematical integer.
         * @author Joseph D. Darcy
         */

        public static double rint(double a)
        {
            /*
             * If the absolute value of a is not less than 2^52, it
             * is either a finite integer (the double format does not have
             * enough significand bits for a number that large to have any
             * fractional portion), an infinity, or a NaN.  In any of
             * these cases, rint of the argument is the argument.
             *
             * Otherwise, the sum (twoToThe52 + a ) will properly Round
             * away any fractional portion of a since ulp(twoToThe52) ==
             * 1.0; subtracting out twoToThe52 from this sum will then be
             * exact and leave the rounded integer portion of a.
             *
             * This method does *not* need to be declared strictfp to get
             * fully reproducible results.  Whether or not a method is
             * declared strictfp can only make a difference in the
             * returned result if some operation would overflow or
             * underflow with strictfp semantics.  The operation
             * (twoToThe52 + a ) cannot overflow since large values of a
             * are screened out; the Add cannot underflow since twoToThe52
             * is too large.  The subtraction ((twoToThe52 + a ) -
             * twoToThe52) will be exact as discussed above and thus
             * cannot overflow or meaningfully underflow.  Finally, the
             * last multiply in the return statement is by plus or minus
             * 1.0, which is exact too.
             */
            double twoToThe52 = (1L << 52); // 2^52

            double sign = MathHelper.RawCopySign(1.0, a); // preserve sign info
            a = Math.Abs(a);

            if (a < twoToThe52)
            {
                // E_min <= ilogb(a) <= 51
                a = ((twoToThe52 + a) - twoToThe52);
            }

            return sign*a; // restore original sign
        }

        public static long convert_double_to_long(double x)
        {
            if (x > max_long_as_double)
            {
                throw new ArgumentException("double is too large to be recast as long");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("double is not, arithmetically, an integer");
            }
            return (long) ((x));
        }

        public static int convert_double_to_int(double x)
        {
            if (x > max_int_as_double)
            {
                throw new ArgumentException("double is too large to be recast as int");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("double is not, arithmetically, an integer");
            }
            return (int) ((x));
        }

        public static short convert_double_to_short(double x)
        {
            if (x > max_short_as_double)
            {
                throw new ArgumentException("double is too large to be recast as short");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("double is not, arithmetically, an integer");
            }
            return (short) ((x));
        }

        public static byte convert_double_to_byte(double x)
        {
            if (x > max_byte_as_double)
            {
                throw new ArgumentException("double is too large to be recast as byte");
            }
            if (!Fmath.isInteger(x))
            {
                throw new ArgumentException("double is not, arithmetically, an integer");
            }
            return (byte) ((x));
        }


        public static decimal convert_double_to_decimal(double hold1)
        {
            return (decimal) hold1;
        }
    }
}
