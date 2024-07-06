using Microsoft.AspNetCore.Components;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// Form input for dictionaries of simple types.
/// Adding/editing items is done in the table.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public partial class InputPrimitiveDictionaryTemplate<TKey, TValue>
{
    /// <summary>
    /// The dictionary to generate an input field for.
    /// </summary>
    [Parameter]
    public Dictionary<TKey, TValue> Items
    {
        get => Items;
        set
        {
            _items = value;
            _wrappedItems = value?.Select(item => new ItemWrapper<TKey, TValue>() { Key = item.Key, Value = item.Value }).ToList();
        }
    }

    /// <summary>
    /// Title of this input field (optional)
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Max amount of items per page.
    /// </summary>
    /// <remarks>Default = 5.</remarks>
    [Parameter]
    public int MaxAmount { get; set; } = 5;

    private Dictionary<TKey, TValue> _items { get; set; }

    /// <summary>
    /// Internal list used to generate fields. Primitive types are passed by value and this causes issues.
    /// </summary>
    private List<ItemWrapper<TKey, TValue>> _wrappedItems { get; set; }

    /// <summary>
    /// This method is called whenever a new item needs to be added to the list.
    /// </summary>
    /// <returns>The new item to add to the list.</returns>
    private Task<ItemWrapper<TKey, TValue>> AddItem()
    {
        return Task.FromResult(new ItemWrapper<TKey, TValue>());
    }

    /// <summary>
    /// This method is called whenever a new item is added.
    /// </summary>
    /// <param name="item">The item that was added.</param>
    /// <returns></returns>
    private async Task OnItemAdd(ItemWrapper<TKey, TValue> item)
    {

    }

    /// <summary>
    /// This method is called whenever an item is edited.
    /// </summary>
    /// <param name="item">The modified item.</param>
    /// <returns></returns>
    private async Task OnItemEdit(ItemWrapper<TKey, TValue> item)
    {
        var wrappedItems = _wrappedItems;

        _items.Clear();
        foreach (var wrappedItem in wrappedItems)
            _items.Add(wrappedItem.Key, wrappedItem.Value);
    }

    /// <summary>
    /// This method is called whenever an item is deleted.
    /// </summary>
    /// <param name="item">The item that was deleted.</param>
    /// <returns></returns>
    private async Task OnItemDelete(ItemWrapper<TKey, TValue> item)
    {

    }

    /// <summary>
    /// Wrapper class needed to work around pass by value nature of primitive types.
    /// </summary>
    /// <typeparam name="TItem1"></typeparam>
    /// <typeparam name="TItem2"></typeparam>
    private class ItemWrapper<TItem1, TItem2>
    {
        public TItem1 Key { get; set; }
        public TItem2 Value { get; set; }
    }
}