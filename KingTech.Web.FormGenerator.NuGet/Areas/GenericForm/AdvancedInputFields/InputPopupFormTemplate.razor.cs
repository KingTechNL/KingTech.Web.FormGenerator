using Humanizer;
using Microsoft.AspNetCore.Components;
using TabBlazor;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// Input field for complex types.
/// This field uses a modal to add/edit the object.
/// </summary>
/// <typeparam name="TModel">Type of the object to generate an input field for.</typeparam>
public partial class InputPopupFormTemplate<TModel>
{
    /// <summary>
    /// The item this input field is for.
    /// </summary>
    [Parameter]
    public TModel? Item { get; set; }

    /// <summary>
    /// Custom title for this input field (optional)
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// This method is called whenever this item is edited.
    /// </summary>
    /// <param name="item">The modified item.</param>
    /// <returns></returns>
    private async Task OnItemEdit(TModel item)
    {
        var result = item;

        var component = new RenderComponent<PopupForm<TModel>>()
            .Set(e => e.Item, item);

        var popupResult = await _modalService.ShowAsync($"Edit {Title.Singularize()}", component);
        if (popupResult.Cancelled)
            return;
        result = (TModel)popupResult.Data;
    }

    /// <summary>
    /// This method is called whenever a new item is created.
    /// </summary>
    /// <param name="item">The modified item.</param>
    /// <returns></returns>
    private async Task AddItem()
    {
        Item = new TModel();
        var component = new RenderComponent<PopupForm<TModel>>()
            .Set(e => e.Item, Item);
        var popupResult = await _modalService.ShowAsync($"Add {Title.Singularize()}", component);

        if (popupResult.Cancelled)
            return;
        var result = (TModel)popupResult.Data;
    }
}