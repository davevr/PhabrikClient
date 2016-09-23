
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
using Android.Graphics;

namespace Phabrik.AndroidApp
{
	public class SolSysPopFragment : PopSubFragment
    {
		private SolSysObj curSystem;
        private TextView titleText;
        private TextView coordText;
        private TextView planetCountText;
        private ListView planetList;
        private PlanetListAdapter adapter;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			View theView = inflater.Inflate(Resource.Layout.SolSysPopLayout, container, false);

            titleText = theView.FindViewById<TextView>(Resource.Id.SolSysTitle);
            coordText = theView.FindViewById<TextView>(Resource.Id.solSysCoord);
            planetCountText = theView.FindViewById<TextView>(Resource.Id.planetCount);
            planetList = theView.FindViewById<ListView>(Resource.Id.planetList);

            titleText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
            coordText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
            planetCountText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);

            

            return theView;
		}

		public void InitializeForXYZ(int xLoc, int yLoc, int zLoc)
		{
			PhabrikServer.FetchSolSys(xLoc, yLoc, zLoc, (theSys) =>
			{
				if (theSys != null)
				{
					curSystem = theSys;
                    this.Activity.RunOnUiThread(() =>
                    {
                        UpdateForNewSystem();
                    });
				}
			});
		}

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        }

        public override void OnResume()
        {
            base.OnResume();
        }
        private void UpdateForNewSystem()
        {
            titleText.Text = curSystem.systemName;
            coordText.Text = string.Format("coord: X:{0}, Y:{1}, Z:{2}", curSystem.xLoc, curSystem.yLoc, curSystem.zLoc);
            planetCountText.Text = string.Format("{0} planets", curSystem.suns[0].planets.Count);
            adapter = new PlanetListAdapter(this, curSystem.suns[0].planets);
            planetList.Adapter = adapter;

            RefreshListView();

        }

        private void RefreshListView()
        {
            if (this.View != null)
            {
                Activity.RunOnUiThread(() => {
                    adapter.NotifyDataSetChanged();
                    planetList.InvalidateViews();
                });
            }
        }
    }

    public class PlanetListAdapter : BaseAdapter<PlanetObj>
    {
        public List<PlanetObj> allItems;
        SolSysPopFragment fragment;
        static double EARTH_RADIUS_SQRT = Math.Sqrt(6378.0);
        public PlanetListAdapter(SolSysPopFragment context, List<PlanetObj> theItems) : base()
        {
            this.fragment = context;
            this.allItems = theItems;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override PlanetObj this[int position]
        {
            get { return allItems[position]; }
        }
        public override int Count
        {
            get { return allItems.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            {
                view = fragment.Activity.LayoutInflater.Inflate(Resource.Layout.PlanetListItemLayout, null);
            }
            var imageView = view.FindViewById<ImageView>(Resource.Id.planetImage);
            var titleView = view.FindViewById<TextView>(Resource.Id.planetName);
            var infoView = view.FindViewById<TextView>(Resource.Id.planetInfo);
            var distanceLabel = view.FindViewById<TextView>(Resource.Id.distanceLabel);
            var massLabel = view.FindViewById<TextView>(Resource.Id.massLabel);
            var tempLabel = view.FindViewById<TextView>(Resource.Id.tempLabel);
            var dayLabel = view.FindViewById<TextView>(Resource.Id.dayLabel);
            var yearLabel = view.FindViewById<TextView>(Resource.Id.yearLabel);
            var scanBtn = view.FindViewById<Button>(Resource.Id.scanPlanetBtn);
            var gotoBtn = view.FindViewById<Button>(Resource.Id.gotoPlanetBtn);

            if (convertView == null)
            {
                // first time init
                titleView.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
                infoView.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                // all the other views... sigh
                view.FindViewById<TextView>(Resource.Id.distanceStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.massStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.tempStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.dayStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.yearStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);

                distanceLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                massLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                tempLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                dayLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                yearLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);



                scanBtn.Click += ScanBtn_Click;
                gotoBtn.Click += GotoBtn_Click;

            }

            PlanetObj curItem = allItems[position];
            Koush.UrlImageViewHelper.SetUrlDrawable(imageView, curItem.ImageUrl, Resource.Drawable.Icon);
            titleView.Text = curItem.planetName;
            infoView.Text = curItem.Description;

            var metrics = MainActivity.instance.Resources.DisplayMetrics;
            var baseSize = metrics.Density * 64;
            double ratio = Math.Sqrt(curItem.radius) / EARTH_RADIUS_SQRT;;

            imageView.LayoutParameters.Width = (int)(baseSize * ratio);
            imageView.LayoutParameters.Height = (int)(baseSize * ratio);

            distanceLabel.Text = string.Format("{0:f2} AUs", curItem.a);

            massLabel.Text = string.Format("{0:G4}x earth ({1:G4}kg)", curItem.massInEarthMass, curItem.massInKG);

            if (curItem.planetType == PlanetObj.planet_type.tGasGiant ||
                curItem.planetType == PlanetObj.planet_type.tSubGasGiant ||
                curItem.planetType == PlanetObj.planet_type.tSubSubGasGiant)
                tempLabel.Text = "no surface";
            else
                tempLabel.Text = string.Format("{0:f2}C ({1:f2}F)", curItem.surf_temp - 273.15, curItem.surf_temp * (9.0 / 5.0) - 459.67);
            yearLabel.Text = string.Format("{0:f2} days", curItem.orb_period);
            if ((int)curItem.day == (int)(curItem.orb_period * 24.0)
                                 || (curItem.resonant_period))
                dayLabel.Text = string.Format("tidally locked", curItem.day);
            else
                dayLabel.Text = string.Format("{0:f2} hours", curItem.day);

            scanBtn.Tag = curItem.Id;
            gotoBtn.Tag = curItem.Id;
            return view;
        }

        private void GotoBtn_Click(object sender, EventArgs e)
        {
            Button gotoBtn = sender as Button;

            if (gotoBtn != null)
            {
                long planetId = (long)gotoBtn.Tag;
                fragment.parent.InstallPlanet(planetId);
                
            }
        }

        private void ScanBtn_Click(object sender, EventArgs e)
        {
            // todo scan the planet
        }
    }
}
