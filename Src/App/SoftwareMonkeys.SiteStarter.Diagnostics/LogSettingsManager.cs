using System;
using System.Collections.Generic;
using NLog;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to retrieve log settings from the Web.config file.
	/// </summary>
	public class LogSettingsManager
	{
        static public LogSettingsManager current;
		/// <summary>
		/// Gets the current instance of the log settings manager.
		/// </summary>
		static public LogSettingsManager Current
		{
			get {
				if (current == null)
					current = new LogSettingsManager();
				return current; }
		}
		
		private Dictionary<string, bool> defaults;
		public Dictionary<string, bool> Defaults
		{
			get {
				if (defaults == null)
				{
					defaults = new Dictionary<string, bool>();
					defaults["Debug"] = false;
					defaults["Error"] = true;
					defaults["Fatal"] = true;
					defaults["Info"] = true;
				}
				return defaults; }
			set { defaults = value; }
		}
		
		public LogSettingsManager()
		{
		}
		
		/// <summary>
		/// Checks whether the specified log level is enabled.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="bool"></param>
		/// <returns></returns>
		public virtual bool IsEnabled(LogLevel level)
		{
			return IsEnabled(level, IsEnabledByDefault(level));
		}
		
		/// <summary>
		/// Checks whether the specified log level is enabled.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="bool"></param>
		/// <returns></returns>
		public virtual bool IsEnabled(LogLevel level, bool defaultValue)
		{
			string value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + ".Enabled"];
			if (value != null && value != String.Empty)
			{
				return Convert.ToBoolean(value);
			}
			else
				return defaultValue;
		}
		
		/// <summary>
		/// Checks whether the specified log level is enabled.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public virtual bool IsEnabled(string key, LogLevel level)
		{
			return IsEnabled(key, level, IsEnabledByDefault(level));
		}
		
		/// <summary>
		/// Checks whether the specified log level is enabled.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="level"></param>
		/// <param name="bool"></param>
		/// <returns></returns>
		public virtual bool IsEnabled(string key, LogLevel level, bool defaultValue)
		{
			string value = ConfigurationSettings.AppSettings["Logging." + level.ToString() + "." + key + ".Enabled"];
			if (value != null && value != String.Empty)
			{
				return Convert.ToBoolean(value);
			}
			else
			{
				return IsEnabled(level, defaultValue);
			}
		}
		
		/// <summary>
		/// Gets the default "is enabled" setting for the provided log level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public virtual bool IsEnabledByDefault(LogLevel level)
		{
			if (Defaults.ContainsKey(level.ToString()))
				return Defaults[level.ToString()];
			else
				return false;
		}
	}
}
