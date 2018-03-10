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
    public class TransferOrderExcelParser : ExcelParser
    {
        private TransferOrder _order = new TransferOrder();

        public TransferOrder Order
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
                return string.Format("调拨订单号：{0}", _order.OrderCode);
            }
        }

        public TransferOrderExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.OrderCode))
                throw new ApplicationException("文件格式错误！不能解析为调拨订单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (title != "维护订单" && title != "调拨订单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        //抬头
        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "单据号":
                    _order.OrderCode = fieldValue;
                    _order.NCOrderCode = fieldValue;
                    break;
                //单据类型，因数据库缺少字段
                case "单据日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.OrderDate = dt;
                    }
                    break;
                //case "单据状态":
                //    _order.OrderStatus = fieldValue == "关闭" ? 0 : 1;
                //    break;
                case "调入仓库":
                    _order.InWHName = fieldValue;
                    _order.InWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    if (string.IsNullOrEmpty(_order.InWHCode))
                        throw new ApplicationException("仓库不存在：" + fieldValue);
                    break;
                case "调入组织":
                    _order.InOrganization = fieldValue;
                    break;
                case "调入公司":
                    _order.InCompany = fieldValue;
                    break;
                //case "是否退回":
                case "调出组织":
                    _order.OutOrganization = fieldValue;
                    break;
                case "调出公司":
                    _order.OutCompany = fieldValue;
                    break;
                case "调出仓库":
                    _order.OutWHName = fieldValue;
                    _order.OutWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    if (string.IsNullOrEmpty(_order.OutWHCode))
                        throw new ApplicationException("仓库不存在：" + fieldValue);
                    break;
                //case"出货仓库":
                //case "出货组织":
                //case "出货公司":
                //case "调拨类型":
                //case "币种":
                //case "调入调出在途归属":
                //case "调出出货在途归属":
                //case "H-UDC1":
                //case "H-UDC2":
                //case "H-UDC3":
                //case "H-UDC4":
                //case "H-UDC5":
                //case "H-UDC6":
                //case "H-UDC7":
                //case "H-UDC8":
                //case "H-UDC9":
                //case "H-UDC10":
                //case "H-UDC11":
                //case "H-UDC12":
                //case "H-UDC13":
                //case "H-UDC14":
                //case "H-UDC15":
                //case "H-UDC16":
                //case "H-UDC17":
                //case "H-UDC18":
                //case "H-UDC19":
                //case "H-UDC20":
                case "备注":
                    _order.Comment = fieldValue;
                    break;
                //case "制单人":
                //case "制单时间":
                //case "审批人":
                //case "审批日期":
                //case "审核时间":
                //case "打印次数":
                //case "最后修改人":
                case "最后修改时间":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.LastModifyTime = dt;
                    }
                    break;                
                default:
                    break;
            }
        }

        //行项目
        protected override void BindingOrderItem(object[] array)
        {
            if (_order.TransferOrderDetail == null)
                _order.TransferOrderDetail = new EntityCollection<TransferOrderDetail>();

            if (_itemDefine == null)
                throw new ApplicationException("未发现行项目字段定义!");

            if (_itemDefine.Count() == array.Count())
            {
                TransferOrderDetail orderItem = new TransferOrderDetail();
                for (int i = 0; i < array.Count(); i++)
                {
                    string fieldName = Regex.Replace(_itemDefine[i].ToString(), @"\s", "");
                    string fieldValue = Regex.Replace(array[i].ToString(), @"\s", "");

                    switch (fieldName)
                    {
                        case "行号":
                            if (fieldValue.Contains("."))//如果行号是10.000，则取10，去掉小数点后面的0
                            {
                                orderItem.DetailRowNumber = fieldValue.Substring(0, fieldValue.IndexOf("."));
                            }
                            else
                                orderItem.DetailRowNumber = fieldValue;
                            break;
                        case "存货编码":
                            orderItem.CargoCode = fieldValue;
                            string cargoCode = DbCommonMethod.ParsingCargoCode(fieldValue);
                            if (string.IsNullOrEmpty(cargoCode))
                                throw new ApplicationException("商品不存在：" + fieldValue);
                            break;
                        case "存货名称":
                            orderItem.CargoName = fieldValue;
                            break;
                        //case "产地":
                        //    break;
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
                        case "主计量单位":
                            orderItem.CargoUnits = fieldValue;
                            break;
                        //case "辅单位":                       
                        //    break;
                        //case "换算率":
                        //    break;
                        //case "辅数量":
                        //    break;
                        case "数量":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                double d;
                                if (double.TryParse(fieldValue, out d))
                                    orderItem.NumOriginalPlan = d;
                            }
                            break;
                        //case "自由项":
                        //    break;
                        //case "批次号":
                        //    break;
                        case "生产日期":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {  
                                DateTime dt;
                                if (DateTime.TryParse(fieldValue, out dt))
                                    orderItem.ReleaseYear = dt.Year.ToString();
                            }
                
                            break;
                        //case "失效日期":
                        //    break;
                        //case "是否赠品":
                        //    break;
                        case "调出仓库":
                            orderItem.InWHName = fieldValue;
                            orderItem.InWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                            break;
                        case "调入仓库":
                            orderItem.OutWHName = fieldValue;
                            orderItem.OutWHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                            break;
                        //case "出货仓库":
                        //    break;
                        //case "调出部门":
                        //    break;
                        //case "业务员":
                        //    break;
                        //case "调入部门":
                        //    break;
                        //case "业务员":
                        //    break;
                        //case "出货部门":
                        //    break;
                        //case "业务员":
                        //    break;
                        //case "调出货位":
                        //    break;
                        //case "调入货位":
                        //    break;
                        //case "出货货位":
                        //    break;
                        //case "计划发货日":
                        //    break;
                        //case "计划到货日":
                        //    break;
                        //case "收货客户":
                        //    break;
                        //case "收货地区":
                        //    break;
                        //case "收货地址":
                        //    break;
                        //case "收货地点":
                        //    break;
                        //case "项目":
                        //    break;
                        //case "项目阶段":
                        //    break;
                        //case "发运方式":
                        //    break;
                        //case "非含税单价":
                        //    break;
                        //case "税率":
                        //    break;
                        //case "非含税金额":
                        //    break;
                        //case "含税金额":
                        //    break;
                        //case "询价含税价":
                        //    break;
                        //case "含税单价":
                        //    break;
                        //case "加价率":
                        //    break;
                        //case "询价非含税价":
                        //    break;
                        //case "已调出主数量":
                        //    break;
                        //case "已调出辅数量":
                        //    break;
                        //case "已调入主数量":
                        //    break;
                        //case "已调入辅数量":
                        //    break;
                        //case "调拨在途主数量":
                        //    break;
                        //case "调拨在途辅数量":
                        //    break;
                        //case "已发货主数量":
                        //    break;
                        //case "已发货辅数量":
                        //    break;
                        //case "签收主数量":
                        //    break;
                        //case "签收辅数量":
                        //    break;
                        //case "调出与调入已结算数量":
                        //    break;
                        //case "调出与出货方已经结算数量":
                        //    break;
                        //case "调出与出货方已经结算金额":
                        //    break;
                        //case "发运退回主数量":
                        //    break;
                        //case "发运退回辅数量":
                        //    break;
                        //case "发运途损主数量":
                        //    break;
                        //case "发运途损辅数量":
                        //    break;
                        //case "累计退回数量":
                        //    break;
                        //case "应发未出库数量":
                        //    break;
                        //case "来源单据类型":
                        //    break;
                        //case "来源单据号":
                        //    break;
                        //case "来源单据行号":
                        //    break;
                        //case "B-UDC1":
                        //    break;
                        //case "B-UDC2":
                        //    break;
                        //case "B-UDC3":
                        //    break;
                        //case "B-UDC4":
                        //    break;
                        //case "B-UDC5":
                        //    break;
                        //case "B-UDC6":
                        //    break;
                        //case "B-UDC7":
                        //    break;
                        //case "B-UDC8":
                        //    break;
                        //case "B-UDC9":
                        //    break;
                        //case "B-UDC10":
                        //    break;
                        //case "B-UDC11":
                        //    break;
                        //case "B-UDC12":
                        //    break;
                        //case "B-UDC13":
                        //    break;
                        //case "B-UDC14":
                        //    break;
                        //case "B-UDC15":
                        //    break;
                        //case "B-UDC16":
                        //    break;
                        //case "B-UDC17":
                        //    break;
                        //case "B-UDC18":
                        //    break;
                        //case "B-UDC19":
                        //    break;
                        //case "B-UDC20":
                        //    break;
                        //case "报价计量单位":
                        //    break;
                        //case "报价计量单位数量":
                        //    break;
                        //case "报价计量单位换算率":
                        //    break;
                        //case "报价计量单位原币含税净价":
                        //    break;
                        //case "报价计量单位原币无税净价": 
                        default:
                            break;

                    }
                }

                orderItem.DetailRowStatus = 2;//初始态
                orderItem.OrderCode = _order.OrderCode;               
                orderItem.NCOrderCode = _order.NCOrderCode;
                orderItem.TransferOrder = _order;

                IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                    new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("OrderCode", orderItem.OrderCode),
                        new KeyValuePair<string, object>("DetailRowNumber", orderItem.DetailRowNumber)
                    };
                orderItem.EntityKey = new EntityKey("GoldEntities.TransferOrderDetail", entityKeyValues);
                _order.TransferOrderDetail.Add(orderItem);
            }
        }

        public override string SaveData()
        {
            _order.OrderStatus = 2;//初始态
            _order.EditStatus = 0;//无人编辑

            if (_order.InWHCode == _order.OutWHCode && _order.OutWHCode != null && _order.InWHCode != null)
            {
                throw new Exception("调拨订单不允许调入和调出仓库相同！");
            }

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.TransferOrder.SingleOrDefault(o => o.OrderCode == _order.OrderCode);
                    if (tmp == null)
                    {
                        edm.TransferOrder.AddObject(_order);
                    }
                    else
                    {
                        //判断该订单是否已经开始其他作业,如果是，则不允许覆盖
                        if (tmp.OrderStatus != 2)
                        {                            
                            throw new Exception("此订单(" + tmp.OrderCode + ")已开始其他作业，不允许再次上传！");
                        }

                        foreach (TransferOrderDetail oldDetail in tmp.TransferOrderDetail)
                        {
                            //如果实收数量为0，则直接删除
                            if (oldDetail.NumActual == null || oldDetail.NumActual == 0)
                            {
                                continue;
                            }
                            else
                            {
                                bool isExist = false;//是否新旧订单都有此商品
                                foreach (TransferOrderDetail newDetail in _order.TransferOrderDetail)
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
                                    TransferOrderDetail td = new TransferOrderDetail();
                                    td.CargoCode = oldDetail.CargoCode;
                                    td.CargoModel = oldDetail.CargoModel;
                                    td.CargoName = oldDetail.CargoName;
                                    td.CargoSpec = oldDetail.CargoSpec;
                                    td.CargoUnits = oldDetail.CargoUnits;
                                    td.Comment = oldDetail.Comment;
                                    td.DetailRowNumber = oldDetail.DetailRowNumber;
                                    td.DetailRowStatus = oldDetail.DetailRowStatus;
                                    td.InWHCode = oldDetail.InWHCode;
                                    td.InWHName = oldDetail.InWHName;
                                    td.NCOrderCode = oldDetail.NCOrderCode;                                    
                                    td.NumActual = oldDetail.NumActual;
                                    td.NumCurrentPlan = oldDetail.NumCurrentPlan;
                                    td.NumOriginalPlan = oldDetail.NumOriginalPlan;
                                    td.OrderCode = oldDetail.OrderCode;
                                    td.OrderCode = oldDetail.OrderCode;
                                    td.OutWHCode = oldDetail.OutWHCode;
                                    td.OutWHName = oldDetail.OutWHName;
                                    td.ReleaseYear = oldDetail.Reserve1;
                                    td.Reserve2 = oldDetail.Reserve2;
                                    if (td.NumActual != null && td.NumActual != 0)
                                    {
                                        _order.OrderStatus = 1;//部分已下发
                                    }

                                    _order.TransferOrderDetail.Add(td);                                    
                                }
                            }
                        }

                        edm.TransferOrder.DeleteObject(tmp);
                        edm.SaveChanges();
                        edm.TransferOrder.AddObject(_order);
                    }

                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：" );
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "调拨订单保存异常", ex);
                }
            }

            return sb.ToString();
        }
    }
}