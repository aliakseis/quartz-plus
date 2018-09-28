<%@ Import Namespace="ILCWebApplication"%>
<%@ Page Language="C#" MasterPageFile="./EditForm.Master" AutoEventWireup="true" CodeBehind="EditFormProject.aspx.cs" Inherits="ILCWebApplication.EditForms.EditFormProject1" Title="Project Details" %>
<%@ OutputCache Location="None" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">    
    <fieldset>
		<legend>
			Project Settings
		</legend>
    <table cellspacing="5">
        <tr><td class="EditLabel"><asp:Label ID="Label1" runat="server" Text="Server name:"></asp:Label></td><td>
            <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource1"
                Width="130px" DataTextField="NAME" DataValueField="IVR_SERVER_ID" OnDataBound="OnServersListDataBound">
            </asp:DropDownList><asp:SqlDataSource ID="SqlDataSource1" runat="server"
        ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>"
        SelectCommand='<%$ Code: "SELECT IVR_SERVER_ID, NAME FROM " + WebSettings.GetSchemaName() + ".IVR_SERVER" %>'
       />
        </td></tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label3" runat="server" Text="Name:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbName" runat="server" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbName"
                    ErrorMessage="*"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label4" runat="server" Text="Emails:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbEmails" runat="server" TextMode="MultiLine" MaxLength="2000" Width="250px"></asp:TextBox>&nbsp;
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tbEmails"
                    ErrorMessage="Invalid entry" ValidationExpression='<%$ Code: ConstExpressions.MULTI_EMAIL_REGEXP %>'></asp:RegularExpressionValidator>
                &nbsp;&nbsp;<asp:CustomValidator ID="CustomValidator4" runat="server" ControlToValidate="tbEmails"
                    ErrorMessage="Invalid entry" OnServerValidate="OnValidateEmails" Display="Dynamic"></asp:CustomValidator>&nbsp;
            </td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label5" runat="server" Text="Enabled:"></asp:Label></td>
            <td>
                <asp:CheckBox ID="cbEnabled" runat="server" /></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label6" runat="server" Text="Cron:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbCron" runat="server" MaxLength="50"></asp:TextBox>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="tbCron" ErrorMessage="Incorrect Cron Expression"
                    OnServerValidate="OnValidateCron"></asp:CustomValidator>
            </td>
        </tr>
    </table>
    </fieldset>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderButtons" runat="server">
    <div style="text-align:center">
    <asp:Button ID="Button2" runat="server" OnClick="Button1_Click" Text="Save" style="width:60px"/>
    <input id="Button4" type="button" value="Cancel" onclick='self.close();' style="width:60px"/>
    </div>
</asp:Content>
