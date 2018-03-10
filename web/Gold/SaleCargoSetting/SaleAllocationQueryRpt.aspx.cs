using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Gold.SaleCargoSetting
{
    public partial class SaleAllocationQueryRpt : System.Web.UI.Page
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
                    string saleStatusStr = Request.QueryString["saleStatusStr"].ToString().Trim();                    
                    int saleStatus = -1;
                    if (!string.IsNullOrEmpty(saleStatusStr))
                    {
                        if (int.TryParse(saleStatusStr, out saleStatus) == false)
                            saleStatus = -1;
                    }

                    using (DAL.GoldEntities context = new DAL.GoldEntities())
                    {
                        List<DAL.proc_WebSelectEachDeptSaleAllocation_Result> resultList = context.proc_WebSelectEachDeptSaleAllocation().ToList<DAL.proc_WebSelectEachDeptSaleAllocation_Result>();


                        var queryResult = (from r in resultList
                                           where r.CargoCode.Contains(cargoCode)
                                           && r.CargoName.Contains(cargoName)
                                           && r.CargoModel.Contains(modelName)
                                           && r.CargoSpec.Contains(specName)
                                           && (saleStatus >= 0 ? r.SaleStatus == saleStatus : 1 == 1)
                                           select r).ToList();

                        Microsoft.Reporting.WebForms.ReportDataSource rds = new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", queryResult);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        Microsoft.Reporting.WebForms.ReportParameter ReportParameter_CargoCode = new Microsoft.Reporting.WebForms.ReportParameter("ReportParameter_CargoCode", cargoCode);
                        Microsoft.Reporting.WebForms.ReportParameter ReportParameter_CargoName = new Microsoft.Reporting.WebForms.ReportParameter("ReportParameter_CargoName", cargoName);
                        Microsoft.Reporting.WebForms.ReportParameter ReportParameter_CargoModel = new Microsoft.Reporting.WebForms.ReportParameter("ReportParameter_CargoModel", modelName);
                        Microsoft.Reporting.WebForms.ReportParameter ReportParameter_CargoSpec = new Microsoft.Reporting.WebForms.ReportParameter("ReportParameter_CargoSpec", specName);
                        Microsoft.Reporting.WebForms.ReportParameter ReportParameter_SaleStatus = new Microsoft.Reporting.WebForms.ReportParameter("ReportParameter_SaleStatus", saleStatus >= 0 ? (saleStatus>0?"可售":"不可售"): "");
                        
                        Microsoft.Reporting.WebForms.ReportParameterCollection rpList = new Microsoft.Reporting.WebForms.ReportParameterCollection();
                        rpList.Add(ReportParameter_CargoCode);
                        rpList.Add(ReportParameter_CargoName);
                        rpList.Add(ReportParameter_CargoModel);
                        rpList.Add(ReportParameter_CargoSpec);
                        rpList.Add(ReportParameter_SaleStatus);

                        ReportViewer1.LocalReport.SetParameters(rpList);
                        ReportViewer1.DataBind();
                    }
                }
            }
            catch (Exception ex) 
            {
                lblGridViewMsg.Text = Utility.LogHelper.GetExceptionMsg(ex);
            }
        }
    }
}