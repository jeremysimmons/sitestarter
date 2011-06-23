using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Writes entries to the log.
	/// </summary>
	public static class LogWriter
	{	

		/// <summary>
		/// Converts the provided NLog.LogLevel value to the internal LogLevel value.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <returns></returns>
		public static LogLevel ConvertLevel(NLog.LogLevel logLevel)
		{
			return (LogLevel)Enum.Parse(typeof(LogLevel), logLevel.ToString());
		}
		
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
			WriteGroup(message, callingMethod, ConvertLevel(level), groupID, parentID);
		}
		
		/// <summary>
		/// Writes a group entry to the log.
		/// </summary>
		/// <param name="data">The log entry data.</param>
		/// <param name="callingMethod">The method that started the group.</param>
		/// <param name="level">The log level.</param>
		/// <param name="groupID">The ID of the group that the entry represents.</param>
		/// <param name="parentID">The ID of the parent group.</param>
		public static void WriteGroup(string message, MethodBase callingMethod, LogLevel level, Guid groupID, Guid parentID)
		{
			string entry = CreateLogEntry(level, message, callingMethod.DeclaringType.Name, callingMethod.Name, groupID, parentID, DiagnosticState.GroupIndent-1);
			if (entry != null && entry.Trim() != String.Empty)
			{
				LogFileWriter.Current.Write(groupID, entry);
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
			LogLevel logLevel,
			string message,
			string callingTypeName,
			string callingMethodName,
			Guid id,
			Guid parentID,
			int indent)
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
				logEntry.AppendFormat("<Indent>{0}</Indent>\r\n", indent); // TODO: Remove indent as a parameter - it's obsolete; the parent ID is used to figure out location
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
		
		public static void Error(Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException("ex");
			
			Error(ex.ToString());
		}
		
		public static void Error(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Error(message, callingMethod);
		}

		public static void Error(string message, MethodBase callingMethod)
		{
			Write(message, callingMethod, LogLevel.Error);
		}

		public static void Info(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Info(message, callingMethod);
		}

		public static void Info(string message, MethodBase callingMethod)
		{
			Write(message, callingMethod, LogLevel.Info);
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message)
		{
			MethodBase callingMethod = null;
			if (new LogSupervisor().LoggingEnabled(LogLevel.Debug))
			{
				callingMethod = Reflector.GetCallingMethod();
				Debug(message, callingMethod);
			}
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message, MethodBase callingMethod)
		{
			Write(message, callingMethod, LogLevel.Debug);
		}
		
		public static void Write(string message, MethodBase callingMethod, LogLevel level)
		{
			Write(message, callingMethod.DeclaringType.Name, callingMethod.Name, level);
		}
		
		public static void Write(string message, string callingType, string callingMethod, LogLevel level)
		{
			if (new LogSupervisor().LoggingEnabled(callingType, level))
			{
				Guid id = Guid.NewGuid();
				
				string entry = CreateLogEntry(level, message, callingType, callingMethod, id, DiagnosticState.CurrentGroupID, DiagnosticState.GroupIndent);
				if (entry != null && entry.Trim() != String.Empty)
				{
					LogFileWriter.Current.Write(id, entry);
				}
			}
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
		/// Disposes the current log file writer if it exists in application state.
		/// </summary>
		static public void Dispose()
		{
			if (StateAccess.IsInitialized && StateAccess.State.ContainsApplication(LogFileWriter.LogFileWriterKey))
			{
				LogFileWriter fileWriter = (LogFileWriter)StateAccess.State.GetApplication(LogFileWriter.LogFileWriterKey);
				fileWriter.Dispose();
				
				StateAccess.State.RemoveApplication(LogFileWriter.LogFileWriterKey);
			}
		}

	}
}
