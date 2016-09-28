
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

namespace Phabrik.AndroidApp
{
	public class SectorPopFragment : PopSubFragment
    {
        TextView header;
        GridLayout structureGrid;
        ImageView background;
        Button buildBtn;

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

            return theView;
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
