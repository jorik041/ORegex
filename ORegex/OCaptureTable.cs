﻿using System.Collections;
using System.Collections.Generic;

namespace Eocron
{
    public sealed class OCaptureTable<TValue> : IEnumerable<KeyValuePair<string, List<OCapture<TValue>>>>
    {
        private static readonly OCapture<TValue>[] EmptyArray = new OCapture<TValue>[0];
        private static readonly Dictionary<string, List<OCapture<TValue>>> EmptyCaptures = new Dictionary<string, List<OCapture<TValue>>>();

        private Dictionary<string, List<OCapture<TValue>>> _captures;

        public int Count
        {
            get { return _captures == null ? 0 : _captures.Count; }
        }
        public IEnumerable<OCapture<TValue>> this[string name]
        {
            get
            {
                if (_captures == null)
                {
                    return EmptyArray;
                }
                return _captures[name];
            }
        }

        internal void Add(string name, OCapture<TValue> capture)
        {
            if (_captures == null)
            {
                _captures = new Dictionary<string, List<OCapture<TValue>>>();
            }
            List<OCapture<TValue>> list;
            if (!_captures.TryGetValue(name, out list))
            {
                list = new List<OCapture<TValue>>();
                _captures.Add(name,list);
            }
            list.Add(capture);
        }

        internal void Remove(string name)
        {
            if (_captures == null)
            {
                return;
            }
            _captures.Remove(name);
        }

        internal void Add(OCaptureTable<TValue> table)
        {
            if (table._captures != null)
            {
                foreach (var c in table._captures)
                {
                    foreach (var v in c.Value)
                    {
                        Add(c.Key, v);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<string, List<OCapture<TValue>>>> GetEnumerator()
        {
            if (_captures == null)
            {
                return EmptyCaptures.GetEnumerator();
            }
            return _captures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}