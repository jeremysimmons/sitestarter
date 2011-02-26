using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	public class AppLogger
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private static int indent;
		/// <summary>
		/// Gets/sets the indent of the current log entries.
		/// </summary>
		public static int Indent
		{
			get { return indent; }
		}

		private AppLogger()
		{ }

		// public static void Info(string message, string componentName, string methodName)
		//{
		//     Info(message, componentName, methodName, CurrentGroup.Indent);
		// }

		#region Write functions

		public static void Error(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Error(message, callingMethod);
		}

		public static void Error(string message, MethodBase callingMethod)
		{
			logger.Info(FormatLogEntry(LogLevel.Error, message, callingMethod, Indent));
		}

		public static void Info(string message)
		{
			MethodBase callingMethod = Reflector.GetCallingMethod();
			Info(message, callingMethod);
		}

		public static void Info(string message, MethodBase callingMethod)
		{
			logger.Info(FormatLogEntry(LogLevel.Info, message, callingMethod, Indent));
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
			if (PerformLogging(LogLevel.Debug))
			{
				logger.Info(FormatLogEntry(LogLevel.Debug, message, callingMethod, Indent));
			}
		}

		#endregion

		private static string FormatLogEntry(
			LogLevel logLevel,
			string message,
			MethodBase callingMethod,
			int indent)
		{
			StringBuilder logEntry = new StringBuilder();

			logEntry.Append("<Entry>\n");
			logEntry.AppendFormat("<GroupID>{0}</GroupID>\n", CurrentGroup != null ? CurrentGroup.ID : Guid.Empty);
			logEntry.AppendFormat("<LogLevel>{0}</LogLevel>\n", logLevel);
			logEntry.AppendFormat("<Timestamp>{0}</Timestamp>\n", DateTime.Now);
			logEntry.AppendFormat("<Indent>{0}</Indent>\n", indent);
			logEntry.AppendFormat("<Component>{0}</Component>\n", EscapeLogData(callingMethod.DeclaringType.ToString()));
			logEntry.AppendFormat("<Method>{0}</Method>\n", EscapeLogData(callingMethod.Name));
			logEntry.AppendFormat("<Data>{0}</Data>\n", EscapeLogData(message));
			logEntry.AppendFormat("<StackTrace>{0}</StackTrace>\n", EscapeLogData(CreateStackTrace()));
			logEntry.Append("</Entry>\n");
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
			newGroup.Start(callingMethod, logLevel);

			currentGroup = newGroup;

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
			indent++;
		}

		static public void RemoveIndent()
		{
			indent--;
		}

		static public void Push(LogGroup group)
		{
			CurrentGroup = group;
			GroupStack.Push(group);
		}

		static public void Pop()
		{
			if (GroupStack.Count > 0)
			{
				GroupStack.Pop();

				if (GroupStack.Count > 1)
				{
					currentGroup = GroupStack.ToArray()[GroupStack.Count - 1];
				}
				else
					currentGroup = null;

			}
		}
		
		static public bool PerformLogging(LogLevel level)
		{
			if (ConfigurationSettings.AppSettings["EnableDebugLogging"].ToLower() == "true"
			    || level != LogLevel.Debug)
			{
				return true;
			}
			else
				return false;
		}

	}

}