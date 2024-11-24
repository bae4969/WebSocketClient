#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
#elif WINDOWS
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Toolkit.Uwp.Notifications;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient.Classes
{
	static class Notification
	{
		private const string CHANNEL_ID = "default_channel";

		public static void Show(string title, string message)
		{
#if ANDROID
			var context = Android.App.Application.Context;

			// Android 8.0 이상에서 알림 채널 생성
			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
			{
				var channel = new NotificationChannel(CHANNEL_ID, "Default", NotificationImportance.Default)
				{
					Description = "Default notification channel"
				};

				var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
				notificationManager?.CreateNotificationChannel(channel);
			}

			// 알림 생성
			var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
				.SetContentTitle(title)
				.SetContentText(message)
				.SetSmallIcon(Resource.Drawable.app_icon) // 유효한 아이콘 설정
				.SetPriority(NotificationCompat.PriorityDefault)
				.SetAutoCancel(true);

			var notificationManagerCompat = NotificationManagerCompat.From(context);
			notificationManagerCompat.Notify(0, builder.Build());
#elif WINDOWS
			new ToastContentBuilder()
				.AddText(title)
				.AddText(message)
				.Show();
#endif
		}

	}
}
