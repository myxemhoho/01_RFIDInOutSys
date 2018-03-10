using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gold.Utility
{
    /// <summary>
    /// 定义项目中使用的各种枚举类型数据
    /// </summary>
    public class EnumData
    {
        /// <summary>
        /// 层位类型
        /// </summary>
        public enum BinTypeEnum : int
        {
            [Description("实体层位")]
            Factual = 1,

            [Description("虚拟层位")]
            Virtual = 0

        }

        /// <summary>
        /// 层位标签状态
        /// </summary>
        public enum BinTagStatusEnum
        {
            [Description("亮灯中")]
            Lighting=2,

            [Description("正常")]
            Normal = 1,

            [Description("异常")]
            Abnormal = 0
        }

        /// <summary>
        /// 商品可售状态
        /// </summary>
        public enum CargoSaleStatus
        {
            [Description("不可售")]
            Forbid = 0,

            [Description("可售")]
            Permit = 1
        }

        /// <summary>
        /// 盘点计划单状态
        /// </summary>
        public enum SCPStatusEnum
        {
            //0-已创建，1-盘点执行中，2-盘点已完成
            [Description("初始态")]
            Initial = 0,

            [Description("执行中")]
            Executing = 1,

            [Description("已完成")]
            Finished = 2
        }

        /// <summary>
        /// 盘点明细表状态
        /// </summary>
        public enum SCDetailStatusEnum
        {
            [Description("未完成")]
            Uncompleted = 0,

            [Description("已完成")]
            Complete = 1
        }

        /// <summary>
        /// 盘点明细表状态
        /// </summary>
        public enum SCPTypeEnum
        {
            [Description("粗盘")]
            Simple = 0,

            [Description("细盘")]
            Complex = 1
        }

        /// <summary>
        /// 是否低于安全库存量，1低于，0不低于
        /// </summary>
        public enum IsUnderSafeLine
        {
            [Description("低于安全量")]
            Under = 1,

            [Description("不低于安全量")]
            NotUnder = 0,
        }

        /// <summary>
        /// 盘盈盘亏
        /// </summary>
        public enum IsProfitOrLoss 
        {
            [Description("盘盈")]
            Profit=1,

            [Description("盘亏")]
            Loss=0
        }

        /// <summary>
        /// 存货提货
        /// </summary>
        public enum PickOrStore 
        {
            [Description("提货")]
            Pick=1,

            [Description("存货")]
            Store=2
        }

        /// <summary>
        /// 将枚举类型转换成（名称-值）类的list
        /// </summary>
        /// <param name="typeOfEnum">枚举类型的type</param>
        /// <returns></returns>
        public static List<NameValueModel> GetEnumsList(Type typeOfEnum)
        {
            List<NameValueModel> modelList = new List<NameValueModel>();
            //System.Reflection.MemberInfo[] memberInfoArray;            
            System.Reflection.FieldInfo[] fieldInfoArray = typeOfEnum.GetFields();
            foreach (System.Reflection.FieldInfo field in fieldInfoArray)
            {
                if (field.FieldType.IsEnum)
                {
                    int filedValue = 0;
                    string filedName = "";
                    try
                    {
                        filedValue = (int)Enum.Parse(typeOfEnum, field.Name);
                    }
                    catch
                    {
                        continue;
                    }

                    DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (EnumAttributes.Length > 0)
                    {
                        filedName = EnumAttributes[0].DescriptionText;//使用描述
                    }
                    else
                        filedName = field.Name;//使用英文名

                    modelList.Add(new NameValueModel(filedName, filedValue));
                }
            }
            return modelList;
        }

        //获取枚举的Description特性值
        public static string GetEnumDesc(Enum e)
        {
            System.Reflection.FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.
                GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
            {
                return EnumAttributes[0].DescriptionText;
            }
            return e.ToString();
        }
    }

    /// <summary>
    /// 枚举值的名称描述类
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        private string descriptionText;
        public string DescriptionText
        {
            get { return descriptionText; }
            set { descriptionText = value; }
        }

        public DescriptionAttribute(string descriptionText)
        {
            this.descriptionText = descriptionText;
        }
    }

    /// <summary>
    /// 自定义键值对类(用于处理枚举名称-值对)
    /// </summary>
    public class NameValueModel
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public NameValueModel(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}