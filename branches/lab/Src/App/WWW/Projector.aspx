<%@ Page Language="C#" MasterPageFile="~/Site.master" ValidateRequest="false" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
    
    protected override void OnInit(EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Initializing the projector.", NLog.LogLevel.Debug))
    	{
    		base.OnInit(e);
    	}
    }
    
    private void Page_Load(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Loading the projector.", NLog.LogLevel.Debug))
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

