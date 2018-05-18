<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormCreateZakaz.aspx.cs" Inherits="VetClientView.FormCreateZakaz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body style="height: 470px">
    <form id="form1" runat="server">
        <div>
            Клиент&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="DropDownListClient" runat="server" AutoPostBack="True" Height="16px" Width="285px">
            </asp:DropDownList>
            &nbsp;&nbsp;Посещение<asp:DropDownList ID="DropDownListVisit" runat="server" AutoPostBack="True" Height="16px" Width="285px">
            </asp:DropDownList>
        <asp:Button ID="ButtonVisit" runat="server" OnClick="ButtonVisit_Click" Text="Создать посещение" />
            <br />
        </div>
        Сумма<asp:TextBox ID="TextBoxPrice" runat="server" Enabled="False"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonSave" runat="server" OnClick="ButtonSave_Click" Text="Сохранить" />
        <asp:Button ID="ButtonCancel" runat="server" OnClick="ButtonCancel_Click" Text="Отмена" />
    </form>
</body>
</html>
