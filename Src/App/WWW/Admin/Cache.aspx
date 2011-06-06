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
<h1>Application Cache</h1>
<p>View the application cache using the links below.</p>
<h2>Entities</h2>
<ul>
<li><a href="Entities.aspx">Entities</a></li>
</ul>
<h2>Business</h2>
<ul>
<li><a href="Strategies.aspx">Strategies</a></li>
<li><a href="Reactions.aspx">Reactions</a></li>
</ul>
<h2>Web/UI</h2>
<ul>
<li><a href="Projections.aspx">Projections</a></li>
<li><a href="Parts.aspx">Parts</a></li>
<li><a href="Controllers.aspx">Controllers</a></li>
</ul>
</asp:Content>