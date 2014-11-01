using NAnt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncExec
{
    public abstract class LoopItems : DataTypeBase, IEnumerable
    {
        #region Public Methods

        public virtual void Executing(string item)
        {
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.GetStrings();
        }

        #endregion

        #region Protected Methods

        protected abstract IEnumerator GetStrings();

        #endregion

    }
}
