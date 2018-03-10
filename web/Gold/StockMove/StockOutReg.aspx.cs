using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data.Objects.DataClasses;
using System.Data;
using Gold.Utility;

namespace Gold.StockMove
{
    public partial class StockOutReg : System.Web.UI.Page
    {
        #region 方法

        //切换为插入模式
        protected void SwitchToInsertMode()
        {
            lblTitle.Text = "新建出库单";
            FormView1.ChangeMode(FormViewMode.Insert);

            ////初始化下拉列表值
            //InitDropList();

            ((TextBox)FormView1.FindControl("ctlSOCode")).Text = "(自动生成)";
            if (ViewState["StockOut"] != null)
            {
                StockOut so = (StockOut)ViewState["StockOut"];
                if (so.SOType != null)//来源单据类型
                {
                    DropDownList ddl = (DropDownList)FormView1.FindControl("ctlSOType");
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value == so.SOType)
                        {
                            ddl.SelectedIndex = i;
                        }
                    }
                }               
                if (so.SOStatus != null)//单据状态
                {
                    DropDownList ddlSOStatus = (DropDownList)FormView1.FindControl("ctlSOStatus");
                    for (int i = 0; i < ddlSOStatus.Items.Count; i++)
                    {
                        if (ddlSOStatus.Items[i].Value == so.SOType)
                        {
                            ddlSOStatus.SelectedIndex = i;
                        }
                    }                   
                }          
                if( so.SODate == null)
                {
                    ((TextBox)FormView1.FindControl("ctlSODate")).Text =System.DateTime.Now.ToShortDateString();
                }
                else
                {
                    ((TextBox)FormView1.FindControl("ctlSODate")).Text=((DateTime)(so.SODate)).ToString("yyyy-MM-dd");
                }
               
                if (so.FromUCOrderNo != null )
                {
                    ((TextBox)FormView1.FindControl("ctlFromUCOrderNo")).Text = so.FromUCOrderNo;
                    if (so.FromType != "03" && so.FromType != "" && so.FromUCOrderNo.ToString() != string.Empty)
                    {
                        ((TextBox)FormView1.FindControl("ctlFromUCOrderNo")).Enabled = false;
                        lbtnAddRow.Visible = false;
                        lbtnDeleteRow.Visible = false;

                        DropDownList ddlSOType = (DropDownList)FormView1.FindControl("ctlSOType");
                        DropDownList ddlFromType = (DropDownList)FormView1.FindControl("ctlFromType");
                        ddlSOType.Enabled = false;
                        ddlFromType.Enabled = false; 

                        TextBox ctlFromBillNo = (TextBox)FormView1.FindControl("ctlFromBillNo");                       
                        if (so.FromType == "02")
                        {
                            ctlFromBillNo.Enabled = false;
                        }
                    }
                }

                //源Excel出库单号
                if (so.FromBillNo != null)
                {
                    TextBox ctlFromBillNo = (TextBox)FormView1.FindControl("ctlFromBillNo");
                    ctlFromBillNo.Text = so.FromBillNo;
                    if (so.FromType == "02")
                    {
                        ctlFromBillNo.Enabled = false;
                    }
                }

                if (so.FromType != null)//来源订单类型
                {
                    DropDownList ddl=(DropDownList)FormView1.FindControl("ctlFromType");
                    for(int i=0;i<ddl.Items.Count;i++)
                    {
                        if(ddl.Items[i].Value==so.FromType)
                        {
                            ddl.SelectedIndex=i;
                        }
                    }
                }                

                if (so.SellDepartmentName != null)
                {
                    DropDownList ddlDepartmentName = (DropDownList)FormView1.FindControl("ctlSellDepartmentName");
                    for (int i = 0; i < ddlDepartmentName.Items.Count; i++)
                    {
                        if (ddlDepartmentName.Items[i].Value == so.SellDepartmentName)
                        {
                            ddlDepartmentName.SelectedIndex = i;
                        }
                    }
                }
                
                ((TextBox)FormView1.FindControl("ctlCustomerName")).Text = so.CustomerName;

                //业务员
                if (so.BussinessMan != null)//业务类型
                {
                    DropDownList ddlBussinessMan = (DropDownList)FormView1.FindControl("ctlBussinessMan");
                    for (int i = 0; i < ddlBussinessMan.Items.Count; i++)
                    {
                        if (ddlBussinessMan.Items[i].Value == so.BussinessMan)
                        {
                            ddlBussinessMan.SelectedIndex = i;
                        }
                    }
                }  
               
                //仓库
                if (so.WHName != null)
                {
                    DropDownList ddlWHName = (DropDownList)FormView1.FindControl("ctlWHName");
                    for (int i = 0; i < ddlWHName.Items.Count; i++)
                    {
                        if (ddlWHName.Items[i].Value == so.WHName)
                        {
                            ddlWHName.SelectedIndex = i;
                        }
                    }
                } 

                //库管员
                if (so.StorageMan != null)
                {
                    DropDownList ddlStorageMan = (DropDownList)FormView1.FindControl("ctlStorageMan");
                    for (int i = 0; i < ddlStorageMan.Items.Count; i++)
                    {
                        if (ddlStorageMan.Items[i].Value == so.StorageMan)
                        {
                            ddlStorageMan.SelectedIndex = i;
                        }
                    }
                } 

                if (so.BusinessType != null)//业务类型
                {
                    DropDownList ddl = (DropDownList)FormView1.FindControl("ctlBusinessType");
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value == so.BusinessType)
                        {
                            ddl.SelectedIndex = i;
                        }
                    }
                }

                //收发类别
                if (so.OutCategory != null)
                {
                    DropDownList ddlOutCategory = (DropDownList)FormView1.FindControl("ctlOutCategory");
                    ddlOutCategory.SelectedIndex = ddlOutCategory.Items.IndexOf(ddlOutCategory.Items.FindByValue(so.OutCategory));
                }
                
                ((TextBox)FormView1.FindControl("ctlComment")).Text = so.Comment;
            }
            else
            {
                //暂时写固定值
                ((DropDownList)FormView1.FindControl("ctlSOStatus")).SelectedIndex = 1;//初始态
                //((TextBox)FormView1.FindControl("ctlSOStatus")).Text= "初始态";//初始态
            }
        }

        //从销售订单生成出库单
        protected StockOut CreateStockOutFromSalesOrder(string orderCode, string sourceSort)
        {
            StockOut so = null;
           
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.SalesOrder.Where(o => o.OrderCode == orderCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此采购订单记录。采购订单号=" + orderCode);
                else
                {
                    so = new StockOut();
                    so.SODate = System.DateTime.Now;
                    so.SOType = CommonConvert.GetSIOTypeCode("销售出库单");
                    so.FromOrderNo = tmp.OrderCode;
                    so.FromType = CommonConvert.GetFromTypeCode("源于订单");                   
                    so.FromUCOrderNo = tmp.NCOrderCode;//用友系统订单号
                    so.FromOrderType = "01";//源于销售订单
                    so.SOStatus = 0;//初始态
                    so.CustomerName = tmp.ClientName;//客户名称
                    so.AccountSign = tmp.AccountSign;
                    so.BusinessSign = tmp.BusinessSign;
                    so.LeaderSign = tmp.LeaderSign;
                    so.WarehouseSign = tmp.WarehouseSign;
                    so.EditorSign = tmp.Preparer;//制单人
                    so.SellDepartmentName = tmp.SellDepartmentName;//销售部门
                    so.BussinessMan = tmp.Operator;//业务员
                    so.TotalMoney = tmp.TotalMoney;//合计金额
                    so.TotalNumber = tmp.TotalNumber;//合计数量
                    so.LastModifyTime = tmp.LastModifyTime;//最后修改时间
                    so.RFIDActorID = tmp.RFIDActorID == null ? "" : tmp.RFIDActorID.ToString();
                    so.RFIDActorName = tmp.RFIDActorName;
                    so.RFIDActorTime = tmp.RFIDActorTime;
                    so.Comment = tmp.Comment;//备注
                    so.Reserve1 = tmp.Reserve1;
                    so.Reserve2 = tmp.Reserve2;
                    //so.FromBillType = "01";//出库单              

                    so.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
                    if (tmp.SalesOrderDetail != null && tmp.SalesOrderDetail.Count > 0)
                    {
                        List<SalesOrderDetail> tmpdetail = tmp.SalesOrderDetail.ToList();

                        //重新根据订单界面的排序方式进行排序赋值
                        if (sourceSort != null && tmpdetail.Count != 0)
                        {
                            string[] sort = sourceSort.Split(',');
                            if (sort.Count() > 1)
                            {
                                string express = sort[1];
                                if (sort[0].ToString() == "ASC")
                                {
                                    tmpdetail = tmpdetail.OrderBy(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                                else if (sort[0].ToString() == "DESC")
                                {
                                    tmpdetail = tmpdetail.OrderByDescending(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                            }
                        }

                        foreach (SalesOrderDetail item in tmpdetail)
                        {
                            //如果实发数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }

                            StockDetail detail = new StockDetail();
                            detail.BillCode = so.SOCode;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec; 
                            detail.CargoUnits = item.CargoUnits;
                            if (item.WHCode != null)
                            {
                                detail.InOutWHCode = item.WHCode; 
                            }                            
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);
                            detail.NumCurrentPlan = item.NumCurrentPlan == null ? 0 : (double)(item.NumCurrentPlan);                            
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //应收数量=订单数量-实际已收数量
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual; 
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应发数量
                            detail.RowTotalMoney = item.TotalMoney;//总金额
                            detail.ReleaseYear = item.ReleaseYear;//发行年份
                            detail.Reserve1 = item.Reserve1;
                            detail.Reserve2 = item.Reserve2;                          
                            detail.RowTotalMoney = item.TotalMoney;                            
                            detail.SOCode = so.SOCode;
                            detail.UCOrderNo = item.NCOrderCode;
                            detail.BillRowNumber = item.DetailRowNumber;//订单明细行号
                            detail.BillType = "01";

                            //设置默认区位
                            if (detail.CargoCode != null)
                            {
                                var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == detail.CargoCode).ToList();
                                //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                                if (tmpBinCoe.Count > 0)
                                {
                                    detail.BinCode = tmpBinCoe[0].BinCode;
                                    detail.InOutWHCode = tmpBinCoe[0].WHCode;
                                    detail.InOutWHName = tmpBinCoe[0].WHName;
                                }
                            }

                            so.StockDetail.Add(detail);
                        }
                    }                    
                }
            }          

            return so;
        }

        //从转库单生成出库单
        protected StockOut CreateStockOutFromShiftorder(string orderCode, string sourceSort)
        {
            StockOut so = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.ShiftOrder.Where(o => o.OrderCode == orderCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此转库单记录。转库单号=" + orderCode);
                else
                {
                    so = new StockOut();
                    so.SODate = System.DateTime.Now;
                    so.SOType = CommonConvert.GetSIOTypeCode("其他出库单");
                    so.FromOrderNo = tmp.OrderCode;
                    so.FromType = CommonConvert.GetFromTypeCode("源于订单");
                    so.FromUCOrderNo = tmp.NCOrderCode;//用友系统订单号
                    so.FromOrderType = "04";//转库单
                    so.SOStatus = 0;//初始态
                    //so.CustomerName = tmp.ClientName;//客户名称
                    //so.AccountSign = tmp.AccountSign;
                    //so.BusinessSign = tmp.BusinessSign;
                    so.LeaderSign = tmp.LeaderName;
                    //so.WarehouseSign = tmp.WarehouseSign;
                    //so.EditorSign = tmp.Preparer;//制单人
                    //so.SellDepartmentName = tmp.SellDepartmentName;//销售部门
                    //so.BussinessMan = tmp.Operator;//业务员
                    //so.TotalMoney = tmp.TotalMoney;//合计金额
                    so.TotalNumber = tmp.TotalNumber;//合计数量
                    //so.LastModifyTime = tmp.LastModifyTime;//最后修改时间
                    so.RFIDActorID = tmp.RFIDActorID == null ? "" : tmp.RFIDActorID.ToString();
                    so.RFIDActorName = tmp.RFIDActorName;
                    so.RFIDActorTime = tmp.RFIDActorTime;
                    so.Comment = tmp.Comment;//备注
                    so.Reserve1 = tmp.Reserve1;
                    so.Reserve2 = tmp.Reserve2;
                    //so.FromBillType = "01";//出库单               

                    so.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
                    if (tmp.ShiftOrderDetail != null && tmp.ShiftOrderDetail.Count > 0)
                    {
                        List<ShiftOrderDetail> tmpdetail = tmp.ShiftOrderDetail.ToList();

                        //重新根据订单界面的排序方式进行排序赋值
                        if (sourceSort != null && tmpdetail.Count != 0)
                        {
                            string[] sort = sourceSort.Split(',');
                            if (sort.Count() > 1)
                            {
                                string express = sort[1];
                                if (sort[0].ToString() == "ASC")
                                {
                                    tmpdetail = tmpdetail.OrderBy(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                                else if (sort[0].ToString() == "DESC")
                                {
                                    tmpdetail = tmpdetail.OrderByDescending(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                            }
                        }

                        foreach (ShiftOrderDetail item in tmpdetail)
                        {
                            //如果实发数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }

                            StockDetail detail = new StockDetail();
                            detail.BillCode = so.SOCode;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec;
                            detail.CargoUnits = item.CargoUnits;
                            //detail.InOutWHCode = item.WHCode;
                            detail.NumCurrentPlan = item.NumCurrentPlan == null ? 0 : (double)(item.NumCurrentPlan);
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);                            
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //应收数量=订单数量-实际已收数量
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual; 
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应发数量
                            //detail.RowTotalMoney = item.TotalMoney;//总金额
                            detail.ReleaseYear = item.ReleaseYear;//发行年份
                            detail.Reserve1 = item.Reserve1;
                            detail.Reserve2 = item.Reserve2;
                            //detail.RowTotalMoney = item.TotalMoney;
                            detail.SOCode = so.SOCode;
                            detail.UCOrderNo = item.NCOrderCode;
                            detail.BillRowNumber = item.DetailRowNumber;//订单明细行号
                            detail.BillType = "01";

                            //设置默认区位
                            if (detail.CargoCode != null)
                            {
                                var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == detail.CargoCode).ToList();
                                //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                                if (tmpBinCoe.Count > 0)
                                {
                                    detail.BinCode = tmpBinCoe[0].BinCode;
                                    detail.InOutWHCode = tmpBinCoe[0].WHCode;
                                    detail.InOutWHName = tmpBinCoe[0].WHName;
                                }
                            }

                            so.StockDetail.Add(detail);
                        }
                    }
                }
            }

            return so;
        }

        //从调拨单生成出库单
        protected StockOut CreateStockOutFromTransferorder(string orderCode, string sourceSort)
        {
            StockOut so = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.TransferOrder.Where(o => o.OrderCode == orderCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此调拨单单记录。调拨单号=" + orderCode);
                else
                {
                    so = new StockOut();
                    so.SODate = System.DateTime.Now;
                    so.SOType = CommonConvert.GetSIOTypeCode("调拨出库单");
                    so.FromOrderNo = tmp.OrderCode;
                    so.FromType = CommonConvert.GetFromTypeCode("源于订单");
                    so.FromOrderType = "03";//源于调拨订单
                    so.FromUCOrderNo = tmp.NCOrderCode;//用友系统订单号
                    so.SOStatus = 0;//初始态
                    //so.CustomerName = tmp.ClientName;//客户名称
                    //so.AccountSign = tmp.AccountSign;
                    //so.BusinessSign = tmp.BusinessSign;
                    //so.LeaderSign = tmp.LeaderName;
                    so.WarehouseSign = tmp.StoreKeeper;
                    //so.EditorSign = tmp.Preparer;//制单人
                    //so.SellDepartmentName = tmp.SellDepartmentName;//销售部门
                    so.BussinessMan = tmp.Operator;//业务员
                    //so.TotalMoney = tmp.TotalMoney;//合计金额
                    so.TotalNumber = tmp.TotalNumber;//合计数量
                    so.LastModifyTime = tmp.LastModifyTime;//最后修改时间
                    so.RFIDActorID = tmp.RFIDActorID == null ? "" : tmp.RFIDActorID.ToString();
                    so.RFIDActorName = tmp.RFIDActorName;
                    so.RFIDActorTime = tmp.RFIDActorTime;
                    so.Comment = tmp.Comment;//备注
                    so.Reserve1 = tmp.Reserve1;
                    so.Reserve2 = tmp.Reserve2;
                    //so.FromBillType = "01";//出库单
                    so.WHCode = tmp.OutWHCode;
                    so.WHName = tmp.OutWHName;//调入仓库

                    so.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
                    if (tmp.TransferOrderDetail != null && tmp.TransferOrderDetail.Count > 0)
                    {
                        List<TransferOrderDetail> tmpdetail = tmp.TransferOrderDetail.ToList();

                        //重新根据订单界面的排序方式进行排序赋值
                        if (sourceSort != null && tmpdetail.Count != 0)
                        {
                            string[] sort = sourceSort.Split(',');
                            if (sort.Count() > 1)
                            {
                                string express = sort[1];
                                if (sort[0].ToString() == "ASC")
                                {
                                    tmpdetail = tmpdetail.OrderBy(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                                else if (sort[0].ToString() == "DESC")
                                {
                                    tmpdetail = tmpdetail.OrderByDescending(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                            }
                        }

                        foreach (TransferOrderDetail item in tmpdetail)
                        {
                            //如果实发数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }

                            StockDetail detail = new StockDetail();
                            detail.BillCode = so.SOCode;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec;
                            detail.CargoUnits = item.CargoUnits;
                            //detail.InOutWHCode = item.WHCode;                            
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);                            
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //应收数量=订单数量-实际已收数量
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual; 
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应发数量
                            //detail.RowTotalMoney = item.TotalMoney;//总金额
                            detail.ReleaseYear = item.ReleaseYear;//发行年份
                            detail.Reserve1 = item.Reserve1;
                            detail.Reserve2 = item.Reserve2;
                            //detail.RowTotalMoney = item.TotalMoney;
                            detail.SOCode = so.SOCode;
                            detail.UCOrderNo = item.NCOrderCode;
                            detail.BillRowNumber = item.DetailRowNumber;//订单明细行号
                            detail.BillType = "01";

                            //设置默认区位
                            if (detail.CargoCode != null)
                            {
                                var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == detail.CargoCode).ToList();
                                //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                                if (tmpBinCoe.Count > 0)
                                {
                                    detail.BinCode = tmpBinCoe[0].BinCode;
                                    detail.InOutWHCode = tmpBinCoe[0].WHCode;
                                    detail.InOutWHName = tmpBinCoe[0].WHName;
                                }
                            }

                            so.StockDetail.Add(detail);
                        }
                    }
                }
            }

            return so;
        }

        //Formview切换不同模式
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
                    btnReset.Visible = false;
                    btnReturn.Visible = true;
                    grdStockDetail.Visible = true;
                    grdStockDetailEdit.Visible = false;                    
                    lbtnAddRow.Visible = false;
                    lbtnDeleteRow.Visible = false;
                    if (ViewState["sourceType"] != null)
                    {
                        string type = ViewState["sourceType"].ToString();
                        if (type == "salesorder")
                        {
                            btnReturnSales.Visible = true;
                            btnReturnShift.Visible = false;
                            btnReturnTransfer.Visible = false;
                        }
                        else if (type == "shiftorder")
                        {
                            btnReturnSales.Visible = false;
                            btnReturnShift.Visible = true;
                            btnReturnTransfer.Visible = false;
                        }
                        else if (type == "transferorder")
                        {
                            btnReturnSales.Visible = false;
                            btnReturnShift.Visible = false;
                            btnReturnTransfer.Visible = true;
                        }
                    }
                    break;
                case FormViewMode.Edit:
                    lblTitle.Text = "编辑出库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = true;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    btnReset.Visible = false;
                    btnReturn.Visible = false;
                    grdStockDetail.Visible = false;
                    grdStockDetailEdit.Visible = true;                    
                    lbtnAddRow.Visible = true;
                    lbtnDeleteRow.Visible = true;
                    btnReturnSales.Visible = false;
                    btnReturnShift.Visible = false;
                    btnReturnTransfer.Visible = false;
                    break;
                case FormViewMode.Insert:
                    lblTitle.Text = "新建出库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = true;
                    btnUpdate.Visible = false;
                    btnReset.Visible = true;
                    btnReturn.Visible = false;
                    grdStockDetail.Visible = false;
                    grdStockDetailEdit.Visible = false;                    
                    lbtnAddRow.Visible = true;
                    lbtnDeleteRow.Visible = true;
                    btnReturnSales.Visible = false;
                    btnReturnShift.Visible = false;
                    btnReturnTransfer.Visible = false;
                    var ctlCode = FormView1.FindControl("ctlSOCode") as TextBox;
                    ctlCode.Text = "(自动生成)";
                    break;
            }
            if (status < 2 || status == null)
            {
                btnSubmit.Visible = true;
            }
            else
            {
                btnSubmit.Visible = false;
            }
        }

        //更新出库单数据
        private void UpdateStockOut(ref StockOut so)
        {
            if (FormView1.CurrentMode == FormViewMode.Insert || FormView1.CurrentMode == FormViewMode.Edit)
            {
                if (so.SOStatus == 0 || so.SOStatus == null)
                {
                    so.SOStatus = 1; //已保存 
                }

                so.SOType = ((DropDownList)FormView1.FindControl("ctlSOType")).SelectedValue;                
                DateTime da;
                if (!string.IsNullOrEmpty(((TextBox)FormView1.FindControl("ctlSODate")).Text) &&
                    DateTime.TryParse(((TextBox)FormView1.FindControl("ctlSODate")).Text, out da))
                    so.SODate = da;
                else
                {
                    so.SODate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                }

                TextBox ctlFromBillNo = ((TextBox)FormView1.FindControl("ctlFromBillNo"));
                if (ctlFromBillNo != null)
                {
                    if (ctlFromBillNo.Text.Trim() != string.Empty)
                    {
                        so.FromBillNo = ctlFromBillNo.Text.Trim();
                    }
                }

                TextBox ctlFromUCOrderNo=(TextBox)FormView1.FindControl("ctlFromUCOrderNo");
                if(ctlFromUCOrderNo!=null)
                {
                    if(ctlFromUCOrderNo.Text.Trim()!=string.Empty)
                    {
                        so.FromUCOrderNo = ctlFromUCOrderNo.Text.Trim();
                    }
                }

                if (so.FromBillNo != null && so.FromUCOrderNo != null)
                {
                    if (so.FromBillNo.ToString() != string.Empty && so.FromUCOrderNo.ToString() != string.Empty)
                    {
                        throw new Exception("用友参考单号和源Excel出库单号，不能同时有值！");
                    }
                }
                
                so.FromType = ((DropDownList)FormView1.FindControl("ctlFromType")).SelectedValue;
                so.SellDepartmentName = ((DropDownList)FormView1.FindControl("ctlSellDepartmentName")).SelectedValue;
                so.CustomerName = ((TextBox)FormView1.FindControl("ctlCustomerName")).Text.Trim();

                so.BussinessMan = ((DropDownList)FormView1.FindControl("ctlBussinessMan")).SelectedValue;

                DropDownList ddlWH=(DropDownList)FormView1.FindControl("ctlWHName");
                if (ddlWH.SelectedIndex != 0)
                {
                    so.WHCode = ddlWH.SelectedValue;
                    so.WHName = ddlWH.Items[ddlWH.SelectedIndex].Text;
                }
                //else
                //{
                //    throw new Exception("请选择仓库！");
                //}

                //收发类别
                DropDownList ddlOutCategory = (DropDownList)FormView1.FindControl("ctlOutCategory");
                if (ddlOutCategory.SelectedIndex != 0)
                {
                    so.OutCategory = ddlOutCategory.SelectedValue;
                }              

                so.StorageMan = ((DropDownList)FormView1.FindControl("ctlStorageMan")).SelectedValue;
                so.BusinessType = ((DropDownList)FormView1.FindControl("ctlBusinessType")).SelectedValue;

                //so.TotalNumber
                //so.TotalMoney

                //单据创建信息
                if (Session["UserInfo"] != null)
                {                    
                    Users userInfo = (Users)Session["UserInfo"];
                    so.RFIDActorID = userInfo.UserId;
                    so.RFIDActorName = userInfo.UserName;
                }                
                so.RFIDActorTime = System.DateTime.Now;                

                so.Comment = ((TextBox)FormView1.FindControl("ctlComment")).Text.Trim();
            }

            //提交
            if (so.SOStatus == 2 && FormView1.CurrentMode == FormViewMode.ReadOnly)
            {
                foreach (StockDetail sd in so.StockDetail)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime;
                }
                ViewState["StockOut"] = so;
                return;
            }

            //绑定行项目
            if (so.StockDetail == null)
                so.StockDetail = new EntityCollection<StockDetail>();
            else
                so.StockDetail.Clear();

            double totalNum = 0;
            for (int i = grdStockDetailEdit.Rows.Count - 1; i >= 0; i--)
            {
                GridViewRow r = grdStockDetailEdit.Rows[i];
                
                //如果是源于订单，且用友参考单号不为空，则需要判断是否被选中，是有选中的行项目，才会保存或者提交
                if ((so.FromType == "01" && so.FromType != "" && so.FromUCOrderNo != null)
                    || (so.FromType== "02"))
                {
                    if (!(r.Cells[0].Controls[1] as CheckBox).Checked)
                        continue;
                }                

                StockDetail sd = new StockDetail();
                sd.BillCode = so.SOCode;//r.Cells[15].Text
                sd.SOCode = so.SOCode;

                if (r.Cells[1].Text.ToString() != string.Empty && r.Cells[1].Text.ToString() != "&nbsp;" && (so.FromType == "01" || so.FromType == "02"))//行号
                {
                    sd.BillRowNumber = r.Cells[1].Text;
                }
                else
                {
                    sd.BillRowNumber = (i + 1).ToString();
                }
                sd.CargoStatus = 0;//0:手持机流程还未完成
                sd.CargoCode = (r.Cells[2].Controls[1] as TextBox).Text;//商品编码
                sd.CargoName = (r.Cells[3].Controls[1] as TextBox).Text;//商品名称               
                sd.CargoModel = r.Cells[5].Text == "&nbsp;" ? "" : r.Cells[5].Text;//商品型号
                sd.CargoSpec = r.Cells[4].Text == "&nbsp;" ? "" : r.Cells[4].Text;//商品规格
                sd.CargoUnits = r.Cells[6].Text == "&nbsp;" ? "" : r.Cells[6].Text;                

                //订单数量
                double douNumOriginalPlan;
                if (!string.IsNullOrEmpty((r.Cells[7]).Text) &&
                 double.TryParse((r.Cells[7]).Text, out douNumOriginalPlan))
                    sd.NumOriginalPlan = douNumOriginalPlan;
                //sd.NumCurrentPlan = r.Cells[9].Text == "&nbsp;" ? 0 : Convert.ToDouble(r.Cells[9].Text);//订单数量

                //应发数量
                double douNumCurrentPlan;
                if (!string.IsNullOrEmpty(((TextBox)r.Cells[8].Controls[1]).Text) &&
                 double.TryParse(((TextBox)r.Cells[8].Controls[1]).Text, out douNumCurrentPlan))
                    sd.NumCurrentPlan = douNumCurrentPlan;

                //源于手工新增的 订单数量=应发数量
                if (so.SOType == "13" && so.FromType == "03")
                {
                    sd.NumOriginalPlan = sd.NumCurrentPlan;
                }

                //实收数量
                double dou;
                if (!string.IsNullOrEmpty((r.Cells[9]).Text) &&
                 double.TryParse((r.Cells[9]).Text, out dou))
                {
                    sd.NumActual = dou;
                    totalNum += dou;
                }
                
                if (((TextBox)r.Cells[10].Controls[1]).Text.Trim() != string.Empty
                  && ((TextBox)r.Cells[10].Controls[1]).Text != "&nbsp;")
                {
                    sd.BinCode = ((TextBox)r.Cells[10].Controls[1]).Text;//区位编码
                }                
                sd.UCOrderNo = r.Cells[11].Text == "&nbsp;" ? "" : r.Cells[11].Text; ;//来源单号
                sd.RFIDOrderNo = sd.UCOrderNo;                
                sd.Comment = r.Cells[12].Text == "&nbsp;" ? "" : r.Cells[12].Text;
                //if (((r.Cells[13].Controls[0]) as Label).Text.Trim() != string.Empty && ((r.Cells[13].Controls[0]) as Label).Text != "&nbsp;")
                //{
                //    if (((r.Cells[13].Controls[1]) as Label).Text == "未完成")
                //    {
                //        sd.CargoStatus = 0;//商品状态
                //    }
                //    else if (((r.Cells[13].Controls[1]) as Label).Text == "已完成")
                //    {
                //        sd.CargoStatus = 1;//商品状态
                //    }
                //}
                //else
                //{
                    sd.CargoStatus = 0;
                //}

                if (r.Cells[14].Text.Trim() != string.Empty && r.Cells[14].Text != "&nbsp;")
                {
                    sd.ReleaseYear = r.Cells[14].Text;//发行年份
                }
                if (r.Cells[15].Text.Trim() != string.Empty && r.Cells[15].Text != "&nbsp;")
                {
                    sd.InOutWHCode = r.Cells[15].Text;//出货仓库编码
                }
                if (r.Cells[16].Text.Trim() != string.Empty && r.Cells[16].Text != "&nbsp;")
                {
                    sd.InOutWHName = r.Cells[16].Text;//仓库名称
                }

                //sd.BillCode = r.Cells[16].Text;//隐藏列，无法取值
                sd.BillType = "01";//01-出库单，02-入库单，03-撤销入库单（即出库），04-撤销出库单（即入库）                
                sd.RowTotalMoney = r.Cells[20].Text.ToString() == string.Empty ? 0 : Convert.ToDecimal(r.Cells[20].Text);                
                //sd.HandSetFinishTime
                //sd.HandSetPersonID
                //sd.HandSetPersonName
                //sd.Reserve1
                //sd.Reserve2
                if (so.SOStatus == 2)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//单据提交时间
                }

                so.StockDetail.Add(sd);
            }
                
            so.TotalNumber = totalNum;//合计总数量

            ViewState["StockOut"] = so;
        }

        //初始化下拉框状态
        private void InitDropList()
        {             
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

        #endregion

        #region 事件

        //窗体加载
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BinCodeSelect1.PostBack += this.BinCodeSelect1_PostBack;
                GoodsSelect1.PostBack += this.GoodsSelect1_PostBack;

                if (!IsPostBack)
                {
                    string sourceCode = Request.QueryString["sourceCode"];
                    string sourceType = Request.QueryString["sourceType"];
                    string sourceSort = Request.QueryString["sortExDi"];
                    ViewState["sortExDi"] = sourceSort;
                    ViewState["sourceType"] = sourceType;

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

                    if (sourceType == "stockout")
                    {
                        using (var edm = new Gold.DAL.GoldEntities())
                        {
                            var tmp = edm.StockOut.Include("StockDetail").Where(o => o.SOCode == sourceCode).ToList();
                            
                            string warehouseCode = string.Empty;//           
                            //从配置文件读取默认的仓库--地王26库
                            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                            {
                                warehouseCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                            }
                            else
                            {
                                warehouseCode = "20101";
                            }

                            if (tmp == null || tmp.Count == 0)
                                Server.Transfer("../error.aspx?errorInfo=无此出库单记录。出库单号=" + sourceCode);
                            else
                            {
                                //绑定数据
                                ViewState["StockOut"] = tmp[0];
                                ViewState["OrignalStockOut"] = tmp[0];
                                grdStockDetailEdit.Visible = false;

                                FormView1.ChangeMode(FormViewMode.ReadOnly);

                                btnSave.Visible = false;
                                btnReset.Visible = false;
                                btnReturn.Visible = true;
                                lbtnAddRow.Visible = false;
                                lbtnDeleteRow.Visible = false;
                                if (tmp[0].SOStatus < 2 && tmp[0].WHCode == warehouseCode)
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

                    if (sourceType == "salesorder")//销售订单
                    {
                        ViewState["StockOut"] = CreateStockOutFromSalesOrder(sourceCode, sourceSort);
                        ViewState["OrignalStockOut"] = ViewState["StockOut"];
                        SwitchToInsertMode();
                        ////初始化下拉列表值
                        //InitDropList();

                        grdStockDetail.Visible = false;
                        btnSubmit.Visible = true;
                        return;
                    }

                    if (sourceType == "shiftorder")//转库单
                    {
                        ViewState["StockOut"] = CreateStockOutFromShiftorder(sourceCode, sourceSort);
                        ViewState["OrignalStockOut"] = ViewState["StockOut"];
                        SwitchToInsertMode();
                        grdStockDetail.Visible = false;
                        btnSubmit.Visible = true;
                        return;
                    }

                    if (sourceType == "transferorder")//调拨单
                    {
                        ViewState["StockOut"] = CreateStockOutFromTransferorder(sourceCode, sourceSort);
                        ViewState["OrignalStockOut"] = ViewState["StockOut"];
                        SwitchToInsertMode();
                        grdStockDetail.Visible = false;
                        btnSubmit.Visible = true;
                        return;
                    }
                }

                //编辑控件赋值
                if (ViewState["StockOut"] != null)
                {
                    StockOut so = (StockOut)ViewState["StockOut"];
                    lblEditorID.Text = so.EditorID;
                    lblEditorName.Text = so.EditorName;
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "出现异常！" + msg);
            }
        }

        //保存
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    StockOut so;                   
                    if (ViewState["StockOut"] != null)
                        so = (StockOut)ViewState["StockOut"];
                    else
                        so = new StockOut();
                    so.SOCode = KeyGenerator.Instance.GetStockOutKey();
                    so.SOStatus = 1;
                    //so.FromType = "ft";
                    UpdateStockOut(ref so);
                    
                    if (so == null)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效商品!");
                        return;
                    }

                    so.LastModifyTime = System.DateTime.Now;//最后修改时间

                    string warehouseCode = string.Empty;//           
                    //从配置文件读取默认的仓库--地王26库
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                    {
                        warehouseCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                    }
                    else
                    {
                        warehouseCode = "20101";
                    }
                    if (so.WHCode != warehouseCode)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        foreach (StockDetail sd in so.StockDetail)
                        {
                            if (sd.BinCode == null || sd.InOutWHCode == null)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                return;
                            }
                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                return;
                            }
                            if (sd.InOutWHCode != warehouseCode)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                return;
                            }                         
                        }

                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应发数量=Sum（所有detail应发数量）-撤销单的应发数量-手持机尚未开始操作被撤销的出库单应发数量
                        if (so.FromOrderNo != null && so.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockOuts = edmOrder.StockOut.Where(o => (o.FromOrderNo == so.FromOrderNo && o.SOCode != so.SOCode)).ToList();
                            if (so.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);                                
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单,减去撤销单的应发数量
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == salesOrderDetail.NumOriginalPlan || Num > salesOrderDetail.NumOriginalPlan)
                                                    {
                                                        salesOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        salesOrderDetail.DetailRowStatus = 2;//初始态
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
                                        tmpSalesOrder.RFIDActorID = userInfo.UserId;
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
                            else if (so.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == purchaseOrderDetail.NumOriginalPlan || Num > purchaseOrderDetail.NumOriginalPlan)
                                                    {
                                                        purchaseOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        purchaseOrderDetail.DetailRowStatus = 2;//初始态
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
                            else if (so.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == transferOrderDetail.NumOriginalPlan || Num > transferOrderDetail.NumOriginalPlan)
                                                    {
                                                        transferOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        transferOrderDetail.DetailRowStatus = 2;//初始态
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
                            else if (so.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == shiftOrderDetail.NumOriginalPlan || Num > shiftOrderDetail.NumOriginalPlan)
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 2;//初始态
                                                    }
                                                    else
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 4;//部分已转       
                                                    }
                                                }
                                            }
                                        }
                                        edm.ShiftOrderDetail.ApplyCurrentValues(shiftOrderDetail);

                                        //如果状态为部分已转或者是初始态或为空
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

                        edm.StockOut.AddObject(so);
                        edm.SaveChanges();
                    }

                    ViewState["StockOut"] = so;
                    ViewState["OrignalStockOut"] = so;                   
                    ChangeFormViewMode(FormViewMode.ReadOnly, so.SOStatus);
                    grdStockDetail.DataBind();
                    grdStockDetailEdit.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存成功!单号：" + so.SOCode);
                }
                catch (Exception ex)
                {
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存数据出现异常！" + msg);
                    return;
                }
            }
        }

        //更新
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    //string errMsg = "";
                    StockOut so;
                    if (ViewState["StockOut"] != null)
                    {
                        so = (StockOut)ViewState["StockOut"];
                        so.SOStatus = 1;
                    }
                    else
                    {
                        so = new StockOut();
                        so.SOCode = KeyGenerator.Instance.GetStockOutKey();
                        so.SOType = "03";
                    }

                    UpdateStockOut(ref so);
                    so.LastModifyTime = System.DateTime.Now;//最后修改时间

                    if (so == null)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效商品!");
                        return;
                    }

                    //增加修改编辑字段的数据
                    if (Session["UserInfo"] != null)
                    {
                        Users userInfo = (Users)Session["UserInfo"];
                        so.EditorID = userInfo.UserId;
                        so.EditorName = userInfo.UserName;
                    }
                    so.EditTime = System.DateTime.Now;
                    so.EditStatus = 0;//无人编辑

                    string warehouseCode = string.Empty;//           
                    //从配置文件读取默认的仓库--地王26库
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                    {
                        warehouseCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                    }
                    else
                    {
                        warehouseCode = "20101";
                    }
                    if (so.WHCode != warehouseCode)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == so.SOCode);
                        foreach (StockDetail sd in so.StockDetail)
                        {
                            if (sd.BinCode == null || sd.InOutWHCode == null)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                return;
                            }
                            if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                return;
                            }
                            if (sd.InOutWHCode != warehouseCode)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                return;
                            }
                        }
                        if (tmp == null)
                        {                           
                            edm.StockOut.AddObject(so);
                        }
                        else
                        {

                            edm.StockOut.DeleteObject(tmp);
                            edm.SaveChanges();
                            edm.StockOut.AddObject(so);
                        }
                       
                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应发数量=Sum（所有detail应发数量）-撤销单的应发数量-手持机尚未开始操作被撤销的出库单应发数量
                        if (so.FromOrderNo != null && so.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockOuts = edmOrder.StockOut.Where(o => (o.FromOrderNo == so.FromOrderNo && o.SOCode != so.SOCode)).ToList();
                            if (so.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == salesOrderDetail.NumOriginalPlan || Num > salesOrderDetail.NumOriginalPlan)
                                                    {
                                                        salesOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        salesOrderDetail.DetailRowStatus = 2;//初始态
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
                                    }
                                    tmpSalesOrder.RFIDActorTime = System.DateTime.Now;

                                    edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                                }
                            }
                            else if (so.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == purchaseOrderDetail.NumOriginalPlan || Num > purchaseOrderDetail.NumOriginalPlan)
                                                    {
                                                        purchaseOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        purchaseOrderDetail.DetailRowStatus = 2;//初始化
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
                            else if (so.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == transferOrderDetail.NumOriginalPlan || Num > transferOrderDetail.NumOriginalPlan)
                                                    {
                                                        transferOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        transferOrderDetail.DetailRowStatus = 2;//初始态
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
                                    }
                                    tmpTransferOrder.RFIDActorTime = System.DateTime.Now;

                                    edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                                }
                            }
                            else if (so.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int i = 0; i < tmpStockOuts.Count; i++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[i];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num == shiftOrderDetail.NumOriginalPlan || Num > shiftOrderDetail.NumOriginalPlan)
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num == 0)
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 2;//初始态
                                                    }
                                                    else
                                                    {
                                                        shiftOrderDetail.DetailRowStatus = 4;//部分已转       
                                                    }
                                                }
                                            }
                                        }
                                        edm.ShiftOrderDetail.ApplyCurrentValues(shiftOrderDetail);

                                        //如果状态为部分已转或者是初始态或为空
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
                                    }
                                    tmpShiftOrder.RFIDActorTime = System.DateTime.Now;

                                    edm.ShiftOrder.ApplyCurrentValues(tmpShiftOrder);
                                }
                            }
                        }

                        edm.SaveChanges();
                    }
                   
                    ViewState["OrignalStockOut"] = so;
                    ChangeFormViewMode(FormViewMode.ReadOnly, so.SOStatus);
                    grdStockDetail.DataBind();
                    grdStockDetailEdit.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新成功!单号：" + so.SOCode);
                }
                catch (Exception ex)
                {
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    //lblMessage.Text = "更新数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新数据出现异常！" + msg);

                    return;
                
                }
            }
        }

        //取消
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewState["StockOut"] = ViewState["OrignalStockOut"];
            FormView1.DataBind();
            grdStockDetail.DataBind();
            grdStockDetailEdit.DataBind();
            if (ViewState["StockOut"] != null)
            {
                StockOut so = (StockOut)ViewState["StockOut"];
                if (so.SOCode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == so.SOCode);
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
                        edm.StockOut.ApplyCurrentValues(tmp);
                        edm.SaveChanges();
                    }
                }
            }
           
            ChangeFormViewMode(FormViewMode.ReadOnly, 0);
        }

        //重置
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ViewState["StockOut"] = ViewState["OrignalStockOut"];
            FormView1.DataBind();
            grdStockDetail.Visible = false;
            grdStockDetailEdit.DataBind();
            grdStockDetail.DataBind();
            SwitchToInsertMode();
            lblMessage.Text = null;
        }

        //编辑
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //保存编辑人信息
            StockOut so;
            if (ViewState["StockOut"] != null)
            {
                so = (StockOut)ViewState["StockOut"];
                if (so.SOCode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == so.SOCode);
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
                                edm.StockOut.ApplyCurrentValues(tmp);
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

        //返回列表
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("StockOutMgr.aspx");
        }

        //提交
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //防止时间过长，用户多次单击提交
            btnSubmit.Enabled = false;
            if (IsValid)
            {                
                string shortMsg = "";                
                try
                {
                    //先清空提示
                    lblMessage.Text = "";
                    StockOut so;
                    if (ViewState["StockOut"] != null)
                    {
                        so = (StockOut)ViewState["StockOut"];
                        if (so.SOCode == null)
                        {
                            so.SOCode = KeyGenerator.Instance.GetStockOutKey();
                        }
                    }
                    else
                    {
                        so = new StockOut();
                        so.SOCode = KeyGenerator.Instance.GetStockOutKey();
                        so.SOType = "03";
                    }

                    so.SOStatus = 2;//已提交                

                    if (Session["UserInfo"] != null)
                    {
                        Users userInfo = (Users)Session["UserInfo"];
                        so.RFIDSubmitPersonID = userInfo.UserId;
                        so.RFIDSubmitPersonName = userInfo.UserName;

                        //增加修改编辑字段的数据
                        so.EditorID = userInfo.UserId;
                        so.EditorName = userInfo.UserName;
                    }
                    so.RFIDSubmitTime = System.DateTime.Now;
                    so.EditTime = System.DateTime.Now;
                    so.EditStatus = 0;//无人编辑

                    UpdateStockOut(ref so);                                      

                    if (so == null)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请填写出库单数据!");                       
                        return;
                    }
                    if (so.StockDetail.Count == 0)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择要出库的商品!");                       
                        return;
                    }
                    if (so.FromType.ToString() == string.Empty)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择订单类型!");
                        return;
                    }
                    if (so.SOType.ToString() == string.Empty)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择单据类型!");
                        return;
                    }
                    if (so.OutCategory == null)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择收发类别！");
                        return;
                    }

                    string warehouseCode = string.Empty;//           
                    //从配置文件读取默认的仓库--地王26库
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                    {
                        warehouseCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                    }
                    else
                    {
                        warehouseCode = "20101";
                    }
                    if (so.WHCode != warehouseCode)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    List<string> binTagIDs = new List<string>();
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == so.SOCode);
                        if (tmp == null)
                        {                            
                            foreach (StockDetail sd in so.StockDetail)
                            {
                                if (sd.BinCode == null || sd.InOutWHCode == null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                    return;
                                }
                                if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                    return;
                                }
                                if (sd.InOutWHCode != warehouseCode)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                    return;
                                }
                                if (sd.NumCurrentPlan == 0)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应发数量！");
                                    return;
                                }
                                else if (sd.NumCurrentPlan > sd.NumOriginalPlan && so.FromOrderNo != null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "应发数量不能大于订单数量！");
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

                                sd.RFIDSubmitTime = System.DateTime.Now;

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
                            }
                            edm.StockOut.AddObject(so);
                        }
                        else
                        {
                            edm.StockOut.ApplyCurrentValues(so);
                            foreach (StockDetail sd in so.StockDetail)
                            {
                                if (sd.BinCode == null || sd.InOutWHCode == null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                    return;
                                }
                                if (sd.BinCode.ToString() == string.Empty || sd.InOutWHCode.ToString() == string.Empty)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库和区位!");
                                    return;
                                }
                                if (sd.NumCurrentPlan == 0)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应发数量！");
                                    return;
                                }
                                else if (sd.NumCurrentPlan > sd.NumOriginalPlan && so.FromOrderNo != null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "应发数量不能大于订单数量！");
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

                                sd.RFIDSubmitTime = System.DateTime.Now;

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
                                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime

                                    edm.StockDetail.ApplyCurrentValues(sd);
                                }
                            }                            
                        }

                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应发数量=Sum（所有detail应发数量）-撤销单的应发数量-手持机尚未开始操作被撤销的出库单应发数量
                        if (so.FromOrderNo != null && so.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockOuts = edmOrder.StockOut.Where(o => (o.FromOrderNo == so.FromOrderNo && o.SOCode != so.SOCode)).ToList();
                            if (so.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int j = 0; j < tmpStockOuts.Count; j++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[j];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }

                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                        btnSubmit.Enabled = true;
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
                                        tmpSalesOrder.RFIDActorID = userInfo.UserId;
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
                            else if (so.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int j = 0; j < tmpStockOuts.Count; j++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[j];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }

                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                        btnSubmit.Enabled = true;
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
                            else if (so.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int j = 0; j < tmpStockOuts.Count; j++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[j];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                    if (Num ==transferOrderDetail.NumOriginalPlan)
                                                    {
                                                        transferOrderDetail.DetailRowStatus = 3;//全部已转
                                                    }
                                                    else if (Num > transferOrderDetail.NumOriginalPlan)
                                                    {
                                                        btnSubmit.Enabled = true;
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
                            else if (so.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == so.FromOrderNo);
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in so.StockDetail)//当前入库单
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
                                                    if (tmpStockOuts != null)
                                                    {
                                                        for (int j = 0; j < tmpStockOuts.Count; j++)
                                                        {
                                                            StockOut stockOut = tmpStockOuts[j];
                                                            foreach (StockDetail oldDetail in stockOut.StockDetail)
                                                            {
                                                                if (oldDetail.CargoCode == detail.CargoCode && oldDetail.BillRowNumber == detail.BillRowNumber)
                                                                {
                                                                    Num += oldDetail.NumCurrentPlan;
                                                                    break;
                                                                }
                                                            }
                                                            //查找撤销单
                                                            var tmpCancel = edmOrder.StockCancel.Where(o => o.CancelBillCode == stockOut.SOCode).ToList();
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
                                                                if (stockOut.SOStatus == 4)
                                                                {
                                                                    foreach (StockDetail oldDetail in stockOut.StockDetail)
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
                                                        btnSubmit.Enabled = true;
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

                                        //如果状态为部分已转或者是初始态或为空
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

                        //提交前，查看当前提交的商品数量是否大于库存数量
                        foreach (StockDetail detail in so.StockDetail)
                        {
                            string cargoCode = detail.CargoCode;
                            double sumNum = 0;//此次提交商品数量
                            double remainderNum = 0;//库存数量
                            foreach (StockDetail det in so.StockDetail)
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
                            var tmpOutCargo = (from r in edm.StockOut where (r.SOStatus==2 && r.SOCode != so.SOCode)
                                               join x in edm.StockDetail on r.SOCode equals x.BillCode where (x.CargoCode==cargoCode&&x.CargoStatus==0)
                                               orderby r.SOCode
                                               select new { x.NumCurrentPlan } ).ToList();

                            if (tmpOutCargo != null && tmpOutCargo.Count != 0)
                            {
                                remainderNum = tmpOutCargo.Sum(r => r.NumCurrentPlan);                                
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
                        var edmStockout = new Gold.DAL.GoldEntities();
                        var tmpStockOut = edmStockout.StockOut.SingleOrDefault(o => o.SOCode == so.SOCode);
                        if (tmpStockOut != null)
                        {
                            if (tmpStockOut.SOStatus > 1)
                            {
                                ViewState["OrignalStockOut"] = tmpStockOut;
                                btnSubmit.Enabled = true;
                                ChangeFormViewMode(FormViewMode.ReadOnly, tmpStockOut.SOStatus);
                                grdStockDetail.DataBind();
                                grdStockDetailEdit.DataBind();
                                DAL.CommonConvert.ShowMessageBox(this.Page, "此出库单已被" + tmpStockOut.RFIDSubmitPersonName + "提交!");
                                return;
                            }
                        }

                        //调用报警函数
                        StartOrStopAlarm(binTagIDs.ToArray(), true, out shortMsg);
                        edm.SaveChanges();
                    }
                   
                    ViewState["OrignalStockOut"] = so;
                    btnSubmit.Enabled = true;
                    ChangeFormViewMode(FormViewMode.ReadOnly, so.SOStatus);
                    grdStockDetail.DataBind();
                    grdStockDetailEdit.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "提交成功!单号：" + so.SOCode);
                }
                catch (Exception ex)
                {
                    btnSubmit.Enabled = true;
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    //lblMessage.Text = "提交至备货出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                    DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货出现异常！" + msg);                    
                }                
            }
        }

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
        }

        protected void btnCloseGoods_Click(object sender, EventArgs e)
        { 
        }
        
        //返回销售订单列表
        protected void btnReturnSales_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Order/SalesOrderMgr.aspx");
        }

        //返回转库单列表
        protected void btnReturnShift_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Order/ShiftOrderMgr.aspx");
        }

        //返回调拨订单列表
        protected void btnReturnTransfer_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Order/TransferOrderMgr.aspx");
        }

        //弹出提示框的确定按钮
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["StockOut"] != null)
                {
                    StockOut so = (StockOut)ViewState["StockOut"];
                    string soCode = so.SOCode;
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == soCode);
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
                            edm.StockOut.ApplyCurrentValues(tmp);
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

        #region 数据源控件控件，数据绑定

        public StockOut SelectStockOut()
        {
            return ViewState["StockOut"] == null ? null : ((StockOut)ViewState["StockOut"]);
        }

        protected void odsStockOut_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        protected void odsStockOut_ObjectDisposing(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        public IList<StockDetail> SelectStockDetail(string billRowNumber)
        {
            var tmp = ViewState["StockOut"] == null ? null : (StockOut)ViewState["StockOut"];
            if (tmp == null || tmp.StockDetail == null)
                return null;


            try
            {                
                var ret = tmp.StockDetail.ToList();
                if (ViewState["sortExDi"] != null)
                {
                    string[] sort = ViewState["sortExDi"].ToString().Split(',');
                    if (sort.Count() > 1)
                    {
                        string express = sort[1];
                        if (sort[0].ToString() == "ASC")
                        {
                            ret = ret.OrderBy(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                        }
                        else if (sort[0].ToString() == "DESC")
                        {
                            ret = ret.OrderByDescending(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                        }
                    }
                    //else
                    //{
                    //    ret = tmp.StockDetail.OrderBy(o => o.CargoName).ToList();
                    //}
                }
                //else
                //{
                //    ret = tmp.StockDetail.OrderBy(o => o.CargoName).ToList();
                //}
                return ret;
            }
            catch
            {
                using (var edm = new GoldEntities())
                {
                    var so = edm.StockOut.SingleOrDefault(o => o.SOCode == tmp.SOCode);
                    if (so == null)
                    {
                        return null;
                    }
                    else
                    {
                        var ret = so.StockDetail.ToList();
                        ViewState["StockOut"] = so;
                        if (ViewState["sortExDi"] != null)
                        {
                            string[] sort = ViewState["sortExDi"].ToString().Split(',');
                            if (sort.Count() > 1)
                            {
                                string express = sort[1];
                                if (sort[0].ToString() == "ASC")
                                {
                                    ret = ret.OrderBy(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                                else if (sort[0].ToString() == "DESC")
                                {
                                    ret = ret.OrderByDescending(r => r.GetType().GetProperty(express).GetValue(r, null)).ToList();
                                }
                            }                            
                        }
                        //else
                        //{
                        //    ret = tmp.StockDetail.OrderBy(o => o.CargoName).ToList();
                        //}
                        return ret;                       
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

        #region GridView操作

        protected void lbtnAddRow_Click(object sender, EventArgs e)
        {
            StockOut so;
            if (ViewState["StockOut"] != null)
                so = (StockOut)ViewState["StockOut"];
            else
                so = new StockOut();
           
            UpdateStockOut(ref so);
            so.StockDetail.Add(new StockDetail());
           
            ViewState["StockOut"] = so;            
            grdStockDetailEdit.DataBind();          
        }

        protected void lbtnDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["StockOut"] != null)
                {
                    StockOut so = (StockOut)ViewState["StockOut"];

                    //从界面获取值，更新一下所保存的si值
                    if (so != null)
                    {
                        UpdateStockOut(ref so);
                    }

                    if (so.StockDetail != null && so.StockDetail.Count > 0)
                    {
                        int count = grdStockDetailEdit.Rows.Count;                        
                        for (int i = 0; i < count; i++)
                        {
                            int j = 0;
                            if (((CheckBox)grdStockDetailEdit.Rows[i].FindControl("CheckBox1")).Checked == true)
                            {
                                string rowNO = grdStockDetailEdit.Rows[i].Cells[1].Text;//序号
                                string cargoCode = (grdStockDetailEdit.Rows[i].Cells[2].Controls[1] as TextBox).Text;//商品编号

                                foreach (StockDetail detail in so.StockDetail)
                                {
                                    if (rowNO != null && rowNO != "&nbsp;")
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
                                so.StockDetail.Remove(so.StockDetail.ElementAt(j));
                            }
                        }
                    }

                    ViewState["StockOut"] = so;
                    grdStockDetailEdit.DataBind();
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除商品发生异常！" + msg);
            }
        }

        protected void grdStockDetailEdit_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //商品编码
            if (e.CommandName == "GoodsSelect")
            {
                GridViewRow gvr = (e.CommandSource as Button).Parent.Parent as GridViewRow;
                grdStockDetailEdit.SelectedIndex = gvr.RowIndex;
                string cargoCode = ((TextBox)(grdStockDetailEdit.Rows[gvr.RowIndex].Cells[2].Controls[1])).Text.Trim();
                string cargoName = ((TextBox)(grdStockDetailEdit.Rows[gvr.RowIndex].Cells[3].Controls[1])).Text.Trim();
                string[] cargoCondition = new string[] { cargoCode, cargoName};
                GoodsSelect1.CargoQueryCondition = cargoCondition;
                GoodsSelect1.DataBindForQuery();
                this.popWindowGoods.Show();
            }

            //区位选择
            if (e.CommandName == "BinCodeSelect")
            {
                GridViewRow gvr = (e.CommandSource as Button).Parent.Parent as GridViewRow;
                grdStockDetailEdit.SelectedIndex = gvr.RowIndex;
                string cargoCode=((TextBox)(grdStockDetailEdit.Rows[gvr.RowIndex].Cells[2].Controls[1])).Text.Trim();
                if(string.IsNullOrEmpty(cargoCode))
                {
                    ScriptManager.RegisterStartupScript(grdStockDetailEdit, this.GetType(), "Fail", "alert('请先选择要出库的商品编码！');", true);
                    return;
                }

                BinCodeSelect1.CargoCode = cargoCode;
                BinCodeSelect1.DataBind1();
                this.popWindow.Show();
            }
        }

        //仓库区位信息
        protected void BinCodeSelect1_PostBack(object sender, EventArgs e)
        {
            if (!BinCodeSelect1.ShowPop)
            {
                this.popWindow.Show();
                return;
            }   
        }

        //区位选择返回值
        protected void BinCodeSelect1_OnGetBinCodeSelect(object sender, EventArgs e)
        {            
            if (BinCodeSelect1.ListSelectedBinCode != null)
            {
                for (int i = 0; i < BinCodeSelect1.ListSelectedBinCode.Count; i++)
                {
                    string[] binCodeSelect = BinCodeSelect1.ListSelectedBinCode[i];
                    if (i == 0)
                    {
                        GridViewRow gvr = grdStockDetailEdit.SelectedRow;
                        string binCode = binCodeSelect[0];
                        (gvr.Cells[10].Controls[1] as TextBox).Text = binCode;
                        gvr.Cells[15].Text = binCodeSelect[1];//仓库编码
                        gvr.Cells[16].Text = binCodeSelect[2];//仓库名称
                    }
                    else
                    {
                        //根据选择的区位个数，新增行项目 
                        StockOut so;
                        if (ViewState["StockOut"] != null)
                            so = (StockOut)ViewState["StockOut"];
                        else
                            so = new StockOut();

                        UpdateStockOut(ref so);

                        StockDetail sd = new StockDetail();
                        GridViewRow gvr = grdStockDetailEdit.SelectedRow;
                        sd.CargoCode = (gvr.Cells[2].Controls[1] as TextBox).Text.Trim() == string.Empty ? "" : (gvr.Cells[2].Controls[1] as TextBox).Text;
                        sd.CargoName = (gvr.Cells[3].Controls[1] as TextBox).Text.Trim() == string.Empty ? "" : (gvr.Cells[3].Controls[1] as TextBox).Text;
                        //商品规格
                        if (gvr.Cells[4].Text.Trim() != string.Empty && (!gvr.Cells[4].Text.Contains("&nbsp;")))
                        {
                            sd.CargoSpec = gvr.Cells[4].Text.Trim();
                        }
                        //商品型号
                        if (gvr.Cells[5].Text.Trim() != string.Empty && (!gvr.Cells[5].Text.Contains("&nbsp;")))
                        {
                            sd.CargoModel = gvr.Cells[5].Text.Trim();
                        }
                        if (gvr.Cells[6].Text.Trim() != string.Empty && (!gvr.Cells[6].Text.Contains("&nbsp;")))
                        {
                            sd.CargoUnits = gvr.Cells[6].Text.Trim();
                        }
                        if (gvr.Cells[14].Text.Trim() != string.Empty && (!gvr.Cells[16].Text.Contains("&nbsp;")))
                        {
                            sd.ReleaseYear = gvr.Cells[14].Text.Trim();
                        }
                        sd.BinCode = binCodeSelect[0];
                        sd.InOutWHCode = binCodeSelect[1];//仓库编码
                        sd.InOutWHName = binCodeSelect[2];//仓库名称
                       
                        so.StockDetail.Add(sd);

                        ViewState["StockOut"] = so;
                        grdStockDetailEdit.DataBind(); 
                    }
                }
            }      
        }

        //商品相关信息
        protected void GoodsSelect1_PostBack(object sender, EventArgs e)
        {
            if (!GoodsSelect1.ShowPop)
            {
                this.popWindowGoods.Show();
                return;
            }
        }

        //商品选择返回值
        protected void GoodsSelect1_GetCargoSelect(object sender, EventArgs e)
        {            
            for (int i = 0; i < GoodsSelect1.ListSelectedCargo.Count; i++)
            {
                string[] goodSelect = GoodsSelect1.ListSelectedCargo[i];
                if (i == 0)
                {
                    GridViewRow gvr = grdStockDetailEdit.SelectedRow;
                    string binCode = goodSelect[0];
                    (gvr.Cells[2].Controls[1] as TextBox).Text = binCode;//商品编码
                    (gvr.Cells[3].Controls[1] as TextBox).Text = goodSelect[1];//商品名称
                    gvr.Cells[4].Text = goodSelect[3];//规格
                    gvr.Cells[5].Text = goodSelect[2];//型号
                    gvr.Cells[6].Text = goodSelect[4];//单位
                    gvr.Cells[14].Text = goodSelect[5];//发行年份

                    //设置默认区位
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == binCode).ToList();
                        //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                        if (tmpBinCoe.Count > 0)
                        {                            
                            (gvr.Cells[10].Controls[1] as TextBox).Text = tmpBinCoe[0].BinCode;//区位
                            gvr.Cells[15].Text = tmpBinCoe[0].WHCode;//出货仓库编码
                            gvr.Cells[16].Text = tmpBinCoe[0].WHName;//出货仓库名称
                        }
                    }                   
                }
                else
                {
                    //根据选择的商品个数，新增行项目
                    StockOut so;
                    if (ViewState["StockOut"] != null)
                        so = (StockOut)ViewState["StockOut"];
                    else
                        so = new StockOut(); 

                    UpdateStockOut(ref so);

                    StockDetail sd = new StockDetail();
                    string binCode = goodSelect[0];
                    sd.CargoCode = binCode;//商品编码
                    sd.CargoName = goodSelect[1];//商品编码
                    sd.CargoModel = goodSelect[2];//型 号
                    sd.CargoSpec = goodSelect[3];//规格
                    sd.CargoUnits = goodSelect[4];//单位
                    //sd.CargoStatus = Convert.ToInt32(goodSelect[5]);//状态
                    sd.ReleaseYear = goodSelect[5];//发行年份
                    sd.BillRowNumber = (so.StockDetail.Count + 1).ToString();//行项目号

                    //设置默认区位
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == binCode).ToList();
                        //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                        if (tmpBinCoe.Count > 0)
                        {
                            sd.BinCode = tmpBinCoe[0].BinCode;
                            sd.InOutWHCode = tmpBinCoe[0].WHCode;
                            sd.InOutWHName = tmpBinCoe[0].WHName;
                        }
                    }

                    so.StockDetail.Add(sd);                    

                    ViewState["StockOut"] = so;
                    grdStockDetailEdit.DataBind();                   
                }

                
            }
        }

        #endregion

        //选择商品按钮是否显示的判断
        protected bool GetButtonVisible()
        {
            StockOut so = (StockOut)ViewState["StockOut"];
            if (so == null) return true;
            //如果是来源于订单，且来源订单号不为空，则不能改变行项目的商品信息，只能删除或者增加
            if ((so.FromOrderNo != "" && so.FromOrderNo != null && so.FromType != "" && so.FromType == "03")//03:源于手工新增的出库单
               || (so.FromType == "02"))
            {               
                return false;
            }
            else
            {               
                return true;
            }
        }        

        //设置默认值或输入框的只读性
        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            try
            {
                //设置来源单号输入框为只读
                if (FormView1.CurrentMode == FormViewMode.Edit || FormView1.CurrentMode == FormViewMode.Insert)
                {
                    DropDownList drpFromType = FormView1.FindControl("ctlFromType") as DropDownList;
                    TextBox txtFromUCOrderNo = FormView1.FindControl("ctlFromUCOrderNo") as TextBox;
                    DropDownList drpSOType = FormView1.FindControl("ctlSOType") as DropDownList;//单据类型
                    TextBox txtFromBillNo = FormView1.FindControl("ctlFromBillNo") as TextBox;//源Excel出库单号
                    if ((drpFromType.SelectedValue == "01" && drpFromType.SelectedIndex != 0 && txtFromUCOrderNo.Text.Trim() != string.Empty)
                        || (drpFromType.SelectedValue == "02"))
                    {
                        txtFromUCOrderNo.Enabled = false;
                        drpFromType.Enabled = false;
                        drpSOType.Enabled = false;
                        txtFromBillNo.Enabled = false;

                        //设置添加商品、删除所选商品按钮的隐藏
                        lbtnAddRow.Visible = false;
                        lbtnDeleteRow.Visible = false;
                    }
                }

                //设置出库日期的默认值--今天
                if (FormView1.CurrentMode != FormViewMode.ReadOnly)
                {
                    TextBox txtSODate = FormView1.FindControl("ctlSODate") as TextBox;
                    if (txtSODate.Text.Trim() == string.Empty)
                    {
                        txtSODate.Text = System.DateTime.Now.ToShortDateString();
                    }
                }

                //设置库管员默认值--古燕            
                DropDownList drpStorageMan = FormView1.FindControl("ctlStorageMan") as DropDownList;
                TextBox txtStorageMan = FormView1.FindControl("txtStorageMan") as TextBox;
                if (drpStorageMan != null)
                {
                    if (txtStorageMan != null && txtStorageMan.Text.Trim() != string.Empty && drpStorageMan.Items.Count > 1)
                    {
                        drpStorageMan.SelectedIndex = drpStorageMan.Items.IndexOf(drpStorageMan.Items.FindByValue(txtStorageMan.Text));
                    }

                    if (drpStorageMan.SelectedIndex == 0)
                    {
                        drpStorageMan.SelectedValue = "古燕";
                    }
                }

                //设置仓库默认值--地王26库            
                DropDownList drpWHName = FormView1.FindControl("ctlWHName") as DropDownList;
                TextBox txtWHName = FormView1.FindControl("txtWHName") as TextBox;
                if (drpWHName != null)
                {
                    if (txtWHName != null && txtWHName.Text.Trim() != string.Empty && drpWHName.Items.Count > 1)
                    {
                        drpWHName.SelectedIndex = drpWHName.Items.IndexOf(drpWHName.Items.FindByValue(txtWHName.Text));
                    }

                    if (drpWHName.SelectedIndex == 0)
                    {
                        string defaultWHCode = string.Empty;
                        //从配置文件读取默认的仓库--地王26库
                        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                        {
                            defaultWHCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                        }
                        drpWHName.SelectedValue = defaultWHCode;
                    }
                }

                //设置部门默认值--物流部
                DropDownList ddlDepartmentName = FormView1.FindControl("ctlSellDepartmentName") as DropDownList;
                TextBox txtSellDepartmentName = FormView1.FindControl("txtSellDepartmentName") as TextBox;
                if (ddlDepartmentName != null)
                {
                    if (txtSellDepartmentName != null && txtSellDepartmentName.Text != "" && ddlDepartmentName.Items.Count > 1)
                    {
                        ddlDepartmentName.SelectedIndex = ddlDepartmentName.Items.IndexOf(ddlDepartmentName.Items.FindByText(txtSellDepartmentName.Text));
                    }

                    if (ddlDepartmentName.SelectedIndex == 0)
                    {
                        ddlDepartmentName.SelectedValue = "物流部";
                    }
                    
                }

                //已绑定数据时设置选中项--业务类型
                DropDownList ddlBusinessType = FormView1.FindControl("ctlBusinessType") as DropDownList;
                TextBox txtBusinessType_Selected = FormView1.FindControl("txtBusinessType_Selected") as TextBox;
                if (ddlBusinessType != null)
                {
                    if (txtBusinessType_Selected != null && txtBusinessType_Selected.Text != "" && ddlBusinessType.Items.Count > 1)
                    {
                        ddlBusinessType.SelectedIndex = ddlBusinessType.Items.IndexOf(ddlBusinessType.Items.FindByText(txtBusinessType_Selected.Text));
                    }
                }

                //已保定数据时设置选中项-业务员
                DropDownList ddlBussinessMan = FormView1.FindControl("ctlBussinessMan") as DropDownList;
                TextBox txtBussinessMan = FormView1.FindControl("txtBussinessMan") as TextBox;
                if (ddlBussinessMan != null)
                {
                    if (txtBussinessMan != null && txtBussinessMan.Text != "" && ddlBussinessMan.Items.Count > 1)
                    {
                        ddlBussinessMan.SelectedIndex = ddlBussinessMan.Items.IndexOf(ddlBussinessMan.Items.FindByText(txtBussinessMan.Text));
                    }
                }

                //已绑定数据时设置选中项-收发类别
                DropDownList ddlOutCategory = FormView1.FindControl("ctlOutCategory") as DropDownList;
                TextBox txtOutCategory = FormView1.FindControl("txtOutCategory") as TextBox;
                if (ddlOutCategory != null)
                {
                    if (txtOutCategory != null && txtOutCategory.Text != "" && ddlOutCategory.Items.Count > 1)
                    {
                        ddlOutCategory.SelectedIndex = ddlOutCategory.Items.IndexOf(ddlOutCategory.Items.FindByValue(txtOutCategory.Text));
                    }
                }

                //单据类型
                DropDownList ddlSOType = FormView1.FindControl("ctlSOType") as DropDownList;
                TextBox txtSOType = FormView1.FindControl("txtSOType") as TextBox;
                if (ddlSOType != null)
                {
                    if (txtSOType != null && txtSOType.Text.Trim() != string.Empty && ddlSOType.Items.Count > 1)
                    {
                        ddlSOType.SelectedIndex = ddlSOType.Items.IndexOf(ddlSOType.Items.FindByValue(txtSOType.Text));
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "绑定数据出现异常！" + msg);
            }
        }
        
        protected void grdStockDetailEdit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtcargoCode = e.Row.FindControl("ctlCargoCode") as TextBox;
                TextBox txtBinCode = e.Row.FindControl("txtBinCode") as TextBox;
                if (txtcargoCode != null && txtBinCode != null)
                {
                    //如果商品编码不为空且区位为空，则设置默认值
                    if (txtcargoCode.Text.Trim() != string.Empty && txtBinCode.Text.Trim() == string.Empty)
                    {
                        using (var edm = new Gold.DAL.GoldEntities())
                        {
                            string cargoCode = txtcargoCode.Text.Trim();
                            var tmpBinCoe = edm.VBinCode.Where(o => o.CargoCode == cargoCode).ToList();
                            //设定查询结果的其中一个为默认值，因为一个商品有可能在多个位置
                            if (tmpBinCoe.Count > 0)
                            {
                                txtBinCode.Text = tmpBinCoe[0].BinCode;
                                e.Row.Cells[15].Text = tmpBinCoe[0].WHCode;
                                e.Row.Cells[13].Text = tmpBinCoe[0].WHName;
                            }
                        }
                    }
                }
            }
        }

        //获取当前订单当前商品的应发数量
        public double GetCargosNumCurrentPlan(string cargoCode, string fromOrderNo, double? numOriginalPlan, string detailRowNo)
        {
            double numCurrentPlan = 0;
            double numOriginal = numOriginalPlan == null ? 0 : Convert.ToDouble(numOriginalPlan);
            using (var edm = new Gold.DAL.GoldEntities())
            {
                //根据订单编号和商品号，查询当前的应发数量
                var tmp = edm.VSelectAllOutCancelDetail.Where(o => (o.FromOrderNo == fromOrderNo && o.CargoCode == cargoCode && o.BillRowNumber == detailRowNo)).ToList();
                if (tmp.Count != 0)
                {
                    //已使用的应发数量
                    double numCurrent = tmp[0].sumnumCurrentPlan == null ? 0 : Convert.ToDouble(tmp[0].sumnumCurrentPlan);
                    //应发数量=订单数量-已使用的应发数量
                    numCurrentPlan = numOriginal - numCurrent;
                }
                else
                {
                    numCurrentPlan = numOriginal;
                }
            }

            return numCurrentPlan;
        }
    }
}