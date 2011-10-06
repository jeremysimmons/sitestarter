<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Navigation" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Elements" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Authorisation.EnsureIsInRole("Administrator");
	
	ResetButton.DataBind();
}

private void ResetButton_Click(object sender, EventArgs e)
{
	new ElementsResetter().Reset();
	
	new ElementsInitializer().Initialize();
	
	Result.Display(Resources.Language.ElementsReset);
}

private string GetShortTypeName(ElementInfo strategy)
{
	Type type = Type.GetType(strategy.ElementType);
	if (type != null)
		return type.Name;
	else
		return String.Empty;
}

private string GetNamespace(ElementInfo element)
{
	Type type = Type.GetType(element.ElementType);
	if (type != null)
		return type.Namespace;
	else
		return String.Empty;
}

private string GetAssemblyName(ElementInfo element)
{
	Type type = Type.GetType(element.ElementType);
	if (type != null)
		return type.Assembly.GetName().Name + ".dll";
	else
		return String.Empty;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a> &gt; <a id="CacheIndexLink" href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Cache.aspx" %>'><%= Resources.Language.Cache %></a></div>
<h1>Elements</h1>
<cc:Result runat="Server"/>
<p>The following elements are currently cached in the system. Click "Reset" to rescan and refresh the elements.</p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<p>Total Elements: <%= ElementState.Elements.Count %></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			Element
		</th>
		<th>
			Name
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
	<% foreach (ElementInfo element in ElementState.Elements){ %>
	<tr>
		<td>
			<%= GetShortTypeName(element) %>
		</td>
		<td>
			<%= element.Name %>
		</td>
		<td>
			<%= element.TypeName %>
		</td>
		<td>
			<%= element.Action %>
		</td>
		<td>
			<%= GetNamespace(element) %>
		</td>
		<td>
			<%= GetAssemblyName(element) %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>