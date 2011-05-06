<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Title="Error" ValidateRequest="false" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<script runat="server">
    protected Exception CurrentException = null;
    
    private void Page_Load(object sender, EventArgs e)
    {
        CurrentException = Server.GetLastError();
        Exception exception = CurrentException;

        PageViews.SetActiveView(ErrorView);

        PageViews.DataBind();
    }    

</script>


<asp:Content ID="PageBody" ContentPlaceHolderID="Body" Runat="Server">
<script language="javascript">
function ReportIssue(project)
{
	var path = '';
	if (project == 'SiteStarter')
	{
		path = '<%= ConfigurationSettings.AppSettings["ReportIssueUrl"].Replace("${Project.ID}", ConfigurationSettings.AppSettings["UniversalProjectID"]) %>';
	}
	
	if (path != '')
	{
		var newWindow = window.open(path,
		'ReportError_<%= Guid.NewGuid().ToString().Replace("-","") %>',
		'menubar=no,height=800,width=800,resizable=yes,toolbar=no,location=yes,scrollbars=yes');
	}
}

function GetIssueSubject()
{
	return "Application Exception";
}

function GetIssueDescription()
{
	return document.getElementById('ErrorDetails').value;
}
</script>
<asp:MultiView runat="server" id="PageViews">
<asp:View runat="server" id="ErrorView">
<div class="Heading1"><%# Resources.Language.ErrorPageTitle %></div>

<p>
<textarea id="ErrorDetails" style="width: 100%; height: 300px; font-size: 11px;">
<%# CurrentException != null ? CurrentException.ToString() : Resources.Language.NoErrorOccurred %>
</textarea>
</p>
<p>
<%= Resources.Language.SiteStarter %>: <a href="javascript:;" onclick="ReportIssue('SiteStarter');"><%= Resources.Language.ReportIssue %> &raquo;</a>
</p>
</asp:View>
</asp:MultiView>
</asp:Content>

