using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gold.BaseInfoSetting
{
    public partial class PackagesList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack) 
            {
                gv_PackageList.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_PackageList);//生成固定表头
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

            lblAddMsg.Text = "";
            try
            {
                string newName = tbxNewPackageName.Text.Trim();
                string newComment = tbxNewComment.Text.Trim();
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var sameResult = (from r in context.Packages where r.PackageName.Equals(newName) select r).FirstOrDefault();
                    if (sameResult != null)
                    {
                        lblAddMsg.Text = "系统中已存在名为【" + newName + "】的型号信息！请重填！";
                        return;
                    }

                    DAL.Packages newPackageObj = new DAL.Packages();
                    newPackageObj.PackageName = newName;
                    newPackageObj.Comment = newComment;
                    context.Packages.AddObject(newPackageObj);
                    context.SaveChanges();
                    lblAddMsg.Text = "保存成功";

                    gv_PackageList.DataBind();

                }
            }
            catch (Exception ex)
            {
                lblAddMsg.Text = "保存失败！详细信息:" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }



        /// <summary>
        /// 清空新增栏控件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbxNewPackageName.Text = "";
            tbxNewComment.Text = "";
            lblAddMsg.Text = "";
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

            gv_PackageList.DataBind();
        }

        /// <summary>
        /// 行命令触发前先清空界面消息提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
                lblGridViewMsg.Text = "";

        }

        /// <summary>
        /// 更新完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.AffectedRows >= 0)
                lblGridViewMsg.Text = "更新成功！";
            else
                lblGridViewMsg.Text = "更新失败[" + e.Exception.InnerException.Message.ToString() + "]";
        }

        /// <summary>
        /// 行删除完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.AffectedRows > 0)
                lblGridViewMsg.Text = "删除成功！";
            else
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(e.Exception) + "]";
        }

        /// <summary>
        /// 更新时检测是否重名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string newPackageName = e.NewValues["PackageName"].ToString().Trim();
            string oldPackageName = e.OldValues["PackageName"].ToString().Trim();

            using (DAL.GoldEntities context = new DAL.GoldEntities())
            {
                var selectResult = (from r in context.Packages where (r.PackageName != oldPackageName && r.PackageName == newPackageName) select r).ToList();
                if (selectResult != null && selectResult.Count > 0)
                {
                    lblGridViewMsg.Text = "更新失败,系统中已经存在名为[" + newPackageName + "]的型号信息，请重新填写！";
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PackageList_DataBound(object sender, EventArgs e)
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

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
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
                SortDirection sortDirection = gv_PackageList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_PackageList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_PackageList.Sort(sortExpression, newSortDirection);
                gv_PackageList.DataBind();//由于是绑定的数据源控件，所以需要重新进行绑定
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}