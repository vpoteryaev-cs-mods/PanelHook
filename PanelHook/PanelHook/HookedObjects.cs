using System.Collections.Generic;

namespace PanelHook
{
    class HookedObjects
    {
        private readonly Dictionary<object, string> _objects;

        internal HookedObjects()
        {
            _objects = new Dictionary<object, string>();
        }

        internal bool IsEmpty()
        {
            return (_objects.Count == 0) ? true : false;
        }

        internal bool IsHooked(object obj)
        {
            return _objects.ContainsKey(obj) ? true : false;
        }

        internal void AddObject(object obj, string desc)
        {
            if (IsHooked(obj))
                return;
            _objects.Add(obj, desc);
        }

        internal void RemoveObject(object obj)
        {
            if (!IsHooked(obj))
                return;
            _objects.Remove(obj);
        }

        internal IEnumerable<object> GetObjects()
        {
            return _objects.Keys;
        }

        internal string GetDescription(object obj)
        {
            _objects.TryGetValue(obj, out string desc);
            return desc;
        }
    }
}
