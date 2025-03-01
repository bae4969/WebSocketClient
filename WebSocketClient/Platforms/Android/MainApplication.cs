using Android.App;
using Android.OS;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Microsoft.Maui.Controls.PlatformConfiguration;
using static Microsoft.Maui.ApplicationModel.Platform;

namespace WebSocketClient
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
		}

		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}
}
