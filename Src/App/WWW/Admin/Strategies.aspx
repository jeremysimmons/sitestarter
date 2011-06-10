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
	new StrategiesResetter().Reset();
	
	new StrategyInitializer().Initialize();
	
	Result.Display(Resources.Language.StrategiesReset);
}

private string GetShortTypeName(StrategyInfo strategy)
{
	Type type = Type.GetType(strategy.StrategyType);
	if (type != null)
		return type.Name;
	else
		return String.Empty;
}

private string GetNamespace(StrategyInfo strategy)
{
	Type type = Type.GetType(strategy.StrategyType);
	if (type != null)
		return type.Namespace;
	else
		return String.Empty;
}

private string GetAssemblyName(StrategyInfo strategy)
{
	Type type = Type.GetType(strategy.StrategyType);
	if (type != null)
		return type.Assembly.GetName().Name + ".dll";
	else
		return String.Empty;
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>Strategies</h1>
<cc:Result runat="Server"/>
<p>The following strategies are currently cached in the system. Click "Reset" to rescan and refresh the strategies.</p>
<p><a href="Cache.aspx" id="CacheIndexLink">&laquo; Index</a></p>
<p><asp:button runat="server" id="ResetButton" text='<%# Resources.Language.Reset %>' onclick="ResetButton_Click"/></p>
<table class="Panel" width="100%">
	<tr class="Heading2">
		<th>
			Strategy
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
	<% foreach (StrategyInfo strategy in StrategyState.Strategies){ %>
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