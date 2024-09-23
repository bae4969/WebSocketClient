using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

			// Windows의 초기 화면 크기 설정
			builder.ConfigureLifecycleEvents(events =>
			{
#if WINDOWS
            events.AddWindows(windows =>
            {
                windows.OnWindowCreated(window =>
                {
                    var mauiWindow = (MauiWinUIWindow)window;
                    var appWindow = mauiWindow.AppWindow;

                    if (appWindow != null)
                    {
                        // 원하는 초기 크기를 설정 (폭, 높이)
                        appWindow.Resize(new SizeInt32(600, 800));
                    }
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
