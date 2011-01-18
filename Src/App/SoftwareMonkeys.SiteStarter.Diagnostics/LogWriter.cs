using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using NLog;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Writes entries to the log.
	/// </summary>
	public static class LogWriter
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		

		/// <summary>
		/// Writes a group entry to the log.
		/// </summary>
		/// <param name="data">The log entry data.</param>
		/// <param name="callingMethod">The method that started the group.</param>
		/// <param name="level">The log level.</param>
		/// <param name="groupID">The ID of the group that the entry represents.</param>
		/// <param name="parentID">The ID of the parent group.</param>
		public static void WriteGroup(string message, MethodBase callingMethod, NLog.LogLevel level, Guid groupID, Guid parentID)
		{
			string entry = CreateLogEntry(level, message, callingMethod, groupID, parentID, DiagnosticState.GroupIndent-1);
			if (entry != null && entry.Trim() != String.Empty)
			{
				if (level == NLog.LogLevel.Debug)
					logger.Debug(entry);
				else if (level == NLog.LogLevel.Error)
					logger.Error(entry);
				else if (level == NLog.LogLevel.Info)
					logger.Info(entry);
				
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
		/// <param name="indent">The indent level of the entry.</param>
		/// <returns></returns>
		public static string CreateLogEntry(
			NLog.LogLevel logLevel,
			string message,
			MethodBase callingMethod,
			Guid id,
			Guid parentID,
			int indent)
		{
			StringBuilder logEntry = new StringBuilder();
			
			// If the callingMethod property is null then logging must be disabled, so skip the output
			if (callingMethod != null && new LogSupervisor().LoggingEnabled(callingMethod, logLevel))
			{
				//if (indent > 0 && parentID == Guid.Empty)
				//	throw new Exception("Couldn't detect parent group of an entry at indent '" + indent + "' with data '" + message + "' from method '" + callingMethod.DeclaringType.ToString() + "." + callingMethod.Name + "'.");
				
				logEntry.Append("<Entry>\r\n");
				logEntry.AppendFormat("<ID>{0}</ID>\r\n", id.ToString());
				logEntry.AppendFormat("<ParentID>{0}</ParentID>\r\n", parentID.ToString());
				logEntry.AppendFormat("<Indent>{0}</Indent>\r\n", indent);
				logEntry.AppendFormat("<LogLevel>{0}</LogLevel>\r\n", logLevel);
				logEntry.AppendFormat("<Timestamp>{0}</Timestamp>\r\n", DateTime.Now);
				//logEntry.AppendFormat("<IsThreadTitle>{0}</IsThreadTitle>\r\n", isThreadTitle.ToString());
				logEntry.AppendFormat("<Component>{0}</Component>\r\n", EscapeLogData(callingMethod.DeclaringType.ToString()));
				logEntry.AppendFormat("<Method>{0}</Method>\r\n", EscapeLogData(callingMethod.Name));
				logEntry.AppendFormat("<Data>{0}</Data>\r\n", EscapeLogData(message));
				
				// TODO: Skip stack trace here and save it to a separate file, to reduce log size
				// TODO: Check if needed. Should be obsolete
				//logEntry.AppendFormat("<StackTrace>{0}</StackTrace>\r\n", EscapeLogData());
				
				logEntry.Append("</Entry>\r\n");
				
				logEntry.AppendLine();
				
				SaveStackTrace(id);
			}
			
			return logEntry.ToString();
		}
		
		
		public static void Error(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Error(message, callingMethod);
		}

		public static void Error(string message, MethodBase callingMethod)
		{
			string entry = CreateLogEntry(LogLevel.Error, message, callingMethod, Guid.NewGuid(), DiagnosticState.CurrentGroupID, DiagnosticState.GroupIndent);
			if (entry != null && entry.Trim() != String.Empty)
				logger.Error(entry);
		}

		public static void Info(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Info(message, callingMethod);
		}

		public static void Info(string message, MethodBase callingMethod)
		{
			string entry = CreateLogEntry(LogLevel.Info, message, callingMethod, Guid.NewGuid(), DiagnosticState.CurrentGroupID, DiagnosticState.GroupIndent);
			if (entry != null && entry.Trim() != String.Empty)
				logger.Info(entry);
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Debug(message, callingMethod);
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message, MethodBase callingMethod)
		{
			if (new LogSupervisor().LoggingEnabled(callingMethod, LogLevel.Debug))
			{
				string entry = CreateLogEntry(LogLevel.Debug, message, callingMethod, Guid.NewGuid(), DiagnosticState.CurrentGroupID, DiagnosticState.GroupIndent);
				if (entry != null && entry.Trim() != String.Empty)
					logger.Debug(entry);
			}
		}
		
		/// <summary>
		/// Saves the current stack trace to file with the provided ID.
		/// </summary>
		/// <param name="id"></param>
		private static void SaveStackTrace(Guid id)
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
		
		private static string GetStackTraceDirectoryPath()
		{
			return StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data"
				+ Path.DirectorySeparatorChar + "Logs"
				+ Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd")
				+ Path.DirectorySeparatorChar + "StackTrace";
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
				"DiagnosticState"
			};
			
			foreach (string skippableComponent in skippableComponents)
			{
				if (component.Name == skippableComponent)
					return true;
			}
			
			return false;
		}
		
		private static string EscapeLogData(string data)
		{
			return data.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("\"", "&quot;")
				.Replace("'", "&apos;");
		}

	}
}
