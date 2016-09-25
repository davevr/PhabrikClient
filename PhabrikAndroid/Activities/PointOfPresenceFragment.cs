
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
		public string PopTitle { get; set;} = "No POP";
		public PointOfPresenceObj pop { get; set; } = null;

        public MainActivity mainWindow { get; set; }
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

            }

            return theView;
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

		public virtual void Update()
		{

		}

        public virtual void InitializeForPop(PointOfPresenceObj popObj)
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

        void InitializeSolSys(SolSysObj_callback callback)
        {
            if (pop.structureId == 0)
            {
                InitializeNewPop((theSys) =>
                {
                    pop.curSolSys = theSys;
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

        void InitializeNewPop(SolSysObj_callback callback)
        {
            // for now, just use an empty system
            PhabrikServer.FetchSolSys(0, 0, 0, callback);
        }

        public void GotoPlanet(long planetId)
        {

        }

    }
}
