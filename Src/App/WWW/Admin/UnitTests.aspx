<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Unit Tests" StylesheetTheme="Default" %>

<%@ Register Src="~/Controls/Testing.ascx" TagName="Testing" TagPrefix="uc1" %>
<script runat="server">
void Page_Load(object sender, EventArgs e)
{
    if (!SoftwareMonkeys.SiteStarter.Configuration.Config.IsInitialized)
    {
    //    Response.Redirect("Setup.aspx");
    }
}

    protected void Testing1_Load(object sender, EventArgs e)
    {

    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<div class="Heading1">Unit Tests</div>
<p>The following unit tests (built with NUnit) have been loaded from the assemblies in the /bin directory and executed. To add or edit tests simply open the relevant 'Tests' project or create a new one to suit your needs.</p>
    <uc1:Testing ID="Testing1" runat="server" OnLoad="Testing1_Load" />
</asp:Content>

