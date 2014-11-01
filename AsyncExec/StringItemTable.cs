using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncExec
{
    public class StringItemTable : DictionaryBase
    {
        #region Fields

        private bool _changed;

        private SortedDictionary<int, string> _IndexedTable;

        private bool _sorted;

        private ArrayList _values;

        #endregion

        #region Properties

        private bool Changed
        {
            get { return _changed; }
            set { _changed = value; }
        }

        public SortedDictionary<int, string> IndexedTable
        {
            get
            {
                if (_IndexedTable == null)
                {
                    _IndexedTable = new SortedDictionary<int, string>();
                }
                return _IndexedTable;
            }
            set { _IndexedTable = value; }
        }

        private ArrayList InnerValues
        {
            get
            {
                if (this._values == null || this.Changed)
                {
                    this._values = new ArrayList(this.InnerHashtable.Keys);
                }
                return _values;
            }
        }

        public bool Sorted
        {
            get { return _sorted; }
            set { _sorted = value; }
        }

        public string[] Values
        {
            get { return (string[])this.InnerValues.ToArray(typeof(string)); }
        }

        #endregion

        #region Public Methods

        public void Add(string key, StringItem value)
        {
            this.InnerHashtable.Add(key, value);
            value.Index = this.Count;
            this.IndexedTable.Add(value.Index, value.StringValue);
        }

        public bool Contains(string key)
        {
            return this.InnerHashtable.Contains(key);
        }

        public System.Collections.IEnumerator GetOrderedEnumerator()
        {
            return this.IndexedTable.Values.GetEnumerator();
        }

        public void Remove(string key)
        {
            int Index = ((StringItem)this.InnerHashtable[key]).Index;
            this.IndexedTable.Remove(Index);
            this.InnerHashtable.Remove(key);
        }

        public void ReverseSort()
        {
            this.InnerValues.Sort();
            this.InnerValues.Reverse();
            this.Sorted = true;
        }

        public void Sort()
        {
            this.InnerValues.Sort();
            this.Sorted = true;
        }

        #endregion

        #region Protected Methods

        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            this.Changed = true;
        }

        protected override void OnClear()
        {
            base.OnClear();
            this.Changed = true;
        }

        protected override void OnInsert(object key, object value)
        {
            base.OnInsert(key, value);
            this.Changed = true;
        }

        protected override void OnInsertComplete(object key, object value)
        {
            base.OnInsertComplete(key, value);
            this.Changed = true;
        }

        protected override void OnRemoveComplete(object key, object value)
        {
            base.OnRemoveComplete(key, value);
            this.Changed = true;
        }

        protected override void OnRemove(object key, object value)
        {
            base.OnRemove(key, value);
            this.Changed = true;
        }

        protected override void OnSetComplete(object key, object oldValue, object newValue)
        {
            base.OnSetComplete(key, oldValue, newValue);
            this.Changed = true;
        }

        protected override void OnSet(object key, object oldValue, object newValue)
        {
            base.OnSet(key, oldValue, newValue);
            this.Changed = true;
        }

        #endregion

    }
}
