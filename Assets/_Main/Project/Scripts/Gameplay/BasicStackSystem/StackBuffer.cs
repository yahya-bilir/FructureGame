using System.Collections.Generic;
using UnityEngine;

namespace BasicStackSystem
{
    public sealed class StackBuffer
    {
        private readonly List<IStackable> _items = new();

        public int Capacity { get; private set; }
        public int Count => _items.Count;
        public bool IsFull => Count >= Capacity;
        public bool IsEmpty => Count == 0;
        public IStackable this[int index] => _items[index];

        public StackBuffer(int capacity) { Capacity = Mathf.Max(0, capacity); }

        public void SetCapacity(int capacity)
        {
            Capacity = Mathf.Max(0, capacity);
        }

        public bool TryPush(IStackable s)
        {
            if (IsFull || s == null) return false;
            _items.Add(s);
            return true;
        }

        public bool TryRemove(IStackable s) => s != null && _items.Remove(s);

        public IStackable Pop()
        {
            if (IsEmpty) return null;
            int last = _items.Count - 1;
            var s = _items[last];
            _items.RemoveAt(last);
            return s;
        }

        public int IndexOf(IStackable s) => _items.IndexOf(s);
        public System.Collections.Generic.IEnumerable<IStackable> Items() => _items;
    }
}