<%@ Control Language="C#" ClassName="CreateEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
	protected bool TestSuccessful = false;

  private void Page_Load(object sender, EventArgs e)
  {
	  	if (!IsPostBack)
	  	{
  			TestSuccessful = TestSmtpStrategy.New().RunTest();
	  	}
  }
</script>
                   <h1>
                                <%= Resources.Language.SmtpServerTest %>
                   </h1>
                   <p>Testing server: <%= TestSmtpStrategy.SmtpServer %></p>
                   <p>
                            <% if (TestSuccessful){ %>
                            Success
                            <% } else { %>
                            Failure
                            <% } %>
                            </p>