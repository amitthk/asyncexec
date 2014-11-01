using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core.Attributes;
using NAnt.Core;


namespace AsyncExec
{
    [TaskName("waitforexitasync")]
    public class WaitForExitAsync : Task
    {
        #region Fields

        private StringList _taskNames;

        #endregion

        #region Properties

        [BuildElement("tasknames", Required = true)]
        public StringList TaskNames
        {
            get { return _taskNames; }
            set { _taskNames = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            AsyncExec Worker;
            foreach (string TaskName in this.TaskNames.StringItems.Values)
            {
                Worker = AsyncExecList.GetTask(TaskName);
                if (Worker != null)
                {
                    Worker.Wait();
                    AsyncExecList.Remove(TaskName);
                    if (IsLogEnabledFor(Level.Info))
                    {
                        Log(Level.Info, string.Format("Completed task: [0]",TaskName));
                    }
                }
            }
        }

        #endregion

    }

}