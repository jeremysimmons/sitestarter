<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Navigation" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Authorisation.EnsureIsInRole("Administrator");
	
	ResetButton.DataBind();
	CreateButton.DataBind();
}

private void ResetButton_Click(object sender, EventArgs e)
{
	new ProjectionsResetter(this).Reset();
	
	new ProjectionsInitializer(this).Initialize();
	
	Result.Display(Resources.Language.ProjectionsReset);
}

private void CreateButton_Click(object sender, EventArgs e)
{
	Response.Redirect("EditProjection.aspx");
}


private bool CanView(ProjectionInfo info)
{
	if (info.TypeName == String.Empty && info.Action == String.Empty)
	{
		return true;
	}
	else if (EntityState.IsType(info.TypeName))
	{	
		Type type = EntityState.GetType(info.TypeName);

		return !type.IsInterface
			&& !type.IsAbstract;
	}
	else
		return true;
}

private string GetEditLink(ProjectionInfo projection)
{
	return "EditProjection.aspx?Projection=" + projection.Name;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a> &gt; <a id="CacheIndexLink" href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Cache.aspx" %>'><%= Resources.Language.Cache %></a></div>
<h1>Projections</h1>
<cc:Result runat="Server"/>
<p>The following projections are currently cached in the system. Click "Reset" to rescan and refresh the projections.</p>
<p><asp:button runat="server" id="CreateButton" text='<%# Resources.Language.CreateProjection %>' onclick="CreateButton_Click"/>&nbsp;<asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<p>Total Projections: <%= ProjectionState.Projections.Count %></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			File Name
		</th>
		<th>
			Action
		</th>
		<th>
			Type Name
		</th>
		<th>
			Format
		</th>
		<th>
			Title
		</th>
		<th>
			Category
		</th>
		<th>
			Folder
		</th>
		<th>	
		</th>
	</tr>
	<% foreach (ProjectionInfo projection in ProjectionState.Projections){ %>
	<tr>
		<td>
			<%= Path.GetFileName(projection.ProjectionFilePath) %>
		</td>
		<td>
			<%= projection.Action %>
		</td>
		<td>
			<%= projection.TypeName %>
		</td>
		<td>
			<%= projection.Format.ToString() %>
		</td>
		<td>
			<%= projection.MenuTitle %>
		</td>
		<td>
			<%= projection.MenuCategory %>
		</td>
		<td>
			/<%= Path.GetDirectoryName(projection.ProjectionFilePath).Replace(@"\", "/").Trim('/') %>/
		</td>
		<td>
			<a href='<%= GetEditLink(projection) %>'><%= Resources.Language.Edit %></a>
			
			<% if (CanView(projection)) { %>
			<a href='<%= new UrlCreator().CreateUrl(projection) %>' target="_blank"><%= Resources.Language.View %></a>
			<% } %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>