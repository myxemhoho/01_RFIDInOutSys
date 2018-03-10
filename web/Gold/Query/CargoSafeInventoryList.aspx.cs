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
    public partial class CargoSafeInventoryList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();
                
                GridViewBind();

            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_SaleAllocationList);//生成固定表头
        }

        #region 查询

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GridViewBind();
        }

        void GridViewBind()
        {
            try
            {
                //lblGridViewMsg.Text = "";

                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.Trim();
                whCode = whCode == null ? "" : whCode;

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<DAL.proc_WebSelectCargoSafeInventory_Result> queryResult = context.proc_WebSelectCargoSafeInventory(whCode, cargoCode, cargoName, modelName, specName, "").ToList<DAL.proc_WebSelectCargoSafeInventory_Result>();




                    string sortExpression = gv_SaleAllocationList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_SaleAllocationList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();


                    gv_SaleAllocationList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_SaleAllocationList.DataSource = queryResult;
                    gv_SaleAllocationList.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.gv_SaleAllocationList, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SaleAllocationList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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


                GridViewBind();//根据新页索引重新绑定数据

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
        protected void gv_SaleAllocationList_DataBound(object sender, EventArgs e)
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
        }

        #endregion
        
        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
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

                string whCode = WebConfigHelper.Instance.GetAppSettingValue("DefaultWHCode");
                var WHName = (from r in context.WareHouse where r.WHCode == whCode select r.WHName).FirstOrDefault();
                if (WHName != null) 
                {
                    lblWHName.Text = WHName.ToString();
                }
            }
        }

        void ClearTipMsgLabel()
        {
            //lblCheckMsg.Text = "";
            //lblAddMsg.Text = "";
            lblGridViewMsg.Text = "";
        }

        protected void gv_SaleAllocationList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_SaleAllocationList.Attributes["sortExpression"]) && "ASC".Equals(gv_SaleAllocationList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_SaleAllocationList.Attributes.Add("sortExpression", sortExpression);
            gv_SaleAllocationList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void btnGoToPrintAndExportPage_Click(object sender, EventArgs e)
        {
            //string cargoCode = tbxCargoCode.Text.Trim();
            //string cargoName = tbxCargoName.Text.Trim();
            //string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
            //string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
            //string saleStatusStr = DropDownList_SaleStatus.SelectedItem.Value.Trim();            
            //System.Text.StringBuilder url = new System.Text.StringBuilder("~/SaleCargoSetting/SaleAllocationQueryRpt.aspx?");
            //url.Append("&cargoCode=");
            //url.Append(cargoCode);
            //url.Append("&cargoName=");
            //url.Append(cargoName);
            //url.Append("&modelName=");
            //url.Append(modelName);
            //url.Append("&specName=");
            //url.Append(specName);
            //url.Append("&saleStatusStr=");
            //url.Append(saleStatusStr);
            //Response.Redirect(url.ToString());
        }

        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid == false)
                return;

            try
            {
                lblAddMsg.Text = "";
                string whCode = WebConfigHelper.Instance.GetAppSettingValue("DefaultWHCode");
                List<string> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    lblAddMsg.Text = "请先选中待设置的项";
                    //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先在列表中选中待设置的项，填写要设置的安全库存量后再点击“保存”按钮');</script>");
                    return;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    foreach (string checkCode in checkedDelList)
                    {
                        CargoSafeInventory existModel = (from r in context.CargoSafeInventory where r.CargoCode.Equals(checkCode) && r.WHCode.Equals(whCode) select r).FirstOrDefault();
                        if (existModel != null)
                        {
                            existModel.SafeInventory = int.Parse(tbxNewSafeNumber.Text);
                        }
                        else 
                        {
                            CargoSafeInventory newObj = new CargoSafeInventory();
                            newObj.WHCode = whCode;
                            newObj.CargoCode = checkCode;
                            newObj.SafeInventory = int.Parse(tbxNewSafeNumber.Text);

                            context.CargoSafeInventory.AddObject(newObj);
                        }

                    }
                    int affectedRow = context.SaveChanges();
                    if (affectedRow > 0)
                        lblAddMsg.Text = "保存成功！[已保存" + affectedRow.ToString() + "项]";
                    else
                        lblAddMsg.Text = "保存失败！";

                    GridViewBind();//删除后重新绑定数据
                }
            }
            catch (Exception ex)
            {
                lblAddMsg.Text = "保存失败！";
                //lblGridViewMsg.Text = "删除出现异常！详细信息：" + Utility.LogHelper.GetExceptionMsg(ex);
                LogHelper.WriteLog(LogHelper.LogLevel.Error, "安全库存量设置AddNew", ex);
            }
        }


        private List<string> GetCheckItemID()
        {
            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < gv_SaleAllocationList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_SaleAllocationList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = (currentRow.FindControl("lblCargoCode") as Label).Text.Trim();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;
                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
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
                gv_SaleAllocationList.Sort(sortExpression, gv_SaleAllocationList.SortDirection);
                ScriptManager.RegisterStartupScript(this.gv_SaleAllocationList, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}