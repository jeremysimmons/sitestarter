<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="Db4objects.Db4o" %>
<script runat="server">
	public IEntity[] DataSource = new IEntity[] {};

    protected void Page_Load(object sender, EventArgs e)
    {
    	EnsureAuthorised();
    
    	if (Request.QueryString["SourceType"] == null)
    		throw new ArgumentException("No source type specified by the query string.");
    	
    	if (Request.QueryString["SourceID"] == null)
    		throw new ArgumentException("No source ID specified by the query string.");
    	
    	if (Request.QueryString["ReferenceProperty"] == null)
    		throw new ArgumentException("No reference property specified by the query string.");
    	
    	string sourceType = Request.QueryString["SourceType"];
    	Guid sourceID = new Guid(Request.QueryString["SourceID"]);
    	string referenceProperty = Request.QueryString["ReferenceProperty"];
    	
    	ShowReferences(sourceType, sourceID, referenceProperty);
    	                                   
    }
    
	private void ShowReferences(string sourceType, Guid sourceID, string referenceProperty)
	{
		IEntity source = RetrieveStrategy.New(sourceType, false).Retrieve("ID", sourceID);
		
		Type referenceType = EntitiesUtilities.GetReferenceType(source.GetType(), referenceProperty);
		
		EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(EntityState.GetType(sourceType), sourceID, referenceProperty, referenceType, true);
		
		DataSource = references.GetReferencedEntities(source);
	}
	
    private void EnsureAuthorised()
    {
    	bool isAuthorised = false;
    
    	if (ConfigurationSettings.AppSettings["SecureData"] == null
    		|| ConfigurationSettings.AppSettings["SecureData"].ToLower() != "false")
    	{
    		Authorisation.EnsureIsInRole("Administrator");
    	}
    }

</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<% foreach (IEntity entity in DataSource) { %>
<h2><%= entity.GetType().FullName %></h2>
<%= entity.ToString() %>
<% } %>
</asp:Content>