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
    public class OrderSalesInvoke : OrderInfoInvoke
    {
        //抽象类
        public OrderSalesInvoke()
        {
        }

        #region 销售订单表头
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
                string xmlOld = NCWSClient.BillInfoQuery(base.Warehouse, base.BillType, base.StartTime, base.EndTime, base.MaxCount, true);

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
        public override bool GetNCDataJoinRFID(out List<SalesOrder> lstSalesOrder, out string msg)
        {
            DataTable dt = null;
            lstSalesOrder = null;
            DataTable dtForNo = null;//存放未导入的数据
            DataTable dtForYes = null;//存放已导入的数据
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

                                List<SalesOrder> list = (from r in context.SalesOrder select r).ToList<SalesOrder>();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string nCOrderCode = dr["NCOrderCode"].ToString();
                                    bool isExist = false;
                                    foreach (SalesOrder order in list)
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
                    if (dtForNo==null||dtForNo.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstSalesOrder = GetModelFromDataTable(dtForNo, out msg);
                }
                else if (base.isAlreadyStatus == "1")
                {
                    if (dtForYes==null|| dtForYes.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstSalesOrder = GetModelFromDataTable(dtForYes, out msg);
                }
                else
                {
                    lstSalesOrder = GetModelFromDataTable(dt, out msg);
                }

                if (lstSalesOrder==null|| lstSalesOrder.Count == 0)
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
        public override bool SaveToRFID(List<SalesOrder> list, out string msg)
        {
            msg = "";
            try
            {               
                if (list.Count == 0)
                {
                    //msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                    return false;
                }
                else
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        //查询系统中所有采购订单编号
                        List<SalesOrder> RFIDKeyList = (from r in context.SalesOrder select r).ToList<SalesOrder>();
                        string existNCcode = string.Empty;
                        List<SalesOrder> delList = new List<SalesOrder>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (SalesOrder order in RFIDKeyList)
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
                            foreach (SalesOrder sales in delList)
                            {
                                context.SalesOrder.DeleteObject(sales);
                                context.SaveChanges();
                            }
                        }

                        //其加入数据库
                        foreach (SalesOrder newModel in list)
                        {
                            //搜索对应的销售订单详细信息
                            List<SalesOrderDetail> lstSalesOrderDetail = new List<SalesOrderDetail>();
                            if (GetNCDataDetailJoinRFID(out lstSalesOrderDetail, out msg, newModel.NCOrderCode))
                            {
                                foreach (SalesOrderDetail detail in lstSalesOrderDetail)
                                {
                                    newModel.SalesOrderDetail.Add(detail);
                                }
                            }
                            else
                            {                               
                                return false;
                            }

                            context.SalesOrder.AddObject(newModel);
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
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "SalesOrder SaveToRFID方法出错", ex);
                msg = "SalesOrder SaveToRFID方法出错" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.SalesOrder> GetModelFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.SalesOrder> list = new List<DAL.SalesOrder>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("NCOrderCode", "订单编号");
                dataFieldNameDic.Add("BusinessType", "业务类型");
                dataFieldNameDic.Add("SalesDate", "销售日期");
                dataFieldNameDic.Add("SellDepartmentName", "销售部门");
                dataFieldNameDic.Add("Operator", "业务员");
                dataFieldNameDic.Add("ClientName", "客户名称");
                dataFieldNameDic.Add("MemberCard", "会员卡号");
                dataFieldNameDic.Add("DeliveryAddr", "收货地址");
                dataFieldNameDic.Add("ContactPerson", "联 系 人");
                dataFieldNameDic.Add("ContactPhoneNo", "联系电话");
                dataFieldNameDic.Add("TotalNumber", "合计数量");
                dataFieldNameDic.Add("Comment", "备注");
                dataFieldNameDic.Add("LeaderSign", "领导");
                dataFieldNameDic.Add("WarehouseSign", "仓库");
                dataFieldNameDic.Add("AccountSign", "财务");
                dataFieldNameDic.Add("BusinessSign", "业务");
                dataFieldNameDic.Add("EditorSign", "制单人");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<SalesOrder>();
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
                    return new List<SalesOrder>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    SalesOrder newModel = new SalesOrder();

                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);                    
                    newModel.BusinessSign = DataCheckHelper.GetCellString(dr["BusinessType"]);

                    //销售日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["SalesDate"])))
                    {
                        DateTime salesDate;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["SalesDate"]), out salesDate))
                            newModel.SalesDate = salesDate;
                    }
                    
                    newModel.SellDepartmentName= DataCheckHelper.GetCellString(dr["SellDepartmentName"]);
                    newModel.Operator= DataCheckHelper.GetCellString(dr["Operator"]);
                    newModel.ClientName= DataCheckHelper.GetCellString(dr["ClientName"]);
                    newModel.MemberCard= DataCheckHelper.GetCellString(dr["MemberCard"]);
                    newModel.DeliveryAddr= DataCheckHelper.GetCellString(dr["DeliveryAddr"]);
                    newModel.ContactPerson= DataCheckHelper.GetCellString(dr["ContactPerson"]);
                    newModel.ContactPhoneNo= DataCheckHelper.GetCellString(dr["ContactPhoneNo"]);

                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalNumber"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["TotalNumber"]), out d))
                            newModel.TotalNumber = d;
                    }

                    newModel.Comment= DataCheckHelper.GetCellString(dr["Comment"]);
                    newModel.LeaderSign= DataCheckHelper.GetCellString(dr["LeaderSign"]);
                    newModel.WarehouseSign= DataCheckHelper.GetCellString(dr["WarehouseSign"]);
                    newModel.AccountSign= DataCheckHelper.GetCellString(dr["AccountSign"]);
                    newModel.BusinessSign= DataCheckHelper.GetCellString(dr["BusinessSign"]);
                    newModel.EditorSign = DataCheckHelper.GetCellString(dr["EditorSign"]);
                      
                    newModel.OrderStatus = 2;//初始态
                    newModel.EditStatus = 0;//无人编辑
                    newModel.OrderCode = newModel.NCOrderCode;

                    List<SalesOrder> existSalesOrder = (from r in list where r.NCOrderCode == newModel.NCOrderCode select r).ToList<SalesOrder>();
                    if (existSalesOrder == null || existSalesOrder.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<SalesOrder>(); ;
            }
        }

        #endregion

        #region 销售订单明细
        /// <summary>
        /// 销售订单明细
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
        public override bool GetNCDataDetailJoinRFID(out List<SalesOrderDetail> lstSalesOrderDetail, out string msg, string billCode)
        {
            DataTable dt = null;
            lstSalesOrderDetail = null;
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
                            List<string> list = context.SalesOrderDetail.Where(r => r.NCOrderCode == billCode).Select(r => r.DetailRowNumber).ToList<string>();                           
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

                lstSalesOrderDetail = GetModelDetailFromDataTable(dt, out msg);
                if (lstSalesOrderDetail.Count == 0)
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
        /// 从DataTable中获取销售订单明细实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.SalesOrderDetail> GetModelDetailFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.SalesOrderDetail> list = new List<DAL.SalesOrderDetail>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("NCOrderCode", "订单编号 ");
                dataFieldNameDic.Add("DetailRowNumber", "行号");
                dataFieldNameDic.Add("CargoCode", "商品编码");
                dataFieldNameDic.Add("CargoName", "商品名称");
                dataFieldNameDic.Add("CargoModel", "规格 ");
                dataFieldNameDic.Add("CargoSpec", "型号");
                dataFieldNameDic.Add("CargoUnits", "单位");
                dataFieldNameDic.Add("NumOriginalPlan", "数量");
                dataFieldNameDic.Add("Price","单价");
                dataFieldNameDic.Add("Money","金额");
                dataFieldNameDic.Add("DiscountMoney", "折扣金额");
                dataFieldNameDic.Add("WHName", "发出仓库 ");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<SalesOrderDetail>();
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
                    return new List<SalesOrderDetail>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    SalesOrderDetail newModel = new SalesOrderDetail();

                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    newModel.DetailRowNumber = DataCheckHelper.GetCellString(dr["DetailRowNumber"]);
                              
                    newModel.CargoCode=DataCheckHelper.GetCellString(dr["CargoCode"]);
                    string cargoCode = DbCommonMethod.ParsingCargoCode(newModel.CargoCode);
                    if (string.IsNullOrEmpty(cargoCode))
                        throw new ApplicationException("单号" + newModel.NCOrderCode + ",商品不存在：" + newModel.CargoCode);
               
                    newModel.CargoName=DataCheckHelper.GetCellString(dr["CargoName"]);                                      
                    newModel.CargoModel=DataCheckHelper.GetCellString(dr["CargoModel"]);                                     
                    newModel.CargoSpec=DataCheckHelper.GetCellString(dr["CargoSpec"]);                                      
                    newModel.CargoUnits=DataCheckHelper.GetCellString(dr["CargoUnits"]);
                    
                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["NumOriginalPlan"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["NumOriginalPlan"]), out d))
                            newModel.NumOriginalPlan = d;
                    }

                    //单价
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["Price"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["Price"]), out d))
                            newModel.Price = d;
                    }

                    //金额
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["Money"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["Money"]), out d))
                            newModel.TotalMoney = d;
                    } 

                    //折后金额
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["DiscountMoney"])))
                    {
                        decimal d;
                        if (decimal.TryParse(DataCheckHelper.GetCellString(dr["DiscountMoney"]), out d))
                            newModel.DiscountMoney = d;
                    }  

                    newModel.WHName = DataCheckHelper.GetCellString(dr["WHName"]);
                    if (newModel.WHName != "")
                    {
                        newModel.WHCode = DbCommonMethod.ParsingWarehouse(newModel.WHName);
                        if (newModel.WHCode == "")
                        {
                            throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.WHName);
                        }
                    }

                    newModel.DetailRowStatus = 2;//初始态
                    newModel.OrderCode = newModel.OrderCode;
                    newModel.NCOrderCode = newModel.NCOrderCode;

                    List<SalesOrderDetail> existSalesOrderDetail = (from r in list where r.NCOrderCode == newModel.NCOrderCode && r.DetailRowNumber == newModel.DetailRowNumber select r).ToList<SalesOrderDetail>();
                    if (existSalesOrderDetail == null || existSalesOrderDetail.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<SalesOrderDetail>(); ;
            }
        }
        #endregion
    }
}