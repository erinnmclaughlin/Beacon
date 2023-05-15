using BeaconUI.Core;

namespace BeaconUI.WebApp;

public class PlatformInfo : IPlatformInfo
{
    public string GetPlatformName()
    {
        return "Browser";
    }
}
