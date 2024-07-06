using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.BasicInputFields;

/// <summary>
/// This input field is used to handle TimeSpan fields.
/// </summary>
public class InputTimeSpan : InputBase<TimeSpan>
{
    private const string DateFormat = "yyyy-MM-ddTHH:mm";

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "datetime-local");
        builder.AddAttribute(3, "class", CssClass);
        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TimeSpan value) => value.ToString("c");

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TimeSpan result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        //If no value given, we return a default.
        if (string.IsNullOrWhiteSpace(value))
        {
            result = TimeSpan.Zero;
            return true;
        }

        var ret = TimeSpan.TryParse(value, out result);
        if (!ret)
            validationErrorMessage = $"Unable to parse {value} into timespan.";
        return ret;
    }
}