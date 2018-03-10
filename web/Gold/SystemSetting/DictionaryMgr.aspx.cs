using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.IO;
using System.Xml;
using System.Text;

namespace Gold.SystemSetting
{
    public partial class DictionaryMgr : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitTree();
            }
        }

        private void InitTree()
        {
            TreeView1.Nodes.Clear();

            using (var edm = new GoldEntities())
            {
                var root = (from d in edm.DataDict
                            where d.Category == "root"
                            select d).ToList();

                foreach (DataDict dict in root)
                {
                    TreeNode rootNode = new TreeNode();
                    rootNode.Text = dict.Name + "(" + dict.Code + ")";
                    rootNode.Value = dict.Code;
                    TreeView1.Nodes.Add(rootNode);
                }

            }
        }

        protected void btnSaveRoot_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRootCode.Text.Trim()))
            {
                lblrootMessage.Text = "代码不能为空！";
                return;
            }

            if (string.IsNullOrEmpty(txtRootName.Text.Trim()))
            {
                lblrootMessage.Text = "名称不能为空！";
                return;
            }

            using (var edm = new GoldEntities())
            {
                var tmp = edm.CreateObject<DataDict>();
                tmp.Name = txtRootName.Text.Trim();
                tmp.Code = txtRootCode.Text.Trim();
                tmp.Category = "root";
                tmp.Enabled = true;

                edm.AddToDataDict(tmp);
                edm.SaveChanges();

                TreeNode node = new TreeNode();
                node.Value = tmp.Code;
                node.Text = tmp.Name;
                TreeView1.Nodes.Add(node);

                txtRootCode.Text = "";
                txtRootName.Text = "";
                lblrootMessage.Text = "保存成功！";
            }
        }

        protected void btnSaveValue_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtValueCode.Text.Trim()))
            {
                lblMessage2.Text = "代码不能为空！";
                return;
            }

            if (string.IsNullOrEmpty(txtValueName.Text.Trim()))
            {
                lblMessage2.Text = "名称不能为空！";
                return;
            }


            using (var edm = new GoldEntities())
            {
                var tmp = edm.CreateObject<DataDict>();
                tmp.Name = txtValueName.Text.Trim();
                tmp.Code = txtValueCode.Text.Trim();
                int x;
                if (!string.IsNullOrEmpty(txtValueOrder.Text.Trim()) &&
                    int.TryParse(txtValueOrder.Text.Trim(), out x))
                {
                    tmp.Order = x;
                }

                tmp.Category = TreeView1.SelectedNode.Value;
                tmp.Enabled = true;

                edm.AddToDataDict(tmp);
                edm.SaveChanges();

                GridView1.DataBind();

                txtValueCode.Text = "";
                txtValueName.Text = "";
                txtValueOrder.Text = "";
                lblMessage2.Text = "保存成功！";
            }
        }



        protected void EntityDataSource1_Updating(object sender, EntityDataSourceChangingEventArgs e)
        {

        }

        protected void EntityDataSource1_Updated(object sender, EntityDataSourceChangedEventArgs e)
        {
            if (e.Entity is DataDict)
            {
                DataDict model = e.Entity as DataDict;
                if (model != null)
                {
                    if (model.Code == "NCWebServiceURL") //如果配置项是用友接口参数，那么更改WebConfig中的WebServiceURL
                    {
                        string newURL = model.Name;
                        string oldURL = "";
                        string msg = "";
                        bool ret = SaveURL(newURL, out oldURL, out msg);
                        if (ret == true)
                        {
                            StringBuilder strMsg = new StringBuilder("更新成功！");
                            strMsg.Append("<br />");
                            strMsg.Append(" 原用友接口URL：");
                            strMsg.Append(oldURL);
                            strMsg.Append("<br />");
                            strMsg.Append(" 更新后用友接口URL：");
                            strMsg.Append(newURL);

                            ShowMessageBox(strMsg.ToString());
                        }
                        else
                        {
                            StringBuilder strMsg = new StringBuilder("更新失败！");
                            strMsg.Append(Environment.NewLine);
                            strMsg.Append(" 详细信息：");
                            strMsg.Append(msg);

                            ShowMessageBox(strMsg.ToString());
                        }
                    }
                }
            }
        }

        public bool SaveURL(string newURL, out string oldURL, out string msg)
        {
            oldURL = "";
            msg = "";
            try
            {

                string path = Server.MapPath("~/Web.config");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode xn = xmlDoc.SelectSingleNode(@"configuration/applicationSettings/Gold.Properties.Settings/setting/value");
                if (xn != null)
                {                    
                    oldURL = xn.InnerText;
                    xn.InnerText = newURL;
                    xmlDoc.Save(path);
                    return true;
                }
                else
                {
                    msg = "未能找到原URL值";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "SaveURL", ex);
                msg = Utility.LogHelper.GetExceptionMsg(ex);
                return false;
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)//如果该行不是数据行则不执行后续代码
                return;

            GridView gridView = sender as GridView;
            if (gridView.EditIndex != -1)//进入编辑模式后设置提示
            {
                GridViewRow currentRow = e.Row;
                if (currentRow == null)
                    return;
                //Label lblCode = currentRow.FindControl("lblCode") as Label;
                TextBox tbxRowCode = currentRow.FindControl("tbxRowCode") as TextBox;

                AjaxControlToolkit.ConfirmButtonExtender ConfirmButtonExtender_btnDelete = currentRow.FindControl("ConfirmButtonExtender_btnDelete") as AjaxControlToolkit.ConfirmButtonExtender;
                if (tbxRowCode != null && tbxRowCode.Text == "NCWebServiceURL")
                {
                    tbxRowCode.Enabled = false;
                    ConfirmButtonExtender_btnDelete.Enabled = true;
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

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

    }
}