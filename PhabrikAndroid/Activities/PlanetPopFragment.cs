
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
	public class PlanetPopFragment : PopSubFragment, View.IOnTouchListener
    {
        TerrainObj curPlanet;
        TextView titleView;
        GridLayout gridView;
        HorizontalScrollView hScroller;
        ScrollView vScroller;

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
            hScroller = theView.FindViewById<HorizontalScrollView>(Resource.Id.hscroller);
            vScroller = theView.FindViewById<ScrollView>(Resource.Id.vscroller);

            hScroller.SetOnTouchListener(this);
            vScroller.SetOnTouchListener(this);
            return theView;
		}
        private float mx, my, curX, curY;
        private bool started = false;

        public bool OnTouch(View v, MotionEvent evt) {
            if (v == hScroller)
                return false;
            else 
            {
                curX = evt.GetX();
                curY = evt.GetY();
                int dx = (int)(mx - curX);
                int dy = (int)(my - curY);
                switch (evt.Action) {
                    case MotionEventActions.Move:
                        if (started)
                        {
                            vScroller.ScrollBy(0, dy);
                            hScroller.ScrollBy(dx, 0);
                        }
                        else
                        {
                            started = true;
                        }
                        mx = curX;
                        my = curY;
                        break;
                    case MotionEventActions.Up:
                        vScroller.ScrollBy(0, dy);
                        hScroller.ScrollBy(dx, 0);
                        started = false;
                        break;
                }
                return true;
            }
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
                            View newView = Activity.LayoutInflater.Inflate(Resource.Layout.GridSectorLayout, null);
                            gridView.AddView(newView);
                            var gridLayout = new GridLayout.LayoutParams(GridLayout.InvokeSpec(y), GridLayout.InvokeSpec(x));
                            gridLayout.SetMargins(2, 2, 2, 2);
                            gridLayout.Width = 200;
                            gridLayout.Height = 200;
                            newView.LayoutParameters = gridLayout;
                            var textField = newView.FindViewById<TextView>(Resource.Id.coordLabel);
                            textField.Text = string.Format("{0},{1}", x, y);
                            textField.SetBackgroundColor(Color.Blue);
                            textField.SetTextColor(Color.White);


                            var imageView = newView.FindViewById<ImageView>(Resource.Id.backgroundImage);
                            SectorObj curSec = curPlanet._sectorArray[x][y];
                            Koush.UrlImageViewHelper.SetUrlDrawable(imageView, curSec.TextureURL, Resource.Drawable.Icon);
                        }
                    }
                });
            }
        }
    }
}
