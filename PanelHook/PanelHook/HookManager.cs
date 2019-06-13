using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using UnityEngine;

namespace PanelHook
{
    class HookManager
    {
        /*
         *
         *          Inteface
         *
        */

        //same as CilossalFramework.UI.MouseEventHandler
        //public delegate void pHandlerPointer(UIComponent sender, UIMouseEventParameter eventParam);

        public static bool IsHooked(MouseEventHandler handlerPointer, object obj)
        {
            return _hooksPtrDict.TryGetValue(handlerPointer, out HookedObjects objects) ? objects.IsHooked(obj) : false;
        }

        public static void AddHook(MouseEventHandler ptr, object obj, string desc)
        {
            if (_hooksPtrDict.TryGetValue(ptr, out HookedObjects hookedObjects))
            {
                if (hookedObjects.IsHooked(obj))
                {
                    Debug.LogErrorFormat("PanelHook: HookManager.AddHook() - The object \"{0}\" is already hooked by pointer \"{1}\"", ((UIComponent)obj).name, ptr);
                    return;
                }
                Debug.LogFormat("PanelHook: HookManager.AddHook() - add new object {0} to registered handler {1}", ((UIComponent)obj).name, ptr);
                hookedObjects.AddObject(obj, desc);
            }
            else
            {
                Debug.LogFormat("PanelHook: HookManager.AddHook() - add object {0} to handler {1}", ((UIComponent)obj).name, ptr);
                hookedObjects = new HookedObjects();
                hookedObjects.AddObject(obj, desc);
                _hooksPtrDict.Add(ptr, hookedObjects);
            }

            int numHooks;

            IEnumerable<object> query =
                from qHookedObjects in _hooksPtrDict.Values
                from qObjects in qHookedObjects.GetObjects()
                where qObjects.Equals(obj)
                select qObjects;
            numHooks = query.Count();
            Debug.LogFormat("PanelHook: HookManager.AddHook() - numHooks = {0}", numHooks);
            if (numHooks == 1)
                SetHandler(obj);
        }

        public static void RemoveHook(MouseEventHandler ptr, object obj)
        {
            int numHooks;

            IEnumerable<object> query =
                from qHookedObjects in _hooksPtrDict.Values
                from qObjects in qHookedObjects.GetObjects()
                where qObjects.Equals(obj)
                select qObjects;
            numHooks = query.Count();
            Debug.LogFormat("PanelHook: HookManager.RemoveHook() - numHooks = {0}", numHooks);
            if (numHooks == 1)
                RemoveHandler(obj);
            else if (numHooks == 0)
                return;

            if (_hooksPtrDict.TryGetValue(ptr, out HookedObjects hookedObjects))
            {
                if (hookedObjects.IsHooked(obj))
                {
                    hookedObjects.RemoveObject(obj);
                    if (hookedObjects.IsEmpty())
                        _hooksPtrDict.Remove(ptr);
                }
            }
        }

        /*
         *
         *          Internal stuff
         *
        */

        //todo: Custom UI Panel derived from UIPanel
        //see notes on GitHub project (https://github.com/vpoteryaev/CS-PanelHook/projects/1#card-22746629)
        //internal static PanelHook.UI.HooksPanel hooksPanel;

        static readonly Dictionary<MouseEventHandler, HookedObjects> _hooksPtrDict;

        //private static readonly bool _isLoaded;
        //public static bool IsLoaded
        //{
        //    get => _isLoaded;
        //}

        static HookManager()
        {
            _hooksPtrDict = new Dictionary<MouseEventHandler, HookedObjects>();
            //    _isLoaded = true;
        }

        internal static void OnCreated()
        {
            _hooksPtrDict.Clear();
        }

        internal static void OnLoaded()
        {
        //todo: create the panel instance (https://github.com/vpoteryaev/CS-PanelHook/projects/1#card-22746629)
        //hooksPanel = (PanelHook.UI.HooksPanel)UIView.GetAView().AddUIComponent(typeof(PanelHook.UI.HooksPanel));
        }

        internal static void OnUnloading()
        {
            //CleanUp all hooks
            IEnumerable<object> query =
                from qHookedObjects in _hooksPtrDict.Values
                from qObjects in qHookedObjects.GetObjects()
                select qObjects;
            foreach (object obj in query.Distinct())
                RemoveHandler(obj);

            //todo: HookPanel
            //release resources
            //if (hooksPanel != null)
            //    GameObject.Destroy(hooksPanel);
        }

        internal static void OnReleased()
        {
            _hooksPtrDict.Clear();
        }

        private static void SetHandler(object obj)
        {
            Debug.LogFormat("PanelHook: HookManager.SetHandler() - called for object {0}", ((UIComponent)obj).name);
            ((UIComponent)obj).eventClicked += MouseHandler;
        }

        private static void RemoveHandler(object obj)
        {
            Debug.LogFormat("PanelHook: HookManager.RemoveHandler() - called for object {0}", ((UIComponent)obj).name);
            ((UIComponent)obj).eventClicked -= MouseHandler;
        }

        private static void MouseHandler(UIComponent sender, UIMouseEventParameter eventParam)
        {
            Debug.LogFormat("PanelHook: HookManager.MouseHandler() - called for {0}", sender.name);

            //todo: HookPanel
            // - positioning
            // - actions management
            // - show panel
        }
    }
}
