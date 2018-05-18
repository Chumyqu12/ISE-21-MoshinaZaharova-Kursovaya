<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormReg.aspx.cs" Inherits="VetClientView.FormReg" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Фио<asp:TextBox ID="TextBoxFIO" runat="server"></asp:TextBox>
            <br />
            Логин<asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox>
            <br />
            Пароль<asp:TextBox ID="TextBoxPassword" runat="server"></asp:TextBox>
            <br />
            Подтверждение пароля<asp:TextBox ID="TextBoxPassword1" runat="server"></asp:TextBox>
        </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Сохранить" />
    </form>
</body>
</html>
