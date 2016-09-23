
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

        PopSubFragment curFragment;
        

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			 var theView = inflater.Inflate(Resource.Layout.PointOfPresenceLayout, container, false);


            if (pop == null)
            {
                InstallSolarSystem(0, 0, 0);
            }

            return theView;
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

        public void InstallSolarSystem(int x, int y, int z)
        {
            var newFragment = new SolSysPopFragment();
            newFragment.parent = this;
            if (curFragment == null)
                FragmentManager.BeginTransaction().Add(Resource.Id.fragment, newFragment).Commit();
            else
                FragmentManager.BeginTransaction().Replace(Resource.Id.fragment, newFragment).AddToBackStack(null).Commit();
            newFragment.InitializeForXYZ(x, y, z);
            curFragment = newFragment;
        }

        public void InstallPlanet(long planetId)
        {
            var newFragment = new PlanetPopFragment();
            newFragment.parent = this;
            if (curFragment == null)
                FragmentManager.BeginTransaction().Add(Resource.Id.fragment, newFragment).Commit();
            else
                FragmentManager.BeginTransaction().Replace(Resource.Id.fragment, newFragment).AddToBackStack(null).Commit();
            newFragment.InitializeForPlanetId(planetId);
            curFragment = newFragment;
        }


    }
}
