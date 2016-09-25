
/**
 * Created by davidvronay on 8/23/16.
 */

using System;

namespace Phabrik.Core
{
    public class PointOfPresenceObj
    {
        public enum PopScale
        {
            None,
            Structure,
            Sector,
            Planet,
            System,
            Galaxy
        }
        public long Id { get; set; }
        public long playerId { get; set; }   // player who owns this
        public long structureId { get; set; }
        public long fleetId { get; set; }
        public DateTime created { get; set; }
        public DateTime lastactive { get; set; }
        public StructureObj structure { get; set; }
		public string nickname { get; set; }
        public PopScale scale { get; set; }

        // these are run-time only
        public SolSysObj curSolSys { get; set; }
        public PlanetObj curPlanet { get; set; }
        public TerrainObj curTerrain { get; set; }
        public SectorObj curSector { get; set; }
        public StructureObj curStructure { get; set; }
    }
}