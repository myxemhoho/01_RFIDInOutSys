using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data.Objects.DataClasses;

namespace Gold.Controls
{
    public partial class BinCodeSelect : System.Web.UI.UserControl
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

        public event EventHandler<EventArgs> PostBack;
        public event EventHandler GetBinCodeSelect;
        private List<string[]> _listSelectedBinCode = new List<string[]>();

        public List<string[]> ListSelectedBinCode
        {
            get
            {
                return _listSelectedBinCode;
            }
        }

        public string CargoCode
        {
            set;
            get;
        }

        private void RiseEvent()
        {
            EventHandler<EventArgs> handler = this.PostBack;
            if (handler != null)
            {
                EventArgs e = new EventArgs();
                handler(this, e);
            }
        }       

        public void DataBind1()
        {
            this.grdBinCode.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            grdBinCode.DataBind();
            ViewState["ShowPop"] = null;
            RiseEvent();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdBinCode.Rows)
            {
                CheckBox ckb = (CheckBox)row.FindControl("chbSelect");
                if (ckb.Checked)
                {
                    string binCode = row.Cells[1].Text == "&nbsp;" ? "" : row.Cells[1].Text;
                    string WHNCode = row.Cells[5].Text == "&nbsp;" ? "" : row.Cells[5].Text;//仓库编码
                    string WHName = row.Cells[6].Text == "&nbsp;" ? "" : row.Cells[6].Text;//仓库名称                   
                    string[] binCodeSelect = new string[] { binCode, WHNCode, WHName };
                    _listSelectedBinCode.Add(binCodeSelect);
                }
            }
            if (GetBinCodeSelect != null)
                GetBinCodeSelect(this, EventArgs.Empty); 
            //RiseEvent();
            ViewState["ShowPop"] = true;

        }

        protected void edsBinCode_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            //商品编码
            if (CargoCode != null || ViewState["CargoCode"] != null)
            {
                if (CargoCode != null)
                {
                    ViewState["CargoCode"] = CargoCode;
                }
                else
                {
                    CargoCode = ViewState["CargoCode"].ToString();
                }
                e.Query = e.Query.Cast<VBinCode>().Where(o => o.CargoCode == CargoCode);                
            }

            //层位
            if (!string.IsNullOrEmpty(txtBinCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<VBinCode>().Where(o => o.BinCode.Contains(txtBinCode.Text.Trim()));
            }

            //仓库
            if (!string.IsNullOrEmpty(txtWHCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<VBinCode>().Where(o => o.WHCode.Contains(txtWHCode.Text.Trim()));
            }
        }

        protected void grdBinCode_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ViewState["ShowPop"] = null;//清空
            if (e.CommandName == "BinCodeSelect")
            {
                GridViewRow gvr = (e.CommandSource as Control).Parent.Parent as GridViewRow;
                grdBinCode.SelectedIndex = gvr.RowIndex;
            }

            RiseEvent();
        }        

        protected void edsBinCode_Selecting(object sender, EntityDataSourceSelectingEventArgs e)
        {
            if (!IsPostBack)
            {
                e.Cancel = true;
            }
        }

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdBinCode_PageIndexChanging(object sender, GridViewPageEventArgs e)
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


                grdBinCode.DataBind();//根据新页索引重新绑定数据

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
        protected void grdBinCode_DataBound(object sender, EventArgs e)
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
    }
}