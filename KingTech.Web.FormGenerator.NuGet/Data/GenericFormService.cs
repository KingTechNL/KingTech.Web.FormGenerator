using KingTech.Web.FormGenerator.Abstract;
using Microsoft.Extensions.Logging;

namespace KingTech.Web.FormGenerator.Data;

/// <summary>
/// The FormService serves as the backbone of the FormGenerator.
/// Here most of the background logic for handling the form is implemented.
/// </summary>
public class GenericFormService
{
    /// <summary>
    /// Setup method selected for configuring this service.
    /// </summary>
    internal FormSetup Setup { get; private set; } = new();

    /// <summary>
    /// The visibility mode (basic/advanced) to generate forms for.
    /// </summary>
    internal EVisibilityMode VisibilityMode => Setup.SetupType;
    /// <summary>
    /// Callback to inform services of a change to the VisibilityMode.
    /// </summary>
    internal event Action<EVisibilityMode> VisibilityModeChanged;

    /// <summary>
    /// Keys (section:settingName) listing for all readonly settings)
    /// </summary>
    internal IEnumerable<string>? ReadonlySettingKeys { get; private set; }

    private readonly ILogger<GenericFormService> _logger;

    /// <summary>
    /// Form service serves as backbone of the FormGenerator.
    /// </summary>
    /// <remarks>Constructor is internal as users are supposed to use it using the FormGenerator extension methods.</remarks>
    /// <param name="logger"></param>
    public GenericFormService(ILogger<GenericFormService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// If set to Advanced mode, the form generator will no longer hide fields marked as 'advanced'.
    /// </summary>
    /// <param name="visibilityMode">The visibility mode to set the FormGenerator to (e.g. Basic, Advanced).</param>
    public void SetVisibilityMode(EVisibilityMode visibilityMode)
    {
        Setup.SetupType = visibilityMode;
        _logger?.LogDebug("VisibilityMode set to {visibility}", Setup.SetupType.ToString());
        
        VisibilityModeChanged?.Invoke(Setup.SetupType);
    }

    /// <summary>
    /// Set the generator setup.
    /// </summary>
    /// <remarks>This method is for test purposes only.</remarks>
    /// <param name="setup">The new <see cref="FormSetup"/>.</param>
    internal void SetFormSetup(FormSetup setup)
    {
        Setup = setup;
    }

    public void AddModeListener(FormSetup.IVisibilityModeListener visibilityModeListener)
    {
        Setup.AddListener(visibilityModeListener);
    }
}