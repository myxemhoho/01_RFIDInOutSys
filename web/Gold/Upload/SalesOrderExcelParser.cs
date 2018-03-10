using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Data.Objects.DataClasses;
using Gold.DAL;


namespace Gold.Upload
{
    /// <summary>
    /// 销售订单Excel文件解析
    /// </summary>
    public class SalesOrderExcelParser : ExcelParser
    {
        private SalesOrder _order = new SalesOrder();

        public SalesOrder Order
        {
            get
            {
                return _order;
            }
        }

        public override string Summary
        {
            get
            {
                return string.Format("销售订单号：{0}", _order.NCOrderCode);
            }
        }


        public SalesOrderExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.NCOrderCode))
                throw new ApplicationException("文件格式错误！不能解析为销售订单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (!(title == "`销售单" || title == "销售订单" || title == "销售单"))//(title != "`销售单" && title != "销售订单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "订单编号":
                    _order.NCOrderCode = fieldValue;
                    _order.OrderCode = fieldValue;
                    break;
                case "销售日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.SalesDate = dt;
                    }
                    break;
                case "客户名称":
                    _order.ClientName = fieldValue;
                    break;
                case "会员卡号":
                    _order.MemberCard = fieldValue;
                    break;
                case "业务类型":
                    _order.BusinessType = fieldValue;
                    break;
                case "备注":
                    _order.Comment = fieldValue;
                    break;
                case "收货地址":
                    _order.DeliveryAddr = fieldValue;
                    break;
                case "联系人":
                    _order.ContactPerson = fieldValue;
                    break;
                case "联系电话":
                    _order.ContactPhoneNo = fieldValue;
                    break;
                case "销售部门":
                    _order.SellDepartmentName = fieldValue;
                    break;
                case "业务员":
                    _order.Operator = fieldValue;
                    break;
                case "领导":
                    _order.LeaderSign = fieldValue;
                    break;
                case "仓库":
                    _order.WarehouseSign = fieldValue;
                    break;
                case "财务":
                    _order.AccountSign = fieldValue;
                    break;
                case "业务":
                    _order.BusinessSign = fieldValue;
                    break;
                case "制单人":
                    _order.Preparer = fieldValue;
                    break;
                default:
                    break;
            }
        }

        protected override void BindingOrderItem(object[] array)
        {
            if (_order.SalesOrderDetail == null)
                _order.SalesOrderDetail = new EntityCollection<SalesOrderDetail>();

            if (_itemDefine == null)
                throw new ApplicationException("未发现行项目字段定义!");

            if (_itemDefine.Count() == array.Count())
            {
                SalesOrderDetail orderItem = new SalesOrderDetail();
                for (int i = 0; i < array.Count(); i++)
                {
                    string fieldName = Regex.Replace(_itemDefine[i].ToString(), @"\s", "");
                    string fieldValue = Regex.Replace(array[i].ToString(), @"\s", "");

                    switch (fieldName)
                    {
                        case "商品编码":
                            orderItem.CargoCode = fieldValue;
                            string cargoCode = DbCommonMethod.ParsingCargoCode(fieldValue);
                            if (string.IsNullOrEmpty(cargoCode))
                                throw new ApplicationException("商品不存在：" + fieldValue);
                            break;                           
                        case "商品名称":
                            orderItem.CargoName = fieldValue;
                            break;
                        case "规格":
                            orderItem.CargoSpec = fieldValue;
                            break;
                        case "型号":
                            //过滤型号值，如果型号是90.000，则去90，去掉小数点后面的0
                            double dou;
                            string strModel = fieldValue;
                            double douModel = 0;
                            if (double.TryParse(fieldValue, out dou))
                                douModel = dou;

                            if (douModel != 0 && strModel.Contains("."))
                            {
                                string sdecimal = strModel.Substring(strModel.IndexOf(".") + 1, strModel.Length - strModel.IndexOf(".") - 1);
                                int istring=Convert.ToInt32(sdecimal);
                                if (istring==0)
                                {
                                    strModel = fieldValue.Substring(0, fieldValue.IndexOf("."));
                                }                                
                            }
                            orderItem.CargoModel = strModel;
                            break;
                        case "单位":
                            orderItem.CargoUnits = fieldValue;
                            break;
                        case "数量":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                double d;
                                if (double.TryParse(fieldValue, out d))
                                    orderItem.NumOriginalPlan = d;
                            }
                            break;
                        case "单价":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.Price = d;
                            }
                            break;
                        case "金额":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.TotalMoney = d;
                            }
                            break;
                        case "折后金额":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.DiscountMoney = d;
                            }
                            break;
                        case "发出仓库":
                            orderItem.WHName = fieldValue;
                            orderItem.WHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                            if (string.IsNullOrEmpty(orderItem.WHCode))
                                throw new ApplicationException("仓库不存在：" + fieldValue);
                            break;
                        default:
                            break;

                    }
                }
                if (string.IsNullOrEmpty(orderItem.DetailRowNumber))
                    orderItem.DetailRowNumber = _rowNum.ToString();

                orderItem.DetailRowStatus = 2;//初始态
                orderItem.OrderCode = _order.OrderCode;
                orderItem.NCOrderCode = _order.NCOrderCode;
                orderItem.SalesOrder = _order;
                IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                    new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("OrderCode", orderItem.OrderCode),
                        new KeyValuePair<string, object>("DetailRowNumber", orderItem.DetailRowNumber)
                    };
                orderItem.EntityKey = new EntityKey("GoldEntities.SalesOrderDetail", entityKeyValues);
                _order.SalesOrderDetail.Add(orderItem);
            }
        }

        public override string SaveData()
        {
            Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "进入SaveData");

            _order.OrderStatus = 2;//初始态
            _order.EditStatus = 0;//无人编辑

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.SalesOrder.SingleOrDefault(o => o.OrderCode == _order.OrderCode);
                    if (tmp == null)
                    {
                        edm.SalesOrder.AddObject(_order);
                    }
                    else
                    {
                        //判断该订单是否已经开始其他作业,如果是，则不允许覆盖
                        if (tmp.OrderStatus != 2)
                        {
                            throw new Exception("此订单(" + tmp.OrderCode + ")已开始其他作业，不允许再次上传！");
                        }

                        Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "数据库已存在记录1");
                        foreach (SalesOrderDetail oldDetail in tmp.SalesOrderDetail)
                        {
                            //如果实收数量为0，则直接删除
                            if (oldDetail.NumActual == null || oldDetail.NumActual == 0)
                            {
                                Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "实收数量为0，则删除");
                                continue;
                            }
                            else
                            {
                                bool isExist = false;//是否新旧订单都有此商品
                                foreach (SalesOrderDetail newDetail in _order.SalesOrderDetail)
                                {
                                    //判断在当前的新excel中是否有此商品,如果有，则保存实收数量
                                    if (newDetail.CargoCode == oldDetail.CargoCode)
                                    {
                                        Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "如果已有此商品且数量不为0，则保留实收数量");
                                        newDetail.NumActual = oldDetail.NumActual;
                                        isExist = true;
                                        _order.OrderStatus = 1;//部分已下发
                                        break;
                                    }
                                }
                                if (isExist)
                                {
                                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "如果isExist为true，则continue");
                                    continue;
                                }
                                else
                                {
                                    //如果当前新excel没有此商品，则直接添加
                                    SalesOrderDetail sd = new SalesOrderDetail();
                                    sd.CargoCode = oldDetail.CargoCode;
                                    sd.CargoModel = oldDetail.CargoModel;
                                    sd.CargoName = oldDetail.CargoName;
                                    sd.CargoSpec = oldDetail.CargoSpec;
                                    sd.CargoUnits = oldDetail.CargoUnits;
                                    sd.DetailRowNumber = oldDetail.DetailRowNumber;
                                    sd.DetailRowStatus = oldDetail.DetailRowStatus;
                                    sd.DiscountMoney = oldDetail.DiscountMoney;
                                    sd.NCOrderCode = oldDetail.NCOrderCode;
                                    sd.NumActual = oldDetail.NumActual;
                                    sd.NumCurrentPlan = oldDetail.NumCurrentPlan;
                                    sd.NumOriginalPlan = oldDetail.NumOriginalPlan;
                                    sd.OrderCode = oldDetail.OrderCode;
                                    sd.Price = oldDetail.Price;
                                    sd.ReleaseYear = oldDetail.ReleaseYear;
                                    sd.Reserve1 = oldDetail.Reserve1;
                                    sd.Reserve2 = oldDetail.Reserve2;
                                    sd.TotalMoney = oldDetail.TotalMoney;
                                    sd.WHCode = oldDetail.WHCode;
                                    sd.WHName = oldDetail.WHName;
                                    if (sd.NumActual != null && sd.NumActual != 0)
                                    {
                                        _order.OrderStatus = 1;//部分已下发
                                    }

                                    _order.SalesOrderDetail.Add(sd);
                                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "如果当前新excel没有此商品，则直接添加（包括实收数量）");
                                }
                            }
                        }

                        edm.SalesOrder.DeleteObject(tmp);
                        edm.SaveChanges();
                        edm.SalesOrder.AddObject(_order);
                    }

                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：");
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "销售订单保存异常", ex);
                }
            }

            Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Info, "离开SaveData");

            return sb.ToString();
        }
    }

}
