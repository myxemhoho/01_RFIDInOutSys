using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;
using System.Data;

namespace Gold.SaleCargoSetting
{
    public partial class SaleCargoNewsSet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["UserName"] != null)
                {
                    tbxEditor.Text = Session["UserName"].ToString();
                }
                tbxDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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
                /*
                 dt.Columns.Add("NewsID", typeof(string));//消息ID
            dt.Columns.Add("NewsCreateDate", typeof(DateTime));//消息发布时间
            dt.Columns.Add("NewsTitle", typeof(string));//消息标题
            dt.Columns.Add("NewsContent", typeof(string));//消息内容
            dt.Columns.Add("EditorID", typeof(string));//消息发布人ID
            dt.Columns.Add("EditorName", typeof(string));//消息发布人姓名
                 
                 */

                //lblGridViewMsg.Text = "";

                DataTable dt = ConvertDataTableXML.ReadSaleCargoNews();
                if (dt != null && dt.Columns.Contains("NewsCreateDate"))
                {
                    dt.DefaultView.Sort = "NewsCreateDate desc";
                    dt = dt.DefaultView.ToTable();
                }
                gv_CargoList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
                gv_CargoList.DataSource = dt;
                gv_CargoList.DataBind();

            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "加载历史数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
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
            //string sortExpression = e.SortExpression;
            //string sortDirection = "ASC";
            //if (sortExpression.Equals(gv_CargoList.Attributes["sortExpression"]) && "ASC".Equals(gv_CargoList.Attributes["sortDirection"]))
            //{
            //    sortDirection = "DESC";
            //}
            //gv_CargoList.Attributes.Add("sortExpression", sortExpression);
            //gv_CargoList.Attributes.Add("sortDirection", sortDirection);

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblGridViewMsg.Text = "";
            try
            {
                if (tbxTitle.Text.Trim() == "" || tbxContent.Text.Trim() == "")
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "信息标题和内容不能为空！请填写！");
                    return;
                }

                DataTable dt = ConvertDataTableXML.ReadSaleCargoNews();
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = ConvertDataTableXML.GetSaleCargoNewsTableSchema();
                }

                /*
                 dt.Columns.Add("NewsID", typeof(string));//消息ID
            dt.Columns.Add("NewsCreateDate", typeof(DateTime));//消息发布时间
            dt.Columns.Add("NewsTitle", typeof(string));//消息标题
            dt.Columns.Add("NewsContent", typeof(string));//消息内容
            dt.Columns.Add("EditorID", typeof(string));//消息发布人ID
            dt.Columns.Add("EditorName", typeof(string));//消息发布人姓名
                 
                 */


                DataRow drNew = dt.NewRow();
                drNew["NewsID"] = Guid.NewGuid().ToString();
                drNew["NewsCreateDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string title = tbxTitle.Text;
                string content = tbxContent.Text;

                drNew["NewsTitle"] = tbxTitle.Text;
                drNew["NewsContent"] = Server.HtmlEncode(tbxContent.Text);
                if (Session["UserInfo"] != null)
                {
                    Users userInfo = Session["UserInfo"] as Users;
                    drNew["EditorID"] = userInfo == null ? "" : userInfo.UserId;
                }
                drNew["EditorName"] = tbxEditor.Text;
                dt.Rows.Add(drNew);


                bool ret = ConvertDataTableXML.SaveSaleCargoNewsToXml(dt);
                string msg = ret == true ? "保存成功！" : "保存失败！详细原因请查看系统日志！";
                DAL.CommonConvert.ShowMessageBox(this.Page, msg);
                lblGridViewMsg.Text = msg;

                GridViewBind();//保存成功后重新绑定
            }
            catch (Exception ex)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "保存失败！");
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
    }
}