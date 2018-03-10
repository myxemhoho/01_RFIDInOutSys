using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gold.Upload
{

    public class ExcelParserFactory
    {
        public static ExcelParser CreateInstance(string fileType, string fileName)
        {
            switch(fileType)
            {
                case "采购订单":
                    return new PurchaseOrderExcelParser(fileName);
                case "销售订单":
                    return new SalesOrderExcelParser(fileName);
                case "转库订单":
                    return new ShiftOrderExcelParser(fileName);
                case "其他入库单":
                    return new StockInExcelParser(fileName);
                case "其他出库单":
                    return new StockOutExcelParser(fileName);
                case"调拨订单":
                    return new TransferOrderExcelParser(fileName);
                case"调拨入库单":
                    return new TransferInExcelParser(fileName);
                default:
                    return null;
            }
        }
    }
}
