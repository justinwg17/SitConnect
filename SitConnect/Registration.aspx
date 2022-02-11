<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SitConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration Page</title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_Password.ClientID %>').value;
            if (str.length < 12) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at least 12 characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if(str.search(/[0-9]/) == -1)
            {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 lowercase";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 uppercase";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special_characters");
            }
            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent";
            document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }
        function validateCreditCard() {
            var str = document.getElementById('<%=tb_CreditCard.ClientID %>').value;
            if (str.length != 16) {
                document.getElementById("lbl_CCchecker").innerHTML = "Credit Card number is 16 digits";
                document.getElementById("lbl_CCchecker").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[^0-9]/) > -1) {
                document.getElementById("lbl_CCchecker").innerHTML = "Must be numbers";
                document.getElementById("lbl_CCchecker").style.color = "Red";
                return ("numbers");
            }
            document.getElementById("lbl_CCchecker").innerHTML = "Valid Credit Card Number";
            document.getElementById("lbl_CCchecker").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <fieldset>
                <legend>Registration</legend>
                <p><asp:Label ID="validation" runat="server" Text="" Height="25px" ></asp:Label></p>
                <p>First Name : <asp:TextBox ID="tb_FirstName" runat="server" Height="25px" Width="137px"></asp:TextBox></p>
                <p>Last Name : <asp:TextBox ID="tb_LastName" runat="server" Height="25px" Width="137px"></asp:TextBox></p>
                <p>Email Address : <asp:TextBox ID="tb_Email" TextMode="Email" runat="server" Height="25px" Width="300px"></asp:TextBox></p>
                <p>Password : <asp:TextBox ID="tb_Password" runat="server" Height="25px" Width="137px" onkeyup="javascript:validate()"></asp:TextBox><asp:Label ID="lbl_pwdchecker" runat="server" Text="" Height="25px" ></asp:Label></p>
                <p><asp:Button ID="btnCheckPwd" runat="server" Text="Check Password" onclick="btn_checkPassword_Click" Height="27px" Width="300px"/><asp:Label ID="lbl_pwdchecker2" runat="server" Text="" Height="25px" ></asp:Label></p>
                <p>Credit Card Number : <asp:TextBox ID="tb_CreditCard" runat="server" Height="25px" Width="200px" onkeyup="javascript:validateCreditCard()"></asp:TextBox><asp:Label ID="lbl_CCchecker" runat="server" Text="" Height="25px" ></asp:Label></p>
                <p>Date of Birth : <asp:Calendar ID="DOB" runat="server"></asp:Calendar></p>
                <p>Photo : <asp:FileUpload ID="PhotoUpload" runat="server" /></p>
                
                <br />
                <p><asp:Button ID="btnSubmit" runat="server" Text="Register" OnClick="btn_Submit_Click" Height="27px" Width="133px"/></p>
                <p><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Login.aspx">Click here to Login</asp:HyperLink></p>
               <p><asp:Label ID="lb_error1" runat="server" Text="" Height="25px" ></asp:Label></p>
            </fieldset>
        </div>
    </form>
</body>
</html>
