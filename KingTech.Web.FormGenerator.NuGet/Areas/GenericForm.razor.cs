using Microsoft.AspNetCore.Components;
using TabBlazor;
using KingTech.Web.FormGenerator;
using KingTech.Web.FormGenerator.Abstract;

namespace KingTech.Web.FormGenerator.Areas;

/// <summary>
/// The GenericForm is generated for each section in the settings and creates a generic form for all properties.
/// </summary>
/// <typeparam name="TModel">The settings type this form needs to be generated for.</typeparam>
public partial class GenericForm<TModel>
{
    /// <summary>
    /// The type of settings object this form is generated for.
    /// </summary>
    [Parameter]
    public Type DataType { get; set; } = typeof(TModel);

    /// <summary>
    /// The current value of the settings this form is generated for.
    /// </summary>
    [Parameter]
    public TModel? Data { get; set; }

    /// <summary>
    /// Method to call upon submitting the generated form.
    /// Method should result in a list of error messages (or an empty list if no errors are encountered).
    /// </summary>
    [Parameter]
    public Func<TModel?, IList<string>>? OnSubmit { get; set; }

    /// <summary>
    /// Method to call upon submitting the generated form.
    /// Method should result in a list of error messages (or an empty list if no errors are encountered).
    /// </summary>
    [Parameter]
    public Func<TModel?, Task<IList<string>>>? OnSubmitAsync { get; set; }

    /// <summary>
    /// List of error messages to display.
    /// </summary>
    private IList<string> Errors { get; set; }

    /// <summary>
    /// Method is called when the form is submitted.
    /// Validates and stores the data in the FormService.
    /// Finally moves to next TabPage in the parenting TabControl.
    /// </summary>
    private async Task FormSubmitted()
    {
        if(OnSubmit != null)
            Errors = OnSubmit.Invoke(Data);
        if(OnSubmitAsync != null)
            Errors = await OnSubmitAsync.Invoke(Data);
    }



    private string Description => typeof(TModel).FormInfo()?.Description ?? string.Empty;
}