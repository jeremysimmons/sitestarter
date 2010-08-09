<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Login" EnableEventValidation="false"  %>
<%@ Register Src="../ServerSelectPanel.ascx" TagName="ServerSelectPanel" TagPrefix="uc1" %>
<%@ Register Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
    <cc:Result runat="server"/>
    <asp:Login ID="Login1" runat="server" DestinationPageUrl="Default.aspx">
        <TitleTextStyle CssClass="Heading2" HorizontalAlign="Left" />
    </asp:Login>
</asp:Content>
