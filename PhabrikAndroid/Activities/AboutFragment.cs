﻿
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
	public class AboutFragment : Android.Support.V4.App.Fragment
	{
		public MainActivity MainPage { get; set; }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			var view = inflater.Inflate(Resource.Layout.AboutLayout, container, false);
			var title = view.FindViewById<TextView>(Resource.Id.titleAboutGame);
			title.SetTypeface(MainActivity.titleFace, Android.Graphics.TypefaceStyle.Bold);

			title = view.FindViewById<TextView>(Resource.Id.titleAboutAuthor);
			title.SetTypeface(MainActivity.titleFace, Android.Graphics.TypefaceStyle.Bold);

			title = view.FindViewById<TextView>(Resource.Id.textAboutGame);
			title.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);

			title = view.FindViewById<TextView>(Resource.Id.textAboutDavevr);
			title.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);


			return view;
		}
	}
}
