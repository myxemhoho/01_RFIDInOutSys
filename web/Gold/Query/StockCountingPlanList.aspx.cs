using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;

using System.Text;

namespace Gold.Query
{
    public partial class StockCountingPlanList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                txtStartTime.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtStartTime.Attributes.Add("readonly", "true");
                txtEndTime.Attributes.Add("readonly", "true");

                LoadNewPanelDropDownList();


                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"

                btnQuery_Click(sender, e);
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoList);//生成固定表头
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            lblGridViewMsg.Text = "";

            GridViewBind();
        }

        /// <summary>
        /// 加载dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定盘点计划单状态
                List<NameValueModel> ListStatus = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.SCPStatusEnum));
                DropDownList_SCPStatus.Items.Clear();
                DropDownList_SCPStatus.DataTextField = "Name";
                DropDownList_SCPStatus.DataValueField = "Value";
                DropDownList_SCPStatus.DataSource = ListStatus;
                DropDownList_SCPStatus.DataBind();
                DropDownList_SCPStatus.Items.Insert(0, new ListItem("", ""));

                //绑定盘点计划单状态
                List<NameValueModel> ListType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.SCPTypeEnum));
                DropDownList_Type.Items.Clear();
                DropDownList_Type.DataTextField = "Name";
                DropDownList_Type.DataValueField = "Value";
                DropDownList_Type.DataSource = ListType;
                DropDownList_Type.DataBind();
                DropDownList_Type.Items.Insert(0, new ListItem("", ""));

                //绑定仓库
                var result = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                DropDownList_WHCode.Items.Clear();
                DropDownList_WHCode.DataTextField = "WHName";
                DropDownList_WHCode.DataValueField = "WHCode";
                DropDownList_WHCode.DataSource = result;
                DropDownList_WHCode.DataBind();
                DropDownList_WHCode.Items.Insert(0, new ListItem("", ""));
            }
        }

        void GridViewBind()
        {
            try
            {
                string scpCode = tbxSCPCode.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                string scpStatus = DropDownList_SCPStatus.SelectedItem.Value.ToString();
                int scpStatusInt = -1;
                if (int.TryParse(scpStatus, out scpStatusInt) == false)
                    scpStatusInt = -1;
                string scpType = DropDownList_Type.SelectedItem.Value.ToString();
                int scpTypeInt = -1;
                if (int.TryParse(scpType, out scpTypeInt) == false)
                    scpTypeInt = -1;

                DateTime startDate = DateTime.Parse(txtStartTime.Text);
                DateTime endDate = DateTime.Parse(txtEndTime.Text);
                DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var queryResult = (from r in context.StockCountingPlan
                                       where r.SCPCode.Contains(scpCode)
                                       && r.WHCode.Contains(whCode)
                                       && (scpStatusInt == -1 ? true : r.SCPStatus.Value.Equals(scpStatusInt))
                                       && (scpTypeInt==-1?true:r.SCPType.Value.Equals(scpTypeInt))
                                       && (r.CreateDate.Value >= startTime && r.CreateDate.Value <= endTime)
                                       select r).ToList();

                    //组合盘点层位
                    List<string> allSCPCodeList = queryResult.Select(r => r.SCPCode).ToList<string>();
                    List<StockCountingDetail> allDetail = (from r in context.StockCountingDetail where allSCPCodeList.Contains(r.SCPCode) select r).ToList<StockCountingDetail>();
                                        
                    foreach (var temp in queryResult)
                    {
                        string keyCode = temp.SCPCode;
                        StringBuilder strDetailBinCode = new StringBuilder();
                        var detailList = (from r in allDetail orderby r.SCPCode orderby r.BinCode ascending where r.SCPCode == keyCode select r);
                        foreach (var d in detailList)
                        {
                            if (strDetailBinCode.ToString().Contains(d.BinCode) == false)
                            {
                                if (strDetailBinCode.Length > 0)
                                    strDetailBinCode.Append(",");
                                strDetailBinCode.Append(d.BinCode);
                            }
                        }
                        temp.Reserve1 = strDetailBinCode.ToString();
                    }

                    string sortExpression = gv_CargoList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    gv_CargoList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_CargoList.DataSource = queryResult;
                    gv_CargoList.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        protected void gv_CargoList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

        protected void gv_CargoList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_CargoList.Attributes["sortExpression"]) && "ASC".Equals(gv_CargoList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_CargoList.Attributes.Add("sortExpression", sortExpression);
            gv_CargoList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void gv_CargoList_DataBound(object sender, EventArgs e)
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



        //设置编辑按钮可用性
        protected void gv_CargoList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Label lblRowBinType = e.Row.FindControl("lblRowBinType") as Label;
                Label lblSCPStatus = e.Row.FindControl("lblSCPStatus") as Label;
                LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
                LinkButton btnDel = e.Row.FindControl("btnDel") as LinkButton;
                if (lblSCPStatus != null && btnEdit != null && btnDel != null)
                {
                    if (lblSCPStatus.Text != EnumData.GetEnumDesc(EnumData.SCPStatusEnum.Initial))//只有初始态才可编辑
                    {
                        btnEdit.Enabled = false;
                        btnEdit.OnClientClick = "alert('该盘点计划单不是初始状态，不能进行编辑！');";//使用js显示消息提示文字
                    }

                    if (lblSCPStatus.Text == EnumData.GetEnumDesc(EnumData.SCPStatusEnum.Executing))//执行中的单据不可删除
                    {
                        btnDel.Enabled = false;
                        btnDel.OnClientClick = "alert('该盘点计划单正在执行中，不能删除！');";//使用js显示消息提示文字
                    }
                }
            }
        }

        protected void btnGoToPrintAndExportPage_Click(object sender, EventArgs e)
        {
            //string displayPattern = "";//RadioButton_ByCargo.Checked ? "ByCargo" : "ByWHCode";
            //string cargoCode = tbxCargoCode.Text.Trim();
            //string cargoName = tbxCargoName.Text.Trim();
            //string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
            //string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
            //string whName = DropDownList_WHCode.SelectedItem.Text.Trim();
            //string whCode = DropDownList_WHCode.SelectedItem.Value.Trim();
            //string startDate = txtStartTime.Text;
            //string endDate = txtEndTime.Text;
            //string sortExpression = gv_CargoList.Attributes["sortExpression"];
            //string sortDirection = gv_CargoList.Attributes["sortDirection"];

            //System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/InOutCollectListRpt.aspx?");
            //url.Append("displayPattern=");
            //url.Append(displayPattern);
            //url.Append("&cargoCode=");
            //url.Append(cargoCode);
            //url.Append("&cargoName=");
            //url.Append(cargoName);
            //url.Append("&modelName=");
            //url.Append(modelName);
            //url.Append("&specName=");
            //url.Append(specName);
            //url.Append("&whName=");
            //url.Append(whName);
            //url.Append("&whCode=");
            //url.Append(whCode);
            //url.Append("&startDate=");
            //url.Append(startDate);
            //url.Append("&endDate=");
            //url.Append(endDate);
            //url.Append("&sortExpression=");
            //url.Append(sortExpression);
            //url.Append("&sortDirection=");
            //url.Append(sortDirection);
            //Response.Redirect(url.ToString());
        }

        //根据状态字段获取状态类型
        public static string GetSCPStatusName(string scpStatus)
        {
            string name = "";
            EnumData.SCPStatusEnum enumNotKnown;
            if (Enum.TryParse<EnumData.SCPStatusEnum>(scpStatus, out enumNotKnown))
            {
                name = EnumData.GetEnumDesc(enumNotKnown);
            }
            return name;
        }

        protected void btnShowAddPage_Click(object sender, EventArgs e)
        {
            ////QueryString中EditType 1为新增 2为编辑
            ////QueryString中EditID 0表示不编辑
            //string EditID = "0";
            //System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StockCountingPlanEdit.aspx?EditType=1");
            //url.Append("&EditID=");
            //url.Append(EditID);
            //Response.Redirect(url.ToString());
        }

        protected void btnShowEditPage_Click(object sender, EventArgs e)
        {
            //   //QueryString中EditType 1为新增 2为编辑
            //    //QueryString中EditID 0表示不编辑 1表示编辑
            //    string EditID = "0";

            //    lblCheckMsg.Text = "";
            //    lblGridViewMsg.Text = "";

            //    List<string> checkedItem = GetCheckItemID();
            //    if (checkedItem.Count == 0)
            //    {
            //        lblCheckMsg.Text = "请在列表中选择一条记录进行编辑！请重选！";
            //        return;
            //    }
            //    else if (checkedItem.Count > 1)
            //    {
            //        lblCheckMsg.Text = "只能选择列表中的一条记录进行编辑！请重选！";
            //        return;
            //    }
            //    else
            //    {
            //        EditID = checkedItem[0].Trim();
            //    }

            //    System.Text.StringBuilder url = new System.Text.StringBuilder("~/BaseInfoSetting/CargoEdit.aspx?EditType=2");
            //    url.Append("&EditID=");
            //    url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
            //    Response.Redirect(url.ToString());
        }

        //查看，编辑，删除
        protected void gv_CargoList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView senderGrid = sender as GridView;
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
            {
                lblGridViewMsg.Text = "";
                //ClearTipMsgLabel();//清除提示信息
            }
            else if (e.CommandName == "ViewDetail")
            {
                GridViewRow drv = ((GridViewRow)(((LinkButton)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                Label lblSCPCode = drv == null ? null : drv.FindControl("lblSCPCode") as Label;
                
                string EditID = lblSCPCode.Text;
                System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StockCountingPlanView.aspx?");
                url.Append("EditID=");
                url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
                Response.Redirect(url.ToString());

            }
            else if (e.CommandName == "MyEdit")
            {
                GridViewRow drv = ((GridViewRow)(((LinkButton)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                Label lblSCPCode = drv == null ? null : drv.FindControl("lblSCPCode") as Label;
                //QueryString中EditType 1为新增 2为编辑
                //QueryString中EditID 0表示不编辑 1表示编辑
                string EditID = lblSCPCode.Text;
                lblGridViewMsg.Text = "";

                System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StockCountingPlanEdit.aspx?EditType=2");
                url.Append("&EditID=");
                url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
                Response.Redirect(url.ToString());
            }
            else if (e.CommandName == "MyDelete")
            {
                try
                {
                    GridViewRow drv = ((GridViewRow)(((LinkButton)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                    Label lblSCPCode = drv == null ? null : drv.FindControl("lblSCPCode") as Label;
                    //QueryString中EditType 1为新增 2为编辑
                    //QueryString中EditID 0表示不编辑 1表示编辑
                    string EditID = lblSCPCode.Text;


                    lblGridViewMsg.Text = "";

                    //更新时，先删除全部明细表的行，再新增所有本次选中的层位对应的明细行
                    using (GoldEntities context = new GoldEntities())
                    {
                        //查出待删除的计划单
                        StockCountingPlan delModel = (from r in context.StockCountingPlan where r.SCPCode == EditID select r).FirstOrDefault();
                        //查出待删除的明细单
                        List<StockCountingDetail> delDetailList = (from r in context.StockCountingDetail where r.SCPCode == EditID select r).ToList<StockCountingDetail>();

                        //删除明细行
                        foreach (StockCountingDetail d in delDetailList)
                        {
                            context.StockCountingDetail.DeleteObject(d);
                        }
                        //删除计划单
                        context.StockCountingPlan.DeleteObject(delModel);

                        int AffectRowsCount = context.SaveChanges();
                        lblGridViewMsg.Text = "盘点计划单[" + EditID + "]删除成功![影响行数" + AffectRowsCount.ToString() + "]";

                        GridViewBind();//删除完成后重新绑定盘点计划单
                    }

                }
                catch (Exception ex)
                {
                    lblGridViewMsg.Text = "删除失败！[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
                }


                //lblGridViewMsg.Text = "已删除";
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

                    if (string.IsNullOrEmpty(cell.SortExpression) && cell.HeaderText == "*")
                    {

                        CheckBox chk = new CheckBox();
                        chk.ID = "chkAllCheck" + (i++).ToString();
                        chk.Text = "全选";
                        //chk.AutoPostBack = true;
                        //chk.CheckedChanged += new EventHandler(CheckBox_CheckAll_CheckedChanged);
                        chk.Attributes.Add("onclick", "selectAllCheckBox('" + gv_SaleAllocationList.ClientID + "',this);");

                        td.Controls.Add(chk);

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
                SortDirection sortDirection = gv_CargoList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_CargoList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_CargoList.Sort(sortExpression, newSortDirection);
                
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}