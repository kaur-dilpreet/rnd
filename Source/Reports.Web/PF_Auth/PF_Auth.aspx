﻿<%@ page language="C#" autoeventwireup="true" debug="true" inherits="OKTA_Auth_OKTA_Auth, OktaAuthAppWeb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <title>Agentless OKTA</title>
    <style type="text/css">
        .auto-style3 {
            text-align: center;
        }
        .auto-style4 {
            width: auto;
            height: auto;
            border: 2px solid #000000;
            background-color: #C0C0C0;
        }
        .auto-style5 {
            text-align: center;
            font-size: xx-large;
        }
        .auto-style6 {
            font-size: xx-large;
            text-align: center;
        }
        .auto-style7 {
            text-align: center;
            font-size: large;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style3">

            <table align="center" border="1" class="auto-style4" dir="auto" cellpadding="5" cellspacing="1">
                <tr>
                    <td colspan="2" class="auto-style3">
                        <span class="auto-style6"><strong>OneHP OKTA</strong></span><strong><br class="auto-style6" />
                        </strong><span class="auto-style6"><strong>.Net Agentless Integration</strong></span></td>
                </tr>
                <tr>
                    <td colspan="2" class="auto-style5">
                        Current configuration</td>
                </tr>
                <tr>
                    <td class="auto-style7"><strong>Variables</strong></td>
                    <td class="auto-style7"><strong>Values</strong></td>
                </tr>
                <tr>
                    <td class="auto-style3">Version</td>
                    <td class="auto-style3">
                        <asp:Label ID="Label_Ver" runat="server" Text="Version"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Company</td>
                    <td class="auto-style3">
                        <asp:Label ID="Label_Company" runat="server" Text="Company"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Framework</td>
                    <td class="auto-style3">
                        <asp:Label ID="Label_Frame" runat="server" Text="Framework"></asp:Label>
                    </td>
                </tr>
                </table>
            <br />
        </div>
    </form>
</body>
</html>
