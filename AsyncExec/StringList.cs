using NAnt.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncExec
{
    [ElementName("strings")]
    public class StringList : LoopItems
    {
        #region Fields

        private StringItemTable _StringItems;

        #endregion

        #region Properties

        public StringItemTable StringItems
        {
            get
            {
                if (_StringItems == null)
                {
                    _StringItems = new StringItemTable();
                }
                return _StringItems;
            }
            set { _StringItems = value; }
        }

        [BuildElementArray("string", ElementType = typeof(StringItem))]
        public StringItem[] Strings
        {
            set
            {
                foreach (StringItem Item in value)
                {
                    this.StringItems.Add(Item.StringValue, Item);
                }
            }
        }

        #endregion

        public StringList()
        {
        }

        public StringList(params string[] args)
        {
            foreach (string @string in args)
            {
                StringItem Item = new StringItem(@string);
                this.StringItems.Add(Item.StringValue, Item);
            }
        }

        public void Clear()
        {
            this.StringItems = new StringItemTable();
        }

        #region Protected Methods

        protected override System.Collections.IEnumerator GetStrings()
        {
            if (!this.StringItems.Sorted)
            {
                return this.StringItems.GetOrderedEnumerator();
            }
            return this.StringItems.Values.GetEnumerator();
        }

        #endregion

    }
}
