<%@ Import Namespace="ILCWebApplication" %>
<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="ILCWebApplication.Reports" EnableEventValidation="false" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Reports
</asp:Content>
<asp:Content ID="ContentBody" ContentPlaceHolderID="PlaceHolderBody" runat="server">

<script type="text/javascript" src="<%= ResolveClientUrl("~/scripts/CalendarExt.js") %>" >
</script>
<script type="text/javascript" src="<%= ResolveClientUrl("~/scripts/Clock.js") %>" >
</script>

<div style="height:435px">
    <table style="width: 100%" cellspacing="0" cellpadding="0">
        <tr>
            <td class="VBottomDivider">    
            <table style="width: 100%" cellspacing="0" cellpadding="1">
            <tr>
            <td colspan="5">
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="startDate"
                    Display="Dynamic" ErrorMessage="Invalid start date" ValidationExpression="((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="startTime"
                    Display="Dynamic" ErrorMessage="Invalid start time" ValidationExpression="^ *(1[0-2]|[1-9]|0[1-9]):[0-5][0-9] *(a|p|A|P)(m|M) *$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="endDate"
                    Display="Dynamic" ErrorMessage="Invalid end date" ValidationExpression="((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="endTime"
                    ErrorMessage="Invalid end time" ValidationExpression="^ *(1[0-2]|[1-9]|0[1-9]):[0-5][0-9] *(a|p|A|P)(m|M) *$" Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="startDate"
                    Display="Dynamic" ErrorMessage="Enter start date"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="endDate"
                    Display="Dynamic" ErrorMessage="Enter end date"></asp:RequiredFieldValidator></td>
            <td colspan="8" style="border: #ccc 1px solid">FOR THE PERIOD:</td>
            </tr>
            <tr>
            <td>PROJECT NAME</td>
            <td>SERVER</td>
            <td>PHONE NUMBER</td>
            <td>USER ID</td>
            <td>STATUS</td>
            <td colspan="4">START</td>
            <td colspan="4">END</td>
            <td><asp:Button ID="runButton" runat="server" Width="50px"
        Text="Run" OnClick="runButton_Click" /></td>
            </tr>
            <tr>
            <td style="height: 24px">
                <asp:TextBox ID="projectName" runat="server" Width="80px"></asp:TextBox></td>
            <td style="height: 24px">
                <asp:DropDownList ID="server" runat="server" DataSourceID="SqlDataSource1"  OnDataBound="OnServersListDataBound" Width="90px" DataTextField="NAME" DataValueField="IVR_SERVER_ID" >
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                        ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
                        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>"
                        SelectCommand='<%$ Code: "SELECT IVR_SERVER_ID, NAME FROM " + WebSettings.GetSchemaName() + ".IVR_SERVER" %>'
                       />
            </td>
            <td style="height: 24px">
                <asp:TextBox ID="phoneNumber" runat="server" Width="80px"></asp:TextBox></td>
            <td style="height: 24px">
                <asp:TextBox ID="userId" runat="server" Width="70px"></asp:TextBox></td>
            <td style="height: 24px">
                <asp:DropDownList ID="status" runat="server" Width="70px">
                    <asp:ListItem Selected="True" Text="ANY" Value="" />
                    <asp:ListItem>AWAITING</asp:ListItem>
                    <asp:ListItem>FAILED</asp:ListItem>
                    <asp:ListItem>SUCCEEDED</asp:ListItem>
                </asp:DropDownList></td>
            <td style="height: 24px"><asp:TextBox ID="startDate" runat="server" Width="60px"></asp:TextBox></td>
            <td style="height: 24px"><%= ConstExpressions.GetDateExpression(startDate.ClientID, ResolveClientUrl("~/images/"))%></td>
            <td style="height: 24px"><asp:TextBox ID="startTime" runat="server" Width="50px"></asp:TextBox></td>
            <td style="height: 24px"><%= ConstExpressions.GetTimeExpression(startTime.ClientID, ResolveClientUrl("~/images/"))%></td>
            <td style="height: 24px"><asp:TextBox ID="endDate" runat="server" Width="60px"></asp:TextBox></td>
            <td style="height: 24px"><%= ConstExpressions.GetDateExpression(endDate.ClientID, ResolveClientUrl("~/images/"))%></td>
            <td style="height: 24px"><asp:TextBox ID="endTime" runat="server" Width="50px"></asp:TextBox></td>
            <td style="height: 24px"><%= ConstExpressions.GetTimeExpression(endTime.ClientID, ResolveClientUrl("~/images/"))%></td>
            <td><input id="clearButton" type="button" value="Clear" onclick='window.location="Reports.aspx"' style="width:50px"/> 
             </td>
            </tr>
            </table>
            </td>
        </tr>
        <tr>
            <td>
                <CR:CrystalReportViewer ID="crystalReportViewer" runat="server"
                    EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False" Height="370px"
                    ReportSourceID="CrystalReportSource1" Width="810px" BestFitPage="False" DisplayGroupTree="False" HasCrystalLogo="False" HasDrillUpButton="False" HasRefreshButton="True" HasSearchButton="False" HasToggleGroupTreeButton="False" HasViewList="False" PageZoomFactor="85" OnError="crystalReportViewer_Error" />
                <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
                    <Report FileName="CrystalReport1.rpt">
                    </Report>
                </CR:CrystalReportSource>
            </td>
        </tr>
     </table>
</div>
</asp:Content>
