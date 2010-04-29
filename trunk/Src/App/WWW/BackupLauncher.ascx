<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
    if (Config.IsInitialized)
    {
        bool enableAuto = false;
        if (ConfigurationSettings.AppSettings["Backup.EnableAuto"] != null)
            enableAuto = Convert.ToBoolean(ConfigurationSettings.AppSettings["Backup.EnableAuto"]);

        if (enableAuto)
        {
            string autoIntervalString = ConfigurationSettings.AppSettings["Backup.EnableAuto"];
            int autoInverval = Int32.Parse(ConfigurationSettings.AppSettings["Backup.AutoInterval"]);

            object lastAutoBackupSetting = Config.Application.Settings["LastAutoBackup"];

            if (lastAutoBackupSetting == null || lastAutoBackupSetting == String.Empty)
            {
                lastAutoBackupSetting = DateTime.MinValue;
            }
            DateTime lastAutoBackup = (DateTime)lastAutoBackupSetting;

            if (lastAutoBackup.AddHours(autoInverval) < DateTime.Now)
            {
                LauncherPanel.Visible = true;
            }
            else
            {
                LauncherPanel.Visible = false;
            }


        }
    }
    else
        this.Visible = false;
}

private string GetDateLastBackup()
{
    if (!Config.IsInitialized)
        return String.Empty;
    
    if (Config.Application.Settings["LastAutoBackup"] == null)
        return Resources.Language.Never;
    
    DateTime lastBackup = (DateTime)Config.Application.Settings["LastAutoBackup"];

    return lastBackup.ToString();
    //return DateTime.Now.Subtract(lastBackup).Days + " days";
}
</script>
<asp:Panel runat="server" id="LauncherPanel" visible ="false">
<script language="javascript">
    function LaunchBackup(close) {
        var url = '<%= Request.ApplicationPath + "/Admin/Backup.aspx?HideTemplate=true&Auto=true&AutoClose=" %>' + close;
        var win = window.open(url, "AutoBackup", "location=0,status=0,scrollbars=0,width=350,height=150");
    }

    LaunchBackup(true);
</script>
</asp:Panel>
<span style="font-size: smaller;">
Last automatic backup: <%= GetDateLastBackup() %>
<asp:Panel runat="server" >
<a href='javascript:LaunchBackup(false);'>Backup Now &raquo;</a>
</asp:Panel></span>