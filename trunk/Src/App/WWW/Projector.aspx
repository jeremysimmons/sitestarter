<%@ Page Language="C#" MasterPageFile="~/Site.master" ValidateRequest="false" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
    
    private void Page_Load(object sender, EventArgs e)
    {
        if (QueryStrings.Module != String.Empty && QueryStrings.ControlID != String.Empty)
        {            
            if (!Projector.FoundProjection)
            {
            	// TODO: Review whether a friendly error should be displayed
                Response.Redirect("Default.aspx");
            }
        }
    }


</script>
<asp:Content ID="PageBody" ContentPlaceHolderID="Body" Runat="Server">
<cc:ProjectorControl runat="server" id="Projector"></cc:ProjectorControl>
</asp:Content>

