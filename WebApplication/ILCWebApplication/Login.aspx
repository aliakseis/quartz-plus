<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ILCWebApplication.Login" %>

<%@ Outputcache Location="None"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta http-equiv="Cache-Control" content="no-cache" />
<title>ILC Web Application - Log In</title>
<link rel="stylesheet" href="~/css/base.css" type="text/css" runat="server" />
<style type="text/css" media="screen">
/* <![CDATA[ */
    @import url(css/base.css);
	@import url(css/tables.css);
	@import url(css/widgets.css);
	@import url(css/tabs.css);
	@import url(css/menus.css);
	@import url(css/forms.css);
	
	label {  
        display: inline;
    }
	
/* ]]> */
</style>
</head>
<body>
    <form id="form1" runat="server">
    
    
    
<table width="100%" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td align="left">
		    <div class="clearthis">
	            <span>&copy;2009 PAREXEL International<sup>&reg;</sup>. All rights reserved. A PAREXEL<sup>&reg;</sup> Company.</span>
            </div>
        </td>
	</tr>
	<tr>
		<td height="100" colspan="2">&#160;</td>
	</tr>
	<tr>
		<td align="center" valign="middle" colspan="2">
        <div id="loginTop"></div>
        <div align="center" id="loginBody">
			<table border="0" cellpadding="2" cellspacing="0" width="95%" 
			        style="right bottom no-repeat">
				<tr>
				    <td class="logoCell">
				        <h1>IVRS Line Checker</h1> <h1 class="shifted">Administrator</h1>
				    </td>
					<td align="center" >

                        <%-- to use chain of Authenticate Providers 
                        add OnAuthenticate="OnAuthenticate" attribute to the asp:Login tag below --%>
                        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False">
                        </asp:Login>
	                    <% 
	                    if (User.Identity.IsAuthenticated)
                        { 
                        %>
                        <div align="center" style="color:red">You are not authorized.</div>
	                    <% 
                        } 
                        %>

					</td>
					<td class="logoCell">
				        
				    </td>
				</tr>
			</table>
        </div>
        <div id="loginBottom"></div>
		</td>
	</tr>
</table>
    
    
    
    
    </form>
</body>
</html>
