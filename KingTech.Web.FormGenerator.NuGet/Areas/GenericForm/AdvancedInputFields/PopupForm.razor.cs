using Microsoft.AspNetCore.Components;
using TabBlazor;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// The popupform is passed to a modal in order to create a form for complex types.
/// </summary>
/// <typeparam name="TModel">The type this form is for.</typeparam>
public partial class PopupForm<TModel>
{
    /// <summary>
    /// The item this form is for.
    /// </summary>
    [Parameter]
    public TModel? Item { get; set; }

    /// <summary>
    /// Check if the TModel is a primitive type.
    /// </summary>
    /// <returns>True if TModel is a primitive type, false otherwise.</returns>
    private bool IsPrimitive() => typeof(TModel).IsPrimitive;

    /// <summary>
    /// This method is called whenever the popup form is submitted.
    /// It closes the popup, returning the new data.
    /// </summary>
    private void PopupFormSubmitted()
    {
        _modalService.Close(ModalResult.Ok<TModel>(Item));
    }

    /// <summary>
    /// This method is called whenever the popup form is cancelled.
    /// It closes the popup, returning a 'cancelled' flag.
    /// </summary>
    private void Cancel()
    {
        _modalService.Close(ModalResult.Cancel());
    }
}