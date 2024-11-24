using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.ComponentModel;
using WebSocketClient.Classes;
using WebSocketClient.Pages;

namespace WebSocketClient
{

	public partial class AppShell : Shell
	{
		private string _userId { get; set; }
		private const int _maxStackDepth = 3;

		protected static void CloseApp()
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				var isClose = await Application.Current.MainPage.DisplayAlert("Alert", "Close App?", "Yes", "No");
				if (!isClose) return;
#if ANDROID
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
				Environment.Exit(0);
#endif
			});
		}
		protected async void NavigateToPage<TPage>() where TPage : Page, new()
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is TPage);

			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
			{
				await Navigation.PushAsync(new TPage());
			}

			Shell.Current.FlyoutIsPresented = false;

			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		/************************************************************************************************************/

		public AppShell()
		{
			InitializeComponent();
			UserNameLabel.Text = _userId =  Preferences.Get("user_id", "");
			this.BindingContext = this;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		protected override bool OnBackButtonPressed()
		{
			if (Navigation.NavigationStack.Count == 0)
			{
				AppShell.CloseApp();
				return true;
			}
			else
				return base.OnBackButtonPressed();
		}

		private async void OnLogoutButtonClicked(object sender, EventArgs e)
		{
			await BaeWebSocketClient.Disconnect();
		}

		private void OnExitButtonClicked(object sender, EventArgs e)
		{
			CloseApp();
		}

		private void OnNavigateButtonClicked(object sender, EventArgs e)
		{
			if (sender is not Button btn) return;

			switch (btn.StyleId)
			{
				case "SettingPageButton":
					NavigateToPage<SettingPage>();
					break;

				case "LevelerPageButton":
					NavigateToPage<LevelerPage>();
					break;

				case "LocationAlertPageButton":
					NavigateToPage<LocationAlertPage>();
					break;

				case "StockChartPageButton":
					NavigateToPage<StockChartPage>();
					break;

				case "StockCollectionManagerPageButton":
					NavigateToPage<StockCollectionManagerPage>();
					break;

				case "WolPageButton":
					NavigateToPage<WolPage>();
					break;

				default:
					return; // 정의되지 않은 버튼은 처리하지 않음
			}
		}
	}
}
