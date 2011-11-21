<%@ Page Language="C#" AutoEventWireup="true" Title="Error" ValidateRequest="false" Inherits="SoftwareMonkeys.SiteStarter.Web.BasePage" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
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
    
    private string GetIssueSubject()
    {
    	return CurrentException.GetType().Name + ": " + Utilities.Summarize(CurrentException.Message, 100);
    }
    
    private string GetIssueDescription()
    {
    	return CurrentException.ToString();
    }

</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
	    <title><%# Resources.Language.ErrorPageTitle %></title>
	    <%= StyleUtilities.GetStyleSheet("Styles.css") %>
	</head>
	<body>
	    <form id="form1" runat="server">
			<asp:MultiView runat="server" id="PageViews">
				<asp:View runat="server" id="ErrorView">
				<h1><%# Resources.Language.ErrorPageTitle %></h1>
				
				<p>
				<textarea id="ErrorDetails" style="width: 100%; height: 300px; font-size: 11px;"><%# CurrentException != null ? CurrentException.ToString() : Resources.Language.NoErrorOccurred %>
				</textarea>
				</p>
				<p>
				<cc:ReportIssueLink runat="server" IssueSubject='<%# GetIssueSubject() %>' IssueDescription='<%# GetIssueDescription() %>' />
				</p>
				<p>
					<a href='<%= Request.ApplicationPath %>'><%= Resources.Language.BackToHome %> &raquo;</a>
				</p>
				</asp:View>
			</asp:MultiView>
		</form>
	</body>
</form>