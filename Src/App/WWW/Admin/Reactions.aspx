<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" assembly="SoftwareMonkeys.SiteStarter.Web" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Navigation" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Authorisation.EnsureIsInRole("Administrator");
	
	ResetButton.DataBind();
}

private void ResetButton_Click(object sender, EventArgs e)
{
	new ReactionsResetter().Reset();
	
	new ReactionInitializer().Initialize();
	
	Result.Display(Resources.Language.ReactionsReset);
}

private string GetShortTypeName(ReactionInfo reaction)
{
	Type type = Type.GetType(reaction.ReactionType);
	if (type != null)
		return type.Name;
	else
		return String.Empty;
}

private string GetNamespace(ReactionInfo reaction)
{
	Type type = Type.GetType(reaction.ReactionType);
	if (type != null)
		return type.Namespace;
	else
		return String.Empty;
}

private string GetAssemblyName(ReactionInfo reaction)
{
	Type type = Type.GetType(reaction.ReactionType);
	if (type != null)
		return type.Assembly.GetName().Name + ".dll";
	else
		return String.Empty;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a> &gt; <a id="CacheIndexLink" href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Cache.aspx" %>'><%= Resources.Language.Cache %></a></div>
<h1>Reactions</h1>
<cc:Result runat="Server"/>
<p>The following reactions are currently cached in the system. Click "Reset" to rescan and refresh the reactions.</p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<p>Total Reactions: <%= ReactionState.Reactions.Count %></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			Reaction
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
	<% foreach (ReactionInfo reaction in ReactionState.Reactions.ToArray()){ %>
	<tr>
		<td>
			<%= GetShortTypeName(reaction) %>
		</td>
		<td>
			<%= reaction.TypeName %>
		</td>
		<td>
			<%= reaction.Action %>
		</td>
		<td>
			<%= GetNamespace(reaction) %>
		</td>
		<td>
			<%= GetAssemblyName(reaction) %>
		</td>
	</tr>
	<% } %>
</table>
</asp:Content>