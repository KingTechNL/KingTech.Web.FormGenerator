using KingTech.Web.FormGenerator.Abstract;

namespace KingTech.Web.FormGenerator.Data;

/// <summary>
/// Model containing information about the setup process.
/// </summary>
public class FormSetup
{
    private readonly List<IVisibilityModeListener> _listeners = new();

    private EVisibilityMode _setupType;
    /// <summary>
    /// The setup type (basic, advanced, ...)
    /// </summary>
    public EVisibilityMode SetupType
    {
        get => _setupType;

        set
        {
            _setupType = value;
            foreach (var visibilityModeListener in _listeners)
            {
                visibilityModeListener.VisibilityModeChanged(value);
            }
        }
    }

    public void AddListener(IVisibilityModeListener listener)
    {
        _listeners.Add(listener);
    }

    public void RemoveListener(IVisibilityModeListener listener)
    {
        _listeners.Remove(listener);
    }

    public interface IVisibilityModeListener
    {
        void VisibilityModeChanged(EVisibilityMode value);
    }
}