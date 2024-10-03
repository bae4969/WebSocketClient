using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WebSocketClient.Classes;
using WebSocketClient.Views;
using RecvFuncType = System.Func<Newtonsoft.Json.Linq.JObject, System.Threading.Tasks.Task>;

namespace WebSocketClient.Pages;

// 데이터 모델
public class QueryInfoType
{
	public string stock_name { get; set; }
	public string stock_code { get; set; }
	public string stock_market { get; set; }
	public string table_type { get; set; }
	public string query_type { get; set; }
}


public partial class StockCollectionManagerPage : ContentPage
{
	private List<QueryInfoType> SearchDatas { get; set; } = [];
	private List<QueryInfoType> AddedDatas { get; set; } = [];

	private string ItemRegion = "";
	private string ItemType = "";



	public StockCollectionManagerPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		{
			var ret = await BaeWebSocketClient.Send(
				"stm",
				"get_regi_list",
				new JObject(),
				async (recv_msg) =>
				{
					if (recv_msg["result"].Value<int>() != 200)
					{
						await Application.Current.MainPage.DisplayAlert("Error", $"Fail to recv {recv_msg["msg"].ToString()}", "OK");
						return;
					}

					AddedDatas.Clear();
					recv_msg["data"]["list"].ToList().ForEach(x =>
					{
						QueryInfoType t_info = new()
						{
							table_type = x[0].ToString(),
							stock_code = x[1].ToString(),
							query_type = x[2].ToString(),
							stock_name = x[3].ToString(),
							stock_market = x[4].ToString(),
						};
						AddedDatas.Add(t_info);
					});
					AddedDatas.Sort((x, y) => x.stock_name.CompareTo(y.stock_name));

					AddedList.ClearItems();
					foreach (var t_info in AddedDatas)
					{
						AddedList.AddItem(t_info.stock_name);
					}
				}
				);
			if (!ret)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
			}
		}

		SearchList.SetSearchFunc(async (obj, e) =>
		{
			if (e.NewTextValue == null || e.NewTextValue.Length < 2)
			{
				SearchDatas.Clear();
				SearchList.ClearItems();
				return;
			}

			var ret = await BaeWebSocketClient.Send(
				"stm",
				"search_tot_list",
				new JObject()
				{
					{ "search_keyword", e.NewTextValue },
					{ "stock_region", ItemRegion },
					{ "stock_type", ItemType },
				},
				async (recv_msg) =>
				{
					if (recv_msg["result"].Value<int>() != 200)
					{
						await Application.Current.MainPage.DisplayAlert("Error", $"Fail to recv {recv_msg["msg"].ToString()}", "OK");
						return;
					}

					SearchDatas.Clear();
					recv_msg["data"]["list"].ToList().ForEach(x =>
					{
						QueryInfoType t_info = new()
						{
							table_type = x[0].ToString(),
							stock_code = x[1].ToString(),
							query_type = "EX",
							stock_name = x[2].ToString(),
							stock_market = x[3].ToString(),
						};
						SearchDatas.Add(t_info);
					});
					SearchDatas.Sort((x, y) => x.stock_name.CompareTo(y.stock_name));

					SearchList.ClearItems();
					foreach (var t_info in SearchDatas)
					{
						SearchList.AddItem(t_info.stock_name);
					}
				}
				);
			if (!ret)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
			}
		});
	}


	private async void OnFilterClicked(object sender, EventArgs e)
	{
		var btn = sender as Button;
		if (btn == RegionKrButton ||
			btn == RegionUsButton ||
			btn == RegionCoinButton)
		{
			RegionKrButton.BackgroundColor =
				RegionUsButton.BackgroundColor =
				RegionCoinButton.BackgroundColor =
				Color.FromRgb(0xAC, 0x99, 0xEA);

			if (btn == RegionKrButton)
			{
				if (ItemRegion == "KR")
					ItemRegion = "";
				else
				{
					ItemRegion = "KR";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
			else if (btn == RegionUsButton)
			{
				if (ItemRegion == "US")
					ItemRegion = "";
				else
				{
					ItemRegion = "US";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
			else if (btn == RegionCoinButton)
			{
				if (ItemRegion == "COIN")
					ItemRegion = "";
				else
				{
					ItemRegion = "COIN";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
		}
		else if (
			btn == TypeStockButton ||
			btn == TypeEtfButton ||
			btn == TypeEtnButton)
		{
			TypeStockButton.BackgroundColor =
				TypeEtfButton.BackgroundColor =
				TypeEtnButton.BackgroundColor =
				Color.FromRgb(0xAC, 0x99, 0xEA);

			if (btn == TypeStockButton)
			{
				if (ItemType == "STOCK")
					ItemType = "";
				else
				{
					ItemType = "STOCK";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
			else if (btn == TypeEtfButton)
			{
				if (ItemType == "ETF")
					ItemType = "";
				else
				{
					ItemType = "ETF";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
			else if (btn == TypeEtnButton)
			{
				if (ItemType == "ETN")
					ItemType = "";
				else
				{
					ItemType = "ETN";
					btn.BackgroundColor = Color.FromRgb(0x80, 0x70, 0xB2);
				}
			}
		}

		SearchList.Reload();

	}

	private async void OnAddClicked(object sender, EventArgs e)
	{
		try
		{
			string str = SearchList.GetSelectedString();
			if (str == null) return;

			var search_ret = SearchDatas.FirstOrDefault(q => q.stock_name.Equals(str, StringComparison.OrdinalIgnoreCase));
			if (search_ret == null) return;

			var added_ret = AddedDatas.FirstOrDefault(q => q.stock_name.Equals(str, StringComparison.OrdinalIgnoreCase));
			if (added_ret != null) return;


			var t_list = AddedDatas.Select(s => s.stock_name).ToList();
			int index = t_list.BinarySearch(search_ret.stock_name);
			if (index < 0)
			{
				index = ~index; // 삽입할 위치 계산
			}


			AddedDatas.Add(search_ret);
			AddedList.AddItem(search_ret.stock_name, index);
			AddedList.SetSelectedString(search_ret.stock_name);
		}
		catch { }
	}
	private async void OnDelClicked(object sender, EventArgs e)
	{
		try
		{
			string str = AddedList.GetSelectedString();
			if (str == null) return;

			var added_ret = AddedDatas.FirstOrDefault(q => q.stock_name.Equals(str, StringComparison.OrdinalIgnoreCase));
			if (added_ret == null) return;


			AddedDatas.Remove(added_ret);
			AddedList.RemoveItem(added_ret.stock_name);
		}
		catch { }
	}


	private async void OnExecuteClicked(object sender, EventArgs e)
	{
		List<List<string>> dd = new List<List<string>>();

		JArray query_list = new();
		foreach (var item in AddedDatas)
			query_list.Add(new JArray { item.table_type, item.stock_code, item.query_type });

		var ret = await BaeWebSocketClient.Send(
			"stm",
			"update_regi_list",
			new JObject{
				{ "list", query_list },
			},
			async (recv_msg) =>
			{
				if (recv_msg["result"].Value<int>() != 200)
				{
					await Application.Current.MainPage.DisplayAlert("Error", $"Fail to recv {recv_msg["msg"].ToString()}", "OK");
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Info", $"Success", "OK");
				}
			}
			);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
		}
	}
}