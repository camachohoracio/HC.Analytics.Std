#region

using System;
using HC.Core.Helpers;

//using HC.Core.Io;

#endregion

namespace HC.Analytics.Mathematics.Complex
{
    public class ComplexPoly
    {
        private readonly ComplexClass[] coeff; // Array of polynomial coefficients
        private ComplexClass[] coeffwz; // Array of polynomial coefficients with zero roots removed
        private int deg; // Degree of the polynomial
        private int degwz; // Degree of the polynomial with zero roots removed

        private bool suppressRootsErrorMessages;
        // = true if suppression of 'null returned' error messages in roots is required

        // CONSTRUCTORS
        public ComplexPoly(int n)
        {
            deg = n;
            coeff = ComplexClass.oneDarray(n + 1);
        }

        // Coefficients are complex
        public ComplexPoly(ComplexClass[] aa)
        {
            deg = aa.Length - 1;
            coeff = ComplexClass.oneDarray(deg + 1);
            for (int i = 0; i <= deg; i++)
            {
                coeff[i] = ComplexClass.Copy(aa[i]);
            }
        }

        // Coefficients are real (double)
        public ComplexPoly(double[] aa)
        {
            deg = aa.Length - 1;
            coeff = ComplexClass.oneDarray(deg + 1);
            for (int i = 0; i <= deg; i++)
            {
                coeff[i].reset(aa[i], 0.0);
            }
        }

        // Coefficients are real (float)
        public ComplexPoly(float[] aa)
        {
            deg = aa.Length - 1;
            coeff = ComplexClass.oneDarray(deg + 1);
            for (int i = 0; i <= deg; i++)
            {
                coeff[i].reset(aa[i], 0.0);
            }
        }

        // Coefficients are int
        public ComplexPoly(int[] aa)
        {
            deg = aa.Length - 1;
            coeff = ComplexClass.oneDarray(deg + 1);
            for (int i = 0; i <= deg; i++)
            {
                coeff[i].reset(aa[i], 0.0);
            }
        }


        // Single constant -  complex
        // y = aa
        // needed in class Loop
        public ComplexPoly(ComplexClass aa)
        {
            deg = 0;
            coeff = ComplexClass.oneDarray(1);
            coeff[0] = ComplexClass.Copy(aa);
        }

        // Single constant -  double
        // y = aa
        // needed in class Loop
        public ComplexPoly(double aa)
        {
            deg = 0;
            coeff = ComplexClass.oneDarray(1);
            coeff[0].reset(aa, 0.0);
        }

        // Straight line - coefficients are complex
        // y = aa + bb.x
        public ComplexPoly(ComplexClass aa, ComplexClass bb)
        {
            deg = 1;
            coeff = ComplexClass.oneDarray(2);
            coeff[0] = ComplexClass.Copy(aa);
            coeff[1] = ComplexClass.Copy(bb);
        }

        // Straight line - coefficients are real
        // y = aa + bb.x
        public ComplexPoly(double aa, double bb)
        {
            deg = 1;
            coeff = ComplexClass.oneDarray(2);
            coeff[0].reset(aa, 0.0);
            coeff[1].reset(bb, 0.0);
        }

        // Quadratic - coefficients are complex
        // y = aa + bb.x + cc.x^2
        public ComplexPoly(ComplexClass aa, ComplexClass bb, ComplexClass cc)
        {
            deg = 2;
            coeff = ComplexClass.oneDarray(3);
            coeff[0] = ComplexClass.Copy(aa);
            coeff[1] = ComplexClass.Copy(bb);
            coeff[2] = ComplexClass.Copy(cc);
        }

        // Quadratic - coefficients are real
        // y = aa + bb.x + cc.x^2
        public ComplexPoly(double aa, double bb, double cc)
        {
            deg = 2;
            coeff = ComplexClass.oneDarray(3);
            coeff[0].reset(aa, 0.0);
            coeff[1].reset(bb, 0.0);
            coeff[2].reset(cc, 0.0);
        }

        // Cubic - coefficients are complex
        // y = aa + bb.x + cc.x^2 + dd.x^3
        public ComplexPoly(ComplexClass aa, ComplexClass bb, ComplexClass cc, ComplexClass dd)
        {
            deg = 3;
            coeff = ComplexClass.oneDarray(4);
            coeff[0] = ComplexClass.Copy(aa);
            coeff[1] = ComplexClass.Copy(bb);
            coeff[2] = ComplexClass.Copy(cc);
            coeff[3] = ComplexClass.Copy(dd);
        }

        // Cubic - coefficients are real
        // y = aa + bb.x + cc.x^2 + dd.x^3
        public ComplexPoly(double aa, double bb, double cc, double dd)
        {
            deg = 3;
            coeff = ComplexClass.oneDarray(4);
            coeff[0].reset(aa, 0.0);
            coeff[1].reset(bb, 0.0);
            coeff[2].reset(cc, 0.0);
            coeff[3].reset(dd, 0.0);
        }

        // METHODS

        // Returns a ComplexPoly given the polynomial's roots
        public static ComplexPoly rootsToPoly(ComplexClass[] roots)
        {
            if (roots == null)
            {
                return null;
            }

            int pdeg = roots.Length;

            ComplexClass[] rootCoeff = ComplexClass.oneDarray(2);
            rootCoeff[0] = roots[0].times(ComplexClass.minusOne());
            rootCoeff[1] = ComplexClass.plusOne();
            ComplexPoly rPoly = new ComplexPoly(rootCoeff);
            for (int i = 1; i < pdeg; i++)
            {
                rootCoeff[0] = roots[i].times(ComplexClass.minusOne());
                ComplexPoly cRoot = new ComplexPoly(rootCoeff);
                rPoly = rPoly.times(cRoot);
            }
            return rPoly;
        }

        // Reset the polynomial
        public void resetPoly(ComplexClass[] aa)
        {
            if ((deg + 1) != aa.Length)
            {
                throw new ArgumentException("array lengths do not match");
            }
            for (int i = 0; i < deg; i++)
            {
                coeff[i] = ComplexClass.Copy(aa[i]);
            }
        }

        // Reset a coefficient
        public void resetCoeff(int i, ComplexClass aa)
        {
            coeff[i] = ComplexClass.Copy(aa);
        }

        // Return a Copy of this ComplexPoly [instance method]
        public ComplexPoly Copy()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                ComplexPoly aa = new ComplexPoly(deg);
                for (int i = 0; i <= deg; i++)
                {
                    aa.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                aa.deg = deg;
                return aa;
            }
        }

        // Return a Copy of this ComplexPoly [static]
        public static ComplexPoly Copy(ComplexPoly bb)
        {
            if (bb == null)
            {
                return null;
            }
            else
            {
                ComplexPoly aa = new ComplexPoly(bb.deg);
                for (int i = 0; i <= bb.deg; i++)
                {
                    aa.coeff[i] = ComplexClass.Copy(bb.coeff[i]);
                }
                aa.deg = bb.deg;
                return aa;
            }
        }

        // Clone a ComplexPoly
        public Object Clone()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                ComplexPoly aa = new ComplexPoly(deg);
                for (int i = 0; i <= deg; i++)
                {
                    aa.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                aa.deg = deg;
                return aa;
            }
        }

        // Return a Copy of the polynomial
        public ComplexClass[] polyNomCopy()
        {
            ComplexClass[] aa = ComplexClass.oneDarray(deg + 1);
            for (int i = 0; i <= deg; i++)
            {
                aa[i] = ComplexClass.Copy(coeff[i]);
            }
            return aa;
        }

        // Return a reference to the polynomial
        public ComplexClass[] polyNomReference()
        {
            return coeff;
        }

        // Return a reference to the polynomial
        public ComplexClass[] polyNomPointer()
        {
            return coeff;
        }

        // Return a Copy of a coefficient
        public ComplexClass coeffCopy(int i)
        {
            return ComplexClass.Copy(coeff[i]);
        }

        // Return a reference to a coefficient
        public ComplexClass coeffReference(int i)
        {
            return coeff[i];
        }

        // Return a reference to a coefficient
        public ComplexClass coeffPointer(int i)
        {
            return coeff[i];
        }

        // Return the degree
        public int getDeg()
        {
            return deg;
        }

        // Sets the representation of the square root of minus one to j in Strings
        public void setj()
        {
            ComplexClass.setj();
        }

        // Sets the representation of the square root of minus one to i in Strings
        public void seti()
        {
            ComplexClass.seti();
        }

        // Convert to a string of the form (a+jb)[0] + (a+jb)[1].x + (a+jb)[2].x^2  etc.
        public override string ToString()
        {
            string ss = "";
            ss = ss + coeffCopy(0);
            if (deg > 0)
            {
                ss = ss + " + (" + coeffCopy(1) + ").x";
            }
            for (int i = 2; i <= deg; i++)
            {
                ss = ss + " + (" + coeffCopy(i) + ").x^" + i;
            }
            return ss;
        }

        // Print the polynomial to screen
        public void print()
        {
            Console.Write(ToString());
        }

        // Print the polynomial to screen with line return
        public void println()
        {
            PrintToScreen.WriteLine(ToString());
        }

        // Print the polynomial to a text file with title
        public void printToText(string title)
        {
            title = title + ".txt";
            //StringBuilder fout = new StringBuilder(title, 'n');

            //fout.println("Output File for a ComplexPoly");
            //fout.dateAndTimeln();
            //fout.println();
            //fout.print("Polynomial degree is ");
            //fout.println(deg);
            //fout.println();
            //fout.println("The coefficients are ");

            //for (int i = 0; i <= deg; i++)
            //{
            //    fout.println(coeff[i]);
            //}
            //fout.println();
            //fout.println("End of file.");
            //fout.close();
        }

        // Print the polynomial to a text file without a given title
        public void printToText()
        {
            string title = "ComplexPolyOut";
            printToText(title);
        }

        // LOGICAL TESTS
        // Check if two polynomials are identical
        public bool Equals(ComplexPoly cp)
        {
            return isEqual(cp);
        }

        public bool isEqual(ComplexPoly cp)
        {
            bool ret = false;
            int nDegThis = getDeg();
            int nDegCp = cp.getDeg();
            if (nDegThis == nDegCp)
            {
                bool test = true;
                int i = 0;
                while (test)
                {
                    if (!coeff[i].isEqual(cp.coeffReference(i)))
                    {
                        test = false;
                    }
                    else
                    {
                        i++;
                        if (i > nDegCp)
                        {
                            test = false;
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        // Check if two polynomials are identical (static)
        public static bool isEqual(ComplexPoly cp1, ComplexPoly cp2)
        {
            bool ret = false;
            int nDegCp1 = cp1.getDeg();
            int nDegCp2 = cp2.getDeg();
            if (nDegCp1 == nDegCp2)
            {
                bool test = true;
                int i = 0;
                while (test)
                {
                    if (!cp1.coeffReference(i).isEqual(cp2.coeffReference(i)))
                    {
                        test = false;
                    }
                    else
                    {
                        i++;
                        if (i > nDegCp1)
                        {
                            test = false;
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        // ADDITION OF TWO POLYNOMIALS
        // Addition,  instance method
        public ComplexPoly plus(ComplexPoly b)
        {
            int n = Math.Max(deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition,  static method
        public static ComplexPoly plus(ComplexPoly a, ComplexPoly b)
        {
            int n = Math.Max(a.deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == a.deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(a.coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= a.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of a Complex,  instance method
        public ComplexPoly plus(ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            int n = Math.Max(deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of a Complex,  static method
        public static ComplexPoly plus(ComplexPoly a, ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            int n = Math.Max(a.deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == a.deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(a.coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= a.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of a double,  instance method
        public ComplexPoly plus(double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = Math.Max(deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of a double,  static method
        public static ComplexPoly plus(ComplexPoly a, double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = Math.Max(a.deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == a.deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(a.coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= a.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of an int,  instance method
        public ComplexPoly plus(int bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = Math.Max(deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= deg; i++)
                {
                    c.coeff[i] = coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // Addition of an int,  static method
        public static ComplexPoly plus(ComplexPoly a, int bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = Math.Max(a.deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == a.deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(a.coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(b.coeff[i]);
                }
                for (int i = 0; i <= a.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].plus(b.coeff[i]);
                }
            }
            return c;
        }

        // SUBTRACTION OF TWO POLYNOMIALS
        // Subtraction,  instance method
        public ComplexPoly minus(ComplexPoly b)
        {
            int n = Math.Max(deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = coeff[i].minus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = (b.coeff[i]).times(ComplexClass.minusOne());
                }
                for (int i = 0; i <= deg; i++)
                {
                    c.coeff[i] = b.coeff[i].plus(coeff[i]);
                }
            }
            return c;
        }

        // Subtraction of a Complex,  instance method
        public ComplexPoly minus(ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            return minus(b);
        }

        // Subtraction of a double,  instance method
        public ComplexPoly minus(double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            return minus(b);
        }

        // Subtraction,  static method
        public static ComplexPoly minus(ComplexPoly a, ComplexPoly b)
        {
            int n = Math.Max(a.deg, b.deg);
            ComplexPoly c = new ComplexPoly(n);
            if (n == a.deg)
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = ComplexClass.Copy(a.coeff[i]);
                }
                for (int i = 0; i <= b.deg; i++)
                {
                    c.coeff[i] = a.coeff[i].minus(b.coeff[i]);
                }
            }
            else
            {
                for (int i = 0; i <= n; i++)
                {
                    c.coeff[i] = (b.coeff[i]).times(ComplexClass.minusOne());
                }
                for (int i = 0; i <= a.deg; i++)
                {
                    c.coeff[i] = b.coeff[i].plus(a.coeff[i]);
                }
            }
            return c;
        }

        // Subtraction  of a Complex,  static method
        public static ComplexPoly minus(ComplexPoly a, ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            return minus(a, b);
        }


        // Subtraction  of a double,  static method
        public static ComplexPoly minus(ComplexPoly a, double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            return minus(a, b);
        }

        // MULTIPLICATION OF TWO POLYNOMIALS
        // Multiplication,  instance method
        public ComplexPoly times(ComplexPoly b)
        {
            int n = deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // Multiplication,  static method
        public static ComplexPoly times(ComplexPoly a, ComplexPoly b)
        {
            int n = a.deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= a.deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(a.coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // Multiplication by a Complex,  instance method
        public ComplexPoly times(ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            int n = deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // Multiplication by a Complex,  static method
        public static ComplexPoly times(ComplexPoly a, ComplexClass bb)
        {
            ComplexPoly b = new ComplexPoly(bb);
            int n = a.deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= a.deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(a.coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // Multiplication by a double,  instance method
        public ComplexPoly times(double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // Multiplication by a double,  static method
        public static ComplexPoly times(ComplexPoly a, double bb)
        {
            ComplexPoly b = new ComplexPoly(new ComplexClass(bb, 0.0));
            int n = a.deg + b.deg;
            ComplexPoly c = new ComplexPoly(n);
            for (int i = 0; i <= a.deg; i++)
            {
                for (int j = 0; j <= b.deg; j++)
                {
                    c.coeff[i + j].plusEquals(a.coeff[i].times(b.coeff[j]));
                }
            }
            return c;
        }

        // DERIVATIVES
        // Return the coefficients, as a new ComplexPoly,  of the nth derivative
        public ComplexPoly nthDerivative(int n)
        {
            ComplexPoly dnydxn;

            if (n > deg)
            {
                dnydxn = new ComplexPoly(0.0);
            }
            else
            {
                dnydxn = new ComplexPoly(deg - n);
                ComplexClass[] nc = ComplexClass.oneDarray(deg - n + 1);

                int k = deg - n;
                for (int i = deg; i > n - 1; i--)
                {
                    nc[k] = ComplexClass.Copy(coeff[i]);
                    for (int j = 0; j < n; j++)
                    {
                        nc[k] = ComplexClass.times(nc[k], i - j);
                    }
                    k--;
                }
                dnydxn = new ComplexPoly(nc);
            }
            return dnydxn;
        }

        // EVALUATION OF A POLYNOMIAL AND ITS DERIVATIVES
        // Evaluate the polynomial
        public ComplexClass evaluate(ComplexClass x)
        {
            ComplexClass y = new ComplexClass();
            if (deg == 0)
            {
                y = ComplexClass.Copy(coeff[0]);
            }
            else
            {
                y = ComplexClass.Copy(coeff[deg]);
                for (int i = deg - 1; i >= 0; i--)
                {
                    y = ComplexClass.plus(ComplexClass.times(y, x), coeff[i]);
                }
            }
            return y;
        }

        public ComplexClass evaluate(double xx)
        {
            ComplexClass x = new ComplexClass(xx, 0.0);
            ComplexClass y = new ComplexClass();
            if (deg == 0)
            {
                y = ComplexClass.Copy(coeff[0]);
            }
            else
            {
                y = ComplexClass.Copy(coeff[deg]);
                for (int i = deg - 1; i >= 0; i--)
                {
                    y = ComplexClass.plus(ComplexClass.times(y, x), coeff[i]);
                }
            }
            return y;
        }

        // Evaluate the nth derivative of the polynomial
        public ComplexClass nthDerivEvaluate(int n, ComplexClass x)
        {
            ComplexClass dnydxn = new ComplexClass();
            ComplexClass[] nc = ComplexClass.oneDarray(deg + 1);

            if (n == 0)
            {
                dnydxn = evaluate(x);
                PrintToScreen.WriteLine("n = 0 in ComplexPoly.nthDerivative");
                PrintToScreen.WriteLine("polynomial itself evaluated and returned");
            }
            else
            {
                ComplexPoly nthderiv = nthDerivative(n);
                dnydxn = nthderiv.evaluate(x);
            }
            return dnydxn;
        }

        public ComplexClass nthDerivEvaluate(int n, double xx)
        {
            ComplexClass x = new ComplexClass(xx, 0.0);
            return nthDerivEvaluate(n, x);
        }

        // ROOTS OF POLYNOMIALS
        // For general details of root searching and a discussion of the rounding errors
        // see Numerical Recipes, The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/

        // Calculate the roots (real or complex) of a polynomial (real or complex)
        // polish = true ([for deg>3 see laguerreAll(...)]
        // initial root estimates are all zero [for deg>3 see laguerreAll(...)]
        public ComplexClass[] roots()
        {
            bool polish = true;
            ComplexClass estx = new ComplexClass(0.0, 0.0);
            return roots(polish, estx);
        }

        // Calculate the roots - as above with the exception that the error messages are suppressed
        // Required by BlackBox
        public ComplexClass[] rootsNoMessages()
        {
            suppressRootsErrorMessages = true;
            return roots();
        }

        // Calculate the roots (real or complex) of a polynomial (real or complex)
        // initial root estimates are all zero [for deg>3 see laguerreAll(...)]
        // for polish  see laguerreAll(...)[for deg>3]
        public ComplexClass[] roots(bool polish)
        {
            ComplexClass estx = new ComplexClass(0.0, 0.0);
            return roots(polish, estx);
        }

        // Calculate the roots (real or complex) of a polynomial (real or complex)
        // for estx  see laguerreAll(...)[for deg>3]
        // polish = true  see laguerreAll(...)[for deg>3]
        public ComplexClass[] roots(ComplexClass estx)
        {
            bool polish = true;
            return roots(polish, estx);
        }

        // Calculate the roots (real or complex) of a polynomial (real or complex)
        public ComplexClass[] roots(bool polish, ComplexClass estx)
        {
            // degree = 0 - no roots
            if (deg == 0 && !suppressRootsErrorMessages)
            {
                PrintToScreen.WriteLine("degree of the polynomial is zero in the method ComplexPoly.roots");
                PrintToScreen.WriteLine("null returned");
                return null;
            }

            // Check for no roots, i.e all coefficients but the first = 0
            int counter = 0;
            for (int i = 1; i < deg; i++)
            {
                if (coeff[i].isZero())
                {
                    counter++;
                }
            }
            if (counter == deg - 1 && !suppressRootsErrorMessages)
            {
                PrintToScreen.WriteLine(
                    "polynomial coefficients above the zeroth are all zero in the method ComplexPoly.roots");
                PrintToScreen.WriteLine("null returned");
                return null;
            }

            // check for zero roots
            bool testzero = true;
            int ii = 0, nzeros = 0;
            while (testzero)
            {
                if (coeff[ii].isZero())
                {
                    nzeros++;
                    ii++;
                }
                else
                {
                    testzero = false;
                }
            }
            if (nzeros > 0)
            {
                degwz = deg - nzeros;
                coeffwz = ComplexClass.oneDarray(degwz + 1);
                for (int i = 0; i <= degwz; i++)
                {
                    coeffwz[i] = coeff[i + nzeros].Copy();
                }
            }
            else
            {
                degwz = deg;
                coeffwz = ComplexClass.oneDarray(degwz + 1);
                for (int i = 0; i <= degwz; i++)
                {
                    coeffwz[i] = coeff[i].Copy();
                }
            }
            // calculate non-zero roots
            ComplexClass[] roots = ComplexClass.oneDarray(deg);
            ComplexClass[] root = ComplexClass.oneDarray(degwz);

            switch (degwz)
            {
                case 1:
                    root[0] = ComplexClass.negate(coeffwz[0].over(coeffwz[1]));
                    break;
                case 2:
                    root = quadratic(coeffwz[0], coeffwz[1], coeffwz[2]);
                    break;
                case 3:
                    root = cubic(coeffwz[0], coeffwz[1], coeffwz[2], coeffwz[3]);
                    break;
                default:
                    root = laguerreAll(polish, estx);
                    break;
            }

            for (int i = 0; i < degwz; i++)
            {
                roots[i] = root[i].Copy();
            }
            if (nzeros > 0)
            {
                for (int i = degwz; i < deg; i++)
                {
                    roots[i] = ComplexClass.zero();
                }
            }
            return roots;
        }

        // ROOTS OF A QUADRATIC EQUATION
        // ax^2 + bx + c = 0
        // roots returned in root[]
        // 4ac << b*b accomodated by these methods
        public static ComplexClass[] quadratic(ComplexClass c, ComplexClass b, ComplexClass a)
        {
            double qsign = 1.0;
            ComplexClass qsqrt = new ComplexClass();
            ComplexClass qtest = new ComplexClass();
            ComplexClass bconj = new ComplexClass();
            ComplexClass[] root = ComplexClass.oneDarray(2);

            bconj = b.conjugate();
            qsqrt = ComplexClass.Sqrt((ComplexClass.square(b)).minus((a.times(c)).times(4)));

            qtest = bconj.times(qsqrt);

            if (qtest.getReal() < 0.0)
            {
                qsign = -1.0;
            }

            qsqrt = ((qsqrt.over(qsign)).plus(b)).over(-2.0);
            root[0] = ComplexClass.over(qsqrt, a);
            root[1] = ComplexClass.over(c, qsqrt);

            return root;
        }

        public static ComplexClass[] quadratic(double c, double b, double a)
        {
            ComplexClass aa = new ComplexClass(a, 0.0);
            ComplexClass bb = new ComplexClass(b, 0.0);
            ComplexClass cc = new ComplexClass(c, 0.0);

            return quadratic(cc, bb, aa);
        }

        // ROOTS OF A CUBIC EQUATION
        // ddx^3 + ccx^2 + bbx + aa = 0
        // roots returned in root[]

        // ROOTS OF A CUBIC EQUATION
        // ddx^3 + ccx^2 + bbx + aa = 0
        // roots returned in root[]
        public static ComplexClass[] cubic(ComplexClass aa, ComplexClass bb, ComplexClass cc, ComplexClass dd)
        {
            ComplexClass a = cc.over(dd);
            ComplexClass b = bb.over(dd);
            ComplexClass c = aa.over(dd);

            ComplexClass[] roots = ComplexClass.oneDarray(3);

            ComplexClass bigQ = ((a.times(a)).minus(b.times(3.0))).over(9.0);
            ComplexClass bigR =
                (((((a.times(a)).times(a)).times(2.0)).minus((a.times(b)).times(9.0))).plus(c.times(27.0))).over(54.0);

            ComplexClass sign = ComplexClass.plusOne();
            ComplexClass bigAsqrtTerm = ComplexClass.Sqrt((bigR.times(bigR)).minus((bigQ.times(bigQ)).times(bigQ)));
            ComplexClass bigRconjugate = bigR.conjugate();
            if ((bigRconjugate.times(bigAsqrtTerm)).getReal() < 0.0)
            {
                sign = ComplexClass.minusOne();
            }
            ComplexClass bigA = (ComplexClass.Pow(bigR.plus(sign.times(bigAsqrtTerm)), 1.0/3.0)).times(ComplexClass.minusOne());
            ComplexClass bigB = null;
            if (bigA.isZero())
            {
                bigB = ComplexClass.zero();
            }
            else
            {
                bigB = bigQ.over(bigA);
            }
            ComplexClass aPlusB = bigA.plus(bigB);
            ComplexClass aMinusB = bigA.minus(bigB);
            ComplexClass minusAplusB = aPlusB.times(ComplexClass.minusOne());
            ComplexClass aOver3 = a.over(3.0);
            ComplexClass isqrt3over2 = new ComplexClass(0.0, Math.Sqrt(3.0)/2.0);
            roots[0] = aPlusB.minus(aOver3);
            roots[1] = ((minusAplusB.over(2.0)).minus(aOver3)).plus(isqrt3over2.times(aMinusB));
            roots[2] = ((minusAplusB.over(2.0)).minus(aOver3)).minus(isqrt3over2.times(aMinusB));

            return roots;
        }

        public static ComplexClass[] cubic(double d, double c, double b, double a)
        {
            ComplexClass aa = new ComplexClass(a, 0.0);
            ComplexClass bb = new ComplexClass(b, 0.0);
            ComplexClass cc = new ComplexClass(c, 0.0);
            ComplexClass dd = new ComplexClass(d, 0.0);

            return cubic(dd, cc, bb, aa);
        }

        // LAGUERRE'S METHOD FOR COMPLEX ROOTS OF A COMPLEX POLYNOMIAL

        // Laguerre method for one of the roots
        // Following the procedure in Numerical Recipes for C [Reference above]
        // estx     estimate of the root
        // coeff[]  coefficients of the polynomial
        // m        degree of the polynomial
        public static ComplexClass laguerre(ComplexClass estx, ComplexClass[] pcoeff, int m)
        {
            double eps = 1e-7; // estimated fractional Round-off error
            int mr = 8; // number of fractional values in Adam's method of breaking a limit cycle
            int mt = 1000; // number of steps in breaking a limit cycle
            int maxit = mr*mt; // maximum number of iterations allowed
            int niter = 0; // number of iterations taken

            // fractions used to break a limit cycle
            double[] frac = {0.5, 0.25, 0.75, 0.13, 0.38, 0.62, 0.88, 1.0};

            ComplexClass root = new ComplexClass(); // root
            ComplexClass b = new ComplexClass();
            ComplexClass d = new ComplexClass();
            ComplexClass f = new ComplexClass();
            ComplexClass g = new ComplexClass();
            ComplexClass g2 = new ComplexClass();
            ComplexClass h = new ComplexClass();
            ComplexClass sq = new ComplexClass();
            ComplexClass gp = new ComplexClass();
            ComplexClass gm = new ComplexClass();
            ComplexClass dx = new ComplexClass();
            ComplexClass x1 = new ComplexClass();
            ComplexClass temp1 = new ComplexClass();
            ComplexClass temp2 = new ComplexClass();

            double abp = 0.0D, abm = 0.0D;
            double err = 0.0D, abx = 0.0D;

            for (int i = 1; i <= maxit; i++)
            {
                niter = i;
                b = ComplexClass.Copy(pcoeff[m]);
                err = ComplexClass.Abs(b);
                d = f = ComplexClass.zero();
                abx = ComplexClass.Abs(estx);
                for (int j = m - 1; j >= 0; j--)
                {
                    // Efficient computation of the polynomial and its first two derivatives
                    f = ComplexClass.plus(ComplexClass.times(estx, f), d);
                    d = ComplexClass.plus(ComplexClass.times(estx, d), b);
                    b = ComplexClass.plus(ComplexClass.times(estx, b), pcoeff[j]);
                    err = ComplexClass.Abs(b) + abx*err;
                }
                err *= eps;

                // Estimate of Round-off error in evaluating polynomial
                if (ComplexClass.Abs(b) <= err)
                {
                    root = ComplexClass.Copy(estx);
                    niter = i;
                    return root;
                }
                // Laguerre formula
                g = ComplexClass.over(d, b);
                g2 = ComplexClass.square(g);
                h = ComplexClass.minus(g2, ComplexClass.times(2.0, ComplexClass.over(f, b)));
                sq = ComplexClass.Sqrt(ComplexClass.times((m - 1), ComplexClass.minus(ComplexClass.times(m, h), g2)));
                gp = ComplexClass.plus(g, sq);
                gm = ComplexClass.minus(g, sq);
                abp = ComplexClass.Abs(gp);
                abm = ComplexClass.Abs(gm);
                if (abp < abm)
                {
                    gp = gm;
                }
                temp1.setReal(m);
                temp2.setReal(Math.Cos(i));
                temp2.setImag(Math.Sin(i));
                dx = ((Math.Max(abp, abm) > 0.0 ? ComplexClass.over(temp1, gp) : ComplexClass.times(Math.Exp(1.0 + abx), temp2)));
                x1 = ComplexClass.minus(estx, dx);
                if (ComplexClass.isEqual(estx, x1))
                {
                    root = ComplexClass.Copy(estx);
                    niter = i;
                    return root; // converged
                }
                if ((i%mt) != 0)
                {
                    estx = ComplexClass.Copy(x1);
                }
                else
                {
                    // Every so often we take a fractional step to break any limit cycle
                    // (rare occurence)
                    estx = ComplexClass.minus(estx, ComplexClass.times(frac[i/mt - 1], dx));
                }
                niter = i;
            }
            // exceeded maximum allowed iterations
            root = ComplexClass.Copy(estx);
            PrintToScreen.WriteLine("Maximum number of iterations exceeded in laguerre");
            PrintToScreen.WriteLine("root returned at this point");
            return root;
        }

        // Finds all roots of a complex polynomial by successive calls to laguerre
        // Following the procedure in Numerical Recipes for C [Reference above]
        // Initial estimates are all zero, polish=true
        public ComplexClass[] laguerreAll()
        {
            ComplexClass estx = new ComplexClass(0.0, 0.0);
            bool polish = true;
            return laguerreAll(polish, estx);
        }

        //  Initial estimates estx, polish=true
        public ComplexClass[] laguerreAll(ComplexClass estx)
        {
            bool polish = true;
            return laguerreAll(polish, estx);
        }

        //  Initial estimates are all zero.
        public ComplexClass[] laguerreAll(bool polish)
        {
            ComplexClass estx = new ComplexClass(0.0, 0.0);
            return laguerreAll(polish, estx);
        }

        // Finds all roots of a complex polynomial by successive calls to laguerre
        //  Initial estimates are estx
        public ComplexClass[] laguerreAll(bool polish, ComplexClass estx)
        {
            // polish bool variable
            // if true roots polished also by Laguerre
            // if false roots returned to be polished by another method elsewhere.
            // estx estimate of root - Preferred default value is zero to favour convergence
            //   to smallest remaining root

            int m = degwz;
            double eps = 2.0e-6; // tolerance in determining Round off in imaginary part

            ComplexClass x = new ComplexClass();
            ComplexClass b = new ComplexClass();
            ComplexClass c = new ComplexClass();
            ComplexClass[] ad = new ComplexClass[m + 1];
            ComplexClass[] roots = new ComplexClass[m + 1];

            // Copy polynomial for successive deflation
            for (int j = 0; j <= m; j++)
            {
                ad[j] = ComplexClass.Copy(coeffwz[j]);
            }

            // Loop over each root found
            for (int j = m; j >= 1; j--)
            {
                x = ComplexClass.Copy(estx);
                // Preferred default value is zero to favour convergence to smallest remaining root
                // and find the root
                x = laguerre(x, ad, j);
                if (Math.Abs(x.getImag()) <= 2.0*eps*Math.Abs(x.getReal()))
                {
                    x.setImag(0.0);
                }
                roots[j] = ComplexClass.Copy(x);
                b = ComplexClass.Copy(ad[j]);
                for (int jj = j - 1; jj >= 0; jj--)
                {
                    c = ComplexClass.Copy(ad[jj]);
                    ad[jj] = ComplexClass.Copy(b);
                    b = (x.times(b)).plus(c);
                }
            }

            if (polish)
            {
                // polish roots using the undeflated coefficients
                for (int j = 1; j <= m; j++)
                {
                    roots[j] = laguerre(roots[j], coeffwz, m);
                }
            }

            // Sort roots by their real parts by straight insertion
            for (int j = 2; j <= m; j++)
            {
                x = ComplexClass.Copy(roots[j]);
                int i = 0;
                for (i = j - 1; i >= 1; i--)
                {
                    if (roots[i].getReal() <= x.getReal())
                    {
                        break;
                    }
                    roots[i + 1] = ComplexClass.Copy(roots[i]);
                }
                roots[i + 1] = ComplexClass.Copy(x);
            }
            // shift roots to zero initial index
            for (int i = 0; i < m; i++)
            {
                roots[i] = ComplexClass.Copy(roots[i + 1]);
            }
            return roots;
        }
    }
}
