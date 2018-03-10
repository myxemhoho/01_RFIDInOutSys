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
    public partial class StorePicAccountList : System.Web.UI.Page
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
                lblDisplayPattern.Text = "按每天存提流水查看";
                //DropDownList_WHCode.Enabled = false;
                gv_CargoList.Enabled = true;
                gv_CargoList.Visible = true;
                gv_CargoList2.Enabled = false;
                gv_CargoList2.Visible = false;

                GridViewBind();
            }
            if (RadioButton_ByWHCode.Checked)
            {
                lblDisplayPattern.Text = "按月份存提汇总查看";
                //DropDownList_WHCode.Enabled = true;
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
                
                //绑定存提状态
                List<NameValueModel> ListPickOrStore = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.PickOrStore));
                DropDownList_StorePickType.Items.Clear();
                DropDownList_StorePickType.DataTextField = "Name";
                DropDownList_StorePickType.DataValueField = "Value";
                DropDownList_StorePickType.DataSource = ListPickOrStore;
                DropDownList_StorePickType.DataBind();
                DropDownList_StorePickType.Items.Insert(0, new ListItem("", ""));
                
                //绑定盈亏状态
                List<NameValueModel> ListIsProfitOrLoss = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.IsProfitOrLoss));
                DropDownList_IsProfitOrLoss.Items.Clear();
                DropDownList_IsProfitOrLoss.DataTextField = "Name";
                DropDownList_IsProfitOrLoss.DataValueField = "Value";
                DropDownList_IsProfitOrLoss.DataSource = ListIsProfitOrLoss;
                DropDownList_IsProfitOrLoss.DataBind();
                DropDownList_IsProfitOrLoss.Items.Insert(0, new ListItem("", ""));

                //绑定包装
                var result = (from r in context.Packages select new { r.PackageId, r.PackageName }).OrderBy(r => r.PackageName).ToList();
                DropDownList_PackageName.Items.Clear();
                DropDownList_PackageName.DataTextField = "PackageName";
                DropDownList_PackageName.DataValueField = "PackageId";
                DropDownList_PackageName.DataSource = result;
                DropDownList_PackageName.DataBind();
                DropDownList_PackageName.Items.Insert(0, new ListItem("", ""));

                //绑定仓库
                var resultWH = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                DropDownList_WHCode.Items.Clear();
                DropDownList_WHCode.DataTextField = "WHName";
                DropDownList_WHCode.DataValueField = "WHCode";
                DropDownList_WHCode.DataSource = resultWH;
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
                //string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                //string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                string packageShareNo = tbxPackageShareNo.Text.Trim();
                string isprofitOrLost = DropDownList_IsProfitOrLoss.SelectedItem.Value.ToString(); ;
                string storePickType = DropDownList_StorePickType.SelectedItem.Value.ToString();
                string packageName = DropDownList_PackageName.SelectedItem.Text.Trim();
                DateTime startDate = DateTime.Parse(txtStartTime.Text);
                DateTime endDate = DateTime.Parse(txtEndTime.Text);
                DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<proc_WebSelectStorePickAccount_Result> queryResult = context.proc_WebSelectStorePickAccount(whCode,cargoCode,cargoName,storePickType,isprofitOrLost,packageName,packageShareNo, startTime.ToString(), endTime.ToString()).ToList<proc_WebSelectStorePickAccount_Result>();

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

        //int _sumNumOriginal;
        //int _sumNumAdd;
        //int _sumNumDel;
        //int _sumNumCurrent;

        //计算合计数量
        protected void gv_CargoList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    int sumOriginal;
            //    int sumAdd;
            //    int sumDel;
            //    int sumCurrent;

            //    int.TryParse(e.Row.Cells[6].Text, out sumOriginal);
            //    _sumNumOriginal += sumOriginal;
            //    int.TryParse(e.Row.Cells[7].Text, out sumAdd);
            //    _sumNumAdd += sumAdd;
            //    int.TryParse(e.Row.Cells[8].Text, out sumDel);
            //    _sumNumDel += sumDel;
            //    int.TryParse(e.Row.Cells[9].Text, out sumCurrent);
            //    _sumNumCurrent += sumCurrent;
            //}


            //// 合计 要显示合计行时必须设置GridView的ShowFooter属性
            //if (e.Row.RowType == DataControlRowType.Footer)
            //{
            //    e.Row.Cells[0].Text = "合计";
            //    e.Row.Cells[0].ColumnSpan = 6;
            //    e.Row.Cells[0].Attributes.Add("align", "center");
            //    e.Row.Cells[1].Visible = false;
            //    e.Row.Cells[2].Visible = false;
            //    e.Row.Cells[3].Visible = false;
            //    e.Row.Cells[4].Visible = false;
            //    e.Row.Cells[5].Visible = false;


            //    e.Row.Cells[6].Text = _sumNumOriginal.ToString();
            //    e.Row.Cells[7].Text = _sumNumAdd.ToString();
            //    e.Row.Cells[8].Text = _sumNumDel.ToString();
            //    e.Row.Cells[9].Text = _sumNumCurrent.ToString();

            //    //ViewState["_sumNumOriginal"] = _sumNumOriginal.ToString();
            //    //ViewState["_sumNumAdd"] = _sumNumAdd.ToString();
            //    //ViewState["_sumNumDel"] = _sumNumDel.ToString();
            //    //ViewState["_sumNumCurrent"] = _sumNumCurrent.ToString();

            //    e.Row.Cells[6].Attributes.Add("align", "right");
            //    e.Row.Cells[7].Attributes.Add("align", "right");
            //    e.Row.Cells[8].Attributes.Add("align", "right");
            //    e.Row.Cells[9].Attributes.Add("align", "right");
            //}
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
                //string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                //string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                string packageShareNo = tbxPackageShareNo.Text.Trim();
                string isprofitOrLost = DropDownList_IsProfitOrLoss.SelectedItem.Value.ToString(); ;
                string storePickType = DropDownList_StorePickType.SelectedItem.Value.ToString();
                string packageName = DropDownList_PackageName.SelectedItem.Text.Trim();
                DateTime startDate = DateTime.Parse(txtStartTime.Text);
                DateTime endDate = DateTime.Parse(txtEndTime.Text);
                DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<proc_WebSelectStorePickAccount_Result> queryResult = context.proc_WebSelectStorePickAccount(whCode, cargoCode, cargoName, storePickType, isprofitOrLost, packageName, packageShareNo, startTime.ToString(), endTime.ToString()).ToList<proc_WebSelectStorePickAccount_Result>();
                    queryResult = (from r in queryResult orderby r.RecordMonth descending, r.StorePickType, r.WHCode ascending, r.CargoCode, r.RecordTime ascending select r).ToList<proc_WebSelectStorePickAccount_Result>();

                    string sortExpression = gv_CargoList2.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList2.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    

                    //string sortExpression = gv_CargoList2.Attributes["sortExpression"];
                    //SortDirection sortDirection = gv_CargoList2.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    //if (sortDirection == SortDirection.Ascending)
                    //{
                    //    queryResult = (from r in queryResult orderby r.RecordMonth descending, r.StorePickType, r.WHCode ascending, r.CargoCode, r.RecordTime ascending,r.GetType().GetProperty(sortExpression).GetValue(r, null) ascending select r).ToList<proc_WebSelectStorePickAccount_Result>();//queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    //}
                    //else
                    //{
                    //    queryResult = (from r in queryResult orderby r.RecordMonth descending, r.StorePickType, r.WHCode ascending, r.CargoCode, r.RecordTime ascending, r.GetType().GetProperty(sortExpression).GetValue(r, null) descending select r).ToList<proc_WebSelectStorePickAccount_Result>();
                    //}

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

        int StorePickNumberSum;
        int AccountNumberSum;
        int FactCheckNumberSum;
        GridViewRow _preRow = null;
        int _preRowSeq = 1;
        int _rowSpan = 1;

        //计算合计数量
        protected void gv_CargoList2_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            /*
                                     [StockPickAccountID]
      ,[RecordTime]
      ,[StorePickType]
      ,[StorePickNumber]
      ,[WHCode]
      ,[CargoCode]
      ,[CargoName]
      ,[CargoModel]
      ,[CargoSpec]
      ,[CargoUnits]
      ,[ReleaseYear]
      ,[AccountNumber]
      ,[FactCheckNumber]
      ,[IsProfitOrLoss]
      ,[PackageId]
      ,[PackageName]
      ,[PackageCount]
      ,[StandardCountEachBag]
      ,[PackageNoStart]
      ,[PackageNoEnd]
      ,[StoreDescription]
      ,[Remark]
      ,[PackageLockNo]
      ,[PackageShareNo]
      ,[RecordDetail]
      ,[RecordMonth]
      ,[BadRate]
             */
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int _StorePickNumber;
                int _AccountNumber;
                int _FactCheckNumber;

                int.TryParse(e.Row.Cells[7].Text, out _StorePickNumber);
                int.TryParse(e.Row.Cells[8].Text, out _AccountNumber);
                int.TryParse(e.Row.Cells[9].Text, out _FactCheckNumber);

                //首行，赋值后返回
                if (_preRow == null)
                {
                    StorePickNumberSum += _StorePickNumber;
                    AccountNumberSum += _AccountNumber;
                    FactCheckNumberSum += _FactCheckNumber;
                    

                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                    return;
                }

                //月份、仓库、存提类型、商品编码与前行相同，合并第0~6列 （序号，商品名称，商品编码，型号，规格）
                if (e.Row.Cells[1].Text == _preRow.Cells[1].Text
                    && e.Row.Cells[2].Text == _preRow.Cells[2].Text
                    && e.Row.Cells[3].Text == _preRow.Cells[3].Text
                    && e.Row.Cells[5].Text == _preRow.Cells[5].Text)
                {
                    StorePickNumberSum += _StorePickNumber;
                    AccountNumberSum += _AccountNumber;
                    FactCheckNumberSum += _FactCheckNumber;

                    _preRow.Cells[0].RowSpan = ++_rowSpan;
                    _preRow.Cells[1].RowSpan = _rowSpan;
                    _preRow.Cells[2].RowSpan = _rowSpan;
                    _preRow.Cells[3].RowSpan = _rowSpan;
                    _preRow.Cells[4].RowSpan = _rowSpan;
                    _preRow.Cells[5].RowSpan = _rowSpan;
                    _preRow.Cells[6].RowSpan = _rowSpan;
                    _preRow.Cells[7].RowSpan = _rowSpan;
                    _preRow.Cells[8].RowSpan = _rowSpan;
                    _preRow.Cells[9].RowSpan = _rowSpan;

                    _preRow.Cells[7].Text = StorePickNumberSum.ToString();
                    _preRow.Cells[8].Text = AccountNumberSum.ToString();
                    _preRow.Cells[9].Text = FactCheckNumberSum.ToString();

                    _preRow.VerticalAlign = VerticalAlign.Middle;

                    e.Row.Cells[0].Visible = false;
                    e.Row.Cells[1].Visible = false;
                    e.Row.Cells[2].Visible = false;
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    e.Row.Cells[6].Visible = false;
                    e.Row.Cells[7].Visible = false;
                    e.Row.Cells[8].Visible = false;
                    e.Row.Cells[9].Visible = false;
                }
                else   //商品编码不一样，重新开始
                {
                    StorePickNumberSum += _StorePickNumber;
                    AccountNumberSum += _AccountNumber;
                    FactCheckNumberSum += _FactCheckNumber;

                    _preRowSeq++;
                    _rowSpan = 1;
                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                }
            }


            
        }

        #endregion

        protected void RadioButton_ByCargo_CheckedChanged(object sender, EventArgs e)
        {
            btnQuery_Click(sender, e);
            btnGoToPrintAndExportPage.Visible = true;
            btnShowEditPage.Visible = true;
        }

        protected void RadioButton_ByWHCode_CheckedChanged(object sender, EventArgs e)
        {
            btnQuery_Click(sender, e);
            btnGoToPrintAndExportPage.Visible = false;
            btnShowEditPage.Visible = false;
        }

        protected void btnGoToPrintAndExportPage_Click(object sender, EventArgs e)
        {
            //在url传递之前需要Server.UrlEncode编码（若不编码，则url参数中的加号会变为空格），但是接受时不用解码（不需要Server.UrlDecode），asp.net自动解码
            //string displayPattern = RadioButton_ByCargo.Checked ? "ByCargo" : "ByWHCode";
            string cargoCode = Server.UrlEncode(tbxCargoCode.Text.Trim());
            string cargoName = Server.UrlEncode(tbxCargoName.Text.Trim());
            //string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
            //string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
            string whName = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Text.Trim());
            string whCode = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Value.Trim());
            string startDate = Server.UrlEncode(txtStartTime.Text);
            string endDate = Server.UrlEncode(txtEndTime.Text);
            string sortExpression = Server.UrlEncode(RadioButton_ByCargo.Checked ? gv_CargoList.Attributes["sortExpression"] : gv_CargoList2.Attributes["sortExpression"]);
            string sortDirection = Server.UrlEncode(RadioButton_ByCargo.Checked ? gv_CargoList.Attributes["sortDirection"] : gv_CargoList2.Attributes["sortDirection"]);
            string packageShareNo = Server.UrlEncode(tbxPackageShareNo.Text);
            string storePickType = Server.UrlEncode(DropDownList_StorePickType.SelectedItem.Value.ToString());
            string isProfitOrLoss = Server.UrlEncode(DropDownList_IsProfitOrLoss.SelectedItem.Text.ToString());
            string packageName = Server.UrlEncode(DropDownList_PackageName.SelectedItem.Text.Trim());

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StorePicAccountListRpt.aspx?");
            //url.Append("displayPattern=");
            //url.Append(displayPattern);
            url.Append("cargoCode=");
            url.Append(cargoCode);
            url.Append("&cargoName=");
            url.Append(cargoName);
            //url.Append("&modelName=");
            //url.Append(modelName);
            //url.Append("&specName=");
            //url.Append(specName);
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
            url.Append("&packageShareNo=");
            url.Append(packageShareNo);
            url.Append("&storePickType=");
            url.Append(storePickType);
            url.Append("&isProfitOrLoss=");
            url.Append(isProfitOrLoss);
            url.Append("&packageName=");
            url.Append(packageName);

            Response.Redirect(url.ToString());
        }

        protected void btnShowEditPage_Click(object sender, EventArgs e)
        {   //QueryString中EditType 1为新增 2为编辑
            //QueryString中EditID 0表示不编辑 1表示编辑
            string EditID = "0";

            //lblCheckMsg.Text = "";
            lblGridViewMsg.Text = "";

            List<string> checkedItem = GetCheckItemID();
            if (checkedItem.Count == 0)
            {
                //lblCheckMsg.Text = "请在列表中选择一条记录进行编辑！请重选！";
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "btnShowEditPage_Click_1", "alert('请在列表中选择一条记录进行编辑！');", true);
                return;
            }
            else if (checkedItem.Count > 1)
            {
                //lblCheckMsg.Text = "只能选择列表中的一条记录进行编辑！请重选！";
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "btnShowEditPage_Click_2", "alert('您选择的记录过多，只能选择列表中的一条记录进行编辑！');", true);
                return;
            }
            else
            {
                EditID = checkedItem[0].Trim();
            }

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StorePicAccountEdit.aspx?EditType=2");
            url.Append("&EditID=");
            url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
            Response.Redirect(url.ToString());
        }

        private List<string> GetCheckItemID()
        {
            //StockPickAccountID
            List<string> CheckIDList = new List<string>();

            int StockPickAccountIDIndex = GetColumnIndexByName(gv_CargoList, "流水号");

            if (StockPickAccountIDIndex == -1)
                return CheckIDList;

            
            for (int i = 0; i < gv_CargoList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_CargoList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = currentRow.Cells[StockPickAccountIDIndex].Text;
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;
                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
        }

        private int GetColumnIndexByName(GridView grid, string headerText)
        {
            foreach (DataControlField col in grid.Columns)
            {
                if (col.HeaderText.ToLower().Trim() == headerText.ToLower().Trim())
                {
                    return grid.Columns.IndexOf(col);
                }
            }

            return -1;
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