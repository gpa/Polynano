/*
MIT License

Copyright(c) 2018 Gratian Pawliszyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Polynano.Processing.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Polynano.Processing.Core.Collections
{
    public class SoftDeleteList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TKey : IMeshElementRef, new()
        where TValue : struct, IDeleted
    {
        private readonly List<TValue> _values;

        public TValue this[TKey key]
        {
            get
            {
                return _values[key.Index];
            }
            set
            {
                if (_values[key.Index].IsDeleted())
                    throw new InvalidOperationException($"The element with the key {key} is currently marked as deleted.");

                _values[key.Index] = value;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                for (int i = 0; i < _values.Count; ++i)
                {
                    if (!_values[i].IsDeleted())
                    {
                        yield return new TKey
                        {
                            Index = i
                        };
                    }
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var key in Keys)
                    yield return _values[key.Index];
            }
        }

        public int Count { get; private set; }

        public SoftDeleteList(int capacity)
        {
            _values = new List<TValue>(capacity);
            Count = 0;
        }

        public TKey Add(TValue value)
        {
            TKey key = new TKey()
            {
                Index = Count
            };

            _values.Add(value);
            Count++;

            return key;
        }

        public void Remove(TKey key)
        {
            var oldValue = _values[key.Index];
            if (oldValue.IsDeleted())
                throw new InvalidOperationException("The requested key is already deleted");

            _values[key.Index] = (TValue)_values[key.Index].GetDeletedClone();
            Count--;
        }

        public void Respawn(TKey key, TValue value)
        {
            if (!_values[key.Index].IsDeleted())
                throw new InvalidOperationException($"{nameof(Respawn)} call is not valid because the given item is not deleted.");

            _values[key.Index] = value;
            Count++;
        }

        public bool ContainsKey(TKey key)
        {
            return !_values[key.Index].IsDeleted();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                yield return new KeyValuePair<TKey, TValue>(key, _values[key.Index]);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!_values[key.Index].IsDeleted())
            {
                value = _values[key.Index];
                return true;
            }

            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}