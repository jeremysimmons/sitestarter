using System;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class AutoBackupExecutor
	{
		public AutoBackupExecutor()
		{
		}
		
		public void Initialize()
		{
			if (AutoBackupChecker.Current.AutoBackupDue())
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


	}
}
