using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;

namespace WebSocketClient.Pages.OnDeviceService;

public partial class StereoCameraPage : ContentPage
{
	private readonly ICameraProvider? _camera_provider = Application.Current?.Handler?.MauiContext?.Services?.GetService<ICameraProvider>();


	public StereoCameraPage() {
		InitializeComponent();
	}
	private async void ContentPage_Loaded(object sender, EventArgs e) {
		var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
		var microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();
		var storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
		if (cameraStatus != PermissionStatus.Granted ||
			microphoneStatus != PermissionStatus.Granted ||
			storageStatus != PermissionStatus.Granted) {
			return;
		}

		if (_camera_provider == null) {
			await Application.Current.MainPage.DisplayAlert("Error", $"Empty camera provider", "OK");
			return;
		}
	}

	protected async override void OnNavigatedTo(NavigatedToEventArgs args) {
		base.OnNavigatedTo(args);

		await _camera_provider.RefreshAvailableCameras(CancellationToken.None);
		var cam_list = _camera_provider.AvailableCameras.Where(c => c.Position == CameraPosition.Rear);

		if(cam_list.Count() < 2) {
			await Application.Current.MainPage.DisplayAlert("Error", $"Not enough cameras", "OK");
			return;
		}

		CameraView1.SelectedCamera = cam_list.First();
		CameraView2.SelectedCamera = cam_list.Last();
	}
	protected override void OnNavigatedFrom(NavigatedFromEventArgs args) {
		base.OnNavigatedFrom(args);

		CameraView1.Handler?.DisconnectHandler();
		CameraView2.Handler?.DisconnectHandler();
	}

	// Ä¸ÃÄ ÀÌº¥Æ®
	private void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e) {
		//if (Dispatcher.IsDispatchRequired) {
		//	Dispatcher.Dispatch(() => MyImage.Source = ImageSource.FromStream(() => e.Media));
		//	return;
		//}

		//MyImage.Source = ImageSource.FromStream(() => e.Media);
	}
	private void Button_Clicked(object sender, EventArgs e) {
		//await CameraView1.CaptureImage(CancellationToken.None);
		//await CameraView2.CaptureImage(CancellationToken.None);
	}
}