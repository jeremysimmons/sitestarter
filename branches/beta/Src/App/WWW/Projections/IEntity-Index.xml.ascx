<%@ Control Language="C#" ClassName="Default" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseXmlIndexProjection" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">

	private void Page_Init(object sender, EventArgs e)
	{
		// Get the type from the query string
		Type type = EntitiesUtilities.GetType(QueryStrings.Type);
	
		// Initialize
		Initialize(type);
	} 
                    
</script>