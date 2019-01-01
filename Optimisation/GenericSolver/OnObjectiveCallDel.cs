using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

namespace HC.Analytics.Optimisation.GenericSolver
{
    public delegate void OnObjectiveCallDel(
        Individual individual,
        OptObjFunct optObjFunct,
        double dblLogTime,
        OptStatsCache optStatsCache);
}
