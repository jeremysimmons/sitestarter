<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" assembly="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Register Src="Controls/Guide.ascx" TagName="Guide" TagPrefix="uc1" %>
    <asp:Content runat="server" ContentPlaceHolderID="Body">
    	<cc:result runat="server"/>
        <uc1:Guide ID="Guide1" runat="server" /></asp:Content>