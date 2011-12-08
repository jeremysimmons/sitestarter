<%@ Page Language="C#" Title="Prepare Auto Update" autoeventwireup="true" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="ICSharpCode.SharpZipLib.Zip" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
	
    protected void Page_Load(object sender, EventArgs e)
    {
    	if (EnsureAuthorised())    
        	Start();
	}
    
    private void Start()
    {
    	ApplicationBackup backup = new ApplicationBackup();
    	backup.PrepareForUpdate = true;
    	backup.Backup();    	
    }
    
    private bool EnsureAuthorised()
    {
    	string configFile = Server.MapPath(Request.ApplicationPath + "/AllowAutoUpdate.config");
    
    	bool isAuthorised = false;
    
    	using (StreamReader reader = new StreamReader(File.OpenRead(configFile)))
    	{
    		string content = reader.ReadToEnd().Trim();
    		
    		try
    		{
    			isAuthorised = Convert.ToBoolean(content);
    		}
    		catch (Exception ex)
    		{
    			// If there's an error just log it and presume the user is not authorised
    			LogWriter.Error(ex);
    		}
    	}
    	
    	if (!isAuthorised)
    		Authorisation.InvalidPermissionsRedirect();
    		
    	return isAuthorised;
    }

</script>
<html>
<body>
Ready
</body>
</html>
