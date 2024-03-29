﻿<%@ Control Language="C#" AutoEventWireup="true" inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register tagprefix="cc" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" assembly="SoftwareMonkeys.SiteStarter.Web" %>
<div class="Heading1">Welcome...</div>
<cc:Result runat="server" />
<p>The SiteStarter application contains all of the basic application functionality that drives projects such as the <a href="http://www.softwaremonkeys.net/workhub.aspx" target="_blank">WorkHub</a> application.</p>
<p>The SiteStarter application itself is limited on end user features but is packed with tools and core functionality which can be used to simply and easily create new features, and put together entire new applications.</p>
<p>The code or the libraries from the SiteStarter project can be imported into or referenced by other projects. Or the SiteStarter application can simply be a learning tool.</p>
<p>For assistance creating new features or whole new applications base on or with some of the functionality of SiteStarter please use the <a href="http://www.softwaremonkeys.net/discuss.aspx" target="_blank">discussion boards</a>.</p>
<p>Some of the important pages in this starter kit are listed below.</p>
<ul>
    <li><strong><a href='<%= Request.ApplicationPath + "/User-SignIn.aspx" %>'>Login</a></strong> (/User-SignIn.aspx) - The login page allows registered users to
        log in to the system and interact with their account and data.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/User-Index.aspx" %>'>Users</a></strong> (/User-Index.aspx) - The users page is used by authorised administrators to manage user accounts.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/UserRole-Index.aspx" %>'>Roles</a></strong> (/UserRole-Index.aspx) - The roles is used by authorised administrators to manage user/security roles.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Settings.aspx" %>'>Settings</a></strong> (/Settings.aspx) - The administration settings can be used to configure certain parts of the application.</li>
     <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Backup.aspx" %>'>Backup</a></strong> (/Admin/Backup.aspx) - Used to back up data from the stores to serialized XML files and then zip them into a single file.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Update.aspx" %>'>Update</a></strong> (/Admin/Update.aspx) - Prepares the application to be updated. Important as it ensures the new version can use the existing data.</li>
       <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Data.aspx" %>'>Data</a></strong> (/Admin/Data.aspx) - A simple and direct data browser. (Only accessible by administrators.)</li>
       <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Cache.aspx" %>'>Application Cache</a></strong> (/Admin/Cache.aspx) - Displays the cached info about many of the components in the application.</li>
</ul>