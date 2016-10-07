
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
		Button destroyBtn;
		Button inspectBtn;
        public static Dictionary<string, List<StructureTypeObj>> catalog = null;
		View selectionRect;
        bool isDirty = false;
		private View selectionFrame;


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
			destroyBtn = theView.FindViewById<Button>(Resource.Id.buildBtn);
			inspectBtn = theView.FindViewById<Button>(Resource.Id.destroyBtn);
			selectionRect = theView.FindViewById<View>(Resource.Id.inspectBtn);

            buildBtn.Click += BuildBtn_Click;
			inspectBtn.Click += InspectBtn_Click;
			destroyBtn.Click += DestroyBtn_Click;
			inspectBtn.Enabled = false;
			destroyBtn.Enabled = false;

			structureGrid.Touch += StructureGrid_Touch;
			selectionFrame = new View(this.Activity);
			structureGrid.AddView(selectionFrame);
			selectionFrame.Visibility = ViewStates.Gone;
			selectionFrame.SetBackgroundResource(Resource.Drawable.SelectionBorder);
            return theView;
        }

		void InspectBtn_Click(object sender, EventArgs e)
		{

		}

		void DestroyBtn_Click(object sender, EventArgs e)
		{

		}

		bool isDragging = false;
		float startX, startY, dx, dy;
		float lastX, lastY;
		int dragWidth, maxX, maxY;
		int dragHeight;
		int xWidth;
		int yWidth;
		View dragView = null;
		View hitView;


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


				var newDragView = FindViewAtLoc((int)(lastX / cellsize), (int)(lastY / cellsize), 1, 1);
				SetSelectedView(newDragView);
				if (dragView != null)
				{
					dx = startX - dragView.Left;
					dy = startY - dragView.Top;
					dragWidth = dragView.Width;
					dragHeight = dragView.Height;
					xWidth = dragWidth / cellsize;
					yWidth = dragHeight / cellsize;
					isDragging = true;
					int newLeft = (int)(lastX / cellsize) * cellsize;
					int newTop = (int)(lastY / cellsize) * cellsize;
					maxX = 10 - xWidth;
					maxY = 10 - yWidth;

					selectionRect.Visibility = ViewStates.Visible;
					selectionRect.BringToFront();
					selectionRect.Layout(newLeft, newTop, newLeft + dragWidth, newTop + dragHeight);
					selectionFrame.Visibility = ViewStates.Visible;
					selectionFrame.BringToFront();
					selectionFrame.Layout(newLeft-2, newTop-2, newLeft + dragWidth+2, newTop + dragHeight+2);
				}

			}
			else if (e.Event.Action == MotionEventActions.Move)
			{
				if (isDragging)
				{
					lastX = e.Event.GetX() - dx;
					lastY = e.Event.GetY() - dy;
					dragView.Layout((int)lastX, (int)lastY, (int)lastX + dragWidth, (int)lastY + dragHeight);
					int newLeft = (int)(lastX / cellsize) ;
					int newTop = (int)(lastY / cellsize) ;
					if (newLeft > maxX)
						newLeft = maxX;
					if (newTop > maxY)
						newTop = maxY;
					newLeft *= cellsize;
					newTop *= cellsize;

					selectionRect.Layout(newLeft, newTop, newLeft + dragWidth, newTop + dragHeight);
					hitView = FindViewAtLoc((int)(lastX / cellsize), (int)(lastY / cellsize), xWidth, yWidth, dragView);
					if (hitView == null)
						selectionRect.SetBackgroundColor(new Color(0, 255, 0, 128));
					else
						selectionRect.SetBackgroundColor(new Color(255, 0, 0, 128));

					selectionFrame.Visibility = ViewStates.Invisible;


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
						if (newX > maxX)
							newX = maxX;
						if (newY > maxY)
							newY = maxY;
						gridLayout.SetMargins(newX * cellsize, newY * cellsize, 0, 0);
						dragView.LayoutParameters = gridLayout;
						dragView.ForceLayout();
						UpdateStructureLoc(dragView, newX, newY);
						gridLayout = (FrameLayout.LayoutParams)selectionFrame.LayoutParameters;
						gridLayout.SetMargins(newX * cellsize, newY * cellsize, 0, 0);
						gridLayout.Width = dragWidth;
						gridLayout.Height = dragHeight;
						selectionFrame.LayoutParameters = gridLayout;
						selectionFrame.ForceLayout();
						selectionFrame.Visibility = ViewStates.Visible;

					}

					hitView = null;
					selectionRect.Visibility = ViewStates.Invisible;
				}
			} 
			else {
				selectionRect.Visibility = ViewStates.Invisible;
				dragView = null;
				hitView = null;
				isDragging = false;
				SetSelectedView(null);
			}
			
			e.Handled = true;
		}

		private void SetSelectedView(View theView)
		{
			if (dragView != null)
			{
				selectionFrame.Visibility = ViewStates.Gone;
			}

			dragView = theView;

			if (dragView != null)
			{
				selectionFrame.Visibility = ViewStates.Visible;
				inspectBtn.Enabled = true;
				destroyBtn.Enabled = true;
			}
			else {
				inspectBtn.Enabled = false;
				destroyBtn.Enabled = false;
			}

		}

		private void UpdateStructureLoc(View theView, int newX, int newY)
		{
			StructureObj theStruct = StructureForView(theView);
			if (theStruct != null)
			{
				theStruct.xLoc = newX;
				theStruct.yLoc = newY;
				PhabrikServer.UpdateStructureLoc(theStruct);
                isDirty = true;
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
				if (curView != background && curView != selectionRect && curView != ignoreView && curView != selectionFrame)
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
                    isDirty = true;
				});
            }
        }



        private View AddStructureToView(StructureObj newStruct)
        {
			ImageView newView = new ImageView(this.Activity);
            newView.SetScaleType(ImageView.ScaleType.FitXy);
			newView.Tag = newStruct.Id;
            this.Activity.RunOnUiThread(() =>
            {
				structureGrid.AddView(newView);

                Koush.UrlImageViewHelper.SetUrlDrawable(newView, newStruct.imageURL, Resource.Drawable.Icon);
                UpdateStructureLoc(newStruct, newView);
            });

			return newView;
        }

        private void UpdateStructureLoc(StructureObj theStruct, View theView)
        {
			var cellsize = structureGrid.Width / 10;
            if (cellsize == 0)
            {
                cellsize = Resources.DisplayMetrics.WidthPixels / 10;
            }

			var gridLayout = new FrameLayout.LayoutParams(cellsize * theStruct.xSize, cellsize * theStruct.ySize);
			gridLayout.SetMargins(cellsize * theStruct.xLoc, cellsize * theStruct.yLoc, 0, 0);
            theView.LayoutParameters = gridLayout;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
           
        }

        public override void OnPause()
        {
            if (isDirty)
                SaveSectorImage();
            isDirty = false;
            base.OnPause();
        }

        private void SaveSectorImage()
        {
            Bitmap.Config config = Bitmap.Config.Argb8888;

            Bitmap newBitmap = Bitmap.CreateBitmap(1000,1000, config);
            int cellSize = 100;
            Canvas canvas = new Canvas(newBitmap);
            Rect destRect = new Rect(0, 0, 1024, 1024);
            Bitmap bkgnd = BitmapHelper.GetImageBitmapFromUrl(parent.pop.curSector.DefaultUrl);
            Rect srcRect = new Rect(0, 0, bkgnd.Width, bkgnd.Height);
            Paint thePaint = new Paint(PaintFlags.AntiAlias);
            canvas.DrawBitmap(bkgnd, srcRect, destRect, thePaint);
            SectorObj curSector = parent.pop.curSector;

            foreach (StructureObj curStructure in parent.pop.curSector.structures)
            {
                bkgnd = BitmapHelper.GetImageBitmapFromUrl(curStructure.imageURL);
                srcRect = new Rect(0, 0, bkgnd.Width, bkgnd.Height);
                destRect = new Rect(curStructure.xLoc * cellSize, curStructure.yLoc * cellSize,
                    (curStructure.xLoc + curStructure.xSize) * cellSize,
                    (curStructure.yLoc + curStructure.ySize) * cellSize);
                canvas.DrawBitmap(bkgnd, srcRect, destRect, thePaint);
            }
            // at this point, the bitmap should be drawn
            using (System.IO.MemoryStream photoStream = new System.IO.MemoryStream())
            {
                newBitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, photoStream);
                photoStream.Flush();
                PhabrikServer.UploadImage(photoStream, "sector", (newURL) =>
                {
                    parent.UpdateSectorUrl(curSector, newURL);
                });
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            UpdateSector();
        }
        private void UpdateSector()
        {
            var curSector = parent.pop.curSector;
            if (curSector != null)
            {
                header.Text = string.Format("sector {0},{1}", curSector.xLoc, curSector.yLoc);
                background.SetScaleType(ImageView.ScaleType.CenterCrop);
                background.SetAdjustViewBounds(true);
                Koush.UrlImageViewHelper.SetUrlDrawable(background, curSector.DefaultUrl, Resource.Drawable.Icon);

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
