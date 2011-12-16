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
				LogFileWriter.Current.Write(message, level, callingMethod.DeclaringType.Name, callingMethod.Name, groupID, parentID);
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
				LogFileWriter.Current.Write(message, level, callingType, callingMethod);
			}
		}
		
		/// <summary>
		/// Disposes the current log file writer if it exists in application state.
		/// </summary>
		static public void Dispose()
		{
				
				Debug("Disposing log data.");
				
				// TODO: Check if needed. Should be obsolete
				//LogFileWriter fileWriter = (LogFileWriter)StateAccess.State.GetApplication(LogFileWriter.LogFileWriterKey);

				//fileWriter.Dispose();
				

				//StateAccess.State.RemoveApplication(LogFileWriter.LogFileWriterKey);
		}
	}
}
