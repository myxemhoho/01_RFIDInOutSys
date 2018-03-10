using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

namespace Gold.Utility
{

    /// <summary>
    /// 提供DataTable与XML互转方法的类
    /// </summary>
    public class ConvertDataTableXML
    {
        /// <summary>
        /// 读写XML的编码格式
        /// </summary>
        static UTF8Encoding UTF8EncodingObj = new UTF8Encoding();

        ////定义缓存文件存放的路径
        public static readonly string path = System.Web.HttpContext.Current.Server.MapPath("~/SaleCargoSetting/News/");//Directory.GetCurrentDirectory() + @"\LocalConfigData\";

        #region 基础操作

        /// <summary>
        /// 检查缓存文件存放的目录是否存在，如果不存在则创建。
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CheckDirectory(string directoryPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(directoryPath);
            if (!dInfo.Exists)
            {
                dInfo.Create();
            }
        }

        /// <summary>
        /// 根据给定的路径和文件名创建文件
        /// </summary>
        /// <param name="fileNameAndPath">包含文件名的路径</param>
        private static void CreateFile(string fileNameAndPath)
        {
            FileInfo fi = new FileInfo(fileNameAndPath);
            if (!fi.Exists)
            {
                FileStream fsCreate = null;
                try
                {
                    fsCreate = fi.Create();//注意这里一定要关闭创建文件使用的流，否则下面再次操作流时会报另外进程占用
                }
                catch (Exception ex)
                {
                    Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "创建文件失败！", ex);
                }
                finally
                {
                    if (fsCreate != null)
                    {
                        fsCreate.Close();
                    }
                }
            }
        }



        /// <summary>
        /// 判断数据表对应的xml文件是否存在
        /// </summary>
        /// <param name="TableName">数据表名，即文件名</param>
        /// <returns></returns>
        public static bool IsFileExist(string TableName)
        {
            string fileNameAndPath = path + TableName + ".xml";
            FileInfo fi = new FileInfo(fileNameAndPath);
            return fi.Exists;
        }

        /// <summary>
        /// 将xml字符串转换成DataTable
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static DataTable ConvertXmlStrToDataTable(string xmlStr)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            StringReader strReader = null;
            XmlTextReader xmlTextReader = null;
            try
            {
                strReader = new StringReader(xmlStr);
                xmlTextReader = new XmlTextReader(strReader);
                //dt.ReadXml(xmlTextReader);//不能用dt.ReadXml,会报不支持xml架构推断
                //return dt;
                ds.ReadXml(xmlTextReader);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xmlTextReader != null)
                {
                    xmlTextReader.Close();
                    strReader.Close();
                    strReader.Dispose();
                }
            }
        }

        /// <summary>
        /// 将xml字符串转换成DataTable
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static DataSet ConvertXmlStrToDataSet(string xmlStr)
        {
            DataSet ds = new DataSet();

            StringReader strReader = null;
            XmlTextReader xmlTextReader = null;
            try
            {
                strReader = new StringReader(xmlStr);
                xmlTextReader = new XmlTextReader(strReader);
                //dt.ReadXml(xmlTextReader);//不能用dt.ReadXml,会报不支持xml架构推断
                //return dt;
                ds.ReadXml(xmlTextReader);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xmlTextReader != null)
                {
                    xmlTextReader.Close();
                    strReader.Close();
                    strReader.Dispose();
                }
            }
        }

        /// <summary>
        /// 将DataTable转换成XML字符串,注意DataTable在序列化之前须设置DataTable的TableName属性
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDataTableToXML(DataTable dt)
        {
            MemoryStream ms = null;
            XmlTextWriter xw = null;
            try
            {
                ms = new MemoryStream();
                xw = new XmlTextWriter(ms, UTF8EncodingObj); //Encoding.Unicode);
                dt.WriteXml(xw, XmlWriteMode.WriteSchema);//加上XmlWriteMode.WriteSchema
                int count = (int)ms.Length;
                byte[] temp = new byte[count];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(temp, 0, count);
                //UnicodeEncoding uCode = new UnicodeEncoding();
                string xmlResult = UTF8EncodingObj.GetString(temp).Trim();//uCode.GetString(temp).Trim();
                return xmlResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                    ms.Close();
                    ms.Dispose();
                }
            }
        }


        /// <summary>
        ///将DataTable写入xml文件,默认写入bin\LocalConfigData目录下
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="xmlFileName">无后缀的文件名</param>
        /// <returns>1成功，0失败</returns>
        public static int WriteDataTableToXMLFile(DataTable dt, string xmlFileName)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dtNewTable = dt.Copy();//这里只能用其副本，不能用原来的数据进行添加，否则会报错“DataTable 已属于另一个DataSet”
                ds.Tables.Add(dtNewTable);

                CheckDirectory(path);
                //if (ds.Tables[0].TableName == null || ds.Tables[0].TableName == "")
                //{
                //throw new Exception("数据表名称不能为空！");
                ds.Tables[0].TableName = xmlFileName;
                //}
                if (xmlFileName == null || xmlFileName == "")
                {
                    throw new Exception("xml文件名不能为空！");
                }
                string xmlPathAndFileName = path + xmlFileName + ".xml";
                ds.WriteXml(xmlPathAndFileName, XmlWriteMode.WriteSchema);//加上XmlWriteMode.WriteSchema
                return 1;
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "写XML异常", ex);
                return 0;
            }
        }

        /// <summary>
        /// 从指定文件名（包含路径）读取xml文件并转换成DataTable，有数据则返回DataTable，无数据则返回null
        /// </summary>
        /// <param name="xmlPathAndFileName">包含路径的xml文件名(最好以数据表名来命名文件)</param>
        /// <returns></returns>
        public static DataTable ReadDataTableFromXMLFile(string xmlPathAndFileName)
        {
            DataSet ds = new DataSet();
            FileInfo fileInfo = new FileInfo(xmlPathAndFileName);
            if (fileInfo.Exists)
            {
                ds.ReadXml(xmlPathAndFileName, XmlReadMode.ReadSchema);//
            }
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 特定功能

        /// <summary>
        /// 获取可售商品信息的空表架构
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSaleCargoNewsTableSchema()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("NewsID", typeof(string));//消息ID
            dt.Columns.Add("NewsCreateDate", typeof(DateTime));//消息发布时间
            dt.Columns.Add("NewsTitle", typeof(string));//消息标题
            dt.Columns.Add("NewsContent", typeof(string));//消息内容
            dt.Columns.Add("EditorID", typeof(string));//消息发布人ID
            dt.Columns.Add("EditorName", typeof(string));//消息发布人姓名

            return dt;
        }

        //读取可售商品消息记录
        public static DataTable ReadSaleCargoNews()
        {
            string totalPath = path + "SaleCargoNews.xml";
            return ReadDataTableFromXMLFile(totalPath);
        }

        public static bool SaveSaleCargoNewsToXml(DataTable dt)
        {
            bool isExist = IsFileExist("SaleCargoNews");
            if (isExist == false)
            {
                string totalPath = path + "SaleCargoNews.xml";
                CreateFile(totalPath);
            }
            int ret = WriteDataTableToXMLFile(dt, "SaleCargoNews");
            return ret == 1 ? true : false;
        }

        #endregion
    }


}
