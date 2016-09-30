
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
	public class SectorPopFragment : PopSubFragment
    {
        TextView header;
        FrameLayout structureGrid;
        ImageView background;
        Button buildBtn;
        public static Dictionary<string, List<StructureTypeObj>> catalog = null;
		View selectionRect;


        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            var theView = inflater.Inflate(Resource.Layout.SectorPopLayout, container, false);

            header = theView.FindViewById<TextView>(Resource.Id.header);
            structureGrid = theView.FindViewById<FrameLayout>(Resource.Id.sectorgrid);
            background = theView.FindViewById<ImageView>(Resource.Id.background);
            buildBtn = theView.FindViewById<Button>(Resource.Id.buildBtn);
			selectionRect = theView.FindViewById<View>(Resource.Id.selection);

            buildBtn.Click += BuildBtn_Click;

			structureGrid.Touch += StructureGrid_Touch;
            return theView;
        }

		bool isDragging = false;
		float startX, startY;
		float lastX, lastY;
		int dragWidth;
		int dragHeight;
		int xWidth;
		int yWidth;
		View dragView = null;
		View hitView = null;

		void StructureGrid_Touch(object sender, View.TouchEventArgs e)
		{
			int cellsize = structureGrid.Width / 10;
			if (e.Event.Action == MotionEventActions.Down)
			{
				Console.WriteLine("starting drag");
				startX = e.Event.GetX();
				startY = e.Event.GetY();

				lastX = startX;
				lastY = startY;
				int newLeft = (int)(lastX / cellsize) * cellsize;
				int newTop = (int)(lastY / cellsize) * cellsize;

				dragView = FindViewAtLoc((int)(lastX / cellsize), (int)(lastY / cellsize), 1, 1);
				if (dragView != null)
				{
					dragWidth = dragView.Width;
					dragHeight = dragView.Height;
					xWidth = dragWidth / cellsize;
					yWidth = dragHeight / cellsize;
					isDragging = true;

					selectionRect.Visibility = ViewStates.Visible;
					selectionRect.BringToFront();
					selectionRect.Layout(newLeft, newTop, newLeft + cellsize, newTop + cellsize);
				}

			}
			else if (e.Event.Action == MotionEventActions.Move)
			{
				if (isDragging)
				{
					lastX = e.Event.GetX();
					lastY = e.Event.GetY();
					dragView.Layout((int)lastX, (int)lastY, (int)lastX + dragWidth, (int)lastY + dragHeight);
					int newLeft = (int)(lastX / cellsize) * cellsize;
					int newTop = (int)(lastY / cellsize) * cellsize;
					selectionRect.Layout(newLeft, newTop, newLeft + dragWidth, newTop + dragHeight);
					hitView = FindViewAtLoc((int)(lastX / cellsize), (int)(lastY / cellsize), xWidth, yWidth, dragView);
					if (hitView == null)
						selectionRect.SetBackgroundColor(new Color(0, 255, 0, 128));
					else
						selectionRect.SetBackgroundColor(new Color(255, 0, 0, 128));


				}
			}
			else if (e.Event.Action == MotionEventActions.Up)
			{
				if (isDragging)
				{
					Console.WriteLine("drag ended");
					isDragging = false;
					if (hitView == null)
					{
						FrameLayout.LayoutParams gridLayout = (FrameLayout.LayoutParams)dragView.LayoutParameters;
						int newX = (int)(lastX / cellsize);
						int newY = (int)(lastY / cellsize);
						gridLayout.SetMargins(newX * cellsize, newY * cellsize, 0, 0);
						dragView.LayoutParameters = gridLayout;
						dragView.ForceLayout();
						UpdateStructureLoc(dragView, newX, newY);
					}

					dragView = null;
					hitView = null;
					selectionRect.Visibility = ViewStates.Invisible;
				}
			}
			else {
				selectionRect.Visibility = ViewStates.Invisible;
				dragView = null;
				hitView = null;
				isDragging = false;
			}
			
			e.Handled = true;
		}

		private void UpdateStructureLoc(View theView, int newX, int newY)
		{
			StructureObj theStruct = StructureForView(theView);
			if (theStruct != null)
			{
				theStruct.xLoc = newX;
				theStruct.yLoc = newY;
				PhabrikServer.UpdateStructureLoc(theStruct);
			}
		}

		private View ViewForStructure(StructureObj theStruct)
		{
			return structureGrid.FindViewWithTag(theStruct.Id);
		}

		private StructureObj StructureForView(View theView)
		{
			long theId = (long)theView.Tag;
			return parent.pop.curSector.structures.Find(str => str.Id == theId);
		}

		private View FindViewAtLoc(int xloc, int yLoc, int xWidth, int yWidth, View ignoreView = null)
		{
			int cellSize = structureGrid.Width / 10;

			Rect gridRect = new Rect(xloc * cellSize, yLoc * cellSize, (xloc + xWidth) * cellSize, (yLoc + yWidth) * cellSize);

			for (int i = 0; i < structureGrid.ChildCount; i++)
			{
				var curView = structureGrid.GetChildAt(i);
				if (curView != background && curView != selectionRect && curView != ignoreView)
				{
					Rect curRect = new Rect();
					curView.GetHitRect(curRect);
					if (curRect.Intersect(gridRect))
						return curView;
				}
			}

			return null;
		}


		private void BuildBtn_Click(object sender, EventArgs e)
        {
            if (catalog == null)
            {
                PhabrikServer.FetchStructureCatalog((theResult) =>
                {
                    catalog = new Dictionary<string, List<StructureTypeObj>>();

                    foreach (StructureTypeObj curObj in theResult)
                    {
                        if (catalog.ContainsKey(curObj.structuretype))
                            catalog[curObj.structuretype].Add(curObj);
                        else
                        {
                            List<StructureTypeObj> newList = new List<StructureTypeObj>();
                            newList.Add(curObj);
                            catalog[curObj.structuretype] = newList;
                        }
                    }
                    
                    this.Activity.RunOnUiThread(() =>
                    {
                        ShowCatalog();
                    });

                });
            } else
            {
                ShowCatalog();
            }
        }

        public void ShowCatalog()
        {
            Android.Support.V4.App.FragmentTransaction ft = this.Activity.SupportFragmentManager.BeginTransaction();

            // Create and show the dialog.
            
            Android.Support.V4.App.Fragment prev = this.Activity.SupportFragmentManager.FindFragmentByTag("shopdialog");
            if (prev != null)
                ft.Remove(prev);
            ft.AddToBackStack(null);
            BuyStructureDialog newFragment = new BuyStructureDialog();
            newFragment.callback = (theObj) =>
            {
                HandlePurchase(theObj);
            };

            newFragment.Show(ft, "shopdialog");
        }

       private void HandlePurchase(StructureTypeObj theObj)
        {
            System.Console.WriteLine("Purchased item " + theObj.structurename);
            StructureObj newStruct = StructureObj.Instantiate(theObj);
            if (parent.pop.curSector != null)
            {
                parent.pop.curSector.structures.Add(newStruct);
                newStruct.sectorId = parent.pop.curSector.Id;
				newStruct.ownerId = PhabrikServer.CurrentUser.Id;
				PhabrikServer.SaveNewStructure(newStruct, (newId) =>
				{
					
					AddStructureToView(newStruct);
				});
            }
        }



        private View AddStructureToView(StructureObj newStruct)
        {
			ImageView newView = new ImageView(this.Activity);
			newView.Tag = newStruct.Id;
            this.Activity.RunOnUiThread(() =>
            {
				structureGrid.AddView(newView);

                Koush.UrlImageViewHelper.SetUrlDrawable(newView, newStruct.url, Resource.Drawable.Icon);
                UpdateStructureLoc(newStruct, newView);
            });

			return newView;
        }

        private void UpdateStructureLoc(StructureObj theStruct, View theView)
        {
			var cellsize = structureGrid.Width / 10;

			var gridLayout = new FrameLayout.LayoutParams(cellsize * theStruct.xSize, cellsize * theStruct.ySize);
			gridLayout.SetMargins(cellsize * theStruct.xLoc, cellsize * theStruct.yLoc, 0, 0);
            theView.LayoutParameters = gridLayout;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            UpdateSector();
        }

        private void UpdateSector()
        {
            var curSector = parent.pop.curSector;
            if (curSector != null)
            {
                header.Text = string.Format("sector {0},{1}", curSector.xLoc, curSector.yLoc);
                background.SetScaleType(ImageView.ScaleType.FitCenter);
                Koush.UrlImageViewHelper.SetUrlDrawable(background, curSector.TextureURL, Resource.Drawable.Icon);

				foreach (StructureObj curStructure in parent.pop.curSector.structures)
				{
					AddStructureToView(curStructure);
				}
            } else
            {
                header.Text = "Sorry, no sector...";
            }
        }


			


    }
}
