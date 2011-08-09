<%@ Control Language="C#" ClassName="SignInProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
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
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
			DataBind();
	}

	public void Login_LoggingIn(object sender, LoginCancelEventArgs e)
	{
		// Cancel the automatic login as it is done manually
		e.Cancel = true;
	
		if (Authentication.SignInAndRedirect(Login.UserName, Login.Password, Login.RememberMeSet))
			Response.Redirect("Default.aspx");
		else
			Result.DisplayError(Resources.Language.InvalidCredentials);
	}
</script>
	<h1><%= Resources.Language.SignIn %></h1>
    <cc:Result runat="server"/>
	<p><%= Resources.Language.SignInIntro %></p>
    <% if (Config.Application.Settings.GetBool("EnableUserRegistration")) { %>
    <p><%= Resources.Language.DontHaveAnAccount %> <a href='<%= Navigator.GetLink("Register", "User") %>'><%= Resources.Language.RegisterNow %> &raquo;</a></p>
    <% } %>
    <asp:Login ID="Login" runat="server" DestinationPageUrl="Default.aspx" OnLoggingIn="Login_LoggingIn" titletext='<%# Resources.Language.SignInDetails %>'>
        <TitleTextStyle CssClass="Heading2" HorizontalAlign="Left" />
    </asp:Login>
    <p><a href='<%= Navigator.GetLink("Recover", "Password") %>'><%= Resources.Language.ForgotMyPassword %></a></p>