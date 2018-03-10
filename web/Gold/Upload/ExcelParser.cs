using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Gold.Utility;


namespace Gold.Upload
{
    public class ExcelParser
    {
        /// <summary>
        /// 单据标题
        /// </summary>
        protected string _fileTitle;
        /// <summary>
        /// 行项目定义
        /// </summary>
        protected object[] _itemDefine;
        /// <summary>
        /// 循环周期
        /// </summary>
        protected int _cycle = 0;
        /// <summary>
        /// 当前行
        /// </summary>
        protected int _rowNum = 0;

        public virtual string Summary 
        {
            get { return string.Empty; } 
        }


        public ExcelParser(string fileName)
        {
            DataSet ds = new DataSet();
            string msg;
            if (ExcelOledbHelper.GetOneExcelFileData(fileName, out ds, out msg))
            {
                foreach (DataTable dt in ds.Tables)
                {                    
                    if (fileName.IndexOf("调拨订单") != -1)
                    {
                        LoadTransferData(dt);//加载调拨订单数据
                    }
                    else
                    {
                        LoadData(dt);                        
                    }
                    
                    break;
                }
            }
            else
            {
                throw new ApplicationException(msg);
            }
        }

        protected void LoadData(DataTable dt)
        {
            ReadState state = new ReadState();

            foreach (DataRow dr in dt.Rows)
            {
                _rowNum++;
                ParsingRow(dr.ItemArray, ref state);
            }
        }

        protected void ParsingRow(object[] array, ref ReadState state)
        {
            string preValue = "";
            for (int i = 0; i < array.Length; i++)
            {
                string value = Regex.Replace(array[i].ToString(), @"\s", "");
                if (string.IsNullOrEmpty(value))
                    continue;

                ExcelRowType expectType = state.CalculateExpectExcelRowType();

                if (value == preValue && expectType != ExcelRowType.Item)
                    continue;

                if (expectType == ExcelRowType.Title)
                {
                    if (value.EndsWith("单"))
                    {
                        VerifyFileTitle(value);
                        _fileTitle = value;
                        state.TitleFound = true;
                    }
                    preValue = value;
                }
                else if (expectType == ExcelRowType.Header)
                {
                    if (value.EndsWith("："))
                    {
                        if (_cycle > 0)
                            return;

                        if (i == (array.Length-1))
                            return;

                        string fieldName = Regex.Replace(value, "：", "");
                        string nextValue = array[++i].ToString(); //Regex.Replace(array[++i].ToString(), @"\s", "");
                        if (string.IsNullOrEmpty(nextValue))
                            continue;
                        else if (nextValue.Contains("："))
                        {
                            i--;
                            continue;
                        }
                        else
                        {
                            string fieldValue = nextValue;
                            BindingOrderHeader(fieldName, fieldValue);
                            preValue = nextValue;
                        }
                    }
                    else if (value.Contains("："))
                    {
                        if (_cycle > 0)
                            return;

                        string[] kv = Regex.Split(value, "：");
                        if (kv.Length > 1)
                            BindingOrderHeader(kv[0], kv[1]);
                    }
                    else if (i == 0)
                    {
                        state.ItemDefineFound = true;
                        _itemDefine = array;
                        return;
                    }

                    preValue = value;
                }
                else if (expectType == ExcelRowType.Item)
                {
                    if (value.Contains("合计") || value.Contains("："))
                    {
                        state.FooterFound = true;
                        ParsingRow(array, ref state);
                    }
                    else
                        BindingOrderItem(array);

                    return;
                }
                else if (expectType == ExcelRowType.Footer)
                {
                    if (value.EndsWith("："))
                    {
                        if (_cycle > 0)
                            return;

                        if (i == (array.Length - 1))
                            return;

                        string fieldName = Regex.Replace(value, "：", "");
                        string nextValue = array[++i].ToString();// Regex.Replace(array[++i].ToString(), @"\s", "");
                        if (string.IsNullOrEmpty(nextValue))
                            continue;
                        else if (nextValue.Contains("："))
                        {
                            i--;
                            continue;
                        }
                        else
                        {
                            string fieldValue = nextValue;
                            BindingOrderHeader(fieldName, fieldValue);
                            preValue = nextValue;
                        }
                    }
                    else if (value.Contains("："))
                    {
                        if (_cycle > 0)
                            return;

                        string[] kv = Regex.Split(value, "：");
                        if (kv.Length > 1)
                            BindingOrderHeader(kv[0], kv[1]);
                    }
                    else if (value == _fileTitle)
                    {
                        _cycle++;
                        state.FooterFound = false;
                        state.ItemDefineFound = false;
                        state.TitleFound = true;
                    }
                    else if (i == 0)
                    {
                        return;
                    }

                    preValue = value;
                }
            }
        }

        //加载调拨订单数据
        protected void LoadTransferData(DataTable dt)
        {
            ReadState state = new ReadState();

            foreach (DataRow dr in dt.Rows)
            {
                _rowNum++;
                ParsingTransferRow(dr.ItemArray, ref state);
            }
        }

        //解析调拨订单excel的行项目
        protected void ParsingTransferRow(object[] array, ref ReadState state)
        {
            string preValue = "";
            for (int i = 0; i < array.Length; i++)
            {
                string value = Regex.Replace(array[i].ToString(), @"\s", "");
                if (string.IsNullOrEmpty(value) || value.ToString() == "1" )
                    continue;               

                ExcelRowType expectType = state.CalculateExpectExcelRowType();
               
                if (value == preValue && expectType != ExcelRowType.Item)
                    continue;

                if (expectType == ExcelRowType.Title)
                { 
                    if (value.EndsWith("单"))
                    {
                        VerifyFileTitle(value);
                        _fileTitle = value;
                        state.TitleFound = true;
                    }
                    preValue = value;
                }
                //抬头
                else if (expectType == ExcelRowType.Header)
                {
                    if (value.Contains("行号"))
                    {
                        state.ItemDefineFound = true;
                        _itemDefine = array;
                        return;
                    }

                    if (_cycle > 0)
                        return;

                    if (i == (array.Length - 1))
                        return;

                    string fieldName = value;
                    string nextValue = array[++i].ToString();
                    if (string.IsNullOrEmpty(nextValue))
                        continue;                    
                    else
                    {
                        string fieldValue = nextValue;
                        BindingOrderHeader(fieldName, fieldValue);
                        preValue = nextValue;
                    } 
                   preValue = value;
                }
                //行项目
                else if (expectType == ExcelRowType.Item)
                {
                    if (value.Contains("制单人"))
                    {
                        state.FooterFound = true;
                        ParsingTransferRow(array, ref state);
                    }
                    else
                        BindingOrderItem(array);

                    return;
                }
                //底部
                else if (expectType == ExcelRowType.Footer)
                {                       
                    if (_cycle > 0)
                        return;

                    if (i == (array.Length - 1))
                        return;

                    string fieldName = value;
                    string nextValue = array[++i].ToString();
                    if (string.IsNullOrEmpty(nextValue))
                        continue;                        
                    else
                    {
                        string fieldValue = nextValue;
                        BindingOrderHeader(fieldName, fieldValue);
                        preValue = nextValue;
                    }
                   
                    preValue = value;

                    if (value == _fileTitle)
                    {
                        _cycle++;
                        state.FooterFound = false;
                        state.ItemDefineFound = false;
                        state.TitleFound = true;
                    }
                }
            }
        }

        protected virtual void VerifyFileTitle(string title)
        {
        }

        protected virtual void BindingOrderHeader(string fieldName, string fieldValue)
        {
        }

        protected virtual void BindingOrderItem(object[] array)
        {
        }

        public virtual string SaveData()
        {
            return string.Empty;
        }
    }

}
