using System.ComponentModel.DataAnnotations;
using System.Reflection;
using KingTech.Web.FormGenerator.Abstract;

namespace KingTech.Web.FormGenerator.Data;

public class FormFieldsScanner
{
    public static IEnumerable<PropertyInfo> GetEditableProperties(Type type)
    {
        return GetAllProperties(type)
            .Where(property => property.SetMethod != null) //filter readonly
            .Where(property => property.GetCustomAttribute<EditableAttribute>() is not { } editor || editor.AllowEdit) //filter editors, that have allowEdit false
            ;
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    }

    public static bool IsFieldActive(PropertyInfo property, object owner, Type type)
    {
        //check if property has Attributes DependsOnAttribute
        return property.GetCustomAttributes(false)
            .OfType<DependsOnAttribute>()
            .All(dependsOn =>
                //get all fields from the owner
                //if one of the field matches, get it's value
                GetAllProperties(type)
                    .Where(propInfo => propInfo.Name == dependsOn.FieldName)
                    .All(propInfo => dependsOn.Predicate.Invoke(propInfo.GetValue(owner)))
            );
    }

    public static IEnumerable<PropertyInfo> GetEditableProperties(Type type, EVisibilityMode setupSetupType)
    {
        return GetEditableProperties(type)
            .Where(info =>
            {
                return info
                    .GetCustomAttributes(false)
                    .OfType<FormFieldInfoAttribute>()
                    .All(settingsInfoAttribute => settingsInfoAttribute.Mode <= setupSetupType);
            });
    }
}