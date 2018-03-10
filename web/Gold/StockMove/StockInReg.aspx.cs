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
    public partial class StockInReg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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

                    if (sourceType == "stockin")
                    {
                        using (var edm = new Gold.DAL.GoldEntities())
                        {
                            var tmp = edm.StockIn.Include("StockDetail").Where(o => o.SICode == sourceCode).ToList();
                            
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
                                Server.Transfer("../error.aspx?errorInfo=无此入库单记录。入库单号=" + sourceCode);
                            else
                            {
                                //绑定数据
                                ViewState["StockIn"] = tmp[0];
                                ViewState["OrignalStockIn"] = tmp[0];
                                FormView1.ChangeMode(FormViewMode.ReadOnly);
                                btnSave.Visible = false;
                                btnReset.Visible = false;
                                btnReturn.Visible = true;
                                lbtnAddRow.Visible = false;
                                lbtnDeleteRow.Visible = false;
                                GridView1.Visible = false;
                                GridView2.Visible = true;
                                if (tmp[0].SIStatus < 2 && tmp[0].WHCode == warehouseCode)
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

                    if (sourceType == "purchaseorder")
                    {
                        StockIn si = CreateStockInFromPurchaseOrder(sourceCode, sourceSort);
                        ViewState["StockIn"] = si;
                        ViewState["OrignalStockIn"] = si;
                        SwitchToInsertMode();
                        return;
                    }
                    if (sourceType == "shiftorder")
                    {
                        StockIn so = CreateStockInFromShiftOrder(sourceCode, sourceSort);
                        ViewState["StockIn"] = so;
                        ViewState["OrignalStockIn"] = so;
                        SwitchToInsertMode();
                        return;
                    }

                    if (sourceType == "transferorder")
                    {
                        StockIn sa = CreateStockInFromTransferOrder(sourceCode, sourceSort);
                        ViewState["StockIn"] = sa;
                        ViewState["OrignalStockIn"] = sa;
                        SwitchToInsertMode();
                        return;
                    }                    
                }

                //编辑控件赋值
                if (ViewState["StockIn"] != null)
                {
                    StockIn si = (StockIn)ViewState["StockIn"];
                    lblEditorID.Text = si.EditorID;
                    lblEditorName.Text = si.EditorName;
                }
            }
            catch (Exception ex)
            {
                string msg = Utility.LogHelper.GetExceptionMsg(ex);
                msg = msg.Replace("\r\n", "");
                DAL.CommonConvert.ShowMessageBox(this.Page, "出现异常！" + msg);
            }
        }

        //采购入库单
        private StockIn CreateStockInFromPurchaseOrder(string sourceCode, string sourceSort)
        {
            StockIn si = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.PurchaseOrder.Where(o => o.OrderCode == sourceCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此采购订单记录。采购订单号=" + sourceCode);
                else
                {
                    si = edm.StockIn.CreateObject();
                    si.SIDate = DateTime.Now;
                    si.SIType = CommonConvert.GetSIOTypeCode("采购入库单");
                    //si.FromBillType = CommonConvert.GetBillTypeCode("");
                    si.FromOrderType = "02";//源于采购订单
                    si.FromOrderNo = tmp.OrderCode;
                    si.FromType = CommonConvert.GetFromTypeCode("源于订单");
                    si.FromUCOrderNo = tmp.NCOrderCode;
                    si.SIStatus = 0;
                    //si.BusinessDepartmentName
                    //si.Checker
                    si.Comment = tmp.Comment;
                    //si.FromBillNo
                    si.LastModifyTime = tmp.LastEditTime;
                    si.Operator = tmp.Buyer;
                    si.Reserve1 = tmp.Reserve1;
                    si.Reserve2 = tmp.Reserve2;
                    si.Supplier = tmp.Supplier;//供应商
                    //si.TotalNumber
                    //si.WHCode
                    //si.WHName
                    si.RFIDActorID = Convert.ToString(tmp.RFIDActorID);
                    si.RFIDActorName = tmp.RFIDActorName;
                    si.RFIDActorTime = tmp.RFIDActorTime;

                    si.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
                    if (tmp.PurchaseOrderDetail != null && tmp.PurchaseOrderDetail.Count > 0)
                    {
                        List<PurchaseOrderDetail> tmpdetail=tmp.PurchaseOrderDetail.ToList();
                       
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

                        foreach (PurchaseOrderDetail item in tmpdetail)
                        {
                            //如果实收数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }
                            StockDetail detail = edm.StockDetail.CreateObject(); //new StockDetail();
                            detail.BillRowNumber = item.DetailRowNumber;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec;
                            //detail.CargoStatus = item.
                            detail.CargoUnits = item.CargoUnits;
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量                             
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //应收数量=订单数量-实际已收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual;
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应收数量
                            detail.ReleaseYear = item.ReleaseYear;
                            detail.RFIDOrderNo = item.OrderCode;
                            detail.SICode = si.SICode;
                            detail.UCOrderNo = si.FromUCOrderNo;
                            detail.BillType = "02";//入库单
                            detail.InOutWHCode = item.WHCode == null ? "" : item.WHCode;//收货仓库
                            detail.InOutWHName = item.WHName == null ? "" : item.WHName;

                            si.StockDetail.Add(detail);
                        }

                    }                        
                }
            }

            return si;
        }        

        //调拨单
        private StockIn CreateStockInFromTransferOrder(string sourceCode, string sourceSort)
        {
            StockIn si = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.TransferOrder.Where(o => o.OrderCode == sourceCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此采购订单记录。调拨单号=" + sourceCode);
                else
                {
                    si = edm.StockIn.CreateObject();
                    si.SIDate = DateTime.Now;
                    si.SIType = CommonConvert.GetSIOTypeCode("调拨入库单");
                    si.FromOrderNo = tmp.OrderCode;
                    si.FromType = CommonConvert.GetFromTypeCode("源于订单");
                    si.FromOrderType = "03";//源于调拨单
                    si.FromUCOrderNo = tmp.NCOrderCode;
                    si.SIStatus = 0;
                    si.BusinessDepartmentName = tmp.BusinessDepartmentName;//部门
                    si.Checker = tmp.Checker;
                    si.Comment = tmp.Comment;
                    //si.FromBillNo
                    si.LastModifyTime = tmp.LastModifyTime;
                    si.Operator = tmp.Operator;
                    si.Reserve1 = tmp.Reserve1;
                    si.Reserve2 = tmp.Reserve2;
                    si.Supplier = tmp.Supplier;//供应商
                    si.TotalNumber = tmp.TotalNumber;
                    si.StoreKeeper = tmp.StoreKeeper;//库管员
                    si.WHCode = tmp.InWHCode;//仓库编码
                    si.WHName = tmp.InWHName;
                    si.RFIDActorID = Convert.ToString(tmp.RFIDActorID);
                    si.RFIDActorName = tmp.RFIDActorName;
                    si.RFIDActorTime = tmp.RFIDActorTime;

                    si.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
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
                            //如果实收数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }
                            StockDetail detail = edm.StockDetail.CreateObject(); //new StockDetail();
                            detail.BillRowNumber = item.DetailRowNumber;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec;
                            detail.CargoUnits = item.CargoUnits;
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);                            
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);
                            //应收数量=订单数量-实际已收数量
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual;  
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应收数量
                            detail.ReleaseYear = item.ReleaseYear;
                            detail.RFIDOrderNo = item.OrderCode;
                            detail.SICode = si.SICode;
                            detail.UCOrderNo = si.FromUCOrderNo;
                            detail.BillType = "02";                            

                            si.StockDetail.Add(detail);
                        }
                    }
                }
            }

            return si;
        }

        //转库单
        private StockIn CreateStockInFromShiftOrder(string sourceCode, string sourceSort)
        {
            StockIn si = null;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.ShiftOrder.Where(o => o.OrderCode == sourceCode).SingleOrDefault();
                if (tmp == null)
                    Server.Transfer("../error.aspx?errorInfo=无此采购订单记录。转库单号=" + sourceCode);
                else
                {
                    si = edm.StockIn.CreateObject();
                    si.SIDate = DateTime.Now;
                    si.SIType = CommonConvert.GetSIOTypeCode("其他入库单");
                    si.FromOrderNo = tmp.OrderCode;
                    si.FromType = CommonConvert.GetFromTypeCode("源于订单");
                    si.FromOrderType = "04";//源于转库单
                    si.FromUCOrderNo = tmp.NCOrderCode;
                    si.SIStatus = 0;
                    //si.BusinessDepartmentName = ;//部门
                    //si.Checker = tmp.Checker;
                    si.Comment = tmp.Comment;
                    //si.FromBillNo
                    //si.LastModifyTime = 
                    //si.Operator = tmp.//业务员
                    si.Reserve1 = tmp.Reserve1;
                    si.Reserve2 = tmp.Reserve2;
                    //si.Supplier //供应商
                    si.TotalNumber = tmp.TotalNumber;
                    si.StoreKeeper = tmp.ReceiverName;//库管员==收货人
                    si.WHCode = tmp.InWHCode;//仓库编码
                    si.WHName = tmp.InWHName;
                    si.RFIDActorID = Convert.ToString(tmp.RFIDActorID);
                    si.RFIDActorName = tmp.RFIDActorName;
                    si.RFIDActorTime = tmp.RFIDActorTime;

                    si.StockDetail = new System.Data.Objects.DataClasses.EntityCollection<StockDetail>();
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
                            //如果实收数量==订单数量，则不显示在界面上，即不用再进行提交备货了
                            //如果行项目状态全部已转，也不用显示界面
                            if (item.NumActual == item.NumOriginalPlan || item.DetailRowStatus == 3 || item.DetailRowStatus == 0)
                            {
                                continue;
                            }
                            StockDetail detail = edm.StockDetail.CreateObject(); //new StockDetail();
                            detail.BillRowNumber = item.DetailRowNumber;
                            detail.CargoCode = item.CargoCode;
                            detail.CargoModel = item.CargoModel;
                            detail.CargoName = item.CargoName;
                            detail.CargoSpec = item.CargoSpec;
                            //detail.CargoStatus = item.
                            detail.CargoUnits = item.CargoUnits;
                            detail.NumOriginalPlan = item.NumOriginalPlan == null ? 0 : (double)(item.NumOriginalPlan);                            
                            //detail.NumActual = item.NumActual == null ? 0 : (double)(item.NumActual);
                            //应收数量=订单数量-实际已收数量
                            //double numActual = item.NumActual == null ? 0 : (double)(item.NumActual);//实收数量
                            //detail.NumCurrentPlan = detail.NumOriginalPlan - numActual; 
                            detail.NumCurrentPlan = GetCargosNumCurrentPlan(item.CargoCode, item.OrderCode, item.NumOriginalPlan, item.DetailRowNumber);//应收数量
                            detail.ReleaseYear = item.ReleaseYear;
                            detail.RFIDOrderNo = item.OrderCode;
                            detail.SICode = si.SICode;
                            detail.UCOrderNo = si.FromUCOrderNo;
                            detail.BillType = "02";//入库单

                            si.StockDetail.Add(detail);
                        }
                    }
                }
            }

            return si;
        }

        private void UpdateStockIn(ref StockIn si)
        {
            if (FormView1.CurrentMode == FormViewMode.Insert || FormView1.CurrentMode == FormViewMode.Edit)
            {
                if (si.SIStatus == 0)
                {
                    si.SIStatus = 1; //已保存 
                }
                si.SIType = ((DropDownList)FormView1.FindControl("ctlSIType")).SelectedValue;

                DateTime da;
                if (!string.IsNullOrEmpty(((TextBox)FormView1.FindControl("ctlSIDate")).Text) &&
                    DateTime.TryParse(((TextBox)FormView1.FindControl("ctlSIDate")).Text, out da))
                    si.SIDate = da;
                DropDownList ddlWH = (DropDownList)FormView1.FindControl("ctlWHName");
                if (ddlWH.SelectedIndex != 0)
                {
                    si.WHName = ddlWH.Items[ddlWH.SelectedIndex].Text.ToString();
                    si.WHCode = ddlWH.SelectedValue;
                }                

                //源Excel入库单号
                TextBox ctlFromBillNo = ((TextBox)FormView1.FindControl("ctlFromBillNo"));
                if (ctlFromBillNo != null)
                {
                    if (ctlFromBillNo.Text.Trim() != string.Empty)
                    {
                        si.FromBillNo = ctlFromBillNo.Text.Trim();
                    }
                }

                //用友参考单号
                TextBox ctlFromUCOrderNo = (TextBox)FormView1.FindControl("ctlFromUCOrderNo");
                if (ctlFromUCOrderNo != null)
                {
                    if (ctlFromUCOrderNo.Text.Trim() != string.Empty)
                    {
                        si.FromUCOrderNo = ctlFromUCOrderNo.Text.Trim();
                    }
                }

                if (si.FromBillNo != null && si.FromUCOrderNo != null)
                {
                    if (si.FromBillNo.ToString() != string.Empty && si.FromUCOrderNo.ToString() != string.Empty)
                    {
                        throw new Exception("用友参考单号和源Excel出库单号，不能同时有值！");
                    }
                }

                si.FromType = ((DropDownList)FormView1.FindControl("ctlFromType")).SelectedValue;
                si.Supplier = ((DropDownList)FormView1.FindControl("ctlSupplier")).SelectedValue;
                si.Operator = ((DropDownList)FormView1.FindControl("ctlOperator")).SelectedValue;
                si.StoreKeeper = ((DropDownList)FormView1.FindControl("ctlStoreKeeper")).SelectedValue;
                //si.Checker = ((TextBox)FormView1.FindControl("ctlChecker")).Text;

                DropDownList ddlInCategory = (DropDownList)FormView1.FindControl("ctlInCategory");
                if (ddlInCategory.SelectedIndex != 0)
                {
                    si.InCategory = ddlInCategory.SelectedValue;
                }                

                si.Comment = ((TextBox)FormView1.FindControl("ctlComment")).Text;

                //单据创建信息
                if (Session["UserInfo"] != null)
                {
                    Users userInfo = (Users)Session["UserInfo"];
                    si.RFIDActorID = userInfo.UserId;
                    si.RFIDActorName = userInfo.UserName;
                }
                si.RFIDActorTime = System.DateTime.Now;
            }

            if (si.SIStatus == 2 && FormView1.CurrentMode == FormViewMode.ReadOnly)
            {
                foreach (StockDetail sd in si.StockDetail)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime
                }
                ViewState["StockIn"] = si;
                return;
            }

            //绑定行项目
            if (si.StockDetail == null)
                si.StockDetail = new EntityCollection<StockDetail>();
            else
                si.StockDetail.Clear();

            //获取当前最大的行项目值
            int rowNo = GridView1.Rows.Count - 1;
            int rowNoMax = 0;
            for (int j = 0; j < GridView1.Rows.Count - 1; j++)
            {
                GridViewRow r = GridView1.Rows[j];
                int No;
                if (int.TryParse(r.Cells[1].Text.ToString(), out No))
                {
                    if (rowNoMax < No)
                    {
                        rowNoMax = No;
                    }
                }
            }

            //编辑、插入模式才会继续往下走
            double total = 0;
            for (int i = GridView1.Rows.Count - 1; i >= 0; i--)
            {
                GridViewRow r = GridView1.Rows[i];
                //如果是源于订单，且用友参考单号不为空，则需要判断是否被选中，是有选中的行项目，才会保存提交
                //或源于excel导入的入库单
                if ((si.FromType == "01" && si.FromType != "" && si.FromUCOrderNo != null)
                    || (si.FromType == "02"))
                {
                    if (!(r.Cells[0].Controls[1] as CheckBox).Checked)
                        continue;
                }

                StockDetail sd = new StockDetail();
                sd.BillCode = si.SICode;
                //sd.BillRowNumber = (i + 1).ToString();
                if (r.Cells[1].Text.ToString() != string.Empty && r.Cells[1].Text.ToString() != "&nbsp;" && (si.FromType == "01" || si.FromType == "02"))//行号
                {
                    sd.BillRowNumber = r.Cells[1].Text;
                }
                else
                {
                    sd.BillRowNumber = (i + 1).ToString();
                }
                //else if (r.Cells[1].Text.ToString() == "&nbsp;")
                //{
                //    rowNo += 1;
                //    sd.BillRowNumber = rowNo.ToString();
                //}
                //else
                //{
                //    rowNoMax += 1;
                //    sd.BillRowNumber = rowNoMax.ToString();
                //}

                //var tmp = r.Cells[2].Controls[0] as TextBox;
                //var tmp2 = r.Cells[3];
                sd.CargoStatus = 0;//0:手持机流程还未完成
                sd.CargoCode = (r.Cells[2].Controls[1] as TextBox).Text;
                sd.CargoName = (r.Cells[3].Controls[1] as TextBox).Text;
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

                //源于手工新增的,订单数量=应收数量
                if (si.SIType == "23" && si.FromType == "03")
                {
                    sd.NumOriginalPlan = sd.NumCurrentPlan;
                }

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
                //if (r.Cells[14].Text.Trim() != string.Empty && r.Cells[14].Text != "&nbsp;")
                //{
                //    sd.CargoStatus = Convert.ToInt32(r.Cells[14].Text);//商品状态
                //}

                sd.ReleaseYear = r.Cells[15].Text == "&nbsp;" ? "" : r.Cells[15].Text;//发行年份

                sd.SICode = si.SICode;
                sd.BillType = "02";//01-出库单，02-入库单，03-撤销入库单（即出库），04-撤销出库单（即入库）
                if (si.SIStatus == 2)
                {
                    sd.RFIDSubmitTime = System.DateTime.Now;//RFIDSubmitTime
                }

                sd.StockIn = si;
                si.StockDetail.Add(sd);
            }

            //合计数量
            si.TotalNumber = total;

            ViewState["StockIn"] = si;
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

        #region 页面模式切换

        //保存
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                try
                {
                    StockIn si;
                    if (ViewState["StockIn"] != null)
                        si = (StockIn)ViewState["StockIn"];
                    else
                        si = new StockIn();
                    si.SICode = KeyGenerator.Instance.GetStockInKey();
                    si.FromType = "03";
                    si.SIStatus = 1;
                    UpdateStockIn(ref si);

                    si.LastModifyTime = System.DateTime.Now;//最后修改时间

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
                    if (si.WHCode != warehouseCode)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        foreach (StockDetail sd in si.StockDetail)
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
                            if (sd.InOutWHCode != warehouseCode)
                            {
                                DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                return;
                            }
                        }

                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应收数量=Sum（所有detail应收数量）-撤销单的应收数量-手持机尚未开始操作被撤销的入库单应收数量
                        if (si.FromOrderNo != null && si.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockIns = edmOrder.StockIn.Where(o => (o.FromOrderNo == si.FromOrderNo && o.SICode != si.SICode)).ToList();
                            if (si.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);                                
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
                                        {
                                            //判断商品和序号是否相等
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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

                                        tmpSalesOrder.EditorID = userInfo.UserId;
                                        tmpSalesOrder.EditorName = userInfo.UserName;
                                    }
                                    tmpSalesOrder.RFIDActorTime = System.DateTime.Now;
                                    tmpSalesOrder.EditTime = System.DateTime.Now;
                                    tmpSalesOrder.EditStatus = 0;//无人编辑

                                    edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                                }
                            }
                            else if (si.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);                                
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;
                                    
                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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
                                        if (purchaseOrderDetail.DetailRowStatus == 4 || purchaseOrderDetail.DetailRowStatus==2 || purchaseOrderDetail.DetailRowStatus == null)
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
                            else if (si.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);                               
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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
                                    tmpTransferOrder.EditStatus = 0;//无人编辑状态

                                    edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                                }
                            }
                            else if (si.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);                               
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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

                        
                        edm.StockIn.AddObject(si);
                        edm.SaveChanges();
                    }

                    ViewState["StockIn"] = si;
                    ViewState["OrignalStockIn"] = si;   
                    ChangeFormViewMode(FormViewMode.ReadOnly, si.SIStatus);
                    GridView2.DataBind();
                    GridView1.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "入库单保存成功! 单号：" + si.SICode);
                    //DAL.CommonConvert.ShowMessageBox(this.Page, "入库单保存成功! 单号：" + si.SICode + "<br/><br/>(该单已保存但未提交至备货模块，若要提交至备货模块请<br/>在入库单编辑界面提交！)");
                }
                catch (Exception ex)
                {
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存数据出现异常！" + msg);
                    //lblMessage.Text = "保存数据出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
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
                    StockIn si;
                    ////不重新获取viewstate的数据，是因为按提交的时候，状态已经变为2，会影响到更新的状态
                    if (ViewState["StockIn"] != null)
                    {
                        si = (StockIn)ViewState["StockIn"];
                        si.SIStatus = 1;
                    }
                    else
                    {
                        si = new StockIn();
                        si.SICode = KeyGenerator.Instance.GetStockInKey();
                        si.SIType = "03";
                    }

                    UpdateStockIn(ref si);

                    si.LastModifyTime = System.DateTime.Now;//最后修改时间

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
                    if (si.WHCode != warehouseCode)
                    {
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    foreach (StockDetail sd in si.StockDetail)
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
                        if (sd.InOutWHCode != warehouseCode)
                        {
                            DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                            return;
                        }
                    }

                    //增加修改编辑字段的数据
                    if (Session["UserInfo"] != null)
                    {
                        Users userInfo = (Users)Session["UserInfo"];                        
                        si.EditorID = userInfo.UserId;
                        si.EditorName = userInfo.UserName;
                    }                   
                    si.EditTime = System.DateTime.Now;
                    si.EditStatus = 0;//无人编辑

                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
                        if (tmp == null)
                        {

                            edm.StockIn.AddObject(si);
                        }
                        else
                        {
                            edm.StockIn.DeleteObject(tmp);
                            edm.SaveChanges();
                            edm.StockIn.AddObject(si);
                        }

                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应收数量=Sum（所有detail应收数量）-撤销单的应收数量-手持机尚未开始操作被撤销的入库单应收数量
                        if (si.FromOrderNo != null && si.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockIns = edmOrder.StockIn.Where(o => (o.FromOrderNo == si.FromOrderNo && o.SICode != si.SICode)).ToList();
                            if (si.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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

                                        tmpSalesOrder.EditorID = userInfo.UserId;
                                        tmpSalesOrder.EditorName = userInfo.UserName;
                                    }
                                    tmpSalesOrder.RFIDActorTime = System.DateTime.Now;
                                    tmpSalesOrder.EditTime = System.DateTime.Now;
                                    tmpSalesOrder.EditStatus = 0;//无人编辑

                                    edm.SalesOrder.ApplyCurrentValues(tmpSalesOrder);
                                }
                            }
                            else if (si.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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
                            else if (si.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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
                                    tmpTransferOrder.EditStatus = 0;//无人编辑状态

                                    edm.TransferOrder.ApplyCurrentValues(tmpTransferOrder);
                                }
                            }
                            else if (si.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                                                        for (int i = 0; i < tmpStockIns.Count; i++)
                                                        {
                                                            StockIn stockin = tmpStockIns[i];
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

                        edm.SaveChanges();
                    }

                    ViewState["OrignalStockIn"] = si;
                    ChangeFormViewMode(FormViewMode.ReadOnly, si.SIStatus);
                    GridView2.DataBind();
                    GridView1.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "更新成功!单号：" + si.SICode);
                }
                catch (Exception ex)
                {
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    DAL.CommonConvert.ShowMessageBox(this.Page, "保存数据出现异常！" + msg);
                }
            }
        }

        //取消
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewState["StockIn"] = ViewState["OrignalStockIn"];
            FormView1.DataBind();
            GridView1.DataBind();
            GridView2.DataBind();
            if (ViewState["StockIn"] != null)
            {
                StockIn si = (StockIn)ViewState["StockIn"];
                if (si.SICode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
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
                        edm.StockIn.ApplyCurrentValues(tmp);
                        edm.SaveChanges();
                    }
                }
            }

            ChangeFormViewMode(FormViewMode.ReadOnly, 0);
        }

        //重置
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ViewState["StockIn"] = ViewState["OrignalStockIn"];
            FormView1.DataBind();
            GridView1.DataBind();
            GridView2.DataBind();
            SwitchToInsertMode();
            lblMessage.Text = null;
        }

        //编辑
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //保存编辑人信息
            StockIn si;
            if (ViewState["StockIn"] != null)
            {
                si = (StockIn)ViewState["StockIn"];
                if (si.SICode != null)
                {
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
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
                                edm.StockIn.ApplyCurrentValues(tmp);
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

            if (IsValid)
            {
                string shortMsg = "";                
                try
                {
                    //先清空提示
                    lblMessage.Text = "";

                    StockIn si;
                    if (ViewState["StockIn"] != null)
                    {
                        si = (StockIn)ViewState["StockIn"];
                        if (si.SICode == null)
                        {
                            si.SICode = KeyGenerator.Instance.GetStockInKey();
                        }
                    }
                    else
                    {
                        si = new StockIn();
                        si.SICode = KeyGenerator.Instance.GetStockInKey();
                        si.SIType = "03";
                    }

                    si.SIStatus = 2;//已提交  
                    if (Session["UserInfo"] != null)
                    {
                        Users userInfo = (Users)Session["UserInfo"];
                        si.RFIDSubmitPersonID = userInfo.UserId;
                        si.RFIDSubmitPersonName = userInfo.UserName;
                        //增加修改编辑字段的数据
                        si.EditorID = userInfo.UserId;
                        si.EditorName = userInfo.UserName;
                    }
                    si.RFIDSubmitTime = System.DateTime.Now;
                    si.EditTime = System.DateTime.Now;
                    si.EditStatus = 0;//无人编辑
                    
                    UpdateStockIn(ref si);

                    if (si == null)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "没有有效商品！");
                        return;
                    }
                    if (si.StockDetail.Count == 0)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择要入库的商品！");
                        return;
                    }
                    if (si.FromType.ToString() == string.Empty)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择订单类型！");
                        return;
                    }
                    if (si.SIType.ToString() == string.Empty)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择单据类型！");
                        return;
                    }
                    if (si.WHCode == null)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库！");
                        return;
                    }
                    else if (si.WHCode.ToString() == string.Empty)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "请选择仓库！");
                        return;
                    }
                    if(si.InCategory == null)
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
                    if (si.WHCode != warehouseCode)
                    {
                        btnSubmit.Enabled = true;
                        DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                        return;
                    }

                    List<string> binTagIDs = new List<string>();
                    using (var edm = new Gold.DAL.GoldEntities())
                    { 
                        var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
                        if (tmp == null)
                        {
                            foreach (StockDetail sd in si.StockDetail)
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
                                if (sd.InOutWHCode != warehouseCode)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                    return;
                                }

                                if (sd.NumCurrentPlan == 0)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！");
                                    return;
                                }
                                else if (sd.NumCurrentPlan > sd.NumOriginalPlan && si.FromOrderNo != null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "应收数量不能大于订单数量！");
                                    return;
                                }

                                sd.RFIDSubmitTime = System.DateTime.Now;

                                //获取标签ID
                                string bin=sd.BinCode.Substring(0,2);
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
                            edm.StockIn.AddObject(si);
                        }
                        else
                        {
                            edm.StockIn.ApplyCurrentValues(si);

                            foreach (StockDetail sd in si.StockDetail)
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
                                if (sd.InOutWHCode != warehouseCode)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "仓库必须是地王26库！请重新选择仓库");
                                    return;
                                }

                                if (sd.NumCurrentPlan == 0)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "请输入应收数量！");
                                    return;
                                }
                                else if (sd.NumCurrentPlan > sd.NumOriginalPlan && si.FromOrderNo != null)
                                {
                                    btnSubmit.Enabled = true;
                                    DAL.CommonConvert.ShowMessageBox(this.Page, "应收数量不能大于订单数量！");
                                    return;
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

                                //如果入库单号和rowNumber主键一致，则更新此行项目
                                var tmp1 = edm.StockDetail.SingleOrDefault(o => (o.BillCode == sd.BillCode && o.BillRowNumber == sd.BillRowNumber));
                                if (tmp1 != null)
                                {
                                    edm.StockDetail.ApplyCurrentValues(sd);
                                }
                            }
                        }

                        //如果是来源于订单，则需要给相应的订单填充RFID处理人信息及时间，订单状态和订单行项目状态
                        //当前的应收数量=Sum（所有detail应收数量）-撤销单的应收数量-手持机尚未开始操作被撤销的入库单应收数量
                        if (si.FromOrderNo != null && si.FromOrderType != null)
                        {
                            var edmOrder = new Gold.DAL.GoldEntities();
                            var tmpStockIns = edmOrder.StockIn.Where(o => (o.FromOrderNo == si.FromOrderNo && o.SICode != si.SICode)).ToList();
                            if (si.FromOrderType == "01")//销售订单
                            {
                                var tmpSalesOrder = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpSalesOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (SalesOrderDetail salesOrderDetail in tmpSalesOrder.SalesOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                            else if (si.FromOrderType == "02")//采购订单
                            {
                                var tmpPurchaseOrder = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpPurchaseOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (PurchaseOrderDetail purchaseOrderDetail in tmpPurchaseOrder.PurchaseOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                            else if (si.FromOrderType == "03")//调拨订单
                            {
                                var tmpTransferOrder = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpTransferOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (TransferOrderDetail transferOrderDetail in tmpTransferOrder.TransferOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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
                            else if (si.FromOrderType == "04")//转库单
                            {
                                var tmpShiftOrder = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == si.FromOrderNo);
                                if (tmpShiftOrder != null)
                                {
                                    //查看保存的入库单是否把订单全部转或者是部分转
                                    bool isAllSave = true;

                                    foreach (ShiftOrderDetail shiftOrderDetail in tmpShiftOrder.ShiftOrderDetail)
                                    {
                                        foreach (StockDetail detail in si.StockDetail)//当前入库单
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

                        //提交前，验证该单是否已被提交
                        var edmStockin = new Gold.DAL.GoldEntities();
                        var tmpStockin = edmStockin.StockIn.SingleOrDefault(o => o.SICode == si.SICode);
                        if (tmpStockin != null)
                        {
                            if (tmpStockin.SIStatus > 1)
                            {
                                ViewState["OrignalStockIn"] = tmpStockin;
                                btnSubmit.Enabled = true;
                                ChangeFormViewMode(FormViewMode.ReadOnly, tmpStockin.SIStatus);
                                GridView2.DataBind();
                                GridView1.DataBind();
                                DAL.CommonConvert.ShowMessageBox(this.Page, "此入库单已被" + tmpStockin.RFIDSubmitPersonName + "提交!");
                                return;
                            }
                        }

                        //调用报警函数
                        StartOrStopAlarm(binTagIDs.ToArray(), true, out shortMsg);
                        edm.SaveChanges();
                    }
                                       
                    ViewState["OrignalStockIn"] = si;
                    btnSubmit.Enabled = true;
                    ChangeFormViewMode(FormViewMode.ReadOnly, si.SIStatus);
                    GridView2.DataBind();
                    GridView1.DataBind();
                    DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货成功！单号：" + si.SICode); 
                }
                catch (Exception ex)
                {
                    btnSubmit.Enabled = true;
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    msg = msg.Replace("\r\n", "");
                    DAL.CommonConvert.ShowMessageBox(this.Page, "提交至备货出现异常！" + msg);
                    //lblMessage.Text = "提交至备货出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
                }                
            }
        }
             
        //返回列表
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("StockInMgr.aspx");
        }

        private void ChangeFormViewMode(FormViewMode mode, int status)
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
                    lbtnAddRow.Visible = false;
                    lbtnDeleteRow.Visible = false;
                    GridView1.Visible = false;
                    GridView2.Visible = true;
                    btnReturn.Visible = true;
                    if (ViewState["sourceType"] != null)
                    {
                        string type = ViewState["sourceType"].ToString();
                        if (type == "purchaseorder")
                        {
                            btnReturnPurchase.Visible = true;
                            btnReturnShift.Visible = false;
                            btnReturnTransfer.Visible = false;
                        }
                        else if (type == "shiftorder")
                        {
                            btnReturnPurchase.Visible = false;
                            btnReturnShift.Visible = true;
                            btnReturnTransfer.Visible = false;
                        }
                        else if (type == "transferorder")
                        {
                            btnReturnPurchase.Visible = false;
                            btnReturnShift.Visible = false;
                            btnReturnTransfer.Visible = true;
                        }
                    }
                    break;
                case FormViewMode.Edit:
                    lblTitle.Text = "编辑入库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = true;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    btnReset.Visible = false;
                    lbtnAddRow.Visible = true;
                    lbtnDeleteRow.Visible = true;
                    btnReturn.Visible = false;
                    GridView1.Visible = true;
                    GridView2.Visible = false;
                    btnReturnPurchase.Visible = false;
                    btnReturnShift.Visible = false;
                    btnReturnTransfer.Visible = false;
                    break;
                case FormViewMode.Insert:
                    lblTitle.Text = "新建入库单";
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = true;
                    btnUpdate.Visible = false;
                    btnReset.Visible = true;
                    lbtnAddRow.Visible = true;
                    lbtnDeleteRow.Visible = true;
                    GridView1.Visible = true;
                    GridView2.Visible = false;
                    btnReturn.Visible = false;
                    btnReturnPurchase.Visible = false;
                    btnReturnShift.Visible = false;
                    btnReturnTransfer.Visible = false;
                    var ctlCode = FormView1.FindControl("ctlSICode") as TextBox;
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

        private void SwitchToInsertMode()
        {
            ChangeFormViewMode(FormViewMode.Insert, 0);
            ((TextBox)FormView1.FindControl("ctlSICode")).Text = "(自动生成)";
            if (ViewState["StockIn"] != null)
            {
                StockIn si = (StockIn)ViewState["StockIn"];
                if (si.FromType != null)//来源订单类型
                {
                    DropDownList ddl = (DropDownList)FormView1.FindControl("ctlFromType");
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value == si.FromType)
                        {
                            ddl.SelectedIndex = i;
                        }
                    }
                }

                //单据状态                
                DropDownList ddlSIStatus = (DropDownList)FormView1.FindControl("ctlSIStatus");
                for (int i = 0; i < ddlSIStatus.Items.Count; i++)
                {
                    if (ddlSIStatus.Items[i].Value == si.SIType)
                    {
                        ddlSIStatus.SelectedIndex = i;
                    }
                }

                if (si.SIType != null)//单据类型
                {
                    DropDownList ddl = (DropDownList)FormView1.FindControl("ctlSIType");
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value == si.SIType)
                        {
                            ddl.SelectedIndex = i;
                        }
                    }
                }
                if (si.SIDate != null)
                {
                    ((TextBox)FormView1.FindControl("ctlSIDate")).Text = ((DateTime)(si.SIDate)).ToString("yyyy-MM-dd");
                }
                else
                {
                    ((TextBox)FormView1.FindControl("ctlSIDate")).Text = System.DateTime.Now.ToShortDateString();
                }

                //仓库
                if (si.WHCode != null)
                {
                    DropDownList ddlWH = (DropDownList)FormView1.FindControl("ctlWHName");
                    for (int i = 0; i < ddlWH.Items.Count; i++)
                    {
                        if (ddlWH.Items[i].Value == si.WHCode)
                        {
                            ddlWH.SelectedIndex = i;
                        }
                    }
                }

                if (si.FromUCOrderNo != null)
                {
                    ((TextBox)FormView1.FindControl("ctlFromUCOrderNo")).Text = si.FromUCOrderNo;
                    if (si.FromType != "03" && si.FromType != "" && si.FromUCOrderNo.ToString() != string.Empty)
                    {
                        ((TextBox)FormView1.FindControl("ctlFromUCOrderNo")).Enabled = false;
                        lbtnAddRow.Visible = false;
                        lbtnDeleteRow.Visible = false;

                        DropDownList ddlSIType = (DropDownList)FormView1.FindControl("ctlSIType");
                        DropDownList ddlFromType = (DropDownList)FormView1.FindControl("ctlFromType");
                        ddlSIType.Enabled = false;
                        ddlFromType.Enabled = false; 
                       
                        TextBox ctlFromBillNo = ((TextBox)FormView1.FindControl("ctlFromBillNo"));
                        if (ctlFromBillNo != null)
                        {
                            ctlFromBillNo.Enabled=false;
                        }
                    }
                }

                if (si.FromBillNo != null)
                {
                    TextBox ctlFromBillNo = ((TextBox)FormView1.FindControl("ctlFromBillNo"));
                    if (ctlFromBillNo != null)
                    {
                        ctlFromBillNo.Text = si.FromBillNo;
                        if (si.FromType == "02")
                        {
                            ctlFromBillNo.Enabled = false;
                        }
                    }
                    
                }

                //供货单位
                if (si.Supplier != null)
                {
                    DropDownList ddlSupplier = (DropDownList)FormView1.FindControl("ctlSupplier");
                    for (int i = 0; i < ddlSupplier.Items.Count; i++)
                    {
                        if (ddlSupplier.Items[i].Value == si.Supplier)
                        {
                            ddlSupplier.SelectedIndex = i;
                        }
                    }
                }

                //业务员
                if (si.Operator != null)
                {
                    DropDownList ddlOperator = (DropDownList)FormView1.FindControl("ctlOperator");
                    for (int i = 0; i < ddlOperator.Items.Count; i++)
                    {
                        if (ddlOperator.Items[i].Value == si.Operator)
                        {
                            ddlOperator.SelectedIndex = i;
                        }
                    }
                }

                //库管员
                if (si.StoreKeeper != null)
                {
                    DropDownList ddlStoreKeeper = (DropDownList)FormView1.FindControl("ctlStoreKeeper");
                    for (int i = 0; i < ddlStoreKeeper.Items.Count; i++)
                    {
                        if (ddlStoreKeeper.Items[i].Value == si.StoreKeeper)
                        {
                            ddlStoreKeeper.SelectedIndex = i;
                        }
                    }
                }

                //收发类别
                if (si.InCategory != null)
                {
                    DropDownList ddlInCategory = (DropDownList)FormView1.FindControl("ctlInCategory");
                    ddlInCategory.SelectedIndex = ddlInCategory.Items.IndexOf(ddlInCategory.Items.FindByValue(si.InCategory));
                }

                //备注
                ((TextBox)FormView1.FindControl("ctlComment")).Text = si.Comment;
            }
            else
            {
                //暂时写固定值
                ((DropDownList)FormView1.FindControl("ctlSIStatus")).SelectedIndex = 1;//初始态
            }

        }

        //返回采购订单列表
        protected void btnReturnPurchase_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Order/PurchaseOrderMgr.aspx");
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
                if (ViewState["StockIn"] != null)
                {
                    StockIn si = (StockIn)ViewState["StockIn"];
                    string siCode = si.SICode;
                    using (var edm = new Gold.DAL.GoldEntities())
                    {
                        var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == siCode);
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
                            edm.StockIn.ApplyCurrentValues(tmp);
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

        public StockIn SelectStockIn()
        {
            return ViewState["StockIn"] == null ? null : ((StockIn)ViewState["StockIn"]);
        }

        protected void odsStockIn_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        protected void odsStockIn_ObjectDisposing(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        public IList<StockDetail> SelectStockDetail()
        {
            var tmp = ViewState["StockIn"] == null ? null : (StockIn)ViewState["StockIn"];
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
                    var si = edm.StockIn.SingleOrDefault(o => o.SICode == tmp.SICode);
                    if (si == null)
                    {
                        return null;
                    }
                    else
                    {
                        var ret = si.StockDetail.ToList();
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
                        else
                        {
                            //ret = tmp.StockDetail.OrderBy(o => o.CargoName).ToList();
                        }
                        
                        ViewState["StockIn"] = si;
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
            StockIn si;
            if (ViewState["StockIn"] != null)
                si = (StockIn)ViewState["StockIn"];
            else
                si = new StockIn();

            UpdateStockIn(ref si);
            si.StockDetail.Add(new StockDetail());
            ViewState["StockIn"] = si;
            GridView1.DataBind();
        }

        protected void lbtnDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["StockIn"] != null)
                {
                    StockIn si = (StockIn)ViewState["StockIn"];

                    //从界面获取值，更新一下所保存的si值
                    if (si != null)
                    {
                        UpdateStockIn(ref si);
                    }

                    if (si.StockDetail != null && si.StockDetail.Count > 0)
                    {
                        int count = GridView1.Rows.Count;                       
                        for (int i = 0; i < count; i++)
                        {
                            int j = 0;
                            if (((CheckBox)GridView1.Rows[i].FindControl("CheckBox1")).Checked == true)
                            {
                                string rowNO = GridView1.Rows[i].Cells[1].Text;//序号
                                string cargoCode = (GridView1.Rows[i].Cells[2].Controls[1] as TextBox).Text;//商品编号

                                foreach (StockDetail detail in si.StockDetail)
                                {
                                    if (rowNO != null && rowNO != "&nbsp;" && (si.FromType == "01" || si.FromType == "02"))
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

                    ViewState["StockIn"] = si;
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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GoodsSelect")
            {
                GridViewRow gvr = (e.CommandSource as Button).Parent.Parent as GridViewRow;
                GridView1.SelectedIndex = gvr.RowIndex;
                string cargoCode = ((TextBox)(GridView1.Rows[gvr.RowIndex].Cells[2].Controls[1])).Text.Trim();
                string cargoName = ((TextBox)(GridView1.Rows[gvr.RowIndex].Cells[3].Controls[1])).Text.Trim();
                string[] cargoCondition = new string[] { cargoCode, cargoName };
                GoodsSelect1.CargoQueryCondition = cargoCondition;
                GoodsSelect1.DataBindForQuery();
                this.popWindow.Show();
            }
        }

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            this.popWindow.Hide();
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
            for (int i = 0; i < GoodsSelect1.ListSelectedCargo.Count; i++)
            {
                string[] goodSelect = GoodsSelect1.ListSelectedCargo[i];
                if (i == 0)
                {
                    GridViewRow gvr = GridView1.SelectedRow;
                    string cargoCode = goodSelect[0];
                    (gvr.Cells[2].Controls[1] as TextBox).Text = cargoCode;
                    (gvr.Cells[3].Controls[1] as TextBox).Text = goodSelect[1];
                    gvr.Cells[4].Text = goodSelect[2];
                    gvr.Cells[5].Text = goodSelect[3];
                    gvr.Cells[6].Text = goodSelect[4];
                    gvr.Cells[15].Text = goodSelect[5];//发型年份

                    DropDownList binCode = gvr.FindControl("ctlBinCode") as DropDownList;                   
                    using (var edm = new GoldEntities())
                    {
                        var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode || o.CargoCode == null).Select(o => o.BinCode).Distinct().ToList();
                        if (tmp != null && tmp.Count > 0)
                        {
                            binCode.Items.Clear();
                            binCode.Items.Add(new ListItem("--选择--", ""));

                            foreach (string tag in tmp)
                            {
                                binCode.Items.Add(new ListItem(tag, tag));
                            }

                            //查询该商品实际的层位，并默认选中
                            var tmpCargoBin = edm.CargoTag.Where(o => o.CargoCode == cargoCode).Select(o => o.BinCode).Distinct().ToList();
                            if (tmpCargoBin != null && tmpCargoBin.Count > 0)
                            {
                                string cargoBin = tmpCargoBin[0];
                                binCode.SelectedIndex = binCode.Items.IndexOf(binCode.Items.FindByValue(cargoBin));
                            }
                            else if (binCode.Items.Count == 2)//如果只有一个区位，则默认选中
                            {
                                binCode.SelectedIndex = 1;
                            } 
                        }                       
                    }

                }
                else
                {
                    //根据选择的商品个数，新增行项目
                    StockIn si;
                    if (ViewState["StockIn"] != null)
                        si = (StockIn)ViewState["StockIn"];
                    else
                        si = new StockIn();

                    UpdateStockIn(ref si);

                    StockDetail sd = new StockDetail();
                    sd.CargoCode = goodSelect[0];//商品编码
                    sd.CargoName = goodSelect[1];//商品编码
                    sd.CargoModel = goodSelect[2];//型 号
                    sd.CargoSpec = goodSelect[3];//规格
                    sd.CargoUnits = goodSelect[4];//单位
                    //sd.CargoStatus = Convert.ToInt32(goodSelect[5]);//状态
                    sd.ReleaseYear = goodSelect[5];//发行年份
                    sd.BillRowNumber = (si.StockDetail.Count + 1).ToString();
                    si.StockDetail.Add(sd);

                    ViewState["StockIn"] = si;
                    GridView1.DataBind();

                    int index=GridView1.Rows.Count-1;
                    //int index = 0;
                    DropDownList binCode = GridView1.Rows[index].FindControl("ctlBinCode") as DropDownList;
                    using (var edm = new GoldEntities())
                    {
                        var tmp = edm.CargoTag.Where(o => o.CargoCode == sd.CargoCode || o.CargoCode == null).Select(o => o.BinCode).Distinct().ToList();
                        if (tmp != null && tmp.Count > 0)
                        {
                            binCode.Items.Clear();
                            binCode.Items.Add(new ListItem("--选择--", ""));

                            foreach (string tag in tmp)
                            {
                                binCode.Items.Add(new ListItem(tag, tag));
                            }

                            //查询该商品实际的层位，并默认选中
                            var tmpCargoBin = edm.CargoTag.Where(o => o.CargoCode == sd.CargoCode).Select(o => o.BinCode).Distinct().ToList();
                            if (tmpCargoBin != null && tmpCargoBin.Count > 0)
                            {
                                string cargoBin = tmpCargoBin[0];
                                binCode.SelectedIndex = binCode.Items.IndexOf(binCode.Items.FindByValue(cargoBin));
                            }
                            else if (binCode.Items.Count == 2)//如果只有一个区位，则默认选中
                            {
                                binCode.SelectedIndex = 1;
                            } 
                        }
                    }
                }
            }

            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    DropDownList binCode = gvr.FindControl("ctlBinCode") as DropDownList;
            //    string cargoCode = (gvr.Cells[2].Controls[1] as TextBox).Text.Trim() == string.Empty ? null : (gvr.Cells[2].Controls[1] as TextBox).Text.Trim();
            //    using (var edm = new GoldEntities())
            //    {
            //        var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode || o.CargoCode == null).Select(o => o.BinCode).Distinct().ToList();
            //        if (tmp != null && tmp.Count > 0)
            //        {

            //            binCode.Items.Clear();
            //            binCode.Items.Add(new ListItem("--选择--",""));

            //            foreach (string tag in tmp)
            //            {
            //                binCode.Items.Add(new ListItem(tag, tag));
            //            }
            //        }

            //        //var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode || o.CargoCode == null).ToList();
            //        //if (tmp != null && tmp.Count > 0)
            //        //{
            //        //    foreach (CargoTag tag in tmp)
            //        //    {
            //        //        binCode.Items.Insert(0, new ListItem(tag.BinCode, tag.BinCode));
            //        //    }
            //        //}
            //    }
            //}
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList binCode = e.Row.FindControl("ctlBinCode") as DropDownList;
                if (binCode != null)
                {
                    //if(binCode.SelectedIndex > 0)
                    //return;

                    TextBox cargoCode = e.Row.FindControl("ctlCargoCode") as TextBox;
                    if (binCode.Items.Count <= 1)//无数据时开始绑定数据
                    {                        
                        if (!string.IsNullOrEmpty(cargoCode.Text))
                        {
                            using (var edm = new GoldEntities())
                            {
                                var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode.Text || o.CargoCode == null).Select(o => o.BinCode).Distinct().ToList();
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
                    else if (tbxBinCode_Selected != null && tbxBinCode_Selected.Text == "")
                    {
                        if (!string.IsNullOrEmpty(cargoCode.Text))
                        {
                            using (var edm = new GoldEntities())
                            {
                                //查询该商品实际的层位，并默认选中
                                var tmpCargoBin = edm.CargoTag.Where(o => o.CargoCode == cargoCode.Text).Select(o => o.BinCode).Distinct().ToList();
                                if (tmpCargoBin != null && tmpCargoBin.Count > 0)
                                {
                                    string cargoBin = tmpCargoBin[0];
                                    binCode.SelectedIndex = binCode.Items.IndexOf(binCode.Items.FindByValue(cargoBin));
                                }
                                else if (binCode.Items.Count == 2)
                                {
                                    //如果只有一个区位，则默认选中
                                    binCode.SelectedIndex = 1;
                                }
                            }
                        }
                    }
                }

                //设置行项目仓库的默认值--地王26库
                DropDownList whCode = e.Row.FindControl("ctlInOutWHCode") as DropDownList;
                if (whCode != null)
                {
                    if (whCode.Items.Count > 1)
                    {
                        if (whCode.SelectedIndex == 0)
                        {
                            string defaultWHCode = string.Empty;
                            //从配置文件读取默认的仓库--地王26库
                            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                            {
                                defaultWHCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                            }
                            whCode.SelectedValue = defaultWHCode;
                        }
                    }
                }

                //Label lblBillRowNumber= e.Row.FindControl("lblBillRowNumber") as Label;
                //if (binCode.Items.Count > 1&&) 
                //{
                //}

                //TextBox cargoCode = e.Row.FindControl("ctlCargoCode") as TextBox;
                //if (!string.IsNullOrEmpty(cargoCode.Text))
                //{
                //    using (var edm = new GoldEntities())
                //    {
                //        var tmp = edm.CargoTag.Where(o => o.CargoCode == cargoCode.Text.Trim()).ToList();
                //        if (tmp != null && tmp.Count > 0)
                //        {
                //            foreach (CargoTag tag in tmp)
                //            {
                //                binCode.Items.Insert(1, new ListItem(tag.BinCode, tag.BinCode));
                //            }
                //            binCode.SelectedIndex = 1;
                //        }
                //    }

                //}

            }
        }

        #endregion

        #region DropDownList下拉列表绑定
        //来源订单类型
        //protected void ctlFromType_DataBound(object sender, EventArgs e)
        //{
        //    ListItem item = new ListItem("不限", "");
        //    DropDownList ddlFromType = (DropDownList)FormView1.FindControl("ctlFromType");
        //    ddlFromType.Items.Insert(0, item);
        //}

        //protected void ctlSIType_DataBound(object sender, EventArgs e)
        //{
        //    DropDownList ddlSIType = (DropDownList)FormView1.FindControl("ctlSIType");
        //    //ddlSIType.SelectedIndex = ddlSIType.Items.IndexOf(ddlSIType.Items.FindByValue("qqcrazyer"));
        //    //ListItem item = new ListItem("不限", "");
        //    //DropDownList ddlSIType = (DropDownList)FormView1.FindControl("ctlSIType");
        //    //ddlSIType.Items.Insert(0, item);
        //}

        ////单据状态
        //protected void ctlSIStatus_DataBound(object sender, EventArgs e)
        //{
        //    ListItem item = new ListItem("不限", "");
        //    DropDownList ddlSIStatus = (DropDownList)FormView1.FindControl("ctlSIStatus");
        //    ddlSIStatus.Items.Insert(0, item);
        //}


        #endregion

        //选择商品按钮是否显示的判断
        protected bool GetButtonVisible()
        {
            StockIn si = (StockIn)ViewState["StockIn"];
            if (si == null) return true;
            //如果是来源于订单，且来源订单号不为空，则不能改变行项目的商品信息，只能删除或者增加
            if ((si.FromOrderNo != "" && si.FromOrderNo != null && si.FromType != "" && si.FromType != "03")//03:源于手工新增的出库单
               || (si.FromType == "02"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }        

        //设置默认值
        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            try
            {
                //设置来源单号输入框为只读
                if (FormView1.CurrentMode == FormViewMode.Edit || FormView1.CurrentMode == FormViewMode.Insert)
                {
                    DropDownList drpFromType = FormView1.FindControl("ctlFromType") as DropDownList;//订单类型
                    TextBox txtFromUCOrderNo = FormView1.FindControl("ctlFromUCOrderNo") as TextBox;//用友参考单号
                    DropDownList drpSIType = FormView1.FindControl("ctlSIType") as DropDownList;//单据类型 
                    TextBox txtFromBillNo = FormView1.FindControl("ctlFromBillNo") as TextBox;//源Excel入库单号
                    //如果不是手工新增的入库单，则不允许用户修改单据类型和来源订单类型
                    if (((drpFromType.SelectedValue == "01") && drpFromType.SelectedIndex != 0 && txtFromUCOrderNo.Text.Trim() != string.Empty)
                        || (drpFromType.SelectedValue == "02"))
                    {
                        txtFromUCOrderNo.Enabled = false;
                        drpFromType.Enabled = false;
                        drpSIType.Enabled = false;
                        txtFromBillNo.Enabled = false;

                        //设置添加商品、删除所选商品按钮的隐藏
                        lbtnAddRow.Visible = false;
                        lbtnDeleteRow.Visible = false;
                    }
                    
                }

                //设置出库日期的默认值--今天
                if (FormView1.CurrentMode != FormViewMode.ReadOnly)
                {
                    TextBox txtSIDate= FormView1.FindControl("ctlSIDate") as TextBox;
                    if (txtSIDate.Text.Trim() == string.Empty)
                    {
                        txtSIDate.Text = System.DateTime.Now.ToShortDateString();
                    }
                }

                //设置库管员默认值--古燕
                DropDownList drpStoreKeeper = FormView1.FindControl("ctlStoreKeeper") as DropDownList;
                TextBox txtStoreKeeper = FormView1.FindControl("txtStoreKeeper") as TextBox;
                if (drpStoreKeeper != null)
                {
                    if (txtStoreKeeper != null && txtStoreKeeper.Text.Trim() != string.Empty && drpStoreKeeper.Items.Count > 1)
                    {
                        drpStoreKeeper.SelectedIndex = drpStoreKeeper.Items.IndexOf(drpStoreKeeper.Items.FindByValue(txtStoreKeeper.Text));
                    }

                    if (drpStoreKeeper.SelectedIndex == 0)
                    {
                        drpStoreKeeper.SelectedValue = "古燕";
                    }
                }

                //已绑定数据时设置选中项-业务员
                DropDownList ddlOperator = FormView1.FindControl("ctlOperator") as DropDownList;
                TextBox txOperator = FormView1.FindControl("txOperator") as TextBox;
                if (ddlOperator != null)
                {
                    if (txOperator != null && txOperator.Text != "" && ddlOperator.Items.Count > 1)
                    {
                        ddlOperator.SelectedIndex = ddlOperator.Items.IndexOf(ddlOperator.Items.FindByText(txOperator.Text));
                    }
                }               

                //已绑定数据时设置选中项-收发类别
                DropDownList ddlInCategory = FormView1.FindControl("ctlInCategory") as DropDownList;
                TextBox txtInCategory = FormView1.FindControl("txtInCategory") as TextBox;
                if (ddlInCategory != null)
                {
                    if (txtInCategory != null && txtInCategory.Text != "" && ddlInCategory.Items.Count > 1)
                    {
                        ddlInCategory.SelectedIndex = ddlInCategory.Items.IndexOf(ddlInCategory.Items.FindByValue(txtInCategory.Text));
                    }
                }

                //抬头的仓库
                DropDownList ddlctlWHName = FormView1.FindControl("ctlWHName") as DropDownList;
                TextBox txtWHName = FormView1.FindControl("txtWHName") as TextBox;
                if (ddlctlWHName != null)
                {
                    if (txtWHName != null && txtWHName.Text.Trim() != string.Empty && ddlctlWHName.Items.Count > 1)
                    {
                        ddlctlWHName.SelectedIndex = ddlctlWHName.Items.IndexOf(ddlctlWHName.Items.FindByValue(txtWHName.Text));
                    }

                    if (ddlctlWHName.SelectedIndex == 0)
                    {
                        string defaultWHCode = string.Empty;
                        //从配置文件读取默认的仓库--地王26库
                        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DefaultWHCode"))
                        {
                            defaultWHCode = System.Configuration.ConfigurationManager.AppSettings["DefaultWHCode"].ToString();
                        }
                        ddlctlWHName.SelectedValue = defaultWHCode;
                    }
                }

                //供货单位
                DropDownList ddlSupplier = FormView1.FindControl("ctlSupplier") as DropDownList;
                TextBox txtSupplier = FormView1.FindControl("txtSupplier") as TextBox;
                if (ddlSupplier != null)
                {
                    if (txtSupplier != null && txtSupplier.Text.Trim() != string.Empty && ddlSupplier.Items.Count > 1)
                    {
                        ddlSupplier.SelectedIndex = ddlSupplier.Items.IndexOf(ddlSupplier.Items.FindByValue(txtSupplier.Text));
                    }
                }

                //单据类型
                DropDownList ddlSIType = FormView1.FindControl("ctlSIType") as DropDownList;
                TextBox txtSIType = FormView1.FindControl("txtSIType") as TextBox;
                if (ddlSIType != null)
                {
                    if (txtSIType != null && txtSIType.Text.Trim() != string.Empty && ddlSIType.Items.Count > 1)
                    {
                        ddlSIType.SelectedIndex = ddlSIType.Items.IndexOf(ddlSIType.Items.FindByValue(txtSIType.Text));
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

        //设置行项目仓库的默认值--地王26库
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

        //获取当前订单当前商品的应收数量
        public double GetCargosNumCurrentPlan(string cargoCode, string fromOrderNo, double? numOriginalPlan, string detailRowNo)
        {
            double numCurrentPlan = 0;
            double numOriginal = numOriginalPlan == null ? 0 : Convert.ToDouble(numOriginalPlan);
            using (var edm = new Gold.DAL.GoldEntities())
            {
                //根据订单编号和商品号，查询当前的应收数量
                var tmp = edm.VSelectAllInCancelDetail.Where(o => (o.FromOrderNo == fromOrderNo && o.CargoCode == cargoCode && o.BillRowNumber == detailRowNo)).ToList();
                if (tmp.Count != 0)
                {
                    //已使用的应收数量
                    double numCurrent = tmp[0].sumnumCurrentPlan == null ? 0 : Convert.ToDouble(tmp[0].sumnumCurrentPlan);
                    //应收数量=订单数量-已使用的应收数量
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