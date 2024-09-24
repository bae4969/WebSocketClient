using System.Net.WebSockets;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using WebSocketClient.Classes;
using Newtonsoft.Json.Linq;
using RecvFuncType = System.Func<Newtonsoft.Json.Linq.JObject, System.Threading.Tasks.Task>;

namespace WebSocketClient.Pages;


public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	private async void OnTestClicked(object sender, EventArgs e)
	{
		RecvFuncType recv_func = async (recv_msg) =>
		{
			TestStr.Text = recv_msg.ToString();
		};
		var ret = await BaeWebSocketClient.Send(
			"wol",
			"execute",
			new JObject{
				{ "device_name", "Bae-DeskTop"},
			},
			recv_func
			);
		if (!ret)
		{
			TestStr.Text = "Fail to send";
		}
	}
}
