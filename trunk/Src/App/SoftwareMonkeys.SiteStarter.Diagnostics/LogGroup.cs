using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Helps with the diagnostics tracing.
	/// </summary>
	public class LogGroup : System.ComponentModel.IComponent
	{
		protected bool HasEnded = false;
		protected bool IncludeInRelease = false;

		private Guid id;
		public Guid ID
		{
			get {
				if (id == Guid.Empty)
					id = Guid.NewGuid();
				return id; }
			set { id = value; }
		}
		
		private LogSupervisor logSupervisor;
		public LogSupervisor LogSupervisor
		{
			get {
				if (logSupervisor == null)
					logSupervisor = new LogSupervisor();
				return logSupervisor; }
			set { logSupervisor = value; }
		}
		
		/// <summary>
		/// A flag indicating whether the log group is marked to be output.
		/// </summary>
		public bool IsOutput
		{
			get
			{
				if (CallingMethod == null)
					return LogSupervisor.LoggingEnabled(LogLevel);
				else
					return LogSupervisor.LoggingEnabled(CallingMethod.DeclaringType.Name, LogLevel);
			}
		}
		
		private bool isThreadTitle;
		public bool IsThreadTitle
		{
			get { return isThreadTitle; }
			set { isThreadTitle = value; }
		}

		private LogGroup parent;
		[XmlIgnore]
		public LogGroup Parent
		{
			get { return parent; }
			set { parent = value;
				if (parent == null)
					parentID = Guid.Empty;
				else
					parentID = parent.ID;
			}
		}

		private Guid parentID;
		public Guid ParentID
		{
			get {
				if (parent != null)
					return parent.ID;
				return parentID; }
			set {
				parentID = value;

				if (parent != null)
				{
					if (parent.ID != parentID)
						parent = null;
				}
			}
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

		private string summary;
		/// <summary>
		/// Gets/sets the title of the group.
		/// </summary>
		public string Summary
		{
			get { return summary; }
			set { summary = value; }
		}

		private MethodBase callingMethod;
		/// <summary>
		/// Gets/sets the method that created this log group.
		/// </summary>
		public MethodBase CallingMethod
		{
			get { return callingMethod; }
			set { callingMethod = value; }
		}

		private LogGroup[] subGroups = new LogGroup[]{};
		/// <summary>
		/// The subgroups of this group.
		/// </summary>
		public LogGroup[] SubGroups
		{
			get { return subGroups; }
			set { subGroups = value;
				if (subGroups != null)
				{
					foreach (LogGroup group in subGroups)
					{
						group.Parent = this;
					}
				}
			}
		}


		public LogGroup(string summary, MethodBase callingMethod, LogLevel level)
		{
			this.summary = summary;
			this.callingMethod = callingMethod;
			this.logLevel = level;
		}
		
		private void Write(string message, LogLevel logLevel)
		{
			if (new LogSupervisor().LoggingEnabled(logLevel))
			{
				CallingMethod = Reflector.GetCallingMethod();
				
				Write(message, logLevel, callingMethod);
			}
		}

		private void Write(string message, LogLevel logLevel, MethodBase callingMethod)
		{
			if (callingMethod != null && new LogSupervisor().LoggingEnabled(callingMethod.DeclaringType.Name, logLevel))
			{
				CallingMethod = callingMethod;
				
				Guid parentID = GetOutputParentID();
				
				if (IsOutput)
					LogWriter.WriteGroup(message, callingMethod, LogLevel, ID, parentID);
			}
		}


		internal void Start()
		{
			
			if (callingMethod != null && new LogSupervisor().IsEnabled(callingMethod.DeclaringType.Name, LogLevel))
			{
				DiagnosticState.PushGroup(this);
				
				Write(Summary, LogLevel, CallingMethod);
			}
			
		}

		internal void End()
		{

			// Ensure this doesnt run twice for the same group.
			if (!HasEnded)
			{
				LogSupervisor supervisor = new LogSupervisor();
				
				if (CallingMethod != null && supervisor.IsEnabled(CallingMethod.DeclaringType.Name, LogLevel))
				{
					// Keep popping groups from the stack until it reaches the correct level
					// This loop ensures that if a parent group ends before a child group that the child group(s) will be popped
					// as well and the stack won't become corrupted
					// TODO: Check if this loop should be moved to DiagnosticState.PopGroup
					while (supervisor.CanPop(this, DiagnosticState.CurrentGroup))
						DiagnosticState.PopGroup();
				}
				
				HasEnded = true;

			}
		}
		
		public void Append(LogGroup group)
		{
			List<LogGroup> list = new List<LogGroup>();
			list.Add(group);
			SubGroups = list.ToArray();
		}
		
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
		
		/// <summary>
		/// Gets the ID of the parent but skips any groups not marked to be output.
		/// </summary>
		public Guid GetOutputParentID()
		{
			LogGroup parent = Parent;
			Guid outputParentID = ParentID;
			
			if (parent != null && !parent.IsOutput)
			{
				parent = GetOutputParent(parent);
			}
			
			if (parent != null)
				outputParentID = parent.ID;
			else
				outputParentID = Guid.Empty;
			
			return outputParentID;
		}
		
		/// <summary>
		/// Gets the ID of the parent but skips any groups not marked to be output.
		/// </summary>
		public LogGroup GetOutputParent(LogGroup group)
		{
			if (group != null)
			{
				LogGroup parent = group.Parent;
				
				if (parent != null && !parent.IsOutput)
					parent = GetOutputParent(parent);
				
				return parent;
			}
			else
				return null;
		}
		#endregion
		
		#region Static functions
		
		/// <summary>
		/// Starts a new debug level log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <returns></returns>
		static public LogGroup StartDebug(string summary)
		{
			MethodBase callingMethod = null;
			
			if (new ModeDetector().IsDebug)
				callingMethod = Reflector.GetCallingMethod();

			return Start(summary, LogLevel.Debug, callingMethod);
		}
		
		/// <summary>
		/// Starts a new log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <returns></returns>
		static public LogGroup Start(string summary)
		{
			MethodBase callingMethod = null;

			if (new LogSupervisor().LoggingEnabled(LogLevel.Debug))
				callingMethod = Reflector.GetCallingMethod();

			return Start(summary, LogLevel.Info, callingMethod);
		}
		
		/// <summary>
		/// Starts a new log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="logLevel"></param>
		/// <returns></returns>
		static public LogGroup Start(string summary, NLog.LogLevel logLevel)
		{
			return Start(summary, LogWriter.ConvertLevel(logLevel));
		}
		
		/// <summary>
		/// Starts a new log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="logLevel"></param>
		/// <returns></returns>
		static public LogGroup Start(string summary, LogLevel logLevel)
		{
			MethodBase callingMethod = null;
			
			// Only get the calling method if logging is actually enabled
			if (new LogSupervisor().LoggingEnabled(logLevel))
				callingMethod = Reflector.GetCallingMethod();
			
			return Start(summary, logLevel, callingMethod);
		}
		
		/// <summary>
		/// Starts a new log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="logLevel"></param>
		/// <param name="callingMethod"></param>
		/// <returns></returns>
		static public LogGroup Start(string summary, LogLevel logLevel, MethodBase callingMethod)
		{
			LogGroup newGroup = null;
			
			// Create the log group even if not enabled
			newGroup = new LogGroup(summary, callingMethod, logLevel);
			
			// Only set the parent if the new group is enabled
			if (callingMethod != null && new LogSupervisor().IsEnabled(callingMethod.DeclaringType.Name, logLevel))
			{
				newGroup.Parent = DiagnosticState.CurrentGroup;
			}

			newGroup.Start();

			return newGroup;
		}
		#endregion
	}

}
