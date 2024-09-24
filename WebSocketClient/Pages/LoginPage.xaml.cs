namespace WebSocketClient.Pages;
using WebSocketClient.Classes;

public partial class LoginPage : ContentPage
{
	bool is_activate_auto_login = false;


	public LoginPage(bool activate_auto_login = false)
	{
		InitializeComponent();
		is_activate_auto_login = activate_auto_login;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UserIdEntry.Text = Preferences.Get("user_id", "");
		PasswordEntry.Text = Preferences.Get("user_pw", "");
		AutoLoginCheckBox.IsChecked = Preferences.Get("is_auto_login", false);
		if (AutoLoginCheckBox.IsChecked && is_activate_auto_login)
			OnLoginButtonClicked(LoginButton, new EventArgs());
	}

	private async void OnLoginButtonClicked(object sender, EventArgs e)
	{
		// ID 또는 PW가 비어 있는지 확인
		if (string.IsNullOrWhiteSpace(UserIdEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
		{
			await Application.Current.MainPage.DisplayAlert("Error", "Please enter both ID and password.", "OK");
			return;
		}

		var ret = await BaeWebSocketClient.Connect(UserIdEntry.Text, PasswordEntry.Text, AutoLoginCheckBox.IsChecked);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send", "OK");
		}
	}
}