<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Initialize Screenshots" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
	EnsureNotLive();

    	// This page initializes a session for the screenshot program to be able to view the program in action
    
    	// Load the administrator user
		User administrator = IndexStrategy.New<User>(false).Index<User>()[0];
	
		if (administrator == null)
			throw new Exception("administrator wasn't found.");

		// Sign the screenshot program in
        Authentication.SetAuthenticatedUsername(administrator.Username);
        
        // Send the screenshot program to the page it's taking the screenshot of
        Response.Redirect(Request.ApplicationPath + "/" + Request.QueryString["SendTo"]);
    }

	private void EnsureNotLive()
	{
		// If the path variation is String.Empty then its likely a live installation (ie. it's not local and it's not staging)
		if (Config.Application.PathVariation == String.Empty)
		{
			Result.DisplayError("Can't initialize screenshots in a live installation.");
			Response.Redirect(Request.ApplicationPath);
		}
	}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">

</asp:Content>

