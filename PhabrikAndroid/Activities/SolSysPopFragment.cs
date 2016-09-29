
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
using PullToRefresharp.Android;


namespace Phabrik.AndroidApp
{
	public class SolSysPopFragment : PopSubFragment
    {
        private TextView titleText;
        private TextView coordText;
        private TextView planetCountText;
        private PullToRefresharp.Android.Widget.ListView planetList;
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
            planetList = theView.FindViewById<PullToRefresharp.Android.Widget.ListView>(Resource.Id.planetList);

            titleText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
            coordText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
            planetCountText.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);

            planetList.RefreshActivated += (o, e) =>
            {
                RefreshFromData(true);
            };
            

            return theView;
		}


        private void RefreshFromData(bool fromPull = false)
        {
            // need to load it
            parent.InitializeSolSys((theSystem) =>
            {
                this.Activity.RunOnUiThread(() =>
                {
                    UpdateForNewSystem();
                    if (fromPull)
                        planetList.OnRefreshCompleted();
                });
            });
        }
		public void InitializeForXYZ(int xLoc, int yLoc, int zLoc)
		{
			
		}

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            this.Activity.RunOnUiThread(() =>
            {
                UpdateForNewSystem();
            });
        }

        public override void OnResume()
        {
            base.OnResume();
        }
        private void UpdateForNewSystem()
        {
            if (parent.pop.curSolSys != null) { 
                titleText.Text = parent.pop.curSolSys.systemName;
                coordText.Text = string.Format("coord: X:{0}, Y:{1}, Z:{2}", parent.pop.curSolSys.xLoc, parent.pop.curSolSys.yLoc, parent.pop.curSolSys.zLoc);
                planetCountText.Text = string.Format("{0} planets", parent.pop.curSolSys.suns[0].planets.Count);
                if (adapter == null || adapter.allItems != parent.pop.curSolSys.suns[0].planets)
                {
                    adapter = new PlanetListAdapter(this, parent.pop.curSolSys.suns[0].planets);
                }
                planetList.Adapter = adapter;
                RefreshListView();
            } else
            {
                RefreshFromData();
            }

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
        private bool[] expandMap;


        public PlanetListAdapter(SolSysPopFragment context, List<PlanetObj> theItems) : base()
        {
            this.fragment = context;
            this.allItems = theItems;
            expandMap = new bool[theItems.Count];
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
            var showMoreBtn = view.FindViewById<TextView>(Resource.Id.showMore);
            var layout = view.FindViewById<LinearLayout>(Resource.Id.MoreInfo);

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
                showMoreBtn.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Italic);


                scanBtn.Click += ScanBtn_Click;
                gotoBtn.Click += GotoBtn_Click;
                showMoreBtn.Click += ShowMoreBtn_Click;

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
            layout.Tag = position;

            if (expandMap[position])
            {
                layout.Visibility = ViewStates.Visible;
                showMoreBtn.Text = "hide details";
            } else
            {
                layout.Visibility = ViewStates.Gone;
                showMoreBtn.Text = "show details";
            }

            return view;
        }

        private void ShowMoreBtn_Click(object sender, EventArgs e)
        {
            TextView theText = sender as TextView;

            if (theText != null)
            {
                var parentView = theText.Parent as View;
                LinearLayout layout = parentView.FindViewById<LinearLayout>(Resource.Id.MoreInfo);

                if (layout.Visibility == ViewStates.Gone)
                {
                    layout.Visibility = ViewStates.Visible;
                    theText.Text = "hide details";
                    expandMap[(int)layout.Tag] = true;
                } else
                {
                    layout.Visibility = ViewStates.Gone;
                    theText.Text = "show details";
                    expandMap[(int)layout.Tag] = false;
                }
            }
   
        }

        private void GotoBtn_Click(object sender, EventArgs e)
        {
            Button gotoBtn = sender as Button;

            if (gotoBtn != null)
            {
                long planetId = (long)gotoBtn.Tag;
                fragment.parent.GotoPlanet(planetId);
                
            }
        }

        private void ScanBtn_Click(object sender, EventArgs e)
        {
            Button gotoBtn = sender as Button;

            if (gotoBtn != null)
            {
                long planetId = (long)gotoBtn.Tag;
                fragment.parent.ScanPlanet(planetId);

            }
        }
    }
}
