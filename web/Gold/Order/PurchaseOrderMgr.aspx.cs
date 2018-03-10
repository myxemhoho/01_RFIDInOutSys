using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data;

namespace Gold.Order
{
    public partial class PurchaseOrderMgr : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtStartTime.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtStartTime.Attributes.Add("readonly", "true");
                txtEndTime.Attributes.Add("readonly", "true");
                InitDropList();

                GridView1.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.GridView1);//生成固定表头
        }

        private void InitDropList()
        {

        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            DateTime d1;
            if (!DateTime.TryParse(txtStartTime.Text.Trim(), out d1))
            {
                lblMessage.Text = "日期格式错！";
                return;
            }

            DateTime d2;
            if (!DateTime.TryParse(txtEndTime.Text.Trim(), out d2))
            {
                lblMessage.Text = "日期格式错！";
                return;
            }

            if (d1 > d2)
            {
                lblMessage.Text = "起始日期大于结束日期！";
                return;
            }

            GridView1.DataBind();
            lblGridViewMsg.Text = string.Empty;
        }
                
        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        //根据条件搜索采购订单
        protected void EntityDataSource1_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            //订单编码
            if (!string.IsNullOrEmpty(txtOrderCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.OrderCode == txtOrderCode.Text.Trim());
                return;
            }

            //订单状态
            if (ddlOrderStatus.SelectedIndex != 0)
            {
                int status = Convert.ToInt32(ddlOrderStatus.SelectedValue);
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.OrderStatus == status);
            }

            //订单时间
            DateTime startDate = DateTime.Parse(txtStartTime.Text);
            DateTime endDate = DateTime.Parse(txtEndTime.Text);
            endDate = endDate.AddDays(1);
            e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.OrderDate >= startDate && o.OrderDate < endDate);

            //采购组织
            if (ddlPurachaser.SelectedIndex != 0)
            {
                string purchaser = ddlPurachaser.SelectedValue;
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.Purchaser.Contains(purchaser));
            }

            //部门
            if (ddlDepartment.SelectedIndex != 0)
            {
                string department = ddlDepartment.SelectedValue;
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.DepartmentName.Contains(department));
            }

            //采购员
            if (ddlBuyer.SelectedIndex != 0)
            {
                string buyer = ddlBuyer.SelectedValue;
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.Buyer.Contains(buyer));
            }            

            //制单人
            //if (ddlPreparer.SelectedIndex != 0)
            //{
            //    string preparer = ddlPreparer.SelectedValue;
            //    e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.Preparer.Contains(preparer));
            //}

            //供应商
            if (ddlSupplier.SelectedIndex != 0)
            {
                string supplier = ddlSupplier.SelectedValue;
                e.Query = e.Query.Cast<PurchaseOrder>().Where(o => o.Supplier.Contains(supplier));
            }

            //按照订单状态倒序排列
            e.Query = e.Query.Cast<PurchaseOrder>().OrderByDescending(o => o.OrderStatus == 2).ThenByDescending(o => o.OrderStatus == 4).ThenByDescending(o=>o.OrderStatus == 3).ThenByDescending(o=>o.OrderStatus == 1).ThenByDescending(o=>o.OrderStatus == 0).ThenByDescending(o=>o.OrderCode);
        }
               
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

            GridView1.DataBind();
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

        //删除行        
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {  
                List<string> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    lblGridViewMsg.Text = "请先选中待删除的项";
                    return;
                }

                using (GoldEntities tmp = new GoldEntities())
                {
                    foreach (string delCode in checkedDelList)
                    {
                        DAL.PurchaseOrder delObject = (from r in tmp.PurchaseOrder where r.OrderCode.Equals(delCode) select r).FirstOrDefault();

                        tmp.DeleteObject(delObject);
                    }
                    int delRow = tmp.SaveChanges();
                    if (delRow > 0)
                        lblGridViewMsg.Text = "删除成功！[已删除" + delRow.ToString() + "项]";
                    else
                        lblGridViewMsg.Text = "删除失败！";
                    
                    GridView1.DataBind();//删除后重新绑定数据
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
            }
        }       

        //获取已选中的行项目
        private List<string> GetCheckItemID()
        {
            lblGridViewMsg.Text = string.Empty;

            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridViewRow currentRow = GridView1.Rows[i];
                if (currentRow != null)
                {                   
                    string itemID = (currentRow.FindControl("txtOrderCode") as TextBox).Text.ToString();
                    string orderStatus = (currentRow.FindControl("txtOrderStatus") as TextBox).Text.ToString();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;

                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        //只有初始状态的订单，才可以被删除
                        if (orderStatus != "2")
                        {                          
                            throw new Exception("此订单(" + itemID + ")已开始或已完成其他作业，不能被删除!请重新选择要删除的订单！");
                        }
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
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

        #region 设置下拉列表默认值
        protected void ddlPurachaser_DataBound(object sender, EventArgs e)
        {
            ddlPurachaser.SelectedValue = "深圳中心采购组织";
        }

        //客户提出，不需要设置部门和采购员的默认值--20120904
        //protected void ddlDepartment_DataBound(object sender, EventArgs e)
        //{
        //    ddlDepartment.SelectedValue = "物流部";
        //}
        //protected void ddlddlBuyer_DataBound(object sender, EventArgs e)
        //{
        //    if (Session["UserInfo"] != null)
        //    {
        //        Users userInfo = (Users)Session["UserInfo"];
        //        ddlBuyer.SelectedValue = userInfo.UserName;
        //    }
        //}
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
                GridView1.DataBind();//由于是绑定的数据源控件，所以需要重新进行绑定
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        //导入NC订单
        protected void btnNCDataImport_Click(object sender, EventArgs e)
        {
            Response.Redirect("PurchaseOrderFromNC.aspx");
        }        
    }
}