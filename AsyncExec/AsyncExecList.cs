using System;
using System.Collections;


namespace AsyncExec
{
    public class AsyncExecList
    {
        #region Fields

        private static Hashtable _taskNames;

        #endregion

        #region Constructors

        static AsyncExecList()
        {
        }

        #endregion

        #region Properties

        private static Hashtable TaskNames
        {
            get
            {
                if (_taskNames == null)
                {
                    _taskNames = new Hashtable();
                }
                return _taskNames;
            }
            set { _taskNames = value; }
        }

        #endregion

        #region Public Methods

        public static void Add(string name, AsyncExec task)
        {
            TaskNames.Add(name, task);
        }

        public static AsyncExec GetTask(string name)
        {
            return (AsyncExec)TaskNames[name];
        }

        public static void Remove(string name)
        {
            TaskNames.Remove(name);
        }

        #endregion

    }
}
