using System;
using System.Collections.Generic;

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    public static class GpHelper
    {
        public static List<GpConstants> LoadConstants()
        {
            var constants = new List<GpConstants>
                                {
                                    new GpConstants(1.0, "1"),
                                    new GpConstants(2.0, "2"),
                                    new GpConstants(5.0, "5"),
                                    new GpConstants(10.0, "10"),
                                    new GpConstants(Math.PI, "Pi"),
                                    new GpConstants(Math.E, "E")
                                };
            const double gRatio = 1.618033988749894848204586834365638117720309179805;
            constants.Add(new GpConstants(gRatio, "gRatio"));
            return constants;
        }
    }
}
