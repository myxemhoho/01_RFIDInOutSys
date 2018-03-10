using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.NCInvoke
{
    /// <summary>
    /// 简单工厂模式
    /// </summary>
    public class OrderInfoInvokeFactory
    {
        public static OrderInfoInvoke CreateInstance(string warehouse, string billType, string startTime, string endTime, int maxCount, string isAlreadyStatus)
        {
            OrderInfoInvoke orderInfoInvokeObj = new OrderInfoInvoke();

            switch (billType)
            {
                case "PurchaseOrder"://采购订单
                    orderInfoInvokeObj = new OrderPurchaseInvoke();
                    break;
                case "SalesOrder"://销售订单
                    orderInfoInvokeObj = new OrderSalesInvoke();
                    break;
                case "ShiftOrder"://转库单
                    orderInfoInvokeObj = new OrderShiftInvoke();
                    break;
                case "TransferOrder"://调拨订单
                    orderInfoInvokeObj = new OrderTransferInvoke();
                    break;
                case "OtherStockInBill"://其他入库单
                    orderInfoInvokeObj = new OtherStockInBillInvoke();
                    break;
                case "OtherStockOutBill"://其他出库单
                    orderInfoInvokeObj = new OtherStockOutBillInvoke();
                    break;
                default:
                    break;
            }

            orderInfoInvokeObj.Warehouse = warehouse;
            orderInfoInvokeObj.BillType = billType;
            orderInfoInvokeObj.StartTime = startTime;
            orderInfoInvokeObj.EndTime = endTime;
            orderInfoInvokeObj.MaxCount = maxCount;
            orderInfoInvokeObj.isAlreadyStatus = isAlreadyStatus;

            return orderInfoInvokeObj;
        }
    }
}