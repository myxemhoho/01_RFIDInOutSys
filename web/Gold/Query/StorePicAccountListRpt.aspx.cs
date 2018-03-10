using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gold.DAL;
using Microsoft.Reporting.WebForms;
using Gold.Utility;

namespace Gold.Query
{
    public partial class StorePicAccountListRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    /*System.Text.StringBuilder url = new System.Text.StringBuilder("~/Query/StorePicAccountListRpt.aspx?");
            //url.Append("displayPattern=");
            //url.Append(displayPattern);
            url.Append("cargoCode=");
            url.Append(cargoCode);
            url.Append("&cargoName=");
            url.Append(cargoName);
            //url.Append("&modelName=");
            //url.Append(modelName);
            //url.Append("&specName=");
            //url.Append(specName);
            url.Append("&whName=");
            url.Append(whName);
            url.Append("&whCode=");
            url.Append(whCode);
            url.Append("&startDate=");
            url.Append(startDate);
            url.Append("&endDate=");
            url.Append(endDate);
            url.Append("&sortExpression=");
            url.Append(sortExpression);
            url.Append("&sortDirection=");
            url.Append(sortDirection);
            url.Append("&packageShareNo=");
            url.Append(packageShareNo);
            url.Append("&storePickType=");
            url.Append(storePickType);
            url.Append("&isProfitOrLoss=");
            url.Append(isProfitOrLoss);
            url.Append("&packageName=");
            url.Append(packageName);*/

                    //string displayPattern = Request.QueryString["displayPattern"].ToString().Trim();// "ByCargo" : "ByWHCode";
                    string cargoCode = Request.QueryString["cargoCode"].ToString().Trim();
                    string cargoName = Request.QueryString["cargoName"].ToString().Trim();
                    //string modelName = Request.QueryString["modelName"].ToString().Trim();
                    //string specName = Request.QueryString["specName"].ToString().Trim();
                    string whName = Request.QueryString["whName"].ToString().Trim();
                    string whCode = Request.QueryString["whCode"].ToString().Trim();
                    string startDate = Request.QueryString["startDate"].ToString().Trim();
                    string endDate = Request.QueryString["endDate"].ToString().Trim();
                    string sortExpression = Request.QueryString["sortExpression"].ToString().Trim();
                    string sortDirection = Request.QueryString["sortDirection"].ToString().Trim();

                    string packageShareNo = Request.QueryString["packageShareNo"].ToString().Trim();
                    string storePickType = Request.QueryString["storePickType"].ToString().Trim();
                    string isProfitOrLoss = Request.QueryString["isProfitOrLoss"].ToString().Trim();
                    string packageName = Request.QueryString["packageName"].ToString().Trim();

                    //if (displayPattern == "ByCargo")
                    //{
                    LoadByCargo(whCode, whName, cargoCode, cargoName, packageShareNo, storePickType, isProfitOrLoss, packageName, startDate, endDate, sortExpression, sortDirection);
                    //}
                    //else if (displayPattern == "ByWHCode")
                    //{
                    //    LoadByWHCode(displayPattern, whName, whCode, cargoCode, cargoName, modelName, specName, startDate, endDate, sortExpression, sortDirection);
                    //}
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        void LoadByCargo(string whCode, string whName, string cargoCode, string cargoName,
            string packageShareNo, string storePickType,
            string isProfitOrLoss, string packageName,
            string startDateStr, string endDateStr, string sortExpression, string sortDirectionStr)
        {
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

            using (DAL.GoldEntities context = new DAL.GoldEntities())
            {

                ReportViewer1.LocalReport.ReportPath = @"Reports\Report_StorePicAccountList.rdlc";

                List<proc_WebSelectStorePickAccount_Result> queryResult = context.proc_WebSelectStorePickAccount(whCode, cargoCode, cargoName, storePickType, isProfitOrLoss, packageName, packageShareNo, startTime.ToString(), endTime.ToString()).ToList<proc_WebSelectStorePickAccount_Result>();

                foreach (proc_WebSelectStorePickAccount_Result p in queryResult)
                {
                    if (p.StorePickType == ((int)EnumData.PickOrStore.Pick).ToString())
                    {
                        p.StorePickType = EnumData.GetEnumDesc(EnumData.PickOrStore.Pick);
                    }
                    else if (p.StorePickType == ((int)EnumData.PickOrStore.Store).ToString())
                    {
                        p.StorePickType = EnumData.GetEnumDesc(EnumData.PickOrStore.Store);
                    }
                }

                //string sortExpression = gv_CargoList.Attributes["sortExpression"];
                SortDirection sortDirection = sortExpression == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                if (sortDirection == SortDirection.Ascending)
                    queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                else
                    queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();



                ReportDataSource rds = new ReportDataSource("DataSet_StorePickAccount", queryResult);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);

                /*
                 RP_CargoCode
RP_CargoName
RP_CargoModel
RP_CargoSpec
RP_CargoUnits
RP_DisplayPattern
RP_WHName
RP_StartDate
RP_EndDate
RP_TotalSumNumCurrent
RP_TotalFirstOriginalNum
RP_TotalSumNumAdd
RP_TotalSumDel
                 */
                ReportParameter RP_CargoCode = new ReportParameter("RP_CargoCode", cargoCode);
                ReportParameter RP_CargoName = new ReportParameter("RP_CargoName", cargoName);
                //ReportParameter RP_CargoModel = new ReportParameter("RP_CargoModel", modelName);
                //ReportParameter RP_CargoSpec = new ReportParameter("RP_CargoSpec", specName);
                //ReportParameter RP_CargoUnits = new ReportParameter("RP_CargoUnits", "");
                //ReportParameter RP_DisplayPattern = new ReportParameter("RP_DisplayPattern", (displayPattern == "ByCargo" ? "按商品汇总" : "按仓库汇总"));
                ReportParameter RP_WHName = new ReportParameter("RP_WHName", whName);
                ReportParameter RP_WHCode = new ReportParameter("RP_WHCode", whCode);
                ReportParameter RP_StartDate = new ReportParameter("RP_StartDate", startDateStr);
                ReportParameter RP_EndDate = new ReportParameter("RP_EndDate", endDateStr);

                string storePickTypeName = "";
                if (storePickType == ((int)EnumData.PickOrStore.Pick).ToString())
                {
                    storePickTypeName = EnumData.GetEnumDesc(EnumData.PickOrStore.Pick);
                }
                else if (storePickType == ((int)EnumData.PickOrStore.Store).ToString())
                {
                    storePickTypeName = EnumData.GetEnumDesc(EnumData.PickOrStore.Store);
                }

                ReportParameter RP_StorePickType = new ReportParameter("RP_StorePickType", storePickTypeName);
                ReportParameter RP_PackageShareNo = new ReportParameter("RP_PackageShareNo", packageShareNo);
                ReportParameter RP_IsProfitOrLoss = new ReportParameter("RP_IsProfitOrLoss", isProfitOrLoss);
                ReportParameter RP_PackageName = new ReportParameter("RP_PackageName", packageName);

                ReportParameterCollection rpList = new ReportParameterCollection();
                rpList.Add(RP_CargoCode);
                rpList.Add(RP_CargoName);
                //rpList.Add(RP_CargoModel);
                //rpList.Add(RP_CargoSpec);
                //rpList.Add(RP_CargoUnits);
                //rpList.Add(RP_DisplayPattern);
                rpList.Add(RP_WHName);
                rpList.Add(RP_WHCode);
                rpList.Add(RP_StartDate);
                rpList.Add(RP_EndDate);
                rpList.Add(RP_StorePickType);
                rpList.Add(RP_PackageShareNo);
                rpList.Add(RP_IsProfitOrLoss);
                rpList.Add(RP_PackageName);


                ReportViewer1.LocalReport.SetParameters(rpList);
                ReportViewer1.DataBind();
            }
        }

        //void LoadByWHCode(string displayPattern, string whName, string whCode, string cargoCode, string cargoName, string modelName, string specName, string startDateStr, string endDateStr, string sortExpression, string sortDirectionStr)
        //{
        //    DateTime startDate = DateTime.Parse(startDateStr);
        //    DateTime endDate = DateTime.Parse(endDateStr);
        //    DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
        //    DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);

        //    using (DAL.GoldEntities context = new DAL.GoldEntities())
        //    {
        //        double totalFirstOriginalNum = 0;
        //        double totalNumAdd = 0;
        //        double totalNumDel = 0;
        //        double totalNumCurrent = 0;

        //        ReportViewer1.LocalReport.ReportPath = @"Reports\Report_InOutCollectByWHCode.rdlc";

        //        List<proc_WebSelectInOutCollectByWHCode_Result> queryResult = context.proc_WebSelectInOutCollectByWHCode(whCode, cargoCode, cargoName, modelName, specName, "", startTime, endTime).ToList<proc_WebSelectInOutCollectByWHCode_Result>();

        //        //string sortExpression = gv_CargoList.Attributes["sortExpression"];
        //        SortDirection sortDirection = sortDirectionStr == "ASC" ? SortDirection.Ascending : SortDirection.Descending;//gv_CargoList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

        //        if (sortDirection == SortDirection.Ascending)
        //            queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
        //        else
        //            queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

        //        totalFirstOriginalNum = queryResult.Sum(r => r.FirstOrignialNum).Value;
        //        totalNumAdd = queryResult.Sum(r => r.SumNumAdd).Value;
        //        totalNumDel = queryResult.Sum(r => r.SumNumDel).Value;
        //        totalNumCurrent = queryResult.Sum(r => r.SumNumCurrent).Value;

        //        ReportDataSource rds = new ReportDataSource("DataSet_InOutCollectByWHCode", queryResult);
        //        ReportViewer1.LocalReport.DataSources.Clear();
        //        ReportViewer1.LocalReport.DataSources.Add(rds);


        //        ReportParameter RP_CargoCode = new ReportParameter("RP_CargoCode", cargoCode);
        //        ReportParameter RP_CargoName = new ReportParameter("RP_CargoName", cargoName);
        //        ReportParameter RP_CargoModel = new ReportParameter("RP_CargoModel", modelName);
        //        ReportParameter RP_CargoSpec = new ReportParameter("RP_CargoSpec", specName);
        //        ReportParameter RP_CargoUnits = new ReportParameter("RP_CargoUnits", "");
        //        ReportParameter RP_DisplayPattern = new ReportParameter("RP_DisplayPattern", (displayPattern == "ByCargo" ? "按商品汇总" : "按仓库汇总"));
        //        ReportParameter RP_WHName = new ReportParameter("RP_WHName", whName);
        //        ReportParameter RP_StartDate = new ReportParameter("RP_StartDate", startDateStr);
        //        ReportParameter RP_EndDate = new ReportParameter("RP_EndDate", endDateStr);
        //        ReportParameter RP_TotalFirstOriginalNum = new ReportParameter("RP_TotalFirstOriginalNum", totalFirstOriginalNum.ToString());
        //        ReportParameter RP_TotalSumNumAdd = new ReportParameter("RP_TotalSumNumAdd", totalNumAdd.ToString());
        //        ReportParameter RP_TotalSumDel = new ReportParameter("RP_TotalSumDel", totalNumDel.ToString());
        //        ReportParameter RP_TotalSumNumCurrent = new ReportParameter("RP_TotalSumNumCurrent", totalNumCurrent.ToString());

        //        ReportParameterCollection rpList = new ReportParameterCollection();
        //        rpList.Add(RP_CargoCode);
        //        rpList.Add(RP_CargoName);
        //        rpList.Add(RP_CargoModel);
        //        rpList.Add(RP_CargoSpec);
        //        rpList.Add(RP_CargoUnits);
        //        rpList.Add(RP_DisplayPattern);
        //        rpList.Add(RP_WHName);
        //        rpList.Add(RP_StartDate);
        //        rpList.Add(RP_EndDate);
        //        rpList.Add(RP_TotalFirstOriginalNum);
        //        rpList.Add(RP_TotalSumNumAdd);
        //        rpList.Add(RP_TotalSumDel);
        //        rpList.Add(RP_TotalSumNumCurrent);

        //        ReportViewer1.LocalReport.SetParameters(rpList);
        //        ReportViewer1.DataBind();
        //    }
        //}
    }
}