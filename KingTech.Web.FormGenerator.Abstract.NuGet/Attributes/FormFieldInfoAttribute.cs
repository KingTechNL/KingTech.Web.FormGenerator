using System;
using System.Collections.Generic;
using System.Linq;

namespace KingTech.Web.FormGenerator.Abstract
{

    /// <summary>
    /// A custom attribute adding additional information to a settings property used by the FormGenerator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FormFieldInfoAttribute : Attribute
    {
        /// <summary>
        /// The minimum settings mode required to configure this property using the FormGenerator.
        /// </summary>
        public EVisibilityMode Mode { get; }

        /// <summary>
        /// Description of this setting property.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Example value for this setting property.
        /// </summary>
        public string Example { get; }

        /// <summary>
        /// Custom css classes can be set.
        /// By default, the bootstrapper 'form-control' class is assigned.
        /// </summary>
        public List<string> CustomCssClasses { get; }

        /// <summary>
        /// A custom attribute adding additional information to a settings property used by the FormGenerator.
        /// </summary>
        /// <param name="description">Description of this setting property.</param>
        /// <param name="example">Example value for this setting property.</param>
        /// <param name="mode">The minimum settings mode required to configure this property using the FormGenerator.</param>
        public FormFieldInfoAttribute(string description, string example = "", EVisibilityMode mode = EVisibilityMode.Basic)
        {
            Mode = mode;
            Description = description;
            Example = example;
        }

        /// <summary>
        /// A custom attribute adding additional information to a settings property used by the FormGenerator.
        /// </summary>
        /// <param name="description">Description of this setting property.</param>
        /// <param name="customCssClasses">Custom css classes to add to html input field.</param>
        /// <param name="mode">The minimum settings mode required to configure this property using the FormGenerator.</param>
        public FormFieldInfoAttribute(string description, IEnumerable<string> customCssClasses, EVisibilityMode mode = EVisibilityMode.Basic)
        : this(description, string.Empty, mode)
        {
            CustomCssClasses = customCssClasses?.ToList();
        }
    }
}