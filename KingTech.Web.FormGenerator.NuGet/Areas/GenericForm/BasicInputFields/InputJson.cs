using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.BasicInputFields;

/// <summary>
/// This input field uses json to create complex types.
/// It creates a standard textfield and parses the actual item from and to json.
/// </summary>
/// <typeparam name="TValue">The actual type to handle.</typeparam>
public class InputJson<TValue> : InputBase<TValue>
{
    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "text");
        builder.AddAttribute(3, "class", CssClass);
        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue? value)
    {
        var json = JsonConvert.SerializeObject(value)?.Trim();

        //Remove quotes surrounding json string.
        if (json != null && json.StartsWith("\"") && json.EndsWith("\""))
        {
            json = json.Substring(1, json.Length - 2);
        }

        return json ?? string.Empty;
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
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
            result = JsonConvert.DeserializeObject<TValue>(value);
            return result != null;
        }
        catch (Exception e)
        {
            validationErrorMessage = $"Unable to deserialize {typeof(TValue).Name}";
            result = default;
            return false;
        }
    }
}