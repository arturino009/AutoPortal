using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace AutoPortal;

public class AutoPortalSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public HotkeyNode PortalHotkey { get; set; } = new HotkeyNode(Keys.A);

    [Menu("Auto portal on map completion")]
    public ToggleNode AutoComplete { get; set; } = new ToggleNode(false);
}