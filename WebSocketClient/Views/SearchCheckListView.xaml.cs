using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace WebSocketClient.Views;


public class StringCheckType : INotifyPropertyChanged
{
	public string KeyWord { get; set; } = "";

	private string _value;
	public string Value
	{
		get => _value;
		set
		{
			if (_value != value)
			{
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}
	}

	private bool _isChecked;
	public bool IsChecked
	{
		get => _isChecked;
		set
		{
			if (_isChecked != value)
			{
				_isChecked = value;
				OnPropertyChanged(nameof(IsChecked));
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;
	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

public partial class SearchCheckListView : ContentView
{
	public ObservableCollection<StringCheckType> _items { get; set; } = [];
	private CancellationTokenSource _cancellationTokenSource;
	private EventHandler<TextChangedEventArgs>? _textChangeFunc { get; set; }


	public SearchCheckListView()
	{
		InitializeComponent();
		ListViewItems.ItemsSource = _items;
		this.BindingContext = this;
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
					if (string.IsNullOrWhiteSpace(e.NewTextValue))
					{
						ListViewItems.ItemsSource = _items;
						NumOfItmes.Text = _items.Count.ToString();
					}
					else
					{
						var t_list = _items.Where(item => item.Value.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
						ListViewItems.ItemsSource = t_list;
						NumOfItmes.Text = t_list.Count.ToString();
					}
				}
			}
			catch { }
		};
		ListViewItems.ItemTapped+= (sender, e) =>
		{
			var item = (StringCheckType)e.Item;
			item.IsChecked = !item.IsChecked;
		};
	}

	public void SetSearchFunc(EventHandler<TextChangedEventArgs> textChangeFunc = null)
	{
		_textChangeFunc = textChangeFunc;
	}

	public void Reload()
	{
		var t_str = SearchBar.Text;
		SearchBar.Text = "";
		SearchBar.Text = t_str;
	}
	public bool IsDuplicate(string str)
	{
		var keyWordList = _items.Select(item => item.KeyWord).ToList();
		return keyWordList.Contains(str);
	}
	public int GetTargetIndex(string keyWord)
	{
		var keyWordList = _items.Select(item => item.KeyWord).ToList();
		if (keyWordList.Contains(keyWord))
			return -1;

		int index = keyWordList.BinarySearch(keyWord);
		if (index < 0)
			index = ~index;

		return index;
	}

	public void ClearItems()
	{
		_items.Clear();
		NumOfItmes.Text = _items.Count.ToString();
	}
	public bool AddItem(string keyWord, string showStr, bool isChcekd)
	{
		int idx = GetTargetIndex(keyWord);
		if (idx < 0) return false;

		_items.Insert(idx, new StringCheckType { KeyWord = keyWord, Value = showStr, IsChecked = isChcekd });
		NumOfItmes.Text = _items.Count.ToString();

		return true;
	}
	public bool RemoveItem(string keyWord)
	{
		for(int idx = _items.Count - 1; idx >= 0; idx--)
		{
			if (_items[idx].KeyWord == keyWord)
				_items.RemoveAt(idx);
		}
		NumOfItmes.Text = _items.Count.ToString();

		return true;
	}

	public List<Tuple<string, bool>> GetKeyWordAndSelection()
	{
		return _items.Select(item => new Tuple<string, bool>(item.KeyWord, item.IsChecked)).ToList();
	}
}