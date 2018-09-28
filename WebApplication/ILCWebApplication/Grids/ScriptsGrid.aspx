<%@ Import Namespace="ILCWebApplication" %>

<%@ Page Language="C#" MasterPageFile="~/Grids/GridPage.Master" AutoEventWireup="true" 
    CodeBehind="ScriptsGrid.aspx.cs" Inherits="ILCWebApplication.Grids.ScriptsGrid" 
    Title="Untitled Page" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderTitleCell" runat="server">
Scripts
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderButtonCell" runat="server">
    <asp:Button ID="addButton" runat="server" OnClientClick='<%$ Code:  "showPopup(\"../EditForms/EditFormScript.aspx\",\"\", 640, 420); return false;" %>'
            Text="Add" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="ILC_SCENARIO_ID" CssClass="tblStd" EnableViewState="False">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/images/icon_edit.gif" Text="Edit"
                        NavigateUrl='<%# "javascript:showPopup(\"../EditForms/EditFormScript.aspx?Id=" + Eval("ILC_SCENARIO_ID") + "\", \"EditFormScript\", 640, 420)" %>'> </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NAME" HeaderText="Name" />
            <asp:BoundField DataField="SCRIPTING_EXPRESSION" HeaderText="Scripting expression" />
        </Columns>
        <SelectedRowStyle BackColor="#C0FFFF" />
        <RowStyle CssClass="dark" />
        <AlternatingRowStyle CssClass="light" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand='<%$ Code: "SELECT ILC_SCENARIO_ID, NAME, SCRIPTING_EXPRESSION FROM " + WebSettings.GetSchemaName() + ".ILC_SCENARIO" %>'>
    </asp:SqlDataSource>
</asp:Content>
