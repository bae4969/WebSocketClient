using WebSocketClient.Pages;

namespace WebSocketClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
			App.Current.UserAppTheme = AppTheme.Unspecified;
			MainPage = new NavigationPage(new LoginPage(true));
		}
    }
}
