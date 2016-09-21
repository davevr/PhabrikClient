

/**
 * Created by davidvronay on 8/23/16.
 */
using System;
using System.Collections.Generic;

namespace Phabrik.Core
{
    public class PlanetObj {

        public enum planet_type {
            tUnknown,
            tRock,
            tVenusian,
            tTerrestrial,
            tGasGiant,
            tMartian,
            tWater,
            tIce,
            tSubGasGiant,
            tSubSubGasGiant,
            tAsteroids,
            t1Face
        }

        public enum breathabilty_type {
            NONE,
            BREATHABLE,
            UNBREATHABLE,
            POISONOUS
        }
        public long Id { get; set; }
        public long systemId { get; set; }
        public long discovererId { get; set; }
        public string planetName { get; set; }
        public bool owned { get; set; }
        public long ownerId { get; set; }

        // planet specific stats
        public SunObj sun { get; set; }
        public int planet_no { get; set; }
        public planet_type planetType { get; set; }
        public double a { get; set; }                  /* semi-major axis of solar orbit (in AU)*/
        public double e { get; set; }                /* eccentricity of solar orbit		 */
        public double axial_tilt { get; set; }           /* units of degrees					 */
        public double mass { get; set; }            /* mass (in solar masses)			 */
        public bool gas_giant { get; set; }           /* TRUE if the planet is a gas giant */
        public double dust_mass { get; set; }            /* mass, ignoring gas				 */
        public double gas_mass { get; set; }         /* mass, ignoring dust				 */
        public double moon_a { get; set; }               /* semi-major axis of lunar orbit (in AU)*/
        public double moon_e { get; set; }               /* eccentricity of lunar orbit		 */
        public double core_radius { get; set; }      /* radius of the rocky core (in km)	 */
        public double radius { get; set; }               /* equatorial radius (in km)		 */
        public int orbit_zone { get; set; }          /* the 'zone' of the planet			 */
        public double density { get; set; }          /* density (in g/cc)				 */
        public double orb_period { get; set; }           /* length of the local year (days)	 */
        public double day { get; set; }              /* length of the local day (hours)	 */
        public bool resonant_period { get; set; } /* TRUE if in resonant rotation		 */
        public double esc_velocity { get; set; }     /* units of cm/sec					 */
        public double surf_accel { get; set; }           /* units of cm/sec2					 */
        public double surf_grav { get; set; }            /* units of Earth gravities			 */
        public double rms_velocity { get; set; }     /* units of cm/sec					 */
        public double molec_weight { get; set; }     /* smallest molecular weight retained*/
        public double volatile_gas_inventory { get; set; }
        public double surf_pressure { get; set; }        /* units of millibars (mb)			 */
        public bool greenhouse_effect { get; set; }   /* runaway greenhouse effect?		 */
        public double boil_point { get; set; }           /* the boiling point of water (Kelvin)*/
        public double albedo { get; set; }              /* albedo of the planet				 */
        public double exospheric_temp { get; set; }  /* units of degrees Kelvin			 */
        public double estimated_temp { get; set; }     /* quick non-iterative estimate (K)  */
        public double estimated_terr_temp { get; set; }/* for terrestrial moons and the like*/
        public double surf_temp { get; set; }            /* surface temperature in Kelvin	 */
        public double greenhs_rise { get; set; }     /* Temperature rise due to greenhouse */
        public double high_temp { get; set; }            /* Day-time temperature              */
        public double low_temp { get; set; }         /* Night-time temperature			 */
        public double max_temp { get; set; }         /* Summer/Day						 */
        public double min_temp { get; set; }         /* Winter/Night						 */
        public double hydrosphere { get; set; }      /* fraction of surface covered		 */
        public double cloud_cover { get; set; }      /* fraction of surface covered		 */
        public double ice_cover { get; set; }            /* fraction of surface covered		 */
        public int gases { get; set; }              /* # of gasses in the atmosphere */
        public List<Gas> atmosphere { get; set; }      /* list of gasses in the atmosphere */
        public long planetTypeId { get; set; }               /* Type code						 */
        public bool earthlike { get; set; }
        public bool habitable { get; set; }
        public bool habitable_jovian { get; set; }
        public List<PlanetObj> moonList { get; set; }        /* list of moons, if any */
        public PlanetObj first_moon { get; set; }
        public PlanetObj next_planet { get; set; }

        public PlanetObj() {
            // init one with all zeros
            owned = false;
            greenhouse_effect = false;
            earthlike = false;


        }

        public PlanetObj(int theNum, double orbit, double ecc, double tilt, double themass, bool isGass, double dustMass, double gassMass) {
            planet_no = theNum;
            a = orbit;
            e = ecc;
            axial_tilt = tilt;
            mass = themass;
            gas_giant = isGass;
            dust_mass = dustMass;
            gas_mass = gassMass;


        }

    }
}
