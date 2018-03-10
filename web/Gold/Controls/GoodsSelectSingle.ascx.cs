using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.Controls
{
    public partial class GoodsSelectSingle : System.Web.UI.UserControl
    {
        public bool ShowPop
        {
            get
            {
                if (ViewState["ShowPop"] != null)
                    return true;
                else
                    return false;
            }
        }

        public event EventHandler GetCargoSelect;
        private List<string[]> _listSelectedCargo = new List<string[]>();

        public List<string[]> ListSelectedCargo
        {
            get
            {
                return _listSelectedCargo;
            }
        }

        public string[] CargoQueryCondition
        {
            set;
            get;
        }


        public event EventHandler<EventArgs> PostBack;

        private void RiseEvent()
        {
            EventHandler<EventArgs> handler = this.PostBack;
            if (handler != null)
            {
                EventArgs e = new EventArgs();
                handler(this, e);
            }
        }

        public void DataBindForQuery()
        {
            chkQueryAll.Checked = false;//其他页面调用并打开此控件时，不查询全部商品

            gvGoods.DataSource = null;//清除GridView中视图缓存
            gvGoods.DataBind();


            //从主界面传查询条件进行查询
            if (CargoQueryCondition != null)
            {
                if (CargoQueryCondition[0] != null)
                {
                    txtCode.Text = CargoQueryCondition[0];
                }
                if (CargoQueryCondition[1] != null)
                {
                    txtName.Text = CargoQueryCondition[1];
                }
            }

            if ((string.IsNullOrEmpty(txtCode.Text.Trim()) && string.IsNullOrEmpty(txtName.Text.Trim())) && chkQueryAll.Checked == false) //当未填写查询条件或未勾选查询条件时不进行查询
            {
                return;
            }

            //this.gvGoods.DataBind();
            GridViewBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gvGoods);//生成固定表头
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            
            //gvGoods.DataBind();
            GridViewBind();
            ViewState["ShowPop"] = null;
            RiseEvent();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvGoods.Rows)
            {
                RadioButton ckb = (RadioButton)row.FindControl("rbSelect");//CheckBox ckb = (CheckBox)row.FindControl("chbSelect");
                if (ckb.Checked)
                {
                    string cargoCode = row.Cells[1].Text.Trim() == "&nbsp;" ? "" : row.Cells[1].Text;
                    string cargoName = row.Cells[2].Text.Trim() == "&nbsp;" ? "" : row.Cells[2].Text;
                    string cargoModel = row.Cells[3].Text.Trim() == "&nbsp;" ? "" : row.Cells[3].Text;
                    string cargoSpec = row.Cells[4].Text.Trim() == "&nbsp;" ? "" : row.Cells[4].Text;
                    string cargoUnits = row.Cells[5].Text.Trim() == "&nbsp;" ? "" : row.Cells[5].Text;
                    string produceYear = row.Cells[6].Text.Trim() == "&nbsp;" ? "" : row.Cells[6].Text;
                    string[] binCodeSelect = new string[] { cargoCode, cargoName, cargoModel, cargoSpec, cargoUnits, produceYear };
                    _listSelectedCargo.Add(binCodeSelect);
                }
            }
            if (GetCargoSelect != null)
                GetCargoSelect(this, EventArgs.Empty);

            ViewState["ShowPop"] = true;

            //清空选择框为不选择的状态
            foreach (GridViewRow row in gvGoods.Rows)
            {
                RadioButton ckb = (RadioButton)row.FindControl("rbSelect");//CheckBox ckb = (CheckBox)row.FindControl("chbSelect");
                ckb.Checked = false;
            }
            txtCode.Text = string.Empty;
            txtName.Text = string.Empty;

            //RiseEvent();
        }

        protected void edsCargos_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            //从主界面传查询条件进行查询
            if (CargoQueryCondition != null)
            {
                if (CargoQueryCondition[0] != null)
                {
                    txtCode.Text = CargoQueryCondition[0];
                }
                if (CargoQueryCondition[1] != null)
                {
                    txtName.Text = CargoQueryCondition[1];
                }
            }

            if (!string.IsNullOrEmpty(txtCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<Cargos>().Where(o => o.CargoCode.Contains(txtCode.Text.Trim()));
            }

            if (!string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                e.Query = e.Query.Cast<Cargos>().Where(o => o.CargoName.Contains(txtName.Text.Trim()));
            }
        }

        protected void gvGoods_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ViewState["ShowPop"] = null;
            if (e.CommandName == "GoodsSelect")
            {
                GridViewRow gvr = (e.CommandSource as Control).Parent.Parent as GridViewRow;
                gvGoods.SelectedIndex = gvr.RowIndex;
            }

            RiseEvent();
        }

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGoods_PageIndexChanging(object sender, GridViewPageEventArgs e)
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


                //gvGoods.DataBind();//根据新页索引重新绑定数据
                GridViewBind();

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
        protected void gvGoods_DataBound(object sender, EventArgs e)
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
                        lbtn.Attributes.Add("onclick", "showWaitDiv('divWaitUCCargo');");//js提示

                        td.Controls.Add(lbtn);


                    }
                    else
                    {
                        td.Text = cell.HeaderText;
                    }

                    //本页面是单选框，不需要全选按钮
                    //if (string.IsNullOrEmpty(cell.SortExpression) && cell.HeaderText == "*")
                    //{

                    //    CheckBox chk = new CheckBox();
                    //    chk.ID = "chkAllCheck" + (i++).ToString();
                    //    chk.Text = "全选";
                    //    //chk.AutoPostBack = true;
                    //    //chk.CheckedChanged += new EventHandler(CheckBox_CheckAll_CheckedChanged);
                    //    chk.Attributes.Add("onclick", "selectAllCheckBox('" + gv_SaleAllocationList.ClientID + "',this);");

                    //    td.Controls.Add(chk);

                    //}

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
                SortDirection sortDirection = gvGoods.SortDirection;
                SortDirection newSortDirection;
                switch (gvGoods.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gvGoods.Sort(sortExpression, newSortDirection);

                
                //gvGoods.DataBind();//因使用的是数据源控件，所以要重新绑定
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWaitUCCargo');", true);//js提示
            }


            ViewState["ShowPop"] = null;
            RiseEvent();
        }

        #endregion

        void GridViewBind()
        {
            try
            {
                lblGridViewMsg.Text = "";

                string cargoCode = txtCode.Text.Trim();
                string cargoName = txtName.Text.Trim();

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var queryResult = (from r in context.Cargos
                                       where r.CargoCode.Contains(cargoCode) && r.CargoName.Contains(cargoName)
                                       select r).ToList();

                    string sortExpression = gvGoods.Attributes["sortExpression"];
                    SortDirection sortDirection = gvGoods.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    //gvGoods.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
                    gvGoods.DataSource = queryResult;
                    gvGoods.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWaitUCCargo');", true);//js提示
            }
        }

        protected void gvGoods_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gvGoods.Attributes["sortExpression"]) && "ASC".Equals(gvGoods.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gvGoods.Attributes.Add("sortExpression", sortExpression);
            gvGoods.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }
    }
}