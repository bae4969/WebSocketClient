namespace WebSocketClient.Pages;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
        Entry_ID.Text = Preferences.Get("user_id", "");
        Entry_PW.Text = Preferences.Get("user_pw", "");
    }

    private void OnSettingSaveClicked(object sender, EventArgs e)
    {
        this.Focus();
        Preferences.Set("user_id", Entry_ID.Text);
        Preferences.Set("user_pw", Entry_PW.Text);
    }
}
