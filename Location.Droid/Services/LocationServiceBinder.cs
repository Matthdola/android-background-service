using System;
using Android.OS;

namespace Location.Droid.Services
{
	public class LocationServiceBinder : Binder
	{
		public LocationService Service
		{
			get { return this.service; }
		}

		protected LocationService service;

		public bool IsBound { get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="Location.Droid.Services.LocationServiceBinder"/> class.
		/// </summary>
		/// <param name="service">Service.</param>
		public LocationServiceBinder (LocationService service)
		{
			this.service = service;
		}
	}
}

