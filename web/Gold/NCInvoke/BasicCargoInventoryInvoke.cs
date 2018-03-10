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
    public class BasicCargoInventoryInvoke : BasicInfoInvoke
    {

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
                string xmlOld = NCWSClient.BasicInfoQuery(base.TypeArgs, base.ConditionArgs);
                //按系统设置的编码格式进行xml数据解码
                string xml = XmlHelper.GetStringByDefaultEncodingType(xmlOld);

                //记录日志
                StringBuilder strLog = new StringBuilder("调用BasicInfoQuery 参数：");
                strLog.Append(base.TypeArgs);
                strLog.Append(",");
                strLog.Append(base.ConditionArgs);
                //strLog.Append(" xml数据：");//库存商品信息量大，这里日志不记录xml数据
                //strLog.Append(xml);


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
                StringBuilder strLog = new StringBuilder("调用BasicInfoQuery失败！ 参数：");
                strLog.Append(base.TypeArgs);
                strLog.Append(",");
                strLog.Append(base.ConditionArgs);
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
        public override bool GetNCDataJoinRFID(out DataTable dt, out string msg)
        {
            dt = null;
            msg = "";
            try
            {

                if (GetNCData(out dt, out msg) == false)
                    return false;

                //if (dt != null)
                //{
                //    dt.Columns.Add("IsExistInRFID", typeof(int));//代码表示是否RFID系统已存在此数据
                //    dt.Columns.Add("IsExistInRFIDName", typeof(string));//名称表示是否RFID系统已存在此数据

                //    if (dt.Columns.Contains("CargoCode"))
                //    {
                //        using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                //        {
                //            List<string> list = context.Cargos.Select(r => r.CargoCode).ToList<string>();
                //            foreach (DataRow dr in dt.Rows)
                //            {
                //                string whcode = dr["CargoCode"].ToString();
                //                if (list.Contains(whcode))
                //                {
                //                    dr["IsExistInRFID"] = "1";
                //                    dr["IsExistInRFIDName"] = "已存在";
                //                }
                //                else
                //                {
                //                    dr["IsExistInRFID"] = "0";
                //                    dr["IsExistInRFIDName"] = "未存在";
                //                }
                //            }
                //        }
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogLevel.Error, "调用GetNCDataJoinRFID失败", ex);
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
        public override bool SaveToRFID(System.Data.DataTable dt, out string msg)
        {
            //这里暂不实现库存更新的方法，用友提供的库存只供RFID系统查询和对比，不更新
            return base.SaveToRFID(dt, out msg);
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

    }
}