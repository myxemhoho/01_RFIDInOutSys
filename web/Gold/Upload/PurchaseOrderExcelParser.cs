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
    public class PurchaseOrderExcelParser : ExcelParser
    {
        private PurchaseOrder _order = new PurchaseOrder();

        public PurchaseOrder Order
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
                return string.Format("采购订单号：{0}", _order.NCOrderCode);
            }
        }


        public PurchaseOrderExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.NCOrderCode))
                throw new ApplicationException("文件格式错误！不能解析为采购订单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (title != "维护订单" && title != "采购订单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "公司":
                    _order.Company = fieldValue;
                    break;
                case "订单编号":
                    _order.NCOrderCode = fieldValue;
                    _order.OrderCode = fieldValue;
                    break;
                case "版本号":
                    _order.Version = fieldValue;
                    break;
                //case "是否补货":
                //    break;
                //case "是否发运":
                //    break;
                //case "是否退货":
                //    break;
                case "订单日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.OrderDate = dt;
                    }
                    break;
                case "业务类型":
                    _order.BusinessType = fieldValue;
                    break;
                case "采购组织":
                    _order.Purchaser = fieldValue;
                    break;
                case "供应商":
                    _order.Supplier = fieldValue;
                    break;
                //case "散户":
                //    break;
                //case "开户银行":
                //    break;
                case "供应商收发货地址":
                    _order.SupplierAddr = fieldValue;
                    break;
                case "采购员":
                    _order.Buyer = fieldValue;
                    break;
                case "部门":
                    _order.DepartmentName = fieldValue;
                    break;
                case "收货方":
                    _order.Receiver = fieldValue;
                    break;
                case "发票方":
                    _order.ReceiptCompany = fieldValue;
                    break;
                //case "发运方式":
                //    break;
                //case "付款协议":
                //    break;
                //case "本币预付款限额":
                //    break;
                //case "本币预付款":
                //    break;
                //case "币种":
                //    break;
                //case "折本汇率":
                //    break;
                //case "折辅汇率":
                //    break;
                //case "扣税类别":
                //    break;
                //case "税率":
                //    break;
                case "备注":
                    _order.Comment = fieldValue;
                    break;
                case "制单人":
                    _order.Preparer = fieldValue;
                    break;
                case "审批人":
                    _order.Approver = fieldValue;
                    break;
                //case "审批日期":   //下文还有审批时间
                //    if (!string.IsNullOrEmpty(fieldValue))
                //    {
                //        DateTime dt;
                //        if (DateTime.TryParse(fieldValue, out dt))
                //            _order.ApproveTime = dt;
                //    }
                //    break;
                case "制单时间":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.PrepareTime = dt;
                    }
                    break;
                case "审批时间":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.ApproveTime = dt;
                    }
                    break;
                case "最后修改时间":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.LastEditTime = dt;
                    }
                    break;
                default:
                    break;
            }
        }

        //行项目
        protected override void BindingOrderItem(object[] array)
        {
            if (_order.PurchaseOrderDetail == null)
                _order.PurchaseOrderDetail = new EntityCollection<PurchaseOrderDetail>();

            if (_itemDefine == null)
                throw new ApplicationException("未发现行项目字段定义!");

            if (_itemDefine.Count() == array.Count())
            {
                PurchaseOrderDetail orderItem = new PurchaseOrderDetail();
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
                        case "合同号":
                            orderItem.ContractNo = fieldValue;
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
                        //case "自由项":
                        //    orderItem.FreedomItem = fieldValue;
                        //    break;
                        case "收货仓库":
                            orderItem.WHName = fieldValue;
                            orderItem.WHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                            if (string.IsNullOrEmpty(orderItem.WHCode))
                                throw new ApplicationException("仓库不存在：" + fieldValue);                    
                            break;
                        case "计量单位":
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
                        case "含税单价":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.PriceOfTax = d;
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
                        case "扣率":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                double d;
                                if (double.TryParse(fieldValue, out d))
                                    orderItem.DeductRate = d;
                            }
                            break;
                        case "净含税单价":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.PriceOfNetTax = d;
                            }
                            break;
                        case "净单价":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.NetPrice = d;
                            }
                            break;
                        case "金额":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.TotalPrice = d;
                            }
                            break;
                        case "税率":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                double d;
                                if (double.TryParse(fieldValue, out d))
                                    orderItem.TaxRate = d;
                            }
                            break;
                        case "税额":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.TotalTax = d;
                            }
                            break;
                        case "价税合计":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal d;
                                if (decimal.TryParse(fieldValue, out d))
                                    orderItem.TotalTaxAndPrice = d;
                            }
                            break;
                        case "计划到货日期":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                DateTime d;
                                if (DateTime.TryParse(fieldValue, out d))
                                    orderItem.PlanArrivalDate = d;
                            }
                            break;
                        case "币种":
                            orderItem.CurrencyType = fieldValue;
                            break;
                        case "折本汇率":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                double d;
                                if (double.TryParse(fieldValue, out d))
                                    orderItem.ExchangeRate = d;
                            }
                            break;
                        case "收货公司":
                            orderItem.ReceiveCompany = fieldValue;
                            break;
                        case "收货库存组织":
                            orderItem.ReceiveOrg = fieldValue;
                            break;
                        case "收票公司":
                            orderItem.ReceiveBillCompany = fieldValue;
                            break;
                        default:
                            break;

                    }
                }

                orderItem.DetailRowStatus = 2;//初始态
                orderItem.OrderCode = _order.OrderCode;
                orderItem.NCOrderCode = _order.NCOrderCode;                
                orderItem.PurchaseOrder = _order;
                IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                    new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("OrderCode", orderItem.OrderCode),
                        new KeyValuePair<string, object>("DetailRowNumber", orderItem.DetailRowNumber)
                    };
                orderItem.EntityKey = new EntityKey("GoldEntities.PurchaseOrderDetail", entityKeyValues);
                _order.PurchaseOrderDetail.Add(orderItem);
            }
        }

        //保存数据
        public override string SaveData()
        {
            //if (_order.EntityKey == null)
            //{
            //    IEnumerable<KeyValuePair<string, object>> entityKeyValues =
            //        new KeyValuePair<string, object>[] {
            //            new KeyValuePair<string, object>("OrderCode", _order.OrderCode),
            //        };
            //    _order.EntityKey = new EntityKey("GoldEntities.PurchaseOrder", entityKeyValues);
            //}

            _order.OrderStatus = 2;//初始态
            _order.EditStatus = 0;//无人编辑

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.PurchaseOrder.SingleOrDefault(o => o.OrderCode == _order.OrderCode);
                    if (tmp == null)
                    {
                        edm.PurchaseOrder.AddObject(_order);
                    }
                    else
                    {
                        //判断该订单是否已经开始其他作业,如果是，则不允许覆盖
                        if (tmp.OrderStatus != 2)
                        {
                            throw new Exception("此订单(" + tmp.OrderCode + ")已开始其他作业，不允许再次上传！");
                        }

                        foreach (PurchaseOrderDetail oldpod in tmp.PurchaseOrderDetail)
                        {
                            //如果实收数量为0，则直接删除
                            if (oldpod.NumActual==null || oldpod.NumActual == 0)
                            {
                                continue;
                            }
                            else
                            {
                                bool isExist = false;//是否新旧订单都有此商品
                                foreach (PurchaseOrderDetail newpod in _order.PurchaseOrderDetail)
                                {
                                    //判断在当前的新excel中是否有此商品,如果有，则保存实收数量
                                    if (newpod.CargoCode == oldpod.CargoCode)
                                    {                                        
                                        newpod.NumActual = oldpod.NumActual;
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
                                    PurchaseOrderDetail pd = new PurchaseOrderDetail();
                                    pd.CargoCode = oldpod.CargoCode;
                                    pd.CargoModel = oldpod.CargoModel;
                                    pd.CargoName = oldpod.CargoName;
                                    pd.CargoSpec = oldpod.CargoSpec;
                                    pd.CargoUnits = oldpod.CargoUnits;
                                    pd.ContractNo = oldpod.ContractNo;
                                    pd.CurrencyType = oldpod.CurrencyType;//币种
                                    pd.DeductRate = oldpod.DeductRate;
                                    pd.DetailRowNumber = oldpod.DetailRowNumber;
                                    pd.DetailRowStatus = oldpod.DetailRowStatus;
                                    pd.ExchangeRate = oldpod.ExchangeRate;
                                    pd.FreedomItem = oldpod.FreedomItem;
                                    pd.NCOrderCode = oldpod.NCOrderCode;
                                    pd.NetPrice = oldpod.NetPrice;
                                    pd.NumActual = oldpod.NumActual;
                                    pd.NumCurrentPlan = oldpod.NumCurrentPlan;
                                    pd.NumOriginalPlan = oldpod.NumOriginalPlan;
                                    pd.OrderCode = oldpod.OrderCode;
                                    pd.PlanArrivalDate = oldpod.PlanArrivalDate;
                                    pd.Price = oldpod.Price;
                                    pd.PriceOfNetTax = oldpod.PriceOfNetTax;
                                    pd.PriceOfTax = oldpod.PriceOfTax;
                                    pd.ReceiveBillCompany = oldpod.ReceiveBillCompany;
                                    pd.ReceiveCompany = oldpod.ReceiveCompany;
                                    pd.ReceiveOrg = oldpod.ReceiveOrg;
                                    pd.ReleaseYear = oldpod.ReleaseYear;
                                    pd.Reserve1 = oldpod.Reserve1;
                                    pd.Reserve2 = oldpod.Reserve2;
                                    pd.TaxRate = oldpod.TaxRate;
                                    pd.TotalPrice = oldpod.TotalPrice;
                                    pd.TotalTax = oldpod.TotalTax;
                                    pd.TotalTaxAndPrice = oldpod.TotalTaxAndPrice;
                                    pd.WHCode = oldpod.WHCode;
                                    pd.WHName = oldpod.WHName;
                                    if (pd.NumActual != null && pd.NumActual != 0)
                                    {
                                        _order.OrderStatus = 1;//部分已下发
                                    }

                                    _order.PurchaseOrderDetail.Add(pd);
                                } 
                            }                            
                        }

                        edm.PurchaseOrder.DeleteObject(tmp);
                        edm.SaveChanges();
                        edm.PurchaseOrder.AddObject(_order);
                        //int count = tmp.PurchaseOrderDetail.Count;
                        //List<EntityKey> orignalKeys = new List<EntityKey>();
                        //for (int i = 0; i < count; )
                        //{
                        //    orignalKeys.Add(tmp.PurchaseOrderDetail.ElementAt(i).EntityKey);
                        //    edm.ObjectStateManager.ChangeObjectState(tmp.PurchaseOrderDetail.ElementAt(i), EntityState.Deleted);
                        //    count--;
                        //}

                        //foreach (PurchaseOrderDetail item in _order.PurchaseOrderDetail)
                        //{
                        //    if (orignalKeys.Contains(item.EntityKey))
                        //    {
                        //        edm.ObjectStateManager.ChangeObjectState(item, EntityState.Modified);
                        //        edm.PurchaseOrderDetail.ApplyCurrentValues(item as PurchaseOrderDetail);
                        //    }
                        //    else
                        //    {
                        //        item.EntityKey = null;
                        //        edm.PurchaseOrderDetail.AddObject(item);
                        //        edm.ObjectStateManager.ChangeObjectState(item, EntityState.Added);
                        //    }
                        //}

                        //edm.PurchaseOrder.ApplyCurrentValues(_order);
                    }


                    //    //List<T> collectionItemList = collection.ToList();
                        
                    //    //GenericUpdateEntityCollection<PurchaseOrderDetail>(_order.PurchaseOrderDetail, edm);

                    //    //edm.PurchaseOrder.Detach(tmp);
                    //    //edm.PurchaseOrder.Attach(_order);
                    //    //edm.ObjectStateManager.ChangeObjectState(_order, EntityState.Modified);
                    //    //sb.AppendLine("数据库存在相同单号的订单！将被更新...~");
                    //    //PurchaseOrderDetail dd = _order.PurchaseOrderDetail.First();
                    //    //edm.ObjectStateManager.ChangeObjectState(dd, EntityState.Deleted);
                    //    //foreach (PurchaseOrderDetail item in _order.PurchaseOrderDetail)
                    //    //{
                    //    //    edm.PurchaseOrderDetail.ApplyCurrentValues(item);
                    //    //    edm.ObjectStateManager.ChangeObjectState(item, EntityState.Modified);
                    //    //}
                    //    ////tmp.PurchaseOrderDetail = _order.PurchaseOrderDetail;
                    //}


                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：" );
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "采购订单保存异常", ex);
                }
            }

            return sb.ToString();
        }


        //private void GenericUpdateEntityCollection<T>(EntityCollection<T> collection, System.Data.Objects.ObjectContext dbContext)
        //    where T : EntityObject, new()
        //{
        //    int count = collection.Count();
        //    int current = 0;
        //    List<T> collectionItemList = collection.ToList();
        //    while (current < count)
        //    {
        //        bool isAdded = false;
        //        Object obj = null;
        //        dbContext.TryGetObjectByKey(collectionItemList[current].EntityKey, out obj);
        //        if (obj == null)
        //        {
        //            //obj = new PurchaseOrderDetail();
        //            //((T)obj).EntityKey = collectionItemList[current].EntityKey;
        //           // dbContext.AddObject();
        //            //dbContext.TryGetObjectByKey(collectionItemList[current].EntityKey, out obj);
        //            //dbContext.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
        //            //collection.CreateSourceQuery().Context.ObjectStateManager.ChangeObjectState(collectionItemList[current], System.Data.EntityState.Modified);
        //            //isAdded = true;
        //        }

        //        if (obj != null)
        //        {
        //            dbContext.ApplyCurrentValues<T>(((T)obj).EntityKey.EntitySetName, collectionItemList[current]);
        //            if (isAdded)
        //            {
        //                dbContext.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Added);
        //                collection.CreateSourceQuery().Context.ObjectStateManager.ChangeObjectState(collectionItemList[current], System.Data.EntityState.Added);
        //            }
        //        }
        //        current++;
        //    }
        //}

    }

}
