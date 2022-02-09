<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffHomePage.aspx.cs" Inherits="SitConnect.StaffHomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>Staff Home Page</legend>
                <br />
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                <br />
                <br />
                <asp:Button ID="btnLogout" runat="server" Text="Logout" Onclick="LogoutMe" Visible="false"/>
                <p></p>
            </fieldset>
        </div>
    </form>
</body>
</html>
