using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WebSocketClient.Classes;
using WebSocketClient.Views;

namespace WebSocketClient.Pages;

// µ•¿Ã≈Õ ∏µ®
public class QueryInfoType
{
	public string table_type { get; set; }
	public string stock_code { get; set; }
	public string query_type { get; set; }
	public string stock_name { get; set; }
	public string stock_market { get; set; }
	public string stock_type { get; set; }
}


public partial class StockCollectionManagerPage : ContentPage
{
	private Dictionary<string, QueryInfoType> _searchList { get; set; } = [];
	private Dictionary<string, QueryInfoType> _addedList { get; set; } = [];

	private string _regionFilter = "";
	private string _typeFilter = "";



	private void UpdateShowListWithAddedData()
	{
		ItemListView.ClearItems();
		foreach (var item in _addedList)
		{
			if (_regionFilter == "KR" &&
				item.Value.stock_market != "KOSPI" &&
				item.Value.stock_market != "KOSDAQ" &&
				item.Value.stock_market != "KONEX"
				)
				continue;
			else if (_regionFilter == "US" &&
				item.Value.stock_market != "NYSE" &&
				item.Value.stock_market != "NASDAQ" &&
				item.Value.stock_market != "AMEX"
				)
				continue;
			else if (_regionFilter == "COIN" &&
				item.Value.stock_market != "COIN"
				)
				continue;

			if (_typeFilter == "STOCK" && item.Value.stock_type != "STOCK")
				continue;
			else if (_typeFilter == "ETF" && item.Value.stock_type != "ETF")
				continue;
			else if (_typeFilter == "ETN" && item.Value.stock_type != "ETN")
				continue;

			ItemListView.AddItem(
				item.Key,
				$"[{item.Value.stock_code}] {item.Value.stock_name}",
				true
				);
		}
	}
	private void UpdateShowListWithSearchData()
	{
		ItemListView.ClearItems();
		foreach (var item in _searchList)
		{
			if (_regionFilter == "KR" &&
				item.Value.stock_market != "KOSPI" &&
				item.Value.stock_market != "KOSDAQ" &&
				item.Value.stock_market != "KONEX"
				)
				continue;
			else if (_regionFilter == "US" &&
				item.Value.stock_market != "NYSE" &&
				item.Value.stock_market != "NASDAQ" &&
				item.Value.stock_market != "AMEX"
				)
				continue;
			else if (_regionFilter == "COIN" &&
				item.Value.stock_market != "COIN"
				)
				continue;

			if (_typeFilter == "STOCK" && item.Value.stock_type != "STOCK")
				continue;
			else if (_typeFilter == "ETF" && item.Value.stock_type != "ETF")
				continue;
			else if (_typeFilter == "ETN" && item.Value.stock_type != "ETN")
				continue;

			ItemListView.AddItem(
				item.Key,
				$"[{item.Value.stock_code}] {item.Value.stock_name}",
				_addedList.ContainsKey(item.Value.stock_code)
				);
		}
	}
	private void UpdateAddedListFromSearchData()
	{
		var t_items = ItemListView.GetKeyWordAndSelection();
		foreach (var t_item in t_items)
		{
			if (t_item.Item2 &&
				!_addedList.ContainsKey(t_item.Item1) &&
				_searchList.ContainsKey(t_item.Item1))
			{
				_addedList.Add(t_item.Item1, _searchList[t_item.Item1]);
			}
			else if (!t_item.Item2 &&
				_addedList.ContainsKey(t_item.Item1))
			{
				_addedList.Remove(t_item.Item1);
			}
		}
	}


	public StockCollectionManagerPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		RegionFilter.SelectedIndex = 0;
		TypeFilter.SelectedIndex = 0;

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

					_addedList.Clear();
					recv_msg["data"]["list"].ToList().ForEach(x =>
					{
						QueryInfoType t_info = new()
						{
							table_type = x[0].ToString(),
							stock_code = x[1].ToString(),
							query_type = x[2].ToString(),
							stock_name = x[3].ToString(),
							stock_market = x[4].ToString(),
							stock_type = x[5].ToString(),
						};
						_addedList.Add($"{t_info.stock_market}_{t_info.stock_code}", t_info);
					});
					UpdateShowListWithAddedData();
				}
				);
			if (!ret)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
			}
		}

		ItemListView.SetSearchFunc(async (obj, e) =>
		{
			UpdateAddedListFromSearchData();


			if (e.NewTextValue == null || e.NewTextValue.Length < 2)
			{
				UpdateShowListWithAddedData();
				return;
			}
			else
			{
				var ret = await BaeWebSocketClient.Send(
					"stm",
					"search_tot_list",
					new JObject()
					{
					{ "search_keyword", e.NewTextValue },
					{ "stock_region", _regionFilter },
					{ "stock_type", _typeFilter },
					},
					async (recv_msg) =>
					{
						if (recv_msg["result"].Value<int>() != 200)
						{
							await Application.Current.MainPage.DisplayAlert("Error", $"Fail to recv {recv_msg["msg"].ToString()}", "OK");
							return;
						}

						_searchList.Clear();
						recv_msg["data"]["list"].ToList().ForEach(x =>
						{
							QueryInfoType t_info = new()
							{
								table_type = x[0].ToString(),
								stock_code = x[1].ToString(),
								query_type = "EX",
								stock_name = x[2].ToString(),
								stock_market = x[3].ToString(),
								stock_type = x[4].ToString(),
							};
							_searchList.Add($"{t_info.stock_market}_{t_info.stock_code}", t_info);
						});
						UpdateShowListWithSearchData();
					}
					);
				if (!ret)
				{
					await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
				}
			}
		});
	}

	private void OnRegionFilterSelectedIndexChanged(object sender, EventArgs e)
	{
		switch (RegionFilter.SelectedItem.ToString())
		{
			case "Korea":
				_regionFilter = "KR";
				TypeFilter.IsVisible = true;
				break;
			case "Us":
				_regionFilter = "US";
				TypeFilter.IsVisible = true;
				break;
			case "Coin":
				_regionFilter = "COIN";
				_typeFilter = "";
				TypeFilter.IsVisible = false;
				break;
			default:
				_regionFilter = "";
				TypeFilter.IsVisible = true;
				break;
		}
		ItemListView.Reload();
	}
	private void OnTypeFilterSelectedIndexChanged(object sender, EventArgs e)
	{
		switch (TypeFilter.SelectedItem.ToString())
		{
			case "Stock":
				_typeFilter = "STOCK";
				break;
			case "ETF":
				_typeFilter = "ETF";
				break;
			case "ETN":
				_typeFilter = "ETN";
				break;
			default:
				_typeFilter = "";
				break;
		}
		ItemListView.Reload();
	}




	private async void OnExecuteClicked(object sender, EventArgs e)
	{
		UpdateAddedListFromSearchData();

		List<List<string>> dd = new List<List<string>>();

		JArray query_list = new();
		foreach (var item in _addedList)
			query_list.Add(new JArray { item.Value.table_type, item.Value.stock_code, item.Value.query_type });

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