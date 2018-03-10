using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Data.Objects;
using Gold.Utility;

namespace Gold.StockMove
{
    public partial class StockInCancel : System.Web.UI.Page
    {

        #region 事件
        //页面加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sourceCode = Request.QueryString["sourceCode"];
                string sourceType = Request.QueryString["sourceType"];

                if (!string.IsNullOrEmpty(sourceCode) && string.IsNullOrEmpty(sourceType))
                {
                    Server.Transfer("../error.aspx?errorInfo=缺少参数:sourceType (来源单据类型)");
                    return;
                }

                if (string.IsNullOrEmpty(sourceCode))
                {
                    SwitchToInsertMode();
                    return;
                }

                //从入库单管理界面跳转到详细信息界面
                if (sourceType == "stockin")
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockCancel.Include("StockDetail").Where(o => o.SCCode == sourceCode).ToList();
                        if (tmp == null || tmp.Count == 0)
                            Server.Transfer("../error.aspx?errorInfo=无此入库单记录。入库单号=" + sourceCode);
                        else
                        {
                            //绑定数据
                            ViewState["StockCancel"] = tmp[0];
                            ViewState["OrignalStockCancel"] = tmp[0];
                            FormView1.ChangeMode(FormViewMode.ReadOnly);
                            btnSave.Visible = false;
                            //btnReset.Visible = false;
                            btnReturn.Visible = true;
                            lbtnDeleteRow.Visible = false;
                            GridView1.Visible = false;
                            GridView2.Visible = true;
                            if (tmp[0].SCStatus < 2)
                            {
                                btnSubmit.Visible = true;//提交
                                btnEdit.Visible = true;
                            }
                            else
                            {
                                btnSubmit.Visible = false;
                                btnEdit.Visible = false;
                            }

                            return;
                        }
                    }
                }

                //新建入库撤销单
                if (sourceType == "stockcancel")
                {
                    StockCancel sc = CreateStockCancelFromStockMgr(sourceCode);
                    ViewState["StockCancel"] = sc;
                    ViewState["OrignalStockCancel"] = sc;
                    SwitchToInsertMode();
                    return;
                }
            }

            //编辑控件赋值
            if (ViewState["StockCancel"] != null)
            {
                StockCancel sc = (StockCancel)ViewState["StockCancel"];
                lblEditorID.Text = sc.EditorID;
                lblEditorName.Text = sc.EditorName;
            }
        }

        //保存
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    StockCancel sc;
                    if (ViewState["StockCancel"] != null)
                        sc = (StockCancel)ViewState["StockCancel"];
                    else
                        sc = new StockCancel();
                    sc.SCCode = KeyGenerator.Instance.GetStockCancleKey("In");
                    sc.SCType = "31";//撤销入库

                    UpdateStockCancel(ref sc);

                    if (sc == null)
                    {                       
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请填写入库单数据");
                        return;
                    }
                    if (sc.StockDetail.Count == 0)
                    {                       
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效撤销商品");
                        return;
                    }                  

                    sc.LastModifyTime = System.DateTime.Now;//最后修改时间

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        //更新入库单状态
                        var tmpIn = edm.StockIn.SingleOrDefault(o => o.SICode == sc.CancelBillCode);
                        if (tmpIn != null)
                        {
                            tmpIn.SIStatus = 3;//撤销中
                            edm.StockIn.ApplyCurrentValues(tmpIn);                            
                        }

                        foreach (StockDetail sd in sc.StockDetail)
                        {
                            if (sd.BinCode == null || sd.InOutWHCode == null)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }
                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }

                            //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
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

                        //被冲销程度
                        if (tmpIn.StockDetail.Count() > sc.StockDetail.Count)
                        {
                            sc.CancelLevel = "2";//部分冲销
                        }
                        else
                        {
                            sc.CancelLevel = "1";//全部冲销
                        }

                        edm.StockCancel.AddObject(sc); 

                        edm.SaveChanges();
                    }

                    ViewState["StockCancel"] = sc;
                    ViewState["OrignalStockCancel"] = sc;   
                    ChangeFormViewMode(FormViewMode.ReadOnly, sc.SCStatus);

                    GridView2.DataBind();
                    GridView1.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存成功!");
                }
                catch (Exception ex)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存数据出现异常" + Utility.LogHelper.GetExceptionMsg(ex));
                   // lblMessage.Text = "保存数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                }
            }
        }

        //编辑
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //保存编辑人信息
            StockCancel sc;
            if (ViewState["StockCancel"] != null)
            {
                sc = (StockCancel)ViewState["StockCancel"];
                if (sc.SCCode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockCancel.SingleOrDefault(o => o.SCCode == sc.SCCode);
                        if (tmp != null)
                        {
                            //判断是否有人正在编辑此订单
                            if (tmp.EditStatus == null)//如果为空，则设置状态为0
                            {
                                tmp.EditStatus = 0;
                            }

                            if (tmp.EditStatus == 0)//0-无人编辑，则保存当前用户信息
                            {
                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];
                                    tmp.EditorID = userInfo.UserId;
                                    tmp.EditorName = userInfo.UserName;
                                }
                                tmp.EditTime = System.DateTime.Now;
                                tmp.EditStatus = 1;//设置当前订单状态为正在编辑

                                //保存数据
                                edm.StockCancel.ApplyCurrentValues(tmp);
                                edm.SaveChanges();
                            }
                            else if (tmp.EditStatus == 1)
                            {
                                //判断该数据库编辑人是否是当前用户
                                if (Session["UserInfo"] != null)
                                {
                                    Users userInfo = (Users)Session["UserInfo"];

                                    if (tmp.EditorID != userInfo.UserId)
                                    {
                                        lblInOrOut.Text = "入库单";
                                        programmaticModalPopup.Show();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ChangeFormViewMode(FormViewMode.Edit, 1);
            lblMessage.Text = "";
        }

        //提交
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //防止时间过长，用户多次单击提交
            btnSubmit.Enabled = false;

            string shortMsg = "";
            try
            {
                //先清空提示
                lblMessage.Text = "";

                StockCancel sc;
                if (ViewState["StockCancel"] != null)
                {
                    sc = (StockCancel)ViewState["StockCancel"];
                    if (sc.SCCode == null)
                    {
                        sc.SCCode = KeyGenerator.Instance.GetStockCancleKey("In");
                    }
                }
                else
                {
                    sc = new StockCancel();
                    sc.SCCode = KeyGenerator.Instance.GetStockCancleKey("In");
                    sc.SCType = "31";//撤销入库
                }

                sc.SCStatus = 2;//已提交
                if (Session["UserInfo"] != null)
                {
                    Users userInfo = (Users)Session["UserInfo"];
                    sc.RFIDSubmitPersonID = userInfo.UserId;
                    sc.RFIDSubmitPersonName = userInfo.UserName;
                    //增加修改编辑字段的数据
                    sc.EditorID = userInfo.UserId;
                    sc.EditorName = userInfo.UserName;
                }
                sc.RFIDSubmitTime = System.DateTime.Now;
                sc.EditTime = System.DateTime.Now;
                sc.EditStatus = 0;//无人编辑
                UpdateStockCancel(ref sc);

                if (sc == null)
                {
                    btnSubmit.Enabled = true;
                    DAL.CommonConvert.ShowMessageBox(this.Page, "请填写入库单数据!");
                    return;
                }
                if (sc.StockDetail.Count == 0)
                {
                    btnSubmit.Enabled = true;
                    DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效撤销商品!");
                    return;
                }
               
               
                List<string> binTagIDs = new List<string>();                
                using (var edm = new Gold.DAL.GoldEntities())
                {
                    //更新入库单状态
                    var tmpIn = edm.StockIn.SingleOrDefault(o => o.SICode == sc.CancelBillCode);
                    if (tmpIn != null)
                    {
                        tmpIn.SIStatus = 3;//撤销中
                        edm.StockIn.ApplyCurrentValues(tmpIn);

                        //被冲销程度
                        if (tmpIn.StockDetail.Count() > sc.StockDetail.Count)
                        {
                            sc.CancelLevel = "2";//部分冲销
                        }
                        else
                        {
                            sc.CancelLevel = "1";//全部冲销
                        }
                    }
                    else
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page,"此入库单:" + sc.CancelBillCode + "已不存在!");
                        return;
                    }                    

                    var tmp = edm.StockCancel.SingleOrDefault(o => o.SCCode == sc.SCCode);
                    if (tmp == null)
                    {
                        foreach (StockDetail sd in sc.StockDetail)
                        {
                            if (sd.BinCode == null || sd.InOutWHCode == null)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }
                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }
                            if (sd.NumCurrentPlan == 0)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！");
                                return;
                            }
                            else if (sd.NumCurrentPlan > sd.NumOriginalPlan)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + sd.CargoCode + ")应收数量不能大于订单数量！");
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
                                            btnSubmit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + sd.CargoCode + ")的当前库存量为：" + tmpNumCurrent[0].ToString() + "!应发数量不能大于库存数量！");
                                            return;
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
                                    if (!binTagIDs.Contains(binTag))
                                    {
                                        binTagIDs.Add(binTag);    
                                    }                                                                    
                                }

                                //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
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
                        }
                        edm.StockCancel.AddObject(sc);
                    }
                    else
                    {
                        edm.StockCancel.ApplyCurrentValues(sc);

                        foreach (StockDetail sd in sc.StockDetail)
                        {
                            if (sd.BinCode == null || sd.InOutWHCode == null)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }
                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                                return;
                            }
                            if (sd.NumCurrentPlan == 0)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！");
                                return;
                            }
                            else if(sd.NumCurrentPlan>sd.NumOriginalPlan)
                            {
                                btnSubmit.Enabled = true;
                                DAL.CommonConvert.ShowMessageBox(this.Page,"商品("+ sd.CargoCode+")应收数量不能大于订单数量！");
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
                                            btnSubmit.Enabled = true;
                                            DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + sd.CargoCode + ")的当前库存量为：" + tmpNumCurrent[0].ToString() + "!应发数量不能大于库存数量！");
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

                            var tmp1 = edm.StockDetail.SingleOrDefault(o => (o.BillCode == sd.BillCode && o.BillRowNumber == sd.BillRowNumber));
                            if (tmp1 != null)
                            {
                                edm.StockDetail.ApplyCurrentValues(sd);
                            }

                            //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
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
                    }

                    //提交前，查看当前提交的商品数量是否大于库存数量
                    foreach (StockDetail detail in sc.StockDetail)
                    {
                        string cargoCode = detail.CargoCode;
                        double sumNum = 0;//此次提交商品数量
                        double remainderNum = 0;//库存数量
                        foreach (StockDetail det in sc.StockDetail)
                        {
                            if (cargoCode == det.CargoCode)
                            {
                                sumNum += det.NumCurrentPlan;
                            }
                        }

                        int numCurrent = 0;
                        int sumCurrent = 0;//汇总当前cargoTag库存数量
                        var tmpNumCurrent = edm.CargoTag.Where(o => o.CargoCode == cargoCode).Select(o => o.Number).ToList();
                        if (tmpNumCurrent.Count != 0)
                        {
                            if (!string.IsNullOrEmpty(tmpNumCurrent[0].ToString()) && int.TryParse(tmpNumCurrent[0].ToString(), out numCurrent))
                            {
                                sumCurrent += numCurrent;
                            }
                        }

                        //查询当前已提交手持机未开始操作的当前商品的应发数量
                        var tmpOutCargo = (from r in edm.StockOut
                                           where (r.SOStatus == 2 && r.SOCode != sc.SCCode)
                                           join x in edm.StockDetail on r.SOCode equals x.BillCode
                                           where (x.CargoCode == cargoCode && x.CargoStatus == 0)
                                           orderby r.SOCode
                                           select new { x.NumCurrentPlan }).ToList();

                        if (tmpOutCargo != null && tmpOutCargo.Count != 0)
                        {
                            remainderNum = tmpOutCargo.Sum(r => r.NumCurrentPlan);
                        }

                        //撤销入库单
                        var tmpOutCargo2 = (from r in edm.StockCancel
                                            where (r.SCStatus == 2 &&  r.SCType == "31" && r.SCCode !=sc.SCCode)
                                            join x in edm.StockDetail on r.SCCode equals x.BillCode
                                            where (x.CargoCode == cargoCode && x.CargoStatus == 0)
                                            orderby r.SCCode
                                            select new { x.NumCurrentPlan }).ToList();
                        if (tmpOutCargo2 != null && tmpOutCargo2.Count != 0)
                        {
                            remainderNum = tmpOutCargo2.Sum(r => r.NumCurrentPlan);
                        }

                        //判断应发数量是否超发
                        //当前出库当的应发数量 是否大于 库存数量-已提交出库单的数量
                        if (sumNum > (sumCurrent - remainderNum))
                        {
                            btnSubmit.Enabled = true;
                            DAL.CommonConvert.ShowMessageBox(this.Page, "商品(" + cargoCode + ")的当前总库存量为：" + (sumCurrent - remainderNum).ToString() + "!应发数量不能大于库存数量！");
                            return;
                        }
                    }

                    //提交前，验证该单是否已被提交
                    var edmStockcancel = new Gold.DAL.GoldEntities();
                    var tmpStockcancel = edmStockcancel.StockCancel.SingleOrDefault(o => o.SCCode == sc.SCCode);
                    if (tmpStockcancel != null)
                    {
                        if (tmpStockcancel.SCStatus > 1)
                        {
                            ViewState["OrignalStockCancel"] = tmpStockcancel;
                            btnSubmit.Enabled = true;
                            ChangeFormViewMode(FormViewMode.ReadOnly, tmpStockcancel.SCStatus);
                            GridView2.DataBind();
                            GridView1.DataBind();
                            DAL.CommonConvert.ShowMessageBox(this.Page, "此撤销单已被" + tmpStockcancel.RFIDSubmitPersonName + "提交!");
                            return;
                        }
                    }

                    //调用报警函数
                    SetAlarm(binTagIDs.ToArray(), true, out shortMsg);
                    edm.SaveChanges();
                }                
                
                ViewState["OrignalStockCancel"] = sc;
                btnSubmit.Enabled = true;
                ChangeFormViewMode(FormViewMode.ReadOnly, sc.SCStatus);
                GridView2.DataBind();
                GridView1.DataBind();
                DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货成功！单号：" + sc.SCCode);
            }
            catch (Exception ex)
            {
                btnSubmit.Enabled = true;
                DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货出现异常！" + Utility.LogHelper.GetExceptionMsg(ex));
                //lblMessage.Text = "提交至备货出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        //重置
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ViewState["StockCancel"] = ViewState["OrignalStockCancel"];
            FormView1.DataBind();
            GridView1.DataBind();
            GridView2.DataBind();
            SwitchToInsertMode();
            lblMessage.Text = null;
        }

        //更新
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    StockCancel sc;
                    if (ViewState["StockCancel"] != null)
                    {
                        sc = (StockCancel)ViewState["StockCancel"];
                        sc.SCStatus = 1;//已保存
                    }
                    else
                    {
                        sc = new StockCancel();
                        //获取撤销单的单号
                        sc.SCCode = KeyGenerator.Instance.GetStockCancleKey("In");
                        sc.SCType = "31";//撤销入库
                    }

                    UpdateStockCancel(ref sc);
                    sc.LastModifyTime = System.DateTime.Now;//最后修改时间

                    if (sc.StockDetail.Count == 0)
                    {                        
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效撤销商品");
                        return;
                    }

                    foreach (StockDetail sd in sc.StockDetail)
                    {
                        if (sd.BinCode == null || sd.InOutWHCode == null)
                        {
                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                            return;
                        }
                        if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                        {
                            DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位");
                            return;
                        }                        
                    }

                    //增加修改编辑字段的数据
                    if (Session["UserInfo"] != null)
                    {
                        Users userInfo = (Users)Session["UserInfo"];
                        sc.EditorID = userInfo.UserId;
                        sc.EditorName = userInfo.UserName;
                    }
                    sc.EditTime = System.DateTime.Now;
                    sc.EditStatus = 0;//无人编辑

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        //设置被冲销程度的值
                        var tmpIn = edm.StockIn.SingleOrDefault(o => o.SICode == sc.CancelBillCode);
                        if (tmpIn != null)
                        {
                            if (tmpIn.StockDetail.Count > sc.StockDetail.Count)
                            {
                                sc.CancelLevel = "2";//部分冲销
                            }
                            else
                            {
                                sc.CancelLevel = "1";//全部冲销
                            }
                        }

                        var tmp = edm.StockCancel.SingleOrDefault(o => o.SCCode == sc.SCCode);
                        if (tmp == null)
                        {

                            edm.StockCancel.AddObject(sc);
                        }
                        else
                        {
                            //删除数据库存在的信息
                            edm.StockCancel.DeleteObject(tmp);
                            edm.SaveChanges();
                            //保存现在新的数据
                            edm.StockCancel.AddObject(sc);
                        }
                        edm.SaveChanges();
                    }                    
                    
                    ViewState["OrignalStockCancel"] = sc;
                    ChangeFormViewMode(FormViewMode.ReadOnly, sc.SCStatus);
                    GridView2.DataBind();
                    GridView1.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新成功!");
                }
                catch (Exception ex)
                {
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex));
                    //lblMessage.Text = "保存数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                }
            }
        }

        //取消
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Insert)
            {
                Response.Redirect("StockInMgr.aspx");
            }

            ViewState["StockCancel"] = ViewState["OrignalStockCancel"];
            FormView1.DataBind();
            GridView1.DataBind();
            GridView2.DataBind();
            if (ViewState["StockCancel"] != null)
            {
                StockCancel sc = (StockCancel)ViewState["StockCancel"];
                if (sc.SCCode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockCancel.SingleOrDefault(o => o.SCCode == sc.SCCode);
                        if (tmp != null)
                        {
                            if (Session["UserInfo"] != null)
                            {
                                Users userInfo = (Users)Session["UserInfo"];
                                //增加修改编辑字段的数据
                                tmp.EditorID = userInfo.UserId;
                                tmp.EditorName = userInfo.UserName;
                            }
                            tmp.EditTime = System.DateTime.Now;
                            tmp.EditStatus = 0;//无人编辑
                        }
                        edm.StockCancel.ApplyCurrentValues(tmp);
                        edm.SaveChanges();
                    }
                }
            }
            ChangeFormViewMode(FormViewMode.ReadOnly, 0);
        }

        //返回入库单列表
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("StockInMgr.aspx");
        }

        //删除行
        protected void lbtnDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["StockCancel"] != null)
                {
                    StockCancel si = (StockCancel)ViewState["StockCancel"];
                    if (si.StockDetail != null && si.StockDetail.Count > 0)
                    {
                        int count = GridView1.Rows.Count;                        
                        for (int i = 0; i < count; i++)
                        {
                            int j = 0;
                            if (((CheckBox)GridView1.Rows[i].FindControl("CheckBox1")).Checked == true)
                            {
                                string rowNO = GridView1.Rows[i].Cells[1].Text;//序号
                                string cargoCode = GridView1.Rows[i].Cells[2].Text;//商品编号
                                                               
                                foreach (StockDetail detail in si.StockDetail)
                                {
                                    if (rowNO != null)
                                    {
                                        //同时判断商品编码和行号
                                        if (detail.CargoCode == cargoCode && detail.BillRowNumber == rowNO)
                                        {
                                            break;
                                        }
                                    }
                                    else if (detail.CargoCode == cargoCode)//只判断商品编码是否相等
                                    {
                                        break;
                                    }                                    
                                    j++;                                    
                                }
                                si.StockDetail.Remove(si.StockDetail.ElementAt(j));
                            }
                        }
                    }

                    ViewState["StockCancel"] = si;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除商品发生异常！" + msg);
            }
        }

        //弹出提示框的确定按钮
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["StockCancel"] != null)
                {
                    StockCancel sc = (StockCancel)ViewState["StockCancel"];
                    string scCode = sc.SCCode;
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockCancel.SingleOrDefault(o => o.SCCode == scCode);
                        if (tmp != null)
                        {
                            if (Session["UserInfo"] != null)
                            {
                                Users userInfo = (Users)Session["UserInfo"];
                                tmp.EditorID = userInfo.UserId;
                                tmp.EditorName = userInfo.UserName;
                            }
                            tmp.EditTime = System.DateTime.Now;
                            tmp.EditStatus = 1;//设置当前订单状态为正在编辑

                            //保存数据
                            edm.StockCancel.ApplyCurrentValues(tmp);
                            edm.SaveChanges();
                        }
                    }
                }

                this.programmaticModalPopup.Hide();

                ChangeFormViewMode(FormViewMode.Edit, 1);
                lblMessage.Text = "";
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "跳转界面发生异常！" + msg);
            }
        }

        //弹出提示框的取消按钮
        protected void hideClose_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup.Hide();
        } 
        #endregion

        #region 绑定数据方法
        public StockCancel SelectStockCancel()
        {
            return ViewState["StockCancel"] == null ? null : ((StockCancel)ViewState["StockCancel"]);
        }

        protected void odsStockCancle_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        protected void odsStockCancle_ObjectDisposing(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        public IList<StockDetail> SelectStockDetail()
        {
            var tmp = ViewState["StockCancel"] == null ? null : (StockCancel)ViewState["StockCancel"];
            if (tmp == null || tmp.StockDetail == null)
                return null;


            try
            {
                var ret = tmp.StockDetail.OrderBy(o => o.CargoName);
                return ret.ToList();
            }
            catch
            {
                using (var edm = new GoldEntities())
                {
                    var si = edm.StockCancel.SingleOrDefault(o => o.SCCode == tmp.SCCode);
                    if (tmp == null)
                    {
                        return null;
                    }
                    else
                    {
                        ViewState["StockCancel"] = si;
                        return si.StockDetail.OrderBy(o => o.CargoName).ToList();
                    }
                }
            }
        }

        protected void odsStockDetail_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        protected void odsStockDetail_ObjectDisposing(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion

        #region 方法
        //新增模式
        protected void SwitchToInsertMode()
        {
            ChangeFormViewMode(FormViewMode.Insert, 0);
            ((Label)FormView1.FindControl("ctlSCCode")).Text = "(自动生成)";
            if (ViewState["StockCancel"] != null)
            {
                StockCancel sc = (StockCancel)ViewState["StockCancel"];
                if (sc.SCType != null)//撤销类型
                {
                    DropDownList ddl = (DropDownList)FormView1.FindControl("ctlSCType");
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value == sc.SCType)
                        {
                            ddl.SelectedIndex = i;
                        }
                    }
                }

                //撤销状态                
                DropDownList ddlSCStatus = (DropDownList)FormView1.FindControl("ctlSCStatus");
                for (int i = 0; i < ddlSCStatus.Items.Count; i++)
                {
                    if (ddlSCStatus.Items[i].Value == sc.SCStatus.ToString())
                    {
                        ddlSCStatus.SelectedIndex = i;
                    }
                }

                //被撤销入库单号
                if (sc.CancelBillCode != null)
                {
                    ((TextBox)FormView1.FindControl("ctlCancelBillCode")).Text = sc.CancelBillCode;
                }

                //撤销日期
                if (sc.SCDate != null)
                {
                    ((TextBox)FormView1.FindControl("ctlSCDate")).Text = ((DateTime)(sc.SCDate)).ToString("yyyy-MM-dd");
                }
                else
                {
                    ((TextBox)FormView1.FindControl("ctlSCDate")).Text = System.DateTime.Now.ToShortDateString();
                }

                ////被冲销程度
                //if (sc.CancelLevel != null)
                //{
                //    DropDownList ddlCancelLevel = (DropDownList)FormView1.FindControl("ctlCancelLevel");
                //    for (int i = 0; i < ddlCancelLevel.Items.Count; i++)
                //    {
                //        if (ddlCancelLevel.Items[i].Value == sc.CancelLevel)
                //        {
                //            ddlCancelLevel.SelectedIndex = i;
                //        }
                //    }
                //}

                //仓库
                if (sc.WHCode != null)
                {
                    DropDownList ddlWH = (DropDownList)FormView1.FindControl("ctlWHName");
                    for (int i = 0; i < ddlWH.Items.Count; i++)
                    {
                        if (ddlWH.Items[i].Value == sc.WHCode)
                        {
                            ddlWH.SelectedIndex = i;
                        }
                    }
                }
                //备注
                ((TextBox)FormView1.FindControl("ctlComment")).Text = sc.Comment;
            }
            else
            {
                //暂时写固定值
                ((DropDownList)FormView1.FindControl("ctlSCStatus")).SelectedIndex = 1;//初始态
            }
        }

        private void ChangeFormViewMode(FormViewMode mode, int? status)
        {
            FormView1.ChangeMode(mode);
            switch (mode)
            {
                case FormViewMode.ReadOnly:
                    lblTitle.Text = "抬头信息";
                    if (status < 2)
                    {
                        btnEdit.Visible = true;
                    }
                    else
                    {
                        btnEdit.Visible = false;
                    }
                    btnCancel.Visible = false;
                    btnSave.Visible = false;
                    btnUpdate.Visible = false;
                    //btnReset.Visible = false;                   
                    lbtnDeleteRow.Visible = false;
                    GridView1.Visible = false;
                    GridView2.Visible = true;
                    btnReturn.Visible = true;
                    break;
                case FormViewMode.Edit:
                    lblTitle.Text = "编辑撤销入库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = true;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    //btnReset.Visible = false;                  
                    lbtnDeleteRow.Visible = true;
                    btnReturn.Visible = false;
                    GridView1.Visible = true;
                    GridView2.Visible = false;
                    break;
                case FormViewMode.Insert:
                    lblTitle.Text = "新建撤销入库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    btnUpdate.Visible = false;
                    //btnReset.Visible = true;                   
                    lbtnDeleteRow.Visible = true;
                    GridView1.Visible = true;
                    GridView2.Visible = false;
                    btnReturn.Visible = false;
                    var ctlCode = FormView1.FindControl("ctlSCCode") as Label;
                    ctlCode.Text = "(自动生成)";
                    break;
            }

            if (status < 2)
            {
                btnSubmit.Visible = true;
            }
            else
            {
                btnSubmit.Visible = false;
            }
        }

        //从入库单生产撤销单
        private StockCancel CreateStockCancelFromStockMgr(string sourceCode)
        {
            StockCancel sc = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockIn.Where(o => o.SICode == sourceCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此入库单记录。入库单号=" + sourceCode);
                else
                {
                    sc = edm.StockCancel.CreateObject();
                    sc.Comment = tmp.Comment;//备注
                    //sc.CancelLevel
                    sc.CancelBillCode = tmp.SICode;
                    sc.Reserve1 = tmp.Reserve1;
                    sc.Reserve2 = tmp.Reserve2;
                    sc.SCDate = System.DateTime.Now;//撤销日期
                    sc.SCType = "31";//撤销入库
                    sc.SCStatus = 0;
                    sc.TotalNumber = tmp.TotalNumber;
                    sc.WHCode = tmp.WHCode;

                    sc.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
                    if (tmp.StockDetail != null && tmp.StockDetail.Count > 0)
                    {
                        foreach (StockDetail item in tmp.StockDetail)
                        {
                            StockDetail sd = edm.StockDetail.CreateObject();
                            sd = item;
                            //实收数量
                            sd.NumActual = 0;
                            sd.CargoStatus = 0;
                            sd.RFIDSubmitTime = null;
                            sc.StockDetail.Add(sd);
                        }
                    }
                }
            }
            return sc;
        }

        //获取更新后的数据
        protected void UpdateStockCancel(ref StockCancel sc)
        {
            if (FormView1.CurrentMode == FormViewMode.Insert || FormView1.CurrentMode == FormViewMode.Edit)
            {
                if (sc.SCStatus == 0)
                {
                    sc.SCStatus = 1; //已保存 
                }
                //撤销类型
                sc.SCType = ((DropDownList)FormView1.FindControl("ctlSCType")).SelectedValue;

                //撤销日期
                DateTime da;
                if (!string.IsNullOrEmpty(((TextBox)FormView1.FindControl("ctlSCDate")).Text) &&
                    DateTime.TryParse(((TextBox)FormView1.FindControl("ctlSCDate")).Text, out da))
                    sc.SCDate = da;


                //被冲销单号
                sc.CancelBillCode = ((TextBox)FormView1.FindControl("ctlCancelBillCode")).Text;
                ////被冲销程度
                //DropDownList ddlCancelLevel = (DropDownList)FormView1.FindControl("ctlCancelLevel");
                //if (ddlCancelLevel.SelectedIndex != 0)
                //{
                //    sc.CancelLevel = (ddlCancelLevel).SelectedValue;
                //}                

                //仓库编码
                DropDownList ddlWH = (DropDownList)FormView1.FindControl("ctlWHName");
                if (ddlWH.SelectedIndex != 0)
                {
                    sc.WHCode = ddlWH.SelectedValue;
                }
                //备注
                sc.Comment = ((TextBox)FormView1.FindControl("ctlComment")).Text;

                //单据创建信息
                if (Session["UserInfo"] != null)
                {
                    Users userInfo = (Users)Session["UserInfo"];
                    sc.RFIDActorID = userInfo.UserId;
                    sc.RFIDActorName = userInfo.UserName;
                }
                sc.RFIDActorTime = System.DateTime.Now;
            }

            //针对在只读模式下提交的单据
            if (sc.SCStatus == 2 && FormView1.CurrentMode == FormViewMode.ReadOnly)
            {
                foreach (StockDetail sd in sc.StockDetail)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime
                }
                ViewState["StockCancel"] = sc;
                return;
            }

            //绑定行项目
            if (sc.StockDetail == null)
                sc.StockDetail = new EntityCollection<StockDetail>();
            else
                sc.StockDetail.Clear();

            //编辑、插入模式才会继续往下走
            double total = 0;
            for (int i = GridView1.Rows.Count - 1; i >= 0; i--)
            {
                GridViewRow r = GridView1.Rows[i];
                StockDetail sd = new StockDetail();
                sd.BillCode = sc.SCCode;//单号
                if (r.Cells[1].Text.ToString() != string.Empty && r.Cells[1].Text.ToString() != "&nbsp;")//行号
                {
                    sd.BillRowNumber = r.Cells[1].Text;
                }
                else
                {
                    sd.BillRowNumber = (i + 1).ToString();
                }
                //sd.BillRowNumber = (i + 1).ToString();
                sd.CargoStatus = 0;//0:手持机流程还未完成
                sd.CargoCode = (r.Cells[2]).Text;//商品编码
                sd.CargoName = (r.Cells[3]).Text;
                sd.CargoModel = r.Cells[4].Text == "&nbsp;" ? "" : r.Cells[4].Text;
                sd.CargoSpec = r.Cells[5].Text == "&nbsp;" ? "" : r.Cells[5].Text;
                sd.CargoUnits = r.Cells[6].Text == "&nbsp;" ? "" : r.Cells[6].Text;
                

                //订单数量
                double douNumOriginalPlan;
                if (!string.IsNullOrEmpty((r.Cells[7]).Text) &&
                 double.TryParse((r.Cells[7]).Text, out douNumOriginalPlan))
                    sd.NumOriginalPlan = douNumOriginalPlan;

                //应收数量
                double douNumCurrentPlan;
                if (!string.IsNullOrEmpty(((TextBox)r.Cells[8].Controls[1]).Text) &&
                 double.TryParse(((TextBox)r.Cells[8].Controls[1]).Text, out douNumCurrentPlan))
                    sd.NumCurrentPlan = douNumCurrentPlan;

                //实收数量
                double dou;
                if (!string.IsNullOrEmpty((r.Cells[9]).Text) &&
                    double.TryParse((r.Cells[9]).Text, out dou))
                {
                    sd.NumActual = dou;
                    total += dou;
                }

                //行总金额
                //sd.RowTotalMoney

                DropDownList ddlWH = (DropDownList)r.Cells[10].Controls[1];
                if (ddlWH.SelectedIndex != 0)
                {
                    sd.InOutWHCode = ddlWH.SelectedValue;
                    sd.InOutWHName = ddlWH.Items[ddlWH.SelectedIndex].Text;
                }

                DropDownList ddlBinCode = (DropDownList)r.Cells[11].Controls[1];
                if (ddlBinCode.SelectedValue.ToString() != string.Empty)
                {
                    sd.BinCode = (ddlBinCode).SelectedValue;//层位编码
                }
                sd.UCOrderNo = r.Cells[12].Text == "&nbsp;" ? "" : r.Cells[12].Text;//来源单号
                sd.RFIDOrderNo = sd.UCOrderNo;
                sd.Comment = r.Cells[13].Text == "&nbsp;" ? "" : r.Cells[13].Text;//备注
                if (r.Cells[14].Text.Trim() != string.Empty && r.Cells[14].Text != "&nbsp;")
                {
                    sd.CargoStatus = Convert.ToInt32(r.Cells[7].Text);//商品状态
                }

                sd.ReleaseYear = r.Cells[15].Text == "&nbsp;" ? "" : r.Cells[15].Text;//发行年份

                sd.SCCode = sc.SCCode;
                sd.BillType = "03";//01-出库单，02-入库单，03-撤销入库单（即出库），04-撤销出库单（即入库）
                if (sc.SCStatus == 2)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime
                }

                sd.StockCancel = sc;
                sc.StockDetail.Add(sd);
            }

            //合计数量
            sc.TotalNumber = total;

            ViewState["StockCancel"] = sc;
        }

        //设置默认值
        protected void ctlInOutWHCode_DataBound(object sender, EventArgs e)
        {
            DropDownList ddlInOutWHCode = GridView1.FindControl("ctlInOutWHCode") as DropDownList;
            if (ddlInOutWHCode != null)
            {
                if (ddlInOutWHCode.SelectedIndex == 0)//如果当前没有选择其他值，则设置默认值
                {
                    string defaultWHCode = string.Empty;
                    //从配置文件读取默认的仓库--地王26库
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                    {
                        defaultWHCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                    }
                    ddlInOutWHCode.SelectedValue = defaultWHCode;
                }
            }
        }

        /// <summary>
        /// 标签报警测试
        /// </summary>
        /// <param name="binTagID">标签编码（可以是一个或者多个）</param>
        /// <param name="IsStartAlarm">true-开始报警，false-停止报警</param>
        /// <param name="shortMsg">调用报警函数简短消息提示</param>
        /// <returns></returns>
        private void SetAlarm(string[] binTagID, bool IsStartAlarm, out string shortMsg)
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
        #endregion

        #region Gridview && FromView
        //设置默认值
        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            //仓库
            DropDownList drpWHName = FormView1.FindControl("ctlWHName") as DropDownList;
            TextBox txtWHName = FormView1.FindControl("txtWHName") as TextBox;
            if (drpWHName != null)
            {
                if (txtWHName != null && txtWHName.Text.Trim() != string.Empty && drpWHName.Items.Count > 1)
                {
                    drpWHName.SelectedIndex = drpWHName.Items.IndexOf(drpWHName.Items.FindByValue(txtWHName.Text));
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    DropDownList binCode = e.Row.FindControl("ctlBinCode") as DropDownList;
            //    if (binCode.SelectedIndex > 0)
            //        return;

            //    string cargoCode = e.Row.Cells[2].Text.Trim();
            //    if (!string.IsNullOrEmpty(cargoCode))
            //    {
            //        using (var edm = new GoldEntities())
            //        {
            //            var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode).ToList();
            //            if (tmp != null && tmp.Count > 0)
            //            {
            //                foreach (CargoTag tag in tmp)
            //                {
            //                    binCode.Items.Insert(1, new ListItem(tag.BinCode, tag.BinCode));
            //                }
            //                binCode.SelectedIndex = 1;
            //            }
            //        }
            //    }

            //}


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList binCode = e.Row.FindControl("ctlBinCode") as DropDownList;
                if (binCode != null)
                {
                    //if(binCode.SelectedIndex > 0)
                    //return;

                    if (binCode.Items.Count <= 1)//无数据时开始绑定数据
                    {
                        //TextBox cargoCode = //e.Row.FindControl("ctlCargoCode") as TextBox;
                        string cargoCode = e.Row.Cells[2].Text.Trim();
                        if (!string.IsNullOrEmpty(cargoCode))
                        {
                            using (var edm = new GoldEntities())
                            {
                                var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode || o.CargoCode == null).Select(o => o.BinCode).Distinct().ToList();
                                if (tmp != null && tmp.Count > 0)
                                {
                                    foreach (string tag in tmp)
                                    {
                                        binCode.Items.Add( new ListItem(tag, tag));
                                    }
                                }
                            }
                        }
                    }
                    //已绑定数据时设置选中项
                    TextBox tbxBinCode_Selected = e.Row.FindControl("tbxBinCode_Selected") as TextBox;
                    if (tbxBinCode_Selected != null && tbxBinCode_Selected.Text != "" && binCode.Items.Count > 1)
                    {
                        binCode.SelectedIndex = binCode.Items.IndexOf(binCode.Items.FindByText(tbxBinCode_Selected.Text));
                    }

                }

            }
        }
        #endregion


    }
}