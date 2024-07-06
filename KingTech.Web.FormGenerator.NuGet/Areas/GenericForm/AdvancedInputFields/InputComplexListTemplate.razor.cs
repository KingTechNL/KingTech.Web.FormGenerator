using System.Collections;
using System.Reflection;
using Humanizer;
using KingTech.Web.FormGenerator.Abstract;
using KingTech.Web.FormGenerator.Data;
using Microsoft.AspNetCore.Components;
using TabBlazor;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// Form input for lists of complex types.
/// Adding/editing items is done using a modal.
/// </summary>
/// <typeparam name="TModel">The type of items in the list.</typeparam>
public partial class InputComplexListTemplate<TModel> : FormSetup.IVisibilityModeListener
{
    /// <summary>
    /// The list to generate an input field for.
    /// </summary>
    [Parameter]
    public List<TModel> Items { get; set; }

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

    private EVisibilityMode SetupType { get; set; }
    /// <summary>
    /// This method is called whenever a new item needs to be added to the list.
    /// </summary>
    /// <returns>The new item to add to the list.</returns>
    private async Task AddItem()
    {
        var newItem = new TModel();
        await OnItemAdd(newItem);
    }

    /// <summary>
    /// This method is called whenever a new item is added.
    /// </summary>
    /// <param name="item">The item that was added.</param>
    /// <returns></returns>
    private async Task OnItemAdd(TModel item)
    {
        var result = item;

        var component = new RenderComponent<PopupForm<TModel>>()
            .Set(e => e.Item, item);

        var popupResult = await _modalService.ShowAsync($"Add {Title.Singularize()}", component);
        if (popupResult.Cancelled)
            return;
        result = (TModel)popupResult.Data;

        Items.Add(result);
    }

    /// <summary>
    /// This method is called whenever an item is edited.
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
    /// This method is called whenever an item is deleted.
    /// </summary>
    /// <param name="item">The item that was deleted.</param>
    /// <returns></returns>
    private async Task OnItemDelete(TModel item)
    {
        Items.Remove(item);
    }

    private IEnumerable<PropertyInfo> GetModelFields(EVisibilityMode visibilityMode)
    {
        return FormFieldsScanner.GetEditableProperties(typeof(TModel), visibilityMode);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        GenericFormService.AddModeListener(this);
        SetupType = GenericFormService.VisibilityMode;
    }

    public void VisibilityModeChanged(EVisibilityMode value)
    {
        SetupType = value;
    }
}