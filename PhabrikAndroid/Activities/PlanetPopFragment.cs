
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Phabrik.Core;

namespace Phabrik.AndroidApp
{
	[Activity(Label = "PlanetPopFragment")]
	public class PlanetPopFragment : PopSubFragment
    {
        TerrainObj curPlanet;
        TextView titleView;
        GridLayout gridView;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var theView = inflater.Inflate(Resource.Layout.PlanetPopLayout, container, false);
            titleView = theView.FindViewById<TextView>(Resource.Id.PlanetInfo);
            gridView = theView.FindViewById<GridLayout>(Resource.Id.Map);

            return theView;
		}

        public void InitializeForPlanetId(long planetId)
        {
            PhabrikServer.FetchTerrain(planetId, (thePlanet) =>
            {
                if (thePlanet != null)
                {
                    curPlanet = thePlanet;
                    this.Activity.RunOnUiThread(() =>
                    {
                        UpdateForNewPlanet();
                    });
                }
            });
        }

        private void UpdateForNewPlanet()
        {
            titleView.Text = string.Format("A cool planet that is {0} by {1}", curPlanet.width, curPlanet.height);
            /*
            titleText.Text = curSystem.systemName;
            coordText.Text = string.Format("coord: X:{0}, Y:{1}, Z:{2}", curSystem.xLoc, curSystem.yLoc, curSystem.zLoc);
            planetCountText.Text = string.Format("{0} planets", curSystem.suns[0].planets.Count);
            adapter = new PlanetListAdapter(this, curSystem.suns[0].planets);
            planetList.Adapter = adapter;
            */
            gridView.ColumnCount = curPlanet.width;
            gridView.RowCount = curPlanet.height;
            RefreshGridView();

        }

        private void RefreshGridView()
        {
            if (this.View != null)
            {
                Activity.RunOnUiThread(() => {
                    for (int x = 0; x < curPlanet.width; x++)
                    {
                        for (int y = 0; y < curPlanet.height; y++)
                        {
                            TextView newView = new TextView(this.Context);
                            gridView.AddView(newView);
                            var gridLayout = new GridLayout.LayoutParams(GridLayout.InvokeSpec(y), GridLayout.InvokeSpec(x));
                            gridLayout.SetMargins(2, 2, 2, 2);
                            gridLayout.Width = 200;
                            gridLayout.Height = 200;
                            newView.LayoutParameters = gridLayout;
                            newView.Text = string.Format("{0},{1}", x, y);
                            newView.SetBackgroundColor(Color.Blue);
                            newView.SetTextColor(Color.White);

                        }
                    }
                });
            }
        }
    }
}
