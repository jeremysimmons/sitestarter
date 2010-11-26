<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Test Contact Form" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
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
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	
}

</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<cc:ContactForm runat="server" Subject='Re: Something' ToName="Joe Bloggs" ToEmail="john@softwaremonkeys.net" FromName="System Admin" FromEmail="testadmin@softwaremonkeys.net" HeaderText="Contact Form" HeaderCssClass="Heading2" width="100%" SuccessMessage="Success"/>
</asp:Content>
