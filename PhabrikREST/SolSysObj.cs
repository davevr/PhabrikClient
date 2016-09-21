
/**
 * Created by davidvronay on 8/23/16.
 */

using System;
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class SolSysObj {
        public long Id { get; set; }
        public int xLoc { get; set; }
        public int yLoc { get; set; }
        public int zLoc { get; set; }
        public long discovererId { get; set; }
        public string systemName { get; set; }
        public bool underProtection { get; set; }

        public List<SunObj> suns { get; set; }

    }
}