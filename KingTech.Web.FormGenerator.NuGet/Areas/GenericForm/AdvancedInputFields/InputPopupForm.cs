using Fuzzy.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

public class InputPopupForm<TModel> : ABaseAdvancedInputField<TModel> where TModel : new()
{
    /// <summary>
    /// The name of the property for which this list is created.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Create a RenderFragment that is used for the actual user interface for this type.
    /// This RenderFragment will display a listview with buttons for CRUD operations. The Create and Update buttons will spawn a popup with a form for the inner type.
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to add user interface for this type to.</param>
    public override void CreateFragment(RenderTreeBuilder builder)
    {
        builder.Build()
            .OpenComponent(typeof(InputPopupFormTemplate<TModel>))
            .Attribute("Title", Title)
            .Attribute("Item", CurrentValue)
            .Close();
    }
}