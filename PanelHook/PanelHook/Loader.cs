using ICities;

namespace PanelHook
{
    public class Loader: LoadingExtensionBase
    {
        internal static bool initialized = false;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            CreatedSetup();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.NewGame &&
                mode != LoadMode.NewGameFromScenario &&
                mode != LoadMode.LoadGame)
                return;

            LoadedSetup();
        }

        public override void OnLevelUnloading()
        {
            UnloadedCleanup();

            base.OnLevelUnloading();
        }

        public override void OnReleased()
        {
            ReleasedCleanup();

            base.OnReleased();
        }

        private void CreatedSetup()
        {
            HookManager.OnCreated();
        }

        private void LoadedSetup()
        {
            HookManager.OnLoaded();
            initialized = true;
            UI.Settings.CleanButton.isEnabled = true;
        }

        private void UnloadedCleanup()
        {
            if (initialized)
            {
                HookManager.OnUnloading();
            }
        }

        private void ReleasedCleanup()
        {
            HookManager.OnReleased();
        }
    }
}
