using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data.Entity;
using System.Data.Objects;

using Gold.NCInvoke;
using System.Data;

namespace Gold.SystemSetting
{
    public partial class Departments : System.Web.UI.Page
    {

        private DropDownList departmentDropDownList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblCurrentDeptCode.Text = "";
                lblCurrentDeptName.Text = "";
                btnDelDept.Enabled = false;
                btnDelDept.CssClass = "ButtonImageStyleEnableFalse";

                InitTree();
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
                    rootNode.Select();
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

        protected void EntityDataSource1_Inserted(object sender, EntityDataSourceChangedEventArgs e)
        {
            if (e.Entity is Department)
            {
                TreeNode node = new TreeNode(((Department)(e.Entity)).DepartmentName, ((Department)(e.Entity)).DepartmentCode);
                TreeView1.SelectedNode.ChildNodes.Add(node);
                node.Select();
            }
        }

        protected void EntityDataSource1_Updated(object sender, EntityDataSourceChangedEventArgs e)
        {
            if (e.Entity is Department)
                TreeView1.SelectedNode.Text = ((Department)(e.Entity)).DepartmentName;
            
        }

        protected void DepartmentsDropDownList_Init(object sender, EventArgs e)
        {
            departmentDropDownList = sender as DropDownList;
            //if (DetailsView1.CurrentMode == DetailsViewMode.ReadOnly ||
            //    (this.DetailsView1.CurrentMode == DetailsViewMode.Edit &&
            //    TreeView1.SelectedValue == "root"))

            departmentDropDownList.SelectedValue = TreeView1.SelectedValue;
            departmentDropDownList.Enabled = false;
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            DetailsView1.ChangeMode(DetailsViewMode.ReadOnly);

            lblCurrentDeptCode.Text = "";
            lblCurrentDeptName.Text = "";
            btnDelDept.Enabled = false;
            btnDelDept.CssClass = "ButtonImageStyleEnableFalse";

            if (TreeView1.SelectedNode != null)
            {
                if (TreeView1.SelectedNode.Value != "root")
                {
                    lblCurrentDeptCode.Text = TreeView1.SelectedNode.Value;
                    lblCurrentDeptName.Text = TreeView1.SelectedNode.Text;
                    btnDelDept.Enabled = true;
                    btnDelDept.CssClass = "ButtonImageStyle";
                }
            }
        }

        protected void DetailsView1_DataBound(object sender, EventArgs e)
        {
            //var item = departmentDropDownList.Items.FindByValue(TreeView1.SelectedValue);
            //if (item != null)
            //    departmentDropDownList.Items.Remove(item);
        }

        protected void btnDelDept_Click(object sender, EventArgs e)
        {
            if (TreeView1.SelectedNode != null && string.IsNullOrEmpty(TreeView1.SelectedNode.Value) == false)
            {
                string msg = "";
                try 
                {
                    string deptcode = lblCurrentDeptCode.Text;
                    string deptname=lblCurrentDeptName.Text;
                    if (deptcode == "root") 
                    {
                        ShowMessageBox("顶级部门不可删除！");
                        return;
                    }
                    using (var edm = new GoldEntities())
                    {
                        Department query = (from p in edm.Department
                                    where p.DepartmentCode == deptcode
                                    select p).FirstOrDefault<Department>();
                        edm.Department.DeleteObject(query);
                        int affectRows= edm.SaveChanges();
                        msg = "部门["+deptname+"]删除成功！[影响行数" + affectRows.ToString() + "]";
                    }
                    ShowMessageBox(msg);

                    //删除成功后清除界面信息
                    lblCurrentDeptCode.Text = "";
                    lblCurrentDeptName.Text = "";
                    btnDelDept.Enabled = false;
                    btnDelDept.CssClass = "ButtonImageStyleEnableFalse";
                }
                catch (Exception ex) 
                {
                    msg = "删除失败";
                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "删除部门信息出错！", ex);
                    ShowMessageBox(msg, ex);
                }
                //重新加载TreeView
                InitTree();
            }
            else 
            {
                ShowMessageBox("请先点击选中左侧的部门，然后再点击此删除按钮！");
            }
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

                string typeArgs = "Department";
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

                //GridViewBind();//数据导入成功后重新绑定数据
                //gv_SpecList.DataBind();
                InitTree();//重新绑定部门信息
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