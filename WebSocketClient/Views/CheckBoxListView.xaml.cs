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

	// ���õ� �׸���� �������� �޼���
	public List<string> GetSelectedItems()
	{
		return Items.Where(item => item.IsChecked)
					.Select(item => item.Name)
					.ToList();
	}

	public bool AddItem(string itemName, bool isChecked)
	{
		// �ߺ� �׸� Ȯ��
		if (Items.Any(item => item.Name == itemName))
		{
			return false; // �̹� �����ϴ� �׸��̶�� false ��ȯ
		}

		// �ߺ��� ���ٸ� ���ο� �׸� �߰�
		Items.Add(new CheckBoxListItem { Name = itemName, IsChecked = isChecked });
		return true; // ���������� �߰��Ǿ��� �� true ��ȯ
	}
	public bool RemoveItem(string itemName)
	{
		var itemToRemove = Items.FirstOrDefault(item => item.Name == itemName);
		if (itemToRemove != null)
		{
			Items.Remove(itemToRemove);
			return true; // ���������� �����Ǿ��� �� true ��ȯ
		}
		return false; // �ش� �׸��� ���� ��� false ��ȯ
	}
	public void Clear()
	{
		Items.Clear(); // �÷����� ���ϴ�.
	}

	// ���ε��� ������ ��
	private class CheckBoxListItem
	{
		public string Name { get; set; }
		public bool IsChecked { get; set; }
	}
}
