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
	public void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Loading the EntityTreeTests page.", NLog.LogLevel.Debug))
		{
			if (!IsPostBack)
			{
				AppLogger.Debug("!IsPostBack");
				
				EntityTree1.DataSource = UserFactory.Current.GetUsers();
				
				UserFactory.Current.Activate((User[])EntityTree1.DataSource, "Roles");
				
				AppLogger.Debug("DataBinding");
				DataBind();
			}
			else
				AppLogger.Debug("IsPostBack");
		}
	}

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>EntityTreeTests</title>

		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />

		<link href="SoftwareMonkeys.SiteStarter.WWW.Admin.Tests.css" type="text/css" rel="stylesheet" />
		
	</head>
	<body>
		<form id="Form_EntityTreeTests" method="post" runat="server">

<div>
		Standard test:<br/>
			<ss:EntityTree NavigateUrl="EntityTreeTests.aspx?EntityID=${Entity.ID}" BranchesProperty="Roles" EntityType="SoftwareMonkeys.SiteStarter.Entities.BaseEntity, SoftwareMonkeys.SiteStarter.Entities" Rows="8" Width="400px" runat="server"
                                      ValuePropertyName='Name' NoDataText="No users found." id="EntityTree1">
                                  </ss:EntityTree><br/>
                                  </div>
	<div>
		No data text:<br/>
			<ss:EntityTree NavigateUrl="EntityTreeTests.aspx?EntityID=${Entity.ID}" EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" Rows="8" Width="400px" runat="server"
                                      ValuePropertyName='Name' NoDataText="No users found." id="EntityTree2">
                                  </ss:EntityTree><br/>
                                  </div>
                                  

		</form>
	</body>
</html>
