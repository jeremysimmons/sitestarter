using System;
using System.Reflection;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	public class LogSupervisor
	{
		private ModeDetector modeDetector;
		/// <summary>
		/// Gets/sets the component used to detect the current build mode.
		/// </summary>
		public ModeDetector ModeDetector
		{
			get {
				if (modeDetector == null)
					modeDetector = new ModeDetector();
				return modeDetector; }
			set { modeDetector = value; }
		}
		
		/// <summary>
		/// Checks whether the logging is enabled for the specified log level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool LoggingEnabled(NLog.LogLevel level)
		{
			if (level == NLog.LogLevel.Debug && !IsDebug())
				return false;
			
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + ".Enabled"];
			if (value != null)
			{
				bool loggingEnabled = ((string)value).ToLower() == "true";
				
				return loggingEnabled;
			}
			else
				return true;
			
		}
		
		/// <summary>
		/// Checks whether the logging is enabled for the specified log and the specified method.
		/// </summary>
		/// <param name="callingMethod"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool LoggingEnabled(MethodBase callingMethod, NLog.LogLevel level)
		{
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + ".Enabled"];
			if (value != null)
			{
				bool allLoggingEnabled = value == null
					|| Convert.ToBoolean(value.ToString()) != false;
				
				if (allLoggingEnabled && IsModeEnabled(level) && IsEnabled(level, callingMethod))
				{
					return true;
				}
				
				// Defaults to false
				return false;
			}
			else 
			{
				// Defaults to true if no setting was found in the web.config.
				return true;
			}
		}
		
		/// <summary>
		/// Checks whether the application is currently compiled in debug mode.
		/// </summary>
		/// <returns></returns>
		private bool IsDebug()
		{
			bool isDebug = ModeDetector.IsDebug;
			
			return isDebug;
		}
		
		/// <summary>
		/// Checks whether logging for the specified calling method is enabled according to the settings in the Web.config.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="callingMethod"></param>
		/// <returns></returns>
		public bool IsEnabled(NLog.LogLevel level, MethodBase callingMethod)
		{
			// If the callingMethod parameter is null then logging is disabled
			if (callingMethod == null)
				return false;
			
			Type declaringType = callingMethod.DeclaringType;
			
			return IsEnabled(level, declaringType.Name);
		}
		
		/// <summary>
		/// Checks whether logging for the specified type is enabled according to the settings in the Web.config.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public bool IsEnabled(NLog.LogLevel level, string typeName)
		{
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + "." + typeName + ".Enabled"];
			
			
			if (value != null)
			{
				string setting = ((string)value);
				
				if (setting.ToLower() == false.ToString().ToLower())
					return false;
			}
			
			return IsModeEnabled(level);
		}
		
		/// <summary>
		/// Checks whether logging for the current build mode is enabled.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool IsModeEnabled(NLog.LogLevel level)
		{
			if (level == NLog.LogLevel.Debug)
				return IsDebug();
			else
				return true;
		}
		
		/// <summary>
		/// Checks whether the provided group can be popped from the list.
		/// </summary>
		/// <param name="endingGroup"></param>
		/// <param name="currentGroup"></param>
		/// <returns></returns>
		public bool CanPop(LogGroup endingGroup, LogGroup currentGroup)
		{
			if (endingGroup == null)
				throw new ArgumentNullException("endingGroup");
			
			return currentGroup != null
				&& DiagnosticState.GroupStack.Count > 0
				&& currentGroup.ID != Guid.Empty
				&& endingGroup.ParentID != currentGroup.ID
				&& HasParent(currentGroup, endingGroup.ID); // If the ending group is a parent of the current group then it matches
		}
		
		
		/// <summary>
		/// Checks wether the provided log group has a parent with the specified ID.
		/// </summary>
		/// <param name="logGroup"></param>
		/// <param name="parentID"></param>
		/// <returns></returns>
		public bool HasParent(LogGroup logGroup, Guid parentID)
		{
			if (logGroup == null)
				throw new ArgumentNullException("logGroup");
			
			bool hasParent = false;
			
			LogGroup l = logGroup;
			
			while (l != null && hasParent == false)
			{
				if (l != null && l.ID == parentID)
					hasParent = true;
				
				l = l.Parent;
			}
			
			return hasParent;
		}
	}
}
