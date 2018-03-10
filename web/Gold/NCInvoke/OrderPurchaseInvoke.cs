using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Gold.NCWebServiceForRFID;
using Gold.Utility;
using System.Text;
using Gold.DAL;

namespace Gold.NCInvoke
{
    public class OrderPurchaseInvoke : OrderInfoInvoke
    {
        //抽象类
        public OrderPurchaseInvoke()
        {
        }

        #region 采购订单表头
        /// <summary>
        /// 调用用友WebService并将返回的Xml字符串解析为DataTable
        /// </summary>
        /// <param name="dt">xml字符串转换成的DataTable</param>
        /// <param name="msg">信息提示</param>
        /// <returns>true成功，false失败</returns>
        public override bool GetNCData(out DataTable dt, out string msg)
        {
            dt = null;
            msg = "";
            try
            {
                //调用WebService的基础资料查询方法
                NCWebServiceForRFID.IBillXMLExportService NCWSClient = new IBillXMLExportService();
                string xmlOld = NCWSClient.BillInfoQuery(base.Warehouse, base.BillType, base.StartTime, base.EndTime, base.MaxCount,true);

                //按系统设置的编码格式进行xml数据解码
                string xml = XmlHelper.GetStringByDefaultEncodingType(xmlOld);
                        

                //记录日志
                StringBuilder strLog = new StringBuilder("调用BillInfoQuery 参数：");
                strLog.Append(base.BillType);
                strLog.Append(",");
                strLog.Append(base.Warehouse);
                strLog.Append(base.StartTime);
                strLog.Append(base.EndTime);
                strLog.Append(" xml数据：");
                strLog.Append(xml);

                LogHelper.WriteLog(LogHelper.LogLevel.Info, strLog.ToString());

                //获取用友返回的xml字符串中的Total和Error字段信息
                string NCTotal = "", NCError = "";
                if (XmlHelper.GetXmlStringTotalAndErrorMsg(xml, out NCTotal, out NCError))
                {
                    if (NCTotal.Trim() == "0")
                    {
                        msg = "用友系统未能提供有效数据！用友系统返回信息：" + NCError;
                        return false;
                    }
                }

                ////将XML字符串中的数据转换成DataTable
                DataSet ds = ConvertDataTableXML.ConvertXmlStrToDataSet(xml);
                if (ds != null)
                {
                    foreach (DataTable dtTemp in ds.Tables)
                    {
                        if (dtTemp.TableName == "Data")
                        {
                            dt = dtTemp;
                            break;
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                StringBuilder strLog = new StringBuilder("调用BillInfoQuery失败！ 参数：");
                strLog.Append(base.BillType);
                strLog.Append(",");
                strLog.Append(base.Warehouse);
                strLog.Append(base.StartTime);
                strLog.Append(base.EndTime);
                strLog.Append(" 异常信息:");
                strLog.Append(LogHelper.GetExceptionMsg(ex));

                msg = strLog.ToString();

                LogHelper.WriteLog(LogHelper.LogLevel.Error, strLog.ToString(), ex);
                return false;
            }

            //return base.GetNCData(out dt, out msg);
        }        

        /// <summary>
        /// 调用用友WebService并将返回的Xml字符串解析为DataTable,并为解析后的DataTable增加两列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool GetNCDataJoinRFID(out List<PurchaseOrder> lstPurchaseOrder, out string msg)
        {
            DataTable dt = null;
            lstPurchaseOrder = null;
            DataTable dtForNo = null;//存放未导入的数据
            DataTable dtForYes= null;//存放已导入的数据
            msg = "";
            try
            {
                if (GetNCData(out dt, out msg) == false)
                    return false;

                if (dt != null)
                {
                    if (base.isAlreadyStatus != string.Empty)
                    {
                        if (dt.Columns.Contains("NCOrderCode"))
                        {
                            using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                            {
                                dtForNo = dt.Clone();
                                dtForYes = dt.Clone(); 

                                List<PurchaseOrder> list = (from r in context.PurchaseOrder select r).ToList<PurchaseOrder>();
                                foreach (DataRow dr in dt.Rows)
                                {  
                                    string nCOrderCode = dr["NCOrderCode"].ToString();
                                    bool isExist = false;
                                    foreach (PurchaseOrder order in list)
                                    {
                                        //如果不是初始态，则说明已经导入且已开始其他操作
                                        //if (order.NCOrderCode == nCOrderCode && order.OrderStatus != 2)
                                        if(order.NCOrderCode == nCOrderCode)
                                        {
                                            //0：未导入， 1：已导入                                                                                         
                                            dtForYes.Rows.Add(dr.ItemArray);
                                            isExist = true;
                                            break;                                            
                                        }                                        
                                    }
                                    if (!isExist)
                                    {
                                        dtForNo.Rows.Add(dr.ItemArray);
                                    }
                                }
                            }
                        }
                    }
                }

                if (base.isAlreadyStatus == "0")
                {
                    if (dtForNo==null|| dtForNo.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstPurchaseOrder = GetModelFromDataTable(dtForNo, out msg);
                }
                else if (base.isAlreadyStatus == "1")
                {
                    if (dtForYes==null||dtForYes.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstPurchaseOrder = GetModelFromDataTable(dtForYes, out msg);
                }
                else
                {
                    lstPurchaseOrder = GetModelFromDataTable(dt, out msg);
                }

                if (lstPurchaseOrder==null|| lstPurchaseOrder.Count == 0)
                {
                    msg = "获取用友系统数据失败！详细信息:" + msg;                    
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogLevel.Error, "调用GetNCDataJoinRFID失败", ex);
                msg = "调用GetNCDataJoinRFID失败" + ex.Message;
                return false;
            }
            //return base.GetNCDataJoinRFID(out dt, out msg);
        }

        /// <summary>
        /// 将从用友获取的DataTable数据保存到RFID系统
        /// </summary>
        /// <param name="dt">从用友获取的DataTable数据</param>
        /// <param name="msg">信息提示</param>
        /// <returns>true成功，false失败</returns>
        public override bool SaveToRFID(List<PurchaseOrder> list, out string msg)
        {
            msg = "";
            try
            {
                //string convertMsg = "";
                //List<PurchaseOrder> list = GetModelFromDataTable(dt, out convertMsg);
                if (list.Count == 0)
                {
                    //msg = "获取用友系统数据失败!";
                    return false;
                }
                else
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        //查询系统中所有采购订单编号
                        List<PurchaseOrder> RFIDKeyList = (from r in context.PurchaseOrder select r).ToList<PurchaseOrder>();
                        
                        string existNCcode = string.Empty;
                        List<PurchaseOrder> delList = new List<PurchaseOrder>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (PurchaseOrder order in RFIDKeyList)
                            {
                                if (order.NCOrderCode == list[i].NCOrderCode && order.OrderStatus == 2)
                                {                                    
                                    delList.Add(order);
                                    break;
                                }
                                else if (order.NCOrderCode == list[i].NCOrderCode)
                                {
                                    existNCcode += list[i].NCOrderCode + ",";
                                    break;
                                }
                            }                            
                        }

                        if (existNCcode != string.Empty)
                        {
                            existNCcode = existNCcode.Substring(0, existNCcode.Length - 1);
                            msg = "保存失败!。用友系统中订单数据(" + existNCcode + ")存在于RFID系统中且已开始其他操作。";
                            return false;
                        }

                        //删除数据库中原有的数据
                        if (delList.Count != 0)
                        {
                            foreach (PurchaseOrder pur in delList)
                            {
                                context.PurchaseOrder.DeleteObject(pur);
                                context.SaveChanges();
                            }
                        }
                       
                        //其加入数据库
                        foreach (PurchaseOrder newModel in list)
                        {
                            //搜索对应的采购订单详细信息
                            List<PurchaseOrderDetail> lstPurchaseOrderDetail = new List<PurchaseOrderDetail>();
                            if (GetNCDataDetailJoinRFID(out lstPurchaseOrderDetail, out msg, newModel.NCOrderCode))
                            {
                                foreach(PurchaseOrderDetail detail in lstPurchaseOrderDetail)
                                {
                                    newModel.PurchaseOrderDetail.Add(detail);
                                }
                            }
                            else
                            {                                
                                return false;
                            }

                            context.PurchaseOrder.AddObject(newModel);
                        }

                        //提交变更
                        int AffectRowsCount = context.SaveChanges();
                        msg = "保存成功!影响行数：" + AffectRowsCount.ToString();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "PurchaseOrder SaveToRFID方法出错", ex);
                msg = "PurchaseOrder SaveToRFID方法出错" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.PurchaseOrder> GetModelFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.PurchaseOrder> list = new List<DAL.PurchaseOrder>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("NCOrderCode", "NC系统订单号");
                dataFieldNameDic.Add("OrderDate", "订单日期");
                dataFieldNameDic.Add("CompanyName", "公司名称");
                dataFieldNameDic.Add("Version", "版本号");
                dataFieldNameDic.Add("BusinessType", "业务类型");
                dataFieldNameDic.Add("Purchaser", "采购组织");
                dataFieldNameDic.Add("Buyer", "采购员");
                dataFieldNameDic.Add("Supplier", "供应商");
                dataFieldNameDic.Add("SupplierAddr", "供应商收货地址");
                dataFieldNameDic.Add("Receiver", "收货方");
                dataFieldNameDic.Add("CurrencyType", "币种");
                dataFieldNameDic.Add("ReceiptCompany", "发票方");
                dataFieldNameDic.Add("DepartmentName", "部门名称");
                dataFieldNameDic.Add("Preparer", "制单人");
                dataFieldNameDic.Add("PrepareTime", "制单时间");
                dataFieldNameDic.Add("Approver", "审批人");
                dataFieldNameDic.Add("ApproveTime", "审批时间");
                dataFieldNameDic.Add("LastEditTime", "最后修改时间");

                if (dt==null|| dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<PurchaseOrder>();
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
                    return new List<PurchaseOrder>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    PurchaseOrder newModel = new PurchaseOrder();

                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    
                    //订单日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["OrderDate"])))
                    {
                        DateTime datetime;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["OrderDate"]), out datetime))
                            newModel.OrderDate = datetime;
                    }
                    
                    newModel.Company = DataCheckHelper.GetCellString(dr["CompanyName"]);
                    newModel.Version = DataCheckHelper.GetCellString(dr["Version"]);
                    newModel.BusinessType = DataCheckHelper.GetCellString(dr["BusinessType"]);
                    newModel.Purchaser = DataCheckHelper.GetCellString(dr["Purchaser"]);
                    newModel.Buyer = DataCheckHelper.GetCellString(dr["Buyer"]);
                    newModel.Supplier = DataCheckHelper.GetCellString(dr["Supplier"]);
                    newModel.SupplierAddr = DataCheckHelper.GetCellString(dr["SupplierAddr"]);
                    newModel.Receiver = DataCheckHelper.GetCellString(dr["Receiver"]);
                    newModel.CurrencyType = DataCheckHelper.GetCellString(dr["CurrencyType"]);
                    newModel.ReceiptCompany = DataCheckHelper.GetCellString(dr["ReceiptCompany"]);
                    newModel.DepartmentName = DataCheckHelper.GetCellString(dr["DepartmentName"]);
                    newModel.Preparer = DataCheckHelper.GetCellString(dr["Preparer"]);

                    //制单时间
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["PrepareTime"])))
                    {
                        DateTime prepareTime;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["PrepareTime"]), out prepareTime))
                            newModel.PrepareTime = prepareTime;
                    }
                    
                    newModel.Approver = DataCheckHelper.GetCellString(dr["Approver"]);

                    //审批时间
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["ApproveTime"])))
                    {
                        DateTime approveTime;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["ApproveTime"]), out approveTime))
                            newModel.ApproveTime = approveTime;
                    }
                    
                    //最后修改时间
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["LastEditTime"])))
                    {
                        DateTime lastEditTime;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["LastEditTime"]), out lastEditTime))
                            newModel.LastEditTime = lastEditTime;
                    }                    

                    newModel.OrderStatus = 2;//初始态
                    newModel.EditStatus = 0;//无人编辑
                    newModel.OrderCode = newModel.NCOrderCode;

                    List<PurchaseOrder> existPurchaseOrder = (from r in list where r.NCOrderCode == newModel.NCOrderCode select r).ToList<PurchaseOrder>();
                    if (existPurchaseOrder == null || existPurchaseOrder.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<PurchaseOrder>(); ;
            }
        }

        #endregion

        #region 采购订单明细
        /// <summary>
        /// 采购订单明细
        /// </summary>
        /// <param name="dt">xml字符串转换成的DataTable</param>
        /// <param name="msg">信息提示</param>
        /// <returns>true成功，false失败</returns>
        public override bool GetNCDataDetail(out DataTable dt, out string msg, string billCode)
        {
            dt = null;
            msg = "";
            try
            {
                //调用WebService的基础资料查询方法
                NCWebServiceForRFID.IBillXMLExportService NCWSClient = new IBillXMLExportService();
                string xmlOld = NCWSClient.BillDetailQuery(base.Warehouse, base.BillType, billCode);
                //按系统设置的编码格式进行xml数据解码
                string xml = XmlHelper.GetStringByDefaultEncodingType(xmlOld);

                //记录日志
                StringBuilder strLog = new StringBuilder("调用BillDetailQuery 参数：");
                strLog.Append(base.BillType);
                strLog.Append(",");
                strLog.Append(base.Warehouse);
                strLog.Append(billCode);
                strLog.Append(" xml数据：");
                strLog.Append(xml);
                LogHelper.WriteLog(LogHelper.LogLevel.Info, strLog.ToString());

                //获取用友返回的xml字符串中的Total和Error字段信息
                string NCTotal = "", NCError = "";
                if (XmlHelper.GetXmlStringTotalAndErrorMsg(xml, out NCTotal, out NCError))
                {
                    if (NCTotal.Trim() == "0")
                    {
                        msg = "用友系统未能提供有效数据！用友系统返回信息：" + NCError;
                        return false;
                    }
                }

                //将XML字符串中的数据转换成DataTable
                DataSet ds = ConvertDataTableXML.ConvertXmlStrToDataSet(xml);
                if (ds != null)
                {
                    foreach (DataTable dtTemp in ds.Tables)
                    {
                        if (dtTemp.TableName == "Data")
                        {
                            dt = dtTemp;
                            break;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                StringBuilder strLog = new StringBuilder("调用BillDetailQuery失败！ 参数：");
                strLog.Append(base.BillType);
                strLog.Append(",");
                strLog.Append(base.Warehouse);
                strLog.Append(base.StartTime);
                strLog.Append(base.EndTime);
                strLog.Append(" 异常信息:");
                strLog.Append(LogHelper.GetExceptionMsg(ex));

                msg = strLog.ToString();

                LogHelper.WriteLog(LogHelper.LogLevel.Error, strLog.ToString(), ex);
                return false;
            }           
        }

        /// <summary>
        /// 调用用友WebService并将返回的Xml字符串解析为DataTable,并为解析后的DataTable增加两列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool GetNCDataDetailJoinRFID(out List<PurchaseOrderDetail> lstPurchaseOrderDetail, out string msg, string billCode)
        {
            DataTable dt = null;
            lstPurchaseOrderDetail = null;
            msg = "";
            try
            {

                if (GetNCDataDetail(out dt, out msg, billCode) == false)
                    return false;

                if (dt != null)
                {
                    dt.Columns.Add("IsExistInRFID", typeof(int));//代码表示是否RFID系统已存在此数据
                    dt.Columns.Add("IsExistInRFIDName", typeof(string));//名称表示是否RFID系统已存在此数据

                    if (dt.Columns.Contains("NCOrderCode"))
                    {
                        using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                        {
                            List<string> list = context.PurchaseOrderDetail.Where(r => r.NCOrderCode == billCode).Select(r => r.DetailRowNumber).ToList<string>();
                            foreach (DataRow dr in dt.Rows)
                            {
                                string detailRowNumber = dr["DetailRowNumber"].ToString();
                                if (list.Contains(detailRowNumber))
                                {
                                    dr["IsExistInRFID"] = "1";
                                    dr["IsExistInRFIDName"] = "已存在";
                                }
                                else
                                {
                                    dr["IsExistInRFID"] = "0";
                                    dr["IsExistInRFIDName"] = "未存在";
                                }
                            }
                        }
                    }
                }

                lstPurchaseOrderDetail = GetModelDetailFromDataTable(dt, out msg);
                if (lstPurchaseOrderDetail.Count == 0)
                {
                    //msg = "获取用友系统数据失败！详细信息:" + msg;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogLevel.Error, "调用GetNCDataJoinRFID失败", ex);
                msg = "调用GetNCDataJoinRFID失败" + ex.Message;
                return false;
            }
            //return base.GetNCDataJoinRFID(out dt, out msg);
        }

        /// <summary>
        /// 从DataTable中获取采购订单明细实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.PurchaseOrderDetail> GetModelDetailFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.PurchaseOrderDetail> list = new List<DAL.PurchaseOrderDetail>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("NCOrderCode", "NC系统订单号");
                dataFieldNameDic.Add("DetailRowNumber", "行号");
                dataFieldNameDic.Add("ContractNo", "合同号");
                dataFieldNameDic.Add("CargoCode", "商品编码");
                dataFieldNameDic.Add("CargoName", "商品名称");
                dataFieldNameDic.Add("CargoModel", "商品型号");
                dataFieldNameDic.Add("CargoSpec", "商品规格");
                dataFieldNameDic.Add("WHName", "收货仓库");
                dataFieldNameDic.Add("CargoUnits", "商品单位");
                dataFieldNameDic.Add("PlanNumber", "应收数量");
                dataFieldNameDic.Add("PriceOfTax", "含税单价");
                dataFieldNameDic.Add("Price", "单价");
                dataFieldNameDic.Add("DeductRate", "折扣");
                dataFieldNameDic.Add("PriceOfNetTax", "净含税单价");
                dataFieldNameDic.Add("NetPrice", "净单价");
                dataFieldNameDic.Add("TotalPrice", "金额");
                dataFieldNameDic.Add("TaxRate", "税率");
                dataFieldNameDic.Add("TotalTax", "税额");
                dataFieldNameDic.Add("TotalTaxAndPrice", "价税合计");
                dataFieldNameDic.Add("PlanArrivalDate", "计划到货日期");
                dataFieldNameDic.Add("CurrencyType", "币种");
                dataFieldNameDic.Add("ExchangeRate", "汇率");
                dataFieldNameDic.Add("ReceiveCompany", "收货公司");
                dataFieldNameDic.Add("ReceiveOrg", "收货库存组织");
                dataFieldNameDic.Add("ReceiveBillCompany", "收票公司");                

                if (dt==null|| dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<PurchaseOrderDetail>();
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
                    return new List<PurchaseOrderDetail>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    PurchaseOrderDetail newModel = new PurchaseOrderDetail();

                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    newModel.DetailRowNumber = DataCheckHelper.GetCellString(dr["DetailRowNumber"]);
                    newModel.ContractNo = DataCheckHelper.GetCellString(dr["ContractNo"]);

                    //商品编码
                    newModel.CargoCode = DataCheckHelper.GetCellString(dr["CargoCode"]);
                    string cargoCode = DbCommonMethod.ParsingCargoCode(newModel.CargoCode);
                    if (string.IsNullOrEmpty(cargoCode))
                        throw new ApplicationException("单号" + newModel.NCOrderCode + ",商品不存在：" + newModel.CargoCode);
                    
                    newModel.CargoName = DataCheckHelper.GetCellString(dr["CargoName"]);
                    newModel.CargoModel = DataCheckHelper.GetCellString(dr["CargoModel"]);
                    newModel.CargoSpec = DataCheckHelper.GetCellString(dr["CargoSpec"]);

                    //仓库
                    newModel.WHName = DataCheckHelper.GetCellString(dr["WHName"]);
                    if (newModel.WHName != "")
                    {
                        newModel.WHCode = DbCommonMethod.ParsingWarehouse(newModel.WHName);
                        if (string.IsNullOrEmpty(newModel.WHCode))
                            throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.WHName);
                    }

                    newModel.CargoUnits = DataCheckHelper.GetCellString(dr["CargoUnits"]);

                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["PlanNumber"])))
                    {
                        Double number;
                        if (Double.TryParse(DataCheckHelper.GetCellString(dr["PlanNumber"]), out number))
                            newModel.NumOriginalPlan = number;
                    }

                    //含税单价
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["PriceOfTax"])))
                    {
                        decimal priceOfTax;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["PriceOfTax"]), out priceOfTax))
                            newModel.PriceOfTax = priceOfTax;
                    }
                    
                    //单价
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["Price"])))
                    {
                        decimal price;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["Price"]), out price))
                            newModel.Price = price;
                    }

                    //折扣率
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["DeductRate"])))
                    {
                        double deductRate;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["DeductRate"]), out deductRate))
                            newModel.DeductRate = deductRate;
                    }
                   
                    //净含税单价
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["PriceOfNetTax"])))
                    {
                        decimal priceOfNetTax;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["PriceOfNetTax"]), out priceOfNetTax))
                            newModel.PriceOfNetTax = priceOfNetTax;
                    }

                    //净单价
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["NetPrice"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["NetPrice"]), out d))
                            newModel.NetPrice = d;
                    }
                    
                    //金额
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalPrice"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["TotalPrice"]), out d))
                            newModel.TotalPrice = d;
                    }
                    
                    //税率
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TaxRate"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["TaxRate"]), out d))
                            newModel.TaxRate = d;
                    }

                    //税额
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalTax"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["TotalTax"]), out d))
                            newModel.TotalTax = d;
                    }
                    
                    //价税合计
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalTaxAndPrice"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["TotalTaxAndPrice"]), out d))
                            newModel.TotalTaxAndPrice = d;
                    }
                    
                    //计划到货日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["PlanArrivalDate"])))
                    {
                        DateTime d;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["PlanArrivalDate"]), out d))
                            newModel.PlanArrivalDate = d;
                    }
                    
                    newModel.CurrencyType = DataCheckHelper.GetCellString(dr["CurrencyType"]);

                    //折本汇率
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["ExchangeRate"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["ExchangeRate"]), out d))
                            newModel.ExchangeRate = d;
                    }
                    
                    newModel.ReceiveCompany = DataCheckHelper.GetCellString(dr["ReceiveCompany"]);
                    newModel.ReceiveOrg = DataCheckHelper.GetCellString(dr["ReceiveOrg"]);
                    newModel.ReceiveBillCompany = DataCheckHelper.GetCellString(dr["ReceiveBillCompany"]);

                    newModel.DetailRowStatus = 2;//初始态
                    newModel.OrderCode = newModel.OrderCode;
                    newModel.NCOrderCode = newModel.NCOrderCode;                    

                    List<PurchaseOrderDetail> existPurchaseOrderDetail = (from r in list where r.NCOrderCode == newModel.NCOrderCode && r.DetailRowNumber== newModel.DetailRowNumber select r).ToList<PurchaseOrderDetail>();
                    if (existPurchaseOrderDetail == null || existPurchaseOrderDetail.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<PurchaseOrderDetail>(); ;
            }
        }
        #endregion
    }
}