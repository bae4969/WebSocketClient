using NetTopologySuite.Index.HPRtree;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WebSocketClient.Pages.SubPages;

public partial class SearchItemPage : ContentPage
{
	public ObservableCollection<string> TotalItems { get; set; } = new ObservableCollection<string>();
	public ObservableCollection<string> FilteredItems { get; set; } = new ObservableCollection<string>();

	private TaskCompletionSource<string> _taskCompletionSource;

	public SearchItemPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
	{
		var searchText = e.NewTextValue.ToLower();

		FilteredItems.Clear();
		if (searchText == "")
			foreach (var item in TotalItems)
				FilteredItems.Add(item);
		else
			foreach (var item in TotalItems.Where(str => str.ToLower().Contains(searchText)))
				FilteredItems.Add(item);
	}

	private void OnItemSelected(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is not string selectedItem) return;
		_taskCompletionSource?.SetResult(selectedItem);
		ResultsCollectionView.SelectedItem = null;
		Navigation.PopAsync();
	}


	public void SetItems(List<string> items)
	{
		TotalItems.Clear();
		FilteredItems.Clear();
		foreach (var item in items)
			TotalItems.Add(item);
		SearchBarObj.Text = "";
	}

	public Task<string> GetSelectionResult()
	{
		_taskCompletionSource = new TaskCompletionSource<string>();
		return _taskCompletionSource.Task;
	}
}