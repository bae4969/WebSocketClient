using WebSocketClient.Classes;
using WebSocketClient.Pages;

namespace WebSocketClient
{

	public partial class AppShell : Shell
	{
		public string UserId { get; set; }

		public AppShell()
        {
            InitializeComponent();
			UserId =  Preferences.Get("user_id", "");
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
			await Navigation.PushAsync(new SettingPage());
		}

		private async void OnStockChartPageButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new StockChartPage());
		}

		private async void OnStockCollectionManagerPageButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new StockCollectionManagerPage());
		}

		private async void OnWolPageButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new WolPage());
		}
	}
}
