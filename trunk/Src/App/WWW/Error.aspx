<%@ Page Language="C#" AutoEventWireup="true" Title="Error" ValidateRequest="false" Inherits="SoftwareMonkeys.SiteStarter.Web.BasePage" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
    protected Exception CurrentException = null;
    
    private void Page_Load(object sender, EventArgs e)
    {
		try
		{
			CurrentException = Server.GetLastError();
			Exception exception = CurrentException;

			PageViews.SetActiveView(ErrorView);

			PageViews.DataBind();
		}
		catch (Exception ex)
		{
			LogWriter.Error("An error occurred on the error display page.");
			
			LogWriter.Error(ex);
			
			throw ex;
		}
    }    

</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
	    <title><%# Resources.Language.ErrorPageTitle %></title>
	    <link href='<%= Request.ApplicationPath + "/Styles/Simple.css" %>' rel="stylesheet" type="text/css" />
	</head>
	<body>
	    <form id="form1" runat="server">
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
				<h1><%# Resources.Language.ErrorPageTitle %></h1>
				
				<p>
				<textarea id="ErrorDetails" style="width: 100%; height: 300px; font-size: 11px;"><%# CurrentException != null ? CurrentException.ToString() : Resources.Language.NoErrorOccurred %>
				</textarea>
				</p>
				<p>
				<%= Resources.Language.SiteStarter %>: <a href="javascript:;" onclick="ReportIssue('SiteStarter');"><%= Resources.Language.ReportIssue %> &raquo;</a>
				</p>
				<p>
					<a href='<%= Request.ApplicationPath %>'><%= Resources.Language.BackToHome %> &raquo;</a>
				</p>
				</asp:View>
			</asp:MultiView>
		</form>
	</body>
</form>