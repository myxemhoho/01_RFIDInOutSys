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
    public partial class WareHouseList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //要在GridView标记中加入自定义的标记sortExpression和sortDirection
                GridViewBind();
            }
        }

        void GridViewBind()
        {
            try
            {
                string whCode = tbxWHCode.Text.Trim();
                string whName = tbxWHName.Text.Trim();

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var queryResult = (from r in context.WareHouse
                                       where r.WHCode.Contains(whCode) && r.WHName.Contains(whName)
                                       select r).ToList();

                    string sortExpression = gv_WareHouseList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_WareHouseList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    gv_WareHouseList.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_WareHouseList.DataSource = queryResult;
                    gv_WareHouseList.DataBind();
                }
            }
            catch (Exception ex) 
            {
                //lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                ShowMessageBox("查询出现异常！", ex);
            }
        }

        protected void gv_WareHouseList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView theGrid = sender as GridView;   // refer to the GridView
            int newPageIndex = 0;

            if (-2 == e.NewPageIndex)
            { // when click the "GO" Button

                TextBox txtNewPageIndex = null;
                //GridViewRow pagerRow = theGrid.Controls[0].Controls[theGrid.Controls[0].Controls.Count - 1] as GridViewRow; // refer to PagerTemplate
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

            GridViewBind();
        }


        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GridViewBind();
        }

        protected void gv_WareHouseList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_WareHouseList.Attributes["sortExpression"]) && "ASC".Equals(gv_WareHouseList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_WareHouseList.Attributes.Add("sortExpression", sortExpression);
            gv_WareHouseList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void btnShowAddPage_Click(object sender, EventArgs e)
        {
            //QueryString中EditType 1为新增 2为编辑
            //QueryString中EditID 0表示不编辑
            string EditID = "0";
            System.Text.StringBuilder url = new System.Text.StringBuilder("~/BaseInfoSetting/WareHouseEdit.aspx?EditType=1");
            url.Append("&EditID=");
            url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=1";
            Response.Redirect(url.ToString());
        }

        protected void btnShowEditPage_Click(object sender, EventArgs e)
        {   //QueryString中EditType 1为新增 2为编辑
            //QueryString中EditID 0表示不编辑 1表示编辑
            string EditID = "0";

            lblCheckMsg.Text = "";
            lblGridViewMsg.Text = "";

            List<string> checkedItem = GetCheckItemID();
            if (checkedItem.Count == 0)
            {
                //lblCheckMsg.Text = "请在列表中选择一条记录进行编辑！请重选！";
                ShowMessageBox("请在列表中选择一条记录进行编辑！请重选！");
                return;
            }
            else if (checkedItem.Count > 1)
            {
                //lblCheckMsg.Text = "只能选择列表中的一条记录进行编辑！请重选！";
                ShowMessageBox("只能选择列表中的一条记录进行编辑！请重选！");
                return;
            }
            else
            {
                EditID = checkedItem[0].Trim();
            }

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2");
            url.Append("&EditID=");
            url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
            Response.Redirect(url.ToString());
        }

        private List<string> GetCheckItemID()
        {
            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < gv_WareHouseList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_WareHouseList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = (currentRow.FindControl("lblWHCode") as Label).Text.Trim();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;
                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                lblCheckMsg.Text = "";
                lblGridViewMsg.Text = "";
                List<string> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    //lblCheckMsg.Text = "请先选中待删除的项";
                    ShowMessageBox("请先选中待删除的项");
                    return;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    foreach (string delCode in checkedDelList)
                    {
                        DAL.WareHouse delObject = (from r in context.WareHouse where r.WHCode.Equals(delCode) select r).FirstOrDefault();

                        context.DeleteObject(delObject);
                    }
                    int delRow = context.SaveChanges();
                    if (delRow > 0)
                    {
                        //lblCheckMsg.Text = "删除成功！[已删除" + delRow.ToString() + "项]";
                        ShowMessageBox("删除成功！[已删除" + delRow.ToString() + "项]");
                    }
                    else
                    {
                        //lblCheckMsg.Text = "删除失败！";
                        ShowMessageBox("删除失败！");
                    }

                    GridViewBind();//删除后重新绑定数据
                }
            }
            catch (Exception ex) 
            {
                //lblCheckMsg.Text = "删除出现异常！";
                //lblGridViewMsg.Text = Utility.LogHelper.GetExceptionMsg(ex);
                ShowMessageBox("删除失败！",ex);
            }
        }

        protected void gv_WareHouseList_DataBound(object sender, EventArgs e)
        {
            //设置自定义分页的显示状态
            GridView theGrid = sender as GridView;
            //if (theGrid.Rows.Count > theGrid.PageSize)
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

                string typeArgs = "WareHouse";
                string conditionArgs = string.Empty;
                DataTable dt = null;
                string queryMsg = "";
                string saveMsg = "";

                BasicInfoInvoke BasicInfoInvokeObj = BasicInfoInvokeFactory.CreateInstance(typeArgs, conditionArgs);

                if (BasicInfoInvokeObj.GetNCDataJoinRFID(out dt, out queryMsg) == false)
                {
                    ShowMessageBox("查询用友系统仓库信息失败！详细信息：" + queryMsg);
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