using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHashMape
{
    public class MyHashMap<TKey, TValue> 
    {
        public class Entry<TK, TV>
        {
            public TK Key { get; }
            public TV Value { get; set; }
            public Entry<TK, TV> Next { get; set; }
            public Entry(TK key, TV value) { Key = key; Value = value; }
        }
        private int _size ;
        private Entry<TKey, TValue>[] _table;
        public MyHashMap(int size)
        {
            _size = size;
            _table = new Entry<TKey, TValue>[_size];
        }
        public void Put(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int index = GetIndex(key);
            if (CheckEntry(index))
            {
                InsertAtRoot(key, value, index);
                return;
            }
            Entry<TKey, TValue> current = _table[index];
            Entry<TKey, TValue> NextEntry = new Entry<TKey, TValue>(key,value);
            UpdateOrInsert(NextEntry, current); 
        }
        private void UpdateOrInsert(Entry<TKey, TValue> nextEntry , Entry<TKey, TValue> currentEntry)
        {
            while (true)
            {
                if (nextEntry.Key.Equals(currentEntry.Key))
                {
                    currentEntry.Value = nextEntry.Value;
                    return;
                }
                else if (currentEntry.Next == null)
                {
                    currentEntry.Next = nextEntry;
                    return;
                }
                currentEntry = currentEntry.Next;
            }
        }
        public int GetIndex(TKey key)
        {
            int index = Math.Abs(key.GetHashCode() % _size);
            return index;
        }
        public bool CheckEntry(int index)
        {
            Entry<TKey, TValue> current = _table[index];
            if(current== null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Entry<TKey, TValue> GetEntry(int index)
        {
            return _table[index];
        }

        private void InsertAtRoot(TKey key , TValue value , int index)
        {
            _table[index] = new Entry<TKey, TValue>(key, value);
        }
        public TValue GetValueFromKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int index = GetIndex(key);
            Entry<TKey, TValue> current = GetEntry(index);
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    return current.Value;
                }
                current = current.Next;
            }
            throw new KeyNotFoundException("The key was not present in the bucket.");
        }
        public TValue RemoveEntry(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = GetIndex(key);
            Entry<TKey, TValue> root = _table[index];
            if (root == null) throw new KeyNotFoundException();
            if (key.Equals(root.Key))
            {
                return RemoveRoot(index);
            }
            return RemoveFromChain(root, key);
        }
        private TValue RemoveRoot(int index)
        {
            Entry<TKey, TValue> target = _table[index];
            _table[index] = target.Next;
            TValue value = target.Value;
            target.Next = null;
            return value;
        }
        private TValue RemoveFromChain(Entry<TKey, TValue> root, TKey key)
        {
            Entry<TKey, TValue> prev = root;
            Entry<TKey, TValue> current = root.Next;

            while (current != null)
            {
                if (key.Equals(current.Key))
                {
                    prev.Next = current.Next;
                    TValue value = current.Value;
                    current.Next = null;
                    return value;
                }
                prev = current;
                current = current.Next;
            }
            throw new KeyNotFoundException();
        }
    }
    
}
