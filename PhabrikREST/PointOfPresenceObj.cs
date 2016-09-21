
/**
 * Created by davidvronay on 8/23/16.
 */

using System;

namespace Phabrik.Core
{
    public class PointOfPresenceObj
    {
        public long Id { get; set; }
        public long playerId { get; set; }   // player who owns this
        public long structureId { get; set; }
        public DateTime created { get; set; }
        public DateTime lastactive { get; set; }
        public StructureObj structure { get; set; }
		public string nickname { get; set; }
    }
}