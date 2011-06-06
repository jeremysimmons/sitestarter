<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
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
	
	ResetButton.DataBind();
}

private void ResetButton_Click(object sender, EventArgs e)
{
	new ProjectionsResetter().Reset();
	
	new ProjectionsInitializer(this).Initialize();
	
	Result.Display(Resources.Language.ProjectionsReset);
}

private bool CanView(ProjectionInfo info)
{
	if (EntityState.IsType(info.TypeName))
	{	
		Type type = EntityState.GetType(info.TypeName);

		return !type.IsInterface
			&& !type.IsAbstract;
	}
	else
		return true;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>Projections</h1>
<cc:Result runat="Server"/>
<p>The following projections are currently cached in the system. Click "Reset" to rescan and refresh the projections.</p>
<p>&laquo; <a href="Cache.aspx">Index</a></p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			File Name
		</th>
		<th>
			Type Name
		</th>
		<th>
			Action
		</th>
		<th>
			Folder
		</th>
		<th>	
			View
		</th>
	</tr>
	<% foreach (ProjectionInfo projection in ProjectionState.Projections){ %>
	<tr>
		<td>
			<%= Path.GetFileName(projection.ProjectionFilePath) %>
		</td>
		<td>
			<%= projection.TypeName %>
		</td>
		<td>
			<%= projection.Action %>
		</td>
		<td>
			/<%= Path.GetDirectoryName(projection.ProjectionFilePath).Replace(@"\", "/").Trim('/') %>/
		</td>
		<td>
			<% if (CanView(projection)) { %>
			<a href='<%= Navigator.Current.GetLink(projection.Action, projection.TypeName) %>' target="_blank"><%= Resources.Language.View %> &raquo;</a>
			<% } %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>