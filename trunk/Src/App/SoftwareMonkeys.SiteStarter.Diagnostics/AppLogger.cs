using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.State;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	static public class AppLogger
	{
		
		#region Write functions

		
		public static void Error(string message)
		{
			LogWriter.Error(message);
		}

		public static void Error(string message, MethodBase callingMethod)
		{
			LogWriter.Error(message, callingMethod);
		}

		public static void Info(string message)
		{
			LogWriter.Info(message);
		}

		public static void Info(string message, MethodBase callingMethod)
		{
			LogWriter.Info(message, callingMethod);
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message)
		{
			LogWriter.Debug(message);
		}

		[ Conditional("DEBUG") ]
		public static void Debug(string message, MethodBase callingMethod)
		{
			LogWriter.Debug(message, callingMethod);
		}

		#endregion
		
		

		/// <summary>
		/// Gets/sets the current group.
		/// </summary>
		static public LogGroup CurrentGroup
		{
			get {
				return DiagnosticState.CurrentGroup;
			}
			set { DiagnosticState.CurrentGroup = value; }
		}

		/// <summary>
		/// The current stack of groups.
		/// </summary>
		static public StateStack<LogGroup> GroupStack
		{
			get { return DiagnosticState.GroupStack; }
			set { DiagnosticState.GroupStack = value; }
		}
		
		/*static private bool IsLevel(LogLevel level)
		{
			bool isDebug = false;
			#if (DEBUG)
			isDebug = true;
			#else
			isDebug = false;
			#endif
			
			return isDebug;
		}*/

		static public LogGroup StartGroup(string summary)
		{
			return LogGroup.Start(summary);
		}


		static public LogGroup StartGroup(string summary, LogLevel logLevel)
		{
			return LogGroup.Start(summary, logLevel);
		}

		static public LogGroup StartGroup(string summary, LogLevel logLevel, MethodBase callingMethod)
		{
			return LogGroup.Start(summary, logLevel, callingMethod);
		}
		
		
	}

}
