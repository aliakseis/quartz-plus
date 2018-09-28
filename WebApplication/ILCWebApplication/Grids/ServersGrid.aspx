<%@ Import Namespace="ILCWebApplication" %>

<%@ Page Language="C#" MasterPageFile="./GridPage.Master" AutoEventWireup="true"
    Codebehind="ServersGrid.aspx.cs" Inherits="ILCWebApplication.ServersGrid1" Title="Untitled Page" %>

<%@ OutputCache Location="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderTitleCell" runat="server">
    Servers:
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderButtonCell" runat="server">
    <asp:Button ID="addButton" runat="server" OnClientClick='<%$ Code:  "showPopup(\"../EditForms/EditFormServer.aspx\",\"\", 640, 420); return false;" %>'
        Text="Add" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1"
        CssClass="tblStd" EnableViewState="False">
        <SelectedRowStyle BackColor="#C0FFFF" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/images/icon_edit.gif" Text="Edit"
                        NavigateUrl='<%# "javascript:showPopup(\"../EditForms/EditFormServer.aspx?Id=" + Eval("IVR_SERVER_ID") + "\", \"EditFormServer\", 640, 420)" %>'> </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NAME" HeaderText="Name" />
            <asp:BoundField DataField="NUM_CHANNELS" HeaderText="Channels" />
            <asp:CheckBoxField DataField="ENBL" HeaderText="Enabled" />
            <asp:CheckBoxField DataField="AUTH_CHECKER" HeaderText="Aspirin" />
            <asp:BoundField DataField="SCHEDULE_CRON" HeaderText="Cron" />
        </Columns>
        <RowStyle CssClass="dark" />
        <AlternatingRowStyle CssClass="light" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand='<%$ Code: "SELECT IVR_SERVER_ID, NAME, NUM_CHANNELS," + ConstExpressions.GetCheckExpression("ENABLED", "0") + " AS ENBL," + ConstExpressions.GetCheckExpression("AUTH_CHECKER", "null") + " AS AUTH_CHECKER, SCHEDULE_CRON FROM " + WebSettings.GetSchemaName()+ ".IVR_SERVER" %>' />
</asp:Content>
