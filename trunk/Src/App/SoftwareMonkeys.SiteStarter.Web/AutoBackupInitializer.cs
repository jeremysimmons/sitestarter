using System;
using System.Configuration;
using System.Net;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class AutoBackupInitializer
	{
		public AutoBackupInitializer()
		{
		}
		
		public void Initialize()
		{
			if (new AutoBackupChecker().AutoBackupDue())
			{
				// Extend the timeout to ensure there is no timeout error
				using (TimeoutExtender extender = TimeoutExtender.NewMinutes(60))
				{
					try
					{
						LaunchBackup();
					}
					catch (Exception ex)
					{
						LogWriter.Error(new Exception("An error occurred during the automatic backup.", ex));
					}
				}
			}
		}
		
		void LaunchBackup()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Launching backup."))
			{
				if (Config.IsInitialized)
				{
					string url = new UrlConverter().ToAbsolute(HttpContext.Current.Request.ApplicationPath + "/AutoBackup.aspx");
					
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
					
					request.Method = "GET";
					
					IAsyncResult result = request.BeginGetResponse(null, null);
				}
				else
				{
					LogWriter.Debug("!Config.IsInitialized - Skipping backup.");
				}
			}
		}
	}
}
