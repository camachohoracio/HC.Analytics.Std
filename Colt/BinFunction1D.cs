namespace HC.Analytics.Colt
{
    ////package bin;

    /**
     * Interface that represents a function object: a function that takes 
     * two bins as arguments and returns a single value.
     */

    public interface BinFunction1D
    {
        /**
         * Applies a function to one bin argument.
         *
         * @param x   the argument passed to the function.
         * @return the result of the function.
         */
        double Apply(DynamicBin1D x);
        /**
         * Returns the name of this function.
         *
         * @return the name of this function.
         */
        string name();
    }
}
