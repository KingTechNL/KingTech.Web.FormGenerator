using Humanizer;
using Microsoft.AspNetCore.Components;
using TabBlazor;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// Form input for lists of complex types.
/// Adding/editing items is done using a modal.
/// </summary>
/// <typeparam name="TModel">The type of items in the list.</typeparam>
public partial class InputComplexDictionaryTemplate<TKey, TValue>
{
    /// <summary>
    /// The list to generate an input field for.
    /// </summary>
    [Parameter]
    public Dictionary<TKey, TValue> Items
    {
        get => Items;
        set
        {
            _items = value;
            _wrappedItems = value?.Select(item => new ItemWrapper<TKey, TValue>() { Key = item.Key, Value = item.Value })
                .ToList();
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
    /// Internal list used to generate forms.
    /// </summary>
    private List<ItemWrapper<TKey, TValue>> _wrappedItems { get; set; }

    /// <summary>
    /// This method is called whenever a new item needs to be added to the list.
    /// </summary>
    /// <returns>The new item to add to the list.</returns>
    private async Task<ItemWrapper<TKey, TValue>> AddItem()
    {
        var newItem = new ItemWrapper<TKey, TValue>();
        await OnItemAdd(newItem);
        return newItem;
    }

    /// <summary>
    /// This method is called whenever a new item is added.
    /// </summary>
    /// <param name="item">The item that was added.</param>
    /// <returns></returns>
    private async Task OnItemAdd(ItemWrapper<TKey, TValue> item)
    {
        var result = item;

        var component = new RenderComponent<PopupForm<ItemWrapper<TKey, TValue>>>()
            .Set(e => e.Item, item);

        var popupResult = await _modalService.ShowAsync($"Add {Title.Singularize()}", component);
        if (popupResult.Cancelled)
            return;
        result = (ItemWrapper<TKey, TValue>)popupResult.Data;
    }

    /// <summary>
    /// This method is called whenever an item is edited.
    /// </summary>
    /// <param name="item">The modified item.</param>
    /// <returns></returns>
    private async Task OnItemEdit(ItemWrapper<TKey, TValue> item)
    {
        var result = item;

        var component = new RenderComponent<PopupForm<ItemWrapper<TKey, TValue>>>()
            .Set(e => e.Item, item);

        var popupResult = await _modalService.ShowAsync($"Edit {Title.Singularize()}", component);
        if (popupResult.Cancelled)
            return;
        result = (ItemWrapper<TKey, TValue>)popupResult.Data;
    }

    /// <summary>
    /// This method is called whenever an item is deleted.
    /// </summary>
    /// <param name="item">The item that was deleted.</param>
    /// <returns></returns>
    private async Task OnItemDelete(ItemWrapper<TKey, TValue> item)
    {

    }


    private class ItemWrapper<TItem1, TItem2>
    {
        public TItem1 Key { get; set; }
        public TItem2 Value { get; set; }
    }
}