<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormDoc.aspx.cs" Inherits="VetClientView.FormDoc" %>

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
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="док файл отправить" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Ексель файл отправить" />
        <p>
            Мейл<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        </p>
    </form>
</body>
</html>
