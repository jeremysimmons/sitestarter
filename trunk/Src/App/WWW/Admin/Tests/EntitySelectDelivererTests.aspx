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
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
	public Guid EntityID;

	public void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Loading the EntitySelectTests page.", NLog.LogLevel.Debug))
		{
			if (!IsPostBack)
			{
				AppLogger.Debug("!IsPostBack");
				
				AppLogger.Debug("DataBinding");
				//DataBind();
				
				EntityID = Guid.NewGuid();
				
				IDField.Text = EntityID.ToString();
				TextField.Text = "Test Text";
				
				DataBind();
			}
			else
				AppLogger.Debug("IsPostBack");
		}
	}
    
    private void RunButton_OnClick(object sender, EventArgs e)
    {
	    using (LogGroup logGroup = AppLogger.StartGroup("Processing RunButton.Click event.", NLog.LogLevel.Debug))
		{
	    	
	    	
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
		ID: <asp:TextBox runat="server" id="IDField"/>
			<br/>
			Text: <asp:TextBox runat="server" id="TextField"/>
			<br/>
			Source ID: <asp:TextBox runat="server" id="SourceID" Text='<%# Deliverer1.RequesterEntityID %>'/>
			<br/>
			Title: <asp:TextBox runat="server" id="Title"/>
			<br/>
			Description: <asp:TextBox runat="server" id="Description"/>
			 <ss:EntitySelectDeliverer runat="server" id="Deliverer1" TransferFields="Title,Description"
			 	TextControlID="Title" EntityID='<%# EntityID %>'
                                  	
                                  	/>
                                  </div>
                                 
                                  <div>
                                  <asp:Button runat="server" id="RunButton" onclick="RunButton_OnClick" text="Run Tests"/>
                                  </div>
                                  

		</form>
	</body>
</html>
