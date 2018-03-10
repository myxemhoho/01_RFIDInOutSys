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
    public partial class StockCountingPlanView : System.Web.UI.Page
    {
        protected List<Users> actorList = new List<Users>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string EditID = "";
                if (Request.QueryString.Count > 0)
                {
                    EditID = Request.QueryString["EditID"].ToString().Trim();

                }
                ViewState["EditID"] = EditID;

                //using (GoldEntities context = new GoldEntities()) 
                //{
                //    var UserIDList=from r in context.StockCountingDetail where r.SCPCode==EditID select new{ r.ActorID});
                //}

                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"

                GridViewBind();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoList);//生成固定表头
        }

        void GridViewBind()
        {
            try
            {
                string scpCode = ViewState["EditID"].ToString();


                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    //查询基本信息
                    StockCountingPlan planModel = (from r in context.StockCountingPlan where r.SCPCode == scpCode select r).FirstOrDefault();
                    if (planModel != null)
                    {
                        lblSCPCodeShow.Text = planModel.SCPCode;
                        lblWHName.Text = planModel.WHName;
                        EnumData.SCPStatusEnum status = (EnumData.SCPStatusEnum)Enum.Parse(typeof(EnumData.SCPStatusEnum), planModel.SCPStatus.Value.ToString());
                        lblStatusShow.Text = EnumData.GetEnumDesc(status);
                        EnumData.SCPTypeEnum types = (EnumData.SCPTypeEnum)Enum.Parse(typeof(EnumData.SCPTypeEnum), planModel.SCPType.Value.ToString());
                        lblType.Text = EnumData.GetEnumDesc(types);
                        lblCreateTime.Text = planModel.CreateDate.ToString();

                        if (lblStatusShow.Text == EnumData.GetEnumDesc(EnumData.SCDetailStatusEnum.Complete))//结束盘点的按钮不可用
                        {
                            btnFinish.Enabled = false;
                            btnFinish.CssClass = "ButtonImageStyleEnableFalse";
                        }
                    }


                    var queryResult = (from r in context.StockCountingDetail
                                       where r.SCPCode == scpCode
                                       select r).ToList();

                    foreach (var d in queryResult)
                    {
                        DateTime FinishTime = DateTime.Now;//如果盘点未结束则计算盘点开始至当前时间段内的出入数量
                        if (d.Status == (int)EnumData.SCDetailStatusEnum.Complete) //如果盘点已经结束则计算盘点开始和结束之间的出入数量
                        {
                            FinishTime = d.CountingEndTime.Value;
                        }

                        //台账中ChangeType 1,3出，2,4入

                        //此层位此商品 盘点至今/结束的入库数量
                        double? sumIn = (from r in context.InventoryBook
                                         where r.WHCode == planModel.WHCode
                                         && r.CargoCode == d.CargoCode
                                         && r.BinCode == d.BinCode
                                             && (r.ChangeType == 2 || r.ChangeType == 4)
                                             && r.ChangeTime >= planModel.CreateDate
                                             && r.ChangeTime <= FinishTime
                                         select r.NumAdd).Sum();
                        //此层位此商品 盘点至今/结束的出库数量
                        double? sumOut = (from r in context.InventoryBook
                                          where r.WHCode == planModel.WHCode
                                            && r.CargoCode == d.CargoCode
                                            && r.BinCode == d.BinCode
                                              && (r.ChangeType == 1 || r.ChangeType == 3)
                                              && r.ChangeTime >= planModel.CreateDate
                                              && r.ChangeTime <= FinishTime
                                          select r.NumDel).Sum();

                        d.PeriodInNum = sumIn == null ? 0 : sumIn;
                        d.PeriodOutNum = sumOut == null ? 0 : sumOut;

                        if (d.NumActual != null)
                        {
                            d.NumDifference = d.NumActual - (d.NumPlan + d.PeriodInNum - d.PeriodOutNum);
                        }
                    }

                    string sortExpression = gv_CargoList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

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
                Label lblStatus = e.Row.FindControl("lblStatus") as Label;
                if (lblStatus != null)
                {
                    if (lblStatus.Text == EnumData.GetEnumDesc(EnumData.SCDetailStatusEnum.Uncompleted))//只要有未完成的项，结束盘点的按钮就不可用
                    {
                        btnFinish.Enabled = false;
                        btnFinish.CssClass = "ButtonImageStyleEnableFalse";
                        btnFinish.OnClientClick = "alert('该盘点计划单还有未完成的盘点项，请先完成盘点项后再点击此[结束盘点]按钮！');";//使用js显示消息提示文字
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
        public static string GetStatusName(string Status)
        {
            string name = "";
            EnumData.SCDetailStatusEnum enumNotKnown;
            if (Enum.TryParse<EnumData.SCDetailStatusEnum>(Status, out enumNotKnown))
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

        //
        protected void gv_CargoList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        //结束盘点任务
        protected void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                lblGridViewMsg.Text = "";


                using (GoldEntities context = new GoldEntities())
                {
                    string scpCode = lblSCPCodeShow.Text;
                    foreach (GridViewRow row in gv_CargoList.Rows)
                    {
                        /*
                         * lblSCPCode
                         * lblDetailRowNumber
                         lblNumPlan
                         * lblPeriodInNum
                         * lblPeriodOutNum
                         * lblNumActual
                         * lblNumDifference
                         */

                        Label lblDetailRowNumber = row.FindControl("lblDetailRowNumber") as Label;
                        //Label lblNumPlan = row.FindControl("lblNumPlan") as Label;
                        Label lblPeriodInNum = row.FindControl("lblPeriodInNum") as Label;
                        Label lblPeriodOutNum = row.FindControl("lblPeriodOutNum") as Label;
                        //Label lblNumActual = row.FindControl("lblNumActual") as Label;
                        Label lblNumDifference = row.FindControl("lblNumDifference") as Label;


                        string DetailRowNumber = lblDetailRowNumber.Text.Trim();
                        //double NumPlan=0;
                        double PeriodInNum = 0;
                        double PeriodOutNum = 0;
                        //double NumActual=0;
                        double NumDifference = 0;

                        //if(lblNumActual!=null&&lblNumActual.Text.Trim()!="")
                        //    NumActual=double.Parse(lblNumActual.Text);
                        if (lblPeriodInNum != null && lblPeriodInNum.Text.Trim() != "")
                            PeriodInNum = double.Parse(lblPeriodInNum.Text);
                        if (lblPeriodOutNum != null && lblPeriodOutNum.Text.Trim() != "")
                            PeriodOutNum = double.Parse(lblPeriodOutNum.Text);
                        if (lblNumDifference != null && lblNumDifference.Text.Trim() != "")
                            NumDifference = double.Parse(lblNumDifference.Text);

                        StockCountingDetail updateModel = (from r in context.StockCountingDetail where r.SCPCode == scpCode && r.DetailRowNumber == DetailRowNumber select r).FirstOrDefault();

                        updateModel.PeriodInNum = PeriodInNum;
                        updateModel.PeriodOutNum = PeriodOutNum;
                        updateModel.NumDifference = NumDifference;

                    }

                    StockCountingPlan planModel = (from r in context.StockCountingPlan where r.SCPCode == scpCode select r).FirstOrDefault();
                    planModel.SCPStatus = (int)EnumData.SCPStatusEnum.Finished;
                    if (Session["UserInfo"] != null)
                    {
                        Users LoginUser = Session["UserInfo"] as Users;
                        planModel.FinishPersonID = LoginUser.UserId;
                        planModel.FinishPersonName = LoginUser.UserName;
                    }
                    planModel.FinishDate = DateTime.Now;

                    int AffectRowsCount = context.SaveChanges();
                    lblGridViewMsg.Text = "盘点计划单[" + scpCode + "]成功结束![影响行数" + AffectRowsCount.ToString() + "]";

                    btnFinish.Enabled = false;
                    btnFinish.CssClass = "ButtonImageStyleEnableFalse";

                    GridViewBind();//删除完成后重新绑定盘点计划单
                }

            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "操作失败！[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
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