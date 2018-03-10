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
    /// 其他入库单Excel文件解析
    /// </summary>
    public class StockInExcelParser : ExcelParser
    {
        private StockIn _order = new StockIn();

        public StockIn Order
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
                return string.Format("入库单号：{0}", _order.SICode);
            }
        }

        public StockInExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.SICode))
                throw new ApplicationException("文件格式错误！不能解析为转库单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (title != "其他入库单" && title != "入库单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "入库单号":
                    //_order.SICode = fieldValue;
                    _order.FromBillNo = fieldValue;
                    _order.SICode = DbCommonMethod.GetStockInCode(fieldValue);                    
                    break;
                case "日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.SIDate = dt;
                    }
                    break;
                case "供货单位":
                    _order.Supplier = fieldValue;
                    break;
                case "业务部门":
                    _order.BusinessDepartmentName = fieldValue;
                    break;
                case "业务员":
                    _order.Operator = fieldValue;
                    break;
                case "仓库":
                    _order.WHName = fieldValue;
                    _order.WHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    if (string.IsNullOrEmpty(_order.WHCode))
                        throw new ApplicationException("仓库不存在：" + fieldValue);
                    break;
                case "库管员":
                    _order.StoreKeeper = fieldValue;
                    break;
                case "备注":
                    _order.Comment = fieldValue;
                    break;
                case "合计数量":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        double d;
                        if (double.TryParse(fieldValue, out d))
                            _order.TotalNumber = d;
                    }
                    break;
                case "仓管":
                   // _order.BusinessManager = fieldValue;
                    break;
                case "验收":
                    _order.Checker = fieldValue;
                    break;
                case "收发类别":
                    _order.InCategory = DbCommonMethod.GetInCategory(fieldValue);
                    break;
                default:
                    break;
            }
        }

        protected override void BindingOrderItem(object[] array)
        {
            if (_order.StockDetail == null)
                _order.StockDetail = new EntityCollection<StockDetail>();

            if (_itemDefine == null)
                throw new ApplicationException("未发现行项目字段定义!");

            if (_itemDefine.Count() == array.Count())
            {
                StockDetail orderItem = new StockDetail();
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
                        case "备注":
                            orderItem.Comment = fieldValue;
                            break;
                        default:
                            break;

                    }
                }
                if (string.IsNullOrEmpty(orderItem.BillRowNumber))
                    orderItem.BillRowNumber = _rowNum.ToString();

                //if (string.IsNullOrEmpty(_order.SICode))
                //    _order.SICode = KeyGenerator.Instance.GetStockInKey();

                orderItem.NumCurrentPlan = orderItem.NumOriginalPlan;//订单数量=应收数量，默认值
                orderItem.CargoStatus = 0;//手持机未完成
                orderItem.BillCode = _order.SICode;
                orderItem.SICode = _order.SICode;
                orderItem.BillType = CommonConvert.GetBillTypeCode("入库单");
                orderItem.InOutWHCode = _order.WHCode;
                //orderItem.NCOrderCode = _order.NCOrderCode;
                //orderItem.ShiftOrder = _order;
                //IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                //    new KeyValuePair<string, object>[] {
                //        new KeyValuePair<string, object>("BillCode", orderItem.BillCode),
                //        new KeyValuePair<string, object>("BillRowNumber", orderItem.BillRowNumber)
                //    };
                //orderItem.EntityKey = new EntityKey("GoldEntities.StockDetail", entityKeyValues);
                _order.StockDetail.Add(orderItem);
            }
        }

        public override string SaveData()
        {
            if (string.IsNullOrEmpty(_order.SIType))
                _order.SIType = CommonConvert.GetSIOTypeCode("其他入库单");

            if (string.IsNullOrEmpty(_order.FromType))
                _order.FromType = CommonConvert.GetFromTypeCode("源于Excel导入的入库单");

            _order.SIStatus = 1;//已保存
            _order.FromBillType = "23";//21采购入库单、22调拨入库单、23其他入库单
            _order.EditStatus = 0;//无人编辑

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.StockIn.SingleOrDefault(o => o.SICode == _order.SICode);
                    if (tmp == null)
                    {
                        edm.StockIn.AddObject(_order);
                    }
                    else
                    {
                        if (tmp.SIStatus == 2 )
                        {
                            throw new Exception("此入库单(" + tmp.SICode + ")已提交，不允许再次上传！"); 
                        }
                        else if (tmp.SIStatus == 5)
                        {
                            throw new Exception("此入库单(" + tmp.SICode + ")正在撤销中，不允许再次上传！");                            
                        }
                        else if (tmp.SIStatus == 3)
                        {
                            throw new Exception("此入库单(" + tmp.SICode + ")入库已完成，不允许再次上传！");
                        }
                        else if (tmp.SIStatus == 4)//已撤销，则默认是全部撤销，用户可重新上传而不是覆盖
                        {
                            //_order.SICode = KeyGenerator.Instance.GetStockInKey();
                            //foreach (StockDetail sd in _order.StockDetail)
                            //{
                            //    sd.BillCode = _order.SICode;
                            //    sd.SOCode = _order.SICode;
                            //}
                            //edm.StockIn.AddObject(_order);
                            throw new Exception("此入库单(" + tmp.SICode + ")已完成撤销，不允许再次上传！");
                        }
                        else
                        {

                            List<StockDetail> lstDetail = new List<StockDetail>();
                            foreach (StockDetail oldDetail in tmp.StockDetail)
                            {
                                lstDetail.Add(oldDetail);
                                //如果实收数量为0，则直接删除
                                if (oldDetail.NumActual == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    bool isExist = false;//是否新旧订单都有此商品
                                    foreach (StockDetail newDetail in _order.StockDetail)
                                    {
                                        //判断在当前的新excel中是否有此商品,如果有，则保存实收数量
                                        if (newDetail.CargoCode == oldDetail.CargoCode)
                                        {
                                            newDetail.NumActual = oldDetail.NumActual;
                                            isExist = true;
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
                                        StockDetail sd = new StockDetail();
                                        sd.BillCode = oldDetail.BillCode;
                                        sd.BillRowNumber = oldDetail.BillRowNumber;
                                        sd.BillType = oldDetail.BillType;
                                        sd.BinCode = oldDetail.BinCode;
                                        sd.BinName = oldDetail.BinName;
                                        sd.CargoCode = oldDetail.CargoCode;
                                        sd.CargoModel = oldDetail.CargoModel;
                                        sd.CargoName = oldDetail.CargoName;
                                        sd.CargoSpec = oldDetail.CargoSpec;
                                        sd.CargoStatus = oldDetail.CargoStatus;
                                        sd.CargoUnits = oldDetail.CargoUnits;
                                        sd.Comment = oldDetail.Comment;
                                        sd.HandSetFinishTime = oldDetail.HandSetFinishTime;
                                        sd.HandSetPersonID = oldDetail.HandSetPersonID;
                                        sd.HandSetPersonName = oldDetail.HandSetPersonName;
                                        sd.InOutWHCode = oldDetail.InOutWHCode;
                                        sd.InOutWHName = oldDetail.InOutWHName;
                                        sd.NumActual = oldDetail.NumActual;
                                        sd.NumCurrentPlan = oldDetail.NumCurrentPlan;
                                        sd.NumOriginalPlan = oldDetail.NumOriginalPlan;
                                        sd.ReleaseYear = oldDetail.ReleaseYear;
                                        sd.Reserve1 = oldDetail.Reserve1;
                                        sd.Reserve2 = oldDetail.Reserve2;
                                        sd.RFIDOrderNo = oldDetail.RFIDOrderNo;
                                        sd.RFIDSubmitTime = oldDetail.RFIDSubmitTime;
                                        sd.RowTotalMoney = oldDetail.RowTotalMoney;
                                        sd.SICode = oldDetail.SICode;
                                        sd.UCOrderNo = oldDetail.UCOrderNo;

                                        _order.StockDetail.Add(sd);
                                    }
                                }
                            }

                            //由于遍历原数据行项目，导致后续删除无法级联删除。故先删除行项目。
                            foreach (StockDetail detail in lstDetail)
                            {
                                edm.StockDetail.DeleteObject(detail);
                                edm.SaveChanges();
                            }

                            edm.StockIn.DeleteObject(tmp);
                            edm.SaveChanges();
                            edm.StockIn.AddObject(_order);
                        }                        
                    }

                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：" );
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "其他入库单保存异常", ex);
                }
            }

            return sb.ToString();
        }
    }

}
