<%@ Import Namespace="ILCWebApplication" %>

<%@ Page Language="C#" MasterPageFile="./GridPage.Master" AutoEventWireup="true"
    Codebehind="ProjectsGrid.aspx.cs" Inherits="ILCWebApplication.ProjectsGrid1"
    Title="Untitled Page" %>

<%@ OutputCache Location="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderTitleCell" runat="server">
    <%= Request.Params["name"] + " Projects:" %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderButtonCell" runat="server">
    <asp:Button ID="addButton" runat="server" OnClientClick='<%$ Code:  "showPopup(\"../EditForms/EditFormProject.aspx?parentId=" + Request.Params["server"] + "\",\"\", 640, 420); return false;" %>'
        Text="Add" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="IVR_PROJECT_ID" CssClass="tblStd" EnableViewState="False">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/images/icon_edit.gif" Text="Edit"
                        NavigateUrl='<%# "javascript:showPopup(\"../EditForms/EditFormProject.aspx?Id=" + Eval("IVR_PROJECT_ID") + "&parentId=" + Request.Params["server"] + "\", \"EditFormProject\", 640, 420)" %>'> </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NAME" HeaderText="Name" />
            <asp:CheckBoxField DataField="ENBL" HeaderText="Enabled" />
            <asp:BoundField DataField="SCHEDULE_CRON" HeaderText="Cron" />
        </Columns>
        <SelectedRowStyle BackColor="#C0FFFF" />
        <RowStyle CssClass="dark" />
        <AlternatingRowStyle CssClass="light" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand='<%$ Code: "SELECT IVR_PROJECT_ID, NAME," + ConstExpressions.GetCheckExpression("ENABLED", "0") + " AS ENBL,SCHEDULE_CRON FROM " + WebSettings.GetSchemaName() + ".IVR_PROJECT WHERE (IVR_SERVER_ID = :PARAM1)" %>'>
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="" Name="PARAM1" QueryStringField="server" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
