using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Mapsui.Layers;
using Mapsui.Utilities;
using Microsoft.Maui.ApplicationModel;

namespace WebSocketClient.Pages.OnDeviceService;

public partial class LocationAlertPage : ContentPage
{

	public LocationAlertPage()
	{
		InitializeComponent();
	}
	private async void ContentPage_Loaded(object sender, EventArgs e) {
		var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
		if (status != PermissionStatus.Granted) {
			return;
		}

		LocMapView.Map.Navigator.ZoomTo(10);
		var seoul_loc = SphericalMercator.FromLonLat(127.0, 37.5);
		LocMapView.Map.Navigator.CenterOn(seoul_loc.x, seoul_loc.y);
		LocMapView.Map.Navigator.FlyTo(new MPoint(seoul_loc.x, seoul_loc.y), 10, duration: 0);
	}
	protected override void OnAppearing()
	{
		base.OnAppearing();

		var osmLayer = OpenStreetMap.CreateTileLayer();
		var map = new Mapsui.Map { CRS = "EPSG:3857" };
		map.Layers.Add(osmLayer);
		LocMapView.Map = map;
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
		LocMapView.Map.Navigator.FlyTo(new MPoint(ttt.x, ttt.y), 10, duration: 0);


		// 중심 좌표 설정
		// 마커 스타일 생성
		var marker = new Mapsui.UI.Maui.Pin
		{
			Position = new Position(curLoc.Longitude, curLoc.Latitude),
			Label = "현재 위치",
			Type = PinType.Pin
		};

		LocMapView.Pins.Add(marker);
	}

}