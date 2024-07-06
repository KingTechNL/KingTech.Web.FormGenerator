using Fuzzy.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

public class InputList<TActualType> : ABaseAdvancedInputField<TActualType>
{
    /// <summary>
    /// The name of the property for which this list is created.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// The type of object stored in this list.
    /// </summary>
    public Type InnerType { get; private set; }

    public InputList() : base()
    {
        if (typeof(TActualType).IsGenericType)
        {
            var genericTypeArguments = typeof(TActualType).GetGenericArguments();
            InnerType = genericTypeArguments.First();
        }
        else
        {
            throw new Exception($"Creating {nameof(InputList<TActualType>)} without generic type.");
        }
    }

    /// <summary>
    /// Create a RenderFragment that is used for the actual user interface for this type.
    /// This RenderFragment will display a listview with buttons for CRUD operations. The Create and Update buttons will spawn a popup with a form for the inner type.
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to add user interface for this type to.</param>
    public override void CreateFragment(RenderTreeBuilder builder)
    {
        var primitive = !IsUserDefinedType();
        var componentType = primitive
            ? typeof(InputPrimitiveListTemplate<>).MakeGenericType(InnerType)
            : typeof(InputComplexListTemplate<>).MakeGenericType(InnerType);

        builder.Build()
            .OpenComponent(componentType)
            .Attribute("Title", Title)
            .Attribute("Items", CurrentValue)
            .Close();
    }

    /// <summary>
    /// Check if the InnerType is a user defined type.
    /// </summary>
    /// <returns>True if InnerType is a primitive type, false otherwise.</returns>
    private bool IsUserDefinedType() => InnerType.IsUserDefined();
}