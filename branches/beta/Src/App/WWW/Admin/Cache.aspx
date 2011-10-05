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
<li><a href="Entities.aspx" id="EntitiesCacheLink">Entities</a></li>
</ul>
<h2>Business</h2>
<ul>
<li><a href="Strategies.aspx" id="StrategiesCacheLink">Strategies</a></li>
<li><a href="Reactions.aspx" id="ReactionsCacheLink">Reactions</a></li>
</ul>
<h2>Web/UI</h2>
<ul>
<li><a href="Projections.aspx" id="ProjectionsCacheLink">Projections</a></li>
<li><a href="Parts.aspx" id="PartsCacheLink">Parts</a></li>
<li><a href="Controllers.aspx" id="ControllersCacheLink">Controllers</a></li>
</ul>
</asp:Content>