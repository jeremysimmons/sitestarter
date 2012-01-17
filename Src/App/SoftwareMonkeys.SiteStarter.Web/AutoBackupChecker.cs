using System;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	///
	/// </summary>
	public class AutoBackupChecker
	{
		static private AutoBackupChecker current;
		static public AutoBackupChecker Current
		{
			get {
				if (current == null)
					current = new AutoBackupChecker();
				return current; }
		}
		
		public bool GetEnableAuto()
		{
			if (ConfigurationSettings.AppSettings["Backup.EnableAuto"] != null)
				return Convert.ToBoolean(ConfigurationSettings.AppSettings["Backup.EnableAuto"]);
			else
				return false;
		}

		public bool AutoBackupDue()
		{
			if (!GetEnableAuto())
				return false;
			
			DateTime lastAutoBackup = GetLastAutoBackup();
			int autoInterval = GetAutoInterval();
			// true if DateTime.Now is greater than last backup + interval
			return (DateTime.Now > lastAutoBackup.AddHours(autoInterval));
		}

		public DateTime GetLastAutoBackup()
		{
			if (StateAccess.IsInitialized
			    && StateAccess.State.ContainsApplication("LastAutoBackup"))
			{
				return (DateTime)StateAccess.State.GetApplication("LastAutoBackup");
			}
			else
			{
				return DateTime.Now;
			}
		}

		public int GetAutoInterval()
		{
			if (Config.IsInitialized
			    && ConfigurationSettings.AppSettings["Backup.AutoInterval"] != null)
			{
				return Int32.Parse(ConfigurationSettings.AppSettings["Backup.AutoInterval"]);
			}
			else
				return 24;
		}
	}
}
