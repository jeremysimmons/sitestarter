<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="ICSharpCode.SharpZipLib.Zip" %>
<script runat="server">
	protected string ExportDirectoryName = "Exports";
	protected int TotalRecords = 0;
	protected int TotalRecordsExported = 0;
	protected int TotalRecordsZipped = 0;
	protected string ZipPath = String.Empty;
	protected string ZipWebPath = String.Empty;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Request.QueryString["a"] == null ||
			Request.QueryString["a"] == String.Empty)
		{
			Result.DisplayError("No help file specified in the querystring.");
		
			Response.Redirect("../Default.aspx");
		}
		
		PageViews.SetActiveView(HelpFileView);
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" id="HelpFileView">
		<iframe src='<%= "HelpRaw.aspx?a=" + Request.QueryString["a"] %>' style="width: 100%; height:100%; border: 0px;" scrolling="no" marginwidth="0" marginheight="0" frameborder="0" vspace="0" hspace="0"/>
</asp:View>
</asp:MultiView>
</asp:Content>

