using System.Collections.Generic;

namespace KingTech.Web.FormGenerator.Abstract
{

    /// <summary>
    /// Interface required for generic forms to be generated.
    /// </summary>
    public interface IBaseForm
    {
        /// <summary>
        /// Verifies the form when submitted.
        /// </summary>
        /// <param name="errorList">The list of errors that resulted in a verify failure</param>
        /// <returns>boolean indicating success</returns>
        bool Verify(ref IList<string> errorList);
    }
}