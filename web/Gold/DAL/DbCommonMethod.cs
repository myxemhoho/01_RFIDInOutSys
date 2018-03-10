using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.DAL
{
    public class DbCommonMethod
    {
        /// <summary>
        /// 由仓库名称找到仓库编码，若不存在，则返回空串
        /// </summary>
        /// <param name="whName">仓库名称</param>
        /// <returns>仓库编码</returns>
        public static string ParsingWarehouse(string whName)
        {
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.WareHouse.SingleOrDefault(o => o.WHName == whName);
                if (tmp == null)
                    return "";
                else
                    return tmp.WHCode;
            }
        }

        //跟据商品编码查询商品表是否已存在，若不存在，则返回空串
        public static string ParsingCargoCode(string cargoCode)
        {
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.Cargos.SingleOrDefault(o => o.CargoCode == cargoCode);
                if (tmp == null)
                {
                    return "";
                }
                else
                {
                    return tmp.CargoCode;
                }
            }
        }

        public static string GetStockInCode(string fromBillNo)
        {
            string ret = "";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockIn
                    .Select("it.SICode, it.FromBillNo")
                    .Where("it.FromBillNo == '" + fromBillNo + "' ")
                    .FirstOrDefault();

                if (tmp != null)
                    ret = tmp[0].ToString();
                else
                    ret = KeyGenerator.Instance.GetStockInKey();

                return ret;
            }
        }

        public static string GetStockOutCode(string fromBillNo)
        {
            string ret = "";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockOut
                    .Select("it.SOCode, it.FromBillNo")
                    .Where("it.FromBillNo == '" + fromBillNo + "' ")
                    .FirstOrDefault();

                if (tmp != null)
                    ret = tmp[0].ToString();
                else
                    ret = KeyGenerator.Instance.GetStockOutKey();

                return ret;
            }
        }


        //获取收发类别（发）的code
        public static string GetOutCategory(string categoryName)
        {
            string ret = "";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.SingleOrDefault(o => (o.Category == "OutCategory") && (o.Name == categoryName));

                if (tmp != null)
                {
                    ret = tmp.Code;
                }
                return ret;
            }
        }


        //获取收发类别（收）的code
        public static string GetInCategory(string categoryName)
        {
            string ret = "";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.SingleOrDefault(o => (o.Category == "InCategory") && (o.Name == categoryName));
            }





            return ret;
        }


        /// <summary>
        /// 获取标签首次亮灯次数
        /// </summary>
        /// <returns></returns>
        public static int GetTagFirstLightCount()
        {
            int ret = 50;//
            string categoryName = "层标首次亮灯次数";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.SingleOrDefault(o => (o.Category == "TagLightSound") && (o.Name == categoryName));


                if (tmp != null)
                {
                    if (int.TryParse(tmp.Code, out ret) == false)
                        ret = 50;
                }
                return ret;
            }
        }

        /// <summary>
        /// 获取标签首次鸣音次数
        /// </summary>
        /// <returns></returns>
        public static int GetTagFirstSoundCount()
        {
            int ret = 50;//
            string categoryName = "层标首次鸣音次数";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.SingleOrDefault(o => (o.Category == "TagLightSound") && (o.Name == categoryName));

                if (tmp != null)
                {
                    if (int.TryParse(tmp.Code, out ret) == false)
                        ret = 50;
                }
                return ret;
            }
        }

        /// <summary>
        /// 获取标签间隔鸣音次数
        /// </summary>
        /// <returns></returns>
        public static int GetTagIntervalSoundCount()
        {
            int ret = 50;//
            string categoryName = "层标间隔鸣音次数";
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.DataDict.SingleOrDefault(o => (o.Category == "TagLightSound") && (o.Name == categoryName));

                if (tmp != null)
                {
                    if (int.TryParse(tmp.Code, out ret) == false)
                        ret = 50;
                }
                return ret;
            }
        }
    }

}