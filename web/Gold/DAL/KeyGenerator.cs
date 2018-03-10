using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.DAL
{
    public class KeyGenerator
    {
        /// <summary>
        /// 入库单序列号
        /// </summary>
        private int _stockInKey = 0;
        /// <summary>
        /// 出库单序列号
        /// </summary>
        private int _stockOutKey = 0;

        /// <summary>
        /// 盘点计划单序列号
        /// </summary>
        private int _stockCountPlanKey = 0;

        /// <summary>
        /// 撤销单序列号
        /// </summary>
        private int _stockCancleKey = 0;

        /// <summary>
        /// 重启标识
        /// </summary>
        private bool _restart = true;

        /// <summary>
        /// 日期
        /// </summary>
        private int _day;
                
        private KeyGenerator()
        {         
        }

        /// 返回类的实例
        /// </summary>
        public static KeyGenerator Instance
        {
            get { return SingletonCreator.instance; }
        }

        class SingletonCreator
        {
            internal static readonly KeyGenerator instance = new KeyGenerator();
        }

        /// <summary>
        /// 获取入库单的单据序号
        /// </summary>
        /// <returns></returns>
        public string GetStockInKey()
        {
            lock (this)
            {
                if (_restart)
                {
                    _stockInKey = GetMaxStockInKey();
                    //_restart = false;//在调试阶段，暂时注释掉此语句
                }
                else if (_day != DateTime.Now.Day)
                {
                    _day = DateTime.Now.Day;
                    _stockInKey = 0;
                }
                    
                return string.Format("R{0}{1:d4}", DateTime.Now.ToString("yyMM"), ++_stockInKey);
            }
        }

        /// <summary>
        /// 获取出库单的单据序号
        /// </summary>
        /// <returns></returns>
        public string GetStockOutKey()
        {
            lock (this)
            {
                if (_restart)
                {
                    _stockOutKey = GetMaxStockOutKey();
                    //_restart = false;//在调试阶段，暂时注释掉此语句
                }
                else if (_day != DateTime.Now.Day)
                {
                    _day = DateTime.Now.Day;
                    _stockOutKey = 0;
                }

                return string.Format("C{0}{1:d4}", DateTime.Now.ToString("yyMM"), ++_stockOutKey);
            }
        }

        /// <summary>
        /// 获取盘点计划单完整的单据序号
        /// </summary>
        /// <returns></returns>
        public string GetStockCountingPlanKey()
        {
            lock (this)
            {
                if (_restart)
                {
                    _stockCountPlanKey = GetMaxStockCountPlanKey();                    
                }
                else if (_day != DateTime.Now.Day)
                {
                    _day = DateTime.Now.Day;
                    _stockCountPlanKey = 0;
                }

                return string.Format("PD{0}{1:d4}", DateTime.Now.ToString("yyMM"), ++_stockCountPlanKey);
            }
        }

        /// <summary>
        /// 获取撤销单的单据序号
        /// </summary>
        /// <param name="type">出库或者是入库</param>
        /// <returns></returns>
        public string GetStockCancleKey(string typeInOrOut)
        {
            lock (this)
            {
                if (_restart)
                {
                    _stockCancleKey = GetMaxStockCancleKey(typeInOrOut);                    
                }
                else if (_day != DateTime.Now.Day)
                {
                    _day = DateTime.Now.Day;
                    _stockCancleKey = 0;
                }

                string cancleCode = null;
                if (typeInOrOut == "In")
                {
                    cancleCode = string.Format("XR{0}{1:d4}", DateTime.Now.ToString("yyMM"), ++_stockCancleKey);
                }
                else if (typeInOrOut == "Out")
                {
                    cancleCode = string.Format("XC{0}{1:d4}", DateTime.Now.ToString("yyMM"), ++_stockCancleKey);
                }

                return cancleCode;
            }
        }

        private int GetMaxStockInKey()
        {
            int ret = 0;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockIn
                    .Select("it.SICode")
                    .Where("it.SICode like 'R" + DateTime.Now.ToString("yyMM") + "%'")
                    .OrderBy("it.SICode desc")
                    .FirstOrDefault();

                if (tmp != null )
                {
                    string tmp2 = tmp[0].ToString().Substring(5);
                    if (false == int.TryParse(tmp2, out ret))
                        ret = 1000;
                }

                return ret;
            }
        }

        private int GetMaxStockOutKey()
        {
            int ret = 0;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockOut
                    .Select("it.SOCode")
                    .Where("it.SOCode like 'C" + DateTime.Now.ToString("yyMM") + "%'")
                    .OrderBy("it.SOCode desc")
                    .FirstOrDefault();

                if (tmp != null)
                {
                    string tmp2 = tmp[0].ToString().Substring(5);
                    if (false == int.TryParse(tmp2, out ret))
                        ret = 1000;
                }

                return ret;
            }
        }

        /// <summary>
        /// 获取盘点计划单最大的序列号(末四位自增的序号)
        /// </summary>
        /// <returns></returns>
        private int GetMaxStockCountPlanKey()
        {
            int ret = 0;
            using (var edm = new Gold.DAL.GoldEntities())
            {
                var tmp = edm.StockCountingPlan
                    .Select("it.SCPCode")
                    .Where("it.SCPCode like 'PD" + DateTime.Now.ToString("yyMM") + "%'")
                    .OrderBy("it.SCPCode desc")
                    .FirstOrDefault();

                if (tmp != null)
                {
                    string tmp2 = tmp[0].ToString().Substring(6);
                    if (false == int.TryParse(tmp2, out ret))
                        ret = 1000;
                }

                return ret;
            }
        }

        /// <summary>
        /// 获取撤销单最大的序列号（末四位自增的序号）
        /// </summary>
        /// <returns></returns>
        private int GetMaxStockCancleKey(string typeInOrOut)
        {
            int ret = 0;
            using (var edm = new Gold.DAL.GoldEntities())
            {                
                if (typeInOrOut == "In")
                {
                    var tmp = edm.StockCancel
                        .Select("it.SCCode")
                        .Where("it.SCCode like 'XR" + DateTime.Now.ToString("yyMM") + "%'")
                        .OrderBy("it.SCCode desc")
                        .FirstOrDefault();
                    if (tmp != null)
                    {
                        string tmp2 = tmp[0].ToString().Substring(6);
                        if (false == int.TryParse(tmp2, out ret))
                            ret = 1000;
                    }
                }
                else if (typeInOrOut == "Out")
                {
                   var tmp = edm.StockCancel
                        .Select("it.SCCode")
                        .Where("it.SCCode like 'XC" + DateTime.Now.ToString("yyMM") + "%'")
                        .OrderBy("it.SCCode desc")
                        .FirstOrDefault();
                   if (tmp != null)
                   {
                       string tmp2 = tmp[0].ToString().Substring(6);
                       if (false == int.TryParse(tmp2, out ret))
                           ret = 1000;
                   }
                }                

                return ret;
            }
        }
    }
}