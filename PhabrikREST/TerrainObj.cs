

/**
 * Created by davidvronay on 9/5/16.
 */

namespace Phabrik.Core
{
    public class TerrainObj {
        public long Id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public long planetId { get; set; }
        

        public SectorObj[][] _sectorArray { get; set; }


        public TerrainObj() {
        }
    }

    
}