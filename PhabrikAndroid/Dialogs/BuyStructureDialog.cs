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
using Android.Support.V7.Widget;
using Android.Support.V7.View;
using Android.Support.V7.AppCompat;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Graphics;
using Phabrik.Core;

namespace Phabrik.AndroidApp
{
    public delegate void purchase_callback(StructureTypeObj theObj);

    public class BuyStructureDialog : Android.Support.V4.App.DialogFragment
    {
        ListView categoryList;
        ListView itemList;
        TextView walletStr;
        Button cancelBtn;
        Button buyBtn;
        public purchase_callback callback;
        StructureCatAdapter structAdapter;
        StructureItemAdapter itemAdapter;
        public string curCategory = "";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.BuyStructureLayout, container);

            categoryList = view.FindViewById<ListView>(Resource.Id.categoryList);
            itemList = view.FindViewById<ListView>(Resource.Id.itemList);
            walletStr = view.FindViewById<TextView>(Resource.Id.walletStr);
            cancelBtn = view.FindViewById<Button>(Resource.Id.cancelBtn);
            buyBtn = view.FindViewById<Button>(Resource.Id.buyBtn);

            cancelBtn.Click += (o, e) =>
            {
                Dismiss();
            };

            buyBtn.Click += (o, e) =>
            {
                var curItem = itemAdapter[itemList.CheckedItemPosition];
                Dismiss();
                callback(curItem);
            };

            categoryList.ItemClick += CategoryList_ItemClick;
            categoryList.ChoiceMode = ChoiceMode.Single;

            itemList.ItemClick += ItemList_ItemClick;
            itemList.ChoiceMode = ChoiceMode.Single;

            return view;
        }

        private void ItemList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
        }

        private void CategoryList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            curCategory = structAdapter[e.Position];
            UpdateItemList();
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // wire up our list views
            structAdapter = new StructureCatAdapter(this);
            categoryList.Adapter = structAdapter;
            structAdapter.NotifyDataSetChanged();
            categoryList.InvalidateViews();

            itemAdapter = new StructureItemAdapter(this);
            itemList.Adapter = itemAdapter;
            UpdateItemList();
        }

        private void UpdateItemList()
        {
            itemAdapter.NotifyDataSetChanged();
            itemList.InvalidateViews();
        }

    }

    public class StructureCatAdapter : BaseAdapter<string>
    {
        BuyStructureDialog fragment;
        List<string> keys = new List<string>();


        public StructureCatAdapter(BuyStructureDialog context) : base()
        {
            this.fragment = context;
            foreach (string curKey in SectorPopFragment.catalog.Keys)
            {
                keys.Add(curKey);
            }
            this.fragment.curCategory = keys[0];
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override string this[int position]
        {
            get { return keys[position]; }
        }
        public override int Count
        {
            get { return keys.Count; }
        }

        

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            {
                view = fragment.Activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated1, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = keys[position];

            if (convertView == null)
            {
                // first time init
                view.FindViewById<TextView>(Android.Resource.Id.Text1).SetTypeface(MainActivity.titleFace, TypefaceStyle.Bold);
            }
            return view;
        }
    }

    public class StructureItemAdapter : BaseAdapter<StructureTypeObj>
    {
        BuyStructureDialog fragment;


        public StructureItemAdapter(BuyStructureDialog context) : base()
        {
            this.fragment = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override StructureTypeObj this[int position]
        {
            get { return SectorPopFragment.catalog[fragment.curCategory][position]; }
        }
        public override int Count
        {
            get { return SectorPopFragment.catalog[fragment.curCategory].Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            {
                view = fragment.Activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated2, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = SectorPopFragment.catalog[fragment.curCategory][position].structurename;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = SectorPopFragment.catalog[fragment.curCategory][position].description;

            if (convertView == null)
            {
                // first time init
                view.FindViewById<TextView>(Android.Resource.Id.Text1).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Bold);
                view.FindViewById<TextView>(Android.Resource.Id.Text2).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            }
            return view;
        }
    }
}