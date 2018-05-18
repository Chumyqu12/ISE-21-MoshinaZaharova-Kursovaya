<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormVisits.aspx.cs" Inherits="VetClientView.FormVisits" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:GridView ID="DataGridView" runat="server">
        </asp:GridView>
        <p>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Добавить" />
        <asp:Button ID="Button2" runat="server" Text="Изменить" OnClick="Button2_Click" />
        <asp:Button ID="Button3" runat="server" Text="Удалить" OnClick="Button3_Click" />
        <asp:Button ID="Button4" runat="server" Text="Обновить" OnClick="Button4_Click" style="height: 29px" />
        <br />
        <br />
        </p>
    </form>
</body>
</html>
