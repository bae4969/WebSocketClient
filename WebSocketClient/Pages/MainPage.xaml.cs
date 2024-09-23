using System.Net.WebSockets;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using WebSocketClient.Classes;
using Newtonsoft.Json.Linq;

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
		var ret = await BaeWebSocketClient.RequestService(
			"req",
			"wol",
			"execute",
			new JObject{
				{ "device_name", "Bae-DeskTop"},
			}
			);
		TestStr.Text = ret.ToString();

  //      TestStr.Text = "";

		//      var delayTask = Task.Delay(1000);

		//      var task_conn = BaeWebSocketClient.Connect();
		//      var completedTask = await Task.WhenAny(task_conn, delayTask);
		//      if (completedTask == delayTask)
		//      {
		//          BaeWebSocketClient.Close().Wait();
		//          TestStr.Text = "Fail to auth";
		//          return;
		//      }

		//var ret = await task_conn;
		//      if (ret["result"].Value<int>() != 200)
		//      {
		//          TestStr.Text = "Fail to auth";
		//          return;
		//      }

		//var data = new JObject
		//{
		//    { "device_name", "Bae-DeskTop" },
		//};

		//delayTask = Task.Delay(1000);
		//var type_list = BaeWebSocketClient.RequestService("wol", "wol_device", data);
		//completedTask = await Task.WhenAny(type_list, delayTask);
		//if (completedTask == delayTask)
		//{
		//    BaeWebSocketClient.Close().Wait();
		//    TestStr.Text = "Fail to get list";
		//    return;
		//}

		//ret = await type_list;
		//if (ret["result"].Value<int>() != 200)
		//{
		//    TestStr.Text = "Fail to get list";
		//    return;
		//}


		//TestStr.Text = ret.ToString();
	}
}
