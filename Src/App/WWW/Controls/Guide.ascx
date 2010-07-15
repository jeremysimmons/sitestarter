<%@ Control Language="C#" ClassName="GettingStarted" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">

</script>
<div class="Heading1">Welcome...</div>
<p>Some of the important pages in this framework are described below.</p>
<p>
</p>
<ul>
<li>
<b><a href='<%= Request.ApplicationPath + "/Setup.aspx" %>'>Setup</a></b> (/Setup.aspx) - The setup script creates a configuration file with default settings along with a test user account (default set to username:
    "test" password: "pass") allowing the application to function. This file is used
    during development and testing to reset the data (usually after deleting the contents
    of the /Data/ directory) and is deleted once the application is deployed.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Members/Login.aspx" %>'>Login</a></strong> (/Members/Login.aspx) - The login page allows registered users to
        log in to the system and interact with their account and data.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/Admin/Users.aspx" %>'>Manage Users</a></strong> (/Admin/Users.aspx) - The manage users page (in the
        administration section) is used by authorised administrators to browse, create,
        edit, and delete user accounts.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/UnitTests.aspx" %>'>Unit Tests</a></strong> (/Testing.aspx) - The testing page triggers the execution
        of all unit tests found in assemblies within the /bin/ directory and displays the
        results of each. Each project and function within the solution should have a corresponding
        test project and test function to streamline the testing process and to aid in quickly
        tracking down issues.</li>
    <li><strong><a href='<%= Request.ApplicationPath + "/DBContents.aspx" %>'>DB Contents</a></strong> (/DBContents.aspx) - The DB contents page displays
        a simple output of all object/data types in the application's database. This file
        is often used throughout development to ensure that code has the desired result.</li>
</ul>