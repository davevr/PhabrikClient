
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
        TextView titleView;
        GridLayout gridView;
        HorizontalScrollView hScroller;
        ScrollView vScroller;
        Button scanBtn;
        Button gotoBtn;
        Button paintBtn;
        Button bombardBtn;
        Button colonizeBtn;
        Button landBtn;
        bool isDirty = false;
        
        private static int kGridSize = 300;
        private View selectionView;

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

            scanBtn = theView.FindViewById<Button>(Resource.Id.scanBtn);
            gotoBtn = theView.FindViewById<Button>(Resource.Id.gotoSectorBtn);
            paintBtn = theView.FindViewById<Button>(Resource.Id.paintBtn);
            bombardBtn = theView.FindViewById<Button>(Resource.Id.bombardBtn);
            colonizeBtn = theView.FindViewById<Button>(Resource.Id.colonizeBtn);
            landBtn = theView.FindViewById<Button>(Resource.Id.landBtn);

            hScroller.SetOnTouchListener(this);
            vScroller.SetOnTouchListener(this);

            gridView.Touch += GridView_Touch;

            scanBtn.Click += ScanBtn_Click;
            gotoBtn.Click += GotoBtn_Click;
            paintBtn.Click += PaintBtn_Click;
            bombardBtn.Click += BombardBtn_Click;
            colonizeBtn.Click += ColonizeBtn_Click;
            landBtn.Click += LandBtn_Click;

            return theView;
		}

        private void LandBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ColonizeBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BombardBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PaintBtn_Click(object sender, EventArgs e)
        {
            if (sectorSelected && parent.pop.curTerrain != null)
            {
                var curSector = parent.pop.curTerrain._sectorArray[selectedX][selectedY];
                var curPlanet = parent.pop.curPlanet;
                PlanetObj.planet_type planetType = PlanetObj.planet_type.tUnknown;
                if (curPlanet != null)
                    planetType = curPlanet.planetType;
                bool updated = true;

                switch (planetType)
                {
                    case PlanetObj.planet_type.tRock:
                    case PlanetObj.planet_type.tGasGiant:
                    case PlanetObj.planet_type.tSubGasGiant:
                    case PlanetObj.planet_type.tSubSubGasGiant:
                    case PlanetObj.planet_type.tAsteroids:
                    case PlanetObj.planet_type.tVenusian:
                        updated = false;
                        break;
                    case PlanetObj.planet_type.tTerrestrial:
                    case PlanetObj.planet_type.t1Face:
                        if (curSector.surfaceType == SectorObj.SurfaceType.Rock)
                            curSector.surfaceType = SectorObj.SurfaceType.Grass;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Grass)
                            curSector.surfaceType = SectorObj.SurfaceType.Dirt;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Dirt)
                            curSector.surfaceType = SectorObj.SurfaceType.Ice;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Ice)
                            curSector.surfaceType = SectorObj.SurfaceType.Water;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Water)
                            curSector.surfaceType = SectorObj.SurfaceType.Rock;
                        updated = true;
                        break;
                    case PlanetObj.planet_type.tMartian:
                        if (curSector.surfaceType == SectorObj.SurfaceType.Rock)
                            curSector.surfaceType = SectorObj.SurfaceType.Dirt;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Dirt)
                            curSector.surfaceType = SectorObj.SurfaceType.Ice;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Ice)
                            curSector.surfaceType = SectorObj.SurfaceType.Rock;
                        updated = true;
                        break;
                    case PlanetObj.planet_type.tWater:
                        if (curSector.surfaceType == SectorObj.SurfaceType.Rock)
                            curSector.surfaceType = SectorObj.SurfaceType.Grass;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Grass)
                            curSector.surfaceType = SectorObj.SurfaceType.Dirt;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Dirt)
                            curSector.surfaceType = SectorObj.SurfaceType.Water;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Water)
                            curSector.surfaceType = SectorObj.SurfaceType.Ice;
                        break;
                    case PlanetObj.planet_type.tIce:
                        if (curSector.surfaceType == SectorObj.SurfaceType.Rock)
                            curSector.surfaceType = SectorObj.SurfaceType.Ice;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Ice)
                            curSector.surfaceType = SectorObj.SurfaceType.Rock;
                        break;
                    case PlanetObj.planet_type.tUnknown:
                        if (curSector.surfaceType == SectorObj.SurfaceType.Rock)
                            curSector.surfaceType = SectorObj.SurfaceType.Grass;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Grass)
                            curSector.surfaceType = SectorObj.SurfaceType.Dirt;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Dirt)
                            curSector.surfaceType = SectorObj.SurfaceType.Ice;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Ice)
                            curSector.surfaceType = SectorObj.SurfaceType.Water;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Water)
                            curSector.surfaceType = SectorObj.SurfaceType.Gas;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Gas)
                            curSector.surfaceType = SectorObj.SurfaceType.Unknown;
                        else if (curSector.surfaceType == SectorObj.SurfaceType.Unknown)
                            curSector.surfaceType = SectorObj.SurfaceType.Rock;
                        updated = true;
                        break;

                }

                if (updated)
                {
                    curSector.dirty = true;
                    View sectorView = gridView.FindViewWithTag(curSector.Id);
                    if (sectorView != null)
                    {
                        var imageView = sectorView.FindViewById<ImageView>(Resource.Id.backgroundImage);
                        Koush.UrlImageViewHelper.SetUrlDrawable(imageView, curSector.DefaultUrl, Resource.Drawable.Icon);

                    }
                    isDirty = true;
                }
            }
        }

        private void GotoBtn_Click(object sender, EventArgs e)
        {
           if (sectorSelected && parent.pop.curTerrain != null)
            {
                parent.GotoSector(parent.pop.curTerrain._sectorArray[selectedX][selectedY].Id);
            }
        }

        private void ScanBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private int downX = 0, downY = 0;

        private void GridView_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                downX = (int)e.Event.GetX() / kGridSize;
                downY = (int)e.Event.GetY() / kGridSize;

            } else if (e.Event.Action == MotionEventActions.Up)
            {
                var newX = (int)e.Event.GetX() / kGridSize;
                var newY = (int)e.Event.GetY() / kGridSize;

                if (newX == downX && newY == downY)
                {
                    SelectGridSquare(newX, newY);
                }
            }
        }

        public override void OnPause()
        {
            if (isDirty)
            {
                PhabrikServer.SaveTerrainPaint(parent.pop.curTerrain, (didIt) =>
                {
                    isDirty = false;
                });
            }
            base.OnPause();
        }
        private int selectedX, selectedY;

        private void SelectGridSquare(int newX, int newY)
        {
            if (sectorSelected && selectedX == newX && selectedY == newY)
            {
                ClearSelection();
            }
            else
            {
                var gridLayout = new GridLayout.LayoutParams(GridLayout.InvokeSpec(newY), GridLayout.InvokeSpec(newX));
                gridLayout.Width = kGridSize;
                gridLayout.Height = kGridSize;
                selectionView.LayoutParameters = gridLayout;
                selectionView.Visibility = ViewStates.Visible;
                selectedX = newX;
                selectedY = newY;
            }
            UpdateBtns();
        }

        private void ClearSelection()
        {
            selectionView.Visibility = ViewStates.Gone;
            selectedX = selectedY = -1;
            UpdateBtns();
        }

        private void UpdateBtns()
        {
            if (sectorSelected)
            {
                gotoBtn.Enabled = true;
                paintBtn.Enabled = true;
                colonizeBtn.Enabled = true;
                landBtn.Enabled = true;
            } else
            {
                gotoBtn.Enabled = false;
                paintBtn.Enabled = false;
                colonizeBtn.Enabled = false;
                landBtn.Enabled = false;
            }
        }
        

        private bool sectorSelected
        {
            get
            {
                return selectionView.Visibility == ViewStates.Visible;
            }
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

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            this.Activity.RunOnUiThread(() =>
            {
                UpdateForNewPlanet();
            });
        }

        private void UpdateForNewPlanet()
        {
            titleView.Text = string.Format("A cool planet that is {0} by {1}", parent.pop.curTerrain.width, parent.pop.curTerrain.height);

            gridView.ColumnCount = parent.pop.curTerrain.width;
            gridView.RowCount = parent.pop.curTerrain.height;
            RefreshGridView();

        }

        public void UpdateSectorURL(SectorObj theSector)
        {
            if (theSector.terrainId == parent.pop.curTerrain.Id)
            {
                RefreshGridSquare(theSector.Id, theSector.sectorUrl);
            }
        }

        private void RefreshGridSquare(long sectorId, string theUrl)
        {
            Activity.RunOnUiThread(() =>
            {
                View newView = gridView.FindViewWithTag(sectorId);
                var imageView = newView.FindViewById<ImageView>(Resource.Id.backgroundImage);
                Koush.UrlImageViewHelper.SetUrlDrawable(imageView, theUrl + "=s" + kGridSize);
            });
        }
       
        private void RefreshGridView()
        {
            if (this.View != null)
            {
                Activity.RunOnUiThread(() => {
                    var curPlanet = parent.pop.curPlanet;
                    var curTerrain = parent.pop.curTerrain;

                    bool tidallyLocked = ((int)curPlanet.day == (int)(curPlanet.orb_period * 24.0)) || curPlanet.resonant_period;
                    var width = curTerrain.width;
                    var height = curTerrain.height;


                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            View newView = Activity.LayoutInflater.Inflate(Resource.Layout.GridSectorLayout, null);
                            gridView.AddView(newView);
                            var gridLayout = new GridLayout.LayoutParams(GridLayout.InvokeSpec(y), GridLayout.InvokeSpec(x));
                            //gridLayout.SetMargins(2, 2, 2, 2);
                            gridLayout.Width = kGridSize;
                            gridLayout.Height = kGridSize;
                            newView.LayoutParameters = gridLayout;
                            var textField = newView.FindViewById<TextView>(Resource.Id.coordLabel);
                            if (PhabrikServer.CurrentUser.isAdmin)
                            {
                                textField.Text = string.Format("{0},{1}", x, y);
                                textField.SetBackgroundColor(Color.Blue);
                                textField.SetTextColor(Color.White);
                            } else
                            {
                                ((ViewGroup)newView).RemoveView(textField);
                            }
                            
                            var imageView = newView.FindViewById<ImageView>(Resource.Id.backgroundImage);
                            if (curTerrain._sectorArray != null) { 
                                SectorObj curSec = curTerrain._sectorArray[x][y];
                                newView.Tag = curSec.Id;
                                imageView.SetScaleType(ImageView.ScaleType.Center);
                                string url = curSec.sectorUrl;
                                if (string.IsNullOrEmpty(url))
                                    url = curSec.DefaultUrl;
                                else
                                    url += "=s" + kGridSize;

                                Koush.UrlImageViewHelper.SetUrlDrawable(imageView, url, Resource.Drawable.Icon);
                            } else
                            {
                                // todo - handle unknown terrain..
                            }
                            if (!tidallyLocked || x <= width / 2)
                            {
                                var darkness = newView.FindViewById(Resource.Id.nighttime);
                                ((ViewGroup)newView).RemoveView(darkness);
                            }
                        }
                    }

                    selectionView = new View(this.Activity);
                    gridView.AddView(selectionView);
                    selectionView.Visibility = ViewStates.Gone;
                    selectionView.SetBackgroundResource(Resource.Drawable.SelectionBorder);
                });
            }
        }
    }
}
