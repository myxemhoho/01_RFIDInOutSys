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
    public class OrderTransferInvoke : OrderInfoInvoke
    {
        public OrderTransferInvoke()
        { }

        #region 调拨订单表头
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
        public override bool GetNCDataJoinRFID(out List<TransferOrder> lstTransferOrder, out string msg)
        {
            DataTable dt = null;
            lstTransferOrder = null;
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

                                List<TransferOrder> list = (from r in context.TransferOrder select r).ToList<TransferOrder>();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string nCOrderCode = dr["NCOrderCode"].ToString();
                                    bool isExist = false;
                                    foreach (TransferOrder order in list)
                                    {
                                        //如果不是初始态，则说明已经导入且已开始其他操作
                                        //if (order.NCOrderCode == nCOrderCode && order.OrderStatus != 2)
                                        if (order.NCOrderCode == nCOrderCode)
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
                    lstTransferOrder = GetModelFromDataTable(dtForNo, out msg);
                }
                else if (base.isAlreadyStatus == "1")
                {
                    if (dtForYes==null|| dtForYes.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstTransferOrder = GetModelFromDataTable(dtForYes, out msg);
                }
                else
                {
                    lstTransferOrder = GetModelFromDataTable(dt, out msg);
                }

                if (lstTransferOrder==null|| lstTransferOrder.Count == 0)
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
        public override bool SaveToRFID(List<TransferOrder> list, out string msg)
        {
            msg = "";
            try
            {
                //string convertMsg = "";
                //List<PurchaseOrder> list = GetModelFromDataTable(dt, out convertMsg);
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
                        List<TransferOrder> RFIDKeyList = (from r in context.TransferOrder select r).ToList<TransferOrder>();
                        string existNCcode = string.Empty;
                        List<TransferOrder> delList = new List<TransferOrder>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (TransferOrder order in RFIDKeyList)
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
                            foreach (TransferOrder tran in delList)
                            {
                                context.TransferOrder.DeleteObject(tran);
                                context.SaveChanges();
                            }
                        }

                        //其加入数据库
                        foreach (TransferOrder newModel in list)
                        {
                            //搜索对应的采购订单详细信息
                            List<TransferOrderDetail> lstTransferOrderDetail = new List<TransferOrderDetail>();
                            if (GetNCDataDetailJoinRFID(out lstTransferOrderDetail, out msg, newModel.NCOrderCode))
                            {
                                foreach (TransferOrderDetail detail in lstTransferOrderDetail)
                                {
                                    newModel.TransferOrderDetail.Add(detail);
                                }
                            }
                            else
                            {
                                //msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                                return false;
                            }

                            context.TransferOrder.AddObject(newModel);
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
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "TransferOrder SaveToRFID方法出错", ex);
                msg = "TransferOrder SaveToRFID方法出错" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.TransferOrder> GetModelFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.TransferOrder> list = new List<DAL.TransferOrder>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();                
                dataFieldNameDic.Add("NCOrderCode", "订单编号");
                dataFieldNameDic.Add("OrderDate", "单据日期");
                dataFieldNameDic.Add("OutWHName", "调出仓库");
                dataFieldNameDic.Add("OutOrganization", "调出组织");
                dataFieldNameDic.Add("OutCompany", "调出公司");
                dataFieldNameDic.Add("InWHName", "调入仓库");
                dataFieldNameDic.Add("InOrganization", "调入组织");
                dataFieldNameDic.Add("InCompany", "调入公司");
                dataFieldNameDic.Add("BillType", "调拨类型");
                dataFieldNameDic.Add("Comment", "备注");
                dataFieldNameDic.Add("Preparer", "制单人");
                dataFieldNameDic.Add("PreparerTime", "制单时间");
                dataFieldNameDic.Add("Approver", "审批人");
                dataFieldNameDic.Add("ApproverDate", "审批日期");
                dataFieldNameDic.Add("CheckTime", "审核时间");
                dataFieldNameDic.Add("LastEditor", "最后修改人");
                dataFieldNameDic.Add("LastEditTime", "最后修改时间");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<TransferOrder>();
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
                    return new List<TransferOrder>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    TransferOrder newModel = new TransferOrder();
                    
                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);

                    //订单日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["OrderDate"])))
                    {
                        DateTime d;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["OrderDate"]), out d))
                            newModel.OrderDate = d;
                    }

                    newModel.OutWHName = DataCheckHelper.GetCellString(dr["OutWHName"]);
                    if (newModel.OutWHName != "")
                    {
                        newModel.OutWHCode = DbCommonMethod.ParsingWarehouse(newModel.OutWHName);
                        if (string.IsNullOrEmpty(newModel.OutWHCode))
                            throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.OutWHName);
                    }

                    newModel.OutOrganization = DataCheckHelper.GetCellString(dr["OutOrganization"]);
                    newModel.OutCompany = DataCheckHelper.GetCellString(dr["OutCompany"]);

                    newModel.InWHName = DataCheckHelper.GetCellString(dr["InWHName"]);
                    if (newModel.InWHName != "")
                    {
                        newModel.InWHCode = DbCommonMethod.ParsingWarehouse(newModel.InWHName);
                        if (string.IsNullOrEmpty(newModel.InWHCode))
                            throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.InWHName);
                    }

                    newModel.InOrganization = DataCheckHelper.GetCellString(dr["InOrganization"]);
                    newModel.InCompany = DataCheckHelper.GetCellString(dr["InCompany"]);
                    //newModel = DataCheckHelper.GetCellString(dr["BillType"]);
                    //newModel = DataCheckHelper.GetCellString(dr["Comment"]);
                    //newModel = DataCheckHelper.GetCellString(dr["Preparer"]);
                    //newModel = DataCheckHelper.GetCellString(dr["PreparerTime"]);
                    //newModel = DataCheckHelper.GetCellString(dr["Approver"]);
                    //newModel = DataCheckHelper.GetCellString(dr["ApproverDate"]);
                    //newModel = DataCheckHelper.GetCellString(dr["CheckTime"]);
                    //newModel = DataCheckHelper.GetCellString(dr["LastEditor"]);

                    //最后修改时间
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["LastEditTime"])))
                    {
                        DateTime d;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["LastEditTime"]), out d))
                            newModel.LastModifyTime = d;
                    }

                    newModel.OrderStatus = 2;//初始态
                    newModel.EditStatus = 0;//无人编辑
                    newModel.OrderCode = newModel.NCOrderCode;

                    List<TransferOrder> existTransferOrder = (from r in list where r.NCOrderCode == newModel.NCOrderCode select r).ToList<TransferOrder>();
                    if (existTransferOrder == null || existTransferOrder.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<TransferOrder>(); ;
            }
        }

        #endregion

        #region 调拨订单明细
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
        public override bool GetNCDataDetailJoinRFID(out List<TransferOrderDetail> lstTransferOrderDetail, out string msg, string billCode)
        {
            DataTable dt = null;
            lstTransferOrderDetail = null;
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
                            List<string> list = context.TransferOrderDetail.Where(r => r.NCOrderCode == billCode).Select(r => r.DetailRowNumber).ToList<string>();
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

                lstTransferOrderDetail = GetModelDetailFromDataTable(dt, out msg);
                if (lstTransferOrderDetail==null|| lstTransferOrderDetail.Count == 0)
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
        private List<DAL.TransferOrderDetail> GetModelDetailFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.TransferOrderDetail> list = new List<DAL.TransferOrderDetail>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();

                dataFieldNameDic.Add("NCOrderCode", "订单编号");
                dataFieldNameDic.Add("DetailRowNumber", "行号");
                dataFieldNameDic.Add("CargoCode", "存货编码");
                dataFieldNameDic.Add("CargoName", "存货名称");
                dataFieldNameDic.Add("CargoSpec", "规格");
                dataFieldNameDic.Add("CargoModel", "型号");
                dataFieldNameDic.Add("CargoUnits", "主计量单位");
                dataFieldNameDic.Add("CargoUnitsAssist", "辅单位");
                dataFieldNameDic.Add("ChangeRatio", "换算率");
                dataFieldNameDic.Add("NumOriginalPlanAssist", "辅数量");
                dataFieldNameDic.Add("NumOriginalPlan", "数量");
                dataFieldNameDic.Add("Lot", "批次号");
                dataFieldNameDic.Add("ProductDate", "生产日期");
                dataFieldNameDic.Add("OutWHName", "调出仓库");
                dataFieldNameDic.Add("InWHName", "调入仓库");
                dataFieldNameDic.Add("OutWHNameAssist", "出货仓库");
                dataFieldNameDic.Add("OutDept", "调出部门");
                dataFieldNameDic.Add("OutOperator", "调出部门业务员");
                dataFieldNameDic.Add("InDept", "调入部门");
                dataFieldNameDic.Add("InOperator", "调入部门业务员");
                dataFieldNameDic.Add("OutDeptAssist", "出货部门");
                dataFieldNameDic.Add("OutOperatorAssist", "出货部门业务员姓名");
                dataFieldNameDic.Add("OutArea", "调出货位");
                dataFieldNameDic.Add("InArea", "调入货位");
                dataFieldNameDic.Add("OutAreaAssist", "出货货位");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<TransferOrderDetail>();
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
                    return new List<TransferOrderDetail>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    TransferOrderDetail newModel = new TransferOrderDetail();                    

                    newModel.NCOrderCode = DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    newModel.DetailRowNumber = DataCheckHelper.GetCellString(dr["DetailRowNumber"]);

                    newModel.CargoCode = DataCheckHelper.GetCellString(dr["CargoCode"]);
                    string cargoCode = DbCommonMethod.ParsingCargoCode(newModel.CargoCode);
                    if (string.IsNullOrEmpty(cargoCode))
                        throw new ApplicationException("单号" + newModel.NCOrderCode + ",商品不存在：" + newModel.CargoCode);

                    newModel.CargoName = DataCheckHelper.GetCellString(dr["CargoName"]);
                    newModel.CargoSpec = DataCheckHelper.GetCellString(dr["CargoSpec"]);
                    newModel.CargoModel = DataCheckHelper.GetCellString(dr["CargoModel"]);
                    newModel.CargoUnits = DataCheckHelper.GetCellString(dr["CargoUnits"]);
                    //newModel = DataCheckHelper.GetCellString(dr["CargoUnitsAssist"]);
                    //newModel = DataCheckHelper.GetCellString(dr["ChangeRatio"]);
                    //newModel = DataCheckHelper.GetCellString(dr["NumOriginalPlanAssist"]);

                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["NumOriginalPlan"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["NumOriginalPlan"]), out d))
                            newModel.NumOriginalPlan = d;
                    }
                    
                    //newModel = DataCheckHelper.GetCellString(dr["Lot"]);
                    //newModel = DataCheckHelper.GetCellString(dr["ProductDate"]);

                    newModel.OutWHName = DataCheckHelper.GetCellString(dr["OutWHName"]);
                    if (newModel.OutWHName != "")
                    {
                        newModel.OutWHCode = DbCommonMethod.ParsingWarehouse(newModel.OutWHName);
                        throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.OutWHName);
                    }

                    newModel.InWHName = DataCheckHelper.GetCellString(dr["InWHName"]);
                    if (newModel.InWHName != "")
                    {
                        newModel.InWHCode = DbCommonMethod.ParsingWarehouse(newModel.InWHName);
                        throw new ApplicationException("单号" + newModel.NCOrderCode + ",仓库不存在：" + newModel.InWHName);
                    }

                    //newModel = DataCheckHelper.GetCellString(dr["OutWHNameAssist"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutDept"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutOperator"]);
                    //newModel = DataCheckHelper.GetCellString(dr["InDept"]);
                    //newModel = DataCheckHelper.GetCellString(dr["InOperator"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutDeptAssist"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutOperatorAssist"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutArea"]);
                    //newModel = DataCheckHelper.GetCellString(dr["InArea"]);
                    //newModel = DataCheckHelper.GetCellString(dr["OutAreaAssist"]);

                    newModel.DetailRowStatus = 2;//初始态
                    newModel.OrderCode = newModel.OrderCode;
                    newModel.NCOrderCode = newModel.NCOrderCode;

                    List<TransferOrderDetail> existTransferOrderDetail = (from r in list where r.NCOrderCode == newModel.NCOrderCode && r.DetailRowNumber == newModel.DetailRowNumber select r).ToList<TransferOrderDetail>();
                    if (existTransferOrderDetail == null || existTransferOrderDetail.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<TransferOrderDetail>(); ;
            }
        }
        #endregion
    }
}