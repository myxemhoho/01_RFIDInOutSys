﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Gold.NCWebServiceForRFID;
using Gold.Utility;
using System.Text;
using Gold.DAL;
using System.Web.Security;

namespace Gold.NCInvoke
{
    public class BasicUserInfocInvoke : BasicInfoInvoke
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

                    if (dt.Columns.Contains("UserID"))
                    {
                        using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                        {
                            List<string> list = context.Users.Select(r => r.UserId).ToList<string>();
                            foreach (DataRow dr in dt.Rows)
                            {
                                string whcode = dr["UserID"].ToString();
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
                List<Users> list = GetModelFromDataTable(dt, out convertMsg);

                if (list == null)
                {
                    msg = "校验用友数据时出现异常！详细信息:" + convertMsg;
                    return false;
                }
                else if (list.Count == 0)
                {
                    msg = "获取用友系统数据失败！详细信息:" + convertMsg;
                    return false;
                }
                else
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        //查询系统中所有仓库编码
                        List<string> RFIDKeyList = (from r in context.Users select r.UserId).ToList<string>();
                        //NC数据集中与现有系统已重复的数据主键
                        List<int> delKeyList = new List<int>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (RFIDKeyList.Contains(list[i].UserId))
                                delKeyList.Add(i);//如果RFID系统中已存在此数据，则将索引加入待删除列表
                        }
                        delKeyList.Reverse();//待删除索引逆序
                        foreach (int delIndex in delKeyList) //从用友系统中获取的用户信息若在RFID系统中已经存在，则从新增列表中删除掉
                        {
                            list.RemoveAt(delIndex);
                        }

                        //若过滤后数据行为0，那么也认为保存成功并提示用户
                        if (list.Count == 0)
                        {
                            msg = "保存成功!影响行数：0。用友系统中商品规格数据已全部存在于RFID系统中。";
                            return true;
                        }


                        //变量过滤后的实体集，将其加入Gold数据库
                        foreach (Users newModel in list)
                        {
                            //加入到实体框架中
                            context.Users.AddObject(newModel);
                        }

                        //提交变更
                        int AffectRowsCount = context.SaveChanges();

                        try
                        {
                            if (AffectRowsCount > 0)
                            {
                                //变量过滤后的实体集，将其加入Aspnetdb数据库
                                foreach (Users newModel in list)
                                {
                                    //加入到aspnetdb中
                                    MembershipUser user = Membership.CreateUser(newModel.UserId, "123456");
                                    string[] userRoles = { "普通用户" };
                                    if (userRoles != null && userRoles.Count() > 0)
                                        Roles.AddUserToRoles(newModel.UserId, userRoles);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "UserInfo SaveToRFID方法出错", ex);

                            if (AffectRowsCount > 0) //如果Gold数据库新增成功，但membership出错，那么将Gold中和membership中的的数据全都删除
                            {
                                //变量过滤后的实体集，将其加入数据库
                                foreach (Users newModel in list)
                                {
                                    Users delModel = (from r in context.Users where r.UserId == newModel.UserId select r).FirstOrDefault<Users>();
                                    if (delModel != null)//从实体框架中删除
                                    {
                                        context.Users.DeleteObject(delModel);
                                    }

                                    //从aspnetdb中删除
                                    if (Membership.GetUser(newModel.UserId) != null)
                                    {
                                        bool delResult = Membership.DeleteUser(newModel.UserId);//删除Membership中用户
                                    }
                                }
                                context.SaveChanges();//提交变更
                            }

                            msg = "保存失败!详细信息：" + LogHelper.GetExceptionMsg(ex);
                            return false;
                        }

                        msg = "保存成功!影响行数：" + AffectRowsCount.ToString();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "UserInfo SaveToRFID方法出错", ex);
                return false;
            }
        }

        /// <summary>
        /// 从DataTable中获取实体列表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<DAL.Users> GetModelFromDataTable(DataTable dt, out string msg)
        {
            List<DAL.Users> list = new List<DAL.Users>();
            msg = "";

            //接口协议文档中定义的字段
            Dictionary<string, string> dataFieldNameDic = new Dictionary<string, string>();
            dataFieldNameDic.Add("UserID", "用户账号");
            dataFieldNameDic.Add("UserName", "用户姓名");
            dataFieldNameDic.Add("DepartmentNo", "用户所属部门编码");
            dataFieldNameDic.Add("DepartmentName", "用户所属部门名称");
            dataFieldNameDic.Add("CompanyNo", "用户所属公司编码");
            dataFieldNameDic.Add("CompanyName", "用户所属公司名称");

            if (dt.Rows.Count == 0)
            {
                msg = "用友系统返回数据集中无数据！";
                return new List<Users>();
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
                return new List<Users>(); ;
            }

            #region 检查用户数据中的部门信息是否已经在RFID系统中存在
            List<string> RFIDExistDeptList = null;//RFID系统中存在的所有部门信息
            int FirstDefaultPositionID = 0;//RFID系统中存在的所有职位信息
            using (GoldEntities context = new GoldEntities())
            {
                RFIDExistDeptList = (from r in context.Department select r.DepartmentCode).ToList<string>();
                FirstDefaultPositionID = (from r in context.Position select r.PositionId).FirstOrDefault<int>();
            }
            StringBuilder strNotExistDeptName = new StringBuilder();
            ////检查部门信息是否完整
            foreach (DataRow dr in dt.Rows)
            {
                string DepartmentCode = DataCheckHelper.GetCellString(dr["DepartmentNo"]);
                string DepartmentName = DataCheckHelper.GetCellString(dr["DepartmentName"]);
                if (RFIDExistDeptList.Contains(DepartmentCode) == false)
                {
                    if (strNotExistDeptName.Length > 0)
                        strNotExistDeptName.Append(",");
                    string join = DepartmentCode + "-" + DepartmentName;
                    if (strNotExistDeptName.ToString().Contains(join) == false)
                        strNotExistDeptName.Append(join);
                }
            }
            if (strNotExistDeptName.Length > 0)
            {
                msg = "请先导入用友系统的部门信息，然后导入用户信息！RFID系统中缺少的部门信息如下：" + strNotExistDeptName.ToString();
                return null;
            }
            #endregion

            #region 检查RFID系统中是否有“普通用户”角色
            string[] rolesArray = Roles.GetAllRoles();
            if (rolesArray.Contains("普通用户") == false)
            {
                msg = "导入用户信息需要“普通用户”角色，但RFID系统内暂无“普通用户”角色，请联系管理员进行“普通用户”角色新增后再执行用户信息导入操作！";
                return null;
            }
            #endregion

            //遍历数据集创建实体
            foreach (DataRow dr in dt.Rows)
            {
                Users newModel = new Users();

                newModel.UserId = DataCheckHelper.GetCellString(dr["UserID"]);
                newModel.UserName = DataCheckHelper.GetCellString(dr["UserName"]);
                newModel.DepartmentCode = DataCheckHelper.GetCellString(dr["DepartmentNo"]);
                newModel.PositionId = FirstDefaultPositionID;//使用默认的岗位编码
                newModel.Enabled = true;


                List<Users> existWareHouse = (from r in list where r.UserId == newModel.UserId select r).ToList<Users>();
                if (existWareHouse == null || existWareHouse.Count == 0)//过滤重复数据
                    list.Add(newModel);
            }

            return list;
        }
    }
}