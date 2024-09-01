using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using WebSocketClient.Classes;

namespace WebSocketClient.Pages;


public partial class MainPage : ContentPage
{
    private ClientWebSocket _ws = null;

	public MainPage()
	{
		InitializeComponent();
	}

    private void OnTestClicked(object sender, EventArgs e)
    {
        string user_id = Preferences.Get("user_id", "");
        string user_pw = Preferences.Get("user_pw", "");

        var task_conn = BaeWebSocketClient.Connect(user_id, user_pw);
        if (task_conn.Wait(1000))
        {
            var ret = task_conn.Result;

            Console.WriteLine(ret.ToString());
        }
        else
        {
            BaeWebSocketClient.Close().Wait();
        }
    }
}
