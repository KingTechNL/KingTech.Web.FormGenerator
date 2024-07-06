using Microsoft.AspNetCore.Components;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// Form input for lists of simple types.
/// Adding/editing items is done in the table.
/// </summary>
/// <typeparam name="TModel">The type of items in the list.</typeparam>
public partial class InputPrimitiveListTemplate<TModel>
{
    /// <summary>
    /// The list to generate an input field for.
    /// </summary>
    [Parameter]
    public List<TModel> Items
    {
        get => Items;
        set
        {
            _items = value;
            _wrappedItems = value?.Select(item => new ItemWrapper<TModel>() { Item = item }).ToList();
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

    private List<TModel> _items { get; set; }

    /// <summary>
    /// Internal list used to generate fields. Primitive types are passed by value and this causes issues.
    /// </summary>
    private List<ItemWrapper<TModel>> _wrappedItems { get; set; }

    /// <summary>
    /// This method is called whenever a new item needs to be added to the list.
    /// </summary>
    /// <returns>The new item to add to the list.</returns>
    private Task<ItemWrapper<TModel>> AddItem()
    {
        return Task.FromResult(new ItemWrapper<TModel>());
    }

    /// <summary>
    /// This method is called whenever a new item is added.
    /// </summary>
    /// <param name="item">The item that was added.</param>
    /// <returns></returns>
    private async Task OnItemAdd(ItemWrapper<TModel> item)
    {

    }

    /// <summary>
    /// This method is called whenever an item is edited.
    /// </summary>
    /// <param name="item">The modified item.</param>
    /// <returns></returns>
    private async Task OnItemEdit(ItemWrapper<TModel> item)
    {
        var result = item.Item;

        var wrappedItems = _wrappedItems;

        _items.Clear();
        _items.AddRange(wrappedItems.Select(wi => wi.Item));
    }

    /// <summary>
    /// This method is called whenever an item is deleted.
    /// </summary>
    /// <param name="item">The item that was deleted.</param>
    /// <returns></returns>
    private async Task OnItemDelete(ItemWrapper<TModel> item)
    {

    }

    /// <summary>
    /// Wrapper class needed to work around pass by value nature of primitive types.
    /// </summary>
    /// <typeparam name="TItemModel"></typeparam>
    private class ItemWrapper<TItemModel>
    {
        public TItemModel Item { get; set; }
    }
}