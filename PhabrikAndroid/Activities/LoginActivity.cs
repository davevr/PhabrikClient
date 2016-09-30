
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
using Android.Preferences;

using Phabrik.Core;

namespace Phabrik.AndroidApp
{
	[Activity(Label = "Phabrik Login", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class LoginActivity : Android.Support.V7.App.AppCompatActivity
	{
		private LinearLayout manualLayout;
		private LinearLayout autoLayout;
		private TextView signinText;
		private EditText usernameField;
		private EditText passwordField;
		private EditText confirmField;
		private CheckBox createBtn;
		private TextView confirmLabel;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.LoginLayout);
			var signinBtn = FindViewById<Button>(Resource.Id.SignInBtn);

			signinText = FindViewById<TextView>(Resource.Id.signinTextField);
			manualLayout = FindViewById<LinearLayout>(Resource.Id.manualSignIn);
			autoLayout = FindViewById<LinearLayout>(Resource.Id.AutoSignIn);
			usernameField = FindViewById<EditText>(Resource.Id.userNameField);
			passwordField = FindViewById<EditText>(Resource.Id.passwordField);
			confirmField = FindViewById<EditText>(Resource.Id.confirmField);
			confirmLabel = FindViewById<TextView>(Resource.Id.confirmText);
			createBtn = FindViewById<CheckBox>(Resource.Id.createBtn);


			signinBtn.Click += (sender, e) =>
			{
				AttemptSignIn();
			};

			manualLayout.Visibility = ViewStates.Gone;
			signinText.Text = "fetching saved login...";
			autoLayout.Visibility = ViewStates.Visible;
			confirmLabel.Visibility = ViewStates.Gone;
			confirmField.Visibility = ViewStates.Gone;
			createBtn.Checked = false;

			createBtn.Click += (sender, e) => {
				if (createBtn.Checked)
				{
					confirmLabel.Visibility = ViewStates.Visible;
					confirmField.Visibility = ViewStates.Visible;
				}
				else {
					confirmLabel.Visibility = ViewStates.Gone;
					confirmField.Visibility = ViewStates.Gone;
				}
			};


			DoAutoSignIn();
		}


		private void DoAutoSignIn()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			string username = prefs.GetString("username", "");
			string pwd = prefs.GetString("pwd", "");

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pwd))
			{
				DoManualLogin();
			}
			else {
				signinText.Text = "Signing in " + username + "...";
				PhabrikServer.Login(username, pwd, false, (theResult) =>
				{
					if (theResult == null)
					{
						// login failed
						RunOnUiThread(() => { DoManualLogin(); });
					}
					else {
						this.SetResult(Result.Ok, new Intent());
						Finish();
					}
				});
			}
		}

		private void DoManualLogin()
		{
			manualLayout.Visibility = ViewStates.Visible;
			autoLayout.Visibility = ViewStates.Gone;
		}

		private void AttemptSignIn()
		{
			string username = usernameField.Text;
			string pwd = passwordField.Text;
			string confirmpwd = confirmField.Text;
			bool create = createBtn.Checked;

			if (create && pwd != confirmpwd)
			{
				ShowAlert("Login Error", "Passwords don't match!", "OK");
			}
			else {
				signinText.Text = "signing in " + username + "...";
				autoLayout.Visibility = ViewStates.Visible;
				manualLayout.Visibility = ViewStates.Gone;

				PhabrikServer.Login(username, pwd, create, (theResult) =>
				{
					if (theResult != null)
					{
						// user is logged in!
						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
						var editor = prefs.Edit();
						editor.PutString("username", username);
						editor.PutString("pwd", pwd);
						editor.Apply();
						this.SetResult(Result.Ok, new Intent());
						Finish();
					}
					else {
						RunOnUiThread(() =>
						{
							if (create)
							{
								ShowAlert("Login Error", "Account creation failed.  Make sure username is unique!", "OK");
							}
							else {
								ShowAlert("Login Error", "Login failed.  Check username and password.", "OK");
							}

							DoManualLogin();
						});
					}
				});

			}

		}

		public void ShowAlert(string title, string msg, string buttonText = null)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
				.SetTitle(title)
				.SetMessage(msg)
				.SetPositiveButton(buttonText, (s2, e2) => { })
				.Show();
		}
	}
}
