<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormMain.aspx.cs" Inherits="VetClientView.FormMain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        #form1 {
            height: 534px;
            width: 1174px;
        }
    </style>
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Menu ID="Menu" runat="server" BackColor="White" ForeColor="Black" Height="150px">
            <Items>
                <asp:MenuItem Text="Отчёты" Value="Отчёты">
                    <asp:MenuItem Text="В док" Value="док" NavigateUrl="~/FormLogin.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="В ексель" Value="ексель" NavigateUrl="~/FormXls.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="В пдф" Value="Склады" NavigateUrl="~/FormPdf.aspx" Selected="True"></asp:MenuItem>
                </asp:MenuItem>
            </Items>
        </asp:Menu>
        <asp:Button ID="ButtonCreateIndent" runat="server" Text="Создать заказ" OnClick="ButtonCreateIndent_Click" />
        <asp:Button ID="ButtonIndentReady" runat="server" Text="Заказ готов" OnClick="ButtonIndentReady_Click" />
        <asp:Button ID="ButtonIndentPayed" runat="server" Text="Заказ оплачен" OnClick="ButtonIndentPayed_Click" />
        <asp:Button ID="ButtonIndentPolPayed" runat="server" Text="Частично оплатить заказ" OnClick="ButtonIndent1Payed_Click" />
        <asp:Button ID="ButtonUpd" runat="server" Text="Обновить список" OnClick="ButtonUpd_Click" />
         <asp:GridView ID="dataGridView1" runat="server"  ShowHeaderWhenEmpty="True" BackColor="White" BorderColor="#336666" BorderWidth="3px" CellPadding="4" GridLines="Horizontal" BorderStyle="Double">
            <FooterStyle BackColor="White" ForeColor="#333333" />
            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="White" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#339966" ForeColor="White" Font-Bold="True" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#487575" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#275353" />
        </asp:GridView>
    </form>
</body>
</html>
