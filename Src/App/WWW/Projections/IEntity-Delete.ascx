<%@ Control Language="C#" ClassName="DeleteProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseDeleteProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">

	private void Page_Init(object sender, EventArgs e)
	{
		// Get the type from the query string
		Type type = EntitiesUtilities.GetType(QueryStrings.Type);
	
		// Initialize
		Initialize(type);
	}

	// DeleteProjection class takes care of all functionality.
	// Functionality can be customised here if necessary both overriding Delete function
                    
</script>