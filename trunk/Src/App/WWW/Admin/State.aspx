<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="Db4objects.Db4o" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
    }
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>State</h1>
<h2>Application</h2>
<% foreach (string key in StateAccess.State.GetKeys(StateScope.Application)){ %>
<p><%= key %>: <%= StateAccess.State.Application[key] != null ? StateAccess.State.Application[key].ToString() : String.Empty %></p>
<% } %>
<h2>Session</h2>
<% foreach (string key in StateAccess.State.GetKeys(StateScope.Session)){ %>
<p><%= key %>: <%= StateAccess.State.Session[key] != null ? StateAccess.State.Session[key].ToString() : String.Empty %></p>
<% } %>
<h2>Operation</h2>
<% foreach (string key in StateAccess.State.GetKeys(StateScope.Operation)){ %>
<p><%= key %>: <%= StateAccess.State.Operation[key] != null ? StateAccess.State.Operation[key].ToString() : String.Empty %></p>
<% } %>
<h2>User</h2>
<% foreach (string key in StateAccess.State.GetKeys(StateScope.User)){ %>
<p><%= key %>: <%= StateAccess.State.User[key] != null ? StateAccess.State.User[key].ToString() : String.Empty %></p>
<% } %>
</asp:Content>