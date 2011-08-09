<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
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
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">

</asp:Content>

