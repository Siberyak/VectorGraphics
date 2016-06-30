using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Shapes
{
    public class KeysInfo
    {
        List<Keys> _keys = new List<Keys>();

        public bool Empty
        {
            get { return !_keys.Any(); }
        }

        public int Count { get { return _keys.Count; } }

        public override string ToString()
        {
            return string.Join(",", _keys);
        }

        internal void Reset()
        {
            Reset(new List<Keys>());
        }

        internal void Reset(List<Keys> list)
        {
            _keys = list;
        }

        public bool Contains(Keys keyCode)
        {
            return _keys.Contains(keyCode);
        }

        internal bool Add(Keys keyCode)
        {
            if (Contains(keyCode))
                return false;

            _keys.Add(keyCode);
            return true;
        }

        internal bool Remove(Keys keyCode)
        {
            return _keys.Remove(keyCode);
        }

        public static implicit operator Keys(KeysInfo info)
        {
            return info == null || info._keys.Count != 1
                ? Keys.None
                : info._keys[0];
        }

        public static implicit operator Keys[](KeysInfo info)
        {
            return info == null
                ? new Keys[0]
                : info._keys.ToArray();
        }

        public bool Is(params Keys[] keyCodes)
        {
            return _keys.Count == keyCodes.Length && !_keys.Except(keyCodes).Any();
        }

        //public bool ContainsAll(params Keys[] keyCodes)
        //{
        //    return !keyCodes.Except(_keys).Any();
        //}

        public bool ContainsOnlyAny(params Keys[] keyCodes)
        {
            return !_keys.Except(keyCodes).Any();
        }
    }
}