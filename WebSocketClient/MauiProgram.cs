using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.LifecycleEvents;
using OxyPlot.Maui.Skia;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using WebSocketClient.Pages.OnDeviceService;

#if WINDOWS
using Microsoft.UI; // WinUI 네임스페이스
using Microsoft.UI.Windowing; // 창 크기 설정을 위한 네임스페이스
using Windows.Graphics; // 창 크기 조정에 필요한 네임스페이스
#endif
namespace WebSocketClient
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseSkiaSharp(true)
				.UseOxyPlotSkia()
				.UseMauiCommunityToolkit()
				.UseMauiCommunityToolkitCore()
				.UseMauiCommunityToolkitCamera()
				.UseMauiCommunityToolkitMediaElement()
				.ConfigureFonts(fonts => {
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			builder.ConfigureLifecycleEvents(events =>
			{
#if WINDOWS
				events.AddWindows(windows =>
				{
					windows.OnWindowCreated(window =>
					{
						var mauiWindow = (MauiWinUIWindow)window;
						var appWindow = mauiWindow.AppWindow;

						if (appWindow == null) return;
						appWindow.Resize(new SizeInt32(440, 850));
					});
				});
#endif
			});
#if DEBUG
			builder.Logging.AddDebug();
#endif
			return builder.Build();
		}
	}
}