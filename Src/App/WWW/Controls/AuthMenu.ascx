<%@ Control Language="C#" ClassName="AuthMenu" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
		if (!QueryStrings.HideTemplate)
				AuthenticationHolder.DataBind();
}
</script>
<asp:placeholder runat="server" id="AuthenticationHolder">
        	<asp:placeholder runat="server" visible='<%# AuthenticationState.IsAuthenticated %>'>
                <%= Resources.Language.YouAreLoggedInAs %>:
                <%= AuthenticationState.IsAuthenticated ? AuthenticationState.Username : String.Empty %>&nbsp;(<a href='<%= Request.ApplicationPath + "/User/SignOut.aspx" %>'><%= Resources.Language.SignOut %></a>)
            </asp:placeholder>
            <asp:placeholder runat="server" visible='<%# !AuthenticationState.IsAuthenticated %>'>
                <a href='<%= Request.ApplicationPath + "/User/SignIn.aspx" %>'>Sign In</a>
                <asp:placeholder runat="server" visible='<%# Config.Application.Settings.GetBool("EnableUserRegistration") %>'>|
                	<a href='<%= Request.ApplicationPath + "/User/Register.aspx" %>'>Register</a>
                </asp:placeholder>
            </asp:placeholder>
        </asp:placeholder>
