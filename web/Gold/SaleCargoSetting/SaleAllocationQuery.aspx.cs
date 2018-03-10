using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gold.DAL;
using Gold.Utility;
using System.Text;
using System.Data;

namespace Gold.SaleCargoSetting
{
    public partial class SaleAllocationQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ShowNews();//显示消息

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
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "showWaitDiv('divWait');", true);//js提示

                //lblGridViewMsg.Text = "";

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
                    List<DAL.proc_WebSelectEachDeptSaleAllocation_Result> resultList = context.proc_WebSelectEachDeptSaleAllocation().ToList<DAL.proc_WebSelectEachDeptSaleAllocation_Result>();


                    var queryResult = (from r in resultList
                                       where r.CargoCode.Contains(cargoCode)
                                       && r.CargoName.Contains(cargoName)
                                       && r.CargoModel.Contains(modelName)
                                       && r.CargoSpec.Contains(specName)
                                       && (saleStatus >= 0 ? r.SaleStatus == saleStatus : 1 == 1)
                                       select r).ToList();

                    string sortExpression = gv_SaleAllocationList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_SaleAllocationList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    //gv_SaleAllocationList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();//不分页
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
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
            //在url传递之前需要Server.UrlEncode编码（若不编码，则url参数中的加号会变为空格），但是接受时不用解码（不需要Server.UrlDecode），asp.net自动解码
            string cargoCode = Server.UrlEncode(tbxCargoCode.Text.Trim());
            string cargoName = Server.UrlEncode(tbxCargoName.Text.Trim());
            string modelName = Server.UrlEncode(DropDownList_CargoModel.SelectedItem.Text.Trim());
            string specName =Server.UrlEncode( DropDownList_CargoSpec.SelectedItem.Text.Trim());
            string saleStatusStr = Server.UrlEncode(DropDownList_SaleStatus.SelectedItem.Value.Trim());            
            System.Text.StringBuilder url = new System.Text.StringBuilder("~/SaleCargoSetting/SaleAllocationQueryRpt.aspx?");
            url.Append("&cargoCode=");
            url.Append(cargoCode);
            url.Append("&cargoName=");
            url.Append(cargoName);
            url.Append("&modelName=");
            url.Append(modelName);
            url.Append("&specName=");
            url.Append(specName);
            url.Append("&saleStatusStr=");
            url.Append(saleStatusStr);
            Response.Redirect(url.ToString());
        }

        protected void Timer_News_Tick(object sender, EventArgs e)
        {
            ShowNews();
        }

        private void ShowNews()
        {
            DataTable dt = ConvertDataTableXML.ReadSaleCargoNews();
            if (dt != null && dt.Columns.Contains("NewsCreateDate") && dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = "NewsCreateDate desc";
                dt = dt.DefaultView.ToTable();
                /*
                 dt.Columns.Add("NewsID", typeof(string));//消息ID
            dt.Columns.Add("NewsCreateDate", typeof(DateTime));//消息发布时间
            dt.Columns.Add("NewsTitle", typeof(string));//消息标题
            dt.Columns.Add("NewsContent", typeof(string));//消息内容
            dt.Columns.Add("EditorID", typeof(string));//消息发布人ID
            dt.Columns.Add("EditorName", typeof(string));//消息发布人姓名
                 
                 */
                string title = dt.Rows[0]["NewsTitle"].ToString();
                string content = dt.Rows[0]["NewsContent"].ToString();
                string datetime = dt.Rows[0]["NewsCreateDate"].ToString();
                string editorName = dt.Rows[0]["EditorName"].ToString();

                StringBuilder strNews = new StringBuilder("标题：");
                strNews.Append(title);
                strNews.Append("<br /><br />内容：");
                strNews.Append(Server.HtmlDecode(content).Replace("\r\n","<br />"));
                strNews.Append("<br /><br />发布人：");
                strNews.Append(editorName);
                strNews.Append("<br />发布时间：");
                strNews.Append(datetime);

                divNews.InnerHtml = strNews.ToString();
                //tbxNews.Text = strNews.ToString();

            }
            else
            {
                //tbxNews.Text = "暂无信息！";
                divNews.InnerText = "暂无信息！";
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
            table.Width =new Unit(gv_SaleAllocationList.Width.Value);

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
                        td.Width =(int)Math.Round(cell.ItemStyle.Width.Value);
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
                gv_SaleAllocationList.Sort(sortExpression, gv_SaleAllocationList.SortDirection);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');",true);//js提示
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