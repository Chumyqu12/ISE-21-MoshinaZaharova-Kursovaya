<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormVisit.aspx.cs" Inherits="VetClientView.FormVisit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        Название<asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
        <br />
        Цена<asp:TextBox ID="TextBoxPrice" runat="server"></asp:TextBox>
        <br />
        Услуги<asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Добавить" />
        <asp:Button ID="Button2" runat="server" Text="Изменить" OnClick="Button2_Click" />
        <asp:Button ID="Button3" runat="server" Text="Удалить" OnClick="Button3_Click" />
        <asp:Button ID="Button4" runat="server" Text="Обновить" />
        <br />
        <br />
        <br />
        <asp:Button ID="Button5" runat="server" Text="Сохранить" />
        <asp:Button ID="Button6" runat="server" Text="Отмена" />
    </form>
</body>
</html>
