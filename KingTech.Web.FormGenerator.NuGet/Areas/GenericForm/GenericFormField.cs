using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using KingTech.Web.FormGenerator.Abstract;
using KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;
using KingTech.Web.FormGenerator.Areas.GenericForm.BasicInputFields;
using KingTech.Web.FormGenerator.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace KingTech.Web.FormGenerator.Areas.GenericForm;

/// <summary>
/// The GenericFormField is used to generate an input field based on the type of value that needs to be edited.
/// </summary>
/// <typeparam name="TParentModel">The type of property the form this field belongs to is generated for.</typeparam>
public sealed class GenericFormField<TParentModel>
{
    private const string DefaultHtmlClass = "form-control";

    #region Properties

    /// <summary>
    /// This eventhandler is called whenever the value of the input field changes.
    /// </summary>
    internal event EventHandler? ValueChanged;
    /// <summary>
    /// The minimal settings mode this field should show up for.
    /// </summary>
    internal EVisibilityMode Mode => _settingsInfo?.Mode ?? EVisibilityMode.Basic;

    /// <summary>
    /// The Property this field is created for.
    /// </summary>
    internal PropertyInfo Property { get; }
    /// <summary>
    /// Unique ID for the edit field (used by html).
    /// </summary>
    internal string EditorId => _formFields.BaseEditorId + '_' + Property.Name;
    /// <summary>
    /// The object the form this field is for this field belongs to.
    /// </summary>
    internal TParentModel Owner => _formFields.Model!;

    /// <summary>
    /// The name used to create a label for this field.
    /// </summary>
    internal string DisplayName
    {
        get
        {
            var displayAttribute = Property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                var displayName = displayAttribute.GetName();
                if (!string.IsNullOrEmpty(displayName))
                    return displayName;
            }

            var displayNameAttribute = Property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttribute != null)
            {
                var displayName = displayNameAttribute.DisplayName;
                if (!string.IsNullOrEmpty(displayName))
                    return displayName;
            }

            return Property.Name;
        }
    }

    /// <summary>
    /// The index of this field in the parent form.
    /// </summary>
    internal int DisplayOrder
    {
        get
        {
            var displayAttribute = Property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                var displayOrder = displayAttribute.GetOrder();
                if (displayOrder != null)
                    return (int)displayOrder;
            }

            // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute.getorder?view=net-5.0#remarks
            // If an order is not specified, presentation layers should consider setting the value 
            // of the Order property to 10000. This value lets explicitly-ordered fields be displayed 
            // before and after the fields that do not have a specified order.
            return 10_000;
        }
    }

    /// <summary>
    /// Description for this field.
    /// Will be shown on hover (html title).
    /// </summary>
    internal string? Description
    {
        get
        {
            var displayAttribute = Property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                var description = displayAttribute.GetDescription();
                if (!string.IsNullOrEmpty(description))
                    return description;
            }

            var descriptionAttribute = Property.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                var description = descriptionAttribute.Description;
                if (!string.IsNullOrEmpty(description))
                    return description;
            }

            if (!string.IsNullOrWhiteSpace(_settingsInfo?.Description))
            {
                return _settingsInfo.Description;
            }

            return null;
        }
    }

    /// <summary>
    /// The type of the property this field is generated for.
    /// </summary>
    internal Type PropertyType => Property.PropertyType;

    /// <summary>
    /// The current value of the property this field is generated for.
    /// Will automatically update when the field is modified.
    /// </summary>
    internal object? Value
    {
        get => Property.GetValue(Owner);
        set
        {
            if (Property.SetMethod != null && !Equals(Value, value))
            {
                Property.SetValue(Owner, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// This template will be used to display information when the input value of this field isnt valid..
    /// </summary>
    internal RenderFragment? FieldValidationTemplate
    {
        get
        {
            if (!_formFields.EnableFieldValidation)
                return null;

            return _fieldValidationTemplate ??= builder =>
            {
                // () => Owner.Property
                var access = Expression.Property(Expression.Constant(Owner, typeof(TParentModel)), Property);
                var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(PropertyType), access);

                builder.OpenComponent(0, typeof(ValidationMessage<>).MakeGenericType(PropertyType));
                builder.AddAttribute(1, "For", lambda);
                builder.CloseComponent();
            };
        }
    }

    /// <summary>
    /// This template will be used to display an input field for this property.
    /// </summary>
    internal RenderFragment EditorTemplate
    {
        get
        {
            if (_editorTemplate != null)
                return _editorTemplate;

            // () => Owner.Property
            var access = Expression.Property(Expression.Constant(Owner, typeof(TParentModel)), Property);
            var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(PropertyType), access);

            // Create(object receiver, Action<object> callback
            var method = s_eventCallbackFactoryCreate.MakeGenericMethod(PropertyType);

            // value => Field.Value = value;
            var changeHandlerParameter = Expression.Parameter(PropertyType);
            var body = Expression.Assign(Expression.Property(Expression.Constant(this), nameof(Value)), Expression.Convert(changeHandlerParameter, typeof(object)));
            var changeHandlerLambda = Expression.Lambda(typeof(Action<>).MakeGenericType(PropertyType), body, changeHandlerParameter);
            var changeHandler = method.Invoke(EventCallback.Factory, new object[] { this, changeHandlerLambda.Compile() });

            return _editorTemplate ??= builder =>
            {
                var (componentType, additonalAttributes) = GetBaseEditorType(Property, IsReadOnly);
                builder.OpenComponent(0, componentType);
                builder.AddAttribute(1, "Value", Value);
                builder.AddAttribute(2, "ValueChanged", changeHandler);
                builder.AddAttribute(3, "ValueExpression", lambda);
                builder.AddAttribute(4, "id", EditorId);
                builder.AddAttribute(5, "class", _formFields.EditorClass);
                builder.AddMultipleAttributes(6, additonalAttributes);
                if (!string.IsNullOrWhiteSpace(Description))
                    builder.AddAttribute(7, "title", Description);
                if (IsReadOnly)
                    builder.AddAttribute(8, "readonly", "true");
                builder.CloseComponent();
            };
        }
    }

    /// <summary>
    /// FormInfo attribute set on the property this field is generated for.
    /// </summary>
    private FormFieldInfoAttribute? _settingsInfo;

    private static readonly MethodInfo s_eventCallbackFactoryCreate = GetEventCallbackFactoryCreate();
    private readonly GenericFormFields<TParentModel> _formFields;
    private readonly IEnumerable<string> _readOnlyKeys;
    private RenderFragment? _editorTemplate;
    private RenderFragment? _fieldValidationTemplate;

    #endregion

    #region Create GenericFormFields

    /// <summary>
    /// The GenericFormField is used to generate an input field based on the type of value that needs to be edited.
    /// </summary>
    /// <param name="formFields">Form this field is part of.</param>
    /// <param name="readOnlyKeys">The keys to mark readonly</param>
    /// <param name="propertyInfo">The property this field is generated for.</param>
    private GenericFormField(GenericFormFields<TParentModel> formFields, PropertyInfo propertyInfo, IEnumerable<string> readOnlyKeys)
    {
        _formFields = formFields;
        _readOnlyKeys = readOnlyKeys;
        Property = propertyInfo;

        _settingsInfo = propertyInfo.FormInfo();
    }

    /// <summary>
    /// The GenericFormField is used to generate an input field based on the type of value that needs to be edited.
    /// This method creates a GenericFormField for every public property in the given form.
    /// </summary>
    /// <param name="formFields">Form to generate fields for.</param>
    /// <param name="readOnlyKeys">The keys to mark readonly</param>
    /// <returns>A list of GenericFormFields for every public property in the given form.</returns>
    internal static List<GenericFormField<TParentModel>> Create(GenericFormFields<TParentModel> formFields, IEnumerable<string> readOnlyKeys) =>
            FormFieldsScanner.GetEditableProperties(typeof(TParentModel))
             .Select(info => new GenericFormField<TParentModel>(formFields, info, readOnlyKeys)).ToList();

    #endregion

    #region internals

    /// <summary>
    /// Get the actual editor to generate based on the type of the property, like strings, numbers, dates, e.t.c.
    /// </summary>
    /// <remarks>If a new (generic) input is to be introduced, it should be added here.</remarks>
    /// <param name="property">The property to get the editor for.</param>
    /// <returns>All information needed to create a editor for the given property.</returns>
    private static (Type ComponentType, IEnumerable<KeyValuePair<string, object>>? AdditonalAttributes) GetBaseEditorType(PropertyInfo property, bool isReadonly = false)
    {
        // Check EditorAttribute declared on the property
        var editorAttributes = property.GetCustomAttributes<EditorAttribute>();
        foreach (var editorAttribute in editorAttributes)
        {
            if (editorAttribute.EditorBaseTypeName == typeof(InputBase<>).AssemblyQualifiedName)
                return (Type.GetType(editorAttribute.EditorTypeName, throwOnError: true)!, null);
        }

        // Check EditorAttribute declared on the property type
        editorAttributes = property.PropertyType.GetCustomAttributes<EditorAttribute>();
        foreach (var editorAttribute in editorAttributes)
        {
            if (editorAttribute.EditorBaseTypeName == typeof(InputBase<>).AssemblyQualifiedName)
                return (Type.GetType(editorAttribute.EditorTypeName, throwOnError: true)!, null);
        }

        // Infer the editor based on the property type and other annotations
        if (property.PropertyType == typeof(bool))
            return (typeof(InputCheckbox), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("class", "form-check-input")));

        if (property.PropertyType == typeof(string))
        {
            var dataType = property.GetCustomAttribute<DataTypeAttribute>();
            if (dataType != null)
            {
                if (dataType.DataType == DataType.Date)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "date")));

                if (dataType.DataType == DataType.DateTime)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "datetime-local")));

                if (dataType.DataType == DataType.EmailAddress)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "email")));

                if (dataType.DataType == DataType.MultilineText)
                    return (typeof(InputTextArea), GenerateHtmlTags(property));

                if (dataType.DataType == DataType.Password)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "password")));

                if (dataType.DataType == DataType.PhoneNumber)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "tel")));

                if (dataType.DataType == DataType.Time)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "time")));

                if (dataType.DataType == DataType.Url)
                    return (typeof(InputText), GenerateHtmlTags(property, KeyValuePair.Create<string, object>("type", "url")));
            }

            if (property.UseBigField())
                return (typeof(InputTextArea), GenerateHtmlTags(property));
            return (typeof(InputText), GenerateHtmlTags(property));
        }

        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (underlyingType == typeof(short))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(int))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(long))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(float))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(double))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(decimal))
            return (typeof(InputNumber<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

        if (underlyingType == typeof(DateTime))
        {
            var dataType = property.GetCustomAttribute<DataTypeAttribute>();
            if (dataType != null && dataType.DataType == DataType.Date)
                return (typeof(InputDate<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

            return (typeof(InputDateTime<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));
        }

        if (underlyingType == typeof(DateTimeOffset))
        {
            var dataType = property.GetCustomAttribute<DataTypeAttribute>();
            if (dataType != null && dataType.DataType == DataType.Date)
                return (typeof(InputDate<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));

            return (typeof(InputDateTime<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));
        }

        if (property.PropertyType == typeof(Uri))
            return (typeof(InputUrl<Uri>), GenerateHtmlTags(property));

        if (property.PropertyType.IsEnum)
        {
            if (!property.PropertyType.IsDefined(typeof(FlagsAttribute), inherit: true))
                return (typeof(InputEnumSelect<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));
        }

        //Check more advanced types.
        if (property.PropertyType.IsGenericType)
        {
            //Lists
            if (typeof(IList).IsAssignableFrom(property.PropertyType))
                return (typeof(InputList<>).MakeGenericType(property.PropertyType), new[] { KeyValuePair.Create<string, object>("IsDisabled", isReadonly) });
            if (typeof(IDictionary).IsAssignableFrom(property.PropertyType))
                return (typeof(InputDictionary<>).MakeGenericType(property.PropertyType), null);
        }

        if (property.PropertyType.IsUserDefined())
        {
            return (typeof(InputPopupForm<>).MakeGenericType(property.PropertyType), null);
        }

        return (typeof(InputJson<>).MakeGenericType(property.PropertyType), GenerateHtmlTags(property));
    }

    /// <summary>
    /// Create HTML tags for the given property.
    /// These tags will be added to the generated input field.
    /// </summary>
    /// <param name="property">The property to generate tags for.</param>
    /// <param name="baseTags">One or more base tags to include.</param>
    /// <returns>A list of HTML tags to add to the input field for this property.</returns>
    private static KeyValuePair<string, object>[] GenerateHtmlTags(PropertyInfo property, params KeyValuePair<string, object>[] baseTags)
    {
        var formInfo = property.FormInfo();
        var placeholder = formInfo?.Example;

        var htmlTags = baseTags.ToDictionary(baseTag => baseTag.Key, baseTag => baseTag.Value);

        //Add html classes
        var htmlClasses = (formInfo?.CustomCssClasses?.Any() ?? false) ? string.Join(" ",formInfo.CustomCssClasses) : DefaultHtmlClass;
        htmlClasses += " " + htmlTags.GetValueOrDefault("class", "");
        htmlTags["class"] = htmlClasses;
        
        //Add placeholder if given
        if (!string.IsNullOrWhiteSpace(placeholder))
            htmlTags.Add("placeholder", property.FormInfo().Example);
        
        return htmlTags.ToArray();
    }

    /// <summary>
    /// Create HTML tags for the given property.
    /// These tags will be added to the generated input field.
    /// </summary>
    /// <param name="property">The property to generate tags for.</param>
    /// <returns>A list of HTML tags to add to the input field for this property.</returns>
    private static KeyValuePair<string, object>[] GenerateHtmlTags(PropertyInfo property) =>
        GenerateHtmlTags(property, Array.Empty<KeyValuePair<string, object>>());


    /// <summary>
    /// Create an event callback factory.
    /// </summary>
    /// <returns>Method used for creating an event callback.</returns>
    private static MethodInfo GetEventCallbackFactoryCreate()
    {
        return typeof(EventCallbackFactory).GetMethods()
            .Single(m =>
            {
                if (m.Name != "Create" || !m.IsPublic || m.IsStatic || !m.IsGenericMethod)
                    return false;

                var generic = m.GetGenericArguments();
                if (generic.Length != 1)
                    return false;

                var args = m.GetParameters();
                return args.Length == 2 && args[0].ParameterType == typeof(object) && args[1].ParameterType.IsGenericType && args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            });
    }
    #endregion

    public bool IsVisible()
    {
        return FormFieldsScanner.IsFieldActive(Property, Owner, typeof(TParentModel));
    }

    /// <summary>
    /// Is the unique key of this field in the readonly keys selection
    /// </summary>
    internal bool IsReadOnly
    {
        get
        {
            if(Property.GetCustomAttribute<ReadOnlyFieldAttribute>() != null)
                return true;
            if(_readOnlyKeys?.ToList()?.Contains(Key) ?? false)
                return true;
            return false;
        }
    }

    internal string? Key => $"{Owner?.GetType().Name}:{Property.Name}";
}