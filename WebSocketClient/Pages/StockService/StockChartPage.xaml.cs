using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Globalization;
using WebSocketClient.Classes;
using WebSocketClient.Views;
using WebSocketClient.Pages.SubPages;


namespace WebSocketClient.Pages.StockService;


public partial class StockChartPage : ContentPage
{
	private Dictionary<string, QueryInfoType> _queryItems { get; set; } = [];
	private QueryInfoType _lastSelected = null;


	public StockChartPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (_queryItems.Count > 0) return;
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

				_queryItems.Clear();
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
					_queryItems.Add(t_info.ToKeyString(), t_info);
				});
			}
			);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
		}
	}

	private async void SearchBar_Focused(object sender, FocusEventArgs e)
	{
		SearchItemPage searchItemPage = new SearchItemPage();
		searchItemPage.SetItems(_queryItems.Keys.ToList());
		await Navigation.PushAsync(searchItemPage);
		string selected_str = await searchItemPage.GetSelectionResult();

		QueryInfoType queryInfo = _lastSelected;
		if (selected_str == null ||
			selected_str == _lastSelected?.ToKeyString() ||
			!_queryItems.TryGetValue(selected_str, out queryInfo))
		{
			SearchBarObj.Text = _lastSelected?.ToKeyString();
			return;
		}

		_lastSelected = queryInfo;
		SearchBarObj.Text = queryInfo?.ToKeyString();

		var ret = await BaeWebSocketClient.Send(
			"stm",
			"get_candle_data",
			new JObject()
			{
				{ "table_type", _lastSelected.table_type },
				{ "target_code", _lastSelected.stock_code },
				{ "year", 2025 },
				{ "week_from", 1 },
				{ "week_to", 53 },
			},
			async (recv_msg) =>
			{
				if (recv_msg["result"].Value<int>() != 200)
				{
					await Application.Current.MainPage.DisplayAlert("Error", $"Fail to recv {recv_msg["msg"].ToString()}", "OK");
					return;
				}

				PlotModel chartPlot = new PlotModel();

				// Äµµé½ºÆ½ ½Ã¸®Áî Ãß°¡
				var series = new CandleStickSeries
				{
					Color = OxyColors.Black,
					IncreasingColor = OxyColors.Red,
					DecreasingColor = OxyColors.Blue,
				};

				recv_msg["data"]["candle"].ToList().ForEach(x =>
				{
					series.Items.Add(new HighLowItem(
						DateTimeAxis.ToDouble(DateTime.ParseExact(x[0].ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)),
						x[4].Value<double>(),
						x[3].Value<double>(),
						x[1].Value<double>(),
						x[2].Value<double>()
						));
				});

				chartPlot.Series.Add(series);
				chartPlot.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "MM/dd" });
				chartPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

				ChartPlot.Model = chartPlot;
			}
			);
		if (!ret)
		{
			await Application.Current.MainPage.DisplayAlert("Error", $"Fail to send msg", "OK");
		}
	}
}