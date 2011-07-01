using System;
using System.Reflection;

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
		
		private LogSettingsManager settingsManager;
		/// <summary>
		/// Gets/sets an instance of the LogSettingsManager used to check the log settings in the web.config file.
		/// </summary>
		public LogSettingsManager SettingsManager
		{
			get {
				if (settingsManager == null)
					settingsManager = LogSettingsManager.Current;
				return settingsManager; }
			set { settingsManager = value; }
		}
		
		/// <summary>
		/// Checks whether the logging is enabled for the specified log level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool LoggingEnabled(LogLevel level)
		{
			// If the level is debug and the compile mode is NOT debug then return false regardless of the settings
			if (level == LogLevel.Debug && !IsDebug())
				return false;
			else
				return SettingsManager.IsEnabled(level);			
		}
		
		/// <summary>
		/// Checks whether the logging is enabled for the specified log and the specified type.
		/// </summary>
		/// <param name="callingTypeName"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool LoggingEnabled(string callingTypeName, LogLevel level)
		{
			bool modeIsEnabled = IsModeEnabled(level);
			
			bool typeIsEnabled = IsEnabled(callingTypeName, level);
			
			if (modeIsEnabled
			    && typeIsEnabled)
			{
				return true;
			}
			else
			{
				return false;
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
		/// Checks whether logging for the specified type is enabled according to the settings in the Web.config.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool IsEnabled(string typeName, LogLevel level)
		{
			bool isTypeEnabled = SettingsManager.IsEnabled(typeName, level);
			
			bool isModeEnabled = IsModeEnabled(level);
			
			return isModeEnabled && isTypeEnabled;
		}
		
		/// <summary>
		/// Checks whether logging for the current build mode is enabled.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool IsModeEnabled(LogLevel level)
		{
			if (level == LogLevel.Debug)
				return IsDebug();
			else
				return SettingsManager.IsEnabled(level);
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
