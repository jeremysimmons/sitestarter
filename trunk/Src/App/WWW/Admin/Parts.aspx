<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Parts" %>
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
	new PartsResetter().Reset();
	
	new PartsInitializer(this).Initialize();
	
	Result.Display(Resources.Language.PartsReset);
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>Parts</h1>
<cc:Result runat="Server"/>
<p>The following parts are currently cached in the system. Click "Reset" to rescan and refresh the parts.</p>
<p><a href="Cache.aspx" id="CacheIndexLink">&laquo; Index</a></p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<table class="Panel" width="600">
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
	</tr>
	<% foreach (PartInfo part in PartState.Parts){ %>
	<tr>
		<td>
			<%= Path.GetFileName(part.PartFilePath) %>
		</td>
		<td>
			<%= part.TypeName %>
		</td>
		<td>
			<%= part.Action %>
		</td>
		<td>
			/<%= Path.GetDirectoryName(part.PartFilePath).Replace(@"\", "/").Trim('/') %>/
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>