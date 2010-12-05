<%@ Control Language="C#" ClassName="GettingStarted" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">

</script>
<div class="Heading1">Welcome...</div>
<p>Some of the important pages in this starter kit are listed below.</p>
<p>
</p>
<ul>
    <li><strong><a href='<%= Request.ApplicationPath + "/User/SignIn.aspx" %>'>Login</a></strong> (/User/SignIn.aspx) - The login page allows registered users to
        log in to the system and interact with their account and data.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/User/Index.aspx" %>'>Manage Users</a></strong> (/User/Index.aspx) - The manage users page (in the
        administration section) is used by authorised administrators to browse, create,
        edit, and delete user accounts.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Log.aspx" %>'>Log</a></strong> (/Admin/Log.aspx) - The log includes application exceptions and other important information.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Data.aspx" %>'>Data</a></strong> (/Admin/Data.aspx) - A simple and direct data browser.</li>
</ul>