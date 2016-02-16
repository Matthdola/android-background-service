using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Util;

using Location.Droid.Services;

// Singleton class for Application wide objects.
namespace Location.Droid
{
	public class App
	{
		//Events
		public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate {};

		//declarations
		protected readonly string logTag = "App";
		protected static LocationServiceConnection locationServiceConnection;

		//Properties
		public static App Current
		{
			get { return current; }
		}

		private static App current;

		public LocationService LocationService
		{
			get{
				if (locationServiceConnection.Binder == null)
					throw new Exception ("Service not bound yet");
				//note that we use the serviceConnection to get the binder, and the binder to get the Service
				return locationServiceConnection.Binder.Service;
			}
		}


		static App ()
		{
			current = new App ();
		}

		protected App()
		{
			//Create a new service connection so we can get a binder to the service
			locationServiceConnection = new LocationServiceConnection(null);

			//this event will fire when the Service connection in the OnServiceConnected call
			locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
				Log.Debug(logTag, "Service Connected");
				//We will use this event to notify MainActivity when to start updating the UI
				this.LocationServiceConnected(this, e);
			};
		}

		public static void StartLocationService()
		{
			//Starting a service like this is blocking, so we want to do it on a background thread
			new Task (() => {
				//Start our main service
				Log.Debug("App", "Calling StartService");
				Android.App.Application.Context.StartService(new Intent(Android.App.Application.Context, typeof(LocationService)));

				//bind our service  (Android goes and finds the runnong service by type, and puts a reference
				//on the binder to that service)
				//The intent telles the OS where to find our Service(the content) and the type of Service
				//we are looking for (LocationService)
				Intent locationServiceIntent = new Intent(Android.App.Application.Context, typeof(LocationService));
				Log.Debug("App", "Calling service binding");

				//Finally, we can bind to the Service using our Intent and the ServiceConnection we
				//Created in a previous step
				Android.App.Application.Context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
			}).Start();
		}

		public static void StopLocationService()
		{
			//Check for nulls in case StartLocationService task has not yet completed
			Log.Debug("App", "StopLocationService");

			//Unbind from the LocationService, otherwise, Stopwise, StopSelf (below) will not work;
			if (locationServiceConnection != null) {
				Log.Debug ("App", "Unbinding from LocationService");
				Android.App.Application.Context.UnbindService (locationServiceConnection);
			}

			//Stop the LocationService
			if (Current.LocationService != null) {
				Log.Debug ("App", "Stopping the LocationService");
				Current.LocationService.StopSelf ();
			}
		}
	}
}

