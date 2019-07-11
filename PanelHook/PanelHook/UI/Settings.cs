using ICities;
using ColossalFramework.UI;

namespace PanelHook.UI
{
    class Settings
    {
        internal static UIButton CleanButton { get; private set; }

        private const string _info =
            "Before deleting this mod push \"Clean data\" button,\n" +
            "save the game, exit and unsubscribe from \'PanelHook\'";
        internal static void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup("Cleanup");
            CleanButton = (UIButton)group.AddButton("Clean data", () => { HookManager.CleanData(); });
            CleanButton.isEnabled = Loader.initialized;
            helper.AddGroup(_info);
        }
    }
}
