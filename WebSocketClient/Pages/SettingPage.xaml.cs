namespace WebSocketClient.Pages;
using WebSocketClient.Classes;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
		await BaeWebSocketClient.Disconnect();
		Application.Current.MainPage = new NavigationPage(new LoginPage());
	}
}
