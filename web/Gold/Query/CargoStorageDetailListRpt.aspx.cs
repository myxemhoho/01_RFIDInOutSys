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
    public partial class CargoStorageDetailListRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string cargoCode = Request.QueryString["cargoCode"].ToString().Trim();
                    string cargoName = Request.QueryString["cargoName"].ToString().Trim();
                    string modelName = Request.QueryString["modelName"].ToString().Trim();
                    string specName = Request.QueryString["specName"].ToString().Trim();
                    string whName = Request.QueryString["whName"].ToString().Trim();
                    string whCode = Request.QueryString["whCode"].ToString().Trim();
                    int QueryMode = -1;
                    if (int.TryParse(Request.QueryString["QueryMode"].ToString().Trim(), out QueryMode) == false)
                        QueryMode = 0;
                    string binCode = Request.QueryString["binCode"].ToString().Trim();


                    string sortExpression = Request.QueryString["sortExpression"].ToString().Trim();
                    string sortDirection = Request.QueryString["sortDirection"].ToString().Trim();



                    LoadByCargo(whCode, whName, cargoCode, cargoName, modelName, specName, "", QueryMode, binCode, sortExpression, sortDirection);

                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        void LoadByCargo(string whCode, string whName, string cargoCode, string cargoName, string cargoModel, string cargoSpec, string cargoUnits, int queryMode, string binCode, string sortExpression, string sortDirectionStr)
        {
            using (DAL.GoldEntities context = new DAL.GoldEntities())
            {

                ReportViewer1.LocalReport.ReportPath = @"Reports\Report_CargoStorageDetailList.rdlc";

                List<proc_WebSelectCargoStorageDetail_Result> queryResult = context.proc_WebSelectCargoStorageDetail(whCode,
                    cargoCode, cargoName, cargoModel, cargoSpec, cargoUnits, queryMode, binCode).ToList<proc_WebSelectCargoStorageDetail_Result>();

                //string sortExpression = gv_CargoList.Attributes["sortExpression"];
                SortDirection sortDirection = sortExpression == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                if (sortDirection == SortDirection.Ascending)
                    queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                else
                    queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();



                ReportDataSource rds = new ReportDataSource("DataSet_CargoStorageDetail", queryResult);
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
                ReportParameter RP_CargoCode = new ReportParameter("RP_CargoCode", cargoCode.Replace("'",""));
                ReportParameter RP_CargoName = new ReportParameter("RP_CargoName", cargoName);
                ReportParameter RP_CargoModel = new ReportParameter("RP_CargoModel", cargoModel);
                ReportParameter RP_CargoSpec = new ReportParameter("RP_CargoSpec", cargoSpec);
                ReportParameter RP_CargoUnits = new ReportParameter("RP_CargoUnits", "");
                ReportParameter RP_WHName = new ReportParameter("RP_WHName", whName);


                ReportParameterCollection rpList = new ReportParameterCollection();
                rpList.Add(RP_CargoCode);
                rpList.Add(RP_CargoName);
                rpList.Add(RP_CargoModel);
                rpList.Add(RP_CargoSpec);
                rpList.Add(RP_CargoUnits);
                rpList.Add(RP_WHName);
                //rpList.Add(RP_WHCode);

                ReportViewer1.LocalReport.SetParameters(rpList);
                ReportViewer1.DataBind();
            }
        }


    }
}