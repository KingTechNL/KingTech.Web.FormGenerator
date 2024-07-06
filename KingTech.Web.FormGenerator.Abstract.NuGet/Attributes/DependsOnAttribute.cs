using System;

namespace KingTech.Web.FormGenerator.Abstract
{
    /// <summary>
    /// Attribute used to define dependencies between form fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// The name of the field this property depends on.
        /// </summary>
        public string FieldName { get; }
        /// <summary>
        /// The required value for that field in order to show this property.
        /// </summary>
        public Predicate<object> Predicate { get; }

        public DependsOnAttribute(string fieldName, Predicate<object> predicate)
        {
            FieldName = fieldName;
            Predicate = predicate;
        }
    }
}