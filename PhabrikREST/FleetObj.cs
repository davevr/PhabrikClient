
/**
 * Created by davidvronay on 8/23/16.
 */
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class FleetObj {
        public long Id { get; set; }
        public string fleetname { get; set; }

        public List<ShipRecordObj> shiplist { get; set; }
        public long planetid { get; set; }
    }
}
