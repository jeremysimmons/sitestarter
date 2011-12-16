using System;
using System.ComponentModel;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;
using System.Diagnostics;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Writes log entries to the log file.
	/// </summary>
	public class LogFileWriter : Component
	{
		static public string LogFileWriterKey = "Diagnostics.LogFileWriter.Current";
		
		static private LogFileWriter current;
		static public LogFileWriter Current
		{
			get {
				if (current == null)
					current = new LogFileWriter();
				return current; }
		}
		
		static private string logFilePath = String.Empty;
		/// <summary>
		/// Gets/sets the path to the current log file.
		/// </summary>
		static public string LogFilePath
		{
			get {
				if (logFilePath == String.Empty)
					logFilePath = GetLogFilePath();
				return logFilePath; }
			set { logFilePath = value; }
		}
		
		protected string BufferDataKey = "Diagnostics.LogFileWriter.BufferData";
		
		/// <summary>
		/// Gets/sets the current log data held in the buffer.
		/// </summary>
		public string BufferData
		{
			get {
				if (StateAccess.IsInitialized)
				{
					if (StateAccess.State.ContainsApplication(BufferDataKey))
						return (string)StateAccess.State.GetApplication(BufferDataKey);
				}
				else
					throw new InvalidOperationException("Cannot access log buffer when the state access has not been initialized.");
				
				return String.Empty;
			}
			set
			{
				if (StateAccess.IsInitialized)
					StateAccess.State.SetApplication(BufferDataKey, value);
				else
					throw new InvalidOperationException("Cannot write to log file when the state access has not been initialized.");
			}
		}
		
		protected string PreviousCommitTimeKey = "Diagnostics.LogFileWriter.PreviousCommitTime";
		
		/// <summary>
		/// Gets/sets the time that the previous commit occurred.
		/// </summary>
		public DateTime PreviousCommitTime
		{
			get {
				if (StateAccess.State.ContainsApplication(PreviousCommitTimeKey))
					return (DateTime)StateAccess.State.GetApplication(PreviousCommitTimeKey);
				return DateTime.MinValue;
			}
			set
			{
				StateAccess.State.SetApplication(PreviousCommitTimeKey, value);
			}
		}
		
		public LogFileWriter()
		{
		}
		
		public void Write(string message, LogLevel level, string callingType, string callingMethod)
		{
			Write(message, level, callingType, callingMethod, Guid.NewGuid(), DiagnosticState.CurrentGroupID);
		}
		
		public void Write(string message, LogLevel level, string callingType, string callingMethod, Guid entryID, Guid parentID)
		{
			
			string entry = CreateLogEntry(level, message, callingType, callingMethod, entryID, parentID);

			if (entry != null && entry != String.Empty)
			{
				NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
				
				if (level == LogLevel.Debug)
					logger.Debug(entry);
				else if (level == LogLevel.Error)
					logger.Error(entry);
				else if (level == LogLevel.Info)
					logger.Info(entry);
				else
					throw new NotSupportedException("Log level is not currently supported: " + level.ToString());
				
				WriteStackTrace(entryID);
			}
			
		}
		
		/// <summary>
		/// Creates a new log entry XML from the provided data.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="data">The log entry data.</param>
		/// <param name="callingMethod">The method that started the group.</param>
		/// <param name="groupID">The ID of the group that the entry represents.</param>
		/// <param name="parentID">The ID of the parent group.</param>
		/// <returns></returns>
		public static string CreateLogEntry(
			LogLevel logLevel,
			string message,
			string callingTypeName,
			string callingMethodName,
			Guid id,
			Guid parentID)
		{
			StringBuilder logEntry = new StringBuilder();

			// If the callingMethod property is null then logging must be disabled, so skip the output
			if (new LogSupervisor().LoggingEnabled(callingTypeName, logLevel))
			{
				//if (indent > 0 && parentID == Guid.Empty)
				//	throw new Exception("Couldn't detect parent group of an entry at indent '" + indent + "' with data '" + message + "' from method '" + callingMethod.DeclaringType.ToString() + "." + callingMethod.Name + "'.");
				
				logEntry.Append("<Entry>\r\n");
				
				logEntry.AppendFormat("<ID>{0}</ID>\r\n", id.ToString());
				logEntry.AppendFormat("<ParentID>{0}</ParentID>\r\n", parentID.ToString());
				logEntry.AppendFormat("<Indent>{0}</Indent>\r\n", 0); // TODO: Remove indent as a parameter - it's obsolete; the parent ID is used to figure out location
				logEntry.AppendFormat("<LogLevel>{0}</LogLevel>\r\n", logLevel);
				logEntry.AppendFormat("<Timestamp>{0}</Timestamp>\r\n", DateTime.Now);
				logEntry.AppendFormat("<Component>{0}</Component>\r\n", EscapeLogData(callingTypeName));
				logEntry.AppendFormat("<Method>{0}</Method>\r\n", EscapeLogData(callingMethodName));
				logEntry.AppendFormat("<Data>{0}</Data>\r\n", EscapeLogData(message));
				
				logEntry.Append("</Entry>\r\n");
				
				logEntry.AppendLine();
				
			}
			
			return logEntry.ToString();
		}
		
		private static string EscapeLogData(string data)
		{
			return data.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("\"", "&quot;")
				.Replace("'", "&apos;");
		}
			
			/// <summary>
			/// Saves the current stack trace to file with the provided ID.
			/// </summary>
			/// <param name="id"></param>
			public void WriteStackTrace(Guid id)
		{
			string trace = CreateStackTrace();
			
			string path = GetStackTracePath(id);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(trace);
				writer.Close();
			}
		}

		
		static public string GetLogFilePath()
		{
			string dateStamp = DateTime.Now.Year + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.Day;
			
			return StateAccess.State.PhysicalApplicationPath.TrimEnd('\\')
				+ Path.DirectorySeparatorChar + "App_Data"
				+ Path.DirectorySeparatorChar + "Logs"
				+ Path.DirectorySeparatorChar + dateStamp
				+ Path.DirectorySeparatorChar + "Log.xml";
		}
		
		static public string CreateLogHeader()
		{
			string header = @"<?xml version='1.0'?>
<Log>
";
			
			return header;
		}
		
		private static string GetStackTracePath(Guid id)
		{
			return GetStackTraceDirectoryPath() + Path.DirectorySeparatorChar + id.ToString() + ".trace";
		}
		
		/// <summary>
		/// Creates the stack trace for the current entry.
		/// </summary>
		/// <returns></returns>
		private static string CreateStackTrace()
		{
			StringBuilder builder = new StringBuilder();
			
			StackTrace stackTrace = new StackTrace();           // get call stack
			StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

			// write call stack method names
			foreach (StackFrame stackFrame in stackFrames)
			{
				if (!SkipFrame(stackFrame))
				{
					builder.Append(stackFrame.GetMethod().DeclaringType.ToString());
					builder.Append(":");
					builder.Append(stackFrame.GetMethod().ToString());// write method name
					builder.Append("\n");
				}
			}
			
			return builder.ToString();
		}
		
		private static bool SkipFrame(StackFrame frame)
		{
			Type component = frame.GetMethod().DeclaringType;
			
			string[] skippableComponents = new string[] {
				"AppLogger",
				"LogWriter",
				"LogGroup",
				"DiagnosticState",
				"LogFileWriter"
			};
			
			foreach (string skippableComponent in skippableComponents)
			{
				if (component.Name == skippableComponent)
					return true;
			}
			
			return false;
		}
		
		private static string GetStackTraceDirectoryPath()
		{
			return StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data"
				+ Path.DirectorySeparatorChar + "Logs"
				+ Path.DirectorySeparatorChar + DateTime.Now.Year + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("dd")
				+ Path.DirectorySeparatorChar + "StackTrace";
		}
		
		private void EndLog()
		{
			NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info("</Log>");
		}
		
		protected override void Dispose(bool disposing)
		{
			EndLog();
			
			base.Dispose(disposing);
		}
	}
}
