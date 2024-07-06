using KingTech.Web.FormGenerator.Abstract;
using Microsoft.AspNetCore.Components;

namespace KingTech.Web.FormGenerator.Areas;

/// <summary>
/// The TabControl component is order content on a page into tabs.
/// </summary>
public partial class TabControl : ComponentBase
{
    /// <summary>
    /// The content to render for the current TabPage.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Hide this tab in the TabControl.
    /// </summary>
    [Parameter]
    public EVisibilityMode VisibilityMode { get; set; }

    /// <summary>
    /// The currently active tab.
    /// </summary>
    internal TabPage? ActivePage { get; set; } = null;
    /// <summary>
    /// All tabs in this TabControl.
    /// </summary>
    private List<TabPage> Pages = new();

    /// <summary>
    /// Add a new tab to this TabControl.
    /// </summary>
    /// <param name="tabPage"></param>
    internal void AddPage(TabPage tabPage)
    {
        Pages.Add(tabPage);
        Pages = Pages.OrderBy(p => p.Position).ToList();

        if (ActivePage == null && !HideSection(tabPage.DataType, VisibilityMode))
            ActivePage = tabPage;
        StateHasChanged();
    }

    internal bool IsLastPage(TabPage page)
    {
        var index = Pages.FindIndex(p => p.Equals(page));
        var lastIndex = Pages.FindIndex(p => p.Equals(Pages.Last(p2 => !HideSection(p2.DataType, VisibilityMode))));
        return index == lastIndex;
    }

    internal bool IsFirstPage(TabPage page)
    {
        var index = Pages.FindIndex(p => p.Equals(page));
        var firstIndex = Pages.FindIndex(p => p.Equals(Pages.First(p2 => !HideSection(p2.DataType, VisibilityMode))));
        return index == firstIndex;
    }

    /// <summary>
    /// Used to highlight the button for the active tab.
    /// </summary>
    /// <param name="page">The page to get a button class for.</param>
    /// <returns>The css class for the button of the given page.</returns>
    private string GetButtonClass(TabPage page)
    {
        return page == ActivePage ? "btn-primary" : "btn-secondary";
    }

    /// <summary>
    /// Set the active page.
    /// </summary>
    /// <param name="page">The page to activate.</param>
    private void ActivatePage(TabPage page)
    {
        ActivePage = page;
        StateHasChanged();
    }

    /// <summary>
    /// Activate the next tab if possible.
    /// </summary>
    internal void ToNextTab()
    {
        if (!IsLastPage(ActivePage))
        {
            var index = Pages.FindIndex(p => p.Equals(ActivePage));
            var nextPage = Pages.Skip(index + 1).First(p => !HideSection(p.DataType, VisibilityMode));
            ActivatePage(nextPage);
        }
    }

    /// <summary>
    /// Activate previous tab if possible.
    /// </summary>
    internal void ToPreviousTab()
    {
        if (!IsFirstPage(ActivePage))
        {
            var index = Pages.FindIndex(p => p.Equals(ActivePage));
            var reversedPages = Pages.GetRange(0, index);
            reversedPages.Reverse();
            var nextPage = reversedPages.First(p => !HideSection(p.DataType, VisibilityMode));
            ActivatePage(nextPage);
        }
    }


    /// <summary>
    /// Whether to show the given section or not.
    /// </summary>
    /// <param name="dataType">The object type this section (form) represents.</param>
    /// <returns>True if the section needs to be shown, false otherwise.</returns>
    private bool HideSection(Type dataType, EVisibilityMode visibilityMode)
    {
        if (dataType == null)
            return true;
        var sectionInfo = dataType.FormInfo();
        var ret = sectionInfo != null && (sectionInfo.Skip || sectionInfo.Mode > visibilityMode);
        return ret;
    }
}