<%@ Page Language="C#" MasterPageFile="~/Site.master" ValidateRequest="false" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
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
    		// If no projection was found (specified by the query string)
            if (!Projector.FoundProjection)
            {
            	// TODO: Review whether a friendly error should be displayed
                Response.Redirect("Default.aspx");
            }
        }
    }

	private string GetEditUrl()
	{
		if (Projector.DataSource.Name != String.Empty)
			return Request.ApplicationPath + "/Admin/EditProjection.aspx?Projection=" + Projector.DataSource.Name;
		else
			throw new Exception("No name specified for projection.");
	}
	

</script>
<asp:Content ID="PageBody" ContentPlaceHolderID="Body" Runat="Server">
<% if (Authorisation.IsInRole("Administrator")){ %>
<div class="ProjectorMenu"><a href='<%= GetEditUrl() %>'><%= Resources.Language.EditThisProjection %></a></div>
<% } %>
<cc:ProjectorControl runat="server" id="Projector"></cc:ProjectorControl>

</asp:Content>

