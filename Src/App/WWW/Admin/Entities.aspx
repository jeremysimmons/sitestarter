<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
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
	new EntitiesResetter().Reset();
	
	new EntityInitializer().Initialize();
	
	Result.Display(Resources.Language.EntitiesReset);
}

private bool CanView(EntityInfo info)
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

private bool IsAbstract(EntityInfo entity)
{
	Type type = EntityState.GetType(entity.TypeName);
	
	return type.IsAbstract;
}

private bool IsInterface(EntityInfo entity)
{
	Type type = EntityState.GetType(entity.TypeName);
	
	return type.IsInterface;
}

private bool IsConcrete(EntityInfo entity)
{
	return !IsAbstract(entity) && !IsInterface(entity);
}

private string GetNamespace(EntityInfo entity)
{
	Type type = EntityState.GetType(entity.TypeName);
	if (type != null)
		return type.Namespace;
	else
		return String.Empty;
}

private string GetAssemblyName(EntityInfo entity)
{
	Type type = EntityState.GetType(entity.TypeName);
	if (type != null)
		return type.Assembly.GetName().Name + ".dll";
	else
		return String.Empty;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>Entities</h1>
<cc:Result runat="Server"/>
<p>The following entities are currently cached in the system. Click "Reset" to rescan and refresh the entities.</p>
<p>&laquo; <a href="Cache.aspx">Index</a></p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			Entity
		</th>
		<th>
			Concrete
		</th>
		<th>
			Abstract
		</th>
		<th>
			Interface
		</th>
		<th>
			Namespace
		</th>
		<th>
			Assembly
		</th>
	</tr>
	<% foreach (EntityInfo entity in EntityState.Entities){ %>
	<tr>
		<td>
			<%= entity.TypeName %>
		</td>
		<td>
			<%= IsConcrete(entity) %>
		</td>
		<td>
			<%= IsAbstract(entity) %>
		</td>
		<td>
			<%= IsInterface(entity) %>
		</td>
		<td>
			<%= GetNamespace(entity) %>
		</td>
		<td>
			<%= GetAssemblyName(entity) %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>