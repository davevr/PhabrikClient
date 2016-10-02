
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Phabrik.Core;

namespace Phabrik.AndroidApp
{
    public class PointOfPresenceFragment : Android.Support.V4.App.Fragment
    {
        public string PopTitle { get; set; } = "No POP";
        public PointOfPresenceObj pop { get; set; } = null;


        public GameFragment gameFragment { get; set; }

        bool firstFrag = true;

        TextView galaxyBtn;
        TextView solSysBtn;
        TextView planetBtn;
        TextView sectorBtn;
        TextView structureBtn;

        GalaxyPopFragment galaxyFragment;
        SolSysPopFragment solSysFragment;
        PlanetPopFragment planetFragment;
        SectorPopFragment sectorFragment;
        StructurePopFragment structureFragment;




        PointOfPresenceObj.PopScale curScale = PointOfPresenceObj.PopScale.None;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var theView = inflater.Inflate(Resource.Layout.PointOfPresenceLayout, container, false);

            galaxyBtn = theView.FindViewById<TextView>(Resource.Id.GalaxyBtn);
            solSysBtn = theView.FindViewById<TextView>(Resource.Id.SolSysBtn);
            planetBtn = theView.FindViewById<TextView>(Resource.Id.PlanetBtn);
            sectorBtn = theView.FindViewById<TextView>(Resource.Id.SectorBtn);
            structureBtn = theView.FindViewById<TextView>(Resource.Id.StructureBtn);

            galaxyBtn.Click += GalaxyBtn_Click;
            solSysBtn.Click += SolSysBtn_Click;
            planetBtn.Click += PlanetBtn_Click;
            sectorBtn.Click += SectorBtn_Click;
            structureBtn.Click += StructureBtn_Click;

            if (pop == null)
            {
                // no pop yet - better create one
                pop = new PointOfPresenceObj();
                pop.created = DateTime.Now;
                pop.playerId = PhabrikServer.CurrentUser.Id;
                pop.nickname = "primary";
                pop.scale = PointOfPresenceObj.PopScale.System;
                pop.structureId = 0;
                SetScale(PointOfPresenceObj.PopScale.System);
            }
            curScale = PointOfPresenceObj.PopScale.None;
            RefreshForPop();

            return theView;
        }

        public void RefreshForPop()
        {
            if (pop != null)
            {
                SetScale(pop.scale);
                if (pop.structureId == 0)
                {
                    if (pop.fleetId == 0)
                    {
                        // no structure - some btns are invalid
                        if (pop.curSolSys != null)
                            EnableBtn(solSysBtn);
                        else
                            DisableBtn(solSysBtn);

                        if (pop.curPlanet != null)
                            EnableBtn(planetBtn);
                        else
                            DisableBtn(planetBtn);

                        if (pop.curSector != null)
                            EnableBtn(sectorBtn);
                        else
                            DisableBtn(sectorBtn);

                        if (pop.curStructure != null)
                            EnableBtn(structureBtn);
                        else
                            DisableBtn(structureBtn);

                    } else
                    {
                        // pop is with a fleet - we have a planet at least
                        EnableBtn(solSysBtn);
                        EnableBtn(planetBtn);
                        if (pop.curSector != null)
                            EnableBtn(sectorBtn);
                        else
                            DisableBtn(sectorBtn);

                        if (pop.curStructure != null)
                            EnableBtn(structureBtn);
                        else
                            DisableBtn(structureBtn);
                    }
                } else
                {
                    // if there is a structure, we can always go up!
                    EnableBtn(solSysBtn);
                    EnableBtn(planetBtn);
                    EnableBtn(sectorBtn);
                    EnableBtn(structureBtn);
                }

            }
        }

        private void StructureBtn_Click(object sender, EventArgs e)
        {
            SetScale(PointOfPresenceObj.PopScale.Structure);
        }

        private void SectorBtn_Click(object sender, EventArgs e)
        {
            SetScale(PointOfPresenceObj.PopScale.Sector);
        }

        private void PlanetBtn_Click(object sender, EventArgs e)
        {
            SetScale(PointOfPresenceObj.PopScale.Planet);
        }

        private void SolSysBtn_Click(object sender, EventArgs e)
        {
            SetScale(PointOfPresenceObj.PopScale.System);
        }

        private void GalaxyBtn_Click(object sender, EventArgs e)
        {
            SetScale(PointOfPresenceObj.PopScale.Galaxy);
        }

        public void InitPop(PointOfPresenceObj newPop)
        {
            pop = newPop;
            PopTitle = newPop.nickname;
            if (string.IsNullOrEmpty(PopTitle))
                PopTitle = "New Point of Presence";
        }


        public virtual void InitializeForNewPop(PointOfPresenceObj popObj)
        {

        }


        public void SetScale(PointOfPresenceObj.PopScale newScale)
        {
            if (newScale != curScale)
            {
                ClearScaleBtn();

                PopSubFragment newFragment = null;
                switch (newScale)
                {
                    case PointOfPresenceObj.PopScale.Galaxy:
                        if (galaxyFragment == null)
                            galaxyFragment = new GalaxyPopFragment();
                        newFragment = galaxyFragment;
                        break;
                    case PointOfPresenceObj.PopScale.System:
                        if (solSysFragment == null)
                            solSysFragment = new SolSysPopFragment();
                        newFragment = solSysFragment;
                        break;
                    case PointOfPresenceObj.PopScale.Planet:
                        if (planetFragment == null)
                            planetFragment = new PlanetPopFragment();
                        newFragment = planetFragment;
                        break;
                    case PointOfPresenceObj.PopScale.Sector:
                        if (sectorFragment == null)
                            sectorFragment = new SectorPopFragment();
                        newFragment = sectorFragment;
                        break;
                    case PointOfPresenceObj.PopScale.Structure:
                        if (structureFragment == null)
                            structureFragment = new StructurePopFragment();
                        newFragment = structureFragment;
                        break;

                }
                if (newFragment != null) {
                    curScale = newScale;
                    SetScaleBtn();
                    newFragment.parent = this;
                    if (firstFrag)
                        FragmentManager.BeginTransaction().Add(Resource.Id.fragment, newFragment).Commit();
                    else
                        FragmentManager.BeginTransaction().Replace(Resource.Id.fragment, newFragment).Commit();
                    newFragment.Initialize();
                    firstFrag = false;
                }
            }
        }


        private void ClearScaleBtn()
        {
            TextView oldView = null;
            switch (curScale)
            {
                case PointOfPresenceObj.PopScale.Galaxy:
                    oldView = galaxyBtn;
                    break;
                case PointOfPresenceObj.PopScale.System:
                    oldView = solSysBtn;
                    break;
                case PointOfPresenceObj.PopScale.Planet:
                    oldView = planetBtn;
                    break;
                case PointOfPresenceObj.PopScale.Sector:
                    oldView = sectorBtn;
                    break;
                case PointOfPresenceObj.PopScale.Structure:
                    oldView = structureBtn;
                    break;

            }

            if (oldView != null)
            {
                oldView.SetBackgroundColor(Resources.GetColor(Resource.Color.Phabrik_darkgray));
                oldView.SetTextColor(Resources.GetColor(Resource.Color.Phabrik_white));
            }
        }

        private void SetScaleBtn()
        {
            TextView oldView = null;
            switch (curScale)
            {
                case PointOfPresenceObj.PopScale.Galaxy:
                    oldView = galaxyBtn;
                    break;
                case PointOfPresenceObj.PopScale.System:
                    oldView = solSysBtn;
                    break;
                case PointOfPresenceObj.PopScale.Planet:
                    oldView = planetBtn;
                    break;
                case PointOfPresenceObj.PopScale.Sector:
                    oldView = sectorBtn;
                    break;
                case PointOfPresenceObj.PopScale.Structure:
                    oldView = structureBtn;
                    break;

            }

            if (oldView != null)
            {
                oldView.SetBackgroundColor(Resources.GetColor(Resource.Color.Phabrik_green));
                oldView.SetTextColor(Resources.GetColor(Resource.Color.Phabrik_black));
            }
        }

        public override void OnDestroyView()
        {
            // drop the nested fragments to save RAM
            galaxyFragment = null;
            solSysFragment = null;
            planetFragment = null;
            sectorFragment = null;
            structureFragment = null;

            base.OnDestroyView();
        }


        public void InitializeSolSys(SolSysObj_callback callback)
        {
            if (pop.structureId == 0)
            {
                InitializeNewPop((theSys) =>
                {
                    pop.curSolSys = theSys;
                    EnableBtn(solSysBtn);
                    callback(theSys);
                });
            } else
            {
                // laoad it from the structure
                /*
                PhabrikServer.FetchSolSysFromStructure(pop.structureId, (theSys) =>
                {
                    pop.curSolSys = theSys;
                    callback(theSys);
                });
                */
            }
        }



        public void InitializeNewPop(SolSysObj_callback callback)
        {
            // for now, just use an empty system
            PhabrikServer.FetchSolSys(0, 0, 0, callback);
        }

        public void GotoSolSys(long solSysId)
        {
            PhabrikServer.FetchSolSysById(solSysId, (theSys) =>
            {
                if (theSys != null)
                {
                    pop.curSolSys = theSys;
                    EnableBtn(solSysBtn);
                    pop.scale = PointOfPresenceObj.PopScale.System;
                    Activity.RunOnUiThread(() =>
                    {
                        SetScale(PointOfPresenceObj.PopScale.System);
                    });
                } else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        MainActivity.ShowAlert(this.Context, "Missing Data", "You have no data on this system.  Try scanning it now.", "ok");
                    });
                }
            });
        }

        public void UpdateSectorUrl(SectorObj theSector, string theUrl)
        {
            theSector.sectorUrl = theUrl;
            PhabrikServer.UpdateSectorUrl(theSector, theUrl);
            if (curScale == PointOfPresenceObj.PopScale.Planet)
            {
                planetFragment.UpdateSectorURL(theSector);
            }
        }

        public void GotoPlanet(long planetId)
        {
            // go to a planet in the current system
            if (pop.curSolSys != null)
            {
                var newPlanet = pop.curSolSys.suns[0].planets.Find(p => p.Id == planetId);
                if (newPlanet == null)
                {
                    TravelToDistantPlanet(planetId);
                } else
                {
                    // we are the system, we can travel normally
                   
                    PhabrikServer.FetchTerrain(planetId, (theResult) =>
                    {
                        if (theResult != null)
                        {
                            pop.curTerrain = theResult;
                            pop.curPlanet = newPlanet;
                            pop.curStructure = null;
                            pop.curSector = null;
                            
                            pop.scale = PointOfPresenceObj.PopScale.Planet;
                            Activity.RunOnUiThread(() =>
                            {
                                EnableBtn(planetBtn);
                                SetScale(PointOfPresenceObj.PopScale.Planet);
                            });
                        } else
                        {
                            // null result
                            Activity.RunOnUiThread(() =>
                            {
                                MainActivity.ShowAlert(this.Context, "Missing Data", "You have no data on this planet.  Try scanning it now.", "ok");
                            });

                        }
                    });

                }
            } else
            {
                TravelToDistantPlanet(planetId);
            }
        }

        public void GotoSector(long sectorId)
        {
            if (pop.curTerrain != null)
            {
                PhabrikServer.FetchSector(sectorId, (theResult) =>
                {
                    pop.curSector = theResult;
                    Activity.RunOnUiThread(() =>
                    {
                        EnableBtn(sectorBtn);
                        SetScale(PointOfPresenceObj.PopScale.Sector);
                    });
                });
            } else
            {
                TravelToDistantSector(sectorId);
            }
        }
        public void TravelToDistantSector(long sectorId)
        {
            // todo - figure out how to get there
            
        }

        public void GotoStructure(long structureId)
        {
            if (pop.curSector != null)
            {
                var existingStructure = pop.curSector.structures.Find(s => s.Id == structureId);

                if (existingStructure != null)
                {
                    pop.curStructure = existingStructure;
                    EnableBtn(structureBtn);
                    Activity.RunOnUiThread(() =>
                    {
                        SetScale(PointOfPresenceObj.PopScale.Structure);
                    });
                } else
                    TravelToDistantStructure(structureId);
                
            }
            else
            {
                TravelToDistantStructure(structureId);
            }
        }



        public void TravelToDistantStructure(long structureId)
        {
            // todo - figure out how to get there
            PhabrikServer.FetchStructure(structureId, (theResult) =>
            {
                pop.curStructure = theResult;
                EnableBtn(structureBtn);
                Activity.RunOnUiThread(() =>
                {
                    SetScale(PointOfPresenceObj.PopScale.Structure);
                });
            });
        }

        public void EnableBtn(TextView theBtn)
        {
            theBtn.Enabled = true;
            theBtn.Alpha = 1;
        }

        public void DisableBtn(TextView theBtn)
        {
            theBtn.Enabled = false;
            theBtn.Alpha = .5f;
        }

        public void TravelToDistantPlanet(long planetId)
        {
            
        }

        public void ScanPlanet(long planetId)
        {
            PhabrikServer.ProbePlanet(planetId, (theTerrain) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    string msgStr;
                    if (theTerrain != null)
                    {
                        msgStr = "Probe successful.  You can view the planet now.";
                    }
                    else
                        msgStr = "Probe destroyed";

                    MainActivity.ShowAlert(Context, "Probe Results", msgStr, "Ok");
                });
            });
        }

    }
}
