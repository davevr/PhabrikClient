
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
	public class PointOfPresenceFragment : Android.Support.V4.App.Fragment
	{
		public string PopTitle { get; set;} = "Point of Presence";
		public PointOfPresenceObj pop { get; set; } = null;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			 return inflater.Inflate(Resource.Layout.PointOfPresenceLayout, container, false);

			//return base.OnCreateView(inflater, container, savedInstanceState);
		}

		public void InitPop(PointOfPresenceObj newPop)
		{
			pop = newPop;
			PopTitle = newPop.nickname;
			if (string.IsNullOrEmpty(PopTitle))
				PopTitle = "New Point of Presence";
		}

		public void Update()
		{

		}


	}
}
