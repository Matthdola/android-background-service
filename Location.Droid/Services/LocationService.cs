using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Android.Locations;

namespace Location.Droid.Services
{
	[Service]
	public class LocationService: Service, ILocationListener
	{
		public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate{};
		public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate{};
		public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate{};
		public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate{};

		public LocationService ()
		{
		}

		//Set our location manager as the system loaction service
		protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;

		readonly string logTag = "LocationService";
		IBinder binder;

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (logTag, "OnCreate called in the Location Service");
		}

		//This gets called when StartService is called in our App class
		[Obsolete("deprecated in base class")]
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (logTag, "LocationService started");

			return StartCommandResult.Sticky;
		}

		//This called once, the first time any client bind to the service
		//and returns ans instance of the LocationServiceBinder. All funtuer clients will
		//reuse the same instance of the binder
		public override IBinder OnBind (Intent intent)
		{
			Log.Debug (logTag, "Client now bound to service");

			binder = new LocationServiceBinder (this);

			return binder;
		}

		//Handle location updates from the location manager
		public void StartLocationUpdates()
		{
			//We can set different location criteria based on requirements for our app
			//for example, we might want to preserve power, or get extreme accuracy
			var locationCriteria = new Criteria();

			locationCriteria.Accuracy = Accuracy.NoRequirement;
			locationCriteria.PowerRequirement = Power.NoRequirement;


			//Get provider: GPS, Network, etc.
			var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
			Log.Debug(logTag, string.Format("You are about to get location updates via {0}", locationProvider));

			//Get and initial fix on location
			LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this);

			Log.Debug (logTag, "Now sending location updates");
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			Log.Debug (logTag, "Service has been terminated");
		}

		//ILocationListeneris a way for the service to subscribe for updates
		//From the System location Service

		public void OnLocationChanged (Android.Locations.Location location)
		{
			this.LocationChanged (this, new LocationChangedEventArgs (location));

			//This should be updateing every time we request new location updates
			//both when the app is in the boackground, and in the foreground
			Log.Debug(logTag, String.Format("Latitude is {0}", location.Latitude));
			Log.Debug(logTag, String.Format("Longitude is {0}", location.Longitude));
			Log.Debug(logTag, String.Format("Altitude is {0}", location.Altitude));

			Log.Debug(logTag, String.Format("Speed is {0}", location.Speed));
			Log.Debug(logTag, String.Format("Accuracy is {0}", location.Accuracy));
			Log.Debug(logTag, String.Format("Bearing is {0}", location.Bearing));
		}

		public void OnProviderDisabled (string provider)
		{
			this.ProviderDisabled (this, new ProviderDisabledEventArgs (provider));
		}

		public void OnProviderEnabled (string provider)
		{
			this.ProviderEnabled (this, new ProviderEnabledEventArgs (provider));
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			this.StatusChanged (this, new StatusChangedEventArgs (provider, status, extras));
		}
	}
}

