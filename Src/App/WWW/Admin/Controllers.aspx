<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Navigation" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Authorisation.EnsureIsInRole("Administrator");
	
	ResetButton.DataBind();
}

private void ResetButton_Click(object sender, EventArgs e)
{
	new ControllersResetter().Reset();
	
	new ControllersInitializer().Initialize();
	
	Result.Display(Resources.Language.ControllersReset);
}

private string GetShortTypeName(ControllerInfo strategy)
{
	Type type = Type.GetType(strategy.ControllerType);
	if (type != null)
		return type.Name;
	else
		return String.Empty;
}

private string GetNamespace(ControllerInfo controller)
{
	Type type = Type.GetType(controller.ControllerType);
	if (type != null)
		return type.Namespace;
	else
		return String.Empty;
}

private string GetAssemblyName(ControllerInfo controller)
{
	Type type = Type.GetType(controller.ControllerType);
	if (type != null)
		return type.Assembly.GetName().Name + ".dll";
	else
		return String.Empty;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a> &gt; <a id="CacheIndexLink" href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Cache.aspx" %>'><%= Resources.Language.Cache %></a></div>
<h1>Controllers</h1>
<cc:Result runat="Server"/>
<p>The following controllers are currently cached in the system. Click "Reset" to rescan and refresh the controllers.</p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<p>Total Controllers: <%= ControllerState.Controllers.Count %></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			Controller
		</th>
		<th>
			Entity Type
		</th>
		<th>
			Action
		</th>
		<th>
			Namespace
		</th>
		<th>
			Assembly
		</th>
	</tr>
	<% foreach (ControllerInfo strategy in ControllerState.Controllers){ %>
	<tr>
		<td>
			<%= GetShortTypeName(strategy) %>
		</td>
		<td>
			<%= strategy.TypeName %>
		</td>
		<td>
			<%= strategy.Action %>
		</td>
		<td>
			<%= GetNamespace(strategy) %>
		</td>
		<td>
			<%= GetAssemblyName(strategy) %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>