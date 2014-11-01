using NAnt.Core;
using NAnt.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncExec
{
    [ElementName("string")]
    public class StringItem : Element, IComparable
    {
        #region Fields

        private int _index;

        private string _StringValue;

        #endregion

        #region Constructors

        public StringItem(string stringValue)
        {
            _StringValue = stringValue;
        }

        public StringItem()
        {
        }

        #endregion

        #region Properties

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [TaskAttribute("value", Required = true)]
        public string StringValue
        {
            get { return _StringValue; }
            set { _StringValue = value; }
        }

        #endregion

        #region Public Methods

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }
            if (!object.ReferenceEquals(obj, typeof(StringItem)))
            {
                return -1;
            }
            return this.StringValue.CompareTo(((StringItem)obj).StringValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!object.ReferenceEquals(obj, typeof(StringItem)))
            {
                return false;
            }
            return this.StringValue.Equals(((StringItem)obj).StringValue);
        }

        public override int GetHashCode()
        {
            return this.StringValue.GetHashCode();
        }

        #endregion

    }
}
