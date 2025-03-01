
namespace WebSocketClient.Pages.OnDeviceService;

public partial class StereoCameraPage : ContentPage
{
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