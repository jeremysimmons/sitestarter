<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
    	EnsureAuthorised();
    }
    
    private void EnsureAuthorised()
    {
    	//bool isAuthorised = false;
    
    	//if (ConfigurationSettings.AppSettings["SecureData"] == null
    	//	|| ConfigurationSettings.AppSettings["SecureData"].ToLower() != "false")
    	//{
    		Authorisation.EnsureIsInRole("Administrator");
    	//}
    }
    
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
    <h1>Errors</h1>
    <p>The following errors were found in the log:</p>
    <% foreach (string date in LogUtilities.GetLogDates()){ %>
    	<h2><%= date %></h2>
    	<% foreach (XmlElement error in LogUtilities.GetErrors(date)){ %>
    		<h3><%= LogUtilities.GetComponent(error) %>.<%= LogUtilities.GetMethod(error) %></h3>
			<p><%= LogUtilities.GetTime(error) %></p>
    		<p><%= LogUtilities.GetMessage(error).Replace("\n", "<br/>") %></p>
    		<hr/>
    	<% } %>
    <% } %>
</asp:Content>