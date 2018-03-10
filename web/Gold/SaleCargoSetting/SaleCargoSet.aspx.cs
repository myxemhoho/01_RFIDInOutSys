using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;



namespace Gold.SaleCargoSetting
{
    public partial class SaleCargoSet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                

                LoadNewPanelDropDownList();

                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"
                GridViewBind();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoList);//生成固定表头
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
                string saleStatusStr = DropDownList_SaleStatus.SelectedItem.Value.Trim();
                int saleStatus = -1;
                if (!string.IsNullOrEmpty(saleStatusStr))
                {
                    if (int.TryParse(saleStatusStr, out saleStatus) == false)
                        saleStatus = -1;
                }

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var queryResult = (from r in context.Cargos
                                       where r.CargoCode.Contains(cargoCode)
                                       && r.CargoName.Contains(cargoName)
                                       && r.CargoModel.Contains(modelName)
                                       && r.CargoSpec.Contains(specName)
                                       && (saleStatus >= 0 ? r.SaleStatus == saleStatus : 1 == 1)
                                       select r).ToList();

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


        protected void btnQuery_Click(object sender, EventArgs e)
        {
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

        private List<string> GetCheckItemID()
        {
            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < gv_CargoList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_CargoList.Rows[i];
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



        protected void gv_CargoList_DataBound(object sender, EventArgs e)
        {
            string sjs = "GetResultFromServer();";
            ScriptManager.RegisterClientScriptBlock(this.gv_CargoList, this.GetType(), "", sjs, true);

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

                //绑定可售状态
                List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.CargoSaleStatus));
                DropDownList_SaleStatus.Items.Clear();
                DropDownList_SaleStatus.DataTextField = "Name";
                DropDownList_SaleStatus.DataValueField = "Value";
                DropDownList_SaleStatus.DataSource = ListBinType;
                DropDownList_SaleStatus.DataBind();
                DropDownList_SaleStatus.Items.Insert(0, new ListItem("", ""));
            }
        }

        //自定义排序
        protected void lBtnSaleStatus_Click(object sender, EventArgs e)
        {
            gv_CargoList_Sorting(this.gv_CargoList, new GridViewSortEventArgs("SaleStatus", this.gv_CargoList.SortDirection));
        }

        protected void CheckBox_CheckAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            foreach (GridViewRow gvRow in this.gv_CargoList.Rows)
            {
                CheckBox rowChk = gvRow.FindControl("chkSaleStatus") as CheckBox;
                if (rowChk != null)
                    rowChk.Checked = chk.Checked;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblGridViewMsg.Text = "";
            try
            {
                using (GoldEntities context = new GoldEntities())
                {
                    foreach (GridViewRow gvRow in this.gv_CargoList.Rows)
                    {
                        CheckBox rowChk = gvRow.FindControl("chkSaleStatus") as CheckBox;
                        Label lblCargoCode = gvRow.FindControl("lblCargoCode") as Label;
                        if (rowChk != null && lblCargoCode != null)
                        {
                            string cargoCode = lblCargoCode.Text.Trim();
                            DAL.Cargos updateModel = (from r in context.Cargos where r.CargoCode == cargoCode select r).FirstOrDefault();
                            updateModel.SaleStatus = rowChk.Checked ? 1 : 0;
                        }
                    }

                    //使用隐式事务
                    int AffectRowsCount = context.SaveChanges();
                    lblGridViewMsg.Text = "保存成功[更新" + AffectRowsCount.ToString() + "条记录]";
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "保存失败！[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
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

                        if (cell.SortExpression == "SaleStatus") //增加全选按钮
                        {
                            CheckBox chk = new CheckBox();
                            chk.ID = "chkAllCheck" + (i++).ToString();
                            chk.Text = "全选";
                            //chk.AutoPostBack = true;
                            //chk.CheckedChanged += new EventHandler(CheckBox_CheckAll_CheckedChanged);
                            chk.Attributes.Add("onclick", "selectAllCheckBox('"+gv_CargoList.ClientID+"',this);");

                            td.Controls.Add(chk);
                        }
                        
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
                gv_CargoList.Sort(sortExpression, gv_CargoList.SortDirection);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        

        #region 解决ViewState过于庞大的问题
        protected override object LoadPageStateFromPersistenceMedium()
        {
            string viewStateID = (string)((Pair)base.LoadPageStateFromPersistenceMedium()).Second;
            string stateStr = (string)Cache[viewStateID];
            if (stateStr == null)
            {
                string fn = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, @"App_Data/ViewState/" + viewStateID);
                stateStr = System.IO.File.ReadAllText(fn);
            }
            return new ObjectStateFormatter().Deserialize(stateStr);
        }

        protected override void SavePageStateToPersistenceMedium(object state)
        {
            string value = new ObjectStateFormatter().Serialize(state);
            string viewStateID = (DateTime.Now.Ticks + (long)this.GetHashCode()).ToString(); //产生离散的id号码
            string fn = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, @"App_Data/ViewState/" + viewStateID);
            //ThreadPool.QueueUserWorkItem(File.WriteAllText(fn, value));
            System.IO.File.WriteAllText(fn, value);
            Cache.Insert(viewStateID, value);
            base.SavePageStateToPersistenceMedium(viewStateID);
        }
        #endregion
    }
}