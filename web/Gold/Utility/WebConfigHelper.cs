using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;

namespace Gold.Utility
{
    /// <summary>
    /// 读取或设置WebConfig中相关值
    /// </summary>
    public class WebConfigHelper
    {
        #region 单件模式-只能创建一个WebConfigHelperInstance

        static WebConfigHelper WebConfigHelperInstance;
        private WebConfigHelper() //私有构造函数，防止外部初始化
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        /// <returns></returns>
        public static WebConfigHelper CreateWebConfigHelperInstance()
        {
            if (WebConfigHelperInstance == null)
            {
                WebConfigHelperInstance = new WebConfigHelper();
                return WebConfigHelperInstance;
            }
            else
            {
                return WebConfigHelperInstance;
            }
        }

        /// <summary>
        /// 静态实例（作用同CreateWebConfigHelperInstance()）
        /// </summary>
        public static WebConfigHelper Instance
        {
            get { return CreateWebConfigHelperInstance(); }
        }

        #endregion


        /// <summary>
        /// 根据键名查找键值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns>成功返回配置值，失败返回""</returns>
        public string GetAppSettingValue(string keyName)
        {
            string retValue = "";
            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName) == false)
            {
                return "";
            }
            retValue = ConfigurationManager.AppSettings[keyName].ToString();

            return retValue;
        }

        /// <summary>
        /// 获取GridView默认分页记录大小
        /// </summary>
        /// <returns>成功返回配置值，失败返回默认值15</returns>
        public int GetDefaultPageSize()
        {
            int retInt = 15;
            string keyName = "DefaultPageSize";
            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName) == false)
            {
                return retInt;//如果WebConfig中未设置此参数则设置分页大小为15
            }
            string retValue = ConfigurationManager.AppSettings[keyName].ToString();

            if (int.TryParse(retValue, out retInt) == false)//若成功转换为整型则使用配置值，否则设置值为15
                retInt = 15;

            return retInt;
        }

        /// <summary>
        /// 获取AppSetting中Int类型的Value
        /// </summary>
        /// <returns>成功则返回配置值，失败则返回-1</returns>
        public int GetAppSettingIntValue(string keyName)
        {
            int retInt = -1;

            if (ConfigurationManager.AppSettings.AllKeys.Contains(keyName) == false)
            {
                return retInt;//如果WebConfig中未设置此参数则返回-1
            }
            string retValue = ConfigurationManager.AppSettings[keyName].ToString();

            if (int.TryParse(retValue, out retInt) == false)//若成功转换为整型则使用配置值，否则设置值为-1
                retInt = -1;

            return retInt;
        }

        /// <summary>
        /// 获取标签亮灯次数
        /// </summary>
        /// <returns>成功则返回配置值，失败则返回默认值3</returns>
        public int GetBinTagLightAlartCount()
        {
            int ret = GetAppSettingIntValue("BinTagLightAlartCount");
            if (ret == -1)
                return 3;
            else
                return ret;
        }



        /// <summary>
        /// 获取标签鸣笛次数
        /// </summary>
        /// <returns>成功则返回配置值，失败则返回默认值3</returns>
        public int GetBinTagSoundAlartCount()
        {
            int ret = GetAppSettingIntValue("BinTagSoundAlartCount");
            if (ret == -1)
                return 3;
            else
                return ret;
        }
    }
}