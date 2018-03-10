using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.Query
{
    public partial class InventoryBookQty : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                //txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                //txtStartDate.Attributes.Add("readonly", "true");
                //txtEndDate.Attributes.Add("readonly", "true");

                txtDate.Text = DateTime.Now.ToString("yyyy-MM");
                txtDate.Attributes.Add("readonly", "true");

            }

        }

        protected void edsInventoryBook_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCargoCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoCode.Contains(txtCargoCode.Text.Trim()));
            }

            if (!string.IsNullOrEmpty(txtCargoName.Text.Trim()))
            {
                e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoName.Contains(txtCargoName.Text.Trim()));
            }

            if (!string.IsNullOrEmpty(txtCargoModel.Text.Trim()))
            {
                e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoModel.Contains(txtCargoModel.Text.Trim()));
            }

            if (!string.IsNullOrEmpty(txtCargoSpec.Text.Trim()))
            {
                e.Query = e.Query.Cast<InventoryBook>().Where(o => o.CargoSpec.Contains(txtCargoSpec.Text.Trim()));
            }




            //订单时间
            DateTime startDate = DateTime.Parse(txtDate.Text);
            DateTime endDate = startDate.AddMonths(1);  
            e.Query = e.Query.Cast<InventoryBook>().Where(o => o.ChangeTime >= startDate && o.ChangeTime < endDate);

            //仓库
            if (!string.IsNullOrEmpty(drpWHCode.SelectedValue))
            {
                e.Query = e.Query.Cast<InventoryBook>().Where(o => o.WHCode == drpWHCode.SelectedValue);
            }

            e.Query = e.Query.Cast<InventoryBook>().OrderBy(o => o.CargoCode).ThenBy(o => o.ChangeTime);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DateTime d1;
            if (!DateTime.TryParse(txtDate.Text.Trim(), out d1))
            {
                lblMessage.Text = "日期格式错！";
                return;
                
            }

            GridView1.DataBind();
        }


        int _sumNumAdd;
        int _sumNumDel;
        string _numCurrent;
        GridViewRow _preRow = null;
        int _preRowSeq = 1;
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");

                int sumAdd;
                int sumDel;
                int.TryParse(e.Row.Cells[6].Text, out sumAdd);
                int.TryParse(e.Row.Cells[7].Text, out sumDel);

                //首行，赋值后返回
                if (_preRow == null)
                {
                    _sumNumAdd += sumAdd;
                    _sumNumDel += sumDel;
                    _numCurrent = e.Row.Cells[8].Text;

                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                    return;
                }

                //商品编码与前行相同，合并第0~4列 （序号，商品名称，商品编码，型号，规格）
                if (e.Row.Cells[2].Text == _preRow.Cells[2].Text)
                {
                    _sumNumAdd += sumAdd;
                    _sumNumDel += sumDel;
                    _numCurrent = e.Row.Cells[8].Text;

                    e.Row.Visible = false;
                }
                else   //商品编码不一样，重新开始
                {
                    //添加小计行
                    //InsertGridRow(e.Row);
                    _preRow.Cells[6].Text = _sumNumAdd.ToString();
                    _preRow.Cells[7].Text = _sumNumDel.ToString();
                    _preRow.Cells[8].Text = _numCurrent;

                    _sumNumAdd = sumAdd;
                    _sumNumDel = sumDel;
                    _numCurrent = e.Row.Cells[8].Text;

                    _preRowSeq++;
                    //_rowSpan = 1;
                    e.Row.Cells[0].Text = _preRowSeq.ToString();
                    _preRow = e.Row;
                }
            }

            // 合计
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                _preRow.Cells[6].Text = _sumNumAdd.ToString();
                _preRow.Cells[7].Text = _sumNumDel.ToString();
                _preRow.Cells[8].Text = _numCurrent;
            }
        }

        protected void lbtnExport_Click(object sender, EventArgs e)
        {
           
            //GridView1.AllowPaging = false;//取消分页,使GridView显示全部数据.
            GridView1.DataBind();
            Response.Clear();
            Response.Charset = "GB2312";
            Response.AppendHeader("Content-Disposition", "attachment;filename= " + Server.UrlEncode("库存台账（月合计）.xls"));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/ms-excel";//设置输出文件类型为excel文件.
            Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=GB2312\">");
            this.EnableViewState = false;
            System.Globalization.CultureInfo myCItrad = new System.Globalization.CultureInfo("ZH-CN", true);
            System.IO.StringWriter stringWrite = new System.IO.StringWriter(myCItrad);
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            GridView1.RenderControl(htmlWrite);
            Response.Write(stringWrite.ToString());
            Response.Write(@"<style> .text { mso-number-format:\@; } </script> ");
            Response.End();
            //GridView1.AllowPaging = true;

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }
        
    }
}