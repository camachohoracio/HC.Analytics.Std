#region

using System;
using HC.Analytics.Colt.CustomImplementations;
using HC.Analytics.Colt.CustomImplementations.tmp;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Helpers;

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
    //package Appendimpl;

    ////import DoubleDoubleFunction;
    ////import DoubleFunction;
    ////import IntArrayList;
    ////import map.AbstractIntDoubleMap;
    ////import map.OpenIntDoubleHashMap;
    ////import DoubleFactory2D;
    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    ////import AppendDoubleMatrix3D;
    ////import DoubleMatrix2DComparator;
    ////import Algebra;
    ////import LUDecompositionQuick;
    ////import SeqBlas;
    /**
     * Quick and dirty tests.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     */

    [Serializable]
    public class TestMatrix2D
    {
        private static readonly DoubleFactory2D Factory2D = DoubleFactory2D.dense;

        private static readonly Algebra LinearAlgebra = Algebra.DEFAULT;
        private static readonly Property Property = Property.DEFAULT;
        private static Functions F = Functions.m_functions;

        private static DoubleFactory1D Factory1D =
            DoubleFactory1D.dense;

        private static Transform Transform = Transform.transform;

        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */

        public TestMatrix2D()
        {
            throw new HCException("Non instantiable");
        }

        /**
         */

        public static void doubleTest()
        {
            int m_intRows = 4;
            int columns = 5; // make a 4*5 matrix
            DoubleMatrix2D master = new DenseDoubleMatrix2D(m_intRows, columns);
            PrintToScreen.WriteLine(master);
            master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            master.viewPart(2, 1, 2, 3).assign(2); // set [2,1] .. [3,3] to 2
            PrintToScreen.WriteLine(Environment.NewLine + master);

            DoubleMatrix2D copyPart = master.viewPart(2, 1, 2, 3).Copy();
            copyPart.assign(3); // modify an independent copy
            copyPart.set(0, 0, 4);
            PrintToScreen.WriteLine(Environment.NewLine + copyPart); // has changed
            PrintToScreen.WriteLine(Environment.NewLine + master); // master has not changed

            DoubleMatrix2D view1 = master.viewPart(0, 3, 4, 2); // [0,3] .. [3,4]
            DoubleMatrix2D view2 = view1.viewPart(0, 0, 4, 1); // a view from a view 
            PrintToScreen.WriteLine(Environment.NewLine + view1);
            PrintToScreen.WriteLine(Environment.NewLine + view2);
        }

        /**
         */

        public static void doubleTest(
            int m_intRows, 
            int columns, 
            int initialCapacity, 
            double minLoadFactor,
            double maxLoadFactor)
        {
            DoubleMatrix2D matrix = new SparseDoubleMatrix2D(m_intRows, columns, initialCapacity, minLoadFactor,
                                                             maxLoadFactor);
            PrintToScreen.WriteLine(matrix);

            PrintToScreen.WriteLine("adding...");
            int i = 0;
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < m_intRows; row++)
                {
                    //if (i%1000 == 0) { 
                    matrix.set(row, column, i);
                    //}
                    i++;
                }
            }
            PrintToScreen.WriteLine(matrix);

            PrintToScreen.WriteLine("removing...");
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < m_intRows; row++)
                {
                    //if (i%1000 == 0) {
                    matrix.set(row, column, 0);
                    //}
                }
            }

            PrintToScreen.WriteLine(matrix);
            PrintToScreen.WriteLine("bye bye.");
        }

        /**
         */

        public static void doubleTest10()
        {
            int m_intRows = 6;
            int columns = 7; // make a 4*5 matrix
            //DoubleMatrix2D master = new DenseDoubleMatrix2D(m_intRows,columns);
            DoubleMatrix2D master = Factory2D.ascending(m_intRows, columns);
            //Basic.ascending(master);
            //master.assign(1); // set all cells to 1
            Transform.mult(master, Math.Sin(0.3));
            PrintToScreen.WriteLine(Environment.NewLine + master);
            //master.viewPart(2,0,2,3).assign(2); // set [2,1] .. [3,3] to 2
            //PrintToScreen.WriteLine(Environment.NewLine+master);

            int[] rowIndexes = {0, 1, 2, 3};
            int[] columnIndexes = {0, 1, 2, 3};

            int[] rowIndexes2 = {3, 0, 3};
            int[] columnIndexes2 = {3, 0, 3};
            DoubleMatrix2D view1 = master.viewPart(1, 1, 4, 5).viewSelection(rowIndexes, columnIndexes);
            PrintToScreen.WriteLine("\nview1=" + view1);
            DoubleMatrix2D view9 = view1.viewStrides(2, 2).viewStrides(2, 1);
            PrintToScreen.WriteLine("\nview9=" + view9);
            view1 = view1.viewSelection(rowIndexes2, columnIndexes2);
            PrintToScreen.WriteLine("\nview1=" + view1);
            DoubleMatrix2D view2 = view1.viewPart(1, 1, 2, 2);
            PrintToScreen.WriteLine("\nview2=" + view2);
            DoubleMatrix2D view3 = view2.viewRowFlip();
            PrintToScreen.WriteLine("\nview3=" + view3);
            view3.assign(Factory2D.ascending(view3.Rows(), view3.Columns()));
            //Basic.ascending(view3);
            PrintToScreen.WriteLine("\nview3=" + view3);

            //view2.assign(-1);
            PrintToScreen.WriteLine("\nmaster replaced" + master);
            PrintToScreen.WriteLine("\nview1 replaced" + view1);
            PrintToScreen.WriteLine("\nview2 replaced" + view2);
            PrintToScreen.WriteLine("\nview3 replaced" + view3);
        }

        /**
         */

        public static void doubleTest11()
        {
            int m_intRows = 4;
            int columns = 5; // make a 1*1 matrix
            DoubleMatrix2D master = new DenseDoubleMatrix2D(1, 1);
            master.assign(2);
            PrintToScreen.WriteLine(Environment.NewLine + master);

            int[] rowIndexes = new int[m_intRows];
            int[] columnIndexes = new int[columns];

            DoubleMatrix2D view1 = master.viewSelection(rowIndexes, columnIndexes);
            PrintToScreen.WriteLine(view1);

            master.assign(1);
            PrintToScreen.WriteLine(Environment.NewLine + master);
            PrintToScreen.WriteLine(view1);
        }

        /**
         */

        public static void doubleTest12()
        {
            DoubleMatrix2D A; //, J;
            DoubleMatrix2D C; //, J;
            DoubleMatrix2D D; //, J;
            DoubleMatrix2D E; //, J;
            DoubleMatrix2D F; //, J;
            DoubleMatrix2D G; //, J;
            DoubleMatrix2D H; //, J;
            DoubleMatrix2D I; //, J;
            A = Factory2D.make(2, 3, 9);
            DoubleMatrix2D B = Factory2D.make(4, 3, 8);
            C = Factory2D.appendRows(A, B);
            PrintToScreen.WriteLine("\nA=" + A);
            PrintToScreen.WriteLine("\nB=" + B);
            PrintToScreen.WriteLine("\nC=" + C);
            D = Factory2D.make(3, 2, 7);
            E = Factory2D.make(3, 4, 6);
            F = Factory2D.appendColumns(D, E);
            PrintToScreen.WriteLine("\nD=" + D);
            PrintToScreen.WriteLine("\nE=" + E);
            PrintToScreen.WriteLine("\nF=" + F);
            G = Factory2D.appendRows(C, F);
            PrintToScreen.WriteLine("\nG=" + G);
            H = Factory2D.ascending(2, 3);
            PrintToScreen.WriteLine("\nH=" + H);
            I = Factory2D.repeat(H, 2, 3);
            PrintToScreen.WriteLine("\nI=" + I);
        }

        /**
         */

        public static void doubleTest13()
        {
            double[] values = {0, 1, 2, 3};
            DoubleMatrix1D matrix = new DenseDoubleMatrix1D(values);
            PrintToScreen.WriteLine(matrix);

            // Sum( x[i]*x[i] ) 
            PrintToScreen.WriteLine(matrix.viewSelection(
                                  new DoubleProcedureMod2()));
            //--> 14

            // Sum( x[i]*x[i] ) 
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_plus, Functions.m_square));
            //--> 14

            // Sum( x[i]*x[i]*x[i] ) 
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_plus, Functions.pow(3)));
            //--> 36

            // Sum( x[i] ) 
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_plus, Functions.m_identity));
            //--> 6

            // Min( x[i] ) 
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_min, Functions.m_identity));
            //--> 0

            // Max( Sqrt(x[i]) / 2 ) 
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_max, Functions.chain(Functions.div(2), Functions.m_sqrt)));
            //--> 0.8660254037844386

            // Number of all cells with 0 <= value <= 2
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_plus, Functions.between(0, 2)));
            //--> 3

            // Number of all cells with 0.8 <= Log2(value) <= 1.2
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_plus,
                                               Functions.chain(Functions.between(0.8, 1.2), Functions.m_log2)));
            //--> 1

            // Product( x[i] )
            PrintToScreen.WriteLine(matrix.aggregate(Functions.m_mult, Functions.m_identity));
            //--> 0

            // Product( x[i] ) of all x[i] > limit
            double limit = 1;
            DoubleFunction f = new DoubleFunctionLimitOfOne(limit);
            PrintToScreen.WriteLine(
                matrix.aggregate(
                    Functions.m_mult, f));
            //--> 6

            // Sum( (x[i]+y[i])^2 )
            DoubleMatrix1D otherMatrix1D = matrix.Copy();
            PrintToScreen.WriteLine(
                matrix.aggregate(
                    otherMatrix1D,
                    Functions.m_plus,
                    Functions.chain(
                        Functions.m_square,
                        Functions.m_plus)));
            // --> 56


            matrix.assign(Functions.plus(1));
            otherMatrix1D = matrix.Copy();
            //otherMatrix1D.zMult(3);
            PrintToScreen.WriteLine(matrix);
            PrintToScreen.WriteLine(otherMatrix1D);
            // Sum(Math.PI * Math.Log(otherMatrix1D[i] / matrix[i]))
            PrintToScreen.WriteLine(
                matrix.aggregate(
                    otherMatrix1D,
                    Functions.m_plus,
                    Functions.chain(
                        Functions.mult(Math.PI), Functions.chain(Functions.m_log,
                                                                 Functions.swapArgs(Functions.m_div)))));
            // or, perhaps less error prone and more readable: 
            //PrintToScreen.WriteLine(matrix.aggregate(otherMatrix1D, Functions.plus,
            //   new DoubleDoubleFunction() {
            //      public double Apply(double a, double b) { return Math.PI*Math.Log(b/a); }
            //   }
            //)
            //);

            DoubleMatrix3D x = DoubleFactory3D.dense.ascending(2, 2, 2);
            PrintToScreen.WriteLine(x);

            // Sum( x[slice,row,col]*x[slice,row,col] ) 
            PrintToScreen.WriteLine(x.aggregate(
                                  Functions.m_plus, Functions.m_square));
            //--> 140


            DoubleMatrix3D y = x.Copy();
            // Sum( (x[i]+y[i])^2 )
            PrintToScreen.WriteLine(x.aggregate(
                                  y,
                                  Functions.m_plus,
                                  Functions.chain(
                                      Functions.m_square, Functions.m_plus)));
            //--> 560

            PrintToScreen.WriteLine(matrix.assign(Functions.random()));
            //PrintToScreen.WriteLine(
            //    matrix.assign(
            //        new Poisson(5, Poisson.makeDefaultGenerator())));
        }

        /**
         */

        public static void doubleTest14(int r1, int c, int r2)
        {
            double[] values = {0, 1, 2, 3};
            DoubleMatrix2D a = DoubleFactory2D.dense.ascending(r1, c);
            DoubleMatrix2D b = Transform.mult(DoubleFactory2D.dense.ascending(c, r2), -1);


            //PrintToScreen.WriteLine(a);
            //PrintToScreen.WriteLine(b);
            //PrintToScreen.WriteLine(Basic.product(a,b));
            a.assign(0);
            b.assign(0);

            Timer timer = new Timer().start();
            LinearAlgebra.mult(a, b);
            timer.stop().display();
        }

        /**
         */

        public static void doubleTest15(int size, int runs)
        {
            PrintToScreen.WriteLine("\n\n");
            double[,] values =
                {
                    {0, 5, 9},
                    {2, 6, 10},
                    {3, 7, 11}
                };

            //DoubleMatrix2D A = Factory2D.make(values);
            DoubleMatrix2D A = Factory2D.make(size, size);
            double value = 5;
            for (int i = size; --i >= 0;)
            {
                A.setQuick(i, i, value);
            }
            A.viewRow(0).assign(value);

            //DoubleMatrix2D A = Factory2D.makeIdentity(size,size);


            //DoubleMatrix2D A = Factory2D.makeAscending(size,size).assign(new MersenneTwister());
            Timer timer = new Timer().start();
            DoubleMatrix2D inv = null;
            for (int run = 0; run < runs; run++)
            {
                inv = LinearAlgebra.inverse(A);
            }
            timer.stop().display();

            /*
            timer.reset().start();
            for (int run=0; run<runs; run++) {
                new Jama.Matrix(A.ToArray()).inverse();
            }
            timer.stop().display();
            */
            //PrintToScreen.WriteLine("A="+A);
            //PrintToScreen.WriteLine("inverse(A)="+inv);
            //PrintToScreen.WriteLine("formatted inverse(A)="+ new Jama.Matrix(inv.ToArray()));

            /*
            -1.0000000000000018, 2.000000000000007, -1.0000000000000047
            2.000000000000007, -6.750000000000024, 4.500000000000016
            -1.000000000000004, 3.7500000000000133, -2.500000000000009
            */
        }

        /**
         */

        public static void doubleTest17(int size)
        {
            PrintToScreen.WriteLine("\n\n");

            //DoubleMatrix2D A = Factory2D.make(values);
            DoubleMatrix2D A = Factory2D.ascending(3, 4);
            DoubleMatrix2D B = Factory2D.ascending(2, 3);
            DoubleMatrix2D C = Factory2D.ascending(1, 2);
            B.assign(Functions.plus(A.zSum()));
            C.assign(Functions.plus(B.zSum()));


            /*
            PrintToScreen.WriteLine(Environment.NewLine+A);
            PrintToScreen.WriteLine(Environment.NewLine+B);
            PrintToScreen.WriteLine(Environment.NewLine+C);
            PrintToScreen.WriteLine(Environment.NewLine+Factory2D.diag(A,B,C));
            */

            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A.ToString()));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(B.ToString()));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(C.ToString()));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(Factory2D.diagonal(A,B,C).ToString()));
        }

        /**
         */

        public static void doubleTest18(int size)
        {
            PrintToScreen.WriteLine("\n\n");
            int s = 2;

            //DoubleMatrix2D A = Factory2D.make(values);
            DoubleMatrix2D A00, A01, A02, A10, A11, A12, A20, A21, A22, empty;
            empty = Factory2D.make(0, 0);

            A00 = Factory2D.ascending(s, s);
            //A01 = empty;
            A01 = Factory2D.ascending(s, s).assign(Functions.plus(A00.getQuick(s - 1, s - 1)));
            A02 = Factory2D.ascending(s, s).assign(Functions.plus(A01.getQuick(s - 1, s - 1)));
            A10 = Factory2D.ascending(s, s).assign(Functions.plus(A02.getQuick(s - 1, s - 1)));
            A11 = null;
            //A11 = Factory2D.ascending(s,s).assign(Functions.plus(A10.getQuick(s-1,s-1)));
            A12 = Factory2D.ascending(s, s).assign(Functions.plus(A10.getQuick(s - 1, s - 1)));
            //A12 = Factory2D.ascending(s,s).assign(Functions.plus(A11.getQuick(s-1,s-1)));
            A20 = Factory2D.ascending(s, s).assign(Functions.plus(A12.getQuick(s - 1, s - 1)));
            A21 = empty;
            //A21 = Factory2D.ascending(s,s).assign(Functions.plus(A20.getQuick(s-1,s-1)));
            A22 = Factory2D.ascending(s, s).assign(Functions.plus(A20.getQuick(s - 1, s - 1)));
            //A22 = Factory2D.ascending(s,s).assign(Functions.plus(A21.getQuick(s-1,s-1)));


            //B.assign(Functions.plus(A.zSum()));
            //C.assign(Functions.plus(B.zSum()));


            PrintToScreen.WriteLine(Environment.NewLine + A00);
            PrintToScreen.WriteLine(Environment.NewLine + A01);
            PrintToScreen.WriteLine(Environment.NewLine + A02);
            PrintToScreen.WriteLine(Environment.NewLine + A10);
            PrintToScreen.WriteLine(Environment.NewLine + A11);
            PrintToScreen.WriteLine(Environment.NewLine + A12);
            PrintToScreen.WriteLine(Environment.NewLine + A20);
            PrintToScreen.WriteLine(Environment.NewLine + A21);
            PrintToScreen.WriteLine(Environment.NewLine + A22);
            //PrintToScreen.WriteLine(Environment.NewLine+Factory2D.make33(A00,A01,A02,A10,A11,A12,A20,A21,A22));


            /*
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A00.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A01.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A02.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A10.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A11.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A12.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A20.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A21.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A22.ToString()));

            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(Factory2D.make33(A00,A01,A02,A10,A11,A12,A20,A21,A22).ToString()));
            */
        }

        /**
         */

        public static void doubleTest19()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D A;
            int k;
            int uk;
            int lk;

            double[,] values5 =
                {
                    {0, 0, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                };
            A = Factory2D.make(values5);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);

            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            double[,] values4 =
                {
                    {1, 0, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 1}
                };
            A = Factory2D.make(values4);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            double[,] values1 =
                {
                    {1, 1, 0, 0},
                    {1, 1, 1, 0},
                    {0, 1, 1, 1},
                    {0, 0, 1, 1}
                };
            A = Factory2D.make(values1);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));


            double[,] values6 =
                {
                    {0, 1, 1, 1},
                    {0, 1, 1, 1},
                    {0, 0, 0, 1},
                    {0, 0, 0, 1}
                };
            A = Factory2D.make(values6);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            double[,] values7 =
                {
                    {0, 0, 0, 0},
                    {1, 1, 0, 0},
                    {1, 1, 0, 0},
                    {1, 1, 1, 1}
                };
            A = Factory2D.make(values7);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));


            double[,] values2 =
                {
                    {1, 1, 0, 0},
                    {0, 1, 1, 0},
                    {0, 1, 0, 1},
                    {1, 0, 1, 1}
                };
            A = Factory2D.make(values2);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            double[,] values3 =
                {
                    {1, 1, 1, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 1},
                    {0, 0, 1, 1}
                };
            A = Factory2D.make(values3);
            k = Property.DEFAULT.semiBandwidth(A);
            uk = Property.DEFAULT.upperBandwidth(A);
            lk = Property.DEFAULT.lowerBandwidth(A);
            PrintToScreen.WriteLine("\n\nupperBandwidth=" + uk);
            PrintToScreen.WriteLine("lowerBandwidth=" + lk);
            PrintToScreen.WriteLine("bandwidth=" + k + " " + A);
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));
        }

        /**
         */

        public static void doubleTest19(int size)
        {
            PrintToScreen.WriteLine("\n\n");
            int s = 2;

            //DoubleMatrix2D A = Factory2D.make(values);
            DoubleMatrix2D A00, A01, A02, A10, A11, A12, A20, A21, A22, empty;
            empty = Factory2D.make(0, 0);

            A00 = Factory2D.ascending(s, s);
            //A01 = empty;
            A01 = Factory2D.ascending(s, s).assign(Functions.plus(A00.getQuick(s - 1, s - 1)));
            A02 = Factory2D.ascending(s, s).assign(Functions.plus(A01.getQuick(s - 1, s - 1)));
            A10 = Factory2D.ascending(s, s).assign(Functions.plus(A02.getQuick(s - 1, s - 1)));
            A11 = null;
            //A11 = Factory2D.ascending(s,s).assign(Functions.plus(A10.getQuick(s-1,s-1)));
            A12 = Factory2D.ascending(s, s).assign(Functions.plus(A10.getQuick(s - 1, s - 1)));
            //A12 = Factory2D.ascending(s,s).assign(Functions.plus(A11.getQuick(s-1,s-1)));
            A20 = Factory2D.ascending(s, s).assign(Functions.plus(A12.getQuick(s - 1, s - 1)));
            A21 = empty;
            //A21 = Factory2D.ascending(s,s).assign(Functions.plus(A20.getQuick(s-1,s-1)));
            A22 = Factory2D.ascending(s, s).assign(Functions.plus(A20.getQuick(s - 1, s - 1)));
            //A22 = Factory2D.ascending(s,s).assign(Functions.plus(A21.getQuick(s-1,s-1)));


            //B.assign(Functions.plus(A.zSum()));
            //C.assign(Functions.plus(B.zSum()));


            PrintToScreen.WriteLine(Environment.NewLine + A00);
            PrintToScreen.WriteLine(Environment.NewLine + A01);
            PrintToScreen.WriteLine(Environment.NewLine + A02);
            PrintToScreen.WriteLine(Environment.NewLine + A10);
            PrintToScreen.WriteLine(Environment.NewLine + A11);
            PrintToScreen.WriteLine(Environment.NewLine + A12);
            PrintToScreen.WriteLine(Environment.NewLine + A20);
            PrintToScreen.WriteLine(Environment.NewLine + A21);
            PrintToScreen.WriteLine(Environment.NewLine + A22);
            //PrintToScreen.WriteLine(Environment.NewLine+Factory2D.make33(A00,A01,A02,A10,A11,A12,A20,A21,A22));


            /*
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A00.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A01.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A02.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A10.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A11.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A12.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A20.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A21.ToString()));
            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(A22.ToString()));

            PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(Factory2D.make33(A00,A01,A02,A10,A11,A12,A20,A21,A22).ToString()));
            */
        }

        /**
         */

        public static void doubleTest2()
        {
            // using a map
            int[] keys = {0, 3, 100000, 9};
            double[] values = {100.0, 1000.0, 70.0, 71.0};

            int size = keys.Length;
            AbstractIntDoubleMap map = new OpenIntDoubleHashMap(size*2, 0.2, 0.5);

            for (int i = 0; i < keys.Length; i++)
            {
                map.put(keys[i], (int) values[i]);
            }

            PrintToScreen.WriteLine(map.containsKey(3));
            PrintToScreen.WriteLine(map.get(3));

            PrintToScreen.WriteLine(map.containsKey(4));
            PrintToScreen.WriteLine(map.get(4));

            PrintToScreen.WriteLine(map.containsValue((int) 71.0));
            PrintToScreen.WriteLine(map.keyOf((int) 71.0));

            PrintToScreen.WriteLine(map);
        }

        /**
         */

        public static void doubleTest20()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D A;
            //int k;
            //int uk;
            //int lk;

            double[,] values1 =
                {
                    {0, 1, 0, 0},
                    {3, 0, 2, 0},
                    {0, 2, 0, 3},
                    {0, 0, 1, 0}
                };
            A = Factory2D.make(values1);

            PrintToScreen.WriteLine("\n\n" + LinearAlgebra.toVerboseString(A));
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));


            double[,] values2 =
                {
                    {1.0000000000000167, -0.3623577544766736, -0.3623577544766736},
                    {0, 0.9320390859672374, -0.3377315902755755},
                    {0, 0, 0.8686968577706282},
                    {0, 0, 0},
                    {0, 0, 0}
                };

            A = Factory2D.make(values2);

            PrintToScreen.WriteLine("\n\n" + LinearAlgebra.toVerboseString(A));
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            double[,] values3 =
                {
                    {611, 196, -192, 407, -8, -52, -49, 29},
                    {196, 899, 113, -192, -71, -43, -8, -44},
                    {-192, 113, 899, 196, 61, 49, 8, 52},
                    {407, -192, 196, 611, 8, 44, 59, -23},
                    {-8, -71, 61, 8, 411, -599, 208, 208},
                    {-52, -43, 49, 44, -599, 411, 208, 208},
                    {-49, -8, 8, 59, 208, 208, 99, -911},
                    {29, -44, 52, -23, 208, 208, -911, 99}
                };


            A = Factory2D.make(values3);

            PrintToScreen.WriteLine("\n\n" + LinearAlgebra.toVerboseString(A));
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));

            //Exact eigenvalues from Westlake (1968), p.150 (ei'vectors given too):
            double a = Math.Sqrt(10405);
            double b = Math.Sqrt(26);
            double[] e = {-10*a, 0, 510 - 100*b, 1000, 1000, 510 + 100*b, 1020, 10*a};
            PrintToScreen.WriteLine(DoubleFactory1D.dense.make(e));
        }

        /**
         */

        public static void doubleTest21()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D A;
            //int k;
            //int uk;
            //int lk;

            double[,] values1 =
                {
                    {1/3, 2/3, Math.PI, 0},
                    {3, 9, 0, 0},
                    {0, 2, 7, 0},
                    {0, 0, 3, 9}
                };
            A = Factory2D.make(values1);
            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(new Formatter(null).ToString(A));

            //PrintToScreen.WriteLine("\n\n"+LinearAlgebra.toVerboseString(A));
            //PrintToScreen.WriteLine(new LUDecomposition(A));
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));
        }

        /**
         */

        public static void doubleTest22()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D A;
            //int k;
            //int uk;
            //int lk;

            double[,] values1 =
                {
                    {1/3, 2/3, Math.PI, 0},
                    {3, 9, 0, 0},
                    {0, 2, 7, 0},
                    {0, 0, 3, 9}
                };
            A = Factory2D.make(values1);
            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(Property.isDiagonallyDominantByRow(A));
            PrintToScreen.WriteLine(Property.isDiagonallyDominantByColumn(A));
            Property.generateNonSingular(A);
            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(Property.isDiagonallyDominantByRow(A));
            PrintToScreen.WriteLine(Property.isDiagonallyDominantByColumn(A));

            //PrintToScreen.WriteLine("\n\n"+LinearAlgebra.toVerboseString(A));
            //PrintToScreen.WriteLine(new LUDecomposition(A));
            //PrintToScreen.WriteLine("\n\nbandwidth="+k+" "+matrixpattern.Converting.toHTML(A.ToString()));
        }

        /**
         */

        public static void doubleTest23(
            int runs,
            int size,
            double nonZeroFraction,
            bool dense)
        {
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            DoubleMatrix2D A, LU; //, I, Inv;
            DoubleMatrix1D b, solved;

            double mean = 5.0;
            double stdDev = 3.0;
            UnivNormalDist random =
                new UnivNormalDist(mean, stdDev, new RngWrapper());

            PrintToScreen.WriteLine("sampling...");
            double value = 2;
            if (dense)
            {
                A = DoubleFactory2D.dense.sample(size, size, value, nonZeroFraction);
            }
            else
            {
                A = DoubleFactory2D.sparse.sample(size, size, value, nonZeroFraction);
            }
            b = A.like1D(size).assign(1);

            //A.assign(random);
            //A.assign(Functions.rint); // round
            PrintToScreen.WriteLine("generating invertible matrix...");
            Property.generateNonSingular(A);

            //I = Factory2D.identity(size);

            LU = A.like();
            solved = b.like();
            //Inv = Factory2D.make(size,size);

            LUDecompositionQuick lu = new LUDecompositionQuick();

            PrintToScreen.WriteLine("benchmarking assignment...");
            Timer timer = new Timer().start();
            LU.assign(A);
            solved.assign(b);
            timer.stop().display();

            LU.assign(A);
            lu.decompose(LU);

            PrintToScreen.WriteLine("benchmarking LU...");
            timer.reset().start();
            for (int i = runs; --i >= 0;)
            {
                solved.assign(b);
                //Inv.assign(I);
                //lu.decompose(LU);
                lu.solve(solved);
                //lu.solve(Inv);
            }
            timer.stop().display();

            //PrintToScreen.WriteLine("A="+A);
            //PrintToScreen.WriteLine("LU="+LU);
            //PrintToScreen.WriteLine("U="+lu.getU());
            //PrintToScreen.WriteLine("L="+lu.getL());
            PrintToScreen.WriteLine("done.");
        }

        /**
         */

        public static void doubleTest24(int runs, int size, bool dense)
        {
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense)
            {
                factory = DoubleFactory2D.dense;
            }
            else
            {
                factory = DoubleFactory2D.sparse;
            }

            double value = 2;
            double omega = 1.25;
            double alpha = omega*0.25;
            double beta = 1 - omega;
            A = factory.make(size, size, value);

            Double9Function function = new Double9Function_(alpha, beta);
            Timer timer = new Timer().start();

            PrintToScreen.WriteLine("benchmarking stencil...");
            for (int i = 0; i < runs; i++)
            {
                A.zAssign8Neighbors(A, function);
            }
            //A.zSum4Neighbors(A,alpha,beta,runs);
            timer.stop().display();
            //PrintToScreen.WriteLine("A="+A);
            A = null;

            double[,] B = factory.make(size, size, value).ToArray();
            timer.reset().start();

            PrintToScreen.WriteLine("benchmarking stencil scimark...");
            for (int i = 0; i < runs; i++)
            {
                //	jnt.scimark2.SOR.execute(omega, B, runs);
            }
            timer.stop().display();


            PrintToScreen.WriteLine("done.");
        }

        /**
         */

        public static void doubleTest25(int size)
        {
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            bool dense = true;
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense)
            {
                factory = DoubleFactory2D.dense;
            }
            else
            {
                factory = DoubleFactory2D.sparse;
            }

            double value = 0.5;
            A = factory.make(size, size, value);
            Property.generateNonSingular(A);
            Timer timer = new Timer().start();

            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(Algebra.ZERO.inverse(A));

            timer.stop().display();

            PrintToScreen.WriteLine("done.");
        }

        /**
         */

        public static void doubleTest26(int size)
        {
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            bool dense = true;
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense)
            {
                factory = DoubleFactory2D.dense;
            }
            else
            {
                factory = DoubleFactory2D.sparse;
            }

            double value = 0.5;
            A = factory.make(size, size, value);
            Property.generateNonSingular(A);
            Timer timer = new Timer().start();

            DoubleMatrix2DComparator fun = new DoubleMatrix2DComparator_();

            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(Algebra.ZERO.inverse(A));

            timer.stop().display();

            PrintToScreen.WriteLine("done.");
        }

        /**
         */

        public static void doubleTest27()
        {
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");

            int m_intRows = 51;
            int columns = 10;
            double[,] trainingSet = new double[columns,m_intRows];
            for (int i = columns; --i >= 0;)
            {
                trainingSet[i, i] = 2.0;
            }

            int patternIndex = 0;
            int unitIndex = 0;

            DoubleMatrix2D patternMatrix = null;
            DoubleMatrix2D transposeMatrix = null;
            DoubleMatrix2D QMatrix = null;
            DoubleMatrix2D inverseQMatrix = null;
            DoubleMatrix2D pseudoInverseMatrix = null;
            DoubleMatrix2D weightMatrix = null;

            // form a matrix with the columns as training vectors
            patternMatrix = DoubleFactory2D.dense.make(m_intRows, columns);

            // copy the patterns into the matrix
            for (patternIndex = 0; patternIndex < columns; patternIndex++)
            {
                for (unitIndex = 0; unitIndex < m_intRows; unitIndex++)
                {
                    patternMatrix.setQuick(unitIndex, patternIndex, trainingSet[patternIndex, unitIndex]);
                }
            }

            transposeMatrix = Algebra.DEFAULT.transpose(patternMatrix);
            QMatrix = Algebra.DEFAULT.mult(transposeMatrix, patternMatrix);
            inverseQMatrix = Algebra.DEFAULT.inverse(QMatrix);
            pseudoInverseMatrix = Algebra.DEFAULT.mult(inverseQMatrix, transposeMatrix);
            weightMatrix = Algebra.DEFAULT.mult(patternMatrix, pseudoInverseMatrix);
            PrintToScreen.WriteLine("done.");
        }

        /**
         */

        public static void doubleTest28()
        {
            double[] data = {1, 2, 3, 4, 5, 6};
            double[,] arrMatrix =
                {
                    {1, 2, 3, 4, 5, 6},
                    {2, 3, 4, 5, 6, 7}
                };
            DoubleFactory2D f = DoubleFactory2D.dense;
            DoubleMatrix1D vector = new DenseDoubleMatrix1D(data);
            DoubleMatrix2D matrix = f.make(arrMatrix);
            DoubleMatrix1D res = vector.like(matrix.Rows());

            matrix.zMult(vector, res);

            PrintToScreen.WriteLine(res);
        }

        /**
         */

        public static void doubleTest28(DoubleFactory2D f)
        {
            double[] data = {1, 2, 3, 4, 5, 6};
            double[,] arrMatrix =
                {
                    {1, 2, 3, 4, 5, 6},
                    {2, 3, 4, 5, 6, 7}
                };

            DoubleMatrix1D vector = new DenseDoubleMatrix1D(data);
            DoubleMatrix2D matrix = f.make(arrMatrix);
            DoubleMatrix1D res = vector.like(matrix.Rows());

            matrix.zMult(vector, res);

            PrintToScreen.WriteLine(res);
        }

        /**
         */

        public static void doubleTest29(int size)
        {
            /*
	
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            bool dense = false;
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense) 
                factory = Factory2D.dense;
            else 
                factory = Factory2D.sparse;
	
            double value = 0.5;	

            DoubleMatrix2D C = Factory2D.dense.sample(size,size,value,1);

            A = factory.make(size,size);
            Console.Write("A assign C... ");
            Timer timer = new Timer().start();
            A.assign(C);
            timer.stop().display();

            Console.Write("A getquick... ");
            timer.reset().start();
            double sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=A.getQuick(i,j);
                }
            }
            timer.stop().display();
            PrintToScreen.WriteLine(sum);
            //PrintToScreen.WriteLine(A);

            Console.Write("sci set3... ");
            JSci.maths.DoubleSparseMatrix B = new JSci.maths.DoubleSparseMatrix(size);
            timer.reset().start();
            //for (int i=size; --i>=0; ) {
            //	for (int j=size; --j>=0; ) {
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    B.setElement3(i,j,C.getQuick(i,j));
                }
            }
            //PrintToScreen.WriteLine(A);
            timer.stop().display();

            Console.Write("sci get3... ");
            timer.reset().start();
            sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=B.getElement3(i,j);
                }
            }
            PrintToScreen.WriteLine(sum);
            timer.stop().display();

            JSci.maths.DoubleVector vec = new JSci.maths.DoubleVector(size);

            Console.Write("sci mult3... ");
            timer.reset().start();
            B.multiply3(vec);
            timer.stop().display();


            PrintToScreen.WriteLine("done.");
            */
        }

        /**
         */

        public static void doubleTest29(int size, DoubleFactory2D f)
        {
            DoubleMatrix2D x = new DenseDoubleMatrix2D(size, size).assign(0.5);
            DoubleMatrix2D matrix = f.sample(size, size, 0.5, 0.001);

            Timer timer = new Timer().start();
            DoubleMatrix2D res = matrix.zMult(x, null);
            timer.stop().display();

            //PrintToScreen.WriteLine(res);
        }

        /**
         */

        public static void doubleTest29(DoubleFactory2D f)
        {
            double[,] data =
                {
                    {6, 5, 4},
                    {7, 6, 3},
                    {6, 5, 4},
                    {7, 6, 3},
                    {6, 5, 4},
                    {7, 6, 3}
                };

            double[,] arrMatrix =
                {
                    {1, 2, 3, 4, 5, 6},
                    {2, 3, 4, 5, 6, 7}
                };

            DoubleMatrix2D x = new DenseDoubleMatrix2D(data);
            DoubleMatrix2D matrix = f.make(arrMatrix);

            DoubleMatrix2D res = matrix.zMult(x, null);

            PrintToScreen.WriteLine(res);
        }

        /**
         */

        public static void doubleTest3()
        {
            int m_intRows = 4;
            int columns = 5; // make a 4*5 matrix
            DoubleMatrix2D master = new DenseDoubleMatrix2D(m_intRows, columns);
            PrintToScreen.WriteLine(master);
            master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            master.viewPart(2, 0, 2, 3).assign(2); // set [2,1] .. [3,3] to 2
            PrintToScreen.WriteLine(Environment.NewLine + master);

            DoubleMatrix2D flip1 = master.viewColumnFlip();
            PrintToScreen.WriteLine("flip around columns=" + flip1);
            DoubleMatrix2D flip2 = flip1.viewRowFlip();
            PrintToScreen.WriteLine("further flip around m_intRows=" + flip2);

            flip2.viewPart(0, 0, 2, 2).assign(3);
            PrintToScreen.WriteLine("master replaced" + master);
            PrintToScreen.WriteLine("flip1 replaced" + flip1);
            PrintToScreen.WriteLine("flip2 replaced" + flip2);


            /*
            DoubleMatrix2D copyPart = master.copyPart(2,1,2,3);
            copyPart.assign(3); // modify an independent copy
            copyPart.set(0,0,4);
            PrintToScreen.WriteLine(Environment.NewLine+copyPart); // has changed
            PrintToScreen.WriteLine(Environment.NewLine+master); // master has not changed

            DoubleMatrix2D view1 = master.viewPart(0,3,4,2); // [0,3] .. [3,4]
            DoubleMatrix2D view2 = view1.viewPart(0,0,4,1); // a view from a view 
            PrintToScreen.WriteLine(Environment.NewLine+view1);
            PrintToScreen.WriteLine(Environment.NewLine+view2);
            */
        }

        /**
         */

        public static void doubleTest30()
        {
            double[,] data =
                {
                    {6, 5},
                    {7, 6},
                };

            double[] x = {1, 2};
            double[] y = {3, 4};

            DoubleMatrix2D A = new DenseDoubleMatrix2D(data);
            SeqBlas.seqBlas.dger(1, new DenseDoubleMatrix1D(x), new DenseDoubleMatrix1D(y), A);

            PrintToScreen.WriteLine(A);
        }

        /**
         */

        public static void doubleTest30(int size)
        {
            int[] values = {0, 2, 3, 5, 7};
            IntArrayList list = new IntArrayList(values);
            int val = 3;
            int sum = 0;
            Timer timer = new Timer().start();
            for (int i = size; --i >= 0;)
            {
                int k = list.binarySearchFromTo(val, 0, values.Length - 1);
                PrintToScreen.WriteLine(list + ", " + val + " --> " + k);
                sum += k;
            }
            timer.stop().display();
            //PrintToScreen.WriteLine("sum = "+sum);


            /*
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            bool dense = false;
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense) 
                factory = Factory2D.dense;
            else 
                factory = Factory2D.sparse;
	
            double value = 0.5;	

            DoubleMatrix2D C = Factory2D.dense.sample(size,size,value,0.01);

            A = factory.make(size,size);
            Timer timer = new Timer().start();
            A.assign(C);
            timer.stop().display();

            timer.reset().start();
            double sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=A.getQuick(i,j);
                }
            }
            timer.stop().display();
            PrintToScreen.WriteLine(sum);
            //PrintToScreen.WriteLine(A);

            JSci.maths.DoubleSparseMatrix B = new JSci.maths.DoubleSparseMatrix(size);
            timer.reset().start();
            for (int i=size; --i>=0; ) {
                for (int j=size; --j>=0; ) {
            //for (int i=0; i<size; i++) {
            //	for (int j=0; j<size; j++ ) {
                    B.setElement2(i,j,C.getQuick(i,j));
                }
            }
            //PrintToScreen.WriteLine(A);
            timer.stop().display();

            timer.reset().start();
            sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=B.getElement2(i,j);
                }
            }
            PrintToScreen.WriteLine(sum);
            timer.stop().display();

            PrintToScreen.WriteLine("done.");

            */
        }

        /**
         */

        public static void doubleTest30(int size, int val)
        {
            //int[] values = { 0, 2};
            int[] values = {2};
            IntArrayList list = new IntArrayList(values);
            int l = values.Length - 1;
            int sum = 0;
            Timer timer = new Timer().start();
            for (int i = size; --i >= 0;)
            {
                int k = Sorting.binarySearchFromTo(values, val, 0, l);
                //int k = list.binarySearchFromTo(val,0,l);
                //PrintToScreen.WriteLine(list+", "+val+" --> i="+k+", -i-1="+(-k-1));
                sum += k;
            }
            timer.stop().display();
            PrintToScreen.WriteLine("sum = " + sum);


            /*
            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("initializing...");
            bool dense = false;
            DoubleMatrix2D A;
            DoubleFactory2D factory;
            if (dense) 
                factory = Factory2D.dense;
            else 
                factory = Factory2D.sparse;
	
            double value = 0.5;	

            DoubleMatrix2D C = Factory2D.dense.sample(size,size,value,0.01);

            A = factory.make(size,size);
            Timer timer = new Timer().start();
            A.assign(C);
            timer.stop().display();

            timer.reset().start();
            double sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=A.getQuick(i,j);
                }
            }
            timer.stop().display();
            PrintToScreen.WriteLine(sum);
            //PrintToScreen.WriteLine(A);

            JSci.maths.DoubleSparseMatrix B = new JSci.maths.DoubleSparseMatrix(size);
            timer.reset().start();
            for (int i=size; --i>=0; ) {
                for (int j=size; --j>=0; ) {
            //for (int i=0; i<size; i++) {
            //	for (int j=0; j<size; j++ ) {
                    B.setElement2(i,j,C.getQuick(i,j));
                }
            }
            //PrintToScreen.WriteLine(A);
            timer.stop().display();

            timer.reset().start();
            sum=0;
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++ ) {
                    sum+=B.getElement2(i,j);
                }
            }
            PrintToScreen.WriteLine(sum);
            timer.stop().display();

            PrintToScreen.WriteLine("done.");

            */
        }

        /**
         */

        public static void doubleTest31(int size)
        {
            PrintToScreen.WriteLine("\ninit");
            DoubleMatrix1D a = DoubleFactory1D.dense.descending(size);
            DoubleMatrix1D b = new WrapperDoubleMatrix1D(a);
            DoubleMatrix1D c = b.viewPart(2, 3);
            DoubleMatrix1D d = c.viewFlip();
            //DoubleMatrix1D c = b.viewFlip();
            //DoubleMatrix1D d = c.viewFlip();
            d.set(0, 99);
            b = b.viewSorted();
            PrintToScreen.WriteLine("a = " + a);
            PrintToScreen.WriteLine("b = " + b);
            PrintToScreen.WriteLine("c = " + c);
            PrintToScreen.WriteLine("d = " + d);

            PrintToScreen.WriteLine("done");
        }

        /**
         */

        public static void doubleTest32()
        {
            double[,] data =
                {
                    {1, 4, 0},
                    {6, 2, 5},
                    {0, 7, 3},
                    {0, 0, 8},
                    {0, 0, 0},
                    {0, 0, 0}
                };

            DoubleMatrix2D x = new TridiagonalDoubleMatrix2D(data);


            PrintToScreen.WriteLine("\n\n\n" + x);
            PrintToScreen.WriteLine(Environment.NewLine + new DenseDoubleMatrix2D(data));
        }

        /**
         */

        public static void doubleTest33()
        {
            double nan = double.NaN;
            //double inf = Double.PositiveInfinity;
            double ninf = Double.NegativeInfinity;

            double[,] data = {{ninf, nan}};
            /*
            { 
                { 1, 4, 0 },
                { 6, 2, 5 },
                { 0, 7, 3 },
                { 0, 0, 8 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            */

            DoubleMatrix2D x = new DenseDoubleMatrix2D(data);

            PrintToScreen.WriteLine("\n\n\n" + x);
            PrintToScreen.WriteLine(Environment.NewLine + x.Equals(ninf));
        }

        /**
         */

        public static void doubleTest34()
        {
            double[,] data =
                {
                    {3, 0, 0, 0},
                    {0, 4, 2, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0},
                };

            DoubleMatrix2D A = new DenseDoubleMatrix2D(data);
            Property.DEFAULT.generateNonSingular(A);
            DoubleMatrix2D inv = Algebra.DEFAULT.inverse(A);


            PrintToScreen.WriteLine("\n\n\n" + A);
            PrintToScreen.WriteLine(Environment.NewLine + inv);
            DoubleMatrix2D B = A.zMult(inv, null);
            PrintToScreen.WriteLine(B);
            if (!(B.Equals(DoubleFactory2D.dense.identity(A.m_intRows))))
            {
                throw new HCException();
            }
        }

        /**
         * Title:        Aero3D<p>
         * Description:  A Program to analyse aeroelestic evects in transonic wings<p>
         * Copyright:    Copyright (c) 1998<p>
         * HC:      PIERSOL Engineering Inc.<p>
         * @author John R. Piersol
         * @version
         */

        public static void doubleTest35()
        {
            /*
             int DOF = 200;
             MersenneTwister RANDOM = new MersenneTwister();
             Algebra ALGEBRA = new Algebra();
	
            PrintToScreen.WriteLine("\n\n\nStarting...");
            double[,] k = randomMatrix(DOF, RANDOM);
            DoubleMatrix2D kd = new DenseDoubleMatrix2D(k);
            Jama.Matrix km = new Jama.Matrix(k);


	


            DoubleMatrix2D coltL = new LUDecomposition(kd).getL();
            DoubleMatrix2D coltU = new LUDecomposition(kd).getU();
            Jama.Matrix jamaL = new Jama.LUDecomposition(km).getL();
            Jama.Matrix jamaU = new Jama.LUDecomposition(km).getU();

            PrintToScreen.WriteLine(coltL.Equals(kd.like().assign(jamaL.getArrayCopy())));
            PrintToScreen.WriteLine(coltL.aggregate(Functions.plus,Functions.abs));
            double s = 0;
            double[] temp2 = jamaL.getColumnPackedCopy();
            for (int i = 0, n = temp2.Length; i < n; ++i) s += Math.Abs(temp2[i]);
            PrintToScreen.WriteLine(s);

            PrintToScreen.WriteLine(coltU.Equals(kd.like().assign(jamaU.getArrayCopy())));
            PrintToScreen.WriteLine(coltU.aggregate(Functions.plus,Functions.abs));
            s = 0;
            temp2 = jamaU.getColumnPackedCopy();
            for (int i = 0, n = temp2.Length; i < n; ++i) s += Math.Abs(temp2[i]);
            PrintToScreen.WriteLine(s);

            //PrintToScreen.WriteLine("colt="+new LUDecomposition(kd).ToString());
            //PrintToScreen.WriteLine("jama="+new Jama.LUDecomposition(km).ToString());



            Jama.Matrix kmi = km.inverse();

            DoubleMatrix2D kdi = Algebra.DEFAULT.inverse(kd);
            DoubleMatrix2D checkColt = Algebra.DEFAULT.mult(kd, kdi);
            PrintToScreen.WriteLine("Colt checksum = " + checkColt.aggregate(Functions.plus,Functions.abs) + ", correct = " + DOF);

            Jama.Matrix checkJama = kmi.times(km);
            double checksum = 0;
            double[] temp = checkJama.getColumnPackedCopy();
            for (int i = 0, n = temp.Length; i < n; ++i) checksum += Math.Abs(temp[i]);
            PrintToScreen.WriteLine("Jama checksum = " + checksum + ", correct = " + DOF);

            PrintToScreen.WriteLine("done\n");
            */
        }

        /**
         * Title:        Aero3D<p>
         * Description:  A Program to analyse aeroelestic evects in transonic wings<p>
         * Copyright:    Copyright (c) 1998<p>
         * HC:      PIERSOL Engineering Inc.<p>
         * @author John R. Piersol
         * @version
         */

        public static void doubleTest36()
        {
            double[] testSort = new double[5];
            testSort[0] = 5;
            testSort[1] = double.NaN;
            testSort[2] = 2;
            testSort[3] = double.NaN;
            testSort[4] = 1;
            DoubleMatrix1D doubleDense = new DenseDoubleMatrix1D(testSort);
            PrintToScreen.WriteLine("orig = " + doubleDense);
            doubleDense = doubleDense.viewSorted();
            doubleDense.ToArray(testSort);
            PrintToScreen.WriteLine("sort = " + doubleDense);
            PrintToScreen.WriteLine("done\n");
        }

        /**
         */

        public static void doubleTest4()
        {
            int m_intRows = 4;
            int columns = 5; // make a 4*5 matrix
            DoubleMatrix2D master = new DenseDoubleMatrix2D(m_intRows, columns);
            PrintToScreen.WriteLine(master);
            master.assign(1); // set all cells to 1
            DoubleMatrix2D view = master.viewPart(2, 0, 2, 3).assign(2);
            PrintToScreen.WriteLine(Environment.NewLine + master);
            PrintToScreen.WriteLine(Environment.NewLine + view);
            Transform.mult(view, 3);
            PrintToScreen.WriteLine(Environment.NewLine + master);
            PrintToScreen.WriteLine(Environment.NewLine + view);


            /*
            DoubleMatrix2D copyPart = master.copyPart(2,1,2,3);
            copyPart.assign(3); // modify an independent copy
            copyPart.set(0,0,4);
            PrintToScreen.WriteLine(Environment.NewLine+copyPart); // has changed
            PrintToScreen.WriteLine(Environment.NewLine+master); // master has not changed

            DoubleMatrix2D view1 = master.viewPart(0,3,4,2); // [0,3] .. [3,4]
            DoubleMatrix2D view2 = view1.viewPart(0,0,4,1); // a view from a view 
            PrintToScreen.WriteLine(Environment.NewLine+view1);
            PrintToScreen.WriteLine(Environment.NewLine+view2);
            */
        }

        /**
         */

        public static void doubleTest5()
        {
            /*
        int m_intRows = 4;
        int columns = 5; // make a 4*5 matrix
        DoubleMatrix2D master = new DenseDoubleMatrix2D(m_intRows,columns);
        PrintToScreen.WriteLine(master);
        master.assign(1); // set all cells to 1
        DoubleMatrix2D view = master.viewPart(2,0,2,3);
        view.assign(0);
        for (int i=0; i<m_intRows; i++) {
            for (int j=0; j<columns; j++) {
                bool hasIndex = view.hasIndex(master.index(i,j));
                PrintToScreen.WriteLine("("+i+","+j+"):"+hasIndex);
            }
        }
        PrintToScreen.WriteLine(Environment.NewLine+master);
        PrintToScreen.WriteLine(Environment.NewLine+view);
        IntArrayList rowList = new IntArrayList();
        IntArrayList columnList = new IntArrayList();
        DoubleArrayList valueList = new DoubleArrayList();
        master.getNonZeros(rowList,columnList,valueList);
        PrintToScreen.WriteLine(rowList);
        PrintToScreen.WriteLine(columnList);
        PrintToScreen.WriteLine(valueList);
        PrintToScreen.WriteLine(master.toStringSparse());
        */
        }

        /**
         */

        public static void doubleTest6()
        {
            int m_intRows = 4;
            int columns = 5; // make a 4*5 matrix
            DoubleMatrix2D master = Factory2D.ascending(m_intRows, columns);
            //master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            master.viewPart(2, 0, 2, 3).assign(2); // set [2,1] .. [3,3] to 2
            PrintToScreen.WriteLine(Environment.NewLine + master);

            int[] indexes = {0, 1, 3, 0, 1, 2};
            DoubleMatrix1D view1 = master.viewRow(0).viewSelection(indexes);
            PrintToScreen.WriteLine("view1=" + view1);
            DoubleMatrix1D view2 = view1.viewPart(0, 3);
            PrintToScreen.WriteLine("view2=" + view2);

            view2.viewPart(0, 2).assign(-1);
            PrintToScreen.WriteLine("master replaced" + master);
            PrintToScreen.WriteLine("flip1 replaced" + view1);
            PrintToScreen.WriteLine("flip2 replaced" + view2);


            /*
            DoubleMatrix2D copyPart = master.copyPart(2,1,2,3);
            copyPart.assign(3); // modify an independent copy
            copyPart.set(0,0,4);
            PrintToScreen.WriteLine(Environment.NewLine+copyPart); // has changed
            PrintToScreen.WriteLine(Environment.NewLine+master); // master has not changed

            DoubleMatrix2D view1 = master.viewPart(0,3,4,2); // [0,3] .. [3,4]
            DoubleMatrix2D view2 = view1.viewPart(0,0,4,1); // a view from a view 
            PrintToScreen.WriteLine(Environment.NewLine+view1);
            PrintToScreen.WriteLine(Environment.NewLine+view2);
            */
        }

        /**
         */

        public static void doubleTest7()
        {
            int m_intRows = 4;
            int columns = 5; // make a 4*5 matrix
            DoubleMatrix2D master = Factory2D.ascending(m_intRows, columns);
            //master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            //master.viewPart(2,0,2,3).assign(2); // set [2,1] .. [3,3] to 2
            //PrintToScreen.WriteLine(Environment.NewLine+master);

            int[] rowIndexes = {0, 1, 3, 0};
            int[] columnIndexes = {0, 2};
            DoubleMatrix2D view1 = master.viewSelection(rowIndexes, columnIndexes);
            PrintToScreen.WriteLine("view1=" + view1);
            DoubleMatrix2D view2 = view1.viewPart(0, 0, 2, 2);
            PrintToScreen.WriteLine("view2=" + view2);

            view2.assign(-1);
            PrintToScreen.WriteLine("master replaced" + master);
            PrintToScreen.WriteLine("flip1 replaced" + view1);
            PrintToScreen.WriteLine("flip2 replaced" + view2);
        }

        /**
         */

        public static void doubleTest8()
        {
            int m_intRows = 2;
            int columns = 3; // make a 4*5 matrix
            DoubleMatrix2D master = Factory2D.ascending(m_intRows, columns);
            //master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            //master.viewPart(2,0,2,3).assign(2); // set [2,1] .. [3,3] to 2
            //PrintToScreen.WriteLine(Environment.NewLine+master);

            DoubleMatrix2D view1 = master.viewDice();
            PrintToScreen.WriteLine("view1=" + view1);
            DoubleMatrix2D view2 = view1.viewDice();
            PrintToScreen.WriteLine("view2=" + view2);

            view2.assign(-1);
            PrintToScreen.WriteLine("master replaced" + master);
            PrintToScreen.WriteLine("flip1 replaced" + view1);
            PrintToScreen.WriteLine("flip2 replaced" + view2);
        }

        /**
         */

        public static void doubleTest9()
        {
            int m_intRows = 2;
            int columns = 3; // make a 4*5 matrix
            DoubleMatrix2D master = Factory2D.ascending(m_intRows, columns);
            //master.assign(1); // set all cells to 1
            PrintToScreen.WriteLine(Environment.NewLine + master);
            //master.viewPart(2,0,2,3).assign(2); // set [2,1] .. [3,3] to 2
            //PrintToScreen.WriteLine(Environment.NewLine+master);

            DoubleMatrix2D view1 = master.viewRowFlip();
            PrintToScreen.WriteLine("view1=" + view1);
            DoubleMatrix2D view2 = view1.viewRowFlip();
            PrintToScreen.WriteLine("view2=" + view2);

            view2.assign(-1);
            PrintToScreen.WriteLine("master replaced" + master);
            PrintToScreen.WriteLine("flip1 replaced" + view1);
            PrintToScreen.WriteLine("flip2 replaced" + view2);
        }

        public static void doubleTestQR()
        {
            // test case0...
            double[] x0 = {-6.221564, -9.002113, 2.678001, 6.483597, -7.934148};
            double[] y0 = {-7.291898, -7.346928, 0.520158, 5.012548, -8.223725};
            double[] x1 = {1.185925, -2.523077, 0.135380, 0.412556, -2.980280};
            double[] y1 = {13.561087, -15.204410, 16.496829, 16.470860, 0.822198};

            solve(x1.Length, x1, y1);
            solve(x0.Length, x0, y0);
        }

        /**
         */

        public static void main(string[] args)
        {
            int runs = int.Parse(args[0]);
            int val = int.Parse(args[1]);
            doubleTest30(runs, val);
            /*
            int runs = int.Parse(args[0]);
            int size = int.Parse(args[1]);
            double nonZeroFraction = (args[2]);
            bool dense = args[3].Equals("dense");
            //doubleTest23(runs, size, nonZeroFraction, dense);
            doubleTest24(runs, size, dense);
            */
        }

        public static double[,] randomMatrix(int dof, MersenneTwister RANDOM)
        {
            double[,] m = new double[dof,dof];
            /*
            for (int i = 0; i < dof; ++i) {
                for (int j = i - 1, n = i + 1; j <= n; ++j) {
                    if (j < dof && j > -1)
                        m[i,j] = RANDOM.NextDouble();
                }
            }
            */
            for (int i = 0; i < dof; ++i)
            {
                for (int j = 0; j < dof; j++)
                {
                    m[i, j] = 5;
                }
            }
            //        for (int i = 0; i < dof; ++i)
            //            for (int j = 0; j < dof; ++j) m[i,j] = RANDOM.NextDouble();
            return m;
        }

        public static void solve(int numpnt, double[] x, double[] y)
        {
            /*
            // create the matrix object
            DoubleMatrix2D A = new DenseDoubleMatrix2D(numpnt, 5);
            DoubleMatrix2D B = new DenseDoubleMatrix2D(numpnt, 1);
            //fillout the matrix
            for (int i = 0; i < numpnt; i++) {
                A.setQuick(i, 0, x[i] * y[i]);
                A.setQuick(i, 1, y[i] * y[i]);
                A.setQuick(i, 2, x[i]);
                A.setQuick(i, 3, y[i]);
                A.setQuick(i, 4, 1.0);
                B.setQuick(i, 0, -x[i] * x[i]);
            }
            PrintToScreen.WriteLine(A);
            //test the matrix condition
            SingularValueDecomposition svd = new SingularValueDecomposition(A);
            PrintToScreen.WriteLine(svd);
            // Using Algebra to solve the equation
            Algebra alg = new Algebra();
            DoubleMatrix2D resAlg = alg.solve(A.Copy(), B.Copy());
            PrintToScreen.WriteLine("Using Algebra...");
            PrintToScreen.WriteLine(resAlg);
            // Using QRDecomposition to solve the problem..
            QRDecomposition qrd = new QRDecomposition(A);
            DoubleMatrix2D resQRD = qrd.solve(B);
            PrintToScreen.WriteLine("Using QRDecomposition...");
            PrintToScreen.WriteLine(resQRD);
            // Using Jama.QRDecomposition to solve the problem..
            Jama.QRDecomposition qrdJama = new Jama.QRDecomposition(new Jama.Matrix(A.ToArray()));
            resQRD = new DenseDoubleMatrix2D(qrdJama.solve(new Jama.Matrix(B.ToArray())).getArrayCopy());
            PrintToScreen.WriteLine("Using Jama.QRDecomposition...");
            PrintToScreen.WriteLine(resQRD);
            */
        }

        /**
         */

        public static void testLU()
        {
            double[,] vals = {
                                 {-0.074683, 0.321248, -0.014656, 0.286586, 0},
                                 {-0.344852, -0.16278, 0.173711, 0.00064, 0},
                                 {-0.181924, -0.092926, 0.184153, 0.177966, 1},
                                 {-0.166829, -0.10321, 0.582301, 0.142583, 0},
                                 {0, -0.112952, -0.04932, -0.700157, 0},
                                 {0, 0, 0, 0, 0}
                             };

            DoubleMatrix2D H = new DenseDoubleMatrix2D(vals); // see values below...
            PrintToScreen.WriteLine("\nHplus=" + H.viewDice().zMult(H, null));

            DoubleMatrix2D Hplus = Algebra.DEFAULT.inverse(H.viewDice().zMult(H, null)).zMult(H.viewDice(), null);
            Hplus.assign(Functions.round(1.0E-10));
            PrintToScreen.WriteLine("\nHplus=" + Hplus);

            /*
    DoubleMatrix2D HtH = new DenseDoubleMatrix2D( 5, 5 );
    DoubleMatrix2D Hplus = new DenseDoubleMatrix2D( 5, 6 );
    LUDecompositionQuick LUD = new LUDecompositionQuick();
            //H.zMult( H, HtH, 1, 0, true, false );
            //DoubleMatrix2D res = Algebra.DEFAULT.inverse(HtH).zMult(H,null,1,0,false,true);
            LUD.decompose( HtH );
            // first fill Hplus with the transpose of H...
            for (int i = 0; i < 6; i++ ) {
                for ( int j = 0; j < 5; j++ ) {
                    Hplus.set( j, i, H.get( i, j ) );
                }
            }
            LUD.solve( Hplus );

            DoubleMatrix2D perm = Algebra.DEFAULT.permute(Hplus, null,LUD.getPivot());
            DoubleMatrix2D inv = Algebra.DEFAULT.inverse(HtH);//.zMult(H,null,1,0,false,true);
            */

            // in matlab...
            // Hplus = inv(H' * H) * H'

            //PrintToScreen.WriteLine("\nLU="+LUD);
            //PrintToScreen.WriteLine("\nHplus="+Hplus);
            //PrintToScreen.WriteLine("\nperm="+perm);
            //PrintToScreen.WriteLine("\ninv="+inv);
            //PrintToScreen.WriteLine("\nres="+res);
        }

        /**
         */

        public static void testMax()
        {
            double[] temp = new double[2];

            temp[0] = 8.9;
            temp[1] = 1;

            DenseDoubleMatrix1D d1Double = new DenseDoubleMatrix1D(temp);

            DynamicBin1D d1ynamicBin = Statistic.bin(d1Double);

            double max = d1ynamicBin.max();

            PrintToScreen.WriteLine("Max = " + max);
        }
    }
}
