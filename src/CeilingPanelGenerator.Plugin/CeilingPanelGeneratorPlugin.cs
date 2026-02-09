using Rhino.PlugIns;

namespace CeilingPanelGenerator.Plugin;

public sealed class CeilingPanelGeneratorPlugin : PlugIn
{
    public static CeilingPanelGeneratorPlugin? Instance { get; private set; }

    public CeilingPanelGeneratorPlugin()
    {
        Instance = this;
    }
}
