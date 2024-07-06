using Microsoft.AspNetCore.Components;
using TabBlazor;

namespace KingTech.Web.FormGenerator.Areas.GenericForm.AdvancedInputFields;

/// <summary>
/// This class shows a modal containing a given message.
/// </summary>
public partial class PopupMessage
{
    /// <summary>
    /// The message to show.
    /// </summary>
    [Parameter]
    public string Message { get; set; }

    /// <summary>
    /// The text in the confirm button.
    /// </summary>
    [Parameter]
    public string ConfirmButton { get; set; } = "Yes";

    /// <summary>
    /// The text in the cancel button.
    /// </summary>
    [Parameter]
    public string CancelButton { get; set; } = "No";

    /// <summary>
    /// optional: A list of errors to display in the popup.
    /// </summary>
    [Parameter]
    public List<string> Errors { get; set; }

    /// <summary>
    /// This method is called when the 'confirm' button is pressed.
    /// It closes the popup with an 'OK' flag.
    /// </summary>
    private void Confirm()
    {
        _modalService.Close(ModalResult.Ok());
    }

    /// <summary>
    /// This method is called when the 'cancel' button is pressed.
    /// It closes the popup with an 'Cancelled' flag.
    /// </summary>
    private void Cancel()
    {
        _modalService.Close(ModalResult.Cancel());
    }
}