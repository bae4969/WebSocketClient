using Mapsui;
using Mapsui.Projections;
using Mapsui.UI.Maui;

namespace WebSocketClient.Pages;

public partial class LocationAlertPage : ContentPage
{
	private bool _hasLocationPermission = false;


	public LocationAlertPage()
	{
		InitializeComponent();

		LocMapView.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();


		var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
		if (status != PermissionStatus.Granted)
		{
			_hasLocationPermission = false;
			return;
		}

		_hasLocationPermission = true;

	}

	public async Task<Location?> GetCurrentLocationAsync()
	{
		try
		{
			var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
			{
				DesiredAccuracy = GeolocationAccuracy.Medium,
				Timeout = TimeSpan.FromSeconds(30)
			});

			if (location != null)
			{
				return location;
			}
		}
		catch { }

		return null;
	}

	public async void OnGetLocationButtonClicked(object sender, EventArgs e)
	{
		var curLoc = await GetCurrentLocationAsync();
		if (curLoc == null) return;

		var ttt = SphericalMercator.FromLonLat(curLoc.Longitude, curLoc.Latitude);
		LocMapView.Map.Navigator.FlyTo(new MPoint(ttt.x, ttt.y), 500, duration: 1000);

	}

}