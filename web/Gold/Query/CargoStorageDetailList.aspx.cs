using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;
using System.Text;
using Gold.NCInvoke;
using System.Data;

namespace Gold.Query
{
    public partial class CargoStorageDetailList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GoodsSelect1.PostBack += this.GoodsSelect1_PostBack;

            if (!this.IsPostBack)
            {
                RadioButton_ByPrecise.Checked = true;
                RadioButton_ByPrecise_CheckedChanged(sender, new EventArgs());

                LoadNewPanelDropDownList();


                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"

                btnQuery_Click(sender, e);
            }
            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoList);//生成固定表头
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            ClearTipMsgLabel();
            GridViewBind();
        }

        /// <summary>
        /// 加载dropdownlist数据
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
            }
        }



        void GridViewBind()
        {
            try
            {
                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                string strStoreEqual = DropDownList_StoreEqual.SelectedItem.Value.ToString();
                string strNCEqual = DropDownList_NCEqual.SelectedItem.Value.ToString();
                int QueryMode = -1;
                if (RadioButton_ByPrecise.Checked)
                    QueryMode = 0;//精确查询
                if (RadioButton_ByLike.Checked)
                    QueryMode = 1;//模糊查询
                string binCode = tbxBinCode.Text.Trim();

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<proc_WebSelectCargoStorageDetail_Result> queryResult = context.proc_WebSelectCargoStorageDetail(whCode, cargoCode, cargoName, modelName, specName, "", QueryMode, binCode).ToList<proc_WebSelectCargoStorageDetail_Result>();

                    //对查询结果附件用友库存信息
                    if (chkQueryNC.Checked) 
                    {
                        AppendNCData(ref queryResult);
                    }

                    //根据条件筛选仓库和区位存量是否一致
                    if (strStoreEqual != "")
                    {
                        if (strStoreEqual == "1") //一致
                        {
                            queryResult = (from r in queryResult where (r.Number != null) && (r.CargoStockCount == (double)r.Number) select r).ToList<proc_WebSelectCargoStorageDetail_Result>();
                        }
                        else if (strStoreEqual == "0") //不一致
                        {
                            queryResult = (from r in queryResult where (r.CargoStockCount != (double)(r.Number == null ? 0 : r.Number.Value)) select r).ToList<proc_WebSelectCargoStorageDetail_Result>();
                        }
                    }

                    //根据条件筛选用友与RFID仓库存量是否一致
                    if (strNCEqual != "")
                    {
                        if (strNCEqual == "1") //一致
                        {
                            queryResult = (from r in queryResult where (r.CargoStockCount == (double)r.NCCargoStockCount) select r).ToList<proc_WebSelectCargoStorageDetail_Result>();
                        }
                        else if (strNCEqual == "0") //不一致
                        {
                            queryResult = (from r in queryResult where (r.CargoStockCount != (double)r.NCCargoStockCount) select r).ToList<proc_WebSelectCargoStorageDetail_Result>();
                        }
                    }

                    string sortExpression = gv_CargoList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    #region 记录本次查询的层位标签ID供亮灯和关灯使用
                    List<string> BinTagIDList = new List<string>();
                    foreach (proc_WebSelectCargoStorageDetail_Result p in queryResult)
                    {
                        if (string.IsNullOrEmpty(p.BinTagID) == false)
                        {
                            if (BinTagIDList.Contains(p.BinTagID) == false)
                                BinTagIDList.Add(p.BinTagID);
                        }
                    }
                    ViewState["BinTagIDList"] = BinTagIDList;
                    #endregion

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

            ClearTipMsgLabel();
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

            ClearTipMsgLabel();
            GridViewBind();
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


        GridViewRow _preRow = null;
        int _rowSpan = 1;

        double _sumNCStockNum;
        int _sumWhStockNum;
        int _sumNumCurrent;

        //计算合计数量
        protected void gv_CargoList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                #region 设置行中的按钮

                Label lblRowBinTagStatus = e.Row.FindControl("lblRowBinTagStatus") as Label;
                Label lblRowBinTagStatusName = e.Row.FindControl("lblRowBinTagStatusName") as Label;
                if (lblRowBinTagStatus != null && lblRowBinTagStatusName != null)
                {
                    lblRowBinTagStatusName.Text = GetBinTagStatusName(lblRowBinTagStatus.Text.Trim());
                }

                //给Button加上客户端javascript函数
                Label lblCargoCode = e.Row.FindControl("lblCargoCode") as Label;
                Label lblBinCode = e.Row.FindControl("lblBinCode") as Label;
                Label lblWHCode = e.Row.FindControl("lblWHCode") as Label;
                Label lblNumber = e.Row.FindControl("lblNumber") as Label;
                Button btnRowTagTestAlarmStart = e.Row.FindControl("btnRowTagTestAlarmStart") as Button;
                Button btnRowTagTestAlarmStop = e.Row.FindControl("btnRowTagTestAlarmStop") as Button;
                Label lblRowTagTestShortMsg = e.Row.FindControl("lblRowTagTestShortMsg") as Label;
                string divID = "waitDiv_X" + lblCargoCode.Text + lblWHCode.Text + lblBinCode.Text + lblNumber.Text;
                btnRowTagTestAlarmStart.OnClientClick = "showWaitDiv('" + divID + "');clearLabelText('" + lblRowTagTestShortMsg.ClientID + "');";//使用js显示进度并清除单元格中的消息提示文字
                btnRowTagTestAlarmStop.OnClientClick = "showWaitDiv('" + divID + "');clearLabelText('" + lblRowTagTestShortMsg.ClientID + "');";

                if (lblBinCode != null && lblBinCode.Text.Trim() != "")
                {
                    btnRowTagTestAlarmStart.Visible = true;
                    btnRowTagTestAlarmStop.Visible = true;
                }
                else
                {
                    btnRowTagTestAlarmStart.Visible = false;
                    btnRowTagTestAlarmStop.Visible = false;
                }

                #endregion

                double ncStockNum;
                int whStockNum;
                int sumCurrent;

                double.TryParse(e.Row.Cells[7].Text, out ncStockNum);
                int.TryParse(e.Row.Cells[8].Text, out whStockNum);
                int.TryParse(e.Row.Cells[10].Text, out sumCurrent);
                _sumNCStockNum += ncStockNum;
                _sumNumCurrent += sumCurrent;
                _sumWhStockNum += whStockNum;


                //首行，赋值后返回
                if (_preRow == null)
                {
                    _preRow = e.Row;
                    return;
                }

                //商品编码仓库编码 与前行相同，合并第0~7列 （商品编码，商品名称，型号，规格，单位，仓库编码，仓库名称，仓库存量）
                //注意在获取e.Row.Cells[0].Text时要注意该单元格必须是BoundField，不能是TemplateField
                if (e.Row.Cells[1].Text == _preRow.Cells[1].Text && e.Row.Cells[6].Text == _preRow.Cells[6].Text)
                {
                    _preRow.Cells[0].RowSpan = ++_rowSpan;
                    _preRow.Cells[1].RowSpan = _rowSpan;
                    _preRow.Cells[2].RowSpan = _rowSpan;
                    _preRow.Cells[3].RowSpan = _rowSpan;
                    _preRow.Cells[4].RowSpan = _rowSpan;
                    _preRow.Cells[5].RowSpan = _rowSpan;
                    _preRow.Cells[6].RowSpan = _rowSpan;
                    _preRow.Cells[7].RowSpan = _rowSpan;
                    _preRow.Cells[8].RowSpan = _rowSpan;

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
                }
                else   //商品编码不一样，重新开始
                {
                    _rowSpan = 1;
                    _preRow = e.Row;
                }
            }


            // 合计 要显示合计行时必须设置GridView的ShowFooter属性
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "合计";
                e.Row.Cells[0].ColumnSpan = 7;//8;
                e.Row.Cells[0].Attributes.Add("align", "center");
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;
                //e.Row.Cells[7].Visible = false;

                e.Row.Cells[7].Text = _sumNCStockNum.ToString();
                e.Row.Cells[7].Attributes.Add("align", "right");

                e.Row.Cells[8].Text = _sumWhStockNum.ToString();
                e.Row.Cells[8].Attributes.Add("align", "right");

                //e.Row.Cells[8].Visible = false;

                e.Row.Cells[10].Text = _sumNumCurrent.ToString();
                e.Row.Cells[10].Attributes.Add("align", "right");

                
                //e.Row.Cells[13].Visible = false;


            }
        }

        protected void btnGoToPrintAndExportPage_Click(object sender, EventArgs e)
        {
            //在url传递之前需要Server.UrlEncode编码（若不编码，则url参数中的加号会变为空格），但是接受时不用解码（不需要Server.UrlDecode），asp.net自动解码
            string cargoCode = Server.UrlEncode(tbxCargoCode.Text.Trim());
            string cargoName = Server.UrlEncode(tbxCargoName.Text.Trim());
            string modelName = Server.UrlEncode(DropDownList_CargoModel.SelectedItem.Text.Trim());
            string specName = Server.UrlEncode(DropDownList_CargoSpec.SelectedItem.Text.Trim());
            string whName = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Text.Trim());
            string whCode = Server.UrlEncode(DropDownList_WHCode.SelectedItem.Value.Trim());
            int QueryMode = -1;
            if (RadioButton_ByPrecise.Checked)
                QueryMode = 0;//精确查询
            if (RadioButton_ByLike.Checked)
                QueryMode = 1;//模糊查询
            string binCode = Server.UrlEncode(tbxBinCode.Text.Trim());


            string sortExpression = gv_CargoList.Attributes["sortExpression"];
            string sortDirection = gv_CargoList.Attributes["sortDirection"];

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/CargoStorageDetailListRpt.aspx?");
            url.Append("cargoCode=");
            url.Append(cargoCode);
            url.Append("&cargoName=");
            url.Append(cargoName);
            url.Append("&modelName=");
            url.Append(modelName);
            url.Append("&specName=");
            url.Append(specName);
            url.Append("&whName=");
            url.Append(whName);
            url.Append("&whCode=");
            url.Append(whCode);
            url.Append("&QueryMode=");
            url.Append(QueryMode.ToString());
            url.Append("&binCode=");
            url.Append(binCode);
            url.Append("&sortExpression=");
            url.Append(sortExpression);
            url.Append("&sortDirection=");
            url.Append(sortDirection);
            Response.Redirect(url.ToString());
        }

        protected void gv_CargoList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView senderGrid = sender as GridView;

            if (e.CommandName == "TagAlarmStart" || e.CommandName == "TagAlarmStop")
            {
                string shortMsg = "";
                string detailMsg = "";
                string binTagID = "";
                bool result = false;

                GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                Label lblRowBinTagID = drv == null ? null : drv.FindControl("lblRowBinTagID") as Label;
                Label lblRowTagTestShortMsg = drv == null ? null : drv.FindControl("lblRowTagTestShortMsg") as Label;//非编辑状态下 显示标签编码的控件


                //先清空提示
                lblRowTagTestShortMsg.Text = "";
                lblGridViewMsg.Text = "";


                if (lblRowBinTagID == null)
                {
                    lblGridViewMsg.Text = "获取列表中层位标签标签信息失败！";
                    shortMsg = lblGridViewMsg.Text;
                    return;
                }
                else
                {
                    if (senderGrid.EditIndex == -1 && lblRowBinTagID != null)//非编辑态下获取标签编码
                        binTagID = lblRowBinTagID.Text.Trim();
                }

                if (e.CommandName == "TagAlarmStart")
                {
                    result = StartOrStopAlarm(binTagID, true, out shortMsg, out detailMsg);
                }
                else if (e.CommandName == "TagAlarmStop")
                {
                    result = StartOrStopAlarm(binTagID, false, out shortMsg, out detailMsg);
                }

                lblRowTagTestShortMsg.Text = shortMsg;
                lblGridViewMsg.Text = detailMsg;

                //gv_BinList.DataBind();//亮灯和关灯后手持机服务会改标签数据库字段状态，所以这里刷新数据源
                GridViewBind();
            }

        }

        /// <summary>
        /// 标签报警测试
        /// </summary>
        /// <param name="binTagID">标签编码</param>
        /// <param name="IsStartAlarm">true-开始报警，false-停止报警</param>
        /// <param name="shortMsg">调用报警函数简短消息提示</param>
        /// <param name="detailMsg">调用报警函数详细消息提示</param>
        /// <returns></returns>
        private bool StartOrStopAlarm(string binTagID, bool IsStartAlarm, out string shortMsg, out string detailMsg)
        {
            try
            {
                int BinTagLightAlartCount = 3;//层位标签报警测试时亮灯次数
                int BinTagSoundAlartCount = 3;//层位标签报警测试时鸣音次数
                bool ServiceResult = false;
                if (string.IsNullOrEmpty(binTagID))
                {
                    shortMsg = "标签编码不能为空";
                    detailMsg = "用于报警测试的标签编码不能为空！请编辑标签编码后再进行报警测试！";
                }

                //从数据库数据字典读取亮点次数
                BinTagLightAlartCount = DAL.DbCommonMethod.GetTagFirstLightCount();

                //从数据库数据字典读取鸣音次数
                BinTagSoundAlartCount = DAL.DbCommonMethod.GetTagFirstSoundCount();

                ServiceReference_DeviceService.DeviceServiceClient client = new ServiceReference_DeviceService.DeviceServiceClient();
                client.Open();
                if (IsStartAlarm)
                {
                    //调用服务端标签报警函数
                    ServiceResult = client.TagControl(binTagID, BinTagLightAlartCount, BinTagSoundAlartCount);
                }
                else
                {
                    //调用服务端标签报警函数
                    ServiceResult = client.TagControl(binTagID, 0, 0);
                }
                client.Close();

                if (ServiceResult)
                {
                    shortMsg = IsStartAlarm ? "报警已开启" : "报警已停止";
                    detailMsg = "标签[" + binTagID + "]已" + (IsStartAlarm ? "开启报警！" : "停止报警");
                    return true;
                }
                else
                {
                    shortMsg = IsStartAlarm ? "报警开启失败" : "报警停止失败";
                    detailMsg = "标签[" + binTagID + "]" + (IsStartAlarm ? "报警开启失败！" : "报警停止失败！");
                    return false;
                }

            }
            catch (Exception ex)
            {
                shortMsg = IsStartAlarm ? "报警开启失败" : "报警停止失败";
                detailMsg = "标签[" + binTagID + "]" + (IsStartAlarm ? "报警开启失败！" : "报警停止失败！") + "。详细原因：" + LogHelper.GetExceptionMsg(ex);
                return false;
            }
        }

        /// <summary>
        /// 标签报警测试
        /// </summary>
        /// <param name="binTagID">标签编码</param>
        /// <param name="IsStartAlarm">true-开始报警，false-停止报警</param>
        /// <param name="shortMsg">调用报警函数简短消息提示</param>
        /// <param name="detailMsg">调用报警函数详细消息提示</param>
        /// <returns></returns>
        private bool StartOrStopAlarm(string[] binTagID, bool IsStartAlarm, out string shortMsg)
        {
            shortMsg = "";
            try
            {
                int BinTagLightAlartCount = 3;//层位标签报警测试时亮灯次数
                int BinTagSoundAlartCount = 3;//层位标签报警测试时亮灯次数

                //从数据库数据字典读取亮点次数
                BinTagLightAlartCount = DAL.DbCommonMethod.GetTagFirstLightCount();

                //从数据库数据字典读取鸣音次数
                BinTagSoundAlartCount = DAL.DbCommonMethod.GetTagFirstSoundCount();

                ServiceReference_DeviceService.DeviceServiceClient client = new ServiceReference_DeviceService.DeviceServiceClient();
                client.Open();

                if (IsStartAlarm)
                {
                    //调用服务端标签报警函数
                    //同时发送多个标签控制命令,函数立即返回
                    client.TagControlAsyn(binTagID, BinTagLightAlartCount, BinTagSoundAlartCount);
                }
                else
                {
                    client.TagControlAsyn(binTagID, 0, 0);
                }

                return true;
            }
            catch (Exception ex)
            {
                shortMsg = "报警失败!" + Utility.LogHelper.GetExceptionMsg(ex);
                return false;
            }
        }

        /// <summary>
        /// 将GridView行中标签类型码转换为标签类型名称
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        string GetBinTagStatusName(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return "";
            try
            {
                EnumData.BinTagStatusEnum currentEnum = (EnumData.BinTagStatusEnum)Enum.Parse(typeof(EnumData.BinTagStatusEnum), enumValue);
                return Gold.Utility.EnumData.GetEnumDesc(currentEnum);
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetBinTagStatusName", ex);
                return "";
            }
        }



        void ClearTipMsgLabel()
        {
            //lblCheckMsg.Text = "";
            //lblAddMsg.Text = "";
            lblGridViewMsg.Text = "";
        }



        #region 商品选择

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            this.popWindow.Hide();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            return;
        }

        protected void GoodsSelect1_PostBack(object sender, EventArgs e)
        {
            if (!GoodsSelect1.ShowPop)
            {
                this.popWindow.Show();
                return;
            }
        }

        //商品选择返回值
        protected void GoodsSelect1_GetCargoSelect(object sender, EventArgs e)
        {
            this.popWindow.Hide();

            StringBuilder strCargoCodeList = new StringBuilder();

            for (int i = 0; i < GoodsSelect1.ListSelectedCargo.Count; i++)
            {
                string[] goodSelect = GoodsSelect1.ListSelectedCargo[i];
                string cargoCode = goodSelect[0];

                if (strCargoCodeList.Length > 0)
                    strCargoCodeList.Append(",");
                if (string.IsNullOrEmpty(cargoCode) == false)
                {
                    strCargoCodeList.Append("'");
                    strCargoCodeList.Append(cargoCode);
                    strCargoCodeList.Append("'");
                }
            }

            if (tbxCargoCode.Text.Trim() != "")
            {
                if (strCargoCodeList.Length > 0)
                    strCargoCodeList = strCargoCodeList.Insert(0, ",");
                tbxCargoCode.Text += strCargoCodeList.ToString();
            }
            else
            {
                tbxCargoCode.Text = strCargoCodeList.ToString();
            }


            //调用查询
            btnQuery_Click(sender, e);
        }



        protected void BtnSelectCargo_Click(object sender, EventArgs e)
        {
            tbxCargoCode.Text = "";
            tbxCargoName.Text = "";

            string cargoCode = tbxCargoCode.Text;
            string cargoName = tbxCargoName.Text;
            string[] cargoCondition = new string[] { cargoCode, cargoName };
            GoodsSelect1.CargoQueryCondition = cargoCondition;
            GoodsSelect1.DataBindForQuery();
            this.popWindow.Show();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        //累加查询
        protected void BtnAddSelectCargo_Click(object sender, EventArgs e)
        {
            //tbxCargoCode.Text = "";
            tbxCargoName.Text = "";

            string cargoCode = "";//tbxCargoCode.Text;
            string cargoName = tbxCargoName.Text;
            string[] cargoCondition = new string[] { cargoCode, cargoName };
            GoodsSelect1.CargoQueryCondition = cargoCondition;
            GoodsSelect1.DataBindForQuery();
            this.popWindow.Show();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }
        #endregion

        //精确查询
        protected void RadioButton_ByPrecise_CheckedChanged(object sender, EventArgs e)
        {
            //tbxCargoCode.Enabled = false;
            tbxCargoCode.ReadOnly = true;
            tbxCargoCode.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            //tbxCargoName.Enabled = false;
            tbxCargoName.ReadOnly = true;
            tbxCargoName.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            BtnSelectCargo.Visible = true;
            BtnAddSelectCargo.Visible = true;
            tbxCargoCode.Text = "";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        //模糊查询
        protected void RadioButton_ByLike_CheckedChanged(object sender, EventArgs e)
        {
            //tbxCargoCode.Enabled = true;
            tbxCargoCode.ReadOnly = false;
            tbxCargoCode.BackColor = System.Drawing.Color.White;

            //tbxCargoName.Enabled = true;
            tbxCargoName.ReadOnly = false;
            tbxCargoName.BackColor = System.Drawing.Color.White;
            BtnSelectCargo.Visible = false;
            BtnAddSelectCargo.Visible = false;
            tbxCargoCode.Text = "";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        protected void btnStartAlarm_Click(object sender, EventArgs e)
        {
            ClearTipMsgLabel();
            SetMoreTagAlarm(true);

            GridViewBind();//显示亮灯状态
        }

        protected void btnStopAlarm_Click(object sender, EventArgs e)
        {
            ClearTipMsgLabel();
            SetMoreTagAlarm(false);

            GridViewBind();//显示亮灯状态
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openOrClose">true开启，false关闭</param>
        void SetMoreTagAlarm(bool openOrClose)
        {
            string openOrCloseName = openOrClose ? "开启" : "关闭";
            List<string> BinTagIDList = ViewState["BinTagIDList"] as List<string>;
            if (ViewState["BinTagIDList"] == null || BinTagIDList == null || BinTagIDList.Count == 0)
            {
                DAL.CommonConvert.ShowMessageBox(this.Page, "查询结果中无记录，无需对层位标签进行报警操作！");
                return;
            }
            else
            {
                string shortMsg = "";
                bool result = StartOrStopAlarm(BinTagIDList.ToArray(), openOrClose, out shortMsg);
                string displayMsg = result ? "标签报警" + openOrCloseName + "成功！[共" + openOrCloseName + BinTagIDList.Count.ToString() + "个标签]" : "标签" + openOrCloseName + "报警失败！";
                DAL.CommonConvert.ShowMessageBox(this.Page, displayMsg);

                displayMsg = result ? "标签报警" + openOrCloseName + "成功！[共" + openOrCloseName + BinTagIDList.Count.ToString() + "个标签]" : "标签" + openOrCloseName + "报警失败！详细信息：" + shortMsg;
                this.lblGridViewMsg.Text = displayMsg;
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
                        lbtn.ID = "LinkButtonTitle" + (i++).ToString();
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

        #region 仿MessageBox

        /// <summary>
        /// 显示简单消息(例如"保存成功")
        /// </summary>
        /// <param name="msg">提示信息</param>
        void ShowMessageBox(string msg)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = false;//当只显示简单信息时，“详细信息栏”不显示
            lblMessageContentException.Visible = false;
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 显示提示信息和详细的异常信息（例如“保存失败！”,"详细信息：XXX"）
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="ex">异常信息</param>
        void ShowMessageBox(string msg, Exception ex)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = true;//当显示异常信息时，“详细信息栏”进行显示
            lblMessageContentException.Visible = true;
            string exMsg = Utility.LogHelper.GetExceptionMsg(ex);
            lblMessageContentException.Text = exMsg;

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 在服务端关闭提示信息(此方法一般不要用，关闭时用弹出div的javascript关闭会性能更好)
        /// </summary>
        void CloseMessageBox()
        {
            lblMessageContent.Text = "";
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Hide();//隐藏div
        }


        protected void showModalPopupServerOperatorButton_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Show();
        }
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Hide();
        }

        #endregion

        #region 用友NC数据导入
        protected void AppendNCData(ref List<proc_WebSelectCargoStorageDetail_Result> list)
        {
            try
            {
                #region 查询数据

                string typeArgs = "CargoInventory";
                string conditionArgs = "20101";//参数是仓库编码//string.Empty;
                System.Data.DataTable dt = null;
                string queryMsg = "";
                string msg = "";

                BasicInfoInvoke BasicInfoInvokeObj = BasicInfoInvokeFactory.CreateInstance(typeArgs, conditionArgs);

                if (BasicInfoInvokeObj.GetNCDataJoinRFID(out dt, out queryMsg) == false)
                {
                    ShowMessageBox("查询用友系统信息失败！详细信息：" + queryMsg);
                    return;
                }
                else
                {
                    List<StockPositionAndVolume> NCList = GetModelFromDataTable(dt, out msg);
                    if (NCList == null || NCList.Count == 0)
                    {
                        ShowMessageBox("查询用友系统信息失败！详细信息：" + msg);
                    }
                    else
                    {
                        foreach (proc_WebSelectCargoStorageDetail_Result spav in list)
                        {
                            StockPositionAndVolume NCSpav = (from r in NCList where r.WHCode == spav.WHCode && r.CargoCode == spav.CargoCode select r).FirstOrDefault<StockPositionAndVolume>();
                            if (NCSpav != null)
                            {
                                spav.NCCargoStockCount = (decimal)NCSpav.CargoStockCount;//将从用户系统查询出的信息附件到RFID系统查询出的结果上。
                            }
                        }
                    }
                }
                #endregion


            }
            catch (Exception ex)
            {
                ShowMessageBox("查询用友库存信息失败！", ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public List<DAL.StockPositionAndVolume> GetModelFromDataTable(DataTable dt, out string msg)
        {
            List<DAL.StockPositionAndVolume> list = new List<DAL.StockPositionAndVolume>();
            msg = "";

            //接口协议文档中定义的字段
            Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
            /*
WHCode	仓库编码
WHName	仓库名称
CargoCode	商品编码
CargoName	商品名称
CargoSpec	规格
CargoModel	型号
CargoUnit	单位
ReleaseYear	发行年份
CargoStockCount	库存数量
CargoProperty	存货属性
ProjectName	项目名称
             
             */
            dataFieldNameDic.Add("WHCode", "仓库编码");
            dataFieldNameDic.Add("WHName", "仓库名称");
            dataFieldNameDic.Add("CargoCode", "商品编码");
            dataFieldNameDic.Add("CargoName", "商品名称");
            dataFieldNameDic.Add("CargoSpec", "规格");
            dataFieldNameDic.Add("CargoModel", "型号");
            dataFieldNameDic.Add("CargoUnit", "单位");
            dataFieldNameDic.Add("ReleaseYear", "发行年份");
            dataFieldNameDic.Add("CargoStockCount", "库存数量");
            dataFieldNameDic.Add("CargoProperty", "存货属性");
            dataFieldNameDic.Add("ProjectName", "项目名称");

            if (dt.Rows.Count == 0)
            {
                msg = "用友系统返回数据集中无数据！";
                return new List<StockPositionAndVolume>();
            }

            StringBuilder errorColName = new StringBuilder();
            //检查数据集中是否存在指定字段
            foreach (KeyValuePair<string, string> kvp in dataFieldNameDic)
            {
                if (dt.Columns.Contains(kvp.Key) == false)
                {
                    errorColName.Append(Environment.NewLine);
                    errorColName.Append(kvp.Value);
                    errorColName.Append("-");
                    errorColName.Append(kvp.Key);
                }
            }
            if (errorColName.Length > 0)
            {
                errorColName.Insert(0, "用友系统返回的数据集中未包含如下字段，不能进行有效解析！");
                msg = errorColName.ToString();
                return new List<StockPositionAndVolume>(); ;
            }

            //遍历数据集创建实体
            foreach (DataRow dr in dt.Rows)
            {
                StockPositionAndVolume newModel = new StockPositionAndVolume();

                newModel.WHCode = DataCheckHelper.GetCellString(dr["WHCode"]);
                newModel.CargoCode = DataCheckHelper.GetCellString(dr["CargoCode"]);
                newModel.CargoName = DataCheckHelper.GetCellString(dr["CargoName"]);
                newModel.CargoSpec = DataCheckHelper.GetCellString(dr["CargoSpec"]);
                newModel.CargoModel = DataCheckHelper.GetCellString(dr["CargoModel"]);
                newModel.CargoUnits = DataCheckHelper.GetCellString(dr["CargoUnit"]);
                newModel.ReleaseYear = DataCheckHelper.GetCellString(dr["ReleaseYear"]);
                newModel.CargoStockCount = DataCheckHelper.GetCellDouble(dr["CargoStockCount"]);
                newModel.LastUpdateTime = DateTime.Now;
                //newModel.Remark = DataCheckHelper.GetCellString(dr["ProjectName"]);

                List<StockPositionAndVolume> existWareHouse = (from r in list
                                                               where r.CargoCode == newModel.CargoCode && r.WHCode == newModel.WHCode
                                                               select r).ToList<StockPositionAndVolume>();
                if (existWareHouse == null || existWareHouse.Count == 0)//过滤重复数据
                    list.Add(newModel);
            }

            return list;
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