using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Gold.DAL;

namespace Gold
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    TextBox txtUserName = LoginView1.FindControl("txtUserID") as TextBox;
                    TextBox txtPassword = LoginView1.FindControl("txtPassword") as TextBox;                    
                    string userName = "", password = "";
                    GetCookie(out userName, out password);
                    if (txtUserName != null)
                        txtUserName.Text = userName;
                    if (txtPassword != null)
                    {
                        //txtPassword.Text = password;//此句不行，因为TextMode=Password时控件禁止赋值
                        txtPassword.Attributes.Add("Value", password);
                    }

                }
                catch (Exception ex)
                {
                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "登录页加载时获取Cookie失败", ex);
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Label lbMessage = LoginView1.FindControl("lbMessage") as Label;
            try
            {
                TextBox txtUserName = LoginView1.FindControl("txtUserID") as TextBox;
                TextBox txtPassword = LoginView1.FindControl("txtPassword") as TextBox;
                CheckBox ChkRemember = LoginView1.FindControl("ChkRemember") as CheckBox;

                string userName = txtUserName.Text;
                string password = txtPassword.Text;

                if (Membership.ValidateUser(userName, password))
                {
                    if (ChkRemember.Checked) //记住用户名密码，放入cookie中
                    {
                        SetCookie(userName, password);
                    }
                    else //删除Cookie（实际是将Cookie设置为过期）
                    {
                        DelCookie();
                    }

                    SetUserDataAndRedirect(userName);
                }
                else
                {
                    lbMessage.Text = "用户名或密码错误！";
                }
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "用户[" + userName + "]登录成功！");
            }
            catch (Exception ex)
            {
                lbMessage.Text = "登录失败，请检查服务器及网络连接是否正常！";
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "登录异常", ex);
            }
        }

        // 添加自定义的值，然后导航到来到此页面之前的位置
        private void SetUserDataAndRedirect(string userId)
        {

            //存储用户基本信息到Session
            using (var edm = new GoldEntities())
            {
                Users userInfo = edm.Users.SingleOrDefault(o => o.UserId == userId);
                Session["UserInfo"] = userInfo;
                Session["UserName"] = userInfo.UserName;
            }

            // 获得Cookie
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userId, true);

            // 得到ticket凭据
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            // 根据之前的ticket凭据创建新ticket凭据，然后加入自定义信息
            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(
                ticket.Version, ticket.Name, ticket.IssueDate,
                ticket.Expiration, ticket.IsPersistent, "");

            // 将新的Ticke转变为Cookie值，然后添加到Cookies集合中
            authCookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);

            // 获得 来到登录页之前的页面，即url中return参数的值
            string url = FormsAuthentication.GetRedirectUrl(userId, true);

            Response.Redirect(url, false);//增加false参数，防止出现类似"由于代码已经过优化或者本机框架位于调用堆栈之上,无法计算表达式的值"的异常
        }

        //保存Cookie
        private void SetCookie(string userName, string password)
        {
            try
            {
                string desPwd = Utility.DesHelper.DESEncrypt(password);
                string sourcePwd = Utility.DesHelper.DESDecrypt(desPwd);

                //存用户名Cookie
                HttpCookie cUserName = new HttpCookie("LoginUserName");
                cUserName.Value = userName;
                cUserName.Expires = DateTime.Now.AddYears(1);//Cookie一年后过期
                HttpContext.Current.Response.Cookies.Add(cUserName);

                //存加密后的用户名Cookie
                HttpCookie cUserPassword = new HttpCookie("LoginUserPassword");
                cUserPassword.Value = desPwd;
                cUserPassword.Expires = DateTime.Now.AddYears(1);//Cookie一年后过期
                HttpContext.Current.Response.Cookies.Add(cUserPassword);
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "记录用户名密码Cookie失败", ex);
            }
        }

        //获取Cookie
        private void GetCookie(out string userName, out string password)
        {
            userName = "";
            password = "";
            try
            {
                if (HttpContext.Current.Request.Cookies["LoginUserName"] != null && HttpContext.Current.Request.Cookies["LoginUserName"].Value != null)
                {
                    userName = Request.Cookies["LoginUserName"].Value;
                }
                if (HttpContext.Current.Request.Cookies["LoginUserPassword"] != null && HttpContext.Current.Request.Cookies["LoginUserPassword"].Value != null)
                {
                    string desPwd = Request.Cookies["LoginUserPassword"].Value;//获取加密后的密码
                    password = Utility.DesHelper.DESDecrypt(desPwd);//解密
                    password = password.Replace("\0", "");//加密算法会对不满足4倍数长度的密码进行补零，此行是将多补的0去掉。
                }
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "获取用户名密码Cookie失败", ex);
            }
        }

        //删除Cookie（实际是将Cookie设置为过期）
        private void DelCookie()
        {
            try
            {
                //存用户名Cookie
                HttpCookie cUserName = new HttpCookie("LoginUserName");
                cUserName.Expires = DateTime.Now.AddYears(-1);//设置Cookie过期
                Response.Cookies.Add(cUserName);

                //存加密后的用户名Cookie
                HttpCookie cUserPassword = new HttpCookie("LoginUserPassword");
                cUserPassword.Expires = DateTime.Now.AddYears(-1);//设置Cookie过期
                Response.Cookies.Add(cUserPassword);
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "设置用户名密码Cookie过期时失败", ex);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            TextBox txtUserName = LoginView1.FindControl("txtUserID") as TextBox;
            TextBox txtPassword = LoginView1.FindControl("txtPassword") as TextBox;
            
            if (txtUserName != null)
                txtUserName.Text = "";
            if (txtPassword != null)
            {
                //txtPassword.Text = password;//此句不行，因为TextMode=Password时控件禁止赋值
                txtPassword.Attributes.Add("Value", "");
            }
        }
    }
}