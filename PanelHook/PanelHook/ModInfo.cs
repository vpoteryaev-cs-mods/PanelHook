using ICities;

namespace PanelHook
{
    public class ModInfo: IUserMod
    {
        public string Name => "Panel Hook";
        public string Description => "Allows to attach your own mouse click handler to any element derived from the UIComponent";
    }
}
