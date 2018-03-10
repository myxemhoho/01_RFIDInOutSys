using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using System.Xml;

namespace Gold.SystemSetting
{
    public partial class RolesRight : System.Web.UI.Page
    {
        string[] rolesArray;
        IList<SiteMapNode> siteNodes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind roles to GridView.
                //rolesArray = Roles.GetAllRoles();
                //RolesGrid.DataSource = rolesArray;
                //RolesGrid.DataBind();
                BindRoleList();

                bool x = Roles.IsUserInRole("123", "仓管员");
                MembershipUser user = Membership.GetUser("123");

                //SiteMapNode node = SiteMap.RootNode.ChildNodes[0];
                //SiteMapNodeCollection cc = node.ChildNodes;
                //node.Roles.Add("系统管理员");
                //SiteMapGrid.DataSource = cc;
                //SiteMapGrid.DataBind();


                lblRoleName.Text = "";

                TreeView1.Enabled = false;
                btnSave.Enabled = false;

                //string path = Server.MapPath("~/Web.sitemap");
                //List<Utility.SiteMapModel> list = new List<Utility.SiteMapModel>();
                //bool result = Utility.XmlHelper.ReadFirstLevelNodeList(path, out list);
                //if (result && list.Count > 0)
                //{
                //    TreeView1.Nodes.Clear();

                //    foreach (Utility.SiteMapModel temp in list)
                //    {
                //        TreeNode node = new TreeNode(temp.Title, temp.Title);
                //        TreeView1.Nodes.Add(node);
                //    }
                //}

                TreeView1.Attributes.Add("onclick", "OnTreeNodeChecked()");

                LoadTreeView();
            }
        }

        private void LoadTreeView()
        {
            TreeView1.Nodes.Clear();
            SiteMapNodeCollection siteNodeList = SiteMap.RootNode.ChildNodes;
            foreach (SiteMapNode sNode in siteNodeList)
            {
                TreeNode FirstLevelNode = new TreeNode();
                FirstLevelNode.Text = sNode.Title;
                FirstLevelNode.Value = sNode.Title;

                if (sNode.ChildNodes.Count > 0)
                {
                    foreach (SiteMapNode s2Node in sNode.ChildNodes)
                    {
                        TreeNode SecondLevelNode = new TreeNode();
                        SecondLevelNode.Text = s2Node.Title;
                        SecondLevelNode.Value = s2Node.Title;
                        FirstLevelNode.ChildNodes.Add(SecondLevelNode);
                    }
                }
                TreeView1.Nodes.Add(FirstLevelNode);
            }
        }

        private void LoadTreeView(string roleName)
        {
            TreeView1.Nodes.Clear();
            SiteMapNodeCollection siteNodeList = SiteMap.RootNode.ChildNodes;
            foreach (SiteMapNode sNode in siteNodeList)
            {
                TreeNode FirstLevelNode = new TreeNode();
                FirstLevelNode.Text = sNode.Title;
                FirstLevelNode.Value = sNode.Title;
                if (sNode.Roles.Contains(roleName) || sNode.Roles.Contains("*"))
                    FirstLevelNode.Checked = true;

                if (sNode.ChildNodes.Count > 0)
                {
                    foreach (SiteMapNode s2Node in sNode.ChildNodes)
                    {
                        TreeNode SecondLevelNode = new TreeNode();
                        SecondLevelNode.Text = s2Node.Title;
                        SecondLevelNode.Value = s2Node.Title;

                        if (s2Node.Roles.Contains(roleName) || s2Node.Roles.Contains("*"))
                            SecondLevelNode.Checked = true;

                        FirstLevelNode.ChildNodes.Add(SecondLevelNode);
                    }
                }
                TreeView1.Nodes.Add(FirstLevelNode);
            }
        }

        private void BindRoleList()
        {
            rolesArray = Roles.GetAllRoles();
            RolesGrid.DataSource = rolesArray;
            RolesGrid.DataBind();
        }

        public void btnCreate_OnClick(object sender, EventArgs args)
        {
            ClearLabel();
            string createRole = txtRoleName.Text;

            try
            {
                if (Roles.RoleExists(createRole))
                {
                    lblMessage.Text = "角色 '" + Server.HtmlEncode(createRole) + "' 已经存在，请重新填写角色名称！";
                    return;
                }

                Roles.CreateRole(createRole);

                lblMessage.Text = "角色 '" + Server.HtmlEncode(createRole) + "' 创建成功.";

                // Re-bind roles to GridView.


                //rolesArray = Roles.GetAllRoles();
                //RolesGrid.DataSource = rolesArray;
                //RolesGrid.DataBind();
                BindRoleList();
            }
            catch (Exception e)
            {
                lblMessage.Text = "角色 '" + Server.HtmlEncode(createRole) + "' 创建失败!<br/>详细信息：" + Utility.LogHelper.GetExceptionMsg(e);
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "角色创建失败", e);
                //Response.Write(e.ToString());
            }

        }

        public void btnSave_OnClick(object sender, EventArgs args)
        {
            ClearLabel();
            try
            {
                if (lblRoleName.Text == "")
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "请先选择角色并编辑角色下的权限，然后点击“保存按钮”！");
                    return;
                }
                else
                {
                    string roleName = lblRoleName.Text;
                    string path = Server.MapPath("~/Web.sitemap");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("ab", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");//sitemap文件中的名称空间


                    XmlNode rootSiteMap = doc.GetElementsByTagName("siteMap")[0];//选择根节点

                    foreach (TreeNode temp in TreeView1.Nodes)//遍历TreeView中一级节点
                    {
                        XmlNode xn = rootSiteMap.SelectSingleNode("ab:siteMapNode/ab:siteMapNode[@title='" + temp.Text + "']", nsmgr);//选择web.sitemap一级元素
                        XmlElement xe = xn as XmlElement;
                        if (xe == null)
                            continue;
                        string oldRoles = xe.Attributes["roles"].Value;

                        if (oldRoles.Contains("*") == false)//如果角色不为全部时，则设置角色值
                        {
                            System.Text.StringBuilder newRoles = new System.Text.StringBuilder(oldRoles);

                            if (temp.Checked == true)//选中时增加角色
                            {
                                if (string.IsNullOrEmpty(roleName) == false && oldRoles.Contains(roleName) == false)
                                {
                                    if (newRoles.Length > 0)
                                        newRoles.Append(",");
                                    newRoles.Append(roleName);//增加角色
                                }
                            }
                            else //取消选中时去掉角色
                            {
                                if (string.IsNullOrEmpty(roleName) == false && oldRoles.Contains(roleName))
                                {
                                    int beforeIndex = newRoles.ToString().IndexOf(roleName) - 1;

                                    if (beforeIndex > 0 && newRoles[beforeIndex] == ',')
                                    {
                                        newRoles = newRoles.Replace("," + roleName, "");
                                    }
                                    else
                                    {
                                        newRoles = newRoles.Replace(roleName, "");
                                    }
                                }
                            }
                            xe.SetAttribute("roles", newRoles.ToString());//设置web.sitemap一级元素的角色
                        }

                        if (temp.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode secondLevelNode in temp.ChildNodes) //遍历TreeView中一级节点
                            {
                                XmlNode secondXNode = xn.SelectSingleNode("ab:siteMapNode[@title='" + secondLevelNode.Text + "']", nsmgr);//选择web.sitemap二级元素
                                XmlElement secondXE = secondXNode as XmlElement;
                                if (secondXE == null)
                                    continue;
                                string oldSecondRoles = secondXE.Attributes["roles"].Value;
                                if (oldSecondRoles.Contains("*") == false)
                                {
                                    System.Text.StringBuilder newSecondRoles = new System.Text.StringBuilder(oldSecondRoles);

                                    if (secondLevelNode.Checked)//选中则增加角色
                                    {
                                        if (string.IsNullOrEmpty(roleName) == false
                                            && oldSecondRoles.Contains(roleName) == false)
                                        {
                                            if (newSecondRoles.Length > 0)
                                                newSecondRoles.Append(",");
                                            newSecondRoles.Append(roleName);
                                        }
                                    }
                                    else //取消选中则去掉角色
                                    {
                                        if (string.IsNullOrEmpty(roleName) == false && oldSecondRoles.Contains(roleName))
                                        {
                                            int beforeIndex = newSecondRoles.ToString().IndexOf(roleName) - 1;

                                            if (beforeIndex > 0 && newSecondRoles[beforeIndex] == ',')
                                            {
                                                newSecondRoles = newSecondRoles.Replace("," + roleName, "");
                                            }
                                            else
                                            {
                                                newSecondRoles = newSecondRoles.Replace(roleName, "");
                                            }
                                        }
                                    }

                                    secondXE.SetAttribute("roles", newSecondRoles.ToString());//设置web.sitemap二级元素的角色
                                }
                            }
                        }

                    }

                    doc.Save(path);//注意要设置Web.sitemap的访问权限为asp.net用户可写入，否则不能保存

                    lblSaveMsg.Text = "保存成功！";
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存成功！");
                }
            }
            catch (Exception ex)
            {
                string msg = "保存权限信息失败！\n详细信息：" + Utility.LogHelper.GetExceptionMsg(ex);
                lblSaveMsg.Text = msg;
                DAL.CommonConvert.ShowMessageBox(this.Page, "保存权限信息失败！");
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "保存权限信息失败", ex);
            }
        }

        protected void RolesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ClearLabel();
            try
            {
                GridView grid = sender as GridView;
                string roleName = "";
                LinkButton lbtn = grid.Rows[e.RowIndex].FindControl("lbtnRoleName") as LinkButton;
                if (lbtn != null)
                {
                    roleName = lbtn.Text.Trim();
                    bool result = false;

                    if (roleName == "系统管理员" || roleName == "物流部操作员")
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "[" + roleName + "]角色不可删除！");
                        return;
                    }

                    result = Roles.DeleteRole(roleName);
                    if (result)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "删除成功！");
                        BindRoleList();
                    }
                    else
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "删除失败！详细信息：" + Utility.LogHelper.GetExceptionMsg(ex);
                lblDelMsg.Text = msg;
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败！");
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "角色删除失败", ex);
            }
        }

        protected void RolesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                ClearLabel();

                string roleName = e.CommandArgument.ToString();
                lblRoleName.Text = roleName;

                //string path = Server.MapPath("~/Web.sitemap");
                //List<Utility.SiteMapModel> list = new List<Utility.SiteMapModel>();
                //bool result = Utility.XmlHelper.ReadFirstLevelNodeList(path, out list);
                //if (result && list.Count > 0)
                //{
                //    TreeView1.Nodes.Clear();

                //    foreach (Utility.SiteMapModel temp in list)
                //    {
                //        TreeNode node = new TreeNode(temp.Title, temp.Title);
                //        if (string.IsNullOrEmpty(roleName) == false && temp.Roles.Contains(roleName))
                //            node.Checked = true;
                //        TreeView1.Nodes.Add(node);
                //    }
                //}
                LoadTreeView(roleName);
                TreeView1.ExpandAll();

                if (roleName == "系统管理员")
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "[系统管理员]角色不可编辑！");
                    TreeView1.Enabled = false;
                    btnSave.Enabled = false;
                }
                else
                {
                    TreeView1.Enabled = true;
                    btnSave.Enabled = true;
                }
            }
        }

        protected void ClearLabel()
        {
            lblDelMsg.Text = "";
            lblMessage.Text = "";
            lblSaveMsg.Text = "";
        }
    }


}