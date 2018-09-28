<%@ Page Language="C#" MasterPageFile="./EditForm.Master" AutoEventWireup="true" CodeBehind="EditFormItem.aspx.cs" Inherits="ILCWebApplication.EditForms.EditFormItem1" Title="Item Details" %>
<%@ OutputCache Location="None" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <fieldset>
		<legend>
			Item Settings
		</legend>
    <table cellspacing="5">
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label2" runat="server" Text="Phone number:" Width="93px"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbPhone" runat="server" MaxLength="50"></asp:TextBox>&nbsp;
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbPhone"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="tbPhone"
                    ErrorMessage="Enter valid phone number: 0-9 are allowed" ValidationExpression="^(?!911)[\d]+"></asp:RegularExpressionValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label4" runat="server" Text="Login:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbLogin" runat="server" Width="96px" MaxLength="29"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tbLogin"
                    ErrorMessage="Enter valid login: ‘0’ to ‘9’, ‘A’ to ‘D’,‘*’, and ‘#’ are allowed" ValidationExpression="[0-9A-D#*]*"></asp:RegularExpressionValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label5" runat="server" Text="Password:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbPassword" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="tbPassword"
                    ErrorMessage="Enter a valid password: everything is allowed" ValidationExpression=".*"></asp:RegularExpressionValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label6" runat="server" Text="Enabled:"></asp:Label></td>
            <td>
                <asp:CheckBox ID="cbEnabled" runat="server" /></td>
        </tr>
    </table>
    </fieldset>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderButtons" runat="server">
    <div style="text-align:center">
    <asp:Button ID="Button2" runat="server" OnClick="Button1_Click" Text="Save" style="width:60px" />
    <input id="Button4" type="button" value="Cancel" onclick='self.close();' style="width:60px" />
    </div>
</asp:Content>