using System;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.State;

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
			using (LogGroup logGroup = LogGroup.StartDebug("Executing backup."))
			{
				if (Config.IsInitialized)
				{
					LogWriter.Debug("Config.IsInitialized - Continuing backup.");
					
					ApplicationBackup appBackup = new ApplicationBackup();
					
					appBackup.Backup();

					StateAccess.State.SetApplication("LastAutoBackup", DateTime.Now);
				}
				else
				{
					LogWriter.Debug("!Config.IsInitialized - Skipping backup.");
				}
			}
		}


	}
}
