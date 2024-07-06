using KingTech.Web.FormGenerator.Abstract;
using KingTech.Web.FormGenerator.Data;
using Microsoft.AspNetCore.Components;

namespace KingTech.Web.FormGenerator.Areas.GenericForm;

public partial class GenericFormFields<TModel> : FormSetup.IVisibilityModeListener
{
    internal string BaseEditorId { get; } = Guid.NewGuid().ToString();
    private List<GenericFormField<TModel>>? fields;

    [Parameter]
    public TModel? Model { get; set; }

    [Parameter]
    public EventCallback<TModel> ModelChanged { get; set; }

    [Parameter]
    public bool EnableFieldValidation { get; set; } = true;

    [Parameter]
    public string? EditorClass { get; set; }

    [Parameter]
    public RenderFragment<GenericFormField<TModel>>? FieldTemplate { get; set; }
    public EVisibilityMode SetupType { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (fields != null)
        {
            foreach (var field in fields)
            {
                field.ValueChanged -= OnValueChanged;
            }
        }

        if (Model != null)
        {
            fields = GenericFormField<TModel>.Create(this, GenericFormService.ReadonlySettingKeys);
            foreach (var field in fields)
            {
                field.ValueChanged += OnValueChanged;
            }
        }
        else
        {
            fields = null;
        }

        GenericFormService.AddModeListener(this);
        SetupType = GenericFormService.VisibilityMode;
    }

    private void OnValueChanged(object? sender, EventArgs e)
    {
        InvokeAsync(() => ModelChanged.InvokeAsync(Model));
        NotifyOtherFieldsThatMyDependOnThisValue();
    }

    private void NotifyOtherFieldsThatMyDependOnThisValue()
    {
        //we need this for DependsOnAttribute to work
        //fields need to be enabled or disabled everytime a value of one field changes
        StateHasChanged();
    }

    public void VisibilityModeChanged(EVisibilityMode value)
    {
        SetupType = value;
        StateHasChanged();
    }
}