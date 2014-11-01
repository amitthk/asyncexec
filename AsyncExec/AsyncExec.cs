using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


    namespace AsyncExec
    {
        /// <summary>
        /// Asynchrously executes a child process.
        /// </summary>
        /// <example>
        ///   <para>Ping "cifactory.org" and wait, also ping "cifactory.com" without waiting.</para>
        ///   <code>
        ///     <![CDATA[
        /// <asyncexec 
        ///   program="ping"
        ///   commandline="cifactory.org"
        ///   taskname="PingTask"
        ///   resultproperty="PingExitCode"
        ///   failonerror="false"
        ///   outputproperty="PingOutput"/>
        /// 
        /// <asyncexec 
        ///   program="ping" 
        ///   commandline="-t cifactory.com" 
        ///   waitforexit="false" 
        ///   useshellexecute="true" 
        ///   createnowindow="false" 
        ///   redirectoutput="false" />
        /// 
        /// <waitforexit>
        ///   <tasknames>
        ///     <string value="PingTask"/>
        ///   </tasknames>
        /// </waitforexit>
        /// 
        /// <echo message="The exit code for pinging cifactory.org was ${PingExitCode}."/>
        /// <echo message="${PingOutput}"/>
        ///     ]]>
        ///   </code>
        /// </example>
        [TaskName("asyncexec")]
        public class AsyncExec : ExecTask
        {
            #region Fields

            private Process _Process;

            private string _taskName = string.Empty;

            private bool _waitForExit = true;
            private string _OutputProperty;
            private bool _redirectOutput=true;
            protected StreamReader _stdError;
            protected StreamReader _stdOut;
            protected static object _lockObject = new object();
            #endregion

            #region Properties

            private Process Process
            {
                get { return _Process; }
                set { _Process = value; }
            }

            /// <summary>
            /// Names the task so that you can use the waitforexit task later.
            /// </summary>
            [TaskAttribute("taskname", Required = false)]
            public string TaskName
            {
                get { return _taskName; }
                set { _taskName = value; }
            }

            /// <summary>
            /// Indicates the intent to call the waitforexit task in the future.
            /// </summary>
            [TaskAttribute("waitforexit", Required = false)]
            public bool WaitForExit
            {
                get { return _waitForExit; }
                set { _waitForExit = value; }
            }

            [TaskAttribute("outputproperty")]
            public string OutputProperty
            {
                get
                {
                    return this._OutputProperty;
                }
                set
                {
                    this._OutputProperty = value;
                }
            }

            [TaskAttribute("redirectoutput")]
            public bool RedirectOutput
            {
                get
                {
                    return this._redirectOutput;
                }
                set
                {
                    this._redirectOutput = value;
                }
            }

            #endregion

            #region Public Methods

            public void Wait()
            {
                try
                {
                    this.Process.WaitForExit(this.TimeOut);
                    if (!this.Process.HasExited)
                    {
                        try
                        {
                            this.Process.Kill();
                        }
                        catch
                        {
                        }
                        throw new BuildException(string.Format("External Program {0} did not finish within {1} milliseconds.", new object[] { this.ProgramFileName, this.TimeOut }), this.Location);
                    }
                    if ((this.Process.ExitCode != 0))
                    {
                        throw new BuildException(string.Format("External Program Failed: {0} (return code was {1})", new object[] { this.ProgramFileName, this.Process.ExitCode }), this.Location);
                    }
                }
                catch (BuildException exception1)
                {
                    if (base.FailOnError)
                    {
                        throw;
                    }
                    this.Log(Level.Error, exception1.Message);
                }
                finally
                {
                    if ((this.ResultProperty != null && this.WaitForExit))
                    {
                        this.Properties[this.ResultProperty] = this.Process.ExitCode.ToString();
                    }
                }
            }

            #endregion

            #region Protected Methods

            protected override void ExecuteTask()
            {
                this.Process = this.StartProcess();

                if (this.RedirectOutput)
                {
                    Thread outputThread = null;
                    Thread errorThread = null;
                    outputThread = new Thread(new ThreadStart(StreamReaderThread_Output));
                    errorThread = new Thread(new ThreadStart(StreamReaderThread_Error));

                    outputThread.IsBackground = true;
                    errorThread.IsBackground = true;

                    _stdOut = this.Process.StandardOutput;
                    _stdError = this.Process.StandardError;

                    outputThread.Start();
                    errorThread.Start();
                }

                if (this.TaskName != string.Empty && this.WaitForExit == false)
                {
                    Log(Level.Warning, "You set the attribute taskname to {0} and waitforexit to false.  You will not be able to call the waitforexit task with the task name {0} with an error.  If you wanted to wait for this to exit please set waitforexit to true.", this.TaskName);
                }
                if (this.TaskName != string.Empty && this.WaitForExit)
                {
                    AsyncExecList.Add(this.TaskName, this);
                }
                if (this.TaskName == string.Empty && this.WaitForExit)
                {
                    this.Wait();
                }
            }


            /// <summary>
            /// Reads from the stream until the external program is ended.
            /// </summary>
            protected void StreamReaderThread_Output()
            {
                StreamReader reader = _stdOut;
                bool doAppend = OutputAppend;
                StringBuilder Capture = new StringBuilder();

                while (true)
                {
                    string logContents = reader.ReadLine();
                    if (logContents == null)
                    {
                        break;
                    }

                    // ensure only one thread writes to the log at any time
                    lock (_lockObject)
                    {
                        if (!String.IsNullOrEmpty(this.OutputProperty))
                        {
                            Capture.AppendLine(logContents);
                        }
                        if (Output != null)
                        {
                            StreamWriter writer = new StreamWriter(Output.FullName, doAppend);
                            writer.WriteLine(logContents);
                            doAppend = true;
                            writer.Close();
                        }
                        if (OutputWriter!=null)
                        {
                            OutputWriter.WriteLine(logContents);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(this.OutputProperty))
                {
                    if (!this.Properties.Contains(this.OutputProperty))
                        this.Properties.Add(this.OutputProperty, string.Empty);
                    this.Properties[this.OutputProperty] = Capture.ToString();
                }
            }

            /// <summary>
            /// Reads from the stream until the external program is ended.
            /// </summary>
            protected void StreamReaderThread_Error()
            {
                StreamReader reader = _stdError;
                bool doAppend = OutputAppend;

                while (true)
                {
                    string logContents = reader.ReadLine();
                    if (logContents == null)
                    {
                        break;
                    }

                    // ensure only one thread writes to the log at any time
                    lock (_lockObject)
                    {
                        if (Output != null)
                        {
                            StreamWriter writer = new StreamWriter(Output.FullName, doAppend);
                            writer.WriteLine(logContents);
                            doAppend = true;
                            writer.Close();
                        }

                        if (ErrorWriter!=null)
                        {
                            ErrorWriter.Write(logContents);
                        }
                    }
                }
            }

            #endregion

        }

    }
