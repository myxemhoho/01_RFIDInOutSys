using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.NCInvoke;
using System.Data;

namespace Gold.BaseInfoSetting
{
    public partial class SpecList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack) 
            {
                gv_SpecList.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_SpecList);//生成固定表头
        }


        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid == false)
                return;

            lblAddMsg.Text = "";
            try
            {
                string newName = tbxNewSpecName.Text.Trim();
                string newComment = tbxNewComment.Text.Trim();
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var sameResult = (from r in context.Specifications where r.SpecName.Equals(newName) select r).FirstOrDefault();
                    if (sameResult != null)
                    {
                        lblAddMsg.Text = "系统中已存在名为【" + newName + "】的规格信息！请重填！";
                        return;
                    }

                    DAL.Specifications newSpecObj = new DAL.Specifications();
                    newSpecObj.SpecName = newName;
                    newSpecObj.Comment = newComment;
                    context.Specifications.AddObject(newSpecObj);
                    context.SaveChanges();
                    lblAddMsg.Text = "保存成功";

                    gv_SpecList.DataBind();

                }
            }
            catch (Exception ex)
            {
                lblAddMsg.Text = "保存失败！详细信息:" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        /// <summary>
        /// 自定义验证控件的服务端事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator validator = source as CustomValidator;

            if ((validator.Equals(CustomValidator_tbxNewSpecName) || validator.ID == "CustomValidator_tbxRowSpecName") && args.Value.ToString().Trim().Length > 50)
            {
                validator.ErrorMessage = "规格名称长度不能超过50个字符";
                args.IsValid = false;
                return;

            }
            else
            {
                args.IsValid = true;
            }
            if ((validator.Equals(CustomValidator_tbxNewComment) || validator.ID == "CustomValidator_tbxRowComment") && args.Value.ToString().Trim().Length > 100)
            {
                validator.ErrorMessage = "规格备注长度不能超过100个字符";
                args.IsValid = false;
                return;
            }
            else
            {
                args.IsValid = true;
            }


        }

        /// <summary>
        /// 清空新增栏控件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbxNewSpecName.Text = "";
            tbxNewComment.Text = "";
            lblAddMsg.Text = "";
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView theGrid = sender as GridView;   // refer to the GridView
            int newPageIndex = 0;

            if (-2 == e.NewPageIndex)
            { // when click the "GO" Button

                TextBox txtNewPageIndex = null;

                GridViewRow pagerRow = theGrid.BottomPagerRow; //GridView较DataGrid提供了更多的API，获取分页块可以使用BottomPagerRow 或者TopPagerRow，当然还增加了HeaderRow和FooterRow


                if (null != pagerRow)
                {
                    txtNewPageIndex = pagerRow.FindControl("txtNewPageIndex") as TextBox;    // refer to the TextBox with the NewPageIndex value
                }

                if (null != txtNewPageIndex)
                {
                    newPageIndex = int.Parse(txtNewPageIndex.Text) - 1; // get the NewPageIndex
                }
            }
            else
            {   // when click the first, last, previous and next Button
                newPageIndex = e.NewPageIndex;
            }

            // check to prevent form the NewPageIndex out of the range
            newPageIndex = newPageIndex < 0 ? 0 : newPageIndex;
            newPageIndex = newPageIndex >= theGrid.PageCount ? theGrid.PageCount - 1 : newPageIndex;

            // specify the NewPageIndex
            theGrid.PageIndex = newPageIndex;

            // rebind the control
            // in this case of retrieving the data using the xxxDataSoucr control,
            // just do nothing, because the asp.net engine binds the data automatically

            gv_SpecList.DataBind();
        }

        /// <summary>
        /// 行命令触发前先清空界面消息提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
                lblGridViewMsg.Text = "";

        }

        /// <summary>
        /// 更新完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.AffectedRows >= 0)
                lblGridViewMsg.Text = "更新成功！";
            else
                lblGridViewMsg.Text = "更新失败[" + e.Exception.InnerException.Message.ToString() + "]";
        }

        /// <summary>
        /// 行删除完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.AffectedRows > 0)
                lblGridViewMsg.Text = "删除成功！";
            else
                lblGridViewMsg.Text = "删除失败[" + e.Exception.InnerException.Message.ToString() + "]";
        }

        /// <summary>
        /// 更新时检测是否重名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string newSpecName = e.NewValues["SpecName"].ToString().Trim();
            string oldSpecName = e.OldValues["SpecName"].ToString().Trim();

            using (DAL.GoldEntities context = new DAL.GoldEntities())
            {
                var selectResult = (from r in context.Specifications where (r.SpecName!=oldSpecName&& r.SpecName == newSpecName) select r).ToList();
                if (selectResult != null && selectResult.Count > 0)
                {
                    lblGridViewMsg.Text = "更新失败,系统中已经存在名为[" + newSpecName + "]的规格信息，请重新填写！";
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SpecList_DataBound(object sender, EventArgs e)
        {
            string sjs = "GetResultFromServer();";
            ScriptManager.RegisterClientScriptBlock(this.gv_SpecList, this.GetType(), "", sjs, true);

            //设置自定义分页的显示状态
            GridView theGrid = sender as GridView;

            //if (theGrid.Rows.Count >= theGrid.PageSize)
            //{

                int newPageIndex = theGrid.PageIndex;

                GridViewRow pagerRowShow = theGrid.BottomPagerRow;
                if (pagerRowShow == null)
                    return;
                LinkButton btnFirst = pagerRowShow.FindControl("btnFirst") as LinkButton;
                LinkButton btnPrev = pagerRowShow.FindControl("btnPrev") as LinkButton;
                LinkButton btnNext = pagerRowShow.FindControl("btnNext") as LinkButton;
                LinkButton btnLast = pagerRowShow.FindControl("btnLast") as LinkButton;


                if (newPageIndex >= theGrid.PageCount - 1)
                {
                    btnLast.Enabled = false;
                    btnNext.Enabled = false;
                }
                else
                {
                    btnLast.Enabled = true;
                    btnNext.Enabled = true;
                }

                if (newPageIndex <= 0)
                {
                    btnFirst.Enabled = false;
                    btnPrev.Enabled = false;
                }
                else
                {
                    btnFirst.Enabled = true;
                    btnPrev.Enabled = true;
                }
            //}

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        protected void tbxNewSpecName_TextChanged(object sender, EventArgs e)
        {
            if (tbxNewSpecName.Text.Trim()=="") 
            {
                lblAddMsg.Text = "";
            }
        }

        #region 固定GridView表头

        /// <summary>
        /// 生成固定表头
        /// </summary>
        /// <param name="gv_SaleAllocationList"></param>
        void DisplayFixHeader(GridView gv_SaleAllocationList)
        {
            Table table = new Table();
            table.CssClass = "GridViewFixHeader_Table";
            table.Width = new Unit(gv_SaleAllocationList.Width.Value);

            TableRow tr = new TableRow();
            tr.CssClass = "GridViewHeaderStyle";
            int i = 0;
            foreach (DataControlField cell in gv_SaleAllocationList.Columns)
            {
                if (cell.Visible == true)
                {
                    TableCell td = new TableCell();
                    td.Style.Add("text-align", "left");
                    if (cell.ItemStyle.Width.Value != (double)0)
                    {
                        td.Width = (int)Math.Round(cell.ItemStyle.Width.Value);
                    }

                    if (!string.IsNullOrEmpty(cell.SortExpression))
                    {
                        LinkButton lbtn = new LinkButton();
                        lbtn.CommandArgument = cell.SortExpression;
                        lbtn.Text = cell.HeaderText;
                        lbtn.Click += new EventHandler(LinkButtonHeader_Click);
                        lbtn.ID = "LinkButton" + (i++).ToString();
                        //lbtn.OnClientClick = "divWait('divWait');";//js提示
                        lbtn.Attributes.Add("onclick", "showWaitDiv('divWait');");//js提示

                        td.Controls.Add(lbtn);


                    }
                    else
                    {
                        td.Text = cell.HeaderText;
                    }

                    tr.Cells.Add(td);
                }
            }
            table.Rows.Add(tr);

            divHeader.Controls.Clear();
            divHeader.Controls.Add(table);

            //gv_SaleAllocationList.ShowHeader = false;//这里设置无效，要在标记中设置

        }

        //自定义排序
        protected void LinkButtonHeader_Click(object sender, EventArgs e)
        {
            LinkButton lBtn = sender as LinkButton;
            if (lBtn != null)
            {
                string sortExpression = lBtn.CommandArgument;//获取排序字段，进行排序
                SortDirection sortDirection = gv_SpecList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_SpecList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_SpecList.Sort(sortExpression, newSortDirection);
                gv_SpecList.DataBind();//由于是绑定的数据源控件，所以需要重新进行绑定
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

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

                string typeArgs = "Specification";
                string conditionArgs = string.Empty;
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
                gv_SpecList.DataBind();
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