using System.Reflection;
using Blazored.Modal;
using KingTech.Web.FormGenerator.Abstract;
using KingTech.Web.FormGenerator.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TabBlazor;

namespace KingTech.Web.FormGenerator;

/// <summary>
/// Static class used to extend common classes/interfaces used for the FormGenerator.
/// </summary>
public static class FormGeneratorExtensions
{
    /// <summary>
    /// Inject all services needed for the KingTech FormGenerator.
    /// </summary>
    /// <param name="services"></param>
    public static void AddGenericFormService(this IServiceCollection services)
    {

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<GenericFormService>();
        //services.ConfigureOptions(typeof(FormSettings));
        services.AddTabler();
        services.AddBlazoredModal();
    }

    /// <summary>
    /// Check if this type is a user defined class.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if this type is user defined, false otherwise.</returns>
    public static bool IsUserDefined(this Type type)
    {
        return type.IsClass && !type.FullName.StartsWith("System.");
    }

    /// <summary>
    /// Programmical pearl to get an attribute more easily.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to get.</typeparam>
    /// <param name="type">The type to get the attribute for.</param>
    /// <returns>The attribute if available, null otherwise.</returns>
    public static TAttribute GetAttribute<TAttribute>(this Type type)
    where TAttribute : Attribute
    {
        var MyAttribute = Attribute.GetCustomAttribute(type, typeof(TAttribute));
        if (MyAttribute == null)
            return null;
        return (TAttribute)MyAttribute;
    }

    /// <summary>
    /// Programmical pearl to get the FormFieldInfoAttribute of a property.
    /// </summary>
    /// <param name="propertyInfo">The property to get the settings info for.</param>
    /// <returns>The FormInfo attribute if available, null otherwise.</returns>
    public static FormFieldInfoAttribute FormInfo(this PropertyInfo propertyInfo) => propertyInfo?.GetCustomAttributes<FormFieldInfoAttribute>()?.FirstOrDefault();

    /// <summary>
    /// Programmical pearl to get the FormInfoAttribute of a type.
    /// </summary>
    /// <param name="type">The property to get the settings section info for.</param>
    /// <returns>The FormInfo attribute if available, null otherwise.</returns>
    public static FormInfoAttribute FormInfo(this Type type) => type.GetAttribute<FormInfoAttribute>();

    /// <summary>
    /// Check if the UseBigField attibute is set on this property.
    /// </summary>
    /// <param name="propertyInfo">The property to check the attributes for.</param>
    /// <returns>True if the BigField attribute is set, false otherwise.</returns>
    public static bool UseBigField(this PropertyInfo propertyInfo) => propertyInfo?.GetCustomAttributes<BigFieldAttribute>()?.Any() ?? false;
}
