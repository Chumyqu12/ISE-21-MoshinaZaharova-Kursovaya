<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormLogin.aspx.cs" Inherits="VetClientView.FormLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height: 523px">
            Логин<asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox>
            <br />
            Пароль<asp:TextBox ID="textBoxPassword" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click1" Text="Вход" />
        </div>
    </form>
</body>
</html>
