using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

public abstract class ABaseAdvancedInputField<TActualType> : InputBase<TActualType>
{
    /// <summary>
    /// The value of this field as json.
    /// This will be used in the (hidden) field.
    /// </summary>
    protected string ValueAsJson
    {
        get => FormatValueAsString(Value);
        set => TryParseValueFromString(value, out _, out _);
    }

    /// <summary>
    /// The actual value of this field.
    /// </summary>
    protected TActualType? Value { get; set; }

    protected ABaseAdvancedInputField()
    {
    }

    /// <summary>
    /// Create a RenderFragment that is used for the actual user interface for this type.
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to add user interface for this type to.</param>
    public abstract void CreateFragment(RenderTreeBuilder builder);

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //GetInputField(builder);
        CreateFragment(builder);
    }

    /// <summary>
    /// Get a (hidden) input field for this value.
    /// Used to set the value in the actual form.
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to add a hidden input field for this object to.</param>
    protected virtual void GetInputField(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "type", "hidden");
        builder.AddAttribute(2, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(3, "onchange", EventCallback.Factory.CreateBinder<string?>(this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.CloseElement();
    }

    /// <summary>
    /// Serialize the given object to a json string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>The resulting json string.</returns>
    protected override string FormatValueAsString(TActualType? value)
    {
        Value = value;
        var json = JsonConvert.SerializeObject(value);
        return json ?? string.Empty;
    }

    /// <summary>
    /// Parse the actual value of this field from a (json) string.
    /// </summary>
    /// <param name="value">The (json) string value.</param>
    /// <param name="type">The type to parse it to.</param>
    /// <param name="result">The resulting object.</param>
    /// <param name="validationErrorMessage">If something went wrong, we specify an error message here.</param>
    /// <returns>True on success, false otherwise.</returns>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TActualType result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        //If no value given, we return a default.
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return true;
        }

        try
        {
            result = JsonConvert.DeserializeObject<TActualType>(value);
            Value = result;
            return result != null;
        }
        catch (Exception e)
        {
            validationErrorMessage = $"Unable to deserialize {typeof(TActualType).Name}";
            result = default;
            return false;
        }
    }
}