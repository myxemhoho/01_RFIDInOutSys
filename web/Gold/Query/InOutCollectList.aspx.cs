using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;

namespace Gold.Query
{
    public partial class InOutCollectList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                txtStartTime.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtStartTime.Attributes.Add("readonly", "true");
                txtEndTime.Attributes.Add("readonly", "true");

                RadioButton_ByCargo.Checked = true;
                RadioButton_ByWHCode.Checked = false;

                LoadNewPanelDropDownList();


                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"

                btnQuery_Click(sender, e);
            }
            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(GetCurrentShowGrid());//生成固定表头
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            if (RadioButton_ByCargo.Checked)
            {
                lblDisplayPattern.Text = "按商品汇总";
                DropDownList_WHCode.Enabled = false;
                gv_CargoList.Enabled = true;
                gv_CargoList.Visible = true;
                gv_CargoList2.Enabled = false;
                gv_CargoList2.Visible = false;

                GridViewBind();
            }
            if (RadioButton_ByWHCode.Checked)
            {
                lblDisplayPattern.Text = "按仓库汇总";
                DropDownList_WHCode.Enabled = true;
                gv_CargoList.Enabled = false;
                gv_CargoList.Visible = false;
                gv_CargoList2.Enabled = true;
                gv_CargoList2.Visible = true;

                GridViewBind2();
            }
        }

        /// <summary>
        /// 加载dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定型号
                var allModelList = (from r in context.Models orderby r.ModelName select new { r.ModelId, r.ModelName }).ToList();
                DropDownList_CargoModel.Items.Clear();
                DropDownList_CargoModel.DataTextField = "ModelName";
                DropDownList_CargoModel.DataValueField = "ModelID";
                DropDownList_CargoModel.DataSource = allModelList;
                DropDownList_CargoModel.DataBind();
                DropDownList_CargoModel.Items.Insert(0, new ListItem("", ""));

                //绑定规格
                var allSpecList = (from r in context.Specifications orderby r.SpecName select new { r.SpecId, r.SpecName }).ToList();
                DropDownList_CargoSpec.Items.Clear();
                DropDownList_CargoSpec.DataTextField = "SpecName";
                DropDownList_CargoSpec.DataValueField = "SpecId";
                DropDownList_CargoSpec.DataSource = allSpecList;
                DropDownList_CargoSpec.DataBind();
                DropDownList_CargoSpec.Items.Insert(0, new ListItem("", ""));

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

        #region 按商品汇总

        void GridViewBind()
        {
            try
            {
                lblGridViewMsg.Text = "";

                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                DateTime startDate = DateTime.Parse(txtStartTime.Text);
                DateTime endDate = DateTime.Parse(txtEndTime.Text);
                DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<proc_WebSelectInOutCollectByCargo_Result> queryResult = context.proc_WebSelectInOutCollectByCargo(cargoCode, cargoName, modelName, specName, "", startTime, endTime).ToList<proc_WebSelectInOutCollectByCargo_Result>();

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

        int _sumNumOriginal;
        int _sumNumAdd;
        int _sumNumDel;
        int _sumNumCurrent;

        //计算合计数量
        protected void gv_CargoList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int sumOriginal;
                int sumAdd;
                int sumDel;
                int sumCurrent;

                int.TryParse(e.Row.Cells[7].Text, out sumOriginal);
                _sumNumOriginal += sumOriginal;
                int.TryParse(e.Row.Cells[8].Text, out sumAdd);
                _sumNumAdd += sumAdd;
                int.TryParse(e.Row.Cells[9].Text, out sumDel);
                _sumNumDel += sumDel;
                int.TryParse(e.Row.Cells[10].Text, out sumCurrent);
                _sumNumCurrent += sumCurrent;
            }


            // 合计 要显示合计行时必须设置GridView的ShowFooter属性
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "合计";
                e.Row.Cells[0].ColumnSpan = 7;
                e.Row.Cells[0].Attributes.Add("align", "center");
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;


                e.Row.Cells[7].Text = _sumNumOriginal.ToString();
                e.Row.Cells[8].Text = _sumNumAdd.ToString();
                e.Row.Cells[9].Text = _sumNumDel.ToString();
                e.Row.Cells[10].Text = _sumNumCurrent.ToString();

                //ViewState["_sumNumOriginal"] = _sumNumOriginal.ToString();
                //ViewState["_sumNumAdd"] = _sumNumAdd.ToString();
                //ViewState["_sumNumDel"] = _sumNumDel.ToString();
                //ViewState["_sumNumCurrent"] = _sumNumCurrent.ToString();

                
                e.Row.Cells[7].Attributes.Add("align", "right");
                e.Row.Cells[8].Attributes.Add("align", "right");
                e.Row.Cells[9].Attributes.Add("align", "right");
                e.Row.Cells[10].Attributes.Add("align", "right");
            }
        }

        #endregion

        #region 按仓库汇总

        void GridViewBind2()
        {
            try
            {
                lblGridViewMsg.Text = "";

                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                DateTime startDate = DateTime.Parse(txtStartTime.Text);
                DateTime endDate = DateTime.Parse(txtEndTime.Text);
                DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<proc_WebSelectInOutCollectByWHCode_Result> queryResult = context.proc_WebSelectInOutCollectByWHCode(whCode, cargoCode, cargoName, modelName, specName, "", startTime, endTime).ToList<proc_WebSelectInOutCollectByWHCode_Result>();

                    string sortExpression = gv_CargoList2.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList2.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    gv_CargoList2.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_CargoList2.DataSource = queryResult;
                    gv_CargoList2.DataBind();
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

        protected void gv_CargoList2_PageIndexChanging(object sender, GridViewPageEventArgs e)
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
                    txtNewPageIndex = pagerRow.FindControl("txtNewPageIndex2") as TextBox;    // refer to the TextBox with the NewPageIndex value
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

            GridViewBind2();
        }

        protected void gv_CargoList2_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_CargoList2.Attributes["sortExpression"]) && "ASC".Equals(gv_CargoList2.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_CargoList2.Attributes.Add("sortExpression", sortExpression);
            gv_CargoList2.Attributes.Add("sortDirection", sortDirection);

            GridViewBind2();
        }

        protected void gv_CargoList2_DataBound(object sender, EventArgs e)
        {
            //设置自定义分页的显示状态
            GridView theGrid = sender as GridView;
            //if (theGrid.Rows.Count > theGrid.PageSize)
            //{
            int newPageIndex = theGrid.PageIndex;

            GridViewRow pagerRowShow = theGrid.BottomPagerRow;
            if (pagerRowShow == null)
                return;
            LinkButton btnFirst = pagerRowShow.FindControl("btnFirst2") as LinkButton;
            LinkButton btnPrev = pagerRowShow.FindControl("btnPrev2") as LinkButton;
            LinkButton btnNext = pagerRowShow.FindControl("btnNext2") as LinkButton;
            LinkButton btnLast = pagerRowShow.FindControl("btnLast2") as LinkButton;


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

        int _sumNumOriginal2;
        int _sumNumAdd2;
        int _sumNumDel2;
        int _sumNumCurrent2;

        //计算合计数量
        protected void gv_CargoList2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int sumOriginal;
                int sumAdd;
                int sumDel;
                int sumCurrent;

                int.TryParse(e.Row.Cells[8].Text, out sumOriginal);
                _sumNumOriginal2 += sumOriginal;
                int.TryParse(e.Row.Cells[9].Text, out sumAdd);
                _sumNumAdd2 += sumAdd;
                int.TryParse(e.Row.Cells[10].Text, out sumDel);
                _sumNumDel2 += sumDel;
                int.TryParse(e.Row.Cells[11].Text, out sumCurrent);
                _sumNumCurrent2 += sumCurrent;
            }


            // 合计  要显示合计行时必须设置GridView的ShowFooter属性
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "合计";
                e.Row.Cells[0].ColumnSpan = 8;
                e.Row.Cells[0].Attributes.Add("align", "center");
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[7].Visible = false;

                e.Row.Cells[8].Text = _sumNumOriginal2.ToString();
                e.Row.Cells[9].Text = _sumNumAdd2.ToString();
                e.Row.Cells[10].Text = _sumNumDel2.ToString();
                e.Row.Cells[11].Text = _sumNumCurrent2.ToString();

                
                e.Row.Cells[8].Attributes.Add("align", "right");
                e.Row.Cells[9].Attributes.Add("align", "right");
                e.Row.Cells[10].Attributes.Add("align", "right");
                e.Row.Cells[11].Attributes.Add("align", "right");
            }
        }

        #endregion

        protected void RadioButton_ByCargo_CheckedChanged(object sender, EventArgs e)
        {
            btnQuery_Click(sender, e);
        }

        protected void RadioButton_ByWHCode_CheckedChanged(object sender, EventArgs e)
        {
            btnQuery_Click(sender, e);
        }

        protected void btnGoToPrintAndExportPage_Click(object sender, EventArgs e)
        {
            //在url传递之前需要Server.UrlEncode编码（若不编码，则url参数中的加号会变为空格），但是接受时不用解码（不需要Server.UrlDecode），asp.net自动解码
            string displayPattern =Server.UrlEncode(RadioButton_ByCargo.Checked ? "ByCargo" : "ByWHCode");
            string cargoCode = Server.UrlEncode(tbxCargoCode.Text.Trim());
            string cargoName = Server.UrlEncode(tbxCargoName.Text.Trim());
            string modelName = Server.UrlEncode(DropDownList_CargoModel.SelectedItem.Text.Trim());
            string specName = Server.UrlEncode(DropDownList_CargoSpec.SelectedItem.Text.Trim());
            string whName = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Text.Trim());
            string whCode = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Value.Trim());
            string startDate = Server.UrlEncode(txtStartTime.Text);
            string endDate = Server.UrlEncode(txtEndTime.Text);
            string sortExpression = Server.UrlEncode(RadioButton_ByCargo.Checked ? gv_CargoList.Attributes["sortExpression"] : gv_CargoList2.Attributes["sortExpression"]);
            string sortDirection = Server.UrlEncode(RadioButton_ByCargo.Checked ? gv_CargoList.Attributes["sortDirection"] : gv_CargoList2.Attributes["sortDirection"]);

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/InOutCollectListRpt.aspx?");
            url.Append("displayPattern=");
            url.Append(displayPattern);
            url.Append("&cargoCode=");
            url.Append(cargoCode);
            url.Append("&cargoName=");
            url.Append(cargoName);
            url.Append("&modelName=");
            url.Append(modelName);
            url.Append("&specName=");
            url.Append(specName);
            url.Append("&whName=");
            url.Append(whName);
            url.Append("&whCode=");
            url.Append(whCode);
            url.Append("&startDate=");
            url.Append(startDate);
            url.Append("&endDate=");
            url.Append(endDate);
            url.Append("&sortExpression=");
            url.Append(sortExpression);
            url.Append("&sortDirection=");
            url.Append(sortDirection);
            Response.Redirect(url.ToString());
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
                GridView gv_SaleAllocationList = GetCurrentShowGrid();

                string sortExpression = lBtn.CommandArgument;//获取排序字段，进行排序
                gv_SaleAllocationList.Sort(sortExpression, gv_SaleAllocationList.SortDirection);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        //获取当前显示的DataGridView
        public GridView GetCurrentShowGrid() 
        {
            if (RadioButton_ByCargo.Checked)
            {
                return gv_CargoList;
            }
            if (RadioButton_ByWHCode.Checked)
            {
                return gv_CargoList2;
            }

            return gv_CargoList;
        }

        #endregion
    }
}