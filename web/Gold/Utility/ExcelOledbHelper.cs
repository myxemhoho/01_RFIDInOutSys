using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.OleDb;
using System.Data;

namespace Gold.Utility
{
    public class ExcelOledbHelper
    {

        /// <summary>
        /// 获取一个Excel中多个Sheet中的数据,每个Sheet数据存在于DataSet中的一个DataTable
        /// </summary>
        /// <param name="fileFullPathAndName">Excel文件的完整路径名称</param>
        /// <returns>返回DataSet</returns>
        public static bool GetOneExcelFileData( string fileFullPathAndName, out DataSet excelDataSet, out string errorMsg)
        {
            //open the excel file using OLEDB
            OleDbConnection con = null;
            excelDataSet = new DataSet();
            errorMsg = "";

            try
            {
                string connectionString = "";
                string[] workSheetNames = null;

                //if (isExcel2007VersionUp)
                //    //read a 2007 file
                //    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                //        fileFullPathAndName + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                //else
                //    //read a 97-2003 file
                //    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                //        fileFullPathAndName + ";Extended Properties=Excel 8.0;";

                //Microsoft.ACE.OLEDB.12.0可读2003和2007
                connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileFullPathAndName + ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1;\"";

                con = new OleDbConnection(connectionString);
                con.Open();

                #region 获取该Excel中所有Sheet名称

                //get all the available sheets
                System.Data.DataTable dataSet = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //System.Data.DataTable dataSet = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                //get the number of sheets in the file
                workSheetNames = new String[dataSet.Rows.Count];
                int i = 0;
                foreach (DataRow row in dataSet.Rows)
                {
                    //insert the sheet's name in the current element of the array
                    //and remove the $ sign at the end
                    workSheetNames[i] = row["TABLE_NAME"].ToString().Replace("$", "").Replace("'", "");//.Trim(new[] { '$' });//个人注释，Trim未起作用
                    i++;
                }

                #endregion

                #region 遍历Excel中所有Sheet并获取数据

                foreach (string worksheetName in workSheetNames)
                {
                    if (worksheetName.Contains("Print_Titles") == false && worksheetName.Contains("Print_Area") == false)//去掉Sheet中的打印标题和打印区域
                    {
                        OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [" + worksheetName + "$]", con);
                        DataTable dtOneSheet = new DataTable();
                        dtOneSheet.TableName = worksheetName;
                        cmd.Fill(dtOneSheet);
                        excelDataSet.Tables.Add(dtOneSheet.Copy());
                    }
                }

                #endregion

                return true;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        /// <summary>
        /// 根据Olddb连接字符串和Excel的Sheet名称查询该Sheet中的数据
        /// </summary>
        /// <param name="connectionString">Olddb连接字符串</param>
        /// <param name="worksheetName">Excel的Sheet名称</param>
        /// <returns></returns>
        public static DataTable GetWorksheet(string connectionString, string worksheetName)
        {
            try
            {
                using (OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString))
                {
                    OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [" + worksheetName + "$]", con);
                    con.Open();
                    System.Data.DataSet excelDataSet = new DataSet();
                    cmd.Fill(excelDataSet);
                    con.Close();
                    return excelDataSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}