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

	protected override async void OnAppearing()
	{
		base.OnAppearing();
	}

}
