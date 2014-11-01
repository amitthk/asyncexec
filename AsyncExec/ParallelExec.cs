using System;
using System.Threading.Tasks;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;

namespace AsyncExec
{
    [TaskName("parallelexec")]
    public class ParallelExecTask : TaskContainer
    {
        [BuildElementArray("exec", Required = true, ElementType = typeof(ExecTask))]
        public ExecTask[] ExecTasks { get; set; }

        [TaskAttribute("threadcount")]
        public int ThreadCount { get; set; }

        protected override void ExecuteTask()
        {
            var parallelOptions = new ParallelOptions();

            if (ThreadCount > 0)
            {
                parallelOptions.MaxDegreeOfParallelism = ThreadCount;
                Log(Level.Verbose, string.Format("Executing in parallel using at most {0} threads...", ThreadCount));
            }
            else
            {
                Log(Level.Verbose, string.Format("Executing in parallel using at most {0} threads...", ThreadCount));
            }

            try
            {
                Parallel.ForEach(ExecTasks, parallelOptions, Body);
            }
            catch (AggregateException e)
            {
                foreach (Exception innerException in e.InnerExceptions)
                {
                    if (innerException is BuildException)
                        Log(Level.Error, innerException.Message);
                    else
                        throw innerException;
                }
                throw new BuildException("Parallel execution failed for " + e.InnerExceptions.Count + " of " + ExecTasks.Length + " commands executions (see the above log).", Location);
            }
        }

        private void Body(ExecTask execTask)
        {
            try
            {
                execTask.Execute();
            }
            catch (BuildException e)
            {
                throw new BuildException("External Program Failed: " + execTask.ProgramFileName + " " + execTask.CommandLine + " (return code was " + execTask.ExitCode + ")", e);
            }
        }
    }
}