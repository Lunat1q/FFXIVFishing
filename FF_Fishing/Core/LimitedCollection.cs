using System.Collections;
using System.Collections.Generic;

namespace FF_Fishing.Core
{
    public class LimitedCollection<T> : IEnumerable<T>
    {
        protected readonly T[] Storage;
        private int _current;
        private bool _full;
        private bool _first;

        public LimitedCollection(int size)
        {
            Storage = new T[size];
        }

        public void Add(T loc)
        {
            InternalAdd(loc);
        }

        protected virtual void InternalAdd(T loc)
        {
            Storage[FindLeastImportant()] = loc;
            _first = true;
        }

        internal T Latest => Storage[_first ? _current - 1 : 0];

        protected virtual int FindLeastImportant()
        {
            if (Storage.Length <= _current)
            {
                _current = 0;
                _full = true;
            }
            return _current++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = _current; _full && i < Storage.Length; i++)
            {
                yield return Storage[i];
            }

            for (var i = 0; i < _current; i++)
            {
                yield return Storage[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count()
        {
            if (_full)
            {
                return Storage.Length;
            }
            else
            {
                return _current;
            }
        }

        public void Clear()
        {
            for (var i = 0; i < Storage.Length; i++)
            {
                Storage[i] = default(T);
            }

            _first = false;
            _full = false;
        }
    }
}
