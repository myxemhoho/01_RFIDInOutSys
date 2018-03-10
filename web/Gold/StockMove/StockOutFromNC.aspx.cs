using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data;
using Gold.NCInvoke;

namespace Gold.StockMove
{
    public partial class StockOutFromNC : System.Web.UI.Page
    {
        //页面加载事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //设置开始时间和结束时间的默认值
                txtStartTime.Text = System.DateTime.Now.ToShortDateString();
                txtEndTime.Text = System.DateTime.Now.ToShortDateString();

                List<StockOut> lstStock = null;
                OrderInfoInvoke orderInfoInvokeObj = new OrderInfoInvoke();
                QueryData(out orderInfoInvokeObj, out lstStock);
                if (lstStock != null)
                {
                    //绑定数据到界面的Gridview
                    GridView1.DataSource = lstStock;
                    GridView1.DataBind();
                }
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.GridView1);//生成固定表头
        }

        //查询数据
        private void QueryData(out OrderInfoInvoke orderInfoInvokeObj, out List<StockOut> lstStock)
        {
            string warehouse = string.Empty;//           
            //从配置文件读取默认的仓库--地王26库
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
            {
                warehouse = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
            }
            else
            {
                warehouse = "20101";
            }
            string billType = "OtherStockOutBill";//单据类型

            string startTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", System.DateTime.Now);//开始时间
            if (txtStartTime.Text.Trim() != string.Empty)
            {
                DateTime dateEnd = Convert.ToDateTime(txtStartTime.Text.Trim());
                DateTime endT = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 00, 00, 00);//获取开始时间，精确到秒 
                startTime = endT.ToString();
            }

            string endTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", System.DateTime.Now);//结束时间
            if (txtEndTime.Text.Trim() != string.Empty)
            {
                DateTime dateEnd = Convert.ToDateTime(txtEndTime.Text.Trim());
                DateTime endT = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);//获取截止时间，精确到秒 
                endTime = endT.ToString();
            }

            int maxCount = 500;//最大查询条数

            //是否已导入
            string isAlreadyStatus = string.Empty;
            if (ddlisAlreadyStatus.SelectedIndex != 0)
            {
                isAlreadyStatus = ddlisAlreadyStatus.SelectedValue;
            }

            string queryMsg = "";

            orderInfoInvokeObj = OrderInfoInvokeFactory.CreateInstance(warehouse, billType, startTime, endTime, maxCount, isAlreadyStatus);

            if (orderInfoInvokeObj.GetNCDataJoinRFID(out lstStock, out queryMsg) == false)
            {
                ShowMessageBox("查询用友系统其他入库单信息失败！详细信息：" + queryMsg);
                return;
            }
            ViewState["lstStock"] = lstStock;
        }

        //查询按钮
        protected void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                //清空界面数据
                GridView1.DataSource = null;
                GridView1.DataBind();

                List<StockOut> lstStock = null;
                OrderInfoInvoke orderInfoInvokeObj = new OrderInfoInvoke();
                QueryData(out orderInfoInvokeObj, out lstStock);
                if (lstStock != null)
                {
                    //绑定数据到界面的Gridview
                    GridView1.DataSource = lstStock;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox("数据导入失败！", ex);
            }
        }

        //提交
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string saveMsg = "";
            List<StockOut> lstStock = new List<StockOut>();//所有的采购订单
            List<StockOut> lstSelectStock = new List<StockOut>();//被选中的采购订单行项目
            bool isChecked = false;
            OrderInfoInvoke orderInfoInvokeObj = new OrderInfoInvoke();

            QueryData(out orderInfoInvokeObj, out lstStock);
            if (ViewState["lstStock"] != null)
            {
                lstStock = (List<StockOut>)ViewState["lstStock"];
            }

            //获取选中的行项目
            foreach (GridViewRow grow in GridView1.Rows)
            {
                CheckBox chkSelect = grow.Cells[0].Controls[1] as CheckBox;
                if (chkSelect.Checked)
                {
                    isChecked = true;
                    string ncCode = (grow.Cells[1].Controls[1] as Label).Text;
                    foreach (StockOut stock in lstStock)
                    {
                        if (stock.FromBillNo == ncCode)
                        {
                            lstSelectStock.Add(stock);
                        }
                    }
                }
            }

            if (!isChecked || lstSelectStock.Count == 0)
            {
                ShowMessageBox("请选择需要导入的订单！");
                return;
            }

            bool result = orderInfoInvokeObj.SaveToRFID(lstSelectStock, out saveMsg);
            if (result)
            {
                btnReturn.Visible = true;
                btnSubmit.Enabled = false;
            }
            ShowMessageBox(result == true ? "数据导入成功！" : "数据导入失败！", new Exception(saveMsg));
        }

        //列表排序
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            ////保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(GridView1.Attributes["sortExpression"]) && "ASC".Equals(GridView1.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            GridView1.Attributes.Add("sortExpression", sortExpression);
            GridView1.Attributes.Add("sortDirection", sortDirection);

            //GridView1.DataBind();
            List<StockOut> lstStock = null;
            if (ViewState["lstStock"] != null)
            {
                lstStock = (List<StockOut>)ViewState["lstStock"];
            }
            else
            {
                OrderInfoInvoke orderInfoInvokeObj = new OrderInfoInvoke();
                QueryData(out orderInfoInvokeObj, out lstStock);
            }

            if (lstStock != null)
            {
                //string sortExpression = e.SortExpression;
                SortDirection sortDirection1 = GridView1.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                if (sortDirection1 == SortDirection.Ascending)
                    lstStock = lstStock.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                else
                    lstStock = lstStock.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                //绑定数据到界面的Gridview
                GridView1.DataSource = lstStock;
                GridView1.DataBind();
                ViewState["lstStock"] = lstStock;
            }
        }

        //返回订单列表
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("StockOutMgr.aspx");
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
                if (ViewState["lstStock"] != null)
                {
                    GridView1.DataSource = (List<StockOut>)ViewState["lstStock"];
                    GridView1.DataBind();//由于是绑定的数据源控件，所以需要重新进行绑定
                }
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

        //由收发类别代码获取解释
        protected string GetOutCategory(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "OutCategory" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }

        //由源RFID订单类型获取解释
        protected string GetFromOrderType(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "FromOrderType" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }

        //由业务类型获取解释
        protected string GetOutBusinessType(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "OBusinessType" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }
    }
}