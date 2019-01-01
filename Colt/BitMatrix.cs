#region

using System;
using HC.Core.Exceptions;

namespace HC.Analytics.Colt
{
//using System.Drawingg;

    #endregion

/*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
//package bitvector;

//import java.awt.Rectangle;
/**
     * Fixed sized (non resizable) n*m bit matrix.
     * A bit matrix has a number of columns and rows, which are assigned upon instance construction - The matrix's size is then <tt>Columns()*Rows()</tt>.
     * Bits are accessed via <tt>(column,row)</tt> coordinates.
     * <p>
     * Individual bits can be examined, set, or cleared.
     * Rectangular parts (boxes) can quickly be extracted, copied and replaced.
     * Quick iteration over boxes is provided by optimized internal iterators (<tt>forEach()</tt> methods).
     * One <code>BitMatrix</code> may be used to modify the contents of another 
     * <code>BitMatrix</code> through logical AND, OR, XOR and other similar operations.
     * <p>
     * Legal coordinates range from <tt>[0,0]</tt> to <tt>[Columns()-1,Rows()-1]</tt>.
     * Any attempt to access a bit at a coordinate <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt> will throw an <tt>HCException</tt>.
     * Operations involving two bit matrices (like AND, OR, XOR, etc.) will throw an <tt>ArgumentException</tt> if both bit matrices do not have the same number of columns and rows.
     * <p>
     * If you need extremely quick access to individual bits: Although getting and setting individual bits with methods <tt>get(...)</tt> and <tt>put(...)</tt> is quick, it is even quicker (<b>but not safe</b>) to use <tt>getQuick(...)</tt> and <tt>putQuick(...)</tt>.
     * <p>
     * <b>Note</b> that this implementation is not lock.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     * @see     BitVector
     * @see     QuickBitVector
     * @see     BitSet
     */

    [Serializable]
    public class BitMatrix : PersistentObject
    {
        public long[] m_bits;
        public int m_intColumns;
        public int m_intRows;

        /*
         * The bits of this matrix.
         * bits are stored in row major, i.e.
         * bitIndex==row*columns + column
         * columnOf(bitIndex)==bitIndex%columns
         * rowOf(bitIndex)==bitIndex/columns
         */
        /**
         * Constructs a bit matrix with a given number of columns and rows. All bits are initially <tt>false</tt>.
         * @param columns the number of columns the matrix shall have.
         * @param rows the number of rows the matrix shall have.
         * @throws ArgumentException if <tt>columns &lt; 0 || rows &lt; 0</tt>.
         */

        public BitMatrix(int columns, int rows)
        {
            elements(QuickBitVector.makeBitVector(columns*rows, 1), columns, rows);
        }

        /**
         * Performs a logical <b>AND</b> of the receiver with another bit matrix.
         * The receiver is modified so that a bit in it has the
         * value <code>true</code> if and only if it already had the 
         * value <code>true</code> and the corresponding bit in the other bit matrix
         * argument has the value <code>true</code>.
         *
         * @param   other   a bit matrix.
         * @throws ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>.
         */

        public void and(BitMatrix other)
        {
            checkDimensionCompatibility(other);
            toBitVector().and(other.toBitVector());
        }

        /**
         * Clears all of the bits in receiver whose corresponding
         * bit is set in the other bit matrix.
         * In other words, determines the difference (A\B) between two bit matrices.
         *
         * @param   other   a bit matrix with which to mask the receiver.
         * @throws ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>.
         */

        public void andNot(BitMatrix other)
        {
            checkDimensionCompatibility(other);
            toBitVector().andNot(other.toBitVector());
        }

        /**
         * Returns the number of bits currently in the <tt>true</tt> state.
         * Optimized for speed. Particularly quick if the receiver is either sparse or dense.
         */

        public int cardinality()
        {
            return toBitVector().cardinality();
        }

        /**
         * Sanity check for operations requiring matrices with the same number of columns and rows.
         */

        public void checkDimensionCompatibility(BitMatrix other)
        {
            if (m_intColumns != other.Columns() || m_intRows != other.Rows())
            {
                throw new ArgumentException("Incompatible dimensions: (columns,rows)=(" + m_intColumns + "," + m_intRows +
                                            "), (other.columns,other.rows)=(" + other.Columns() + "," + other.Rows() +
                                            ")");
            }
        }

        /**
         * Clears all bits of the receiver.
         */

        public void Clear()
        {
            toBitVector().Clear();
        }

        /**
         * Cloning this <code>BitMatrix</code> produces a new <code>BitMatrix</code> 
         * that is equal to it.
         * The Clone of the bit matrix is another bit matrix that has exactly the 
         * same bits set to <code>true</code> as this bit matrix and the same 
         * number of columns and rows. 
         *
         * @return  a Clone of this bit matrix.
         */

        public new Object Clone()
        {
            BitMatrix Clone = (BitMatrix) base.Clone();
            if (m_bits != null)
            {
                Clone.m_bits = (long[]) m_bits.Clone();
            }
            return Clone;
        }

        /**
         * Returns the number of columns of the receiver.
         */

        public int Columns()
        {
            return m_intColumns;
        }

        /**
         * Checks whether the receiver contains the given box.
         */

        public void containsBox(int column, int row, int width, int height)
        {
            if (column < 0 || column + width > m_intColumns || row < 0 || row + height > m_intRows)
            {
                throw new HCException("column:" + column + ", row:" + row + " ,width:" + width + ", height:" + height);
            }
        }

        /**
         * Returns a shallow Clone of the receiver; calls <code>Clone()</code> and casts the result.
         *
         * @return  a shallow Clone of the receiver.
         */

        public BitMatrix Copy()
        {
            return (BitMatrix) Clone();
        }

        public long[] elements()
        {
            return m_bits;
        }

        /**
         * You normally need not use this method. Use this method only if performance is critical. 
         * Sets the bit matrix's backing bits, columns and rows.
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         * @throws ArgumentException if <tt>columns &lt; 0 || rows &lt; 0 || columns*rows &gt; bits.Length*64</tt>
         */

        public void elements(long[] bits, int columns, int rows)
        {
            if (columns < 0 || columns < 0 || columns*rows > bits.Length*QuickBitVector.BITS_PER_UNIT)
            {
                throw new ArgumentException();
            }
            m_bits = bits;
            m_intColumns = columns;
            m_intRows = rows;
        }

        /**
         * Compares this object against the specified object.
         * The result is <code>true</code> if and only if the argument is 
         * not <code>null</code> and is a <code>BitMatrix</code> object
         * that has the same number of columns and rows as the receiver and 
         * that has exactly the same bits set to <code>true</code> as the receiver.
         * @param   obj   the object to Compare with.
         * @return  <code>true</code> if the objects are the same;
         *          <code>false</code> otherwise.
         */

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is BitMatrix))
            {
                return false;
            }
            if (this == obj)
            {
                return true;
            }

            BitMatrix other = (BitMatrix) obj;
            if (m_intColumns != other.Columns() || m_intRows != other.Rows())
            {
                return false;
            }

            return toBitVector().Equals(other.toBitVector());
        }

        /**
         * Applies a procedure to each coordinate that holds a bit in the given state.
         * Iterates rowwise downwards from [Columns()-1,Rows()-1] to [0,0].
         * Useful, for example, if you want to copy bits into an image or somewhere else.
         * Optimized for speed. Particularly quick if one of the following conditions holds
         * <ul>
         * <li><tt>state==true</tt> and the receiver is sparse (<tt>cardinality()</tt> is small compared to <tt>Size()</tt>).
         * <li><tt>state==false</tt> and the receiver is dense (<tt>cardinality()</tt> is large compared to <tt>Size()</tt>).
         * </ul>
         *
         * @param state element to search for.
         * @param procedure a procedure object taking as first argument the current column and as second argument the current row. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEachCoordinateInState(bool state, IntIntProcedure procedure)
        {
            /*
            this is equivalent to the low level version below, apart from that it iterates in the reverse oder and is slower.
            if (Size()==0) return true;
            BitVector vector = toBitVector();
            return vector.forEachIndexFromToInState(0,Size()-1,state,
                new IntFunction() {
                    public bool Apply(int index) {
                        return function.Apply(index%columns, index/columns);
                    }
                }
            );
            */

            //low level implementation for speed.
            if (Size() == 0)
            {
                return true;
            }
            BitVector vector = new BitVector(m_bits, Size());

            long[] theBits = m_bits;

            int column = m_intColumns - 1;
            int row = m_intRows - 1;

            // for each coordinate of bits of partial unit
            long val = theBits[m_bits.Length - 1];
            for (int j = vector.numberOfBitsInPartialUnit(); --j >= 0;)
            {
                long mask = val & (1L << j);
                if ((state && (mask != 0L)) || ((!state) && (mask == 0L)))
                {
                    if (!procedure.Apply(column, row))
                    {
                        return false;
                    }
                }
                if (--column < 0)
                {
                    column = m_intColumns - 1;
                    --row;
                }
            }


            // for each coordinate of bits of full units
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;
            long comparator;
            if (state)
            {
                comparator = 0L;
            }
            else
            {
                comparator = ~0L; // all 64 bits set
            }

            for (int i = vector.numberOfFullUnits(); --i >= 0;)
            {
                val = theBits[i];
                if (val != comparator)
                {
                    // at least one element within current unit matches.
                    // iterate over all bits within current unit.
                    if (state)
                    {
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (((val & (1L << j))) != 0L)
                            {
                                if (!procedure.Apply(column, row))
                                {
                                    return false;
                                }
                            }
                            if (--column < 0)
                            {
                                column = m_intColumns - 1;
                                --row;
                            }
                        }
                    }
                    else
                    {
                        // unrolled comparison for speed.
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (((val & (1L << j))) == 0L)
                            {
                                if (!procedure.Apply(column, row))
                                {
                                    return false;
                                }
                            }
                            if (--column < 0)
                            {
                                column = m_intColumns - 1;
                                --row;
                            }
                        }
                    }
                }
                else
                {
                    // no element within current unit matches --> skip unit
                    column -= bitsPerUnit;
                    if (column < 0)
                    {
                        // avoid implementation with *, /, %
                        column += bitsPerUnit;
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (--column < 0)
                            {
                                column = m_intColumns - 1;
                                --row;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /**
         * Returns from the receiver the value of the bit at the specified coordinate.
         * The value is <tt>true</tt> if this bit is currently set; otherwise, returns <tt>false</tt>.
         *
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @return    the value of the bit at the specified coordinate.
         * @throws	HCException if <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt>
         */

        public bool get(int column, int row)
        {
            if (column < 0 || column >= m_intColumns || row < 0 || row >= m_intRows)
            {
                throw new HCException("column:" + column + ", row:" + row);
            }
            return QuickBitVector.get(m_bits, row*m_intColumns + column);
        }

        /**
         * Returns from the receiver the value of the bit at the specified coordinate; <b>WARNING:</b> Does not check preconditions.
         * The value is <tt>true</tt> if this bit is currently set; otherwise, returns <tt>false</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid values without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>column&gt;=0 && column&lt;Columns() && row&gt;=0 && row&lt;Rows()</tt>.
         *
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @return    the value of the bit at the specified coordinate.
         */

        public bool getQuick(int column, int row)
        {
            return QuickBitVector.get(m_bits, row*m_intColumns + column);
        }

        /**
         * Returns a hash code value for the receiver. 
         */

        public int hashCode()
        {
            return toBitVector().hashCode();
        }

        /**
         * Performs a logical <b>NOT</b> on the bits of the receiver.
         */

        public void not()
        {
            toBitVector().not();
        }

        /**
         * Performs a logical <b>OR</b> of the receiver with another bit matrix.
         * The receiver is modified so that a bit in it has the
         * value <code>true</code> if and only if it either already had the 
         * value <code>true</code> or the corresponding bit in the other bit matrix
         * argument has the value <code>true</code>.
         *
         * @param   other   a bit matrix.
         * @throws ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>.
         */

        public void or(BitMatrix other)
        {
            checkDimensionCompatibility(other);
            toBitVector().or(other.toBitVector());
        }

        /**
         * Constructs and returns a new matrix with <tt>width</tt> columns and <tt>height</tt> rows which is a copy of the contents of the given box.
         * The box ranges from <tt>[column,row]</tt> to <tt>[column+width-1,row+height-1]</tt>, all inclusive.
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     width   the width of the box.
         * @param     height   the height of the box.
         * @throws	HCException if <tt>column&lt;0 || column+width&gt;Columns() || row&lt;0 || row+height&gt;Rows()</tt>
         */

        public BitMatrix part(int column, int row, int width, int height)
        {
            if (column < 0 || column + width > m_intColumns || row < 0 || row + height > m_intRows)
            {
                throw new HCException("column:" + column + ", row:" + row + " ,width:" + width + ", height:" + height);
            }
            if (width <= 0 || height <= 0)
            {
                return new BitMatrix(0, 0);
            }

            BitMatrix subMatrix = new BitMatrix(width, height);
            subMatrix.replaceBoxWith(0, 0, width, height, this, column, row);
            return subMatrix;
        }

        /**
         * Sets the bit at the specified coordinate to the state specified by <tt>value</tt>.
         *
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param    value the value of the bit to be copied into the specified coordinate.
         * @throws	HCException if <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt>
         */

        public void put(int column, int row, bool value)
        {
            if (column < 0 || column >= m_intColumns || row < 0 || row >= m_intRows)
            {
                throw new HCException("column:" + column + ", row:" + row);
            }
            QuickBitVector.put(m_bits, row*m_intColumns + column, value);
        }

        /**
         * Sets the bit at the specified coordinate to the state specified by <tt>value</tt>; <b>WARNING:</b> Does not check preconditions.
         *
         * <p>Provided with invalid parameters this method may return invalid values without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>column&gt;=0 && column&lt;Columns() && row&gt;=0 && row&lt;Rows()</tt>.
         *
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param    value the value of the bit to be copied into the specified coordinate.
         */

        public void putQuick(int column, int row, bool value)
        {
            QuickBitVector.put(m_bits, row*m_intColumns + column, value);
        }

        /**
         * Replaces a box of the receiver with the contents of another matrix's box.
         * The source box ranges from <tt>[sourceColumn,sourceRow]</tt> to <tt>[sourceColumn+width-1,sourceRow+height-1]</tt>, all inclusive.
         * The destination box ranges from <tt>[column,row]</tt> to <tt>[column+width-1,row+height-1]</tt>, all inclusive.
         * Does nothing if <tt>width &lt;= 0 || height &lt;= 0</tt>.
         * If <tt>source==this</tt> and the source and destination box intersect in an ambiguous way, then replaces as if using an intermediate auxiliary copy of the receiver.
         *
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     width   the width of the box.
         * @param     height   the height of the box.
         * @param     source   the source matrix to copy from(may be identical to the receiver).
         * @param     sourceColumn   the index of the source column-coordinate.
         * @param     sourceRow   the index of the source row-coordinate.
         * @throws	HCException if <tt>column&lt;0 || column+width&gt;Columns() || row&lt;0 || row+height&gt;Rows()</tt>
         * @throws	HCException if <tt>sourceColumn&lt;0 || sourceColumn+width&gt;source.Columns() || sourceRow&lt;0 || sourceRow+height&gt;source.Rows()</tt>
         */

        public void replaceBoxWith(int column, int row, int width, int height, BitMatrix source, int sourceColumn,
            int sourceRow)
        {
            containsBox(column, row, width, height);
            source.containsBox(sourceColumn, sourceRow, width, height);
            if (width <= 0 || height <= 0)
            {
                return;
            }

            if (source == this)
            {
                Rectangle destRect = new Rectangle(column, row, width, height);
                Rectangle sourceRect = new Rectangle(sourceColumn, sourceRow, width, height);
                if (destRect.IntersectsWith(sourceRect))
                {
                    // dangerous intersection
                    source = source.Copy();
                }
            }

            BitVector sourceVector = source.toBitVector();
            BitVector destVector = toBitVector();
            int sourceColumns = source.Columns();
            for (; --height >= 0; row++, sourceRow++)
            {
                int offset = row*m_intColumns + column;
                int sourceOffset = sourceRow*sourceColumns + sourceColumn;
                destVector.replaceFromToWith(offset, offset + width - 1, sourceVector, sourceOffset);
            }
        }

        /**
         * Sets the bits in the given box to the state specified by <tt>value</tt>.
         * The box ranges from <tt>[column,row]</tt> to <tt>[column+width-1,row+height-1]</tt>, all inclusive.
         * (Does nothing if <tt>width &lt;= 0 || height &lt;= 0</tt>).
         * @param     column   the index of the column-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     width   the width of the box.
         * @param     height   the height of the box.
         * @param    value the value of the bit to be copied into the bits of the specified box.
         * @throws	HCException if <tt>column&lt;0 || column+width&gt;Columns() || row&lt;0 || row+height&gt;Rows()</tt>
         */

        public void replaceBoxWith(int column, int row, int width, int height, bool value)
        {
            containsBox(column, row, width, height);
            if (width <= 0 || height <= 0)
            {
                return;
            }

            BitVector destVector = toBitVector();
            for (; --height >= 0; row++)
            {
                int offset = row*m_intColumns + column;
                destVector.replaceFromToWith(offset, offset + width - 1, value);
            }
        }

        /**
         * Returns the number of rows of the receiver.
         */

        public int Rows()
        {
            return m_intRows;
        }

        /**
         * Returns the size of the receiver which is <tt>Columns()*Rows()</tt>.
         */

        public int Size()
        {
            return m_intColumns*m_intRows;
        }

        /**
         * Converts the receiver to a bitvector. 
         * In many cases this method only makes sense on one-dimensional matrices.
         * <b>WARNING:</b> The returned bitvector and the receiver share the <b>same</b> backing bits.
         * Modifying either of them will affect the other.
         * If this behaviour is not what you want, you should first use <tt>Copy()</tt> to make sure both objects use separate internal storage.
         */

        public BitVector toBitVector()
        {
            return new BitVector(m_bits, Size());
        }

        /**
         * Returns a (very crude) string representation of the receiver.
         */

        public override string ToString()
        {
            return toBitVector().ToString();
        }

        /**
         * Performs a logical <b>XOR</b> of the receiver with another bit matrix.
         * The receiver is modified so that a bit in it has the
         * value <code>true</code> if and only if one of the following statements holds:
         * <ul>
         * <li>The bit initially has the value <code>true</code>, and the 
         *     corresponding bit in the argument has the value <code>false</code>.
         * <li>The bit initially has the value <code>false</code>, and the 
         *     corresponding bit in the argument has the value <code>true</code>. 
         * </ul>
         *
         * @param   other   a bit matrix.
         * @throws ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>.
         */

        public void xor(BitMatrix other)
        {
            checkDimensionCompatibility(other);
            toBitVector().xor(other.toBitVector());
        }
    }

    class Rectangle
    {
        private int column;
        private int row;
        private int width;
        private int height;

        public Rectangle(int column, int row, int width, int height)
        {
            this.column = column;
            this.row = row;
            this.width = width;
            this.height = height;
        }

        internal bool IntersectsWith(Rectangle sourceRect)
        {
            throw new NotImplementedException();
        }
    }
}