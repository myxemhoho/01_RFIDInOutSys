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
    /// 其他出库单Excel文件解析
    /// </summary>
    public class StockOutExcelParser: ExcelParser
    {
        private StockOut _order = new StockOut();

        public StockOut Order
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
                return string.Format("出库单号：{0}", _order.SOCode);
            }
        }

        public StockOutExcelParser(string fileName)
            : base(fileName)
        {
            if (string.IsNullOrEmpty(_order.SOCode))
                throw new ApplicationException("文件格式错误！不能解析为转库单");
        }

        protected override void VerifyFileTitle(string title)
        {
            if (title != "其他出库单" && title != "出库单")
                throw new ApplicationException("单据类型错误：" + title);
        }

        protected override void BindingOrderHeader(string fieldName, string fieldValue)
        {
            switch (fieldName)
            {
                case "单据号":                   
                    _order.FromBillNo = fieldValue;                   
                    _order.SOCode = DbCommonMethod.GetStockOutCode(fieldValue);
                    break;
                case "日期":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(fieldValue, out dt))
                            _order.SODate = dt;
                    }
                    break;
                case "发出仓库":
                    _order.WHName = fieldValue;
                    _order.WHCode = DbCommonMethod.ParsingWarehouse(fieldValue);
                    if (string.IsNullOrEmpty(_order.WHCode))
                        throw new ApplicationException("仓库不存在：" + fieldValue);
                    break;
                case "备注":
                    _order.Comment = fieldValue;
                    break;
                case "收发类别":
                    _order.OutCategory = DbCommonMethod.GetOutCategory(fieldValue);
                    break;
                case"领导":
                    _order.LeaderSign = fieldValue;
                    break;
                case "仓管":
                    _order.WarehouseSign = fieldValue;
                    break;
                case "财务":
                    _order.AccountSign = fieldValue;
                    break;
                case "经办人":
                    _order.BusinessSign = fieldValue;
                    break;
                case "制单人":
                    _order.EditorSign = fieldValue;
                    break;
                case "合            计  （RMB)":
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        decimal d;
                        if (decimal.TryParse(fieldValue, out d))
                            _order.TotalMoney = d;
                    }
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
                        case "存货编码":
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

                        //暂留空，因为没看到有字段保存单价信息的(因为系统只管商品,不管金额,所以暂时不用存单价)
                        //case "销售单价":
                        //    if (!string.IsNullOrEmpty(fieldValue))
                        //    {
                        //        decimal d;
                        //        if (decimal.TryParse(fieldValue, out d))
                        //            orderItem.Price = d;
                        //    }
                        //    break;
                        
                        case "销售金额":
                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                decimal de;
                                if (decimal.TryParse(fieldValue, out de))
                                    orderItem.RowTotalMoney = de;
                            }
                            break;
                        default:
                            break;

                    }
                }
                if (string.IsNullOrEmpty(orderItem.BillRowNumber))
                    orderItem.BillRowNumber = _rowNum.ToString();

                orderItem.NumCurrentPlan = orderItem.NumOriginalPlan;//订单数量=应收数量，默认值
                orderItem.CargoStatus = 0;//手持机未完成
                orderItem.BillCode = _order.SOCode;
                orderItem.SOCode = _order.SOCode;
                orderItem.BillType = CommonConvert.GetBillTypeCode("出库单");
                orderItem.InOutWHCode = _order.WHCode;
                orderItem.StockOut = _order;
                IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                    new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("BillCode", orderItem.BillCode),
                        new KeyValuePair<string, object>("BillRowNumber", orderItem.BillRowNumber)
                    };
                orderItem.EntityKey = new EntityKey("GoldEntities.StockDetail", entityKeyValues);
                _order.StockDetail.Add(orderItem);
            }
        }

        public override string SaveData()
        {
            if (string.IsNullOrEmpty(_order.SOType))
                _order.SOType = CommonConvert.GetSIOTypeCode("其他出库单");

            if (string.IsNullOrEmpty(_order.FromType))
                _order.FromType = CommonConvert.GetFromTypeCode("源于Excel导入的出库单");

            _order.SOStatus = 1;//已保存
            _order.FromBillType = "13";//11-销售出库单、12-调拨出库单、13-其他出库单
            _order.EditStatus = 0;//无人编辑

            StringBuilder sb = new StringBuilder();
            using (var edm = new Gold.DAL.GoldEntities())
            {
                try
                {
                    var tmp = edm.StockOut.SingleOrDefault(o => o.SOCode == _order.SOCode);
                    if (tmp == null)
                    {
                        edm.StockOut.AddObject(_order);                        
                    }
                    else
                    {
                        if (tmp.SOStatus == 2)
                        {
                            throw new Exception("此出库单(" + tmp.SOCode + ")已提交，不允许再次上传！");                            
                        }
                        else if (tmp.SOStatus == 5)
                        {
                            throw new Exception("此出库单(" + tmp.SOCode + ")正在撤销中，不允许再次上传！");                            
                        }
                        else if (tmp.SOStatus == 3)
                        {
                            throw new Exception("此出库单(" + tmp.SOCode + ")出库已完成，不允许再次上传！");                            
                        }
                        else if (tmp.SOStatus == 4)//已撤销，则默认是全部撤销，用户可重新上传而不是覆盖
                        {
                            //_order.SOCode = KeyGenerator.Instance.GetStockOutKey(); 
                            //foreach(StockDetail  sd in _order.StockDetail)
                            //{
                            //    sd.BillCode = _order.SOCode;
                            //    sd.SOCode = _order.SOCode;
                            //}
                            //edm.StockOut.AddObject(_order);
                            throw new Exception("此出库单(" + tmp.SOCode + ")已完成撤销，不允许再次上传！");
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
                                        sd.SOCode = oldDetail.SOCode;
                                        sd.UCOrderNo = oldDetail.UCOrderNo;
                                        sd.SICode = oldDetail.SICode;
                                        sd.SCCode = oldDetail.SCCode;
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

                            edm.StockOut.DeleteObject(tmp);
                            edm.SaveChanges();
                            edm.StockOut.AddObject(_order);
                        }
                    }

                    edm.SaveChanges();
                    sb.AppendLine("保存成功！");
                }
                catch (Exception ex)
                {
                    sb.Append("保存数据时发生异常：");
                    string msg = Utility.LogHelper.GetExceptionMsg(ex);
                    sb.Append(msg);

                    Utility.LogHelper.WriteLog(Utility.LogHelper.LogLevel.Error, "其他出库单保存异常", ex);
                }
            }

            return sb.ToString();
        }
    }
}