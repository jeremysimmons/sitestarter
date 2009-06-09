using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using NLog;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Helps with the diagnostics tracing.
	/// </summary>
	public class LogGroup : IComponent
    {
		protected bool HasEnded = false;
		protected bool IncludeInRelease = false;
		//protected string IndentSpacer = "  ";

        private Guid id;
        public Guid ID
        {
            get {
                if (id == Guid.Empty)
                    id = Guid.NewGuid();
                return id; }
            set { id = value; }
        }


        private LogLevel logLevel = LogLevel.Debug;
        /// <summary>
        /// Gets/sets the base log level for this group.
        /// </summary>
        public LogLevel LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        // TODO: Clean up
		/*private TraceGroup parent;
		/// <summary>
		/// Gets/sets the parent group.
		/// </summary>
		public TraceGroup Parent
		{
			get { return parent; }
			set { parent = value; }
		}*/

        /*private int indent;
        /// <summary>
        /// Gets/sets the indent of the group.
        /// </summary>
        public int Indent
        {
            get { return indent; }
           // set { indent = value; }
        }*/

		private string summary;
		/// <summary>
		/// Gets/sets the title of the group.
		/// </summary>
		public string Summary
		{
            get { return summary; }
            set { summary = value; }
		}

		/*private string description;
		/// <summary>
		/// Gets/sets the description of the group.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}*/

/*		private LogGroupMode mode;
		/// <summary>
		/// Gets/sets the mode of the group.
		/// </summary>
		public LogGroupMode Mode
		{
			get { return mode; }
			set { mode = value; }
		}*/

        private MethodBase callingMethod;
        /// <summary>
        /// Gets/sets the method that created this log group.
        /// </summary>
        public MethodBase CallingMethod
        {
            get { return callingMethod; }
            set { callingMethod = value; }
        }

        // TODO: Clean up
		/*private List<TraceGroup> subGroups = new List<TraceGroup>();
		/// <summary>
		/// The subgroups of this group.
		/// </summary>
		public List<TraceGroup> SubGroups
		{
			get { return subGroups; }
			set { subGroups = value; }
		}*/


		public LogGroup(string summary, MethodBase callingMethod)
		{
            this.summary = summary;
            this.callingMethod = callingMethod;
		}

        // TODO: Remove unnecessary code
		/*public LogGroup(LogGroup parent, string title, string description, LogGroupMode mode)
		{
			this.parent = parent;
			this.title = title;
			this.description = description;
			this.mode = mode;
		}*/

		/*public LogGroup(string title, LogGroupMode mode)
		{
			this.title = title;
			this.mode = mode;
		}*/

		/*public LogGroup(LogGroup parent, string title, LogGroupMode mode)
		{
			this.parent = parent;
			this.title = title;
			this.mode = mode;
		}*/

		public LogGroup(string summary, bool includeInRelease)
		{
		    this.summary = summary;
			//this.description = description;
			//this.mode = mode;
			this.IncludeInRelease = includeInRelease;
		}

        // TODO: Remove unnecessary code
		/*public LogGroup(LogGroup parent, string title, string description, LogGroupMode mode, bool includeInRelease)
		{
			this.parent = parent;
			this.title = title;
			this.description = description;
			this.mode = mode;
			this.IncludeInRelease = includeInRelease;
		}*/

        	/*public void Write(string message)
		{
			string indentString = GetIndentString(this);

			Tracer.Logger.Info(indentString + message);
		}*/

        //	public void WriteLine(string message, MethodBase callingMethod)
		//{
			//string indentString = GetIndentString();

            //Tracer.Logger.Info(indentString + message);
            //NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            //logger.Info(message);
          //  DataLogger.Info(message, callingMethod.Type.ToString(), callingMethod.MethodName.ToString(), DataLogger.Indent);
		//}

        	/*public void Write(string category, string message)
		{
			//log4net.ILog logger = log4net.LogManager.GetLogger("File");
			//logger.Write(category, message);
		}*/

        public void Write(string message, LogLevel logLevel)
        {
            MethodBase callingMethod = Reflector.GetCallingMethod();
            Write(message, logLevel, callingMethod);
        }

        public void Write(string message, LogLevel logLevel, MethodBase callingMethod)
		{
            if (logLevel < this.LogLevel)
                throw new ArgumentException("The provided log level " + logLevel + " must be equal or greater than the log level of the group, which is " + logLevel + ".");

            if (LogLevel == LogLevel.Debug)
            {
                Debug(message, callingMethod);
            }
            else if (LogLevel == LogLevel.Info)
            {
                Info(message, callingMethod);
            }
            else
                throw new NotImplementedException("Support for the LogLevel type of " + logLevel + " has not yet been implemented.");
		}

        public void Error(string message)
        {
            MethodBase callingMethod = Reflector.GetCallingMethod();
            Error(message, callingMethod);
        }

        public void Error(string message, MethodBase callingMethod)
        {
            AppLogger.Error(message, callingMethod);
        }

	[ Conditional("DEBUG") ]
        public void Debug(string message)
        {
            MethodBase callingMethod = Reflector.GetCallingMethod();
            Debug(message, callingMethod);
        }

	[ Conditional("DEBUG") ]
        public void Debug(string message, MethodBase callingMethod)
        {
            AppLogger.Debug(message, callingMethod);
        }

        public void Info(string message)
        {
            MethodBase callingMethod = Reflector.GetCallingMethod();
            Info(message, callingMethod);
        }

        public void Info(string message, MethodBase callingMethod)
        {
            AppLogger.Info(message, callingMethod);
        }

        internal void Start(MethodBase callingMethod)
		{
            AppLogger.Push(this);

            CallingMethod = callingMethod;

                    StartLevel1(callingMethod);

                    AppLogger.AddIndent();

            // TODO: Remove...obsolete
            //DataLogger.Indent = DataLogger.GroupStack.Count - 1;

			//switch (mode)
			//{
			//	case LogGroupMode.Level1:
				/*	break;
				case LogGroupMode.Level2:
                    StartLevel2(callingMethod);
					break;
				case LogGroupMode.Level3:
                    StartLevel3(callingMethod);
					break;
				case LogGroupMode.Level4:
                    StartLevel4(callingMethod);
					break;
				case LogGroupMode.Level5:
                    StartLevel5(callingMethod);
					break;
				default:
					throw new Exception("The trace group mode is not valid.");*/
			//  }
		}

        internal void End()
        {

            // Ensure this doesnt run twice for the same group.
            if (!HasEnded)
            {
                AppLogger.RemoveIndent();
                //switch (mode)
                //{
                //	case LogGroupMode.Level1:
                EndLevel1(CallingMethod);
                //		break;
                //	case LogGroupMode.Level2:
                /*        EndLevel2(callingMethod);
						break;
					case LogGroupMode.Level3:
                        EndLevel3(callingMethod);
						break;
					case LogGroupMode.Level4:
                        EndLevel4(callingMethod);
						break;
					case LogGroupMode.Level5:
                        EndLevel5(callingMethod);
						break;
					default:
						throw new Exception("The trace group mode is not valid.");*/
                //}

                //Trace.Indent();

                HasEnded = true;


                AppLogger.Pop();
            }
        }

		[Conditional("DEBUG")]
        private void StartLevel1(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
				//WriteLine("** ====================================================== **");
				Write("Start... - " + Summary, LogLevel, callingMethod);
                //WriteLine("** Date/Time: " + DateTime.Now, callingMethod);
				//WriteLine("** ====================================================== **");
				//WriteLine("************************************************************");
               // WriteLine("" + Description, callingMethod);
				//WriteLine("************************************************************");
				//WriteLine("************************************************************");
			//}
		}

		[Conditional("DEBUG")]
        private void EndLevel1(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
				//WriteLine("** ====================================================== **");
            Write("...End", LogLevel, callingMethod);
           // WriteLine("** Date/Time: " + DateTime.Now, callingMethod);
				//WriteLine("** ====================================================== **");
				//WriteLine("************************************************************");
			//}
		}

		/*[Conditional("DEBUG")]
        private void StartLevel2(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
				WriteLine("Starting: " + Summary, callingMethod);
            //WriteLine("** Date/Time: " + DateTime.Now, callingMethod);
				//WriteLine("************************************************************");
            ///WriteLine("** " + Description, callingMethod);
				//WriteLine("************************************************************");
			//}
		}

		[Conditional("DEBUG")]
        private void EndLevel2(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
            WriteLine("...End: ", callingMethod);
            //WriteLine("** Date/Time: " + DateTime.Now, callingMethod);
				//WriteLine("************************************************************");
			//}
		}

		[Conditional("DEBUG")]
        private void StartLevel3(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
            WriteLine("** ====== Starting: " + Summary, callingMethod);
            //WriteLine("** Date/Time: " + DateTime.Now, callingMethod);
            //WriteLine("** " + Description, callingMethod);
				//WriteLine("************************************************************");
			//}
		}

		[Conditional("DEBUG")]
        private void EndLevel3(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
				//WriteLine("************************************************************");
            WriteLine("** ====== Ending: " + Title, callingMethod);
            //WriteLine("**  Date/Time: " + DateTime.Now, callingMethod);
				//WriteLine("************************************************************");
			//}
		}

		[Conditional("DEBUG")]
        private void StartLevel4(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
            WriteLine("** ====== Starting: " + Title, callingMethod);
            //WriteLine("** " + Description, callingMethod);
			//}
		}

		[Conditional("DEBUG")]
        private void EndLevel4(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
            WriteLine("** ====== Ending: " + Title, callingMethod);
			//}
		}

		[Conditional("DEBUG")]
        private void StartLevel5(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
            WriteLine("** == Starting: " + Title, callingMethod);
            //WriteLine("** " + Description, callingMethod);
			//}
		}

		[Conditional("DEBUG")]
        private void EndLevel5(MethodBase callingMethod)
		{
			//if (!SkipLogging())
			//{
            WriteLine("** == Ending: " + Title, callingMethod);
			//}
		}*/

		/*protected bool SkipLogging()
		{
			bool isDebugMode = false;

			bool isDebugMode = true;

			// If the project is running in debug mode
			// Or the trace group is to be included in the release
			// Then return false, indicating NOT to skip the logging for this group
			if (isDebugMode || IncludeInRelease)
				return false;
			else
				return true;
		}*/


		#region IComponent members
        	public event EventHandler Disposed;

		private ISite site;
		public virtual ISite Site
		{
			get { return site; }
			set { site = value; }
		}

		public void Dispose()
		{
			End();

			if(Disposed != null)
                		Disposed(this,EventArgs.Empty);

		}
		#endregion
	}

}
