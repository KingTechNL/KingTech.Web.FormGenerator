using System;

namespace KingTech.Web.FormGenerator.Abstract
{

    /// <summary>
    /// A custom attribute adding additional information to a class to generate a form for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormInfoAttribute : Attribute
    {
        /// <summary>
        /// The minimum settings mode required to configure this section using the FormGenerator.
        /// </summary>
        public EVisibilityMode Mode { get; }

        /// <summary>
        /// Description for this setting section.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// If set, this section will be ignored by the FormGenerator.
        /// </summary>
        public bool Skip { get; }

        /// <summary>
        /// A custom attribute adding additional information to a settings class used by the FormGenerator.
        /// </summary>
        /// <param name="description">Description for this setting section.</param>
        /// <param name="mode">The minimum settings mode required to configure this section using the FormGenerator.</param>
        /// <param name="skip">If set, this section will be ignored by the FormGenerator.</param>
        public FormInfoAttribute(string description, EVisibilityMode mode = EVisibilityMode.Basic, bool skip = false)
        {
            Mode = mode;
            Description = description;
            Skip = skip;
        }

        /// <summary>
        /// A custom attribute adding additional information to a settings class used by the FormGenerator.
        /// </summary>
        /// <param name="skip">If set, this section will be ignored by the FormGenerator.</param>
        public FormInfoAttribute(bool skip = true) : this(string.Empty, skip: skip) { }
    }

    /// <summary>
    /// By setting this attribute on your settings class, the FormGenerator will not generate a form for it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SkipFormAttribute : FormInfoAttribute
    {
        /// <summary>
        /// By setting this attribute on your settings class, the FormGenerator will not generate a form for it.
        /// </summary>
        public SkipFormAttribute() : base(true) { }
    }
}