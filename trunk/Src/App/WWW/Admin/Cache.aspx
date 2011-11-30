<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Navigation" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Authorisation.EnsureIsInRole("Administrator");
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a></div>
<h1>Application Cache</h1>
<p>View the application cache using the links below.</p>
<h2>Entities</h2>
<ul>
<li><a href="Entities.aspx" id="EntitiesCacheLink">Entities</a> - Entities are records of information held as objects that can be stored, retrieved, and used by the rest of the application.</li>
</ul>
<h2>Business</h2>
<ul>
<li><a href="Strategies.aspx" id="StrategiesCacheLink">Strategies</a> - Business strategies take care of business logic.</li>
<li><a href="Reactions.aspx" id="ReactionsCacheLink">Reactions</a> - Reactions get triggered after a particular action.</li>
</ul>
<h2>Web/UI</h2>
<ul>
<li><a href="Projections.aspx" id="ProjectionsCacheLink">Projections</a> - Projections are managed "pages" contained in user controls that can be created/edited via the browser.</li>
<li><a href="Controllers.aspx" id="ControllersCacheLink">Controllers</a> - Controllers take care of standardised UI related functionality by controlling projections.</li>
<li><a href="Parts.aspx" id="PartsCacheLink">Parts</a> - Parts are sections of pages or projections that can be arranged by users.</li>
<li><a href="Elements.aspx" id="ElementsCacheLink">Elements</a> - Elements are managed server controls which can be displayed by referencing the short name, without even knowing the entire assembly and namespace.</li>
</ul>
</asp:Content>