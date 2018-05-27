<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormPdf.aspx.cs" Inherits="VetClientView.FormPdf" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            С&nbsp;&nbsp;&nbsp;
            <asp:Calendar ID="Calendar1" runat="server"></asp:Calendar>
&nbsp;по<asp:Calendar ID="Calendar2" runat="server"></asp:Calendar>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server">
            </rsweb:ReportViewer>
        </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Назад" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Сформировать" />
        <asp:Button ID="Button3" runat="server" Text="В PDF" OnClick="Button3_Click" />
        <asp:Button ID="Button4" runat="server" Text="На почту" OnClick="Button4_Click" />
        Клиент<asp:DropDownList ID="DropDownListClient" runat="server" style="margin-bottom: 0px">
        </asp:DropDownList>
    </form>
</body>
</html>
