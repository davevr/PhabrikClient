
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
	public class SectorPopFragment : PopSubFragment
    {
        TextView header;
        GridLayout structureGrid;
        ImageView background;
        Button buildBtn;
        public static Dictionary<string, List<StructureTypeObj>> catalog = null;
        int PURCHASE_RESULT = 0x2121;


        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            var theView = inflater.Inflate(Resource.Layout.SectorPopLayout, container, false);

            header = theView.FindViewById<TextView>(Resource.Id.header);
            structureGrid = theView.FindViewById<GridLayout>(Resource.Id.sectorgrid);
            background = theView.FindViewById<ImageView>(Resource.Id.background);
            buildBtn = theView.FindViewById<Button>(Resource.Id.buildBtn);

            buildBtn.Click += BuildBtn_Click;
            return theView;
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
                newStruct.sector = parent.pop.curSector;
                AddStructureToView(newStruct);
            }

        }

        private void AddStructureToView(StructureObj newStruct)
        {
            this.Activity.RunOnUiThread(() =>
            {
                ImageView newView = new ImageView(this.Activity);
                structureGrid.AddView(newView);

                Koush.UrlImageViewHelper.SetUrlDrawable(newView, newStruct.url, Resource.Drawable.Icon);
                UpdateStructureLoc(newStruct, newView);
            });
            

        }

        private void UpdateStructureLoc(StructureObj theStruct, View theView)
        {
            var gridLayout = new GridLayout.LayoutParams(GridLayout.InvokeSpec(theStruct.xLoc), GridLayout.InvokeSpec(theStruct.yLoc));
            gridLayout.SetMargins(2, 2, 2, 2);
            var cellsize = structureGrid.Width / 10;
            gridLayout.Width = cellsize * theStruct.xSize;
            gridLayout.Height = cellsize * theStruct.ySize;
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
            } else
            {
                header.Text = "Sorry, no sector...";
            }
        }


    }
}
