using System;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Description of AutoBackupLauncher.
	/// </summary>
	public class AutoBackupInitializer
	{
		public AutoBackupInitializer()
		{
		}
		
		public void Initialize()
		{
			if (AutoBackupDue())
			{
				// Extend the timeout to ensure there is no error
				using (TimeoutExtender extender = TimeoutExtender.NewMinutes(60))
				{
					try
					{
						ExecuteBackup();
					}
					catch (Exception ex)
					{
						LogWriter.Error(new Exception("An error occurred during the automatic backup.", ex));
					}
				}
			}
		}
		
		void ExecuteBackup()
		{
			using (LogGroup logGroup = LogGroup.Start("Executing backup.", NLog.LogLevel.Debug))
			{
				if (Config.IsInitialized)
				{
					LogWriter.Debug("Config.IsInitialized - Continuing backup.");
					
					ApplicationBackup appBackup = new ApplicationBackup();
					
					appBackup.Backup();

					Config.Application.Settings["LastAutoBackup"] = DateTime.Now;

					Config.Application.Save();
				}
				else
				{
					LogWriter.Debug("!Config.IsInitialized - Skipping backup.");
				}
			}
		}

		private bool GetEnableAuto()
		{
			if (ConfigurationSettings.AppSettings["Backup.EnableAuto"] != null)
				return Convert.ToBoolean(ConfigurationSettings.AppSettings["Backup.EnableAuto"]);
			else
				return false;
		}

		private bool AutoBackupDue()
		{
			if (!GetEnableAuto())
				return false;
			
			DateTime lastAutoBackup = GetLastAutoBackup();
			int autoInterval = GetAutoInterval();
			// true if DateTime.Now is greater than last backup + interval
			return (DateTime.Now > lastAutoBackup.AddHours(autoInterval));
		}

		private DateTime GetLastAutoBackup()
		{
			if (Config.IsInitialized
			    && Config.Application.Settings.ContainsKey("LastAutoBackup"))
			{
				return (DateTime)Config.Application.Settings["LastAutoBackup"];
			}
			else
			{
				return DateTime.MinValue;
			}
		}

		private int GetAutoInterval()
		{
			if (Config.IsInitialized
			    && ConfigurationSettings.AppSettings["Backup.AutoInterval"] != null)
			{
				return Int32.Parse(ConfigurationSettings.AppSettings["Backup.AutoInterval"]);
			}
			else
				return 24;
		}

		private string GetDateLastBackup()
		{
			if (!Config.IsInitialized)
				return "N/A";
			
			DateTime lastAutoBackup = GetLastAutoBackup();

			if (lastAutoBackup == DateTime.MinValue)
				return DynamicLanguage.GetText("Never");
				
			TimeSpan span = DateTime.Now.Subtract(lastAutoBackup);

			string time = String.Empty;

			if (span.Days > 0)
				time = time + span.Days + " " + DynamicLanguage.GetText("Days").ToLower() + ", ";
			if (span.Hours > 0)
				time = time + span.Hours + " " + DynamicLanguage.GetText("Hours").ToLower() + ", ";
			if (span.Minutes > 0)
				time = time + span.Minutes + " " + DynamicLanguage.GetText("Minutes").ToLower() + ", ";
			time = time + span.Seconds + " " + DynamicLanguage.GetText("Seconds") + " " + DynamicLanguage.GetText("Ago");

			return time;
		}
	}
}
