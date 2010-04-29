using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.State;
//using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	public class AppLogger
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		//private static int indent;
		/// <summary>
		/// Gets/sets the indent of the current log entries.
		/// </summary>
		public static int Indent
		{
			get
			{
				if (GroupStack.Count == 0)
					return 0;
				else
					return GroupStack.Count;
			}
		}

		private AppLogger()
		{ }

		// public static void Info(string message, string componentName, string methodName)
		//{
		//     Info(message, componentName, methodName, CurrentGroup.Indent);
		// }

		#region Write functions

		public static void WriteGroup(string message, MethodBase callingMethod, NLog.LogLevel level, Guid groupID)
		{
			/*LogEntry log = new LogEntry();
			log.Message = FormatLogEntry(level, message, callingMethod, groupID, Indent-1);
			log.Categories.Add("General");
			log.Priority = 1;
			Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(log);*/
			if (level == NLog.LogLevel.Info)
				logger.Info(FormatLogEntry(LogLevel.Info, message, callingMethod, groupID, Indent-1));
			else if (level == NLog.LogLevel.Debug)
				logger.Debug(FormatLogEntry(LogLevel.Debug, message, callingMethod, groupID, Indent-1));
			else
				throw new NotSupportedException("Invalid level.");
		}
		
		public static void Error(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Error(message, callingMethod);
		}

		public static void Error(string message, MethodBase callingMethod)
		{
			
			/*LogEntry log = new LogEntry();
			log.Message = FormatLogEntry(LogLevel.Error, message, callingMethod, Guid.NewGuid(), Indent);
			log.Categories.Add("General");
			log.Priority = 1;
			Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(log);*/
			logger.Error(FormatLogEntry(LogLevel.Error, message, callingMethod, Guid.NewGuid(), Indent));
		}

		public static void Info(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Info(message, callingMethod);
		}

		public static void Info(string message, MethodBase callingMethod)
		{
			/*LogEntry log = new LogEntry();
			log.Message = FormatLogEntry(LogLevel.Info, message, callingMethod, Guid.NewGuid(), Indent);
			log.Categories.Add("General");
			log.Priority = 1;
			Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(log);*/
			logger.Info(FormatLogEntry(LogLevel.Info, message, callingMethod, Guid.NewGuid(), Indent));
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
			if (PerformLogging(callingMethod, LogLevel.Debug))
			{
				/*LogEntry log = new LogEntry();
				log.Message = FormatLogEntry(LogLevel.Debug, message, callingMethod, Guid.NewGuid(), Indent);
				log.Categories.Add("General");
				log.Priority = 1;
				log.Priority = 1;
				Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(log);*/
				logger.Info(FormatLogEntry(LogLevel.Debug, message, callingMethod, Guid.NewGuid(), Indent));
			}
		}

		#endregion
		
		
		private static string FormatLogEntry(
			LogLevel logLevel,
			string message,
			MethodBase callingMethod,
			Guid id,
			int indent)
		{
			
			//bool isThreadTitle = Indent == 3;
			
			StringBuilder logEntry = new StringBuilder();

            logEntry.Append("<Entry>\r\n");
			logEntry.AppendFormat("<ID>{0}</ID>\r\n", id.ToString());
            logEntry.AppendFormat("<GroupID>{0}</GroupID>\r\n", CurrentGroup != null ? CurrentGroup.ID : Guid.Empty);
            logEntry.AppendFormat("<Indent>{0}</Indent>\r\n", indent);
            logEntry.AppendFormat("<LogLevel>{0}</LogLevel>\r\n", logLevel);
            logEntry.AppendFormat("<Timestamp>{0}</Timestamp>\r\n", DateTime.Now);
            //logEntry.AppendFormat("<IsThreadTitle>{0}</IsThreadTitle>\r\n", isThreadTitle.ToString());
            logEntry.AppendFormat("<Component>{0}</Component>\r\n", EscapeLogData(callingMethod.DeclaringType.ToString()));
            logEntry.AppendFormat("<Method>{0}</Method>\r\n", EscapeLogData(callingMethod.Name));
            logEntry.AppendFormat("<Data>{0}</Data>\r\n", EscapeLogData(message));
            logEntry.AppendFormat("<StackTrace>{0}</StackTrace>\r\n", EscapeLogData(CreateStackTrace()));
            logEntry.Append("</Entry>\r\n");
			logEntry.AppendLine();

			return logEntry.ToString();
		}
		
		private static string CreateStackTrace()
		{
			StringBuilder builder = new StringBuilder();
			
			StackTrace stackTrace = new StackTrace();           // get call stack
			StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

			// write call stack method names
			foreach (StackFrame stackFrame in stackFrames)
			{
				builder.Append(stackFrame.ToString());   // write method name
				builder.Append("\n");   // write method name
			}
			
			return builder.ToString();
		}

		private static string EscapeLogData(string data)
		{
			return data.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("\"", "&quot;")
				.Replace("'", "&apos;");
		}

		static private LogGroup currentGroup;
		/// <summary>
		/// Gets/sets the current, deepest group in the tree.
		static public LogGroup CurrentGroup
		{
			get { return currentGroup; }
			set { currentGroup = value; }
		}

		static private Stack<LogGroup> groupStack = new Stack<LogGroup>();
		/// <summary>
		/// The current stack of groups.
		/// </summary>
		static public Stack<LogGroup> GroupStack
		{
			get { return groupStack; }
			set { groupStack = value; }
		}

		/*static public void Write(string message)
    {
        throw new NotImplementedException();
    }*/

		// static public void WriteLine(string message)
		/// {
		/*if (CurrentGroup != null)
                CurrentGroup.WriteLine(message);
            else
                Logger.Info(message);*/
		//   logger.Info(message);
		//}

		/*static public void Write(string category, string message)
    {
        throw new NotImplementedException();
    }

        static public void WriteLine(string category, string message)
    {
        throw new NotImplementedException();
    }*/

		static public LogGroup StartGroup(string summary)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();

			return StartGroup(summary, LogLevel.Info, callingMethod);
		}


		static public LogGroup StartGroup(string summary, LogLevel logLevel)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();

			return StartGroup(summary, logLevel, callingMethod);
		}

		static public LogGroup StartGroup(string summary, LogLevel logLevel, MethodBase callingMethod)
		{
			LogGroup newGroup = new LogGroup(summary, callingMethod);
			if (CurrentGroup == null)
				newGroup.ParentID = Guid.Empty;
			else
				newGroup.ParentID = CurrentGroup.ID;
			CurrentGroup = newGroup;
			
			newGroup.Start(callingMethod, logLevel);


			return newGroup;
		}

		static internal void EndGroup()
		{
			if (CurrentGroup != null)
			{
				CurrentGroup.End();
			}
		}

		static public void AddIndent()
		{
			//indent++;
		}

		static public void RemoveIndent()
		{
			//indent--;
		}

		static public void Push(LogGroup group)
		{
			CurrentGroup = group;
			if (GroupStack.Count == 0)
				group.IsThreadTitle = true;
			GroupStack.Push(group);
		}

		static public void Pop()
		{
			if (GroupStack.Count > 0)
			{
				GroupStack.Pop();

				if (GroupStack.Count > 1)
				{
					CurrentGroup = GroupStack.ToArray()[GroupStack.Count - 1];
				}
				else
					CurrentGroup = null;
			}
		}
		
		static public bool PerformLogging(MethodBase callingMethod, LogLevel level)
		{
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + ".Enabled"];
			if (value != null)
			{
				bool allDebugLoggingEnabled = ((string)value).ToLower() == "true"
					|| level != LogLevel.Debug;
				
				if (allDebugLoggingEnabled && IsEnabled(level, callingMethod))
				{
					return true;
				}
				
				// Defaults to false
				return false;
			}
			else // Defaults to true if no setting was found in the web.config.
				return true;
			
		}
		
		static public bool IsEnabled(NLog.LogLevel level, MethodBase callingMethod)
		{
			Type declaringType = callingMethod.DeclaringType;
			
			return IsEnabled(level, declaringType.Name);
		}
		
		static public bool IsEnabled(NLog.LogLevel level, string typeName)
		{
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + "." + typeName + ".Enabled"];
			
			
			if (value != null)
			{
				string setting = ((string)value);
				
				if (setting.ToLower() == false.ToString().ToLower())
					return false;
			}
			
			// Defaults to true if no setting was found in the web.config
			return true;
		}

	}

}
