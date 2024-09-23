using WebSocketClient.Pages;

namespace WebSocketClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

			MainPage = new NavigationPage(new LoginPage(true));
		}
    }
}
