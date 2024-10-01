using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebSocketClient.Views;

public partial class TextListView : ContentView
{
	private readonly ObservableCollection<string> _items = [];
	private CancellationTokenSource _cancellationTokenSource;
	private EventHandler<TextChangedEventArgs>? _textChangeFunc { get; set; }

	public TextListView()
	{
		InitializeComponent();
		ListViewItems.ItemsSource = _items;
		SearchBar.TextChanged += async (sender, e) =>
		{
			if (_cancellationTokenSource != null)
				_cancellationTokenSource.Cancel();

			_cancellationTokenSource = new CancellationTokenSource();

			try
			{
				await Task.Delay(1000, _cancellationTokenSource.Token);
				if (_textChangeFunc != null)
				{
					_textChangeFunc(sender, e);
				}
				else
				{
					ListViewItems.ItemsSource =
						string.IsNullOrWhiteSpace(e.NewTextValue) ?
						_items :
						_items.Where(item => item.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
				}
			}
			catch { }
		};
	}

	public void SetSearchFunc(EventHandler<TextChangedEventArgs> textChangeFunc = null)
	{
		_textChangeFunc = textChangeFunc;
	}

	public void ClearItems()
	{
		_items.Clear();
	}
	public bool AddItem(string str, int index = -1)
	{
		if (_items.Contains(str))
			return false;

		if(index < 0)
			_items.Add(str);
		else
			_items.Insert(index, str);

		return true;
	}
	public bool RemoveItem(string str)
	{
		if (!_items.Contains(str))
			return false;

		_items.Remove(str);
		return true;
	}
	public void SetSelectedString(string str)
	{
		try
		{
			ListViewItems.SelectedItem = str;
			ListViewItems.ScrollTo(str, ScrollToPosition.MakeVisible, true);
		}
		catch { }
	}

	public string GetSelectedString()
	{
		return ListViewItems.SelectedItem.ToString();
	}
}