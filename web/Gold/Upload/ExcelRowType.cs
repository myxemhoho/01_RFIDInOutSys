using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gold.Upload
{
    public enum ExcelRowType
    {
        /// <summary>
        /// 标题行
        /// </summary>
        Title,
        /// <summary>
        /// 抬头行
        /// </summary>
        Header,
        /// <summary>
        /// 行项目定义
        /// </summary>
        ItemDefine,
        /// <summary>
        /// 行项目
        /// </summary>
        Item,
        /// <summary>
        /// 抬尾行
        /// </summary>
        Footer,
        /// <summary>
        /// 空行
        /// </summary>
        Empty
    }

    public class ReadState
    {
        public bool TitleFound { get; set; }
        public bool HeaderFound { get; set; }
        public bool ItemDefineFound { get; set; }
        public bool ItemFound { get; set; }
        public bool FooterFound { get; set; }

        public ExcelRowType CalculateExpectExcelRowType()
        {
            if (FooterFound)
                return ExcelRowType.Footer;
            else if ((ItemDefineFound || ItemFound) && !FooterFound)
                return ExcelRowType.Item;
            else if ((TitleFound || HeaderFound) && !ItemDefineFound)
                return ExcelRowType.Header;
            else 
                return ExcelRowType.Title;
        }
    }
}
