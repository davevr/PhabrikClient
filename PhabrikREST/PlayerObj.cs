

/**
 * Created by davidvronay on 8/23/16.
 */
using System;
using System.Collections.Generic;

namespace Phabrik.Core
{

    public class PlayerObj
    {
        public long Id;
        public List<PointOfPresenceObj> popList;
        public bool isAdmin;
        public string playerName;
        public DateTime lastlogin;

    }
}
