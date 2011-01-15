<%@ Control Language="C#" ClassName="AdminFooter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<div id="AdminFooter">
        <% if (AuthenticationState.UserIsInRole("Administrator")) { %>
        <p>
        	<a href='<%= UrlCreator.Current.CreateUrl("Index", "Settings") %>' id="SettingsLink"><%= Resources.Language.Settings %></a>
        </p>
        <% } %>
</div>