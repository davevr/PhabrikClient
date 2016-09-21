
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
	public class SolSysPopFragment : PointOfPresenceFragment
	{
		private SolSysObj curSystem;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			View theView = inflater.Inflate(Resource.Layout.SolSysPopLayout, container, false);

			return theView;
		}

		public void InitializeForXYZ(int xLoc, int yLoc, int zLoc)
		{
			PhabrikServer.FetchSolSys(xLoc, yLoc, zLoc, (theSys) =>
			{
				if (theSys != null)
				{
					curSystem = theSys;
				}
			});
		}
	}
}
