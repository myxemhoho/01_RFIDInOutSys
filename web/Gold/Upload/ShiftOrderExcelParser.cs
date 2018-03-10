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
    /// 转库订单Excel文件解析
    /// </summary>
    public class ShiftOrderExcelParser : ExcelParser
    {
        private ShiftOrder _order = new ShiftOrder();

        public ShiftOrder Order
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
                return string.Format("转库订单号：{0}", _order.NCOrderCode);
            }
        }

        public ShiftOrderExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.NCOrderCode))
                throw new ApplicationException("文件格式错误！不能解析为转库单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (title != "转库单" && title != "转库订单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "转库单号":
                    _order.NCOrderCode = fieldValue;
                    _order.OrderCode = fieldValue;
                    break;
                case "日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.OrderDate = dt;
                    }
                    break;
                case "摘要":
                    _order.Comment = fieldValue;
                    break;
                case "调出仓库":
                    _order.OutWHName = fieldValue;
                    _order.OutWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    //string whCode=DbCommonMethod.ParsingWarehouse(fieldValue);
                    //if (whCode != string.Empty && whCode != null)
                    //{
                    //    _order.OutWHCode = whCode;
                    //}
                    if (string.IsNullOrEmpty(_order.OutWHCode))
                        throw new ApplicationException("调出仓库不存在：" + fieldValue);                    
                    break;
                case "调入仓库":
                    _order.InWHName = fieldValue;
                    _order.InWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    //string whInCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    //if (whInCode != string.Empty && whInCode != null)
                    //{
                    //    _order.InWHCode = whInCode;
                    //}
                    if (string.IsNullOrEmpty(_order.InWHCode))
                        throw new ApplicationException("调入仓库不存在：" + fieldValue);
                    break;
                case "领导":
                    _order.LeaderName = fieldValue;
                    break;
                case "发货人":
                    _order.SenderName = fieldValue;
                    break;
                case "收货人":
                    _order.ReceiverName = fieldValue;
                    break;
                case "业务经理":
                    _order.BusinessManager = fieldValue;
                    break;
                default:
                    break;
            }
        }

        protected override void BindingOrderItem(object[] array)
        {
            if (_order.ShiftOrderDetail == null)
                _order.ShiftOrderDetail = new EntityCollection<ShiftOrderDetail>();

            if (_itemDefine == null)
                throw new ApplicationException("未发现行项目字段定义!");

            if (_itemDefine.Count() == array.Count())
            {
                ShiftOrderDetail orderItem = new ShiftOrderDetail();
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
                        default:
                            break;

                    }
                }
                if (string.IsNullOrEmpty(orderItem.DetailRowNumber))
                    orderItem.DetailRowNumber = _rowNum.ToString();

                orderItem.DetailRowStatus = 2;//初始态
                orderItem.OrderCode = _order.OrderCode;
                orderItem.NCOrderCode = _order.NCOrderCode;
                orderItem.ShiftOrder = _order;
                IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                    new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("OrderCode", orderItem.OrderCode),
                        new KeyValuePair<string, object>("DetailRowNumber", orderItem.DetailRowNumber)
                    };
                orderItem.EntityKey = new EntityKey("GoldEntities.ShiftOrderDetail", entityKeyValues);
                _order.ShiftOrderDetail.Add(orderItem);
            }
        }

        public override string SaveData()
        {
            _order.OrderStatus = 2;//初始态
            _order.EditStatus = 0;//无人编辑

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.ShiftOrder.SingleOrDefault(o => o.OrderCode == _order.OrderCode);                    
                    if (tmp == null)
                    {
                        edm.ShiftOrder.AddObject(_order);
                    }
                    else
                    {
                        //判断该订单是否已经开始其他作业,如果是，则不允许覆盖
                        if (tmp.OrderStatus != 2)
                        {
                            throw new Exception("此订单(" + tmp.OrderCode + ")已开始其他作业，不允许再次上传！");
                        }

                        foreach (ShiftOrderDetail oldDetail in tmp.ShiftOrderDetail)
                        {
                            //如果实收数量为0，则直接删除
                            if (oldDetail.NumActual == null || oldDetail.NumActual == 0)
                            {
                                continue;
                            }
                            else
                            {
                                bool isExist = false;//是否新旧订单都有此商品
                                foreach (ShiftOrderDetail newDetail in _order.ShiftOrderDetail)
                                {
                                    //判断在当前的新excel中是否有此商品,如果有，则保存实收数量
                                    if (newDetail.CargoCode == oldDetail.CargoCode)
                                    {
                                        newDetail.NumActual = oldDetail.NumActual;
                                        isExist = true;
                                        _order.OrderStatus = 1;//部分已下发
                                        break;
                                    }
                                }
                                if (isExist)
                                {
                                    continue;
                                }
                                else
                                {
                                    //如果当前新excel没有此商品，则直接添加
                                    ShiftOrderDetail sd = new ShiftOrderDetail();
                                    sd.CargoCode = oldDetail.CargoCode;
                                    sd.CargoModel = oldDetail.CargoModel;
                                    sd.CargoName = oldDetail.CargoName;
                                    sd.CargoSpec = oldDetail.CargoSpec;
                                    sd.CargoUnits = oldDetail.CargoUnits;
                                    sd.Comment = oldDetail.Comment;
                                    sd.DetailRowNumber = oldDetail.DetailRowNumber;
                                    sd.DetailRowStatus = oldDetail.DetailRowStatus;
                                    sd.NCOrderCode = oldDetail.NCOrderCode;
                                    sd.NumActual = oldDetail.NumActual;
                                    sd.NumCurrentPlan = oldDetail.NumCurrentPlan;
                                    sd.NumOriginalPlan = oldDetail.NumOriginalPlan;
                                    sd.OrderCode = oldDetail.OrderCode;
                                    sd.ReleaseYear = oldDetail.ReleaseYear;
                                    sd.Reserve1 = oldDetail.Reserve1;
                                    sd.Reserve2 = oldDetail.Reserve2;
                                    if (sd.NumActual != null && sd.NumActual != 0)
                                    {
                                        _order.OrderStatus = 1;//部分已下发
                                    }

                                    _order.ShiftOrderDetail.Add(sd);
                                }
                            }
                        }

                        edm.ShiftOrder.DeleteObject(tmp);
                        edm.SaveChanges();
                        edm.ShiftOrder.AddObject(_order);
                    }

                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：" );
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "转库订单保存异常", ex);
                }
            }

            return sb.ToString();
        }
    }

}
