<%@ Control Language="C#" ClassName="CreateEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
	public void Login_LoggingIn(object sender, LoginCancelEventArgs e)
	{
		// Cancel the automatic login as it is done manually
		e.Cancel = true;
	
		if (Authentication.SignIn(Login.UserName, Login.Password))
			Response.Redirect("Default.aspx");
		else
			Result.DisplayError(Resources.Language.InvalidCredentials);
	}
</script>
    <cc:Result runat="server"/>
    <asp:Login ID="Login" runat="server" DestinationPageUrl="Default.aspx" OnLoggingIn="Login_LoggingIn">
        <TitleTextStyle CssClass="Heading2" HorizontalAlign="Left" />
    </asp:Login>