using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gold.DAL;

using System.Linq.Expressions;
using System.Management;

namespace Gold.BaseInfoSetting
{
    public partial class CargoTagList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();

                gv_CargoTagList.PageSize =  Utility.WebConfigHelper.Instance.GetDefaultPageSize();

                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"
                GridViewBind();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoTagList);//生成固定表头
        }

        void GridViewBind()
        {
            try
            {
                lblGridViewMsg.Text = "";
                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                //string whCode = DropDownList_WareHouse.SelectedItem.Value;
                string storageBinCode = tbxCargoTagBinCode.Text.Trim();//DropDownList_StorageBin.SelectedItem.Value;
                string storageBinStatus = DropDownList_StorageState.SelectedItem.Value;


                //&& r.StorageBin.WareHouse.Contains(whCode)
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var firstResult = (from r in context.CargoTag select r);
                    if (string.IsNullOrEmpty(storageBinCode) == false)
                        firstResult = (from r in firstResult where r.BinCode.Contains(storageBinCode) select r);

                    if (storageBinStatus == "")
                    {
                        if (string.IsNullOrEmpty(cargoCode) == false)
                            firstResult = (from r in firstResult where r.CargoCode.Contains(cargoCode) select r);
                        if (string.IsNullOrEmpty(cargoName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoName.Contains(cargoName) select r);
                        if (string.IsNullOrEmpty(modelName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoModel.Contains(modelName) select r);
                        if (string.IsNullOrEmpty(specName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoSpec.Contains(specName) select r);
                    }
                    else if (storageBinStatus == "1")
                    {
                        firstResult = from r in firstResult where r.CargoCode != null select r;

                        if (string.IsNullOrEmpty(cargoCode) == false)
                            firstResult = (from r in firstResult where r.CargoCode.Contains(cargoCode) select r);
                        if (string.IsNullOrEmpty(cargoName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoName.Contains(cargoName) select r);
                        if (string.IsNullOrEmpty(modelName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoModel.Contains(modelName) select r);
                        if (string.IsNullOrEmpty(specName) == false)
                            firstResult = (from r in firstResult where r.Cargos.CargoSpec.Contains(specName) select r);
                    }
                    else if (storageBinStatus == "0")
                    {
                        firstResult = from r in firstResult where (r.CargoCode == null || r.CargoCode == "") select r;
                    }

                    var queryResult = (from r in firstResult
                                       select new
                                       {
                                           //r.StorageBin.WareHouse1.WHName,
                                           //r.StorageBin.WareHouse,
                                           r.BinCode,
                                           //r.StorageBin.BinName,
                                           r.TagCode,
                                           r.Number,
                                           r.CargoCode,
                                           r.Cargos.CargoName,
                                           r.Cargos.CargoModel,
                                           r.Cargos.CargoSpec,
                                           r.Cargos.CargoUnits,
                                           r.Comment,
                                           r.Reserve1,
                                           r.Reserve2

                                       }).ToList();


                    string sortExpression = gv_CargoTagList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoTagList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    
                    gv_CargoTagList.DataSource = queryResult.ToList();
                    gv_CargoTagList.DataBind();
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

        protected void gv_CargoTagList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

        protected void gv_CargoTagList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_CargoTagList.Attributes["sortExpression"]) && "ASC".Equals(gv_CargoTagList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_CargoTagList.Attributes.Add("sortExpression", sortExpression);
            gv_CargoTagList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void gv_CargoTagList_DataBound(object sender, EventArgs e)
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

        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            try
            {
                using (GoldEntities context = new GoldEntities())
                {
                    ////绑定仓库
                    //var allWareHouse = (from r in context.WareHouse orderby r.WHCode select new { r.WHCode, r.WHName }).ToList();
                    //DropDownList_WareHouse.Items.Clear();
                    //DropDownList_WareHouse.DataTextField = "WHName";
                    //DropDownList_WareHouse.DataValueField = "WHCode";
                    //DropDownList_WareHouse.DataSource = allWareHouse;
                    //DropDownList_WareHouse.DataBind();
                    //DropDownList_WareHouse.Items.Insert(0, new ListItem("", ""));

                    ////绑定层位
                    //DropDownList_WareHouse_SelectedIndexChanged(DropDownList_WareHouse, new EventArgs());

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
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询条件绑定数据时出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        protected void DropDownList_WareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string whCode = (sender as DropDownList).SelectedItem.Value;
            //using (GoldEntities context = new GoldEntities())
            //{
            //    DropDownList_StorageBin.Items.Clear();
            //    DropDownList_StorageBin.DataTextField = "BinName";
            //    DropDownList_StorageBin.DataValueField = "BinCode";
            //    //绑定层位
            //    if (whCode == null || whCode.Trim() == "")
            //    {
            //        var allStorageBin = (from r in context.StorageBin orderby r.BinCode select new { r.BinCode, r.BinName }).ToList();

            //        DropDownList_StorageBin.DataSource = allStorageBin;
            //        DropDownList_StorageBin.DataBind();
            //    }
            //    else
            //    {
            //        var allStorageBin = (from r in context.StorageBin where r.WareHouse == whCode orderby r.BinCode select new { r.BinCode, r.BinName }).ToList();

            //        DropDownList_StorageBin.DataSource = allStorageBin;
            //        DropDownList_StorageBin.DataBind();
            //    }
            //    DropDownList_StorageBin.Items.Insert(0, new ListItem("", ""));
            //}
        }


        #region 导出Excel

        protected void lbtnExport_Click(object sender, EventArgs e)
        {

            //注意必须设置<%@ Page  EnableEventValidation ="false"  否则页面会全部输出到Excel中

            try
            {
                ////gv_CargoTagList.DataBind();
                gv_CargoTagList.AllowPaging = false;//取消分页,使GridView显示全部数据.
                GridViewBind();
                Response.Clear();
                Response.Charset = "GB2312";
                Response.AppendHeader("Content-Disposition", "attachment;filename= " + Server.UrlEncode("tag.xls"));
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                Response.ContentType = "application/ms-excel";//设置输出文件类型为excel文件.
                Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=GB2312\">");
                this.EnableViewState = false;
                System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
                System.IO.StringWriter stringWrite = new System.IO.StringWriter(myCItrad);
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                System.Web.UI.HtmlTextWriter htmlWrite2 = new HtmlTextWriter(stringWrite);
                divHeader.RenderControl(htmlWrite2);
                gv_CargoTagList.RenderControl(htmlWrite);
                Response.Write(stringWrite.ToString());
                Response.Write(@"<style> .text { mso-number-format:\@; } </style> ");
                Response.End();
                gv_CargoTagList.AllowPaging = true;//
            }
            catch (Exception ex)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "导出Excel失败！");
                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "导出Excel失败", ex);
            }

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            
        }

        #endregion

        protected void gv_CargoTagList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@;");//防止Excel输出乱码
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
                SortDirection sortDirection = gv_CargoTagList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_CargoTagList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_CargoTagList.Sort(sortExpression, newSortDirection);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}