using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using System.Web.Configuration;
using System.Configuration;
using System.Collections;

namespace Gold
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                //如果Sessions超时，则注销用户
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                LoginName control = HeadLoginView.FindControl("HeadLoginName") as LoginName;
                if (control != null)
                    control.FormatString = Session["UserName"].ToString();


                //Configuration conf = WebConfigurationManager.OpenWebConfiguration("~");
                //SystemWebSectionGroup swsg = (SystemWebSectionGroup)conf.GetSectionGroup("system.web");
                //AuthorizationRule rule = new AuthorizationRule(AuthorizationRuleAction.Deny);
                //rule.Users.Add("?");
                //swsg.Authorization.Rules.Add(rule);


                //conf.Save(ConfigurationSaveMode.Modified);
            }
        }

        protected void NavigationMenu_DataBound(object sender, EventArgs e)
        {
            if (Session["UserInfo"] == null)
                return;

            if (this.IsPostBack)
                return;

            DAL.Users userInfo = Session["UserInfo"] as DAL.Users;
            string userID = userInfo == null ? "" : userInfo.UserId;

            //获取sitemap中的所有节点
            SiteMapNodeCollection list = SiteMap.RootNode.GetAllNodes();
            //获取该用户下拥有的所有角色
            string[] roleNameList = Roles.GetRolesForUser(userID);
            //该用户各个角色拥有的菜单功能项
            List<string[]> menuItemTitleHas = new List<string[]>();
            //取出拥有的菜单后，将List中各个String[]取并集，显示给页面
            foreach (string role in roleNameList)
            {
                List<string> strTheRoleHasMenu = new List<string>();
                GetItem(NavigationMenu, list, role, out strTheRoleHasMenu);//遍历每个角色下拥有的菜单项
                if (strTheRoleHasMenu.Count > 0)
                    menuItemTitleHas.Add(strTheRoleHasMenu.ToArray());
            }

            //合并权限菜单并集
            List<string> theUserHasMenu = new List<string>();
            foreach (string[] strList in menuItemTitleHas)
            {
                foreach (string str in strList)
                {
                    if (theUserHasMenu.Contains(str) == false)
                    {
                        theUserHasMenu.Add(str);
                    }
                }
            }


            //设置菜单显示
            SetMenuItemShow(ref this.NavigationMenu, theUserHasMenu);

        }

        /// <summary>
        /// 根据用户角色下拥有的菜单设置界面中导航菜单的可见性
        /// </summary>
        /// <param name="menu">界面中菜单控件</param>
        /// <param name="theUserHasMenu">登录用户有权访问的的菜单名</param>
        private void SetMenuItemShow(ref Menu menu, List<string> theUserHasMenu)
        {
            List<int> delItemIndex = new List<int>();
            int i = 0;
            foreach (MenuItem menuItemFirstLevel in menu.Items)//遍历导航菜单控件中一级菜单，获取其无权限的菜单并记录下来，待删除
            {
                if (theUserHasMenu.Contains(menuItemFirstLevel.Text) == false)
                {
                    delItemIndex.Add(i);
                }
                else
                {
                    int j = 0;
                    List<int> delChildIndex = new List<int>();
                    foreach (MenuItem secLevel in menuItemFirstLevel.ChildItems)//遍历导航菜单控件中二级菜单，获取其无权限的菜单并记录下来，待删除
                    {
                        if (theUserHasMenu.Contains(secLevel.Text) == false)
                        {
                            delChildIndex.Add(j);
                        }
                        j++;
                    }
                    delChildIndex.Reverse();
                    foreach (int indexChild in delChildIndex)//删除二级菜单中的无权限菜单
                    {
                        menuItemFirstLevel.ChildItems.RemoveAt(indexChild);
                    }
                }

                i++;
            }

            delItemIndex.Reverse();
            foreach (int index in delItemIndex)//删除一级菜单中的无权限菜单
            {
                menu.Items.RemoveAt(index);
            }
        }


        /// <summary>
        /// 根据界面菜单、Web.sitemap中role和menu及用户角色名筛选出用户可访问的菜单名集合
        /// </summary>
        /// <param name="menu">界面中菜单导航控件</param>
        /// <param name="nodeCollection">web.sitemap的所有节点</param>
        /// <param name="roleName">用户的一个角色名</param>
        /// <param name="menuItemTitleHas">输出参数：用户该角色下可访问的菜单名</param>
        private void GetItem(Menu menu, SiteMapNodeCollection nodeCollection, string roleName, out List<string> menuItemTitleHas)
        {
            menuItemTitleHas = new List<string>();

            GetMenuItem(menu.Items, roleName, nodeCollection, ref menuItemTitleHas);
        }

        /// <summary>
        /// 递归实现， 根据界面菜单、Web.sitemap中role和menu及用户角色名筛选出用户可访问的菜单名集合
        /// </summary>
        /// <param name="menu">界面中菜单导航控件</param>
        /// <param name="nodeCollection">web.sitemap的所有节点</param>
        /// <param name="roleName">用户的一个角色名</param>
        /// <param name="menuItemTitleHas">输出参数：用户该角色下可访问的菜单名</param>
        private void GetMenuItem(MenuItemCollection menuItemCollection, string roleName, SiteMapNodeCollection nodeCollection, ref List<string> menuItemTitleHas)
        {
            foreach (MenuItem menuItem in menuItemCollection)
            {

                string title = menuItem.Text;
                if (IsRoleHasTile(roleName, title, nodeCollection))
                {
                    if (!menuItemTitleHas.Contains(title))
                        menuItemTitleHas.Add(title);
                }
                if (menuItem.ChildItems.Count > 0)
                {
                    GetMenuItem(menuItem.ChildItems, roleName, nodeCollection, ref menuItemTitleHas);//对子节点递归
                }
            }
        }

        /// <summary>
        /// 根据角色名，菜单名遍历Web.sitemap中的各个节点，判断该角色是否有该菜单的访问权限
        /// </summary>
        /// <param name="roleName">登录用户的角色名</param>
        /// <param name="title">菜单名</param>
        /// <param name="list">web.sitemap所有节点集合</param>
        /// <returns>是否有权限</returns>
        bool IsRoleHasTile(string roleName, string title, SiteMapNodeCollection list)
        {
            IEnumerator node = list.GetEnumerator();
            while (node.MoveNext())
            {
                SiteMapNode sitenode = node.Current as SiteMapNode;
                if (sitenode != null)
                {
                    if (sitenode.Title == title && (sitenode.Roles.Contains(roleName) || sitenode.Roles.Contains("*")))
                        return true;
                }
            }
            return false;

        }

        //记录注销日志
        protected void HeadLoginStatus_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DAL.Users userInfo = Session["UserInfo"] as DAL.Users;
                string userID = userInfo == null ? "" : (userInfo.UserId == null ? "" : userInfo.UserId);

                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "用户[" +userID + "]退出系统!");
            }
        }
    }


}
