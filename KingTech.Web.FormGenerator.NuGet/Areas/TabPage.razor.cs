using Microsoft.AspNetCore.Components;

namespace KingTech.Web.FormGenerator.Areas;

/// <summary>
/// Single tab shown in TabControl.
/// </summary>
public partial class TabPage : ComponentBase
{
    /// <summary>
    /// The TabControl this page belongs to.
    /// </summary>
    [CascadingParameter]
    private TabControl Parent { get; set; }

    /// <summary>
    /// The content to show in this tab.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The title of this TabPage.
    /// Shown in button.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// The position of this tab in contrast to the other tabs.
    /// </summary>
    [Parameter]
    public ETabPosition Position { get; set; } = ETabPosition.Middle;

    [Parameter]
    public Type DataType { get; set; }

    /// <summary>
    /// This method is called upon creation of this object.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    protected override void OnInitialized()
    {
        if (Parent == null)
            throw new ArgumentNullException(nameof(Parent), "TabPage must exist within a TabControl");
        Parent.AddPage(this);
        base.OnInitialized();
    }

    /// <summary>
    /// Move to the next tab in parent TabControl.
    /// </summary>
    internal void ToNextTab() => Parent.ToNextTab();

    /// <summary>
    /// Move to previous tab in parent TabControl.
    /// </summary>
    internal void ToPreviousTab() => Parent.ToPreviousTab();

    /// <summary>
    /// Whether or not this is the first page in the tab control.
    /// </summary>
    /// <returns>True if this is the first page, false otherwise.</returns>
    internal bool IsFirst() => Parent.IsFirstPage(this);
    /// <summary>
    /// Whether or not this is the last page in the tab control.
    /// </summary>
    /// <returns>True if this is the last page, false otherwise.</returns>
    internal bool IsLast() => Parent.IsLastPage(this);
}

public enum ETabPosition
{
    Start = 0,
    Middle = 1,
    End = 2
}