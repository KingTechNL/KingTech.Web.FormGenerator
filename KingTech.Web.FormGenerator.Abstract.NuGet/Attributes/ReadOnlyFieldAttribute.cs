using System;

namespace KingTech.Web.FormGenerator.Abstract
{

    /// <summary>
    /// If a big input type is available for this datatype (e.g. textarea instead of text) it will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyFieldAttribute : Attribute
    {
        
    }
}