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
    public class OtherStockInBillInvoke:OrderInfoInvoke
    {
        public OtherStockInBillInvoke()
        { }

        #region 其他入库单表头
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
        public override bool GetNCDataJoinRFID(out List<StockIn> lstOtherStockin, out string msg)
        {
            DataTable dt = null;
            lstOtherStockin = null;
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
                        if (dt.Columns.Contains("BillCode"))
                        {
                            using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                            {
                                dtForNo = dt.Clone();
                                dtForYes = dt.Clone();

                                //List<StockIn> list = (from r in context.StockIn select r).ToList<StockIn>();
                                //foreach (DataRow dr in dt.Rows)
                                //{
                                //    string nCBillCode = dr["BillCode"].ToString();
                                //    bool isExist = false;
                                //    foreach (StockIn si in list)
                                //    {
                                //        //如果不是初始态，则说明已经导入且已开始其他操作
                                //        //0-初始态，1-已保存，2-已提交，3-撤销中，4-已撤销，5-已完成
                                //        //if (si.FromBillNo == nCBillCode && !(si.SIStatus == 0 || si.SIStatus == 1 || si.SIStatus == 4))
                                //        if (si.FromBillNo == nCBillCode && )
                                //        {                                                                                                                                   
                                //            dtForYes.Rows.Add(dr.ItemArray);
                                //            isExist = true;
                                //            break;
                                //        }
                                //    }
                                //    if (!isExist)
                                //    {
                                //        dtForNo.Rows.Add(dr.ItemArray);
                                //    }
                                //}
                                
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string nCBillCode = dr["BillCode"].ToString();
                                    bool isExist = false;
                                    List<StockIn> list = (from r in context.StockIn where(r.FromBillNo == nCBillCode && r.SIStatus != 4) select r).ToList<StockIn>();
                                    if(list != null && list.Count != 0)
                                    {                                        
                                        dtForYes.Rows.Add(dr.ItemArray);
                                        isExist=true;                                        
                                    }
                                    if(!isExist)
                                    {
                                        dtForNo.Rows.Add(dr.ItemArray);
                                    }                                    
                                }
                            }
                        }
                    }
                }

                if (base.isAlreadyStatus == "0")//0：未导入， 1：已导入  
                {
                    if (dtForNo==null|| dtForNo.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstOtherStockin = GetModelFromDataTable(dtForNo, out msg);
                }
                else if (base.isAlreadyStatus == "1")
                {
                    if (dtForYes==null|| dtForYes.Rows.Count == 0)
                    {
                        msg = "没有符合条件的用友数据！";
                        return false;
                    }
                    lstOtherStockin = GetModelFromDataTable(dtForYes, out msg);
                }
                else
                {
                    lstOtherStockin = GetModelFromDataTable(dt, out msg);
                }

                if (lstOtherStockin==null|| lstOtherStockin.Count == 0)
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
        public override bool SaveToRFID(List<StockIn> list, out string msg)
        {
            msg = "";
            try
            {
                //string convertMsg = "";               
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
                        List<StockIn> RFIDKeyList = (from r in context.StockIn select r).ToList<StockIn>();

                        string existNCcode = string.Empty;
                        List<StockIn> delList = new List<StockIn>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (StockIn si in RFIDKeyList)
                            {
                                //0-初始态，1-已保存，2-已提交，3-撤销中，4-已撤销，5-已完成
                                if (si.FromBillNo == list[i].FromBillNo && (si.SIStatus == 2 || si.SIStatus == 3 || si.SIStatus == 5))
                                {
                                    existNCcode += list[i].FromBillNo + ",";
                                    break;
                                    
                                }
                                else if (si.FromBillNo == list[i].FromBillNo && si.SIStatus != 4)
                                {
                                    delList.Add(si);
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
                            foreach (StockIn delSI in delList)
                            {
                                foreach (StockIn newSI in list)
                                {
                                    //把旧的入库单号保留到新的单上
                                    if (newSI.FromBillNo == delSI.FromBillNo)
                                    {
                                        newSI.SICode = delSI.SICode;
                                        break;
                                    }
                                }
                                context.StockIn.DeleteObject(delSI);
                                context.SaveChanges();
                            }
                        }

                        //其加入数据库
                        foreach (StockIn newModel in list)
                        {
                            //生成入库单号
                            if (newModel.SICode == null)
                            {
                                newModel.SICode = KeyGenerator.Instance.GetStockInKey();
                            }

                            //搜索对应的采购订单详细信息
                            List<StockDetail> lstStockDetail = new List<StockDetail>();
                            if (GetNCDataDetailJoinRFID(out lstStockDetail, out msg, newModel.FromBillNo, newModel.SICode, newModel.WHCode))
                            {
                                foreach (StockDetail detail in lstStockDetail)
                                {
                                    newModel.StockDetail.Add(detail);
                                }
                            }
                            else
                            {
                                //msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                                return false;
                            }

                            context.StockIn.AddObject(newModel);
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
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "OtherStockInBill SaveToRFID方法出错", ex);
                msg = "OtherStockInBill SaveToRFID方法出错" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.StockIn> GetModelFromDataTable(DataTable dt, out string msg)
        {
            try
            {
                List<DAL.StockIn> list = new List<DAL.StockIn>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("BillCode", "其他入库单号");
                dataFieldNameDic.Add("BillDate", "日期");
                dataFieldNameDic.Add("InCategory", "收发类别");
                dataFieldNameDic.Add("Supplier", "供货单位");
                dataFieldNameDic.Add("BusinessDepartmentName", "业务部门");
                dataFieldNameDic.Add("Operator", "业务员姓名");
                dataFieldNameDic.Add("WHName", "仓库名称");
                dataFieldNameDic.Add("StoreKeeper", "库管员");
                dataFieldNameDic.Add("Comment", "备注");
                dataFieldNameDic.Add("TotalNumber", "合计数量");
                dataFieldNameDic.Add("WareHouseKeeper", "仓管");
                dataFieldNameDic.Add("Checker", "验收");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<StockIn>();
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
                    return new List<StockIn>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    StockIn newModel = new StockIn();
                    //newModel.SICode = KeyGenerator.Instance.GetStockInKey();
                    newModel.FromBillNo = DataCheckHelper.GetCellString(dr["BillCode"]);

                    //日期
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["BillDate"])))
                    {
                        DateTime d;
                        if (DateTime.TryParse(DataCheckHelper.GetCellString(dr["BillDate"]), out d))
                            newModel.SIDate = d;
                    }

                    //收发类别
                    newModel.InCategory = DbCommonMethod.GetInCategory(DataCheckHelper.GetCellString(dr["InCategory"]));
                    
                    newModel.Supplier = DataCheckHelper.GetCellString(dr["Supplier"]);
                    newModel.BusinessDepartmentName = DataCheckHelper.GetCellString(dr["BusinessDepartmentName"]);
                    newModel.Operator = DataCheckHelper.GetCellString(dr["Operator"]);

                    //仓库
                    newModel.WHName = DataCheckHelper.GetCellString(dr["WHName"]);
                    newModel.WHCode = DbCommonMethod.ParsingWarehouse(newModel.WHName);
                    if (string.IsNullOrEmpty(newModel.WHCode))
                        throw new ApplicationException("单号" + newModel.FromBillNo + ",仓库不存在：" + newModel.WHName);

                    newModel.StoreKeeper = DataCheckHelper.GetCellString(dr["StoreKeeper"]);
                    newModel.Comment = DataCheckHelper.GetCellString(dr["Comment"]);

                    //合计数量
                    if (!string.IsNullOrEmpty(DataCheckHelper.GetCellString(dr["TotalNumber"])))
                    {
                        double d;
                        if (double.TryParse(DataCheckHelper.GetCellString(dr["TotalNumber"]), out d))
                            newModel.TotalNumber = d;
                    }
                    
                    //newModel = DataCheckHelper.GetCellString(dr["WareHouseKeeper"]);
                    newModel.Checker = DataCheckHelper.GetCellString(dr["Checker"]);

                    newModel.SIStatus = 1;//已保存
                    newModel.FromBillType = "23";//21采购入库单、22调拨入库单、23其他入库单
                    newModel.SIType = CommonConvert.GetSIOTypeCode("其他入库单");
                    newModel.FromType = CommonConvert.GetFromTypeCode("源于Excel导入的入库单");

                    List<StockIn> existStockIn = (from r in list where r.FromBillNo == newModel.FromBillNo select r).ToList<StockIn>();
                    if (existStockIn == null || existStockIn.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<StockIn>(); ;
            }
        }

        #endregion

        #region 其他入库单明细
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
        public override bool GetNCDataDetailJoinRFID(out List<StockDetail> lstStockDetail, out string msg, string billCode, string SICode,string WHCode)
        {
            DataTable dt = null;
            lstStockDetail = null;
            msg = "";
            try
            {

                if (GetNCDataDetail(out dt, out msg, billCode) == false)
                    return false;

                lstStockDetail = GetModelDetailFromDataTable(dt, out msg, SICode, WHCode);
                if (lstStockDetail==null|| lstStockDetail.Count == 0)
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
        private List<DAL.StockDetail> GetModelDetailFromDataTable(DataTable dt, out string msg, string SICode, string WHCode)
        {
            try
            {
                List<DAL.StockDetail> list = new List<DAL.StockDetail>();
                msg = "";

                //接口协议文档中定义的字段
                Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
                dataFieldNameDic.Add("BillCode", "其他入库单号");
                dataFieldNameDic.Add("DetailRowNumber", "行号");
                dataFieldNameDic.Add("CargoCode", "商品编码");
                dataFieldNameDic.Add("CargoName", "商品名称");
                dataFieldNameDic.Add("CargoSpec", "规格");
                dataFieldNameDic.Add("CargoModel", "型号");
                dataFieldNameDic.Add("CargoUnits", "单位");
                dataFieldNameDic.Add("NumOriginalPlan", "数量");
                dataFieldNameDic.Add("Remark", "备注");

                if (dt == null || dt.Rows.Count == 0)
                {
                    msg = "用友系统返回数据集中无数据！";
                    return new List<StockDetail>();
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
                    return new List<StockDetail>(); ;
                }

                //遍历数据集创建实体
                foreach (DataRow dr in dt.Rows)
                {
                    StockDetail newModel = new StockDetail();
                    
                    //newModel.BillCode= DataCheckHelper.GetCellString(dr["BillCode"]);
                    newModel.BillRowNumber= DataCheckHelper.GetCellString(dr["DetailRowNumber"]);
                    //商品编码
                    newModel.CargoCode= DataCheckHelper.GetCellString(dr["CargoCode"]);
                    string cargoCode = DbCommonMethod.ParsingCargoCode(newModel.CargoCode);
                    if (string.IsNullOrEmpty(cargoCode))
                        throw new ApplicationException("单号" + DataCheckHelper.GetCellString(dr["BillCode"]) + ",商品不存在：" + newModel.CargoCode);

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
                    
                    newModel.Comment = DataCheckHelper.GetCellString(dr["Remark"]);

                    newModel.NumCurrentPlan = newModel.NumOriginalPlan;//订单数量=应收数量，默认值
                    newModel.CargoStatus = 0;//手持机未完成
                    newModel.SICode = SICode;
                    newModel.BillCode = newModel.SICode;                    
                    newModel.BillType = CommonConvert.GetBillTypeCode("入库单");
                    newModel.InOutWHCode = WHCode;

                    string bill=DataCheckHelper.GetCellString(dr["BillCode"]);
                    List<StockDetail> existStockDetail = (from r in list where r.BillCode == bill && r.BillRowNumber == newModel.BillRowNumber select r).ToList<StockDetail>();
                    if (existStockDetail == null || existStockDetail.Count == 0)//过滤重复数据
                        list.Add(newModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new List<StockDetail>(); ;
            }
        }
        #endregion
    }
}