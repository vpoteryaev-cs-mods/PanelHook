using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;
using System;

namespace PanelHook
{
    public class HookManager
    {
        /*
         *
         *          Inteface
         *
        */

        public static bool IsHooked(MouseEventHandler handlerPointer, object obj)
        {
            return _hooksPtrDict.TryGetValue(handlerPointer, out HookedObjects objects) ? objects.IsHooked(obj) : false;
        }

        public static void AddHook(MouseEventHandler ptr, object obj, string desc)
        {
            _AddHook(ptr, obj, desc);
        }

        public static void AddHook(MouseEventHandler ptr, object obj, string desc, Func<bool> checker)
        {
            _AddHook(ptr, obj, desc, checker);
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
#if DEBUG
            Debug.LogFormat("PanelHook: HookManager.RemoveHook() - numHooks = {0}", numHooks);
#endif
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
                    {
                        _hooksPtrDict.Remove(ptr);
                        _checkersDict.Remove(ptr);
                    }
                }
            }
        }

        /*
         *
         *          Internal stuff
         *
        */

        internal static PanelHook.UI.HooksPanel hooksPanel;

        private static readonly Dictionary<MouseEventHandler, HookedObjects> _hooksPtrDict;
        private static readonly Dictionary<MouseEventHandler, Func<bool>> _checkersDict;

        static HookManager()
        {
            _hooksPtrDict = new Dictionary<MouseEventHandler, HookedObjects>();
            _checkersDict = new Dictionary<MouseEventHandler, Func<bool>>();
        }

        internal static void OnCreated()
        {
            _hooksPtrDict.Clear();
            _checkersDict.Clear();
        }

        internal static void OnLoaded()
        {
            hooksPanel = (PanelHook.UI.HooksPanel)UIView.GetAView().AddUIComponent(typeof(PanelHook.UI.HooksPanel));
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

            if (hooksPanel != null)
                GameObject.Destroy(hooksPanel);
        }

        internal static void OnReleased()
        {
            _hooksPtrDict.Clear();
            _checkersDict.Clear();
        }

        private static void _AddHook(MouseEventHandler ptr, object obj, string desc, Func<bool> checker = null)
        {
            if (_hooksPtrDict.TryGetValue(ptr, out HookedObjects hookedObjects))
            {
                if (hookedObjects.IsHooked(obj))
                {
#if DEBUG
                    Debug.LogErrorFormat("PanelHook: HookManager.AddHook() - The object \"{0}\" is already hooked by pointer \"{1}\"", ((UIComponent)obj).name, ptr);
#endif
                    return;
                }
#if DEBUG
                Debug.LogFormat("PanelHook: HookManager.AddHook() - add new object {0} to registered handler {1}", ((UIComponent)obj).name, ptr);
#endif
                hookedObjects.AddObject(obj, desc);
            }
            else
            {
#if DEBUG
                Debug.LogFormat("PanelHook: HookManager.AddHook() - add object {0} to handler {1}", ((UIComponent)obj).name, ptr);
#endif
                hookedObjects = new HookedObjects();
                hookedObjects.AddObject(obj, desc);
                _hooksPtrDict.Add(ptr, hookedObjects);
                if(checker != null)
                    _checkersDict.Add(ptr, checker);
            }

            int numHooks;

            IEnumerable<object> query =
                from qHookedObjects in _hooksPtrDict.Values
                from qObjects in qHookedObjects.GetObjects()
                where qObjects.Equals(obj)
                select qObjects;
            numHooks = query.Count();
#if DEBUG
            Debug.LogFormat("PanelHook: HookManager.AddHook() - numHooks = {0}", numHooks);
#endif
            if (numHooks == 1)
                SetHandler(obj);
        }

        private static void SetHandler(object obj)
        {
#if DEBUG
            Debug.LogFormat("PanelHook: HookManager.SetHandler() - called for object {0}", ((UIComponent)obj).name);
#endif
            ((UIComponent)obj).eventClicked += MouseHandler;
        }

        private static void RemoveHandler(object obj)
        {
#if DEBUG
            Debug.LogFormat("PanelHook: HookManager.RemoveHandler() - called for object {0}", ((UIComponent)obj).name);
#endif
            ((UIComponent)obj).eventClicked -= MouseHandler;
        }

        private static void MouseHandler(UIComponent sender, UIMouseEventParameter eventParam)
        {
#if DEBUG
            Debug.LogFormat("PanelHook: HookManager.MouseHandler() - called for {0}", sender.name);
#endif
            if (hooksPanel.isVisible) return;

            //note: improve positioning
            hooksPanel.pivot = sender.pivot;
            hooksPanel.position = sender.position;

            bool checksPassed = false;
            foreach (KeyValuePair<MouseEventHandler, HookedObjects> kvPair in _hooksPtrDict)
            {
                if (kvPair.Value.IsHooked(sender))
                {
                    if(_checkersDict.TryGetValue(kvPair.Key, out Func<bool> checker))
                    {
                        if(!checker())
                            continue;
                    }
                    checksPassed = true;
                    hooksPanel.AddAction(kvPair.Key, kvPair.Value.GetDescription(sender), sender, eventParam);
                }
            }
            if(checksPassed)
            {
                hooksPanel.Show();
            }
        }

        internal static void CleanData()
        {
            OnUnloading();
            OnReleased();
        }
    }
}
