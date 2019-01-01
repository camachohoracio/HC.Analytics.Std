namespace HC.Analytics.Colt
{
    /**
     * Interface that represents a function object: a function that takes 
     * two argument vectors and returns a single value.
     */

    public interface VectorVectorFunction
    {
        /**
         * Applies a function to two argument vectors.
         *
         * @param x   the first argument vector passed to the function.
         * @param y   the second argument vector passed to the function.
         * @return the result of the function.
         */
        double Apply(DoubleMatrix1D x, DoubleMatrix1D y);
    }
}
