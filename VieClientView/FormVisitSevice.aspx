<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormVisitSevice.aspx.cs" Inherits="VetClientView.FormVisitSevice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Услуга<asp:DropDownList ID="DropDownListVisit" runat="server">
            </asp:DropDownList>
            <br />
            Количество<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Сохранить" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Отмена" />
    </form>
</body>
</html>
