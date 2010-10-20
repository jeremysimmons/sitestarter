<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Login" EnableEventValidation="true"  %>
<%@ Register Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" TagPrefix="cc" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<script runat="server">
	public void Login_LoggingIn(object sender, LoginCancelEventArgs e)
	{
		// Cancel the automatic login as it is done manually
		e.Cancel = true;
	
		if (Authentication.SignIn(Login.UserName, Login.Password))
			Response.Redirect("Default.aspx");
		else
			Result.Display(Resources.Language.InvalidCredentials);
	}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
    <cc:Result runat="server"/>
    <asp:Login ID="Login" runat="server" DestinationPageUrl="Default.aspx" OnLoggingIn="Login_LoggingIn">
        <TitleTextStyle CssClass="Heading2" HorizontalAlign="Left" />
    </asp:Login>
</asp:Content>
