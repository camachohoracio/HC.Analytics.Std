using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

namespace HC.Analytics.Optimisation.GenericSolver
{
    public static class OptHelper
    {
        public static void GetIndividuals(
            Individual individual,
            out Individual intIndividual,
            out Individual blnIndividual,
            out Individual dblIndividual)
        {
            intIndividual = null;
            dblIndividual = null;
            blnIndividual = null;
            foreach (Individual currIndividual in individual.IndividualList)
            {
                if (currIndividual.ContainsChromosomeInt())
                {
                    intIndividual = currIndividual;
                }
                else if (currIndividual.ContainsChromosomeDbl())
                {
                    dblIndividual = currIndividual;
                }
                else if (currIndividual.ContainsChromosomeBln())
                {
                    blnIndividual = currIndividual;
                }
            }
        }

    }
}

