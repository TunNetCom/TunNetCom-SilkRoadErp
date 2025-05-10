namespace BlazorProTemplate.Components.Layout.Sidebar;

public record Suffix(string Title, string Class);



public class MenuItem(string? href = null, string? title = null, string? icon = null, Suffix? suffix = null, List<MenuItem>? childMenuItems = null)
{
    private const int _itemHeight = 50;

    public string? Href { get; set; } = href;
    public string? Title { get; set; } = title;
    public string? Icon { get; set; } = icon;
    public Suffix? Suffix { get; set; } = suffix;
    public List<MenuItem>? ChildMenuItems { get; set; } = childMenuItems;

    private bool _isOpened = false;
    public bool IsOpened
    {
        get => _isOpened;
        set
        {
            if (!HasChildMenuItems) return;

            ChildMenuItems!.ForEach(x => { x.IsActive = value; if (value == false) x.IsOpened = false; });

            _isOpened = value;
        }
    }

    public bool IsActive { get; set; } = false;
    public bool HasChildMenuItems => ChildMenuItems != null && ChildMenuItems.Count != 0;

    public int CalcSubMenuHeight()
    {
        if (ChildMenuItems == null || !IsOpened)
            return 0;

        int sumHeight = 0;
        foreach (var childMenuItem in ChildMenuItems)
        {
            sumHeight += childMenuItem.CalcSubMenuHeight();
        }

        return sumHeight + ChildMenuItems.Count * _itemHeight;
    }
}