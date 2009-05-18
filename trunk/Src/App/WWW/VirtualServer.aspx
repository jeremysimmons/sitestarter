<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" ValidateRequest="false" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
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
            
            VirtualServerState.VirtualServerName = Request.QueryString["Server"];
            if (VirtualServerState.VirtualServerName != String.Empty)
                server = VirtualServerFactory.GetVirtualServerByName(VirtualServerState.VirtualServerName);

            if (server != null)
                VirtualServerState.VirtualServerID = VirtualServerState.VirtualServerID;

            FormsAuthentication.SignOut();

            Response.Redirect("Default.aspx");
        }
    }


</script>
<asp:Content ID="PageBody" ContentPlaceHolderID="Body" Runat="Server">

</asp:Content>

