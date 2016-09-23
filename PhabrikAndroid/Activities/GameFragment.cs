
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
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using com.refractored;
using Android.Text;
using Android.Text.Style;
using Android.Content.PM;
using Phabrik.Core;
using Java.Lang;

namespace Phabrik.AndroidApp
{
	public class GameFragment : Android.Support.V4.App.Fragment, ViewPager.IOnPageChangeListener
	{
		public MainActivity MainPage { get; set; }
		private Android.Support.V7.Widget.Toolbar toolbar = null;
		public static List<PointOfPresenceFragment> fragmentList = new List<PointOfPresenceFragment>();
		private PagerSlidingTabStrip tabs;
		private ViewPager pager;

		public class LeaderboardPageAdapter : FragmentPagerAdapter, ICustomTabProvider
		{
			Android.Support.V7.App.AppCompatActivity activity;

			public LeaderboardPageAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V7.App.AppCompatActivity theActivity)
				: base(fm)
			{
				activity = theActivity;
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
			{
				return new Java.Lang.String(fragmentList[position].PopTitle);
			}

			public override int Count
			{
				get
				{
					return fragmentList.Count;
				}
			}



			public View GetCustomTabView(ViewGroup parent, int position)
			{
				var tabView = activity.LayoutInflater.Inflate(Resource.Layout.NotifyTabView, null);
				var counter = tabView.FindViewById<TextView>(Resource.Id.counter);
				var title = tabView.FindViewById<TextView>(Resource.Id.psts_tab_title);
				title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
				title.Text = fragmentList[position].PopTitle;
				counter.Visibility = ViewStates.Gone;
				return tabView;
			}

			public override Android.Support.V4.App.Fragment GetItem(int position)
			{
				return fragmentList[position];
			}

            public override int GetItemPosition(Java.Lang.Object objectValue)
            {
                //return base.GetItemPosition(objectValue);
                return PositionNone;
            }
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			var view = inflater.Inflate(Resource.Layout.GameLayout, container, false);
			pager = view.FindViewById<ViewPager>(Resource.Id.post_pager);
			pager.Adapter = new LeaderboardPageAdapter(MainPage.SupportFragmentManager, MainPage);
			pager.AddOnPageChangeListener(this);
			pager.OffscreenPageLimit = 2;
			tabs = view.FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
			tabs.SetViewPager(pager);
			tabs.IndicatorColor = Resources.GetColor(Resource.Color.Phabrik_green);
			tabs.IndicatorHeight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_indicator_height);
			tabs.UnderlineColor = Resources.GetColor(Resource.Color.Phabrik_blue);
			tabs.TabPaddingLeftRight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_padding);
			tabs.OnPageChangeListener = this;
			//tabs.ShouldExpand = true;

			tabs.SetTabTextColor(Color.Blue);
			pager.CurrentItem = 0;
			return view;
		}

		public void InitializeFirstConnection()
		{
            // ready to play!
            if (PhabrikServer.CurrentUser.popList.Count == 0)
            {
                var pop1 = new PointOfPresenceFragment();
                pop1.gameFragment = this;
                
                fragmentList.Add(pop1);

            }
            else {
                foreach (PointOfPresenceObj curPop in PhabrikServer.CurrentUser.popList)
                {
                    /*
                    PointOfPresenceFragment popFrag = null;
                    switch (curPop.scale)
                    {
                        case PointOfPresenceObj.PopScale.Structure:
                            popFrag = new StructurePopFragment();
                            break;
                        case PointOfPresenceObj.PopScale.Sector:
                            popFrag = new SectorPopFragment();
                            break;
                        case PointOfPresenceObj.PopScale.Planet:
                            popFrag = new PlanetPopFragment();
                            break;
                        case PointOfPresenceObj.PopScale.System:
                            popFrag = new SolSysPopFragment();
                            break;
                        case PointOfPresenceObj.PopScale.Galaxy:
                            popFrag = new GalaxyPopFragment();
                            break;


                    }

                    popFrag.gameFragment = this;
                    popFrag.InitPop(curPop);
                    fragmentList.Add(popFrag);
                    */

                }
            }
			pager.Adapter.NotifyDataSetChanged();

		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{

		}

		public void OnPageScrollStateChanged(int state)
		{

		}

		public void OnPageSelected(int position)
		{
			fragmentList[position].Update();
		}

        
	}
}
