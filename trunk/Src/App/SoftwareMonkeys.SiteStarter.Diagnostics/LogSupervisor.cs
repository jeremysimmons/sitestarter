using System;
using System.Reflection;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	public static class LogSupervisor
	{
		/// <summary>
		/// Checks whether the logging is enabled for the specified log level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		static public bool LoggingEnabled(NLog.LogLevel level)
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
		static public bool LoggingEnabled(MethodBase callingMethod, NLog.LogLevel level)
		{
			object value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + ".Enabled"];
			if (value != null)
			{
				bool allLoggingEnabled = Convert.ToBoolean(value.ToString()) != false;
				
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
		static private bool IsDebug()
		{
			bool isDebug = false;
			#if (DEBUG)
			isDebug = true;
			#else
			isDebug = false;
			#endif
			
			return isDebug;
		}
		
		/// <summary>
		/// Checks whether logging for the specified calling method is enabled according to the settings in the Web.config.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="callingMethod"></param>
		/// <returns></returns>
		static public bool IsEnabled(NLog.LogLevel level, MethodBase callingMethod)
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
		
		static public bool IsModeEnabled(NLog.LogLevel level)
		{
			if (level == NLog.LogLevel.Debug)
				return IsDebug();
			else
				return true;
		}

	}
}
