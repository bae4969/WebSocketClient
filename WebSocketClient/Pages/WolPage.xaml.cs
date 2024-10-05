using Newtonsoft.Json.Linq;
using WebSocketClient.Classes;
using RecvFuncType = System.Func<Newtonsoft.Json.Linq.JObject, System.Threading.Tasks.Task>;

namespace WebSocketClient.Pages;

public partial class WolPage : ContentPage
{
	public WolPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var ret = await BaeWebSocketClient.Send(
			"wol",
			"list",
			new JObject(),
			async (recv_msg) =>
			{
				if (recv_msg["result"].Value<int>() != 200)
				{
					await Application.Current.MainPage.DisplayAlert("Error", $"Fail to load list {recv_msg["msg"].ToString()}", "OK");
					return;
				}

				WolDeviceListPicker.Items.Clear();
				recv_msg["data"]["list"].ToList().ForEach(x =>
				{
					WolDeviceListPicker.Items.Add(x.ToString());
				});
				if (WolDeviceListPicker.Items.Count > 0)
					WolDeviceListPicker.SelectedIndex = 0;
			}
			);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send for getting device list msg", "OK");
		}
	}

	private async void OnWolExecuteClicked(object sender, EventArgs e)
	{
		if (WolDeviceListPicker.SelectedIndex < 0) return;

		var ret = await BaeWebSocketClient.Send(
			"wol",
			"execute",
			new JObject{
				{ "device_name", WolDeviceListPicker.SelectedItem.ToString()},
			},
			async (recv_msg) =>
			{
				if (recv_msg["result"].Value<int>() == 200)
				{
					await Application.Current.MainPage.DisplayAlert("Info", $"success", "OK");
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Error", $"Fail to execute wol {recv_msg["msg"].ToString()}", "OK");
				}
			}
			);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send for executing msg", "OK");
		}
	}


}