using System.Collections.ObjectModel;

namespace WebSocketClient.Views;

public partial class CheckBoxListView : ContentView
{
	private ObservableCollection<CheckBoxListItem> Items
	{
		get => (ObservableCollection<CheckBoxListItem>)BindingContext;
		set => BindingContext = value;
	}

	public CheckBoxListView()
	{
		InitializeComponent();
	}

	// 선택된 항목들을 가져오는 메서드
	public List<string> GetSelectedItems()
	{
		return Items.Where(item => item.IsChecked)
					.Select(item => item.Name)
					.ToList();
	}

	public bool AddItem(string itemName, bool isChecked)
	{
		// 중복 항목 확인
		if (Items.Any(item => item.Name == itemName))
		{
			return false; // 이미 존재하는 항목이라면 false 반환
		}

		// 중복이 없다면 새로운 항목 추가
		Items.Add(new CheckBoxListItem { Name = itemName, IsChecked = isChecked });
		return true; // 성공적으로 추가되었을 때 true 반환
	}
	public bool RemoveItem(string itemName)
	{
		var itemToRemove = Items.FirstOrDefault(item => item.Name == itemName);
		if (itemToRemove != null)
		{
			Items.Remove(itemToRemove);
			return true; // 성공적으로 삭제되었을 때 true 반환
		}
		return false; // 해당 항목이 없을 경우 false 반환
	}
	public void Clear()
	{
		Items.Clear(); // 컬렉션을 비웁니다.
	}

	// 바인딩할 데이터 모델
	private class CheckBoxListItem
	{
		public string Name { get; set; }
		public bool IsChecked { get; set; }
	}
}
