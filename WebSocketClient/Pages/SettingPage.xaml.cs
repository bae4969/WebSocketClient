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
        // 저장된 사용자 ID와 비밀번호를 불러옴
        UserIdEntry.Text = Preferences.Get("user_id", "");
        PasswordEntry.Text = Preferences.Get("user_pw", "");
    }

    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // ID 또는 PW가 비어 있는지 확인
        if (string.IsNullOrWhiteSpace(UserIdEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            StatusMessage.Text = "Please enter both ID and password.";
            StatusMessage.TextColor = new Color(200, 50, 50);
            StatusMessage.IsVisible = true;
            return;
        }

        // 설정 저장
        Preferences.Set("user_id", UserIdEntry.Text);
        Preferences.Set("user_pw", PasswordEntry.Text);

        // 저장 성공 메시지 표시
        StatusMessage.Text = "Settings saved successfully!";
        StatusMessage.TextColor = new Color(50, 200, 50);
        StatusMessage.IsVisible = true;
    }
}
