

/**
 * Created by davidvronay on 8/23/16.
 */
using System;
using System.Collections.Generic;

namespace Phabrik.Core
{

    public class PlayerObj
    {
		public long Id { get; set; }
        public List<PointOfPresenceObj> popList { get; set; }
        public bool isAdmin { get; set; }
        public string playerName { get; set; }
        public DateTime lastlogin { get; set; }

    }
}
