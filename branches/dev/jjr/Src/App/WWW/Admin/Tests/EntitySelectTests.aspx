<%@ Page
	Language           = "C#"
	AutoEventWireup    = "true"
	ValidateRequest    = "false"
	EnableSessionState = "false"
%>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Loading the EntitySelectTests page.", NLog.LogLevel.Debug))
		{
			if (!IsPostBack)
			{
				AppLogger.Debug("!IsPostBack");
				
				AppLogger.Debug("DataBinding");
				DataBind();
			}
			else
				AppLogger.Debug("IsPostBack");
		}
	}

    protected void EntitySelect1_DataLoading(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = AppLogger.StartGroup("Loading the EntitySelect1 data.", NLog.LogLevel.Debug))
		{
        	EntitySelect1.DataSource = UserFactory.Current.GetUsers();
        }
    }
    
    private void RunButton_OnClick(object sender, EventArgs e)
    {
	    using (LogGroup logGroup = AppLogger.StartGroup("Processing RunButton.Click event.", NLog.LogLevel.Debug))
		{
	    	RunTest1();
	    	RunTest2();
	    	
	    	AppLogger.Debug(EntitySelect1Result.Text);
    	}
    }
    
    private void RunTest1()
    {
    	User[] users = EntitySelect1.SelectedEntities;
	    	
	    	if (users == null)
	    	{
	    		EntitySelect1Result.Text = "SelectedEntities == null";
	    		
	    	}
	    	else
	    	{
	    		if (users.Length == 0)
	    		{
	    			EntitySelect1Result.Text = "0 users selected";
	    		}
	    		else
	    		{
	    			EntitySelect1Result.Text = users.Length + " users selected";
	    		}
	    	}
    }
    
    private void RunTest2()
    {
    		User[] users = EntitySelect2.SelectedEntities;
	    	
	    	if (users == null)
	    	{
	    		EntitySelect2Result.Text = "SelectedEntities == null";
	    		
	    	}
	    	else
	    	{
	    		if (users.Length == 0)
	    		{
	    			EntitySelect2Result.Text = "0 users selected";
	    		}
	    		else
	    		{
	    			EntitySelect2Result.Text = users.Length + " users selected";
	    		}
	    	}
    }
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>EntitySelectTests</title>

		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />

		<link href="SoftwareMonkeys.SiteStarter.WWW.Admin.Tests.css" type="text/css" rel="stylesheet" />
		
	</head>
	<body>
		<form id="Form_EntitySelectTests" method="post" runat="server">

<div>
		Standard test:<br/>
			<ss:EntitySelect EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" Rows="8" Width="400px" runat="server"
                                      ValuePropertyName='Name' id="EntitySelect1" DisplayMode="Multiple" SelectionMode="Multiple" OnDataLoading='EntitySelect1_DataLoading'>
                                  </ss:EntitySelect><br/>
                                  <asp:Label runat="server" id="EntitySelect1Result"></asp:Label>
                                  </div>
                                  <div>
                                  Empty/no data:<br/>
                                  <ss:EntitySelect EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" Rows="8" Width="400px" runat="server"
                                      ValuePropertyName='Name' id="EntitySelect2" DisplayMode="Multiple" SelectionMode="Multiple" OnDataLoading='EntitySelect1_DataLoading'>
                                  </ss:EntitySelect>
                                  <br/>
                                  <asp:Label runat="server" id="EntitySelect2Result"></asp:Label>
                                  </div>
                                  <div>
                                  <asp:Button runat="server" id="RunButton" onclick="RunButton_OnClick" text="Run Tests"/>
                                  </div>
                                  
                                  

		</form>
	</body>
</html>
