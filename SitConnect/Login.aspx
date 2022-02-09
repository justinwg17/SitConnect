<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SitConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LeI1kAeAAAAAFoWmCnKHlgwKeT9rCUnigt-46Cz"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>Login</legend>
                <p><asp:Label ID="validation" runat="server" Text="" Height="25px" ></asp:Label></p>
                <p>Email : <asp:TextBox ID="tb_userid" textmode="Email" runat="server" Height="25px" Width="137px"></asp:TextBox></p>
                <p>Password : <asp:TextBox ID="tb_pwd" runat="server" Height="25px" Width="137px"></asp:TextBox></p>
                <p><asp:Button ID="btnSubmit" runat="server" Text="Login" Onclick="LoginMe" Height="27px" Width="133px"/></p>
                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                <p><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Registration.aspx">Click here to Register</asp:HyperLink></p>
            </fieldset>
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LeI1kAeAAAAAFoWmCnKHlgwKeT9rCUnigt-46Cz', { action: 'Login' }).then(function (token) {
            document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
