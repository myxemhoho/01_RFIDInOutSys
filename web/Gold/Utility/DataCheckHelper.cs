using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.Utility
{
    public class DataCheckHelper
    {
        /// <summary>
        /// 获取数据集单元格中数据
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetCellString(object cell)
        {
            if (cell == null)
                return "";
            else
            {
                return cell.ToString().Trim();
            }
        }

        /// <summary>
        /// 检查单元格数据是否能转换成decimal，否则返回0,是则返回转换后的decimal.""转换成0
        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        public static decimal GetCellDecimal(object dataCell)
        {
            decimal result = 0;
            string strResult = GetCellString(dataCell);
            if (strResult == "")
            {
                return result;
            }
            else
            {
                bool IsOk = decimal.TryParse(strResult, out result);
                return result;
            }
        }

        /// <summary>
        /// 检查单元格数据是否能转换成double，否则返回0,是则返回转换后的double.""转换成0
        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        public static double GetCellDouble(object dataCell)
        {
            double result = 0;
            string strResult = GetCellString(dataCell);
            if (strResult == "")
            {
                return result;
            }
            else
            {
                bool IsOk = double.TryParse(strResult, out result);
                return result;
            }
        }
    }
}