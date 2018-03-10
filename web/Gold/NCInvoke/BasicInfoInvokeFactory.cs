using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.NCInvoke
{
    /// <summary>
    /// 简单工厂模式
    /// </summary>
    public class BasicInfoInvokeFactory
    {
        /// <summary>
        /// 工厂方法
        /// </summary>
        /// <param name="typeArgs">WebService中BasicInfoQuery的第一个参数</param>
        /// <param name="conditionArgs">WebService中BasicInfoQuery的第二个参数</param>
        /// <returns></returns>
        public static BasicInfoInvoke CreateInstance(string typeArgs, string conditionArgs)
        {
            BasicInfoInvoke basicInfoInvokeObj = new BasicInfoInvoke();


            switch (typeArgs)
            {
                case "WareHouse":
                    basicInfoInvokeObj = new BasicWareHouseInvoke();
                    break;
                case "Specification":
                    basicInfoInvokeObj = new BasicSpecInvoke();
                    break;
                case "CargoInfo":
                    basicInfoInvokeObj = new BasicCargoInfoInvoke();
                    break;
                case "CargoPrice":
                    basicInfoInvokeObj = new BasicCargoPriceInvoke();
                    break;
                case "Model":
                    basicInfoInvokeObj = new BasicModelInvoke();
                    break;
                case "Department":
                    basicInfoInvokeObj = new BasicDepartmentInvoke();
                    break;
                case "UserInfo":
                    basicInfoInvokeObj = new BasicUserInfocInvoke();
                    break;
                case "CargoInventory":
                    basicInfoInvokeObj = new BasicCargoInventoryInvoke();
                    break;

                default:
                    break;
            }

            basicInfoInvokeObj.ConditionArgs = conditionArgs;
            basicInfoInvokeObj.TypeArgs = typeArgs;

            return basicInfoInvokeObj;
        }
    }
}