using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using NLog;
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


		public LogGroup(string summary, MethodBase callingMethod)
		{
			this.summary = summary;
			this.callingMethod = callingMethod;
		}
		
		private void Write(string message, LogLevel logLevel)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Write(message, logLevel, callingMethod);
		}

		private void Write(string message, LogLevel logLevel, MethodBase callingMethod)
		{
			if (logLevel < this.LogLevel)
				throw new ArgumentException("The provided log level " + logLevel + " must be equal or greater than the log level of the group, which is " + logLevel + ".");

			LogWriter.WriteGroup(message, callingMethod, LogLevel, ID, ParentID);
		}


		internal void Start(MethodBase callingMethod, NLog.LogLevel logLevel)
		{
			DiagnosticState.PushGroup(this);
						
				CallingMethod = callingMethod;
				
				Start(callingMethod);
		}
		
		private void Start(MethodBase callingMethod)
		{
			Write(Summary, LogLevel, callingMethod);
		}

		internal void End()
		{

			// Ensure this doesnt run twice for the same group.
			if (!HasEnded)
			{
				
				if (new LogSupervisor().LoggingEnabled(CallingMethod, LogLevel))
				{
					End(CallingMethod);

					
				}
				
				DiagnosticState.PopGroup();
				
				// Ensure that the log is still properly formatted
				// If the current group ID is not the ID of this group's parent after this group has ended then the log is corrupt
				if (DiagnosticState.CurrentGroupID != this.ParentID)
					throw new CorruptLogException(this);
				
				HasEnded = true;

			}
		}

		private void End(MethodBase callingMethod)
		{
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
		#endregion
		
		#region Static functions
		
		/// <summary>
		/// Starts a new log group.
		/// </summary>
		/// <param name="summary"></param>
		/// <returns></returns>
		static public LogGroup Start(string summary)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();

			return Start(summary, LogLevel.Info, callingMethod);
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
			
			// Don't get the calling method if its not
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
			LogGroup parent = DiagnosticState.CurrentGroup;
			
			LogGroup newGroup = new LogGroup(summary, callingMethod);
			newGroup.Parent = parent;
			
			
			newGroup.Start(callingMethod, logLevel);


			return newGroup;
		}

		/*/// <summary>
		/// Ends the current log group.
		/// </summary>
		static public void End()
		{
			if (DiagnosticState.CurrentGroup != null)
			{
				DiagnosticState.CurrentGroup.End();
			}
		}*/
		#endregion
	}

}
