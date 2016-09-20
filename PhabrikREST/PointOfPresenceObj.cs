
/**
 * Created by davidvronay on 8/23/16.
 */

using System;

namespace Phabrik.Core
{
    public class PointOfPresenceObj
    {
        public long Id;
        public long playerId;   // player who owns this
        public long structureId;
        public DateTime created;
        public DateTime lastactive;
        public StructureObj structure;
    }
}