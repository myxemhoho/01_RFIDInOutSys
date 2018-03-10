<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Gold.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RFID 库房管理系统</title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
//        function Clear() {
//        var LoginName="LoginView1_txtUserID";
//        var LoginPwd = "LoginView1_txtPassword";
//        var txtLoginName = document.getElementById(LoginName);
//        if (txtLoginName != null && txtLoginName != undefined)
//            txtLoginName.value = "";

//        var txtLoginPwd = document.getElementById(LoginPwd);
//        if (txtLoginPwd != null && txtLoginPwd != undefined)
//            txtLoginPwd.value = "";

//         }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="loginPage">    
        <asp:LoginView ID="LoginView1" runat="server">
            <AnonymousTemplate>
                <div class="loginForm">
                    <table class="login_row">
                        <tr style="height:40px;">
                            <td style="width: 70px; text-align: right">
                                用户名：
                            </td>
                            <td>
                                <asp:TextBox ID="txtUserID" runat="server" class="loginText" MaxLength="32"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="用户名必需!"
                                    ControlToValidate="txtUserID" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="height:40px;">
                            <td style="width: 70px; text-align: right">
                                密&nbsp;&nbsp;&nbsp;码：
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassword" runat="server" class="loginText" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="密码必需!"
                                    ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="height:15px;">
                        <td></td>
                        <td>
                            <asp:CheckBox ID="ChkRemember" runat="server" Checked=true style="color:White; font-size:10pt;" Text="记住用户名密码" />
                        </td>
                        </tr>
                        <tr style="height:40px;">
                            <td>
                            </td>
                            <td style="vertical-align:middle;">
                                <asp:Button ID="btnLogin" runat="server" Text="登 录" Width="80px" Height="26px" 
                                 CssClass="button" onclick="btnLogin_Click" />
                                <%--<input id="btnReset" type="reset"  value="重 置" style="width:80px; height:26px" onclick="javascript:Clear();" 
                                 class="button" />--%>
                                 <asp:Button ID="btnReset" runat="server" Text="重 置" Width="80px" Height="26px" CausesValidation="false" 
                                 CssClass="button" onclick="btnReset_Click" />
                                <asp:Label ID="lbMessage" runat="server" ForeColor="Red" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </AnonymousTemplate>
            <LoggedInTemplate>
            <div class="loginForm">
                    <table class="login_row">
                        <tr style="height:40px;">
                            <td style="width: 200px; text-align: left">
                                登录超时，请重新登录！<br />
                                &nbsp; [
                        <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="重新登录"
                            LogoutPageUrl="~/" />
                        ]
                            </td>                            
                        </tr>                        
                    </table>
                </div>
            </LoggedInTemplate>
        </asp:LoginView>    
    </div>
    </form>
</body>
</html>
