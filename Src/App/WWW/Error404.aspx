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
		Response.TrySkipIisCustomErrors = true;
		Response.StatusCode = 404;
		Response.Status = "404 Not Found";
		
		try
		{
			CurrentException = Server.GetLastError();
			Exception exception = CurrentException;

			PageViews.SetActiveView(ErrorView);

			PageViews.DataBind();
		}
		catch (Exception ex)
		{
			LogWriter.Error(new ExceptionHandler().GetMessage(ex));
			
			throw ex;
		}
		
		HttpContext.Current.ApplicationInstance.CompleteRequest();
    }    
    
    private string GetIssueSubject()
    {
    	return Resources.Language.PageNotFound + ": " + Request.Url.ToString();
    }
    
    private string GetIssueDescription()
    {
    	return new ExceptionHandler().GetMessage(CurrentException);
    }

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
	    <title><%= Resources.Language.PageNotFound %></title>
	    <%= StyleUtilities.GetStyleSheet("Styles.css") %>
	</head>
	<body>
	    <form id="form1" runat="server">
			<asp:MultiView runat="server" id="PageViews">
				<asp:View runat="server" id="ErrorView">
				<h1><%# Resources.Language.PageNotFound %></h1>
				<p>
				<%# Resources.Language.SorryPageNotFound %>
				</p>
				<p>
				<cc:ReportIssueLink runat="server" IssueSubject='<%# GetIssueSubject() %>' IssueDescription='<%# GetIssueDescription() %>' />
				</p>
				<p>
					&laquo; <a href='<%= Request.ApplicationPath %>'><%= Resources.Language.BackToHome %></a>
				</p>
				</asp:View>
			</asp:MultiView>
			<p>
				SiteStarter Version: <%= DataAccess.Data.Schema.ApplicationVersion %>
			</p>
		</form>
	</body>
</form>