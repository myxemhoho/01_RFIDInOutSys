using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Gold.DAL
{
    public class CommonConvert
    {
        /// <summary>
        /// 由出入库单类型解释获取类型代码
        /// </summary>       
        public static string GetSIOTypeCode(string sIOType)
        {
            switch (sIOType)
            {        
                //出库相关
                case "销售出库单":
                    return "11";
                case "调拨出库单":
                    return "12";
                case "其他出库单":
                    return "13";
                //出库相关
                case "采购入库单":
                    return "21";
                case "调拨入库单":
                    return "22";
                case "其他入库单":
                    return "23";
                case "转库入库单":
                    return "24";
            }
            return null;
        }        

        /// <summary>
        /// 由单据来源类型解释获取单据来源类型代码
        /// </summary>       
        public static string GetFromTypeCode(string fromType)
        {
            switch (fromType)
            {
                case "源于订单":
                    return "01";
                case "源于Excel导入的入库单":                  
                case "源于Excel导入的出库单":
                    return "02";
                case "源于手工新增的入库单":                   
                case "源于手工新增的出库单":
                    return "03"; 
            }
            return null;
        }
                
        /// <summary>
        /// 由出入库单类型解释转换为出入库单类型代码
        /// </summary>      
        public static string GetBillTypeCode(string billType)
        {
            //01-出库单，02-入库单，03-撤销入库单（即出库），04-撤销出库单（即入库）
            switch (billType)
            {                    
                case "出库单":
                    return "01";
                case "入库单":
                    return "02";
                case "撤销入库单":
                    return "03";
                case "撤销出库单":
                    return "04";
            }
            return null;
        }

        /// <summary>
        /// 显示网页提示框
        /// </summary>
        /// <param name="control"></param>
        /// <param name="msg"></param>
        public static void ShowMessageBox(Control control,string msg) 
        {
            ScriptManager.RegisterClientScriptBlock(control, control.GetType(), Guid.NewGuid().ToString(), "alert('" + msg + "');", true);
        }
    }
}