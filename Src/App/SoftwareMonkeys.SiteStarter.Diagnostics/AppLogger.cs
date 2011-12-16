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
	[Obsolete("All members of this class are obsolete but remain for backward compatibility.")]
	static public class AppLogger
	{
		
		#region Write functions

		
		[Obsolete("Use LogWriter.Error(...) instead.")]
		public static void Error(string message)
		{
			LogWriter.Error(message);
		}

		[Obsolete("Use LogWriter.Error(...) instead.")]
		public static void Error(string message, MethodBase callingMethod)
		{
			LogWriter.Error(message, callingMethod);
		}

		[Obsolete("Use LogWriter.Info(...) instead.")]
		public static void Info(string message)
		{
			LogWriter.Info(message);
		}

		[Obsolete("Use LogWriter.Info(...) instead.")]
		public static void Info(string message, MethodBase callingMethod)
		{
			LogWriter.Info(message, callingMethod);
		}

		[ Conditional("DEBUG") ]
		[Obsolete("Use LogWriter.Debug(...) instead.")]
		public static void Debug(string message)
		{
			LogWriter.Debug(message);
		}

		[ Conditional("DEBUG") ]
		[Obsolete("Use LogWriter.Debug(...) instead.")]
		public static void Debug(string message, MethodBase callingMethod)
		{
			LogWriter.Debug(message, callingMethod);
		}

		#endregion
		
		

		/// <summary>
		/// Gets/sets the current group.
		/// </summary>
		[Obsolete("Use DiagnosticState.CurrentGroup instead.")]
		static public LogGroup CurrentGroup
		{
			get {
				return DiagnosticState.CurrentGroup;
			}
			set { DiagnosticState.CurrentGroup = value; }
		}

		[Obsolete("Use LogGroup.Start(...) instead.")]
		static public LogGroup StartGroup(string summary)
		{
			return LogGroup.Start(summary);
		}


		[Obsolete("Use LogGroup.Start(...) instead.")]
		static public LogGroup StartGroup(string summary, NLog.LogLevel logLevel)
		{
			return StartGroup(summary, LogWriter.ConvertLevel(logLevel));
		}
		
		[Obsolete("Use LogGroup.Start(...) instead.")]
		static public LogGroup StartGroup(string summary, LogLevel logLevel)
		{
			return LogGroup.Start(summary, logLevel);
		}

		[Obsolete("Use LogGroup.Start(...) instead.")]
		static public LogGroup StartGroup(string summary, NLog.LogLevel logLevel, MethodBase callingMethod)
		{
			return StartGroup(summary, LogWriter.ConvertLevel(logLevel), callingMethod);
		}

		[Obsolete("Use LogGroup.Start(...) instead.")]
		static public LogGroup StartGroup(string summary, LogLevel logLevel, MethodBase callingMethod)
		{
			return LogGroup.Start(summary, logLevel, callingMethod);
		}
		
		
	}

}
