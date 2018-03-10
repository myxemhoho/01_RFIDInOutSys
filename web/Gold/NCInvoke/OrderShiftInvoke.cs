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
    public class OrderShiftInvoke : OrderInfoInvoke
    {
        #region 转库单表头
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
        public override bool GetNCDataJoinRFID(out List<ShiftOrder> lstShiftOrder, out string msg)
        {
            DataTable dt = null;
            lstShiftOrder = null;
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

                                List<ShiftOrder> list = (from r in context.ShiftOrder select r).ToList<ShiftOrder>();
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string nCOrderCode = dr["NCOrderCode"].ToString();
                                    bool isExist = false;
                                    foreach (ShiftOrder order in list)
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
                    lstShiftOrder = GetModelFromDataTable(dtForNo, out msg);
                }
                else if (base.isAlreadyStatus == "1")
                {
                    if (dtForYes==null|| dtForYes.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstShiftOrder = GetModelFromDataTable(dtForYes, out msg);
                }
                else
                {
                    lstShiftOrder = GetModelFromDataTable(dt, out msg);
                }

                if (lstShiftOrder==null|| lstShiftOrder.Count == 0)
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
        public override bool SaveToRFID(List<ShiftOrder> list, out string msg)
        {
            msg = "";
            try
            {                
                //List<ShiftOrder> list = GetModelFromDataTable(dt, out convertMsg);
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
                        List<ShiftOrder> RFIDKeyList = (from r in context.ShiftOrder select r).ToList<ShiftOrder>();
                        string existNCcode = string.Empty;
                        List<ShiftOrder> delList = new List<ShiftOrder>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (ShiftOrder order in RFIDKeyList)
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
                            foreach (ShiftOrder shift in delList)
                            {
                                context.ShiftOrder.DeleteObject(shift);
                                context.SaveChanges();
                            }
                        }

                        //其加入数据库
                        foreach (ShiftOrder newModel in list)
                        {
                            //搜索对应的采购订单详细信息
                            List<ShiftOrderDetail> lstShiftOrderDetail = new List<ShiftOrderDetail>();
                            if (GetNCDataDetailJoinRFID(out lstShiftOrderDetail, out msg, newModel.NCOrderCode))
                            {
                                foreach (ShiftOrderDetail detail in lstShiftOrderDetail)
                                {
                                    newModel.ShiftOrderDetail.Add(detail);
                                }
                            }
                            else
                            {
                                //msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                                return false;
                            }

                            context.ShiftOrder.AddObject(newModel);
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
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "ShiftOrder SaveToRFID方法出错", ex);
                msg = "ShiftOrder SaveToRFID方法出错" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.ShiftOrder> GetModelFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.ShiftOrder> list = new List<DAL.ShiftOrder>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                
                dataFieldNameDic.Add("NCOrderCode","转库单编号");
                dataFieldNameDic.Add("Comment", "转库单摘要");
                dataFieldNameDic.Add("OrderDate", "转库单日期");
                dataFieldNameDic.Add("OutWHName", "调出仓库名称");                
                dataFieldNameDic.Add("InWHName","调入仓库名称");
                dataFieldNameDic.Add("TotalNumber", "合计数量");
                dataFieldNameDic.Add("LeaderName", "领导");
                dataFieldNameDic.Add("SenderName", "发货人");
                dataFieldNameDic.Add("ReceiverName", "收货人");
                dataFieldNameDic.Add("BusinessManager", "业务经理");


                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<ShiftOrder>();
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
                    return new List<ShiftOrder>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    ShiftOrder newModel = new ShiftOrder();                    
                    newModel.NCOrderCode= DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    newModel.Comment= DataCheckHelper.GetCellString(dr["Comment"]);

                    //日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["OrderDate"])))
                    {
                        DateTime d;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["OrderDate"]), out d))
                            newModel.OrderDate = d;
                    }
                    
                    newModel.OutWHName= DataCheckHelper.GetCellString(dr["OutWHName"]);
                    if (newModel.OutWHName != "")
                    {
                        newModel.OutWHCode = DbCommonMethod.ParsingWarehouse(newModel.OutWHName);
                        if (string.IsNullOrEmpty(newModel.OutWHCode))
                            throw new ApplicationException("单号"+newModel.NCOrderCode+",调出仓库不存在：" + newModel.OutWHName);
                    }

                    newModel.InWHName= DataCheckHelper.GetCellString(dr["InWHName"]);
                    if (newModel.InWHName != "")
                    {
                        newModel.InWHCode = DbCommonMethod.ParsingWarehouse(newModel.InWHName);
                        if (string.IsNullOrEmpty(newModel.InWHCode))
                            throw new ApplicationException("单号" + newModel.NCOrderCode + ",调入仓库不存在：" + newModel.InWHName);
                    }

                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalNumber"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["TotalNumber"]), out d))
                            newModel.TotalNumber = d;
                    }
                    
                    newModel.LeaderName= DataCheckHelper.GetCellString(dr["LeaderName"]);
                    newModel.SenderName= DataCheckHelper.GetCellString(dr["SenderName"]);
                    newModel.ReceiverName= DataCheckHelper.GetCellString(dr["ReceiverName"]);
                    newModel.BusinessManager = DataCheckHelper.GetCellString(dr["BusinessManager"]);
    

                    newModel.OrderStatus = 2;//初始态
                    newModel.EditStatus = 0;//无人编辑
                    newModel.OrderCode = newModel.NCOrderCode;

                    List<ShiftOrder> existShiftOrder = (from r in list where r.NCOrderCode == newModel.NCOrderCode select r).ToList<ShiftOrder>();
                    if (existShiftOrder == null || existShiftOrder.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<ShiftOrder>(); ;
            }
        }

        #endregion

        #region 转库单明细
        /// <summary>
        /// 转库单明细
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
        public override bool GetNCDataDetailJoinRFID(out List<ShiftOrderDetail> lstShiftOrderDetail, out string msg, string billCode)
        {
            DataTable dt = null;
            lstShiftOrderDetail = null;
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
                            List<string> list = context.ShiftOrderDetail.Where(r => r.NCOrderCode == billCode).Select(r => r.DetailRowNumber).ToList<string>();                           
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

                lstShiftOrderDetail = GetModelDetailFromDataTable(dt, out msg);
                if (lstShiftOrderDetail.Count == 0)
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
        private List<DAL.ShiftOrderDetail> GetModelDetailFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.ShiftOrderDetail> list = new List<DAL.ShiftOrderDetail>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();               
                dataFieldNameDic.Add("NCOrderCode","转库单编号");
                dataFieldNameDic.Add("DetailRowNumber","行号");     
                dataFieldNameDic.Add("CargoCode","商品编码");  
                dataFieldNameDic.Add("CargoName","商品名称");  
                dataFieldNameDic.Add("CargoSpec","规格");      
                dataFieldNameDic.Add("CargoModel","型号");      
                dataFieldNameDic.Add("CargoUnits","单位");
                dataFieldNameDic.Add("NumOriginalPlan", "数量");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<ShiftOrderDetail>();
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
                    return new List<ShiftOrderDetail>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    ShiftOrderDetail newModel = new ShiftOrderDetail();

                   
                    newModel.NCOrderCode= DataCheckHelper.GetCellString(dr["NCOrderCode"]);
                    newModel.DetailRowNumber= DataCheckHelper.GetCellString(dr["DetailRowNumber"]);

                    newModel.CargoCode= DataCheckHelper.GetCellString(dr["CargoCode"]);
                    string cargoCode = DbCommonMethod.ParsingCargoCode(newModel.CargoCode);
                    if (string.IsNullOrEmpty(cargoCode))
                        throw new ApplicationException("商品不存在：" + newModel.CargoCode);

                    newModel.CargoName= DataCheckHelper.GetCellString(dr["CargoName"]);
                    newModel.CargoSpec= DataCheckHelper.GetCellString(dr["CargoSpec"]);
                    newModel.CargoModel= DataCheckHelper.GetCellString(dr["CargoModel"]);
                    newModel.CargoUnits= DataCheckHelper.GetCellString(dr["CargoUnits"]);

                    //数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["NumOriginalPlan"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["NumOriginalPlan"]), out d))
                            newModel.NumOriginalPlan = d;
                    }                    

                    newModel.DetailRowStatus = 2;//初始态
                    newModel.OrderCode = newModel.OrderCode;
                    newModel.NCOrderCode = newModel.NCOrderCode;

                    List<ShiftOrderDetail> existShiftOrderDetail = (from r in list where r.NCOrderCode == newModel.NCOrderCode && r.DetailRowNumber == newModel.DetailRowNumber select r).ToList<ShiftOrderDetail>();
                    if (existShiftOrderDetail == null || existShiftOrderDetail.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<ShiftOrderDetail>(); ;
            }
        }
        #endregion
    }    
}