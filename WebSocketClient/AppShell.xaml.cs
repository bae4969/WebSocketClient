using WebSocketClient.Classes;
using WebSocketClient.Pages;

namespace WebSocketClient
{

	public partial class AppShell : Shell
	{
		private string _userId { get; set; }
		private const int _maxStackDepth = 3;

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

		private async void OnLogoutButtonClicked(object sender, EventArgs e)
		{
			await BaeWebSocketClient.Disconnect();
		}

		private async void OnSettingPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is SettingPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new SettingPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		/************************************************************************************************************/

		private async void OnLevelerPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is LevelerPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new LevelerPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		private async void OnLocationAlertPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is LocationAlertPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new LocationAlertPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		/************************************************************************************************************/

		private async void OnStockChartPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is StockChartPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new StockChartPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		private async void OnStockCollectionManagerPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is StockCollectionManagerPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new StockCollectionManagerPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

		/************************************************************************************************************/

		private async void OnWolPageButtonClicked(object sender, EventArgs e)
		{
			var existingPage = Navigation.NavigationStack.FirstOrDefault(page => page is WolPage);
			if (existingPage != null)
			{
				Navigation.RemovePage(existingPage);
				await Navigation.PushAsync(existingPage);
			}
			else
				await Navigation.PushAsync(new WolPage());

			Shell.Current.FlyoutIsPresented = false;
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}
	}
}
