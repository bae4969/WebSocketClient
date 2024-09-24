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
	}
}
