<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" ValidateRequest="false" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<script runat="server">
    
    private void Page_Load(object sender, EventArgs e)
    {
        if (QueryStrings.Action == "Create")
        {
            Response.Redirect("CreateVirtualServer.aspx");
        }
        else if (QueryStrings.Action == "Switch")
        {
            VirtualServer server = null;
            
            string name = Request.QueryString["Server"];
            if (name == "Default")
            	name = String.Empty;
            else
	        {
	            if (name != String.Empty)
                    server = VirtualServerFactory.Current.GetVirtualServerByName(name);
            }

            if (server != null)
                VirtualServerState.Switch(name, server.ID);
            else
            	VirtualServerState.Switch(null, Guid.Empty);//String.Empty, Guid.Empty);

            FormsAuthentication.SignOut();

            Response.Redirect("Members/Login.aspx");
        }
    }


</script>
<asp:Content ID="PageBody" ContentPlaceHolderID="Body" Runat="Server">

</asp:Content>

