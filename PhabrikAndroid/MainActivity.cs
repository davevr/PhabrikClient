using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.View;
using Android.Support.V7.AppCompat;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Graphics;
using Android.Media;
using Android.Content;
using Android.Runtime;
using Android.Views;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

using Android.Util;
using Android.Text;
using Android.Text.Style;
using Android.Provider;
using Android.Views.InputMethods;

using Phabrik.Core;

namespace Phabrik.AndroidApp
{
    [Activity(Label = "Phabrik", MainLauncher = true, Icon = "@drawable/icon",
	          Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity
    {
        private string[] mDrawerTitles;
		private DrawerLayout mDrawerLayout;
		private ListView mDrawerList;
		private LinearLayout mDrawerView;
		private MyDrawerToggle mDrawerToggle;
		private bool refreshInProgress = false;

		private GameFragment gamePage;
		private ProfileFragment profilePage;
		private AboutFragment aboutPage;

		public static Typeface bodyFace;
		public static Typeface titleFace;
		public static MainActivity instance;
		public static int LOGIN_RESULT = 0x4444;


		class MyDrawerToggle : Android.Support.V7.App.ActionBarDrawerToggle
		{
			private MainActivity baseActivity;

			public MyDrawerToggle(Activity activity, DrawerLayout drawerLayout, int openDrawerContentDescRes, int closeDrawerContentDescRes) :
			base(activity, drawerLayout, openDrawerContentDescRes, closeDrawerContentDescRes)
			{
				baseActivity = (MainActivity)activity;
			}
			public override void OnDrawerOpened(View drawerView)
			{
				base.OnDrawerOpened(drawerView);
				//baseActivity.Title = openString;


			}

			public override void OnDrawerClosed(View drawerView)
			{
				base.OnDrawerClosed(drawerView);
				//baseActivity.Title = closeString;
			}
		}

		class DrawerItemAdapter<T> : ArrayAdapter<T>
		{
			T[] _items;
			Activity _context;

			public DrawerItemAdapter(Context context, int textViewResourceId, T[] objects) :
			base(context, textViewResourceId, objects)
			{
				_items = objects;
				_context = (Activity)context;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View mView = convertView;
				if (mView == null)
				{
					mView = _context.LayoutInflater.Inflate(Resource.Layout.DrawerListItem, parent, false);

				}

				TextView text = mView.FindViewById<TextView>(Resource.Id.ItemName);

				if (_items[position] != null)
				{
					text.Text = _items[position].ToString();
					text.SetTypeface(MainActivity.titleFace, TypefaceStyle.Normal);
				}

				return mView;
			}
		}



		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
			base.OnCreate(savedInstanceState);

			PhabrikServer.InitServer((result) =>
			{
				if (!result)
				{
					ShowAlert("Error", "Cannot connect to server!  Quit and try again later.");
				}
			});

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			bodyFace = Typeface.CreateFromAsset(Assets, "fonts/Lato-Regular.ttf");
			titleFace = Typeface.CreateFromAsset(Assets, "fonts/Orbitron-Regular.ttf");

			// set up drawer
			mDrawerTitles = new string[] {
				Resources.GetText (Resource.String.Game_Menu),
				Resources.GetText (Resource.String.Profile_Menu),
				Resources.GetText (Resource.String.About_Menu)
			};

			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mDrawerList = FindViewById<ListView>(Resource.Id.left_drawer_list);
			mDrawerView = FindViewById<LinearLayout>(Resource.Id.left_drawer);
			// Set the adapter for the list view
			mDrawerList.Adapter = new DrawerItemAdapter<string>(this, Resource.Layout.DrawerListItem, mDrawerTitles);
			// Set the list's click listener
			mDrawerList.ItemClick += mDrawerList_ItemClick;

			mDrawerToggle = new MyDrawerToggle(this, mDrawerLayout, Resource.String.drawer_open, Resource.String.drawer_close);


			mDrawerLayout.AddDrawerListener(mDrawerToggle);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.Phabrik_white)));


			selectItem(0);

			SupportActionBar.Show();
			MainActivity.instance = this;
			Intent firstRun = new Intent(this, typeof(LoginActivity));

			StartActivityForResult(firstRun, LOGIN_RESULT);


		}

		protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
		{
			if (requestCode == LOGIN_RESULT && resultCode == Android.App.Result.Ok)
			{
				FinishLoad();
			}
		}

		private void FinishLoad()
		{
			RunOnUiThread(() =>
			{

				gamePage.InitializeFirstConnection();
			});
		}

		public override bool DispatchTouchEvent(MotionEvent e)
		{
			if (e.Action == MotionEventActions.Down)
			{
				View v = CurrentFocus;
				if (v is EditText)
				{
					Rect outRect = new Rect();
					v.GetGlobalVisibleRect(outRect);
					if (!outRect.Contains((int)e.RawX, (int)e.RawY))
					{
						v.ClearFocus();
						InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
						imm.HideSoftInputFromWindow(v.WindowToken, 0);
					}
				}
			}
			return base.DispatchTouchEvent(e);
		}

		public void ShowAlert(string title, string msg, string buttonText = null)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
				.SetTitle(title)
				.SetMessage(msg)
				.SetPositiveButton(buttonText, (s2, e2) => { })
				.Show();
		}

        public static void ShowAlert(Context context, string title, string msg, string buttonText = null)
        {
            new Android.Support.V7.App.AlertDialog.Builder(context)
                .SetTitle(title)
                .SetMessage(msg)
                .SetPositiveButton(buttonText, (s2, e2) => { })
                .Show();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}

		protected override void OnTitleChanged(Java.Lang.ICharSequence title, Android.Graphics.Color color)
		{
			//base.OnTitleChanged (title, color);
			this.SupportActionBar.Title = title.ToString();

			SpannableString s = new SpannableString(title);

			CustomTypefaceSpan newSpan = new CustomTypefaceSpan(this, "Orbitron-Regular.ttf");
			s.SetSpan(newSpan, 0, s.Length(), SpanTypes.ExclusiveExclusive);

			s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.Phabrik_black)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

			this.SupportActionBar.TitleFormatted = s;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (mDrawerToggle.OnOptionsItemSelected(item))
			{
				return true;
			}
			else
			{
				/*
				switch (item.ItemId)
				{
					case Resource.Id.PhotoButton:
						TakeAPicture();
						return true;
						break;
					case Resource.Id.CatchButton:
						CatchAPicture();
						return true;
						break;
					default:
						// show never get here.
						break;
				}
				*/
			}
			// Handle your other action bar items...

			return base.OnOptionsItemSelected(item);
		}


		void mDrawerList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			selectItem(e.Position);
		}

		private Android.Support.V4.App.Fragment oldPage = null;

		private void selectItem(int position)
		{
			Android.Support.V4.App.Fragment newPage = null;
			var fragmentManager = this.SupportFragmentManager;
			var ft = fragmentManager.BeginTransaction();
			bool firstTime = false;
			string pageName = "";

			switch (position)
			{
				case 0: // game
					if (gamePage == null)
					{
						gamePage = new GameFragment();
						gamePage.MainPage = this;
						firstTime = true;
					}
					newPage = gamePage;
					pageName = "Phabrik";
					break;
				case 1: // profile
					if (profilePage == null)
					{
						profilePage = new ProfileFragment();
						profilePage.MainPage = this;
						firstTime = true;
					}
					newPage = profilePage;
					break;
				case 2: // about
					if (aboutPage == null)
					{
						aboutPage = new AboutFragment();
						aboutPage.MainPage = this;
						firstTime = true;
					}
					newPage = aboutPage;
					break;
			}

			if (oldPage != newPage)
			{
				if (oldPage != null)
				{
					// to do - deactivate it
					ft.Hide(oldPage);

				}

				oldPage = newPage;

				if (newPage != null)
				{
					if (firstTime)
						ft.Add(Resource.Id.fragmentContainer, newPage);
					else
						ft.Show(newPage);
				}

				ft.Commit();

				// update selected item title, then close the drawer
				if (!string.IsNullOrEmpty(pageName))
					Title = pageName;
				else
					Title = mDrawerTitles[position];

				mDrawerList.SetItemChecked(position, true);
				mDrawerLayout.CloseDrawer(mDrawerView);
			}
		}
    }
}

