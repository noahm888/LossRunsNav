<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="LossRunsNavServer.Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 26px;
            width: 58px;
        }
        .auto-style2 {
            width: 200px;
        }
        .auto-style3 {
            height: 26px;
            width: 200px;
        }
        .auto-style4 {
            width: 223px;
        }
        .auto-style5 {
            height: 26px;
            width: 223px;
        }
        .auto-style6 {
            width: 58px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <h2>Admin</h2>
        <br />
        <br />
    <table style="width:100%;">
        <tr>
            <td class="auto-style6">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style4">FileName:</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style1"></td>
            <td class="auto-style3">
                &nbsp;</td>
            <td class="auto-style5">
                <asp:TextBox ID="tbFileName" runat="server" Height="25px" Width="168px" OnTextChanged="tbFileName_TextChanged">Batch</asp:TextBox>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style4">Batch Size</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6"></td>
            <td class="auto-style2">
                <div style="float: right" ><asp:Label ID="lblBatchSize" runat="server" Text=""></asp:Label></div></td>
            <td class="auto-style4">
                <asp:TextBox ID="tbBatchSize" runat="server" Height="25px" Width="93px" OnTextChanged="tbBatchSize_TextChanged">1000</asp:TextBox>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style4">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6">
                
            </td>
            <td class="auto-style2">
                <div style="float: right" ><asp:Label ID="lblFileUpload" runat="server" Text=""></asp:Label></div></td>
            <td class="auto-style4">
                <asp:FileUpload ID="FileUpload1" runat="server" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6"></td>
            <td class="auto-style2">
                <div style="float: right" ><asp:Label ID="lblProcess" runat="server" Text=""></asp:Label></div></td>
            <td class="auto-style4">
                <asp:Button ID="btnProcess" runat="server" OnClick="btnProcess_Click" Text="Process" />
                <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" Text="Reset" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6"></td>
            <td class="auto-style2">
                
                &nbsp;</td>
            <td class="auto-style4">
                
                <asp:Label ID="lblReset" runat="server"></asp:Label>
                
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style6"></td>
            <td class="auto-style2">
                
                &nbsp;</td>
            <td class="auto-style4">
                
                <asp:Label ID="lblReset2" runat="server"></asp:Label>
                
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>
    </form>
    </body>
</html>

