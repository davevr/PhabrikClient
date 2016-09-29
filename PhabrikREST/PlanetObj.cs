

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

        static double FREEZING_POINT_OF_WATER = 273.15;
        static double EARTH_AVERAGE_CELSIUS = 14;
        static double EARTH_SURF_PRES_IN_MILLIBARS = 1013.25;
        static double EARTH_SURF_PRES_IN_PSI = 14.696;
        static double EARTH_SURF_PRES_IN_MMHG = 760.0;
        static double PSI_TO_MILLIBARS = (EARTH_SURF_PRES_IN_MILLIBARS / EARTH_SURF_PRES_IN_PSI);
        static double MAX_HABITABLE_PRESSURE = (118 * PSI_TO_MILLIBARS);
        static double MMHG_TO_MILLIBARS = (EARTH_SURF_PRES_IN_MILLIBARS / EARTH_SURF_PRES_IN_MMHG);
        static double MIN_O2_IPP = (72.0 * MMHG_TO_MILLIBARS);
        static double SOLAR_MASS_IN_KG = 1.989E30;
        static double EARTH_MASS_IN_KG = 5.972E24;


        public string Description
        {
            get
            {
                string desc = "";

                if ((planetType == planet_type.tGasGiant)
     || (planetType == planet_type.tSubGasGiant)
     || (planetType == planet_type.tSubSubGasGiant))
                {
                    // Nothing, for now.
                    desc = "A Gas Giant";
                }
                else
                {
                    double rel_temp = (surf_temp - FREEZING_POINT_OF_WATER) -
                                        EARTH_AVERAGE_CELSIUS;
                    double seas = (hydrosphere * 100.0);
                    double clouds = (cloud_cover * 100.0);
                    double atmosphere = (surf_pressure /
                                        EARTH_SURF_PRES_IN_MILLIBARS);
                    double ice = (ice_cover * 100.0);
                    double gravity = surf_grav;

                    if (gravity < .8)
                        desc += "Low-G, ";   /* .8 gees */
                    else if (gravity > 1.2)
                        desc += "High-G, ";


                    if (rel_temp < -5.0) desc += "Cold "; /* 5 C below earth */
                    else if (rel_temp < -2.0) desc += "Cool";
                    else if (rel_temp > 7.5) desc += "Hot";
                    else if (rel_temp > 3.0) desc += "Warm";
                    else desc += "Moderate climate";

            

                    if (ice > 10.0)
                        desc += ", Icy ";        /* 10% surface is ice */

                    if (atmosphere < 0.001)
                        desc += ", Airless";
                    else
                    {
                        if (planetType != planet_type.tWater)
                        {
                            if (seas < 25.0) desc += ", Arid";   
                            else if (seas < 50.0) desc += ", Dry";
                            else if (seas > 80.0) desc += ", Wet";
                        }

                        if (clouds < 10.0)
                            desc += ", Cloudless";/* 10% cloud cover */
                        else if (clouds < 40.0)
                            desc += ", Few clouds";
                        else if (clouds > 80.0)
                            desc += ", Cloudy";
            

                         if (max_temp >= boil_point)
                            desc += "Boiling ocean";

                        if (surf_pressure < MIN_O2_IPP)
                            desc += ", Unbreathably thin atmosphere";
                        else if (atmosphere < 0.5)
                            desc += ", Thin atmosphere";
                        else if (atmosphere > MAX_HABITABLE_PRESSURE / EARTH_SURF_PRES_IN_MILLIBARS)
                            desc += ", Unbreathably thick atmosphere";
                        else if (atmosphere > 2.0)
                            desc += ", Thick atmosphere";
                        else if (planetType != planet_type.tTerrestrial)
                            desc += ", Normal atmosphere";
                    }

                    if (earthlike)
                        desc += ", Earth-like";
                    else if (habitable)
                        desc += ", habitable";

                    /*
                    if (planet->gases > 0)
                    {
                        int i;
                        int first = TRUE;
                        unsigned int temp;

                        fprintf(file, " (");

                        for (i = 0; i < planet->gases; i++)
                        {
                            int n;
                            int index = max_gas;

                            for (n = 0; n < max_gas; n++)
                            {
                                if (gases[n].num == planet->atmosphere[i].num)
                                    index = n;
                            }

                            if ((planet->atmosphere[i].surf_pressure / planet->surf_pressure)
                                > .01)
                            {
                                LPRINT(gases[index].html_symbol);
                            }
                        }

                        if ((temp = breathability(planet)) != NONE)
                            fprintf(file, " - %s)",
                                 breathability_phrase[temp]);
                    }
                    */

                    if ((int)day == (int)(orb_period * 24.0)
                     || (resonant_period))
                        desc += ", tidally locked";
                }

                return desc;
            }
        }

        public string ImageUrl
        {
            get
            {
                string newUrl = "unknown";

                switch (this.planetType)
                {
                    case PlanetObj.planet_type.tRock:
                        newUrl = "RockPlanet";
                        break;
                    case PlanetObj.planet_type.tVenusian:
                        newUrl = "VenusianPlanet";
                        break;
                    case PlanetObj.planet_type.tTerrestrial:
                        newUrl = "TerrestrialPlanet";
                        break;
                    case PlanetObj.planet_type.tGasGiant:
                        newUrl = "JovianPlanet";
                        break;
                    case PlanetObj.planet_type.tMartian:
                        newUrl = "MartianPlanet";
                        break;
                    case PlanetObj.planet_type.tWater:
                        newUrl = "WaterPlanet";
                        break;
                    case PlanetObj.planet_type.tIce:
                        newUrl = "IcePlanet";
                        break;
                    case PlanetObj.planet_type.tSubGasGiant:
                        newUrl = "Sub-JovianPlanet";
                        break;
                    case PlanetObj.planet_type.tSubSubGasGiant:
                        newUrl = "GasDwarfPlanet";
                        break;
                    case PlanetObj.planet_type.tAsteroids:
                        newUrl = "AsteroidsPlanet";
                        break;
                    case PlanetObj.planet_type.t1Face:
                        newUrl = "1FacePlanet";
                        break;
                    default:
                        newUrl = "UnknownPlanet";
                        break;
                }

                return PhabrikServer.BaseImageUrl + newUrl + ".png";
            }
        }

        public double massInKG
        {
            get
            {
                return mass * SOLAR_MASS_IN_KG;
            }
        }

        public double massInEarthMass
        {
            get
            {
                return massInKG / EARTH_MASS_IN_KG;
            }
        }
        

    }
}
