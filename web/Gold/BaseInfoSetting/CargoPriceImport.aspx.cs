using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Gold.DAL;
using Gold.Utility;

namespace Gold.BaseInfoSetting
{
    public partial class CargoPriceImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 导入系统前在本页面预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportPreview_Click(object sender, EventArgs e)
        {
            lblPreviewResult.Text = "";
            strStatus.Text = "";
            lblSaveMsg.Text = "";

            //获取上传的完整文件名
            List<string> fileNameList = new List<string>();
            List<bool> isExcel2007List = new List<bool>();

            if (Cache["ExcelCargoList"] != null)
                Cache.Remove("ExcelCargoList");//清空界面缓存数据

            ///'遍历File表单元素
            HttpFileCollection files = HttpContext.Current.Request.Files;
            /// '状态信息
            System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
            strMsg.Append("选择的有效文件分别是：<br>");
            try
            {
                for (int iFile = 0; iFile < files.Count; iFile++)
                {
                    ///'检查文件扩展名字
                    HttpPostedFile postedFile = files[iFile];
                    string fileName, fileExtension;
                    fileName = System.IO.Path.GetFileName(postedFile.FileName);
                    if (fileName != "")
                    {
                        fileExtension = System.IO.Path.GetExtension(fileName);

                        if (fileExtension.Trim() == ".xls")
                        {
                            //fileNameList.Add(postedFile.FileName);
                            //isExcel2007List.Add(false);

                            //strMsg.Append("上传的文件类型：" + postedFile.ContentType.ToString() + "<br>");
                            strMsg.Append("&nbsp;&nbsp;文件地址及名称：" + postedFile.FileName + "<br>");
                            //strMsg.Append("上传文件的文件名：" + fileName + "<br>");
                            //strMsg.Append("上传文件的扩展名：" + fileExtension + "<br><hr>");

                            if (postedFile.ContentLength > 4 * 1024 * 1024) //最大文件为4Mb，即4*1024Kb
                            {
                                lblPreviewResult.Text = "选择的文件中[" + postedFile.FileName + "]超过4M，禁止上传！";
                                return;
                            }
                        }
                        else if (fileExtension.Trim() == ".xlsx")
                        {
                            //fileNameList.Add(postedFile.FileName);
                            isExcel2007List.Add(true);

                            //strMsg.Append("上传的文件类型：" + postedFile.ContentType.ToString() + "<br>");
                            strMsg.Append("&nbsp;&nbsp;文件地址及名称：" + postedFile.FileName + "<br>");
                            //strMsg.Append("上传文件的文件名：" + fileName + "<br>");
                            //strMsg.Append("上传文件的扩展名：" + fileExtension + "<br><hr>");
                            if (postedFile.ContentLength > 4 * 1024 * 1024) //最大文件为4M
                            {
                                lblPreviewResult.Text = "选择的文件中[" + postedFile.FileName + "]超过4M，禁止上传！";
                                return;
                            }
                        }

                        ///'可根据扩展名字的不同保存到不同的文件夹
                        ///注意：可能要修改你的文件夹的匿名写入权限。
                        postedFile.SaveAs(System.Web.HttpContext.Current.Request.MapPath("UploadFiles/") + fileName);
                        fileNameList.Add(System.Web.HttpContext.Current.Request.MapPath("UploadFiles/") + fileName);
                    }
                }

                if (fileNameList.Count == 0)
                {
                    lblPreviewResult.Text = "请先选择Excel文件(仅Excel文件可进行预览和导入)，然后点击“导入前预览”!";
                    return;
                }

                GetExcelDataAndBindData(fileNameList, isExcel2007List);

                strStatus.Text = strMsg.ToString();
                return;
            }
            catch (System.Exception Ex)
            {
                strStatus.Text = Ex.Message;
                return;
            }
        }

        /// <summary>
        /// 从Excel中获取数据
        /// </summary>
        /// <param name="fileNameList"></param>
        /// <param name="isExcel2007"></param>
        private void GetExcelDataAndBindData(List<string> fileNameList, List<bool> isExcel2007)
        {
            List<DataSet> allFileData = new List<DataSet>();//每个Excel的数据存储在一个DataSet中
            List<bool> fileGetResult = new List<bool>();
            List<bool> fileGetErrorMsg = new List<bool>();
            for (int i = 0; i < fileNameList.Count; i++)
            {
                DataSet tempDs = new DataSet();
                string errorMsg = "";
                bool getResult = ExcelOledbHelper.GetOneExcelFileData(fileNameList[i], out tempDs, out errorMsg);
                if (getResult == false)
                {
                    lblPreviewResult.Text = "预览数据加载失败。原因：" + errorMsg;
                    return;
                }
                if (tempDs.Tables.Count > 0)
                    allFileData.Add(tempDs);
            }
            List<DAL.Cargos> excelCargoList = new List<DAL.Cargos>();
            string errorFilterMsg = "";
            bool filterResult = FilterData(allFileData, out excelCargoList, out errorFilterMsg);
            if (filterResult == false)
            {
                lblPreviewResult.Text = "预览数据加载失败。原因：" + errorFilterMsg;
            }
            else
            {
                lblPreviewResult.Text = "预览数据加载成功(共加载" + excelCargoList.Count.ToString() + "条数据)。请检查下面的预览数据，检查无误后请点击“导入到系统”按钮，将数据导入！";

                Cache.Insert("ExcelCargoList", excelCargoList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);//将数据放入缓存中,2小时后缓存自动过期

                gv_CargoList_Preview.DataSource = excelCargoList;
                gv_CargoList_Preview.DataBind();
            }

        }

        /// <summary>
        /// 按特定格式将各个DataSet中的数据合并到List中
        /// </summary>
        /// <param name="dsList"></param>
        /// <param name="cargoList"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool FilterData(List<DataSet> dsList, out List<DAL.Cargos> cargoList, out string errorMsg)
        {
            errorMsg = "";
            //存货名称	存货编码	规格	型号	批发价	会员价（金卡）	会员价（银卡）	会员价（白金卡）	参考售价


            cargoList = new List<DAL.Cargos>();

            try
            {
                foreach (DataSet ds in dsList)
                {
                    foreach (DataTable dtSheet in ds.Tables)
                    {
                        if (dtSheet.Rows.Count == 0)
                            continue;

                        if (dtSheet.Columns.Count <= 1)
                        {
                            errorMsg = "文件中列不足，请检查是否为有效数据文件！";
                            continue;
                        }

                        //标题行索引和数据行索引
                        int headRowIndex = -1, dataRowIndex = -1;

                        //是否首行就是标题行，当首行是标题行时，无论设置hdr为何值，oledb不再将其读取为数据行
                        bool isHeaderInFirstRow = false;

                        if (dtSheet.Columns.Contains("存货名称") && dtSheet.Columns.Contains("存货编码"))
                            isHeaderInFirstRow = true;

                        //找出标题列索引与字段名映射
                        Dictionary<string, int> ColNameIndexDic = new Dictionary<string, int>();

                        //如果首行不是标题行
                        if (isHeaderInFirstRow == false)
                        {
                            //找出标题行索引和数据行索引
                            for (int i = 0; i < dtSheet.Rows.Count; i++)
                            {
                                for (int j = 0; j < dtSheet.Columns.Count; j++)
                                {
                                    if (dtSheet.Columns.Count > 0 && dtSheet.Rows[i][j].ToString() == "存货名称")
                                    {
                                        headRowIndex = i;
                                        if (i + 1 < dtSheet.Rows.Count)
                                            dataRowIndex = i + 1;
                                    }
                                }
                            }

                            if (headRowIndex == -1 || dataRowIndex == -1)
                            {
                                errorMsg = "文件中未包含“存货名称”、“存货编码”等标题行和数据行";
                                return false;
                            }

                            //找出标题列索引与字段名映射                        
                            for (int i = 0; i < dtSheet.Columns.Count; i++)
                            {
                                if (headRowIndex != -1)
                                {
                                    //存货名称	存货编码	规格	型号	批发价	会员价（金卡）	会员价（银卡）	会员价（白金卡）	参考售价
                                    switch (dtSheet.Rows[headRowIndex][i].ToString().Trim())
                                    {
                                        case "存货名称":
                                            ColNameIndexDic.Add("存货名称", i); break;
                                        case "存货编码":
                                            ColNameIndexDic.Add("存货编码", i); break;
                                        case "型号":
                                            ColNameIndexDic.Add("型号", i); break;
                                        case "规格":
                                            ColNameIndexDic.Add("规格", i); break;
                                        case "批发价":
                                            ColNameIndexDic.Add("批发价", i); break;
                                        case "会员价（金卡）":
                                            ColNameIndexDic.Add("会员价（金卡）", i); break;
                                        case "会员价（银卡）":
                                            ColNameIndexDic.Add("会员价（银卡）", i); break;
                                        case "会员价（白金卡）":
                                            ColNameIndexDic.Add("会员价（白金卡）", i); break;
                                        case "参考售价":
                                            ColNameIndexDic.Add("参考售价", i); break;
                                    }
                                }
                            }
                        }
                        else //如果首行是标题行
                        {
                            dataRowIndex = 0;
                            //找出标题列索引与字段名映射         
                            for (int i = 0; i < dtSheet.Columns.Count; i++)
                            {
                                switch (dtSheet.Columns[i].ColumnName.ToString().Trim())
                                {
                                    case "存货名称":
                                        ColNameIndexDic.Add("存货名称", i); break;
                                    case "存货编码":
                                        ColNameIndexDic.Add("存货编码", i); break;
                                    case "型号":
                                        ColNameIndexDic.Add("型号", i); break;
                                    case "规格":
                                        ColNameIndexDic.Add("规格", i); break;
                                    case "批发价":
                                        ColNameIndexDic.Add("批发价", i); break;
                                    case "会员价（金卡）":
                                        ColNameIndexDic.Add("会员价（金卡）", i); break;
                                    case "会员价（银卡）":
                                        ColNameIndexDic.Add("会员价（银卡）", i); break;
                                    case "会员价（白金卡）":
                                        ColNameIndexDic.Add("会员价（白金卡）", i); break;
                                    case "参考售价":
                                        ColNameIndexDic.Add("参考售价", i); break;
                                }
                            }
                        }


                        string[] headerColumnNames = { "存货名称", "存货编码", "型号", "规格", "批发价", "会员价（金卡）", "会员价（银卡）", "会员价（白金卡）", "参考售价" };
                        System.Text.StringBuilder strNoExistHeader = new System.Text.StringBuilder();
                        foreach (string header in headerColumnNames)
                        {
                            if (ColNameIndexDic.Keys.Contains(header) == false)
                            {
                                if (strNoExistHeader.Length > 0)
                                    strNoExistHeader.Append(",");
                                strNoExistHeader.Append(header);
                            }
                        }
                        if (strNoExistHeader.Length > 0)
                        {
                            errorMsg = "选择的Excel文件中不包含以下标题行和数据列（" + strNoExistHeader.ToString() + "）,请检查上传的文件是否正确！";
                            return false;
                        }

                        for (int i = dataRowIndex; i < dtSheet.Rows.Count; i++)
                        {
                            DataRow dr = dtSheet.Rows[i];
                            if (dr[ColNameIndexDic["存货名称"]].ToString().Trim() != "" &&
                                dr[ColNameIndexDic["存货编码"]].ToString().Trim() != "")
                            {
                                DAL.Cargos newCargo = new DAL.Cargos();
                                newCargo.CargoCode = ConvertNull(dr[ColNameIndexDic["存货编码"]]);

                                var existSameCodeModel = (from r in cargoList where r.CargoCode == newCargo.CargoCode select r).FirstOrDefault();
                                if (existSameCodeModel != null)//如果缓存中已经存在相同编号的商品则跳过
                                {
                                    continue;
                                }

                                newCargo.CargoModel = ConvertNull(dr[ColNameIndexDic["型号"]]);
                                newCargo.CargoName = ConvertNull(dr[ColNameIndexDic["存货名称"]]);
                                newCargo.CargoSpec = ConvertNull(dr[ColNameIndexDic["规格"]]);
                                newCargo.CargoType = 1;

                                // price1，price2，price3，price4，price5
                                //	批发价	会员价（金卡）	会员价（银卡）	会员价（白金卡）	参考售价

                                newCargo.Price1 = ConvertDecimal(dr[ColNameIndexDic["批发价"]]);
                                newCargo.Price2 = ConvertDecimal(dr[ColNameIndexDic["会员价（金卡）"]]);
                                newCargo.Price3 = ConvertDecimal(dr[ColNameIndexDic["会员价（银卡）"]]);
                                newCargo.Price4 = ConvertDecimal(dr[ColNameIndexDic["会员价（白金卡）"]]);
                                newCargo.Price5 = ConvertDecimal(dr[ColNameIndexDic["参考售价"]]);

                                newCargo.SaleStatus = null;
                                newCargo.Total = null;
                                newCargo.Variation = null;

                                cargoList.Add(newCargo);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 导入到系统中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportToSystem_Click(object sender, EventArgs e)
        {
            try
            {
                lblSaveMsg.Text = "";
                List<Cargos> excelCargoList = null;
                if (Cache["ExcelCargoList"] != null)
                {
                    excelCargoList = (List<Cargos>)Cache["ExcelCargoList"];
                }
                if (excelCargoList == null || excelCargoList.Count == 0)
                {
                    lblSaveMsg.Text = "未选择文件或缓存过期！请先选择Excel并点击“导入前预览”，然后点击“导入到系统”!";
                    return;
                }
                else
                {

                    int insertCount = 0;//本次新增
                    int updateCount = 0;//本次更新
                    int affectRowsCount = 0;
                    //在本次导入过程中已经新增过的型号
                    List<string> TheTimeModelNameAddedList = new List<string>();
                    //在本次导入过程中已经新增过的规格
                    List<string> TheTimeSpecNameAddedList = new List<string>();

                    using (Gold.DAL.GoldEntities context = new DAL.GoldEntities())
                    {
                        foreach (Cargos newCargos in excelCargoList)
                        {
                            //自动从excel中新增型号
                            var existModel = (from r in context.Models where (r.ModelName.Trim() == newCargos.CargoModel && newCargos.CargoModel.Trim() != "") select r).FirstOrDefault();
                            if (existModel == null && newCargos.CargoModel.Trim() != "")
                            {
                                if (TheTimeModelNameAddedList.Contains(newCargos.CargoModel) == false)
                                {
                                    Models newModel = new Models();
                                    newModel.ModelName = newCargos.CargoModel;
                                    context.Models.AddObject(newModel);

                                    TheTimeModelNameAddedList.Add(newCargos.CargoModel);//记录到临时缓存中
                                }
                            }

                            //自动从excel中新增规格
                            var existSpec = (from r in context.Specifications where (r.SpecName.Trim() == newCargos.CargoSpec && newCargos.CargoSpec.Trim() != "") select r).FirstOrDefault();
                            if (existSpec == null && newCargos.CargoSpec.Trim() != "")
                            {
                                if (TheTimeSpecNameAddedList.Contains(newCargos.CargoSpec) == false)
                                {
                                    Specifications newSpec = new Specifications();
                                    newSpec.SpecName = newCargos.CargoSpec;
                                    context.Specifications.AddObject(newSpec);

                                    TheTimeSpecNameAddedList.Add(newCargos.CargoSpec);//记录到临时缓存中
                                }
                            }

                            //判断是否已经存在该商品
                            Cargos existSameCodeCargo = (from r in context.Cargos where (r.CargoCode == newCargos.CargoCode && newCargos.CargoCode.Trim() != "") select r).FirstOrDefault<Cargos>();
                            if (existSameCodeCargo == null)//新增 
                            {
                                context.Cargos.AddObject(newCargos);

                                insertCount++;
                            }
                            else//更新 
                            {
                                existSameCodeCargo.CargoName = newCargos.CargoName;
                                existSameCodeCargo.CargoModel = newCargos.CargoModel;
                                existSameCodeCargo.CargoSpec = newCargos.CargoSpec;
                                existSameCodeCargo.Price1 = newCargos.Price1;
                                existSameCodeCargo.Price2 = newCargos.Price2;
                                existSameCodeCargo.Price3 = newCargos.Price3;
                                existSameCodeCargo.Price4 = newCargos.Price4;
                                existSameCodeCargo.Price5 = newCargos.Price5;


                                updateCount++;
                            }
                        }

                        affectRowsCount = context.SaveChanges();

                        if (affectRowsCount > 0)
                        {
                            lblSaveMsg.Text = "导入成功！（共操作" + excelCargoList.Count.ToString() + "条记录,其中新增" + insertCount.ToString() + "条，更新" + updateCount.ToString() + "条）";
                        }
                        else
                        {
                            lblSaveMsg.Text = "导入失败！请重试!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblSaveMsg.Text = "导入失败！原因：" + ex.Message + "<br>" + (ex.InnerException == null ? "" : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// 将Null转换为""
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        string ConvertNull(object objValue)
        {
            if (objValue == null)
                return null;
            else
                return objValue.ToString();
        }

        /// <summary>
        /// 将Excel单元格数据转换成可空类型的decimal
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        decimal? ConvertDecimal(object objValue)
        {
            if (objValue == null)
                return null;
            else
            {
                decimal result = 0;
                if (decimal.TryParse(objValue.ToString(), out result))
                    return result;
                else
                    return null;
            }
        }


    }
}