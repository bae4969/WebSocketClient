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
			// 배터리 최적화 제외 시키기
			if (Build.VERSION.SdkInt < BuildVersionCodes.M) return;

			var context = Android.App.Application.Context;
			PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
			if (pm.IsIgnoringBatteryOptimizations(context.PackageName)) return;

			var intent = new Android.Content.Intent(Settings.ActionRequestIgnoreBatteryOptimizations);
			intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
			context.StartActivity(intent);
		}

		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}
}
