using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Gold.DAL;

namespace Gold.NCInvoke
{
    public class OrderInfoInvoke
    {
        /*
           billType: 单据类型，目前所需6种单据的billType取值如下表所示
            1.采购订单 PurchaseOrder
            2.销售订单 SalesOrder
            3.调拨订单 TransferOrder
            4.转库单 ShiftOrder
            5.其他入库单 OtherStockInBill
            6.其他出库单 OtherStockOutBill
         */

        /// <summary>
        /// 库房编号
        /// </summary>
        public string Warehouse { get; set; }

        /// <summary>
        /// 单据类别
        /// </summary>
        public string BillType { get; set; }

        /// <summary>
        /// 起始时间：格式为“yyyy-MM-dd HH:mm:ss”
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间：格式为“yyyy-MM-dd HH:mm:ss”
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 最大查询条数
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// 是否已导入
        /// </summary>
        public string isAlreadyStatus { get; set; }

        /// <summary>
        /// 通过WebService查询用友系统的采购订单数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCData(out DataTable dt, out string msg)
        {
            dt = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 通过WebService查询用友系统的订单明细数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetail(out DataTable dt, out string msg, string billCode)
        {
            dt = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 其他出、入库单明细-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetailJoinRFID(out List<StockDetail> lstStockDetail, out string msg, string billCode, string SICode,string WHCode)
        {
            lstStockDetail = null;
            msg = string.Empty;
            return false;
        }

        #region 采购订单
        /// <summary>
        /// 采购订单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<PurchaseOrder> lstPurchaseOrder, out string msg)
        {
            lstPurchaseOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 采购订单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<PurchaseOrder> lstPurchaseOrder, out string msg)
        {
            lstPurchaseOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 采购订单明细-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetailJoinRFID(out List<PurchaseOrderDetail> lstPurchaseOrderDetail, out string msg, string billCode)
        {
            lstPurchaseOrderDetail = null;
            msg = string.Empty;
            return false;
        }
        #endregion

        #region 销售订单
        /// <summary>
        /// 销售订单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<SalesOrder> lstSalesOrder, out string msg)
        {
            lstSalesOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 销售订单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<SalesOrder> lstSalesOrder, out string msg)
        {
            lstSalesOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 销售订单明细-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetailJoinRFID(out List<SalesOrderDetail> lstSalesOrderDetail, out string msg, string billCode)
        {
            lstSalesOrderDetail = null;
            msg = string.Empty;
            return false;
        }
        #endregion

        #region 转库单
        /// <summary>
        /// 转库单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<ShiftOrder> lstShiftOrder, out string msg)
        {
            lstShiftOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 转库单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<ShiftOrder> lstlstShiftOrder, out string msg)
        {
            lstlstShiftOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 转库单明细-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetailJoinRFID(out List<ShiftOrderDetail> lstShiftOrderDetail, out string msg, string billCode)
        {
            lstShiftOrderDetail = null;
            msg = string.Empty;
            return false;
        }
        #endregion

        #region 调拨订单
        /// <summary>
        /// 调拨订单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<TransferOrder> lstTransferOrder, out string msg)
        {
            lstTransferOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 调拨订单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<TransferOrder> lstTransferOrder, out string msg)
        {
            lstTransferOrder = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 调拨订单明细-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataDetailJoinRFID(out List<TransferOrderDetail> lstTransferOrderDetail, out string msg, string billCode)
        {
            lstTransferOrderDetail = null;
            msg = string.Empty;
            return false;
        }
        #endregion   

        #region 其他入库单
        /// <summary>
        /// 其他入库单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<StockIn> lstStockIn, out string msg)
        {
            lstStockIn = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 其他入库单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<StockIn> lstStockIn, out string msg)
        {
            lstStockIn = null;
            msg = string.Empty;
            return false;
        }

        
        #endregion   

        #region 其他出库单
        /// <summary>
        /// 其他出库单-通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out List<StockOut> lstStockOut, out string msg)
        {
            lstStockOut = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 其他出库单-将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(List<StockOut> lstStockOut, out string msg)
        {
            lstStockOut = null;
            msg = string.Empty;
            return false;
        }        
        #endregion           
    }
}