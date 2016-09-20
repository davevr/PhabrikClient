
/**
 * Created by davidvronay on 8/23/16.
 */

using System;
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class SolSysObj {
        public long Id;
        public int xLoc;
        public int yLoc;
        public int zLoc;
        public long discovererId;
        public string systemName;
        public bool underProtection;

        public List<SunObj> suns;

    }
}