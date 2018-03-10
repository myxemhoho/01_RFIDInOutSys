using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.Order
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        //页面加载事件
        protected void Page_Load(object sender, EventArgs e)
        {
            string orderCode = Request.QueryString["orderCode"];
            if (string.IsNullOrEmpty(orderCode))
            {
                Server.Transfer("error.aspx?errorInfo=缺少参数:orderCode");
            }
            else
            {
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.SalesOrder.Where(o => o.OrderCode == orderCode).ToList();
                    if (tmp == null || tmp.Count == 0)
                        Server.Transfer("error.aspx?errorInfo=无此采购订单记录。订单编码=" + orderCode);
                    else
                    {
                        //绑定数据
                        FormView1.DataSource = tmp;
                        FormView1.DataBind();

                        GridView1.DataSource = tmp[0].SalesOrderDetail.OrderBy(o => o.CargoName);
                        GridView1.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
                        GridView1.DataBind();

                        lblEditorID.Text = tmp[0].EditorID;
                        lblEditorName.Text = tmp[0].EditorName;
                    }
                }
            }
            this.lblTitle.Text = "销售订单：" + orderCode;

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.GridView1);//生成固定表头
        }

        //生成出库单
        protected void btnGenStockOut_Click(object sender, EventArgs e)
        {
            string orderCode = Request.QueryString["orderCode"];
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == orderCode);
                if (tmp != null)
                {
                    //行项目=0的判断
                    if (tmp.SalesOrderDetail.Count == 0)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "此订单没有有效商品，不能生成出库单！");
                        return;
                    }

                    //判断是否有人正在编辑此订单
                    if (tmp.EditStatus == null)//如果为空，则设置状态为0
                    {
                        tmp.EditStatus = 0;
                    }

                    if (tmp.EditStatus == 0)//0-无人编辑，则保存当前用户信息
                    {
                        if (Session["UserInfo"] != null)
                        {
                            Users userInfo = (Users)Session["UserInfo"];
                            tmp.EditorID = userInfo.UserId;
                            tmp.EditorName = userInfo.UserName;
                        }
                        tmp.EditTime = System.DateTime.Now;
                        tmp.EditStatus = 1;//设置当前订单状态为正在编辑

                        //保存数据
                        edm.SalesOrder.ApplyCurrentValues(tmp);
                        edm.SaveChanges();
                    }
                    else if (tmp.EditStatus == 1 && tmp.OrderStatus != 0)//正在编辑 且订单状态不是已完成
                    {
                        //判断该数据库编辑人是否是当前用户
                        if (Session["UserInfo"] != null)
                        {
                            Users userInfo = (Users)Session["UserInfo"];

                            if (tmp.EditorID != userInfo.UserId)
                            {
                                lblInOrOut.Text = "出库单";
                                programmaticModalPopup.Show();
                                return;
                            }  
                        }
                    }
                   
                    if (tmp.OrderStatus == 0)//0-全部完成；1-部分完成；2-初始态；3-全部已转；4-部分已转
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "此订单已完成出库，不能再生成出库单！");
                        return;
                    }
                    else if (tmp.OrderStatus == 3)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "此订单已全部生成出库单，不能再次生成出库单！");
                        return;
                    }
                }
            }

            string sortDiEx = string.Empty;
            if (ViewState["viewSortDirectionExpression"] != null)
            {
                sortDiEx = ViewState["viewSortDirectionExpression"].ToString();
            }
            Response.Redirect("~/StockMove/StockOutReg.aspx?sourceCode=" + Request.QueryString["orderCode"] + "&sourceType=salesorder" + "&sortExDi=" + sortDiEx);
        }

        //返回订单列表
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("SalesOrderMgr.aspx");
        }

        //确定按钮
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            try
            {
                string orderCode = Request.QueryString["orderCode"];
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == orderCode);
                    if (tmp != null)
                    {
                        if (Session["UserInfo"] != null)
                        {
                            Users userInfo = (Users)Session["UserInfo"];
                            tmp.EditorID = userInfo.UserId;
                            tmp.EditorName = userInfo.UserName;
                        }
                        tmp.EditTime = System.DateTime.Now;
                        tmp.EditStatus = 1;//设置当前订单状态为正在编辑

                        //保存数据
                        edm.SalesOrder.ApplyCurrentValues(tmp);
                        edm.SaveChanges();
                    }
                }

                string sortDiEx = string.Empty;
                if (ViewState["viewSortDirectionExpression"] != null)
                {
                    sortDiEx = ViewState["viewSortDirectionExpression"].ToString();
                }
                Response.Redirect("~/StockMove/StockOutReg.aspx?sourceCode=" + Request.QueryString["orderCode"] + "&sourceType=salesorder" + "&sortExDi=" + sortDiEx);
                this.programmaticModalPopup.Hide();
                //return;
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "跳转界面发生异常！" + msg);
            }
        }

        //取消
        protected void hideClose_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup.Hide();
        }      

        //由状态代码获取状态解释
        protected string GetOrderStatus(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "OrderStatus" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }

        //排序
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(GridView1.Attributes["sortExpression"]) && "ASC".Equals(GridView1.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }

            try
            {
                string orderCode = Request.QueryString["orderCode"];
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.SalesOrderDetail.Where(o => o.OrderCode == orderCode).ToList();
                    if (tmp == null || tmp.Count == 0)
                        DAL.CommonConvert.ShowMessageBox(this.Page, "无此销售订单详细记录。订单编码=" + orderCode);
                    else
                    {
                        if (sortDirection == "ASC")
                        {
                            tmp = tmp.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                        }
                        else if (sortDirection == "DESC")
                        {
                            tmp = tmp.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                        }

                        GridView1.DataSource = tmp;
                        GridView1.Attributes.Add("sortExpression", sortExpression);
                        GridView1.Attributes.Add("sortDirection", sortDirection);
                        GridView1.DataBind();
                    }
                }
                ViewState["viewSortDirectionExpression"] = sortDirection + "," + sortExpression;//保存sortExpression和sortDirection
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "排序出错！" + msg);
            }
        }

        //设置生成入库单按钮的显示或隐藏
        protected void GridView1_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblWHName = e.Row.FindControl("lblWHName") as Label;
                if (lblWHName != null)
                {
                    if (lblWHName.Text.Trim() != string.Empty)
                    {
                        string defaultWHCode = string.Empty;
                        string defaultWHName = string.Empty;
                        //从配置文件读取默认的仓库--地王26库
                        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                        {
                            defaultWHCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                        }
                        if (defaultWHCode == string.Empty)
                        {
                            defaultWHCode = "20101";
                        }                        

                        using (var edm = new GoldEntities())
                        {
                            var tmpName = edm.WareHouse.Where(o => o.WHCode == defaultWHCode).Select(o => o.WHName).Distinct().ToList();
                            if (tmpName != null && tmpName.Count > 0)
                            {
                                defaultWHName = tmpName[0];
                            }
                        }
                        if (lblWHName.Text.Trim() != defaultWHName)
                        {
                            btnGenStockOut.Visible = false;
                        }
                        else
                        {
                            btnGenStockOut.Visible = true;
                        }
                    }
                }
            }
        }

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
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


                GridView1.DataBind();//根据新页索引重新绑定数据

            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！ gv_SaleAllocationList_PageIndexChanging" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！ gv_SaleAllocationList_DataBound" + Utility.LogHelper.GetExceptionMsg(ex);
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        #endregion

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
                SortDirection sortDirection = GridView1.SortDirection;
                SortDirection newSortDirection;
                switch (GridView1.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                GridView1.Sort(sortExpression, newSortDirection);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}