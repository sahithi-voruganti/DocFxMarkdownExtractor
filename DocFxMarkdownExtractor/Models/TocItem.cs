using System.Collections.Generic;

public class TocItem
{
    private List<TocItem> _items;

    public TocItem()
    {
    }
    public string name { get; set; }

    public string href { get; set; }
    public string topichref { get; set; }
    public IReadOnlyCollection<TocItem> items => _items?.AsReadOnly();
    public void AddItem(TocItem item)
    {
        if (_items == null) { _items = new List<TocItem>(); }
        _items.Add(item);
    }
}