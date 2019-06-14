using ICities;

namespace PanelHook.UI
{
    class Settings
    {
        private const string _info =
            "Before deleting this mod push \"Clean data\" button,\n" +
            "save the game, exit and unsubscribe from \'PanelHook\'";
        internal static void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup("Cleanup");
            group.AddButton("Clean data", () => { HookManager.CleanData(); });
            helper.AddGroup(_info);
        }
    }
}
