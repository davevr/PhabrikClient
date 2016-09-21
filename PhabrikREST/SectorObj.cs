
/**
 * Created by davidvronay on 9/5/16.
 */
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class SectorObj {

        public enum SurfaceType {
            Rock,
            Ice,
            Gas,
            Dirt,
            Water,
            Unknown
        }

        public long Id { get; set; }
        public double lowTemp { get; set; }
        public double highTemp { get; set; }
        public SurfaceType surfaceType { get; set; }
        public List<StructureObj> structures { get; set; }
        public long ownerId { get; set; }
        public bool claimed { get; set; }
        public int xLoc { get; set; }
        public int yLoc { get; set; }
        public long terrainId { get; set; }

        public SectorObj() {
            structures = new List<StructureObj>();
            ownerId = 0;
            claimed = false;
            surfaceType = SurfaceType.Unknown;

        }

        public SectorObj(SectorObj master) {
            lowTemp = master.lowTemp;
            highTemp = master.highTemp;
            surfaceType = master.surfaceType;
            ownerId = master.ownerId;
            claimed = master.claimed;

            foreach (StructureObj curStruct in master.structures) {
                StructureObj newStruct = new StructureObj(curStruct);
                newStruct.sector = this;
                structures.Add(newStruct);
            }

        }

    }
}
