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

			// 스택이 maxStackDepth를 넘는 경우 앞의 페이지들을 제거
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

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

			// 스택이 maxStackDepth를 넘는 경우 앞의 페이지들을 제거
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

			// 스택이 maxStackDepth를 넘는 경우 앞의 페이지들을 제거
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}

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

			// 스택이 maxStackDepth를 넘는 경우 앞의 페이지들을 제거
			while (Navigation.NavigationStack.Count > _maxStackDepth)
				Navigation.RemovePage(Navigation.NavigationStack.First());
		}
	}
}
