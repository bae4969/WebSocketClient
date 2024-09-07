namespace WebSocketClient.Pages;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        // ����� ����� ID�� ��й�ȣ�� �ҷ���
        UserIdEntry.Text = Preferences.Get("user_id", "");
        PasswordEntry.Text = Preferences.Get("user_pw", "");
    }

    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // ID �Ǵ� PW�� ��� �ִ��� Ȯ��
        if (string.IsNullOrWhiteSpace(UserIdEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            StatusMessage.Text = "Please enter both ID and password.";
            StatusMessage.TextColor = new Color(200, 50, 50);
            StatusMessage.IsVisible = true;
            return;
        }

        // ���� ����
        Preferences.Set("user_id", UserIdEntry.Text);
        Preferences.Set("user_pw", PasswordEntry.Text);

        // ���� ���� �޽��� ǥ��
        StatusMessage.Text = "Settings saved successfully!";
        StatusMessage.TextColor = new Color(50, 200, 50);
        StatusMessage.IsVisible = true;
    }
}
