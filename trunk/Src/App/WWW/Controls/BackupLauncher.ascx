<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
private void Page_Init(object sender, EventArgs e)
{
	//EnableViewState = false;
}

private void Page_Load(object sender, EventArgs e)
{
	using (LogGroup logGroup = LogGroup.Start("Loading the backup launcher control.", NLog.LogLevel.Debug))
	{
		if (Config.IsInitialized)
		{	
			bool enabled = GetEnableAuto();
			if (enabled)
			{
				bool needsBackup = AutoBackupDue();
				
				BackupRequiredHolder.Visible = needsBackup;
				
				LogWriter.Debug("Needs backup: " + needsBackup.ToString());
				
				if (needsBackup)
				{
					if (ScriptManager.IsInAsyncPostBack)
					{
						LogWriter.Debug("ScriptManager.IsInAsyncPostBack");
						LogWriter.Debug("Event target: " + Request["__EVENTTARGET"]);
						LogWriter.Debug("Timer client ID: " + AutoBackupTimer.UniqueID);
							
						//if (Request["__EVENTTARGET"].IndexOf(AutoBackupTimer.ClientID) > -1)
						//{
							LogWriter.Debug("Is update panel post back - Continuing");
						
							ExecuteBackup();
					
							PendingHolder.Visible = false;
							ExecutedHolder.Visible = true;
							AutoBackupTimer.Enabled = false;
						//}
						//else
						//	LogWriter.Debug("Is NOT update panel post back - Skipping");
					}
					else
					{
						LogWriter.Debug("!ScriptManager.IsInAsyncPostBack");
						
						PendingHolder.Visible = true;
						ExecutedHolder.Visible = false;
						AutoBackupTimer.Enabled = true;
					}
				}
			}
		}
    }
}

protected void AutoBackupTimer_Tick(object sender, EventArgs e)
{
    AutoBackupTimer.Enabled = false;
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

        	//Result.Display(Resources.Language.BackupComplete);

            Config.Application.Settings["LastAutoBackup"] = DateTime.Now;

			Config.Application.Save();
			
            //ConfigFactory<AppConfig>.SaveConfig(
            //    Server.MapPath(Request.ApplicationPath + "/App_Data"),
            //    (AppConfig)Config.Application,
        	//Config.Application.PathVariation);
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
        return Resources.Language.Never;

    //return lastBackup.ToString();
    //return DateTime.Now.Subtract(lastBackup).Days + " days";

	//TimeSpan span = lastAutoBackup.Subtract(DateTime.Now);
	TimeSpan span = DateTime.Now.Subtract(lastAutoBackup);

	string time = String.Empty;

	if (span.Days > 0)
        time = time + span.Days + " " + Resources.Language.Days.ToLower() + ", ";
	if (span.Hours > 0)
        time = time + span.Hours + " " + Resources.Language.Hours.ToLower() + ", ";
    if (span.Minutes > 0)
        time = time + span.Minutes + " " + Resources.Language.Minutes.ToLower() + ", ";
    time = time + span.Seconds + " " + Resources.Language.Seconds.ToLower() + " " + Resources.Language.Ago;

	return time;
}

private string GetRemainingTime()
{
	DateTime lastAutoBackup = GetLastAutoBackup();
	int interval = GetAutoInterval();

	if (lastAutoBackup == DateTime.MinValue)
		return Resources.Language.Now;

	TimeSpan span = lastAutoBackup.AddHours(interval).Subtract(DateTime.Now);
	//return DateTime.Now.Subtract(lastAutoBackup.AddHours(interval)).Hours.ToString();

	string time = String.Empty;
	if (span.Days > 0)
		time = time + span.Days + " " + Resources.Language.Days.ToLower() + ", ";
	if (span.Hours > 0)
        time = time + span.Hours + " " + Resources.Language.Hours.ToLower() + ", ";
    if (span.Minutes > 0)
        time = time + span.Minutes + " " + Resources.Language.Minutes.ToLower() + ", ";
    time = time + span.Seconds + " " + Resources.Language.Seconds.ToLower();

	return time;
}
</script>


        <asp:ScriptManager ID="ScriptManager" runat="server" />
			<asp:UpdatePanel ID="BackupPanel" runat="server">
                <ContentTemplate>
					<div class="AutoBackupSummary">
					<%= Resources.Language.AutomaticBackup %><br/>
					<i>
					<%= Resources.Language.Previous %>: <%= GetDateLastBackup() %><br/>
					<%= Resources.Language.Next %>: <%= GetRemainingTime() %>
					</i>
					</div>
					<asp:Timer ID="AutoBackupTimer" runat="server" Interval="10" ontick="AutoBackupTimer_Tick" />
                    <asp:placeholder runat="server" id="BackupRequiredHolder">
					<asp:placeholder runat="server" id="PendingHolder" visible="false">
						Executing backup...
					</asp:placeholder>
					
					<asp:placeholder runat="server" id="ExecutedHolder" visible="false">
						Backup Complete
					</asp:placeholder>
					</asp:placeholder>
                </ContentTemplate>
            </asp:UpdatePanel>