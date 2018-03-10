using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.Query
{
    public partial class InventoryBookQtyGroup : System.Web.UI.Page
    {   
        

        protected void Page_Load(object sender, EventArgs e)
        {
            GoodsSelect1.PostBack += this.GoodsSelect1_PostBack;

            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtStartDate.Attributes.Add("readonly", "true");
                txtEndDate.Attributes.Add("readonly", "true");

                LoadNewPanelDropDownList();//设置仓库下拉菜单数据源

                if (drpWHCode.Items.Count > 0)//设置仓库默认值
                {
                    string defaultWHCode =Utility.WebConfigHelper.Instance.GetAppSettingValue("DefaultWHCode");
                    drpWHCode.SelectedIndex = drpWHCode.Items.IndexOf(drpWHCode.Items.FindByValue(defaultWHCode));
                }

                GridViewBind();
            }
            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.GridView1);//生成固定表头
        }

        void GridViewBind()
        {
            try
            {
                string whCode = drpWHCode.SelectedItem.Value.ToString();
                string cargoCode = txtCargoCode.Text.Trim();
                string cargoName = txtCargoName.Text.Trim();
                string modelName = txtCargoModel.Text.Trim();
                string specName = txtCargoSpec.Text.Trim();
                DateTime datetimeStart = DateTime.Now.AddDays(-30);
                DateTime datetimeEnd = DateTime.Now;
                if (DateTime.TryParse(txtStartDate.Text, out datetimeStart) == false
                    || DateTime.TryParse(txtEndDate.Text, out datetimeEnd) == false)
                {
                    ShowMessageBox("请选择有效的台账日期！");
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    return;
                }

                //设置截止时间为当天的23:59:59:999
                datetimeEnd = new DateTime(datetimeEnd.Year, datetimeEnd.Month, datetimeEnd.Day, 23, 59, 59, 999);


                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    
                    var queryResult = from r in context.VInventoryBookJoinWH
                                       where r.ChangeTime >= datetimeStart && r.ChangeTime <= datetimeEnd
                                       select r;
                    

                    if (string.IsNullOrEmpty(whCode) == false)
                        queryResult = (from r in queryResult where r.WHCode.Contains(whCode) select r);
                    if (string.IsNullOrEmpty(cargoCode) == false)
                        queryResult = (from r in queryResult where r.CargoCode.Contains(cargoCode) select r);
                    if (string.IsNullOrEmpty(cargoName) == false)
                        queryResult = (from r in queryResult where r.CargoName.Contains(cargoName) select r);
                    if (string.IsNullOrEmpty(modelName) == false)
                        queryResult = (from r in queryResult where r.CargoModel.Contains(modelName) select r);
                    if (string.IsNullOrEmpty(specName) == false)
                        queryResult = (from r in queryResult where r.CargoSpec.Contains(specName) select r);

                    #region 排序

                    //获取自定义排序
                    string sortExpression = GridView1.Attributes["sortExpression"];
                    SortDirection sortDirection = GridView1.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;
                    
                    //在商品名相同时还要按商品编码排序
                    if (sortExpression == "CargoCode")
                    {
                        if (sortDirection == SortDirection.Ascending)
                            queryResult = queryResult.OrderBy(r => r.WHCode).ThenBy(r => r.CargoCode).ThenBy(r => r.ChangeTime).ThenBy(r => r.TableAccountID);////加ThenBy(o=>o.TableAccountID)目的是防止台账时间一致，排序结果显示异常的问题
                        else
                            queryResult = queryResult.OrderBy(r => r.WHCode).ThenByDescending(r => r.CargoCode).ThenBy(r => r.ChangeTime).ThenBy(r => r.TableAccountID);
                    }
                    else if (sortExpression == "CargoName")
                    {
                        if (sortDirection == SortDirection.Ascending)
                            queryResult = queryResult.OrderBy(r => r.WHCode).ThenBy(r => r.CargoName).ThenBy(r => r.CargoCode).ThenBy(r => r.ChangeTime).ThenBy(r => r.TableAccountID);
                        else
                            queryResult = queryResult.OrderBy(r => r.WHCode).ThenByDescending(r => r.CargoName).ThenBy(r => r.CargoCode).ThenBy(r => r.ChangeTime).ThenBy(r => r.TableAccountID);
                    }
                    #endregion

                    GridView1.DataSource = queryResult.ToList();
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                //lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                ShowMessageBox("查询出现异常！", ex);
            }
        }

        //protected void edsInventoryBook_QueryCreated(object sender, QueryCreatedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtCargoCode.Text.Trim()))
        //    {
        //        e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoCode.Contains(txtCargoCode.Text.Trim()));
        //    }

        //    if (!string.IsNullOrEmpty(txtCargoName.Text.Trim()))
        //    {
        //        e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoName.Contains(txtCargoName.Text.Trim()));
        //    }

        //    if (!string.IsNullOrEmpty(txtCargoModel.Text.Trim()))
        //    {
        //        e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoModel.Contains(txtCargoModel.Text.Trim()));
        //    }

        //    if (!string.IsNullOrEmpty(txtCargoSpec.Text.Trim()))
        //    {
        //        e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoSpec.Contains(txtCargoSpec.Text.Trim()));
        //    }

        //    //订单时间
        //    DateTime startDate = DateTime.Parse(txtStartDate.Text);
        //    DateTime endDate = DateTime.Parse(txtEndDate.Text);
        //    endDate = endDate.AddDays(1);
        //    e.Query = e.Query.Cast<InventoryBook>().Where(o => o.ChangeTime >= startDate && o.ChangeTime < endDate);

        //    //仓库
        //    if (!string.IsNullOrEmpty(drpWHCode.SelectedValue))
        //    {
        //        e.Query = e.Query.Cast<InventoryBook>().Where(o => o.WHCode == drpWHCode.SelectedValue);
        //    }

        //    //e.Query = e.Query.Cast<InventoryBook>().OrderBy(o => o.WHCode).ThenBy(o => o.CargoCode).ThenBy(o => o.ChangeTime).ThenBy(o => o.TableAccountID);//加ThenBy(o=>o.TableAccountID)目的是防止台账时间一致，排序结果显示异常的问题
        //}

        protected void Button1_Click(object sender, EventArgs e)
        {
            DateTime d1;
            if (!DateTime.TryParse(txtStartDate.Text.Trim(), out d1))
            {
                lblMessage.Text = "日期格式错！";
                return;

            }

            DateTime d2;
            if (!DateTime.TryParse(txtEndDate.Text.Trim(), out d2))
            {
                lblMessage.Text = "日期格式错！";
                return;
            }

            if (d1 > d2)
            {
                lblMessage.Text = "起始日期大于结束日期！";
                return;
            }

            //GridView1.DataBind();
            GridViewBind();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }


        int _sumNumOriginal;//期初 值只能是第一条期初数量
        int _sumNumAdd;
        int _sumNumDel;
        int _sumNumCurrent;//结余 值只能是最后一条结余数量
        GridViewRow _preRow = null;
        int _preRowSeq = 1;
        int _rowSpan = 1;
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");//防止商品编码显示成科学计数法形式

                int sumOriginal;
                int sumAdd;
                int sumDel;
                int sumCurrent;

                int.TryParse(e.Row.Cells[11].Text, out sumOriginal);
                int.TryParse(e.Row.Cells[12].Text, out sumAdd);
                int.TryParse(e.Row.Cells[13].Text, out sumDel);
                int.TryParse(e.Row.Cells[14].Text, out sumCurrent);

                //首行，赋值后返回
                if (_preRow == null)
                {
                    _sumNumOriginal += sumOriginal;
                    _sumNumAdd += sumAdd;
                    _sumNumDel += sumDel;
                    _sumNumCurrent += sumCurrent;

                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                    return;
                }

                //仓库编码商品编码与前行相同，合并第0~6列 （序号，仓库编码，仓库名称，商品名称，商品编码，型号，规格）
                if (e.Row.Cells[1].Text == _preRow.Cells[1].Text && e.Row.Cells[4].Text == _preRow.Cells[4].Text)
                {
                    //_sumNumOriginal += sumOriginal;//期初数量合计值不用累加
                    _sumNumAdd += sumAdd;
                    _sumNumDel += sumDel;
                    _sumNumCurrent = sumCurrent;//结余数量不累加，只取最后一行的值

                    _preRow.Cells[0].RowSpan = ++_rowSpan;
                    _preRow.Cells[1].RowSpan = _rowSpan;
                    _preRow.Cells[2].RowSpan = _rowSpan;
                    _preRow.Cells[3].RowSpan = _rowSpan;
                    _preRow.Cells[4].RowSpan = _rowSpan;
                    _preRow.Cells[5].RowSpan = _rowSpan;
                    _preRow.Cells[6].RowSpan = _rowSpan;

                    _preRow.VerticalAlign = VerticalAlign.Middle;

                    e.Row.Cells[0].Visible = false;
                    e.Row.Cells[1].Visible = false;
                    e.Row.Cells[2].Visible = false;
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    e.Row.Cells[6].Visible = false;
                }
                else   //商品编码不一样，重新开始
                {
                    //添加小计行
                    InsertGridRow(e.Row);

                    _sumNumOriginal += sumOriginal;
                    _sumNumAdd += sumAdd;
                    _sumNumDel += sumDel;
                    _sumNumCurrent += sumCurrent;

                    _preRowSeq++;
                    _rowSpan = 1;
                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                }
            }

            //合计
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                InsertGridRow(e.Row);
            }
        }

        public GridViewRow InsertGridRow(GridViewRow beforeRow)
        {
            Table tbl = (Table)GridView1.Controls[0];
            int newRowIndex = tbl.Rows.GetRowIndex(beforeRow);
            GridViewRow newRow = new GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);

            TableCell[] cells = new TableCell[17];
            for (int i = 0; i < beforeRow.Cells.Count; i++)
            {
                cells[i] = new TableCell();
                cells[i].Visible = false;
            }
            newRow.Cells.AddRange(cells);

            _preRow.Cells[0].RowSpan = ++_rowSpan;
            _preRow.Cells[1].RowSpan = _rowSpan;
            _preRow.Cells[2].RowSpan = _rowSpan;
            _preRow.Cells[3].RowSpan = _rowSpan;
            _preRow.Cells[4].RowSpan = _rowSpan;
            _preRow.Cells[5].RowSpan = _rowSpan;
            _preRow.Cells[6].RowSpan = _rowSpan;
            _preRow.VerticalAlign = VerticalAlign.Middle;

            newRow.Cells[7].Text = "合计";
            newRow.Cells[7].ColumnSpan = 4;
            newRow.Cells[11].Text = _sumNumOriginal.ToString();
            newRow.Cells[12].Text = _sumNumAdd.ToString();
            newRow.Cells[13].Text = _sumNumDel.ToString();
            newRow.Cells[14].Text = _sumNumCurrent.ToString();

            newRow.Cells[7].Visible = true;
            newRow.Cells[11].Visible = true;
            newRow.Cells[12].Visible = true;
            newRow.Cells[13].Visible = true;
            newRow.Cells[14].Visible = true;
            newRow.Cells[15].Visible = true;
            newRow.Cells[16].Visible = true;

            newRow.Cells[11].Attributes.Add("align", "right");
            newRow.Cells[12].Attributes.Add("align", "right");
            newRow.Cells[13].Attributes.Add("align", "right");
            newRow.Cells[14].Attributes.Add("align", "right");

            _sumNumAdd = 0;
            _sumNumCurrent = 0;
            _sumNumDel = 0;
            _sumNumOriginal = 0;

            newRow.HorizontalAlign = HorizontalAlign.Center;
            tbl.Controls.AddAt(newRowIndex, newRow);

            return newRow;
        }

        protected void lbtnExport_Click(object sender, EventArgs e)
        {

            //GridView1.AllowPaging = false;//取消分页,使GridView显示全部数据.
            GridViewBind(); //GridView1.DataBind();
            Response.Clear();
            Response.Charset = "GB2312";
            Response.AppendHeader("Content-Disposition", "attachment;filename= " + Server.UrlEncode("库存台账（在用）.xls"));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/ms-excel";//设置输出文件类型为excel文件.
            Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=GB2312\">");
            this.EnableViewState = false;
            System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
            System.IO.StringWriter stringWrite = new System.IO.StringWriter(myCItrad);
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            System.Web.UI.HtmlTextWriter htmlWrite2 = new HtmlTextWriter(stringWrite);
            divHeader.RenderControl(htmlWrite2);
            GridView1.RenderControl(htmlWrite);

            Response.Write(stringWrite.ToString());
            Response.Write(@"<style> .text { mso-number-format:\@; } </script> ");
            Response.End();
            //GridView1.AllowPaging = true;


            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
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

                    //排序
                    if (!string.IsNullOrEmpty(cell.SortExpression) && (cell.SortExpression == "CargoName" || cell.SortExpression == "CargoCode"))
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
            //LinkButton lBtn = sender as LinkButton;
            //if (lBtn != null)
            //{
            //    string sortExpression = "WHCode,CargoCode,";
            //    string sortExpressionRight = "ChangeTime,TableAccountID";
            //    if (sortExpression.Contains(lBtn.CommandArgument) == false && sortExpressionRight.Contains(lBtn.CommandArgument) == false)
            //    {
            //        sortExpression += lBtn.CommandArgument;//获取排序字段，进行排序
            //    }
            //    if (GridView1.SortDirection == SortDirection.Ascending)
            //        GridView1.Sort(sortExpression, SortDirection.Descending);
            //    else
            //        GridView1.Sort(sortExpression, SortDirection.Ascending);
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            //}

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

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
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

        /// <summary>
        /// 加载dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定仓库
                var result = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                drpWHCode.Items.Clear();
                drpWHCode.DataTextField = "WHName";
                drpWHCode.DataValueField = "WHCode";
                drpWHCode.DataSource = result;
                drpWHCode.DataBind();
                drpWHCode.Items.Insert(0, new ListItem("", ""));
            }
        }

        #region 弹出框商品选择

        protected void SelectCargo_Click(object sender, EventArgs e)
        {
            string cargoCode =txtCargoCode.Text.Trim();
            string cargoName = txtCargoName.Text.Trim();
            string[] cargoCondition = new string[] { cargoCode, cargoName };
            GoodsSelect1.CargoQueryCondition = cargoCondition;
            GoodsSelect1.DataBindForQuery();
            this.popWindow.Show();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            this.popWindow.Hide();

            GridViewBind();//此处不应重新绑定数据，但关闭时若不重新绑定列表数据会出现合计行混乱

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            return;
        }

        //商品选择返回值
        protected void GoodsSelect1_GetCargoSelect(object sender, EventArgs e)
        {
            if (GoodsSelect1.ListSelectedCargo.Count > 1)
            {
                //ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('只能选择一个商品，请重新选择！');</script>");
                //Response.Write("<script type='text/javascript'>alert('只能选择一个商品');</script>");
                //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dddd", "alert('只能选择一个商品，请重新选择！');", true);

                //以上方式只能用在普通页面，对于Ajax页面使用以下函数
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "alertfun", "alert('只能选择一个商品，请重新选择');", true);

                this.popWindow.Show();
                return;
            }

            this.popWindow.Hide();

            //填充数据前先清空数据
            txtCargoCode.Text = "";//商品编码
            txtCargoName.Text = "";//商品名称
            txtCargoModel.Text = "";//型号  
            txtCargoSpec.Text = "";//规格

            for (int i = 0; i < GoodsSelect1.ListSelectedCargo.Count; i++)
            {
                string[] goodSelect = GoodsSelect1.ListSelectedCargo[i];
                if (i == 0)
                {
                    string cargoCode = goodSelect[0];

                    txtCargoCode.Text = goodSelect[0];//商品编码
                    txtCargoName.Text = goodSelect[1];//商品名称
                    //txtCargoModel.Text = goodSelect[2];//型号  台账中某些商品规格型号信息不全，这里不自动填充规格型号
                    //txtCargoSpec.Text = goodSelect[3];//规格
                    break;
                }
            }
            GridViewBind();//重新绑定数据
        }

        protected void GoodsSelect1_PostBack(object sender, EventArgs e)
        {
            if (!GoodsSelect1.ShowPop)
            {
                this.popWindow.Show();
                return;
            }
        }
        #endregion
    }
}