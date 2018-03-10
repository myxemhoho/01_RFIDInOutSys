using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;
using System.IO;

namespace Gold.Utility
{
    public class XmlHelper
    {
        /// <summary>
        /// 解析用友接口返回的XML数据时使用的默认编码格式
        /// </summary>
        public static XmlEncodingType DefaultXmlEncodingType = XmlEncodingType.GB2312;

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Delete(path, "/Node", "")
         * XmlHelper.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
            }
            catch { }
        }


        /// <summary>
        /// 读取web.sitemap中一级标题数据
        /// </summary>
        /// <param name="path">web.sitemap的文件路径</param>        
        /// <param name="list">查询出的SiteMap的一级标题数据</param>
        /// <returns></returns>
        public static bool ReadFirstLevelNodeList(string path, out List<SiteMapModel> list)
        {
            XmlNodeList nodeList = null;
            list = new List<SiteMapModel>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);


                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ab", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");//sitemap文件中的名称空间
                //string xpath="//ab:siteMapNode[@url=\"\"]";//node="siteMapNode";可选出

                XmlNode rootSiteMap = doc.GetElementsByTagName("siteMap")[0];//选择根节点
                nodeList = rootSiteMap.SelectNodes("ab:siteMapNode/ab:siteMapNode", nsmgr);//选择一级元素


                if (nodeList != null)
                {
                    foreach (XmlNode xn in nodeList)
                    {
                        SiteMapModel newObj = new SiteMapModel();
                        newObj.Title = xn.Attributes["title"].Value;
                        newObj.Url = xn.Attributes["url"].Value;
                        newObj.Description = xn.Attributes["description"].Value;
                        newObj.Roles = xn.Attributes["roles"].Value;

                        if (newObj.Title == "首页")//首页权限在web.sitemap中写定，不可修改
                            continue;

                        list.Add(newObj);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "读web.sitemap", ex);
                return false;
            }
        }

        /// <summary>
        /// 读取web.sitemap中一级标题数据
        /// </summary>
        /// <param name="path">web.sitemap的文件路径</param>        
        /// <param name="list">查询出的SiteMap的一级标题数据</param>
        /// <returns></returns>
        public static bool WriteFirstLevelNodeRoleList(string path, List<SiteMapModel> list)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ab", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");//sitemap文件中的名称空间
                //string xpath="//ab:siteMapNode[@url=\"\"]";//node="siteMapNode";可选出

                XmlNode rootSiteMap = doc.GetElementsByTagName("siteMap")[0];//选择根节点
                //nodeList = rootSiteMap.SelectNodes("ab:siteMapNode/ab:siteMapNode", nsmgr);//选择一级元素

                foreach (SiteMapModel temp in list)
                {
                    XmlNode xn = rootSiteMap.SelectSingleNode("ab:siteMapNode/ab:siteMapNode[@title='" + temp.Title + "']", nsmgr);//选择一级元素
                    XmlElement xe = (XmlElement)xn;
                    string oldRoles= xe.Attributes["roles"].Value;
                    System.Text.StringBuilder newRoles = new System.Text.StringBuilder(oldRoles);
                    string[] AddRoleNameList = temp.Roles.Split(',');
                    if (AddRoleNameList != null) 
                    {
                        foreach (string s in AddRoleNameList) 
                        {
                            if (string.IsNullOrEmpty(s) == false && oldRoles.Contains(s) == false) 
                            {
                                if (newRoles.Length > 0)
                                    newRoles.Append(",");
                                newRoles.Append(s);//增加角色
                            }
                        }
                    }

                    xe.SetAttribute("roles", newRoles.ToString());
                }

                doc.Save(path);
                
                return true;
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "写web.sitemap", ex);
                return false;
            }
        }


        /// <summary>
        /// 读取用友系统返回的XML字符串中的记录行数和错误信息值
        /// </summary>
        /// <param name="xmlString">用友系统返回的XML字符串</param>
        /// <param name="total">用友XML字符串中携带的行数信息</param>
        /// <param name="errorMsg">用友XML字符串中携带的错误信息（正常时为空）</param>
        /// <returns></returns>
        public static bool GetXmlStringTotalAndErrorMsg(string xmlString, out string total,out string errorMsg)
        {
            total = "";
            errorMsg = "";
            try
            {
                StringReader strReader  = new StringReader(xmlString);
                XmlDocument doc = new XmlDocument();
                doc.Load(strReader);

                XmlNodeList nodeList = doc.GetElementsByTagName("Ret");
                if (nodeList != null && nodeList.Count != 0)
                {
                    XmlNode rootSiteMap = nodeList[0];//选择根节点
                    XmlElement xe = (XmlElement)rootSiteMap;
                    total = xe.Attributes["Total"].Value;
                    errorMsg = xe.Attributes["Error"].Value;
                }
                else 
                {
                    total = "0";
                }
                strReader.Close();
                strReader.Dispose();
                
                return true;
            }
            catch (Exception ex)
            {
                total = "0";
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetXmlStringTotalAndError", ex);
                return false;
            }
        }

        /// <summary>
        /// 将utf-8字符转为gb2312字符
        /// </summary>
        /// <param name="utfString"></param>
        /// <returns></returns>
        public static string GetGBStringFromUTFString(string utfString) 
        {
            try
            {                
                //utf-8转为国标字符
                Encoding utf8 = Encoding.GetEncoding("utf-8");//(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");//Encoding.Default ,936
                byte[] temp = utf8.GetBytes(utfString);
                byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
                string xml = gb2312.GetString(temp1);

                return xml;
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetGBStringFromUTFString", ex);
                return utfString;
            }
        }

        /// <summary>
        /// 将gb2312编码字符转为utf-8字符
        /// </summary>
        /// <param name="gbString"></param>
        /// <returns></returns>
        public static string GetUtfStringFromGBString(string gbString) 
        {
           
            try
            {
                Encoding uft8 = Encoding.GetEncoding("utf-8");//(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] temp = gb2312.GetBytes(gbString);                  
                byte[] temp1 = Encoding.Convert(gb2312, uft8, temp);                              
                string result = uft8.GetString(temp1);
                return result;
            }
            catch (Exception ex)//(UnsupportedEncodingException ex)
            {   
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error,"GetUtfStringFromGBString",ex);
                return gbString;
            }
        
        }

        /// <summary>
        /// 将gb2312编码字符转为系统默认编码字符
        /// </summary>
        /// <param name="gbString"></param>
        /// <returns></returns>
        public static string GetDefaultStringFromGBString(string gbString) 
        {
            try
            {
                Encoding encodeDefault = Encoding.Default;//.GetEncoding("utf-8");//(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] temp = gb2312.GetBytes(gbString);
                byte[] temp1 = Encoding.Convert(gb2312, encodeDefault, temp);
                string result = encodeDefault.GetString(temp1);
                return result;
            }
            catch (Exception ex)//(UnsupportedEncodingException ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetDefaultStringFromGBString", ex);
                return gbString;
            }
        }

        /// <summary>
        /// 将utf-8字符转为系统默认编码字符
        /// </summary>
        /// <param name="utfString"></param>
        /// <returns></returns>
        public static string GetDefaultStringFromUTFString(string utfString)
        {
            try
            {
                //utf-8转为国标字符
                Encoding utf8 = Encoding.GetEncoding("utf-8");//(65001);
                Encoding encodeDefault = Encoding.Default;//.GetEncoding("gb2312");//Encoding.Default ,936
                byte[] temp = utf8.GetBytes(utfString);
                byte[] temp1 = Encoding.Convert(utf8, encodeDefault, temp);
                string xml = encodeDefault.GetString(temp1);

                return xml;
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetDefaultStringFromUTFString", ex);
                return utfString;
            }
        }

        /// <summary>
        /// 按系统设置的默认编码格式解析用友返回的xml数据，当此方法解析失败时返回原始数据（系统设置的默认编码格式是XmlHelper中的DefaultXmlEncodingType）
        /// </summary>
        /// <param name="initialXMLString"></param>
        /// <returns></returns>
        public static string GetStringByDefaultEncodingType(string initialXMLString) 
        {
            string xml = initialXMLString;
            try
            {                
                if (DefaultXmlEncodingType == XmlEncodingType.UTF8)
                {
                    xml = GetGBStringFromUTFString(initialXMLString);
                }
                else if (DefaultXmlEncodingType == XmlEncodingType.GB2312)
                {
                    xml = initialXMLString;
                }

                return xml;
            }
            catch (Exception ex) 
            {
                LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetStringByDefaultEncodingType", ex);
                return xml;
            }

        }
    }

    

    /// <summary>
    /// 用友接口返回XML数据编码格式
    /// </summary>
    public enum XmlEncodingType 
    {
        /// <summary>
        /// 用友接口返回XML数据编码格式为utf-8
        /// </summary>
        UTF8=0,

        /// <summary>
        /// 用友接口返回XML数据编码格式为gb2312
        /// </summary>
        GB2312=1
    }

    public class SiteMapModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Roles { get; set; }
        public string Description { get; set; }
    }

    /*
     
     ==================================================

XmlFile.xml：
<?xml version="1.0" encoding="utf-8"?>
<Root />

==================================================

使用方法：

string xml = Server.MapPath("XmlFile.xml");
//插入元素
//XmlHelper.Insert(xml, "/Root", "Studio", "", "");
//插入元素/属性
//XmlHelper.Insert(xml, "/Root/Studio", "Site", "Name", "小路工作室");
//XmlHelper.Insert(xml, "/Root/Studio", "Site", "Name", "丁香鱼工作室");
//XmlHelper.Insert(xml, "/Root/Studio", "Site", "Name", "五月软件");
//XmlHelper.Insert(xml, "/Root/Studio/Site[@Name='五月软件]", "Master", "", "五月");
//插入属性
//XmlHelper.Insert(xml, "/Root/Studio/Site[@Name='小路工作室']", "", "Url", "http://www.wzlu.com/");
//XmlHelper.Insert(xml, "/Root/Studio/Site[@Name='丁香鱼工作室']", "", "Url", "http://www.luckfish.net/");
//XmlHelper.Insert(xml, "/Root/Studio/Site[@Name='五月软件]", "", "Url", "http://www.vs2005.com.cn/");
//修改元素值
//XmlHelper.Update(xml, "/Root/Studio/Site[@Name='五月软件]/Master", "", "Wuyue");
//修改属性值
//XmlHelper.Update(xml, "/Root/Studio/Site[@Name='五月软件]", "Url", "http://www.vs2005.com.cn/");
//XmlHelper.Update(xml, "/Root/Studio/Site[@Name='五月软件]", "Name", "MaySoft");
//读取元素值
//Response.Write("<div>" + XmlHelper.Read(xml, "/Root/Studio/Site/Master", "") + "</div>");
//读取属性值
//Response.Write("<div>" + XmlHelper.Read(xml, "/Root/Studio/Site", "Url") + "</div>");
//读取特定属性值
//Response.Write("<div>" + XmlHelper.Read(xml, "/Root/Studio/Site[@Name='丁香鱼工作室']", "Url") + "</div>");
//删除属性
//XmlHelper.Delete(xml, "/Root/Studio/Site[@Name='小路工作室']", "Url");
//删除元素
//XmlHelper.Delete(xml, "/Root/Studio", "");
     
     
     
     */

}