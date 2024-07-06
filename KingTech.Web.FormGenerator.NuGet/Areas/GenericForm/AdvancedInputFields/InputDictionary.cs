using Fuzzy.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

public class InputDictionary<TActualType> : ABaseAdvancedInputField<TActualType>
{
    /// <summary>
    /// The name of the property for which this dictionary is created.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// The type of keys stored in this dictionary.
    /// </summary>
    public Type KeyType { get; private set; }
    /// <summary>
    /// The type of values stored in this dictionary.
    /// </summary>
    public Type ValueType { get; private set; }

    public InputDictionary() : base()
    {
        if (typeof(TActualType).IsGenericType)
        {
            var genericTypeArguments = typeof(TActualType).GetGenericArguments();
            if (genericTypeArguments.Length == 2)
            {
                KeyType = genericTypeArguments[0];
                ValueType = genericTypeArguments[1];
            }
            else
            {
                throw new Exception($"Creating {nameof(InputDictionary<TActualType>)} with {genericTypeArguments.Length} generic type(s).");
            }
        }
        else
        {
            throw new Exception($"Creating {nameof(InputDictionary<TActualType>)} without generic type.");
        }
    }

    /// <summary>
    /// Create a RenderFragment that is used for the actual user interface for this type.
    /// This RenderFragment will display a listview with buttons for CRUD operations. The Create and Update buttons will spawn a popup with a form for the key and value types.
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to add user interface for this type.</param>
    public override void CreateFragment(RenderTreeBuilder builder)
    {
        var primitive = !IsUserDefinedType();
        var componentType = primitive
            ? typeof(InputPrimitiveDictionaryTemplate<,>).MakeGenericType(KeyType, ValueType)
            : typeof(InputComplexListTemplate<>).MakeGenericType(KeyType, ValueType);

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
    private bool IsUserDefinedType() => KeyType.IsUserDefined() || ValueType.IsUserDefined();
}