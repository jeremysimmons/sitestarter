<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.Elements" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	// Property values can be set programmatically but currently are set directly in ASPX code below
	// therefore the following two lines are disabled
	
	//DynamicControl.PropertyValues = new NameValueCollection();
	//DynamicControl.PropertyValues.Add("Message", "Excellent it worked.");
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<cc:ElementControl runat="Server" ElementName="HelloWorld" id="ElementControl" PropertyValuesString="Message=It worked"/>
</asp:Content>