using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.StockMove
{
    public partial class StockInMgr : System.Web.UI.Page
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
        }

        
        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void EntityDataSource1_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            //订单号
            if (!string.IsNullOrEmpty(txtOrderCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.FromOrderNo == txtOrderCode.Text.Trim());
                return;
            }

            //入库单号
            if (!string.IsNullOrEmpty(txtSICode.Text.Trim()))
            {
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.SICode == txtSICode.Text.Trim());
                return;
            }

            DateTime startDate = DateTime.Parse(txtStartTime.Text);
            DateTime endDate = DateTime.Parse(txtEndTime.Text);
            endDate = endDate.AddDays(1);
            e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.SIDate >= startDate && o.SIDate < endDate);

            //单据状态
            if (ddlSIStatus.SelectedIndex != 0)
            {
                int status=Convert.ToInt32(ddlSIStatus.SelectedValue.ToString());
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.SIStatus == status);
            }

            //单据类型
            if (ddlSIType.SelectedIndex != 0)
            {
                string sitype = ddlSIType.SelectedValue;
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.SIType == sitype);
            }

            //库管员
            if (ddlStoreKeeper.SelectedIndex != 0)
            {
                string storeKeeper = ddlStoreKeeper.SelectedValue;
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.StoreKeeper == storeKeeper);
            }
            
            //业务员
            if (ddlOperator.SelectedIndex != 0)
            {
                string operator1=ddlOperator.SelectedValue;
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.Operator == operator1);
            }    
       
            //收货仓库
            if (ddlWHCode.SelectedIndex != 0)
            {
                string whCode=ddlWHCode.SelectedValue;
                e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().Where(o => o.WHCode == whCode);
            }

            //按照日期倒序排列
            e.Query = e.Query.Cast<VSelectAllInCancleBillForInMgr>().OrderByDescending(o => o.SIDate).ThenByDescending(o=>o.SICode);
        }

        //排序
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

        #region 由代码获取解释
        //由入库单状态代码获取状态解释
        protected string GetSIStatus(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "SIStatus" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }

        //由入库单类型获取类型解释
        protected string GetSIType(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "SIType" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp.Count !=0 && tmp != null)
                {
                    reValue = tmp[0].ToString();
                }
            }

            if (reValue.ToString() == string.Empty)
            {
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.DataDict.Where(o => (o.Category == "SCType" && o.Code == code)).Select(o => o.Name).ToList();
                    if (tmp.Count != 0 && tmp != null)
                    {
                        reValue = tmp[0].ToString();
                    }
                }
            }

            return reValue;
        }

        //由订单类型获取类型解释
        protected string GetFromType(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "FromType" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp.Count != 0 && tmp != null)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }
                
        //由收发类别代码获取解释
        protected string GetInCategory(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "InCategory" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }

        //由源RFID订单类型获取解释
        protected string GetFromOrderType(object obj)
        {
            string reValue = string.Empty;
            if (obj == null)
            {
                return "";
            }

            string code = obj.ToString();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.Where(o => (o.Category == "FromOrderType" && o.Code == code)).Select(o => o.Name).ToList();
                if (tmp != null && tmp.Count != 0)
                {
                    reValue = tmp[0].ToString();
                }
            }
            return reValue;
        }
        #endregion

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

        //新建入库单
        protected void lbtnNewIn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/StockMove/StockInReg.aspx");
        }        

        //撤销操作
        protected void lbtStockCancel_Click(object sender, EventArgs e)
        {
            try
            {
                bool isCheckedCancel = false;//保存是否有选择需要撤销的单据行项目
                int rowCount = 0;

                foreach (GridViewRow grow in GridView1.Rows)
                {
                    CheckBox chkSelect = grow.Cells[0].Controls[1] as CheckBox;
                    if (chkSelect.Checked)
                    {
                        rowCount += 1;//计算被选中的行数，撤销单只能针对一个行项目进行撤销

                        string siCode = ((grow.Cells[1].Controls[1]) as LinkButton).Text;

                        if (siCode.Substring(0, 1) == "X")
                        {
                            DAL.CommonConvert.ShowMessageBox(this.Page, "只有入库单才能撤销！撤销单不能被再次撤销！");
                            return;
                        }

                        using (var edm = new Gold.DAL.GoldEntities())
                        {
                            var tmp = edm.StockIn.Where(o => o.SICode == siCode).ToList();
                            if (tmp != null)
                            {
                                //入库单，且状态为撤销中或已撤销，则不允许撤销
                                if (Convert.ToInt32(tmp[0].SIStatus) == 1 || Convert.ToInt32(tmp[0].SIStatus) == 2 
                                    || Convert.ToInt32(tmp[0].SIStatus) == 5 || Convert.ToInt32(tmp[0].SIStatus) == 0)
                                {
                                    isCheckedCancel = true;
                                }
                            }
                        }
                    }
                }

                if (rowCount == 0)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "请选择要撤销的入库单！");
                    return;
                }
                else if (rowCount > 1)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "每次只能针对一个单据进行撤销！");
                    return;
                }
                if (!isCheckedCancel)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "选中的单据不能被撤销！");
                    return;
                }

                foreach (GridViewRow grow in GridView1.Rows)
                {
                    CheckBox chkSelect = grow.Cells[0].Controls[1] as CheckBox;
                    if (chkSelect.Checked)
                    {
                        string siCode = ((grow.Cells[1].Controls[1]) as LinkButton).Text;

                        using (var edm = new Gold.DAL.GoldEntities())
                        {
                            var tmp = edm.StockIn.Where(o => o.SICode == siCode).ToList();
                            if (tmp != null)
                            {
                                //入库单，且状态为已提交
                                if (tmp[0].SIStatus == 2 && siCode.Substring(0, 1) == "R")
                                {
                                    bool isHandleStart = false;//手持机是否开始操作
                                    foreach (StockDetail sd in tmp[0].StockDetail)
                                    {
                                        //判断手持机是否已经开始操作，如果已经开始，则设置isHandleStart为true
                                        if (sd.CargoStatus == 1)
                                        {
                                            isHandleStart = true;
                                            break;
                                        }
                                    }
                                    if (isHandleStart)//手持机已经执行，则复制一份该入库单，用户可选择针对某个行项目撤销
                                    {
                                        DAL.CommonConvert.ShowMessageBox(this.Page, "此入库单" + tmp[0].SICode + "手持机已开始作业！请继续把此单完成后，再进行撤销！");
                                        return;

                                        //调用撤销方法
                                        //Response.Redirect("~/StockMove/StockInCancel.aspx?sourceCode=" + tmp[0].SICode + "&sourceType=stockcancel");                                   
                                    }
                                    else//如果手持机流程尚未执行，则将改单直接撤销，状态为：已撤销
                                    {
                                        //因为提交时，会报警。故撤销时，需要把报警关掉
                                        if (StockCancleByNoHandle(tmp[0]))
                                        {
                                            try
                                            {
                                                List<string> binTagIDs = new List<string>();
                                                foreach (StockDetail detail in tmp[0].StockDetail)
                                                {
                                                    //获取标签ID
                                                    string bin = detail.BinCode.Substring(0, 2);
                                                    var tmp2 = edm.StorageBin.Where(o => o.BinCode == bin).Select(o => o.BinTagID).ToList();
                                                    if (tmp2.Count != 0)
                                                    {
                                                        string binTag = tmp2[0].ToString();
                                                        if (!binTagIDs.Contains(binTag))
                                                        {
                                                            binTagIDs.Add(binTag);
                                                        }
                                                    }
                                                }

                                                int BinTagLightAlartCount = 0;//层位标签报警测试时亮灯次数
                                                int BinTagSoundAlartCount = 0;//层位标签报警测试时亮灯次数
                                                ServiceReference_DeviceService.DeviceServiceClient client = new ServiceReference_DeviceService.DeviceServiceClient();
                                                client.Open();

                                                //调用服务端标签报警函数 //同时发送多个标签控制命令,函数立即返回
                                                client.TagControlAsyn(binTagIDs.ToArray(), BinTagLightAlartCount, BinTagSoundAlartCount);

                                            }
                                            catch(Exception excption)
                                            {
                                                string msgAlarm = Utility.LogHelper.GetExceptionMsg(excption);
                                                msgAlarm = msgAlarm.Replace("\r\n", "");
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "报警失败!" + msgAlarm);
                                                return;
                                            }

                                            GridView1.DataBind();
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "撤销成功！手持机未开始作业，所选入库单已经被撤销。请重新导入修改后的订单！");
                                            return;
                                        }
                                        else
                                        {
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "撤销入库单" + tmp[0].SICode + "失败!");
                                            return;
                                        }
                                    }
                                }
                                //入库单，且为已完成
                                else if (tmp[0].SIStatus == 5 && siCode.Substring(0, 1) == "R")
                                {
                                    //调用撤销方法
                                    Response.Redirect("~/StockMove/StockInCancel.aspx?sourceCode=" + tmp[0].SICode + "&sourceType=stockcancel");
                                }
                                else if ((tmp[0].SIStatus == 1 || tmp[0].SIStatus == 0) && siCode.Substring(0, 1) == "R")//已保存的单据，可直接撤销
                                {
                                    if (StockCancleByNoHandle(tmp[0]))
                                    {
                                        GridView1.DataBind();
                                        DAL.CommonConvert.ShowMessageBox(this.Page, "撤销成功！手持机未开始作业，所选入库单已经被撤销。请重新导入修改后的订单！");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "撤销出现异常！" + msg);
            }
        }

        //多入库单提交
        protected void lbtnSummit_Click(object sender, EventArgs e)
        {            
            try
            {
                lbtnSummit.Enabled = false;

                bool isChecked = false;//保存是否有选择需要提交的单据行项目
                string shortMsg = "";
                //string[] binTagIDs = new string[100];
                int rowCount = 0;
                //int i = 0;
                List<string> binTagIDs = new List<string>();
                string siCodes = string.Empty;

                using (var edm = new Gold.DAL.GoldEntities())
                {
                    foreach (GridViewRow grow in GridView1.Rows)
                    {
                        CheckBox chkSelect = grow.Cells[0].Controls[1] as CheckBox;
                        if (chkSelect.Checked)
                        {
                            rowCount += 1;
                            string siCode = ((grow.Cells[1].Controls[1]) as LinkButton).Text;
                            if (siCode.Substring(0, 1) == "R")
                            {
                                var tmp = edm.StockIn.Where(o => o.SICode == siCode).ToList();
                                if (tmp != null)
                                {
                                    //入库单，且状态为已保存
                                    if (tmp[0].SIStatus == 1)
                                    {
                                        //判断值是否完整    
                                        if (tmp[0].SIDate == null)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择入库日期！单号：" + tmp[0].SICode);
                                            return;
                                        }

                                        if (tmp[0].StockDetail.Count == 0)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择要入库的商品！单号：" + tmp[0].SICode);
                                            return;
                                        }
                                        if (tmp[0].FromType.ToString() == string.Empty)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择订单类型！单号：" + tmp[0].SICode);
                                            return;
                                        }
                                        if (tmp[0].SIType.ToString() == string.Empty)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择单据类型！单号：" + tmp[0].SICode);
                                            return;
                                        }
                                        if (tmp[0].WHCode == null)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库！单号：" + tmp[0].SICode);
                                            return;
                                        }
                                        else if (tmp[0].WHCode.ToString() == string.Empty)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库！单号：" + tmp[0].SICode);
                                            return;
                                        }
                                        if (tmp[0].InCategory == null)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择收发类别！单号：" + tmp[0].SICode);
                                            return;
                                        }

                                        //更新入库单状态 已提交 
                                        tmp[0].SIStatus = 2;
                                        //设置提交人信息
                                        if (Session["UserInfo"] != null)
                                        {
                                            Users userInfo = (Users)Session["UserInfo"];
                                            tmp[0].RFIDSubmitPersonID = userInfo.UserId;
                                            tmp[0].RFIDSubmitPersonName = userInfo.UserName;
                                        }
                                        tmp[0].RFIDSubmitTime = System.DateTime.Now;
                                        edm.StockIn.ApplyCurrentValues(tmp[0]);

                                        foreach (StockDetail sd in tmp[0].StockDetail)
                                        {
                                            if (sd.BinCode == null || sd.InOutWHCode == null)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位！单号：" + sd.BillCode);
                                                return;
                                            }
                                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位！单号：" + sd.BillCode);
                                                return;
                                            }
                                            if (sd.NumCurrentPlan == 0)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！单号：" + sd.BillCode);
                                                return;
                                            }
                                            else if (sd.NumCurrentPlan > sd.NumOriginalPlan)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "应收数量不能大于订单数量！单号：" + sd.BillCode + "。商品：" + sd.CargoCode);
                                                return;
                                            }

                                            sd.RFIDSubmitTime = System.DateTime.Now;//提交时间

                                            //获取标签ID
                                            string bin = sd.BinCode.Substring(0, 2);
                                            var tmp2 = edm.StorageBin.Where(o => o.BinCode == bin).Select(o => o.BinTagID).ToList();
                                            if (tmp2.Count != 0)
                                            {
                                                string binTag = tmp2[0].ToString();
                                                if (!binTagIDs.Contains(binTag))//过滤重复的标签
                                                {
                                                    binTagIDs.Add(binTag);
                                                }
                                            }

                                            edm.StockDetail.ApplyCurrentValues(sd);
                                        }
                                        isChecked = true;
                                        siCodes += tmp[0].SICode + ",";

                                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                                        //当前的应收数量=Sum（所有detail应收数量）-撤销单的应收数量-手持机尚未开始操作被撤销的入库单应收数量
                                        if (tmp[0].FromOrderNo != null && tmp[0].FromOrderType != null)
                                        {
                                            var edmOrder = new Gold.DAL.GoldEntities();
                                            string fromOrderNo = tmp[0].FromOrderNo;
                                            string sicode = tmp[0].SICode;
                                            var tmpStockIns = edmOrder.StockIn.Where(o => (o.FromOrderNo == fromOrderNo && o.SICode != sicode)).ToList();
                                            if (tmp[0].FromOrderType == "01")//销售订单
                                            {
                                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == fromOrderNo);
                                                if (tmpSalesOrder != null)
                                                {
                                                    //查看保存的入库单是否把订单全部转或者是部分转
                                                    bool isAllSave = true;

                                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                                    {
                                                        foreach (StockDetail detail in tmp[0].StockDetail)//当前入库单
                                                        {
                                                            //判断商品是否相等
                                                            if (detail.CargoCode == salesOrderDetail.CargoCode && detail.BillRowNumber == salesOrderDetail.DetailRowNumber)
                                                            {
                                                                //如果入库单的商品应收数量=订单数量，则该行项目状态为3（全部已转）
                                                                if (detail.NumCurrentPlan == salesOrderDetail.NumOriginalPlan)
                                                                {
                                                                    salesOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    //根据该订单之前生成的入库单应收数量，判断该订单是否已全部转成入库单                                                    
                                                                    double Num = detail.NumCurrentPlan;
                                                                    if (tmpStockIns != null)
                                                                    {
                                                                        for (int j = 0; j < tmpStockIns.Count; j++)
                                                                        {
                                                                            StockIn stockin = tmpStockIns[j];
                                                                            foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                            {
                                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                {
                                                                                    Num += oldDetail.NumCurrentPlan;
                                                                                    break;
                                                                                }
                                                                            }

                                                                            //查找撤销单
                                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockin.SICode).ToList();
                                                                            if (tmpCancel.Count != 0)
                                                                            {
                                                                                foreach (StockDetail cancelDetail in tmpCancel[0].StockDetail)
                                                                                {
                                                                                    if (cancelDetail.CargoCode == detail.CargoCode && cancelDetail.BillRowNumber == detail.BillRowNumber)
                                                                                    {
                                                                                        Num -= cancelDetail.NumCurrentPlan;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                //如果手持机尚未开始操作，则需减去被撤销的部分
                                                                                if (stockin.SIStatus == 4)
                                                                                {
                                                                                    foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                                    {
                                                                                        if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                        {
                                                                                            Num -= oldDetail.NumCurrentPlan;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }


                                                                    if (Num == salesOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        salesOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    }
                                                                    else if (Num > salesOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        lbtnSummit.Enabled = true;
                                                                        DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + salesOrderDetail.CargoCode + ")的应收数量已超出范围，请重新填写应收数量！");
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        salesOrderDetail.DetailRowStatus = 4;//部分已转       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        edm.SalesOrderDetail.ApplyCurrentValues(salesOrderDetail);

                                                        //如果状态为部分已转或者是初始态
                                                        if (salesOrderDetail.DetailRowStatus == 4 || salesOrderDetail.DetailRowStatus == 2 || salesOrderDetail.DetailRowStatus == null)
                                                        {
                                                            isAllSave = false;
                                                        }
                                                    }

                                                    if (isAllSave)
                                                    {
                                                        tmpSalesOrder.OrderStatus = 3;//全部已转
                                                    }
                                                    else
                                                    {
                                                        tmpSalesOrder.OrderStatus = 4;//部分已转
                                                    }

                                                    if (Session["UserInfo"] != null)
                                                    {
                                                        Users userInfo = (Users)Session["UserInfo"];
                                                        tmpSalesOrder.RFIDActorID = userInfo.UserId;//类型不正确，需修改数据库字段类型
                                                        tmpSalesOrder.RFIDActorName = userInfo.UserName;

                                                        tmpSalesOrder.EditorID = userInfo.UserId;
                                                        tmpSalesOrder.EditorName = userInfo.UserName;
                                                    }
                                                    tmpSalesOrder.RFIDActorTime = System.DateTime.Now;
                                                    tmpSalesOrder.EditTime = System.DateTime.Now;
                                                    tmpSalesOrder.EditStatus = 0;//无人编辑

                                                    edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                                                }
                                            }
                                            else if (tmp[0].FromOrderType == "02")//采购订单
                                            {
                                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == fromOrderNo);
                                                if (tmpPurchaseOrder != null)
                                                {
                                                    //查看保存的入库单是否把订单全部转或者是部分转
                                                    bool isAllSave = true;

                                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                                    {
                                                        foreach (StockDetail detail in tmp[0].StockDetail)//当前入库单
                                                        {
                                                            //判断商品是否相等
                                                            if (detail.CargoCode == purchaseOrderDetail.CargoCode && detail.BillRowNumber == purchaseOrderDetail.DetailRowNumber)
                                                            {
                                                                //如果入库单的商品应收数量=订单数量，则该行项目状态为3（全部已转）
                                                                if (detail.NumCurrentPlan == purchaseOrderDetail.NumOriginalPlan)
                                                                {
                                                                    purchaseOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    //根据该订单之前生成的入库单应收数量，判断该订单是否已全部转成入库单                                                    
                                                                    double Num = detail.NumCurrentPlan;
                                                                    if (tmpStockIns != null)
                                                                    {
                                                                        for (int j = 0; j < tmpStockIns.Count; j++)
                                                                        {
                                                                            StockIn stockin = tmpStockIns[j];
                                                                            foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                            {
                                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                {
                                                                                    Num += oldDetail.NumCurrentPlan;
                                                                                    break;
                                                                                }
                                                                            }

                                                                            //查找撤销单
                                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockin.SICode).ToList();
                                                                            if (tmpCancel.Count != 0)
                                                                            {
                                                                                foreach (StockDetail cancelDetail in tmpCancel[0].StockDetail)
                                                                                {
                                                                                    if (cancelDetail.CargoCode == detail.CargoCode && cancelDetail.BillRowNumber == detail.BillRowNumber)
                                                                                    {
                                                                                        Num -= cancelDetail.NumCurrentPlan;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                //如果手持机尚未开始操作，则需减去被撤销的部分
                                                                                if (stockin.SIStatus == 4)
                                                                                {
                                                                                    foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                                    {
                                                                                        if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                        {
                                                                                            Num -= oldDetail.NumCurrentPlan;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (Num == purchaseOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        purchaseOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    }
                                                                    else if (Num > purchaseOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        lbtnSummit.Enabled = true;
                                                                        DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + purchaseOrderDetail.CargoCode + ")的应收数量已超出范围，请重新填写应收数量！");
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        purchaseOrderDetail.DetailRowStatus = 4;//部分已转       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        edm.PurchaseOrderDetail.ApplyCurrentValues(purchaseOrderDetail);

                                                        //如果状态为部分已转或者是初始态
                                                        if (purchaseOrderDetail.DetailRowStatus == 4 || purchaseOrderDetail.DetailRowStatus == 2 || purchaseOrderDetail.DetailRowStatus == null)
                                                        {
                                                            isAllSave = false;
                                                        }
                                                    }

                                                    if (isAllSave)
                                                    {
                                                        tmpPurchaseOrder.OrderStatus = 3;//全部已转
                                                    }
                                                    else
                                                    {
                                                        tmpPurchaseOrder.OrderStatus = 4;//部分已转
                                                    }

                                                    //设置RFID处理人信息及时间
                                                    if (Session["UserInfo"] != null)
                                                    {
                                                        Users userInfo = (Users)Session["UserInfo"];
                                                        tmpPurchaseOrder.RFIDActorID = userInfo.UserId;
                                                        tmpPurchaseOrder.RFIDActorName = userInfo.UserName;

                                                        tmpPurchaseOrder.EditorID = userInfo.UserId;
                                                        tmpPurchaseOrder.EditorName = userInfo.UserName;
                                                    }
                                                    tmpPurchaseOrder.RFIDActorTime = System.DateTime.Now;
                                                    tmpPurchaseOrder.EditTime = System.DateTime.Now;
                                                    tmpPurchaseOrder.EditStatus = 0;//无人编辑

                                                    edm.PurchaseOrder.ApplyCurrentValues(tmpPurchaseOrder);
                                                }
                                            }
                                            else if (tmp[0].FromOrderType == "03")//调拨订单
                                            {
                                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == fromOrderNo);
                                                if (tmpTransferOrder != null)
                                                {
                                                    //查看保存的入库单是否把订单全部转或者是部分转
                                                    bool isAllSave = true;

                                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                                    {
                                                        foreach (StockDetail detail in tmp[0].StockDetail)//当前入库单
                                                        {
                                                            //判断商品是否相等
                                                            if (detail.CargoCode == transferOrderDetail.CargoCode && detail.BillRowNumber == transferOrderDetail.DetailRowNumber)
                                                            {
                                                                //如果入库单的商品应收数量=订单数量，则该行项目状态为3（全部已转）
                                                                if (detail.NumCurrentPlan == transferOrderDetail.NumOriginalPlan)
                                                                {
                                                                    transferOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    //根据该订单之前生成的入库单应收数量，判断该订单是否已全部转成入库单                                                   
                                                                    double Num = detail.NumCurrentPlan;
                                                                    if (tmpStockIns != null)
                                                                    {
                                                                        for (int j = 0; j < tmpStockIns.Count; j++)
                                                                        {
                                                                            StockIn stockin = tmpStockIns[j];
                                                                            foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                            {
                                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                {
                                                                                    Num += oldDetail.NumCurrentPlan;
                                                                                    break;
                                                                                }
                                                                            }

                                                                            //查找撤销单
                                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockin.SICode).ToList();
                                                                            if (tmpCancel.Count != 0)
                                                                            {
                                                                                foreach (StockDetail cancelDetail in tmpCancel[0].StockDetail)
                                                                                {
                                                                                    if (cancelDetail.CargoCode == detail.CargoCode && cancelDetail.BillRowNumber == detail.BillRowNumber)
                                                                                    {
                                                                                        Num -= cancelDetail.NumCurrentPlan;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                //如果手持机尚未开始操作，则需减去被撤销的部分
                                                                                if (stockin.SIStatus == 4)
                                                                                {
                                                                                    foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                                    {
                                                                                        if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                        {
                                                                                            Num -= oldDetail.NumCurrentPlan;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (Num == transferOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        transferOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    }
                                                                    else if (Num > transferOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        lbtnSummit.Enabled = true;
                                                                        DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + transferOrderDetail.CargoCode + ")的应收数量已超出范围，请重新填写应收数量！");
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        transferOrderDetail.DetailRowStatus = 4;//部分已转       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        edm.TransferOrderDetail.ApplyCurrentValues(transferOrderDetail);

                                                        //如果状态为部分已转或者是初始态
                                                        if (transferOrderDetail.DetailRowStatus == 4 || transferOrderDetail.DetailRowStatus == 2 || transferOrderDetail.DetailRowStatus == null)
                                                        {
                                                            isAllSave = false;
                                                        }
                                                    }

                                                    if (isAllSave)
                                                    {
                                                        tmpTransferOrder.OrderStatus = 3;//全部已转
                                                    }
                                                    else
                                                    {
                                                        tmpTransferOrder.OrderStatus = 4;//部分已转
                                                    }

                                                    if (Session["UserInfo"] != null)
                                                    {
                                                        Users userInfo = (Users)Session["UserInfo"];
                                                        tmpTransferOrder.RFIDActorID = userInfo.UserId;
                                                        tmpTransferOrder.RFIDActorName = userInfo.UserName;

                                                        tmpTransferOrder.EditorID = userInfo.UserId;
                                                        tmpTransferOrder.EditorName = userInfo.UserName;
                                                    }
                                                    tmpTransferOrder.RFIDActorTime = System.DateTime.Now;
                                                    tmpTransferOrder.EditTime = System.DateTime.Now;
                                                    tmpTransferOrder.EditStatus = 0;//无人编辑

                                                    edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                                                }
                                            }
                                            else if (tmp[0].FromOrderType == "04")//转库单
                                            {
                                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == fromOrderNo);
                                                if (tmpShiftOrder != null)
                                                {
                                                    //查看保存的入库单是否把订单全部转或者是部分转
                                                    bool isAllSave = true;

                                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                                    {
                                                        foreach (StockDetail detail in tmp[0].StockDetail)//当前入库单
                                                        {
                                                            //判断商品是否相等
                                                            if (detail.CargoCode == shiftOrderDetail.CargoCode && detail.BillRowNumber == shiftOrderDetail.DetailRowNumber)
                                                            {
                                                                //如果入库单的商品应收数量=订单数量，则该行项目状态为3（全部已转）
                                                                if (detail.NumCurrentPlan == shiftOrderDetail.NumOriginalPlan)
                                                                {
                                                                    shiftOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    //根据该订单之前生成的入库单应收数量，判断该订单是否已全部转成入库单                                                    
                                                                    double Num = detail.NumCurrentPlan;
                                                                    if (tmpStockIns != null)
                                                                    {
                                                                        for (int j = 0; j < tmpStockIns.Count; j++)
                                                                        {
                                                                            StockIn stockin = tmpStockIns[j];
                                                                            foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                            {
                                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                {
                                                                                    Num += oldDetail.NumCurrentPlan;
                                                                                    break;
                                                                                }
                                                                            }

                                                                            //查找撤销单
                                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockin.SICode).ToList();
                                                                            if (tmpCancel.Count != 0)
                                                                            {
                                                                                foreach (StockDetail cancelDetail in tmpCancel[0].StockDetail)
                                                                                {
                                                                                    if (cancelDetail.CargoCode == detail.CargoCode && cancelDetail.BillRowNumber == detail.BillRowNumber)
                                                                                    {
                                                                                        Num -= cancelDetail.NumCurrentPlan;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                //如果手持机尚未开始操作，则需减去被撤销的部分
                                                                                if (stockin.SIStatus == 4)
                                                                                {
                                                                                    foreach (StockDetail oldDetail in stockin.StockDetail)
                                                                                    {
                                                                                        if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                                        {
                                                                                            Num -= oldDetail.NumCurrentPlan;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (Num == shiftOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        shiftOrderDetail.DetailRowStatus = 3;//全部已转
                                                                    }
                                                                    else if (Num > shiftOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        lbtnSummit.Enabled = true;
                                                                        DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + shiftOrderDetail.CargoCode + ")的应收数量已超出范围，请重新填写应收数量！");
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        shiftOrderDetail.DetailRowStatus = 4;//部分已转       
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        edm.ShiftOrderDetail.ApplyCurrentValues(shiftOrderDetail);

                                                        //如果状态为部分已转或者是初始态
                                                        if (shiftOrderDetail.DetailRowStatus == 4 || shiftOrderDetail.DetailRowStatus == 2 || shiftOrderDetail.DetailRowStatus == null)
                                                        {
                                                            isAllSave = false;
                                                        }
                                                    }

                                                    if (isAllSave)
                                                    {
                                                        tmpShiftOrder.OrderStatus = 3;//全部已转
                                                    }
                                                    else
                                                    {
                                                        tmpShiftOrder.OrderStatus = 4;//部分已转
                                                    }

                                                    if (Session["UserInfo"] != null)
                                                    {
                                                        Users userInfo = (Users)Session["UserInfo"];
                                                        tmpShiftOrder.RFIDActorID = userInfo.UserId;
                                                        tmpShiftOrder.RFIDActorName = userInfo.UserName;

                                                        tmpShiftOrder.EditorID = userInfo.UserId;
                                                        tmpShiftOrder.EditorName = userInfo.UserName;
                                                    }
                                                    tmpShiftOrder.RFIDActorTime = System.DateTime.Now;
                                                    tmpShiftOrder.EditTime = System.DateTime.Now;
                                                    tmpShiftOrder.EditStatus = 0;//无人编辑

                                                    edm.ShiftOrder.ApplyCurrentValues(tmpShiftOrder);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (siCode.Substring(0, 1) == "X")//撤销单
                            {
                                var tmp = edm.StockCancel.Where(o => o.SCCode == siCode).ToList();
                                if (tmp != null)
                                {
                                    //撤销单，且状态为已保存
                                    if (tmp[0].SCStatus == 1)
                                    {
                                        if (tmp[0].SCDate == null)
                                        {
                                            lbtnSummit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "请输入撤销日期！");
                                            return;
                                        }

                                        //更新入库单状态 已提交 
                                        tmp[0].SCStatus = 2;
                                        //设置提交人信息
                                        if (Session["UserInfo"] != null)
                                        {
                                            Users userInfo = (Users)Session["UserInfo"];
                                            tmp[0].RFIDSubmitPersonID = userInfo.UserId;
                                            tmp[0].RFIDSubmitPersonName = userInfo.UserName;
                                        }
                                        tmp[0].RFIDSubmitTime = System.DateTime.Now;
                                        edm.StockCancel.ApplyCurrentValues(tmp[0]);

                                        var tmpIn = edm.StockIn.SingleOrDefault(o => o.SICode == tmp[0].CancelBillCode);

                                        foreach (StockDetail sd in tmp[0].StockDetail)
                                        {
                                            if (sd.BinCode == null || sd.InOutWHCode == null)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和层位！单号：" + sd.BillCode);
                                                return;
                                            }
                                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和层位！单号：" + sd.BillCode);
                                                return;
                                            }
                                            if (sd.NumCurrentPlan == 0)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！单号：" + sd.BillCode);
                                                return;
                                            }
                                            else if (sd.NumCurrentPlan > sd.NumOriginalPlan)
                                            {
                                                lbtnSummit.Enabled = true;
                                                DAL.CommonConvert.ShowMessageBox(this.Page, "应收数量不能大于订单数量！单号：" + sd.BillCode + "。商品：" + sd.CargoCode);
                                                return;                                               
                                            }
                                            else
                                            {
                                                //根据商品和区位为查询条件，搜索当前仓库的库存数量
                                                var tmpNumCurrent = edm.CargoTag.Where(o => (o.CargoCode == sd.CargoCode) && (o.BinCode == sd.BinCode)).Select(o => o.Number).ToList();
                                                if (tmpNumCurrent.Count != 0)
                                                {
                                                    float numCurrent;
                                                    if (!string.IsNullOrEmpty(tmpNumCurrent[0].ToString()) && float.TryParse(tmpNumCurrent[0].ToString(), out numCurrent))
                                                    {
                                                        if (numCurrent < sd.NumCurrentPlan)
                                                        {
                                                            lbtnSummit.Enabled = true;
                                                            DAL.CommonConvert.ShowMessageBox(this.Page, "单号：" + sd.BillCode + "商品(" + sd.CargoCode + ")的当前库存量为：" + tmpNumCurrent[0].ToString() + "!应发数量不能大于库存数量！");
                                                            return;
                                                        }
                                                    }
                                                }
                                            }

                                            sd.RFIDSubmitTime = System.DateTime.Now;//提交至备货时间

                                            //获取标签ID
                                            string bin = sd.BinCode.Substring(0, 2);
                                            var tmp2 = edm.StorageBin.Where(o => o.BinCode == bin).Select(o => o.BinTagID).ToList();
                                            if (tmp2.Count != 0)
                                            {
                                                string binTag = tmp2[0].ToString();
                                                if (!binTagIDs.Contains(binTag))//过滤重复的标签
                                                {
                                                    binTagIDs.Add(binTag);
                                                }
                                            }

                                            if (tmpIn != null)
                                            {
                                                //如果是来源于订单，则需要修改订单状态和订单行项目状态
                                                if (tmpIn.FromOrderNo != null && tmpIn.FromOrderType != null)
                                                {
                                                    var edmOrder = new Gold.DAL.GoldEntities();                                                    
                                                    if (tmpIn.FromOrderType == "01")//销售订单
                                                    {                                                       
                                                        var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == tmpIn.FromOrderNo);
                                                        if (tmpSalesOrder != null)
                                                        {
                                                            foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                                            {
                                                                //判断商品是否相等
                                                                if (sd.CargoCode == salesOrderDetail.CargoCode && sd.BillRowNumber == salesOrderDetail.DetailRowNumber)
                                                                {
                                                                    //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                                                    if (sd.NumCurrentPlan == salesOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        salesOrderDetail.DetailRowStatus = 2;//初始态
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        salesOrderDetail.DetailRowStatus = 4;
                                                                        break;
                                                                    }
                                                                }
                                                                edm.SalesOrderDetail.ApplyCurrentValues(salesOrderDetail);
                                                            }

                                                            //查看保存的出库单是否把订单全部转或者是部分转
                                                            bool isAllSave = true;
                                                            foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                                            {
                                                                if (salesOrderDetail.DetailRowStatus != 2)
                                                                {
                                                                    isAllSave = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (isAllSave)
                                                            {
                                                                tmpSalesOrder.OrderStatus = 2;//初始态
                                                            }
                                                            else
                                                            {
                                                                tmpSalesOrder.OrderStatus = 4;//部分已转
                                                            }

                                                            edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                                                        }
                                                    }
                                                    else if (tmpIn.FromOrderType == "02")//采购订单
                                                    {
                                                        var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == tmpIn.FromOrderNo);
                                                        if (tmpPurchaseOrder != null)
                                                        {
                                                            foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                                            {
                                                                //判断商品是否相等
                                                                if (sd.CargoCode == purchaseOrderDetail.CargoCode && sd.BillRowNumber == purchaseOrderDetail.DetailRowNumber)
                                                                {
                                                                    //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                                                    if (sd.NumCurrentPlan == purchaseOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        purchaseOrderDetail.DetailRowStatus = 2;//初始态
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        purchaseOrderDetail.DetailRowStatus = 4;//部分已转  
                                                                        break;
                                                                    }
                                                                }
                                                                edm.PurchaseOrderDetail.ApplyCurrentValues(purchaseOrderDetail);
                                                            }

                                                            //查看保存的出库单是否把订单全部转或者是部分转
                                                            bool isAllSave = true;
                                                            foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                                            {
                                                                if (purchaseOrderDetail.DetailRowStatus != 2)
                                                                {
                                                                    isAllSave = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (isAllSave)
                                                            {
                                                                tmpPurchaseOrder.OrderStatus = 2;//初始态
                                                            }
                                                            else
                                                            {
                                                                tmpPurchaseOrder.OrderStatus = 4;//部分已转
                                                            }

                                                            edm.PurchaseOrder.ApplyCurrentValues(tmpPurchaseOrder);
                                                        }
                                                    }
                                                    else if (tmpIn.FromOrderType == "03")//调拨订单
                                                    {
                                                        var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == tmpIn.FromOrderNo);
                                                        if (tmpTransferOrder != null)
                                                        {
                                                            foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                                            {
                                                                //判断商品是否相等
                                                                if (sd.CargoCode == transferOrderDetail.CargoCode && sd.BillRowNumber == transferOrderDetail.DetailRowNumber)
                                                                {
                                                                    //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                                                    if (sd.NumCurrentPlan == transferOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        transferOrderDetail.DetailRowStatus = 2;//初始态
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        transferOrderDetail.DetailRowStatus = 4;//部分已转  
                                                                        break;
                                                                    }
                                                                }

                                                                edm.TransferOrderDetail.ApplyCurrentValues(transferOrderDetail);
                                                            }
                                                            //查看保存的出库单是否把订单全部转或者是部分转
                                                            bool isAllSave = true;
                                                            foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                                            {
                                                                if (transferOrderDetail.DetailRowStatus != 2)
                                                                {
                                                                    isAllSave = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (isAllSave)
                                                            {
                                                                tmpTransferOrder.OrderStatus = 2;//初始态
                                                            }
                                                            else
                                                            {
                                                                tmpTransferOrder.OrderStatus = 4;//部分已转
                                                            }

                                                            edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                                                        }
                                                    }
                                                    else if (tmpIn.FromOrderType == "04")//转库单
                                                    {
                                                        var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == tmpIn.FromOrderNo);
                                                        if (tmpShiftOrder != null)
                                                        {
                                                            foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                                            {
                                                                //判断商品是否相等
                                                                if (sd.CargoCode == shiftOrderDetail.CargoCode && sd.BillRowNumber == shiftOrderDetail.DetailRowNumber)
                                                                {
                                                                    //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                                                    if (sd.NumCurrentPlan == shiftOrderDetail.NumOriginalPlan)
                                                                    {
                                                                        shiftOrderDetail.DetailRowStatus = 2;//初始态
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        shiftOrderDetail.DetailRowStatus = 4;//部分已转  
                                                                        break;
                                                                    }
                                                                }

                                                                edm.ShiftOrderDetail.ApplyCurrentValues(shiftOrderDetail);
                                                            }

                                                            //查看保存的出库单是否把订单全部转或者是部分转
                                                            bool isAllSave = true;
                                                            foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                                            {
                                                                if (shiftOrderDetail.DetailRowStatus != 2)
                                                                {
                                                                    isAllSave = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (isAllSave)
                                                            {
                                                                tmpShiftOrder.OrderStatus = 2;//初始态
                                                            }
                                                            else
                                                            {
                                                                tmpShiftOrder.OrderStatus = 4;//部分已转
                                                            }

                                                            edm.ShiftOrder.ApplyCurrentValues(tmpShiftOrder);
                                                        }
                                                    }
                                                }
                                            }

                                            edm.StockDetail.ApplyCurrentValues(sd);
                                        }

                                        isChecked = true;
                                        siCodes += tmp[0].SCCode + ",";
                                    }
                                }
                            }
                        }
                    }

                    if (rowCount == 0)
                    {
                        lbtnSummit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择要提交的单据!");
                        return;
                    }

                    if (!isChecked)
                    {
                        lbtnSummit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有合适的需要提交的单据!");
                        return;
                    }

                    //调用报警函数
                    StartOrStopAlarm(binTagIDs.ToArray(), true, out shortMsg);
                    edm.SaveChanges();

                    GridView1.DataBind();

                    siCodes = siCodes.Substring(0, siCodes.Length - 1);
                    lbtnSummit.Enabled = true;
                    DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货成功!单号：" + siCodes);
                    return;
                }
            }
            catch (Exception ex)
            {
                lbtnSummit.Enabled = true;
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货出现异常！" + msg);
            }
        }

        //单击入库单号事件
        protected void lbtnSICode_Click(object sender, EventArgs e)
        {
            try
            {
                string cargoCode = (sender as LinkButton).Text;
                //获取单据类型
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.VSelectAllInCancleBillForInMgr.Where(o => o.SICode == cargoCode).ToList();
                    if (tmp != null)
                    {
                        if (tmp[0].SIType.ToString() == string.Empty)
                        {
                            //Response.Redirect("~/StockMove/StockInReg.aspx?sourceCode=" + cargoCode + "&sourceType=stockin");                            
                            string url="StockInReg.aspx?sourceCode=" + cargoCode + "&sourceType=stockin";
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(), "window.open('" + url + "') ", true);                            
                        }
                        //根据单据类型判断，是入库单还是撤销单，分别进入不同的界面
                        else if (tmp[0].SIType.Substring(0, 1) == "2")//入库单
                        {
                            //Response.Redirect("~/StockMove/StockInReg.aspx?sourceCode=" + cargoCode + "&sourceType=stockin",true);
                            string url="StockInReg.aspx?sourceCode=" + cargoCode + "&sourceType=stockin";                            
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(), "window.open('" + url + "') ", true);
                        }
                        else if (tmp[0].SIType.Substring(0, 1) == "3")//撤销单
                        {                            
                            //Response.Redirect("~/StockMove/StockInCancel.aspx?sourceCode=" + cargoCode + "&sourceType=stockin",true);
                            string url = "StockInCancel.aspx?sourceCode=" + cargoCode + "&sourceType=stockin";
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(), "window.open('" + url + "') ", true);  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "跳转页面出现异常！" + msg);
            }
        }

        //如果手持机流程尚未执行，则将改单直接撤销，状态为：已撤销
        protected bool StockCancleByNoHandle(StockIn si)
        {
            try
            {
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    //单据状态
                    si.SIStatus = 4;//已撤销
                    var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
                    if (tmp == null)
                    {                   
                        edm.StockIn.AddObject(si);
                    }
                    else
                    {
                        edm.StockIn.ApplyCurrentValues(si);
                    }

                    //订单状态
                    //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                    if (si.FromOrderNo != null && si.FromOrderType != null)
                    {
                        var edmOrder = new Gold.DAL.GoldEntities();
                        //var tmpStockIns = edmOrder.StockIn.Where(o => (o.FromOrderNo == si.FromOrderNo && o.SICode != si.SICode)).ToList();
                        if (si.FromOrderType == "01")//销售订单
                        {
                            var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                            if (tmpSalesOrder != null)
                            {                               
                                foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                {
                                    foreach (StockDetail detail in si.StockDetail)//当前入库单
                                    {
                                        //判断商品是否相等
                                        if (detail.CargoCode == salesOrderDetail.CargoCode && detail.BillRowNumber == salesOrderDetail.DetailRowNumber)
                                        {
                                            //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                            if (detail.NumCurrentPlan == salesOrderDetail.NumOriginalPlan)
                                            {
                                                salesOrderDetail.DetailRowStatus = 2;//初始态
                                                break;
                                            }
                                            else
                                            { 
                                                salesOrderDetail.DetailRowStatus = 4;//部分已转  
                                                break;                                                
                                            }
                                        }
                                    }
                                    edm.SalesOrderDetail.ApplyCurrentValues(salesOrderDetail);                                    
                                }

                                //查看保存的出库单是否把订单全部转或者是部分转
                                bool isAllSave = true;
                                foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                {
                                    if (salesOrderDetail.DetailRowStatus != 2)
                                    {
                                        isAllSave = false;
                                        break;
                                    }
                                }

                                if (isAllSave)
                                {
                                    tmpSalesOrder.OrderStatus = 2;//初始态
                                }
                                else
                                {
                                    tmpSalesOrder.OrderStatus = 4;//部分已转
                                }

                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];
                                    tmpSalesOrder.RFIDActorID = userInfo.UserId;
                                    tmpSalesOrder.RFIDActorName = userInfo.UserName;                                   
                                }
                                tmpSalesOrder.RFIDActorTime = System.DateTime.Now;                               

                                edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                            }
                        }
                        else if (si.FromOrderType == "02")//采购订单
                        {
                            var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                            if (tmpPurchaseOrder != null)
                            {                                
                                foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                {
                                    foreach (StockDetail detail in si.StockDetail)//当前入库单
                                    {
                                        //判断商品是否相等
                                        if (detail.CargoCode == purchaseOrderDetail.CargoCode && detail.BillRowNumber == purchaseOrderDetail.DetailRowNumber)
                                        {
                                            //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                            if (detail.NumCurrentPlan == purchaseOrderDetail.NumOriginalPlan)
                                            {
                                                purchaseOrderDetail.DetailRowStatus = 2;//初始态
                                                break;
                                            }
                                            else
                                            {
                                                purchaseOrderDetail.DetailRowStatus = 4;//部分已转 
                                                break;
                                            }
                                        }
                                    }
                                    edm.PurchaseOrderDetail.ApplyCurrentValues(purchaseOrderDetail);
                                }

                                //查看保存的出库单是否把订单全部转或者是部分转
                                bool isAllSave = true;
                                foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                {
                                    if (purchaseOrderDetail.DetailRowStatus != 2)
                                    {
                                        isAllSave = false;
                                        break;
                                    }
                                }

                                if (isAllSave)
                                {
                                    tmpPurchaseOrder.OrderStatus = 2;//初始态
                                }
                                else
                                {
                                    tmpPurchaseOrder.OrderStatus = 4;//部分已转
                                }

                                //设置RFID处理人信息及时间
                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];
                                    tmpPurchaseOrder.RFIDActorID = userInfo.UserId;
                                    tmpPurchaseOrder.RFIDActorName = userInfo.UserName;                                   
                                }
                                tmpPurchaseOrder.RFIDActorTime = System.DateTime.Now;                             

                                edm.PurchaseOrder.ApplyCurrentValues(tmpPurchaseOrder);
                            }
                        }
                        else if (si.FromOrderType == "03")//调拨订单
                        {
                            var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                            if (tmpTransferOrder != null)
                            {                               
                                foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                {
                                    foreach (StockDetail detail in si.StockDetail)//当前入库单
                                    {
                                        //判断商品是否相等
                                        if (detail.CargoCode == transferOrderDetail.CargoCode && detail.BillRowNumber == transferOrderDetail.DetailRowNumber)
                                        {
                                            //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                            if (detail.NumCurrentPlan == transferOrderDetail.NumOriginalPlan)
                                            {
                                                transferOrderDetail.DetailRowStatus = 2;//初始态
                                                break;
                                            }
                                            else
                                            {                                                                                            
                                                transferOrderDetail.DetailRowStatus = 4;//部分已转       
                                                break;
                                            }
                                        }
                                    }
                                    edm.TransferOrderDetail.ApplyCurrentValues(transferOrderDetail);                                  
                                }
                                //查看保存的出库单是否把订单全部转或者是部分转
                                bool isAllSave = true;
                                foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                {
                                    if (transferOrderDetail.DetailRowStatus != 2)
                                    {
                                        isAllSave = false;
                                        break;
                                    }
                                }

                                if (isAllSave)
                                {
                                    tmpTransferOrder.OrderStatus = 2;//初始态
                                }
                                else
                                {
                                    tmpTransferOrder.OrderStatus = 4;//部分已转
                                }

                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];
                                    tmpTransferOrder.RFIDActorID = userInfo.UserId;
                                    tmpTransferOrder.RFIDActorName = userInfo.UserName;                                    
                                }
                                tmpTransferOrder.RFIDActorTime = System.DateTime.Now;                              

                                edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                            }
                        }
                        else if (si.FromOrderType == "04")//转库单
                        {
                            var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                            if (tmpShiftOrder != null)
                            {                                
                                foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                {
                                    foreach (StockDetail detail in si.StockDetail)//当前入库单
                                    {
                                        //判断商品是否相等
                                        if (detail.CargoCode == shiftOrderDetail.CargoCode && detail.BillRowNumber == shiftOrderDetail.DetailRowNumber)
                                        {
                                            //如果入库单的商品应收数量=订单数量，则该行项目状态为2（初始态）
                                            if (detail.NumCurrentPlan == shiftOrderDetail.NumOriginalPlan)
                                            {
                                                shiftOrderDetail.DetailRowStatus = 2;//初始态
                                                break;
                                            }
                                            else
                                            {                                                                                           
                                                shiftOrderDetail.DetailRowStatus = 4;//部分已转       
                                                break;
                                            }
                                        }
                                    }
                                    edm.ShiftOrderDetail.ApplyCurrentValues(shiftOrderDetail);                                  
                                }

                                //查看保存的出库单是否把订单全部转或者是部分转
                                bool isAllSave = true;
                                foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                {
                                    if (shiftOrderDetail.DetailRowStatus != 2)
                                    {
                                        isAllSave = false;
                                        break;
                                    }
                                }

                                if (isAllSave)
                                {
                                    tmpShiftOrder.OrderStatus = 2;//初始态
                                }
                                else
                                {
                                    tmpShiftOrder.OrderStatus = 4;//部分已转
                                }

                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];
                                    tmpShiftOrder.RFIDActorID = userInfo.UserId;
                                    tmpShiftOrder.RFIDActorName = userInfo.UserName;                                  
                                }
                                tmpShiftOrder.RFIDActorTime = System.DateTime.Now;                             

                                edm.ShiftOrder.ApplyCurrentValues(tmpShiftOrder);
                            }
                        }
                    }
               
                    edm.SaveChanges();
                }
                return true;
            }
            catch
            { 
                return false;
            }
        }

        //设置 用友参考订单的编辑是否显示或隐藏
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lbtnSOCode = e.Row.FindControl("lbtnSICode") as LinkButton;
                if (lbtnSOCode != null)
                {
                    if (lbtnSOCode.Text.Trim() != string.Empty)
                    {
                        if (lbtnSOCode.Text.ToString().Substring(0, 1) == "X")
                        {
                            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
                            if (btnEdit != null)
                            {
                                btnEdit.Visible = false;
                            }
                        }
                    }
                }

                //设置单选按钮事件
                //RadioButton rad = e.Row.FindControl("radSelect") as RadioButton;
                //if (rad != null)
                //{
                //    rad.Attributes.Add("onclick", "SetRadio(this)");
                //}
            }
        }
        
        //选择单选按钮是否显示的判断
        protected bool GetButtonVisible(object siStatus,object siType)
        {
            if(siStatus!=null && siType!=null)
            {
                if (siType.ToString() == string.Empty)
                {
                    return false;
                }

                //入库单，且状态为已提交或已完成状态的，才可以撤销
                if ((Convert.ToInt32(siStatus) == 2 || Convert.ToInt32(siStatus) == 5) && siType.ToString().Substring(0,1)=="2")
                {
                    return true;
                }
            }
            return false;
        }

        //获取跳转界面字符串
        public static string GetUrlString(object obj)
        {
            string url = null;
            if (obj != null)
            {
                string cargoCode = obj.ToString();
                //获取单据类型
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    var tmp = edm.VSelectAllInCancleBillForInMgr.Where(o => o.SICode == cargoCode).ToList();
                    if (tmp != null)
                    {
                        //根据单据类型判断，是入库单还是撤销单，分别进入不同的界面
                        if (tmp[0].SIType.Substring(0, 1) == "2")//入库单
                        {
                            url = "~/StockMove/StockInReg.aspx?sourceCode=" + cargoCode + "&sourceType=stockin";
                        }
                        else if (tmp[0].SIType.Substring(0, 1) == "3")//撤销单
                        {
                            url = "~/StockMove/StorageInCancel.aspx?sourceCode=" + cargoCode + "&sourceType=stockin";
                        }
                    }
                }
            }
            return url;
        }
          
        //设置入库单号的颜色：如果为撤销单号，则设置为红色已区分
        protected System.Drawing.Color GetForeColor(object obj)
        {
            //获取linkbutton默认按钮颜色
            System.Drawing.Color color = lbtnNewIn.ForeColor; 
            if (obj != null)
            {
                string siCode = obj.ToString();
                //判断单号是否是撤销单
                if (siCode.Substring(0, 1) == "X")
                {
                    return System.Drawing.Color.Red;
                }                
            }

            return color;
        }
        
        /// <summary>
        /// 标签报警测试
        /// </summary>
        /// <param name="binTagID">标签编码</param>
        /// <param name="IsStartAlarm">true-开始报警，false-停止报警</param>
        /// <param name="shortMsg">调用报警函数简短消息提示</param>
        /// <param name="detailMsg">调用报警函数详细消息提示</param>
        /// <returns></returns>
        private void StartOrStopAlarm(string[] binTagID, bool IsStartAlarm, out string shortMsg)
        {
            shortMsg = "";
            try
            {
                int BinTagLightAlartCount = 3;//层位标签报警测试时亮灯次数
                int BinTagSoundAlartCount = 3;//层位标签报警测试时亮灯次数

                ////从配置文件读取亮灯次数
                //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("BinTagLightAlartCount"))
                //{
                //    int temp = 0;
                //    string configValue = System.Configuration.ConfigurationManager.AppSettings["BinTagLightAlartCount"].ToString();
                //    if (int.TryParse(configValue, out temp))
                //        BinTagLightAlartCount = temp;
                //}

                ////从配置文件读取鸣笛次数
                //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("BinTagSoundAlartCount"))
                //{
                //    int temp = 0;
                //    string configValue = System.Configuration.ConfigurationManager.AppSettings["BinTagSoundAlartCount"].ToString();
                //    if (int.TryParse(configValue, out temp))
                //        BinTagSoundAlartCount = temp;
                //}

                //从数据库数据字典读取亮点次数
                BinTagLightAlartCount = DAL.DbCommonMethod.GetTagFirstLightCount();

                //从数据库数据字典读取鸣音次数
                BinTagSoundAlartCount = DAL.DbCommonMethod.GetTagFirstSoundCount();

                ServiceReference_DeviceService.DeviceServiceClient client = new ServiceReference_DeviceService.DeviceServiceClient();
                client.Open();

                //调用服务端标签报警函数
                //ServiceResult = client.TagControl(binTagID, BinTagLightAlartCount, BinTagSoundAlartCount);
                //同时发送多个标签控制命令,函数立即返回
                client.TagControlAsyn(binTagID, BinTagLightAlartCount, BinTagSoundAlartCount);
                //void TagControlAsyn(List<string> tags, int light, int sound);

            }
            catch (Exception ex)
            {
                shortMsg = "报警失败!" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        //gridview绑定数据时，做相应的编辑等操作
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView senderGrid = sender as GridView;
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
            {                
                lblGridViewMsg.Text = "";               
                //ClearTipMsgLabel();//清除提示信息
            }
            else if (e.CommandName == "MyDefineUpdate")//更新
            {
                lblGridViewMsg.Text = "";
                if (senderGrid.EditIndex != -1)
                {
                    string editSICode = senderGrid.DataKeys[senderGrid.EditIndex].Value.ToString();
                    GridViewRow editRow = senderGrid.Rows[senderGrid.EditIndex];
                    try
                    {
                        using (GoldEntities context = new GoldEntities())
                        {
                            StockIn updateModel = (from r in context.StockIn where r.SICode == editSICode select r).FirstOrDefault();
                            string msg = "";
                            if (GetUpdateModel(ref updateModel, editRow, out msg) == false)
                            {
                                lblGridViewMsg.Text = msg;
                                return;
                            }
                            else
                            {
                                int result = context.SaveChanges();
                                if (result > 0)
                                {
                                    lblGridViewMsg.Text = "更新成功！";
                                    senderGrid.EditIndex = -1;//取消编辑状态
                                }
                                else
                                    lblGridViewMsg.Text = "更新失败（影响行数为0）";

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblGridViewMsg.Text = "更新失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
                    }
                }
            }
            
        }
                       
        // 获取新增界面中的数据        
        private bool GetUpdateModel(ref StockIn updateModel, GridViewRow editRow, out string msg)
        {
            msg = "";
            try
            {
                if (null == editRow)
                {
                    msg = "获取的编辑行为空！请重试！";
                    return false;
                }

                TextBox tbxFromUCOrderNo = editRow.FindControl("tbxFromUCOrderNo") as TextBox;
                TextBox tbxFromBillNo = editRow.FindControl("tbxFromBillNo") as TextBox;                

                if (tbxFromBillNo != null)
                {
                    if (tbxFromBillNo.Text.Trim() != string.Empty)
                    {
                        updateModel.FromBillNo = tbxFromBillNo.Text.Trim();
                    }
                }
                if (tbxFromUCOrderNo != null)
                {
                    if (tbxFromUCOrderNo.Text.Trim() != string.Empty)
                    {
                        updateModel.FromUCOrderNo = tbxFromUCOrderNo.Text.Trim();
                    }
                }                 

                if (msg.Length > 0)
                    return false;                

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }

        }

        //判断显示用友textbox还是源于excel单的textbox
        protected bool GetTextBoxVisible(object fromType)
        {
            bool revalue = false;
            if (fromType != null)
            {
                if (fromType.ToString() == "02")//源于Excel导入
                {
                    revalue = true;
                }                
            } 
            return revalue;
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
                SortDirection sortDirection = GridView1.SortDirection;
                SortDirection newSortDirection;
                switch (GridView1.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                GridView1.Sort(sortExpression, newSortDirection);                
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        //导入NC订单
        protected void btnNCDataImport_Click(object sender, EventArgs e)
        {
            Response.Redirect("StockInFromNC.aspx");
        } 
    }
}