using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Gold.NCInvoke
{
    public class BasicInfoInvoke
    {
        /*
    condition: 查询条件，用于缩小查询范围，该字段只有在获取部门、用户信息、库存基础资料时才有用，见下表
	type: 基础资料类型，目前所需10种基础资料的type取值如下表所示
序号	基础资料名称	type取值	condition含义	说明	类型
1		仓库	WareHouse	-	返回总公司所有仓库	字符串
2		部门	Department	所属子公司	函数只返回公司名等于condition的部门	字符串
3		商品规格	Specification	-	返回所有商品规格	字符串
4		商品型号	Model	-	返回所有商品型号	字符串
5		商品单位	Unit	-	返回所有商品单位	字符串
6		商品类别	CargoType	-	返回所有商品类别	字符串
7		商品信息	CargoInfo	-	返回所有商品信息	字符串
8		商品价格	CargoPrice	-	返回所有商品价格信息	字符串
9		商品库存	CargoInventory	商品所属库房	函数只返回库房编码等于condition的商品库存	字符串
10		用户信息	UserInfo	所属子公司	函数只返回公司名等于condition的用户	字符串
        */

        /// <summary>
        /// 查询数据的类别
        /// </summary>
        public string TypeArgs { get; set; }

        /// <summary>
        /// 查询数据的条件
        /// </summary>
        public string ConditionArgs { get; set; }

        ///// <summary>
        ///// GridView控件的列
        ///// </summary>
        //public virtual System.Web.UI.WebControls.DataControlFieldCollection Columns { get; set; }

        /// <summary>
        /// 通过WebService查询用友系统的数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCData(out DataTable dt,out string msg) 
        {
            dt = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 通过WebService查询用友系统的数据并与RFID系统进行连接查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool GetNCDataJoinRFID(out DataTable dt, out string msg)
        {
            dt = null;
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 将用户选择的用友数据保存到RFID系统
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool SaveToRFID(DataTable dt, out string msg) 
        {
            msg = string.Empty;
            return false;
        }
    }
}