<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
    if (Config.IsInitialized)
    {
        bool enableAuto = GetEnableAuto();

        if (enableAuto)
        {
            if (AutoBackupDue())
            {
                BackupHolder.Visible = true;
            }
            else
            {
                BackupHolder.Visible = false;
            }


        }
    }
    else
        this.Visible = false;
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
	if (Config.Application.Settings.ContainsKey("LastAutoBackup"))
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
	if (ConfigurationSettings.AppSettings["Backup.AutoInterval"] != null)
	{
		return Int32.Parse(ConfigurationSettings.AppSettings["Backup.AutoInterval"]);
	}
	else
		return 24;
}

private string GetDateLastBackup()
{
    if (!Config.IsInitialized)
        return String.Empty;
    
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

private string GetIFrameSource()
{
	return Request.ApplicationPath + "/Admin/AutoBackup.aspx?HideTemplate=true";
}

private string GetIFrame()
{
	return @"<iframe style='margin:0px;padding:3px;height:70px;width:95%;border:none;' src='" + GetIFrameSource() + "'/>";
}

</script>

<div style="font-size: smaller; padding: 5px;">
<%= Resources.Language.AutomaticBackup %><br/>

<i>
<%= Resources.Language.Previous %>: <%= GetDateLastBackup() %><br/>
<%= Resources.Language.Next %>: <%= GetRemainingTime() %>
</i>
</div>
<i>
<asp:Placeholder runat="server" id="BackupHolder">
<%= AutoBackupDue() ? GetIFrame() : String.Empty %>
</asp:Placeholder>
</i>