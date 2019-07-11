[![Build Status](https://dev.azure.com/vpoteryaev-cs-mods/PanelHook/_apis/build/status/vpoteryaev-cs-mods.PanelHook?branchName=master)](https://dev.azure.com/vpoteryaev-cs-mods/PanelHook/_build/latest?definitionId=1&branchName=master)
# Panel Hook (beta) for modders

## Forewords
When writing your own mod, too many time is spent on placing and precise positioning, let's say, buttons on the building information panel.
Then may happens that another mod also places something in the same panel, overlapping your changes ... or the developers have changed something ... etc.
As an example, when using very useful ModTools, it's annoys sometimes that the buttons and labels of the mod on the panels overlap with the other labels, and sometimes the interaction elements.

This mod will allow you to minimize your time-wasting, and unify the interaction of users with different mods, without changing anything in the panels.

## Purpose
With this mod, although called **Panel**Hook, you'll be able to attach your own mouse click handler to any element derived from the UIComponent.

You just have to define the element you are interested and provide the PanelHook with the following:
- Your handler - it's standard ColossalFramework.UI.MouseEventHandler.
- Element itself (mod accepts 'object' class type).
- The description that will be displayed to the user in the drop-down list for selection when clicking on an element.

As a result, when you click on an element, the panel will appear with the drop-down list of actions and the handler will be called accordingly after choosing action.
One handler can be hooked to different elements, also several different handlers can be attached to one element.
Only full duplication the pair handler-element is not allowed.

The followed functions provide an interface:
```
public static bool IsHooked(MouseEventHandler handlerPointer, object obj)    - check that the couple handler-element has already been registered;
public static void AddHook(MouseEventHandler ptr, object obj, string desc)   - hooking itself 
public static void RemoveHook(MouseEventHandler ptr, object obj)             - unregistering (see the comment inside Cleanup() function of the example)
```
AddHook() function has overrided version:
```
public static void AddHook(MouseEventHandler ptr, object obj, string desc, Func<bool> checker) 
```
**checker** adds ability to filter handler calls. For example, the handler is hooked to the service building panel's element, but should not be called for all buildings of this type, but only for those that suits the certain criteria.
This function is called each time before displaying the list of actions when clicking on an element.
If this function returns false, then the handler will not appear in the action's list.

The following example fully demonstrates the registration of a handler:
```
using System;
using ICities;
using ColossalFramework.UI;
using UnityEngine;
using PanelHook;

namespace MyMod
{
    public class MyMod1: LoadingExtensionBase
    {
        private MouseEventHandler handler1;
        private const string descr1 = "Mod 1 --- Action 1";

        private MouseEventHandler handler2;
        private const string descr2 = "Mod 1 --- Action 2";

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Setup();
        }

        public override void OnLevelUnloading()
        {
            Cleanup();
            base.OnLevelUnloading();
        }

        private void Setup()
        {
            //look for element - "InfoGroupPanel" on "ZonedBuildingWorldInfoPanel"
            UIComponent ui_object = (UIPanel)UIView.library.Get("ZonedBuildingWorldInfoPanel");
            var infoGroupPanel = ui_object?.Find<UIPanel>("InfoGroupPanel");

            //simple handler itself
            handler1 = (sender, e) =>
            {
                Debug.LogFormat("PanelHook: Mod 1 --- Action 1: I'm called from {0}", sender.name);
            };

            //check that the couple handler-element has not already been registered
            if (!HookManager.IsHooked(handler1, infoGroupPanel))
            {
                //hooking
                HookManager.AddHook(handler1, infoGroupPanel, descr1);
            }

            handler2 = (sender, e) =>
            {
                Debug.LogFormat("PanelHook: Mod 1 --- Action 2: I'm called from {0}", sender.name);
            };

            if (!HookManager.IsHooked(handler2, infoGroupPanel))
            {
                HookManager.AddHook(handler2, infoGroupPanel, descr2, MyChecker);
            }
        }

        private bool MyChecker()
        {
            ...
            if (some_defined_by_you_conditions)
                return true;
            return false;
        }

        private void Cleanup()
        {
            // These steps:
            // if (HookManager.IsHooked(testItem1Handler, infoGroupPanel))
            // {
            //     HookManager.RemoveHook(testItem1Handler, infoGroupPanel);
            // }
            // ...
            // are not required, all cleaning stuff is performed by HookManager while unloading the game.
            // Removing the hooks by yourself may be useful before saving the game
            // if your mod is planned to be disabled or unsubscribed.
        }
    }
}
```
## Uninstall
Just in case. To be sure that before uninstalling the mod data will not remain in the game save, before saving you should press the "Clean Data" button in the mod options, save, exit and delete the mod


All feedbacks and comments are welcome.

Want to encourage? [PayPal](https://www.paypal.me/vpoteryaev). THANKS in advance!!! ;)

Sorry for my English.
