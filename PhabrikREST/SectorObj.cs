
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

        public long Id;
        public double lowTemp;
        public double highTemp;
        public SurfaceType surfaceType;
        public List<StructureObj> structures;
        public long ownerId;
        public bool claimed;
        public int xLoc;
        public int yLoc;
        public long terrainId;

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
