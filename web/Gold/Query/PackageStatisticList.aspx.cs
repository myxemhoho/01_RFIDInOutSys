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
    public partial class PackageStatisticList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();

                gv_BinList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_BinList);//生成固定表头
        }

        #region 查询

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

            gv_BinList.DataBind();
        }





        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_DataBound(object sender, EventArgs e)
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

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_BinList);//生成固定表头
        }

        #endregion



        #region 编辑

        /// <summary>
        /// 行命令触发前先清空界面消息提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView senderGrid = sender as GridView;
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
            {
                //lblGridViewMsg.Text = "";
                ClearTipMsgLabel();//清除提示信息
            }
            else if (e.CommandName == "MyDefineUpdate")
            {
                lblGridViewMsg.Text = "";
                if (senderGrid.EditIndex != -1)
                {
                    int editBinCode = (int)senderGrid.DataKeys[senderGrid.EditIndex].Value;
                    try
                    {
                        using (GoldEntities context = new GoldEntities())
                        {
                            PackageStatistic updateModel = (from r in context.PackageStatistic where r.PSID == editBinCode select r).FirstOrDefault();
                            string msg = "";
                            if (GetUpdateModel(ref updateModel, senderGrid.Rows[senderGrid.EditIndex], out msg) == false)
                            {
                                lblGridViewMsg.Text = msg;
                                DAL.CommonConvert.ShowMessageBox(this.Page, msg);
                                return;
                            }
                            else
                            {
                                int result = context.SaveChanges();
                                if (result > 0)
                                {
                                    lblGridViewMsg.Text = "更新成功！";
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新成功!");
                                    senderGrid.EditIndex = -1;//取消编辑状态
                                }
                                else
                                {
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新失败（影响行数为0）!");
                                    lblGridViewMsg.Text = "更新失败（影响行数为0）";
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "更新失败!");
                        lblGridViewMsg.Text = "更新失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
                    }
                }
            }

        }

        /// <summary>
        /// 在行数据绑定完成后绑定编辑界面的DropDownList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //GridView theGrid = sender as GridView;

            //if (theGrid.EditIndex != -1)
            //{
            //    if ((e.Row.RowIndex == theGrid.EditIndex) &&
            //        (e.Row.RowType == DataControlRowType.DataRow) &&
            //        (e.Row.RowState.HasFlag(DataControlRowState.Edit) &&
            //        (e.Row.DataItem != null)))
            //    {
            //        //GridViewRow editRow = theGrid.Rows[theGrid.EditIndex]; //GridView较DataGrid提供了更多的API，获取分页块可以使用BottomPagerRow 或者TopPagerRow，当然还增加了HeaderRow和FooterRow

            //    }
            //}
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetUpdateModel(ref PackageStatistic updateModel, GridViewRow editRow, out string msg)
        {
            msg = "";
            try
            {
                if (null == editRow)
                {
                    msg = "获取的编辑行为空！请重试！";
                    return false;
                }
                TextBox tbxRowPackageTotalCount = null;
                TextBox tbxRowPackageNoStart = null;
                TextBox tbxRowPackageNoEnd = null;
                TextBox tbxRowComment = null;

                tbxRowPackageTotalCount = editRow.FindControl("tbxRowPackageTotalCount") as TextBox;
                tbxRowPackageNoStart = editRow.FindControl("tbxRowPackageNoStart") as TextBox;
                tbxRowPackageNoEnd = editRow.FindControl("tbxRowPackageNoEnd") as TextBox;
                tbxRowComment = editRow.FindControl("tbxRowComment") as TextBox;

                if (tbxRowComment == null || tbxRowPackageNoEnd == null || tbxRowPackageNoStart == null || tbxRowPackageTotalCount == null)
                {
                    msg = "未能获取编辑的数据！";
                    return false;
                }
                int packageTotalCount = 0;
                if (int.TryParse(tbxRowPackageTotalCount.Text, out packageTotalCount) == false)
                {
                    msg = "总数量只能为数字！";
                    return false;
                }
                updateModel.PackageTotalCount = packageTotalCount;
                updateModel.PackageNoStart = tbxRowPackageNoStart.Text.Trim();
                updateModel.PackageNoEnd = tbxRowPackageNoEnd.Text.Trim();
                updateModel.UpdateTime = DateTime.Now;
                updateModel.Remark = tbxRowComment.Text.Trim();

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }

        }


        #endregion

        #region 行删除

        /// <summary>
        /// 行删除完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除成功!");
                lblGridViewMsg.Text = "删除成功！";
            }
            else
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败!");
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(e.Exception) + "]";
            }
        }

        #endregion

        #region 多项删除

        /// <summary>
        /// 获取已选中行
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckItemID()
        {
            List<int> CheckIDList = new List<int>();
            for (int i = 0; i < gv_BinList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_BinList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = (currentRow.FindControl("lblRowPSID") as Label).Text.Trim();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;
                    int tempID = 0;
                    if (int.TryParse(itemID, out tempID) == true)
                    {
                        if (currentCbx.Checked && !CheckIDList.Contains(tempID))
                        {
                            CheckIDList.Add(tempID);
                        }
                    }
                }
            }
            return CheckIDList;
        }

        /// <summary>
        /// 多项删除按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ClearTipMsgLabel();//清除提示信息

                List<int> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    lblCheckMsg.Text = "请先选中待删除的项";
                    DAL.CommonConvert.ShowMessageBox(this.Page, "请先选中待删除的项!");
                    return;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    foreach (int delCode in checkedDelList)
                    {
                        PackageStatistic delObject = (from r in context.PackageStatistic where r.PSID.Equals(delCode) select r).FirstOrDefault();

                        context.DeleteObject(delObject);
                    }
                    int delRow = context.SaveChanges();
                    if (delRow > 0)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "删除成功！[已删除" + delRow.ToString() + "项]");
                        lblCheckMsg.Text = "删除成功！[已删除" + delRow.ToString() + "项]";
                    }
                    else
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败!");
                        lblCheckMsg.Text = "删除失败！";
                    }

                    //GridViewBind();//删除后重新绑定数据
                    gv_BinList.DataBind();
                }
            }
            catch (Exception ex)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败!");
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
            }
        }
        #endregion

        #region 新增

        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定包装  查询
                var result = (from r in context.Packages select new { r.PackageId, r.PackageName }).OrderBy(r => r.PackageName).ToList();
                DropDownList_PackageName.Items.Clear();
                DropDownList_PackageName.DataTextField = "PackageName";
                DropDownList_PackageName.DataValueField = "PackageId";
                DropDownList_PackageName.DataSource = result;
                DropDownList_PackageName.DataBind();
                DropDownList_PackageName.Items.Insert(0, new ListItem("", ""));

                //绑定包装  新增                
                DropDownListNewPackage.Items.Clear();
                DropDownListNewPackage.DataTextField = "PackageName";
                DropDownListNewPackage.DataValueField = "PackageId";
                DropDownListNewPackage.DataSource = result;
                DropDownListNewPackage.DataBind();
                DropDownListNewPackage.Items.Insert(0, new ListItem("", ""));

                //绑定仓库 查询
                var resultWH = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                DropDownList_WHCode.Items.Clear();
                DropDownList_WHCode.DataTextField = "WHName";
                DropDownList_WHCode.DataValueField = "WHCode";
                DropDownList_WHCode.DataSource = resultWH;
                DropDownList_WHCode.DataBind();
                DropDownList_WHCode.Items.Insert(0, new ListItem("", ""));

                //绑定仓库 新增
                DropDownListNewWHCode.Items.Clear();
                DropDownListNewWHCode.DataTextField = "WHName";
                DropDownListNewWHCode.DataValueField = "WHCode";
                DropDownListNewWHCode.DataSource = resultWH;
                DropDownListNewWHCode.DataBind();
                DropDownListNewWHCode.Items.Insert(0, new ListItem("", ""));
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out PackageStatistic newModel, out string msg)
        {
            msg = "";
            newModel = null;
            try
            {
                if (string.IsNullOrEmpty(DropDownListNewWHCode.SelectedItem.Value) ||
                    string.IsNullOrEmpty(DropDownListNewPackage.SelectedItem.Value) ||
                    string.IsNullOrEmpty(tbxNewPackageTotalCount.Text.Trim()))
                {
                    msg = "仓库、包装、总数量 均为必填项！请填写！";
                    return false;
                }
                int tempTotalCount = 0;
                if (int.TryParse(tbxNewPackageTotalCount.Text, out tempTotalCount) == false)
                {
                    msg = "总数量必须为数字！请重新填写！";
                    return false;
                }
                string whCode = "";
                string packageName = "";

                newModel = new PackageStatistic();
                newModel.WHCode = DropDownListNewWHCode.SelectedItem.Value;
                newModel.WHName = DropDownListNewWHCode.SelectedItem.Text;
                newModel.PackageName = DropDownListNewPackage.SelectedItem.Text;
                int tempPackageID = 0;
                if (int.TryParse(DropDownListNewPackage.SelectedItem.Value, out tempPackageID) == true)
                    newModel.PackageId = tempPackageID;
                else
                    newModel.PackageId = null;

                newModel.PackageNoStart = tbxNewPackageStart.Text.Trim();
                newModel.PackageNoEnd = tbxNewPackageEnd.Text.Trim();

                newModel.PackageTotalCount = tempTotalCount;
                newModel.Remark = tbxNewRemark.Text;
                newModel.UpdateTime = DateTime.Now;

                whCode = newModel.WHCode;
                packageName = newModel.PackageName.Trim();

                using (GoldEntities context = new GoldEntities())
                {
                    var sameCode = (from r in context.PackageStatistic where (r.WHCode == whCode && r.PackageName.Trim() == packageName.Trim()) select r).ToList();
                    if (sameCode != null && sameCode.Count > 0)
                        msg += "系统中已经存在仓库为[" + whCode + "]、包装为[" + packageName + "]的记录,请重填或编辑现有记录!";

                    if (msg.Length > 0)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                msg = Utility.LogHelper.GetExceptionMsg(ex);
                return false;
            }

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

            //lblAddMsg.Text = "";
            ClearTipMsgLabel();//清除提示信息
            try
            {
                PackageStatistic newModel = null;
                string msg = "";
                if (GetNewModel(out newModel, out msg) == false)
                {
                    lblAddMsg.Text = msg;
                    DAL.CommonConvert.ShowMessageBox(this.Page, msg);
                    return;
                }
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    context.PackageStatistic.AddObject(newModel);
                    context.SaveChanges();
                    lblAddMsg.Text = "保存成功";
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存成功!");

                    gv_BinList.DataBind();
                }
            }
            catch (Exception ex)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "保存失败!");
                lblAddMsg.Text = "保存失败！详细信息：" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        /// <summary>
        /// 清空新增栏控件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbxNewPackageTotalCount.Text = "";
            tbxNewPackageStart.Text = "";
            tbxNewPackageEnd.Text = "";
            tbxNewRemark.Text = "";
            lblAddMsg.Text = "";
        }

        #endregion


        void ClearTipMsgLabel()
        {
            lblCheckMsg.Text = "";
            lblAddMsg.Text = "";
            lblGridViewMsg.Text = "";
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
                SortDirection sortDirection= gv_BinList.SortDirection;
                SortDirection newSortDirection;
                switch(gv_BinList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_BinList.Sort(sortExpression, newSortDirection);
                gv_BinList.DataBind();//本页面是用EntityDataSource绑定的所以这里要重新绑定下数据
                ScriptManager.RegisterStartupScript(this.gv_BinList, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}