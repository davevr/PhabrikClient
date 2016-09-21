

/**
 * Created by davidvronay on 8/23/16.
 */

using System;
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class SunObj {
        public long Id { get; set; }
        public long solarSystemId { get; set; }
        public List<PlanetObj> planets { get; set; }
        public long discovererId { get; set; }
        public String name { get; set; }

        // simulation variables
        public double luminosity { get; set; }
        public double mass { get; set; }
        public double life { get; set; }
        public double age { get; set; }
        public double r_ecosphere { get; set; }
        public double m2 { get; set; }
        public double e { get; set; }
        public double a { get; set; }

        public int earthlike { get; set; } = 0;
        public int habitable { get; set; } = 0;
        public int habitable_jovians { get; set; } = 0;

    }
}