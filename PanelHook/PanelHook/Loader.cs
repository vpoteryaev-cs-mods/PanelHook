﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System.Reflection;
using UnityEngine;

namespace PanelHook
{
    public class Loader: LoadingExtensionBase
    {
        private bool initialized;

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
            //todo: HookManager onCreated
            //HookManager.OnCreated();
        }

        private void LoadedSetup()
        {
            //todo: HookManager onLoaded
            //HookManager.OnLoaded();
            initialized = true;
        }

        private void UnloadedCleanup()
        {
            if (initialized)
            {
                //todo: HookManager unloading
                //HookManager.OnUnloading();
            }
        }

        private void ReleasedCleanup()
        {
            //todo: HookManager onRelease
            //HookManager.OnRelease();
        }
    }
}
