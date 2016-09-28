
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
using Android.Graphics;
using Phabrik.Core;
using PullToRefresharp.Android;

namespace Phabrik.AndroidApp
{
    public class GalaxyPopFragment : PopSubFragment
    {
        EditText xField;
        EditText yField;
        EditText zField;
        TextView header;
        PullToRefresharp.Android.Widget.ListView systemList;
        private SolSysListAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var theView = inflater.Inflate(Resource.Layout.GalaxyPopLayout, container, false);
            xField = theView.FindViewById<EditText>(Resource.Id.xField);
            yField = theView.FindViewById<EditText>(Resource.Id.yField);
            zField = theView.FindViewById<EditText>(Resource.Id.zField);
            header = theView.FindViewById<TextView>(Resource.Id.header);
            systemList = theView.FindViewById<PullToRefresharp.Android.Widget.ListView>(Resource.Id.systemList);

            systemList.RefreshActivated += SystemList_RefreshActivated;
            return theView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            UpdateGalaxy();
        }

        private void SystemList_RefreshActivated(object sender, EventArgs e)
        {
            RefreshFromServer(true);
        }

        private void RefreshFromServer(bool fromPullDown = false)
        {
            PhabrikServer.FetchKnownSystems((resultList) =>
            {
                resultList.Sort((obj1, obj2) =>
                {
                    int loc1 = obj1.xLoc * obj1.xLoc + obj1.yLoc * obj1.yLoc + obj1.zLoc * obj1.zLoc;
                    int loc2 = obj2.xLoc * obj2.xLoc + obj2.yLoc * obj2.yLoc + obj2.zLoc * obj2.zLoc;

                    if (loc1 < loc2)
                        return -1;
                    else if (loc1 > loc2)
                        return 1;
                    else
                        return 0;

                });
                this.Activity.RunOnUiThread(() =>
                {
                    header.Text = string.Format("{0} known systems", resultList.Count);
                    if (adapter == null)
                    {
                        if (adapter == null)
                        {
                            adapter = new SolSysListAdapter(this, resultList);
                        }
                        systemList.Adapter = adapter;
                        RefreshListView();
                    }

                    if (fromPullDown)
                        systemList.OnRefreshCompleted();
                });

            });
        }

        public void UpdateGalaxy()
        {
            if (adapter != null)
            {
                systemList.Adapter = adapter;
                RefreshListView();
            } else
            {
                RefreshFromServer();
            }
            
        }
        private void RefreshListView()
        {
            if (this.View != null)
            {
                Activity.RunOnUiThread(() =>
                {
                    adapter.NotifyDataSetChanged();
                    systemList.InvalidateViews();
                });
            }
        }
    }

    public class SolSysListAdapter : BaseAdapter<SolSysObj>
    {
        public List<SolSysObj> allItems;
        GalaxyPopFragment fragment;
        private bool[] expandMap;


        public SolSysListAdapter(GalaxyPopFragment context, List<SolSysObj> theItems) : base()
        {
            this.fragment = context;
            this.allItems = theItems;
            expandMap = new bool[theItems.Count];
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override SolSysObj this[int position]
        {
            get { return allItems[position]; }
        }
        public override int Count
        {
            get { return allItems.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            {
                view = fragment.Activity.LayoutInflater.Inflate(Resource.Layout.SolSysItemLayout, null);
            }
            var titleView = view.FindViewById<TextView>(Resource.Id.SolSysTitle);
            var coordView = view.FindViewById<TextView>(Resource.Id.solSysCoord);
            var infoView = view.FindViewById<TextView>(Resource.Id.solSysInfo);
            var showMoreBtn = view.FindViewById<TextView>(Resource.Id.showMore);
            var layout = view.FindViewById<LinearLayout>(Resource.Id.MoreInfo);
            var gotoBtn = view.FindViewById<Button>(Resource.Id.gotoSystem);

            if (convertView == null)
            {
                // first time init
                titleView.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
                infoView.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                // all the other views... sigh
                view.FindViewById<TextView>(Resource.Id.distanceStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.massStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.tempStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.dayStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Resource.Id.yearStatic).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);

                titleView.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
                coordView.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
                infoView.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
                showMoreBtn.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Italic);

                gotoBtn.Click += GotoBtn_Click;
                showMoreBtn.Click += ShowMoreBtn_Click;

            }



            SolSysObj curItem = allItems[position];
            titleView.Text = curItem.systemName;
            coordView.Text = string.Format("[{0},{1},{2}]", curItem.xLoc, curItem.yLoc, curItem.zLoc);
            infoView.Text = string.Format("{0} planets, {1} habitable, {2} earthlike", curItem.planetcount, curItem.suns[0].habitable, curItem.suns[0].earthlike);

            gotoBtn.Tag = curItem.Id;

            if (expandMap[position])
            {
                layout.Visibility = ViewStates.Visible;
                showMoreBtn.Text = "hide details";
            }
            else
            {
                layout.Visibility = ViewStates.Gone;
                showMoreBtn.Text = "show details";
            }

            return view;
        }
        private void ShowMoreBtn_Click(object sender, EventArgs e)
        {
            TextView theText = sender as TextView;

            if (theText != null)
            {
                var parentView = theText.Parent as View;
                LinearLayout layout = parentView.FindViewById<LinearLayout>(Resource.Id.MoreInfo);

                if (layout.Visibility == ViewStates.Gone)
                {
                    layout.Visibility = ViewStates.Visible;
                    theText.Text = "hide details";
                    expandMap[(int)layout.Tag] = true;
                }
                else
                {
                    layout.Visibility = ViewStates.Gone;
                    theText.Text = "show details";
                    expandMap[(int)layout.Tag] = false;
                }
            }

        }

        private void GotoBtn_Click(object sender, EventArgs e)
        {
            Button gotoBtn = sender as Button;

            if (gotoBtn != null)
            {
                long systemId = (long)gotoBtn.Tag;
                fragment.parent.GotoSolSys(systemId);

            }
        }

    }
}
