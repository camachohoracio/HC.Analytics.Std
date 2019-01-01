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
//package cern.clhep;

/**
 * High Energy Physics coherent system of Units.
 * This class is a Java port of the <a href="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Units/SystemOfUnits_h.html">C++ version</a> found in <a href="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</a>, which in turn has been provided by Geant4 (a simulation toolkit for HEP).
 * 
 * @author wolfgang.hoschek@cern.ch
 * @version 1.0, 09/24/99
 */

    [Serializable]
    public class Units
    {
        public static double ampere = coulomb/second; // ampere = 6.24150 e+9 * eplus/ns
        public static double angstrom = 1.0e-10*meter;
        public static double atmosphere = 101325*pascal; // atm    = 6.32420 e+8 * MeV/mm3
        public static double bar = 100000*pascal; // bar    = 6.24150 e+8 * MeV/mm3
        public static double barn = 1.0e-28*meter2;
        public static double becquerel = 1.0/second;
        public static double candela = 1.0;
        public static double centimeter = 10.0*millimeter;
        public static double centimeter2 = centimeter*centimeter;
        public static double centimeter3 = centimeter*centimeter*centimeter;

        public static double cm = centimeter;
        public static double cm2 = centimeter2;
        public static double cm3 = centimeter3;
        public static double coulomb = eplus/e_SI; // coulomb = 6.24150 e+18 * eplus
        public static double curie = 3.7e+10*becquerel;

        public static double deg = degree;
        public static double degree = (Math.PI/180.0)*radian; //(3.14159265358979323846/180.0)*radian;

//
// Time [T]
//
        public static double e_SI = 1.60217733e-19; // positron charge in coulomb
        public static double electronvolt = 1e-6*megaelectronvolt;
        public static double eplus = 1.0; // positron charge
        public static double eV = electronvolt;
        public static double farad = coulomb/volt; // farad = 6.24150e+24 * eplus/Megavolt
        public static double fermi = 1.0e-15*meter;
        public static double g = gram;
        public static double gauss = 1.0e-4*tesla;
        public static double GeV = gigaelectronvolt;
        public static double gigaelectronvolt = 1e+3*megaelectronvolt;
        public static double gram = 1e-3*kilogram;
        public static double gray = joule/kilogram;
        public static double henry = weber/ampere; // henry = 1.60217e-7*MeV*(ns/eplus)**2
        public static double hep_pascal = newton/m2; // pascal = 6.24150 e+3 * MeV/mm3
        public static double hertz = 1/second;
        public static double joule = electronvolt/e_SI; // joule = 6.24150 e+12 * MeV
        public static double kelvin = 1.0;
        public static double keV = kiloelectronvolt;

// symbols
        public static double kg = kilogram;
        public static double kiloelectronvolt = 1e-3*megaelectronvolt;
        public static double kilogauss = 1.0e-1*tesla;
        public static double kilogram = joule*second*second/(meter*meter);
        public static double kilohertz = 1e+3*hertz;
        public static double kilometer = 1000.0*meter;
        public static double kilometer2 = kilometer*kilometer;
        public static double kilometer3 = kilometer*kilometer*kilometer;
        public static double kilovolt = 1.0e-3*megavolt;
        public static double km = kilometer;
        public static double km2 = kilometer2;
        public static double km3 = kilometer3;
        public static double lumen = candela*steradian;

//
// Illuminance [I,L^-2]
//
        public static double lux = lumen/meter2;
        public static double m = meter;
        public static double m2 = meter2;
        public static double m3 = meter3;
        public static double megaelectronvolt = 1;
        public static double megahertz = 1e+6*hertz;
        public static double megavolt = megaelectronvolt/eplus;
        public static double meter = 1000.0*millimeter;
        public static double meter2 = meter*meter;
        public static double meter3 = meter*meter*meter;
        public static double MeV = megaelectronvolt;
        public static double mg = milligram;

//
// Power [E,T^-1]
//
        public static double microampere = 1.0e-6*ampere;
        public static double microbarn = 1.0e-6*barn;
        public static double microfarad = 1.0e-6*farad;
        public static double micrometer = 1.0e-6*meter;
        public static double microsecond = 1e-6*second;
        public static double milliampere = 1.0e-3*ampere;
        public static double millibarn = 1.0e-3*barn;
        public static double millifarad = 1.0e-3*farad;
        public static double milligram = 1e-3*gram;
        public static double millimeter = 1.0;
        public static double millimeter2 = millimeter*millimeter;
        public static double millimeter3 = millimeter*millimeter*millimeter;
        public static double milliradian = 1.0e-3*radian;
        public static double millisecond = 1e-3*second;
        public static double mm = millimeter;
        public static double mm2 = millimeter2;
        public static double mm3 = millimeter3;

//
// Amount of substance
//
        public static double mole = 1.0;
        public static double mrad = milliradian;
        public static double ms = millisecond;
        public static double nanoampere = 1.0e-9*ampere;
        public static double nanobarn = 1.0e-9*barn;
        public static double nanofarad = 1.0e-9*farad;
        public static double nanometer = 1.0e-9*meter;
        public static double nanosecond = 1;
        public static double newton = joule/meter; // newton = 6.24150 e+9 * MeV/mm
        public static double ns = nanosecond;
        public static double ohm = volt/ampere; // ohm = 1.60217e-16*(MeV/eplus)/(eplus/ns)
        public static double pascal = hep_pascal;

//
// Activity [T^-1]
//

//
// Miscellaneous
//
        public static double perCent = 0.01;
        public static double perMillion = 0.000001;
        public static double perThousand = 0.001;
        public static double petaelectronvolt = 1e+9*megaelectronvolt;
        public static double PeV = petaelectronvolt;
        public static double picobarn = 1.0e-12*barn;
        public static double picofarad = 1.0e-12*farad;
        public static double picosecond = 1e-12*second;
        public static double rad = radian;
        public static double radian = 1.0;
        public static double s = second;
        public static double second = 1e+9*nanosecond;
        public static double sr = steradian;
        public static double steradian = 1.0;
        public static double teraelectronvolt = 1e+6*megaelectronvolt;
        public static double tesla = volt*second/meter2; // tesla =0.001*megavolt*ns/mm2
        public static double TeV = teraelectronvolt;
        public static double volt = 1.0e-6*megavolt;
        public static double watt = joule/second; // watt = 6.24150 e+3 * MeV/ns
        public static double weber = volt*second; // weber = 1000*megavolt*ns
/**
 * Makes this class non instantiable, but still let's others inherit from it.
 */
    }
}
