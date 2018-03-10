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
    public class BasicModelInvoke : BasicInfoInvoke
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

                if (dt != null)
                {
                    dt.Columns.Add("IsExistInRFID", typeof(int));//代码表示是否RFID系统已存在此数据
                    dt.Columns.Add("IsExistInRFIDName", typeof(string));//名称表示是否RFID系统已存在此数据

                    if (dt.Columns.Contains("ModelName"))
                    {
                        using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                        {
                            List<string> list = context.WareHouse.Select(r => r.WHCode).ToList<string>();
                            foreach (DataRow dr in dt.Rows)
                            {
                                string whcode = dr["ModelName"].ToString();
                                if (list.Contains(whcode))
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
            msg = "";
            try
            {
                string convertMsg = "";
                List<Models> list = GetModelFromDataTable(dt, out convertMsg);
                if (list.Count == 0)
                {
                    msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                    return false;
                }
                else
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        //查询系统中所有仓库编码
                        List<string> RFIDKeyList = (from r in context.Models select r.ModelName).ToList<string>();
                        //NC数据集中与现有系统已重复的数据主键
                        List<int> delKeyList = new List<int>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (RFIDKeyList.Contains(list[i].ModelName))
                                delKeyList.Add(i);//如果RFID系统中已存在此数据，则将索引加入待删除列表
                        }
                        delKeyList.Reverse();//待删除索引逆序
                        foreach (int delIndex in delKeyList) //删除
                        {
                            list.RemoveAt(delIndex);
                        }

                        //若过滤后数据行为0，那么也认为保存成功并提示用户
                        if (list.Count == 0)
                        {
                            msg = "保存成功!影响行数：0。用友系统中商品型号数据已全部存在于RFID系统中。";
                            return true;
                        }

                        //变量过滤后的实体集，将其加入数据库
                        foreach (Models newModel in list)
                        {
                            context.Models.AddObject(newModel);
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
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "Models SaveToRFID方法出错", ex);
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.Models> GetModelFromDataTable(DataTable dt, out string msg)
        {
            List<DAL.Models> list = new List<DAL.Models>();
            msg = "";

            //接口协议文档中定义的字段
            Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
            dataFieldNameDic.Add("ModelCode", "型号编码");
            dataFieldNameDic.Add("ModelName", "型号名称");

            if (dt.Rows.Count == 0)
            {
                msg = "用友系统返回数据集中无数据！";
                return new List<Models>();
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
                return new List<Models>(); ;
            }

            //遍历数据集创建实体
            foreach (DataRow dr in dt.Rows)
            {
                Models newModel = new Models();

                newModel.Comment = DataCheckHelper.GetCellString(dr["ModelCode"]);
                newModel.ModelName = DataCheckHelper.GetCellString(dr["ModelName"]);


                List<Models> existWareHouse = (from r in list where r.ModelName == newModel.ModelName select r).ToList<Models>();
                if (existWareHouse == null || existWareHouse.Count == 0)//过滤重复数据
                    list.Add(newModel);
            }

            return list;
        }


    }
}