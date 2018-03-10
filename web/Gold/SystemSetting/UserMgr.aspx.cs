using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Web.Security;
using System.Text;

using Gold.NCInvoke;
using System.Data;

namespace Gold.SystemSetting
{
    public partial class UserMgr : System.Web.UI.Page
    {
        string[] rolesArray;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitTree();

                rolesArray = Roles.GetAllRoles();
                cboxListRoles.DataSource = rolesArray;
                cboxListRoles.DataBind();
            }
        }

        private void InitTree()
        {
            TreeView1.Nodes.Clear();

            using (var edm = new GoldEntities())
            {
                var query = from p in edm.Department
                            where p.DepartmentCode == "root"
                            select p;

                Department root;
                if (query.Count<Department>() == 0)
                {
                    var tmp = edm.CreateObject<Department>();
                    tmp.DepartmentCode = "root";
                    tmp.DepartmentName = "深圳金币";
                    tmp.Parent = "0";

                    edm.AddToDepartment(tmp);
                    edm.SaveChanges();

                    root = tmp;
                }
                else
                    root = query.First();

                TreeNode rootNode = new TreeNode();
                rootNode.Text = root.DepartmentName;
                rootNode.Value = root.DepartmentCode;
                CreatChildNodes(rootNode, root, edm);
                TreeView1.Nodes.Add(rootNode);
                rootNode.ExpandAll();

                if (TreeView1.SelectedNode == null)
                {
                    rootNode.Select();
                    GridViewBind();//显示全部用户
                }
            }
        }

        private void CreatChildNodes(TreeNode node, Department department, GoldEntities edm)
        {
            var children = from temp in edm.Department
                           where temp.Parent == department.DepartmentCode
                           && temp.DepartmentCode != "root"
                           select temp;

            if (children.Any())
            {
                foreach (Department child in children)
                {
                    TreeNode childNode = new TreeNode();
                    childNode.Text = child.DepartmentName;
                    childNode.Value = child.DepartmentCode;
                    CreatChildNodes(childNode, child, edm);
                    node.ChildNodes.Add(childNode);
                }
            }

        }

        protected void FormView2_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Create":
                    ClearRolesCheckList();
                    FormView2.ChangeMode(FormViewMode.Insert);
                    break;
                case "Edit":
                    string userName = GridView1.SelectedValue.ToString();
                    string[] userRoles = Roles.GetRolesForUser(userName);
                    SetRolesCheckList(userRoles);
                    break;
            }
        }

        private void SetRolesCheckList(string[] checks)
        {
            foreach (ListItem item in cboxListRoles.Items)
            {
                item.Selected = false;
                if (checks == null || checks.Count() < 1)
                    continue;
                else
                {
                    foreach (string tmp in checks)
                    {
                        if (item.Text == tmp)
                            item.Selected = true;
                    }
                }
            }
        }

        private string[] GetRolesFromCheckList()
        {
            List<String> userRoles = new List<string>();
            foreach (ListItem item in cboxListRoles.Items)
            {
                if (item.Selected)
                {
                    userRoles.Add(item.Text);
                }
            }

            return userRoles.ToArray();
        }

        private void ClearRolesCheckList()
        {
            foreach (ListItem item in cboxListRoles.Items)
                item.Selected = false;
        }

        protected void FormView2_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            var userName = e.Values["UserId"];
            try
            {
                MembershipUser user = Membership.CreateUser(userName.ToString(), "123456");
                string[] userRoles = GetRolesFromCheckList();
                if (userRoles != null && userRoles.Count() > 0)
                    Roles.AddUserToRoles(userName.ToString(), userRoles);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                string msg = Utility.LogHelper.GetExceptionMsg(ex).Replace("\r", "").Replace("\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "新增失败！详细信息：" + msg);
            }

        }

        protected void FormView2_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            //GridView1.DataBind();
            if (e.AffectedRows > 0)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "新增成功！");
            }
            else
            {
                string msg = Utility.LogHelper.GetExceptionMsg(e.Exception).Replace("\r", "").Replace("\n", "");
                if (msg.Contains("Cannot insert duplicate key"))
                    msg = "系统中已存在同名用户！请更改用户名！";
                DAL.CommonConvert.ShowMessageBox(this.Page, "新增失败！详细信息：" + msg);
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "新增用户失败！", e.Exception);
            }
            GridViewBind();
        }

        protected void FormView2_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            string userName = e.OldValues["UserId"].ToString();
            try
            {
                string[] oldRoles = Roles.GetRolesForUser(userName);
                string[] newRoles = GetRolesFromCheckList();

                string[] removeRoles = oldRoles.Except(newRoles).ToArray();
                if (removeRoles != null && removeRoles.Count() > 0)
                    Roles.RemoveUserFromRoles(userName, removeRoles);

                string[] addRoles = newRoles.Except(oldRoles).ToArray();
                if (addRoles != null && addRoles.Count() > 0)
                    Roles.AddUserToRoles(userName, addRoles);

                //if (userRoles != null && userRoles.Count() > 0)
                //    Roles.AddUserToRoles(userName.ToString(), userRoles);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                string msg = Utility.LogHelper.GetExceptionMsg(ex).Replace("\r", "").Replace("\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "更新失败！详细信息：" + msg);
            }
        }

        protected void FormView2_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            //GridView1.DataBind();
            if (e.AffectedRows > 0)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "更新成功！");
            }
            else
            {
                string msg = Utility.LogHelper.GetExceptionMsg(e.Exception).Replace("\r", "").Replace("\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "更新失败！详细信息：" + msg);
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "更新用户失败！", e.Exception);
            }
            GridViewBind();
        }


        protected void drpDepartment_Init(object sender, EventArgs e)
        {
            DropDownList drpDept = sender as DropDownList;
            drpDept.SelectedValue = TreeView1.SelectedValue;
        }

        protected void txtRoles_Load(object sender, EventArgs e)
        {
            TextBox txtRoles = sender as TextBox;
            txtRoles.Text = "";
            foreach (ListItem item in cboxListRoles.Items)
            {
                if (item.Selected)
                {
                    txtRoles.Text += item.Value;
                    txtRoles.Text += ";";
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            //switch (e.CommandName)
            //{
            //    case "Select":
            //        string userName = e.CommandArgument.ToString();
            //        string[] userRoles = Roles.GetRolesForUser(userName);
            //        SetRolesCheckList(userRoles);
            //        FormView2.ChangeMode(FormViewMode.ReadOnly);
            //        break;

            //}

            if (e.CommandName == "Select")
            {
                string userName = e.CommandArgument.ToString();
                string[] userRoles = Roles.GetRolesForUser(userName);
                SetRolesCheckList(userRoles);
                FormView2.ChangeMode(FormViewMode.ReadOnly);
            }
            else if (e.CommandName == "ResetPassword")
            {
                try
                {
                    //注意必须保证web.config中membership的配置为enablePasswordReset="true" requiresQuestionAndAnswer="false"

                    string userName = e.CommandArgument.ToString();
                    MembershipUser membershipUser = Membership.GetUser(userName);
                    string newTempPassword = membershipUser.ResetPassword();
                    bool result = membershipUser.ChangePassword(newTempPassword, "123456");
                    System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
                    strMsg.Append("用户[");
                    strMsg.Append(userName);
                    strMsg.Append("]密码");
                    if (result)
                        strMsg.Append("重置成功!新密码为[123456]");
                    else
                        strMsg.Append("重置失败！");
                    DAL.CommonConvert.ShowMessageBox(this.Page, strMsg.ToString());

                    //记录日志
                    if (Session["UserName"] != null)
                    {
                        strMsg.Append("    重置操作员：");
                        strMsg.Append(Session["UserName"].ToString());
                    }
                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, strMsg.ToString());
                }
                catch (Exception ex)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "密码重置操作失败！");
                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "密码重置", ex);
                }
            }
            else if (e.CommandName == "DeleteUser")
            {
                try
                {

                    string userID = e.CommandArgument.ToString();
                    bool delResult = Membership.DeleteUser(userID);//先删除Membership中用户
                    if (delResult)
                    {
                        using (GoldEntities context = new GoldEntities())
                        {
                            Users delUser = (from r in context.Users where r.UserId == userID select r).FirstOrDefault<Users>();
                            context.Users.DeleteObject(delUser);

                            int delRow = context.SaveChanges();
                            if (delRow > 0)
                            {
                                string msg = "用户[" + userID + "]删除成功！";
                                DAL.CommonConvert.ShowMessageBox(this.Page, msg);

                                
                            }
                            else
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败！");
                            }

                            GridViewBind();//删除后重新绑定数据
                        }
                    }
                    else 
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败！");
                    }
                }
                catch (Exception ex)
                {
                    string msg = Utility.LogHelper.GetExceptionMsg(ex).Replace("\r", "").Replace("\n", "");
                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新失败！详细信息：" + msg);
                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "更新用户失败！", ex);
                }
            }

        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            GridViewBind();
        }

        private void GridViewBind()
        {
            try
            {
                string deptCode = TreeView1.SelectedValue;
                using (GoldEntities context = new GoldEntities())
                {
                    var result = (from r in context.Users select r).ToList();
                    if (deptCode == "root")
                    {
                        deptCode = "";
                    }
                    if (!string.IsNullOrEmpty(deptCode))
                    {
                        result = (from r in result where r.DepartmentCode == deptCode select r).ToList();
                    }



                    string sortExpression = GridView1.Attributes["sortExpression"];
                    SortDirection sortDirection = GridView1.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        result = result.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();


                    GridView1.DataSource = result;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex).Replace("\r", "").Replace("\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "加载用户列表信息失败！详细信息：" + msg);
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "加载用户列表信息失败！", ex);
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(GridView1.Attributes["sortExpression"]) && "ASC".Equals(GridView1.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            GridView1.Attributes.Add("sortExpression", sortExpression);
            GridView1.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        #region 仿MessageBox

        /// <summary>
        /// 显示简单消息(例如"保存成功")
        /// </summary>
        /// <param name="msg">提示信息</param>
        void ShowMessageBox(string msg)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = false;//当只显示简单信息时，“详细信息栏”不显示
            lblMessageContentException.Visible = false;
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 显示提示信息和详细的异常信息（例如“保存失败！”,"详细信息：XXX"）
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="ex">异常信息</param>
        void ShowMessageBox(string msg, Exception ex)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = true;//当显示异常信息时，“详细信息栏”进行显示
            lblMessageContentException.Visible = true;
            string exMsg = Utility.LogHelper.GetExceptionMsg(ex);
            lblMessageContentException.Text = exMsg;

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 在服务端关闭提示信息(此方法一般不要用，关闭时用弹出div的javascript关闭会性能更好)
        /// </summary>
        void CloseMessageBox()
        {
            lblMessageContent.Text = "";
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Hide();//隐藏div
        }


        protected void showModalPopupServerOperatorButton_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Show();
        }
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Hide();
        }

        #endregion

        #region 用友NC数据导入
        protected void btnNCDataImport_Click(object sender, EventArgs e)
        {
            try
            {
                #region 查询数据

                string typeArgs = "UserInfo";
                string conditionArgs = "中国金币深圳经销中心";//string.Empty;
                DataTable dt = null;
                string queryMsg = "";
                string saveMsg = "";

                BasicInfoInvoke BasicInfoInvokeObj = BasicInfoInvokeFactory.CreateInstance(typeArgs, conditionArgs);

                if (BasicInfoInvokeObj.GetNCDataJoinRFID(out dt, out queryMsg) == false)
                {
                    ShowMessageBox("查询用友系统信息失败！详细信息：" + queryMsg);
                    return;
                }
                else
                {
                    bool result = BasicInfoInvokeObj.SaveToRFID(dt, out saveMsg);
                    ShowMessageBox(result == true ? "数据导入成功！" : "数据导入失败！", new Exception(saveMsg));
                }

                #endregion

                GridViewBind();//数据导入成功后重新绑定数据
            }
            catch (Exception ex)
            {
                ShowMessageBox("数据导入失败！", ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}