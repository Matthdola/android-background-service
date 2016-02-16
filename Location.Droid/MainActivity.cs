using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Locations;
using Android.Content.PM;

using Location.Droid.Services;


namespace Location.Droid
{
	[Activity (Label = "Location.Droid", MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenLayout)]
	public class MainActivity : Activity
	{
		readonly string logTag = "MainActivity";
		//make our labels
		TextView latText;
		TextView longText;
		TextView altText;
		TextView speedText;
		TextView bearText;
		TextView accText;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			Log.Debug (logTag, "OnCreate: Location app is becoming active");


			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//This event fires when the ServiceConnection lets the client (our App class) know that
			//the Service is connected. We use this event to start updating the UI with the location
			//updates from the service

			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
				Log.Debug(logTag, "ServiceConnected Event Raised");
					//notifies uf of location changes from the system
				App.Current.LocationService.LocationChanged += HandleLocationChanged;

				//notifies us of the user changes to the location provider (ie user disabled or enabled GPS
				App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
				App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;

				//notifies us of the changeing status of a provider (ie GPS no longer availavle)
				App.Current.LocationService.StatusChanged += HandleStatusChanged;
			};

			latText = FindViewById<TextView> (Resource.Id.lat);
			longText = FindViewById<TextView> (Resource.Id.longx);
			altText = FindViewById<TextView> (Resource.Id.alt);
			speedText = FindViewById<TextView> (Resource.Id.speed);
			bearText = FindViewById<TextView> (Resource.Id.bear);
			accText = FindViewById<TextView> (Resource.Id.acc);

			altText.Text = "altitude";
			speedText.Text = "speed";
			bearText.Text = "bearing";
			accText.Text = "accuracy";

			//Start the location service
			App.StartLocationService();
		}

		protected override void OnPause ()
		{
			Log.Debug (logTag, "OnPause: Location app is movind to background");
			base.OnPause ();
		}

		protected override void OnResume ()
		{
			Log.Debug (logTag, "OnPause: Location app is movind to foreground");
			base.OnResume ();
		}

		protected override void OnDestroy ()
		{
			Log.Debug (logTag, "OnPause: Location app is becoming inactive");
			base.OnDestroy ();

			//Stop the location service
			App.StopLocationService();
		}

		public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		{
			Android.Locations.Location location = e.Location;
			Log.Debug (logTag, "Foreground updating");

			//These event are on a background thread, need to update on the UI thread
			RunOnUiThread(() => {
				
				latText.Text = string.Format("Latitude: {0}", location.Latitude);
				longText.Text = string.Format("Longitude: {0}", location.Longitude);
				altText.Text = string.Format("Altitude: {0}", location.Altitude);
				speedText.Text = string.Format("Speed: {0}", location.Speed);
				accText.Text = string.Format("Accuracy: {0}", location.Accuracy);
				bearText.Text = string.Format("Bearing: {0}", location.Bearing);
			});
		}

		public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
		{
			Log.Debug (logTag, "Location provider disabled event raised");
		}

		public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
		{
			Log.Debug (logTag, "Location provider enabled event raised");
		}

		public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
		{
			Log.Debug (logTag, "Location status changed event raised");
		}
	}
}


