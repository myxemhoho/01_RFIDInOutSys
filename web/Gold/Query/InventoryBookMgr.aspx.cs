using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.Account
{
    public partial class InventoryBook : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtStartTime.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtStartTime.Attributes.Add("readonly", "true");
                txtEndTime.Attributes.Add("readonly", "true");

                //////绑定订单状态下拉框的值
                //using (var edm = new Gold.DAL.GoldEntities())
                //{
                //    var tmp = (edm.WareHouse.Where(o => o.WHCode == "WHCode")).ToList();
                //    if (tmp != null || tmp.Count != 0)
                //    {
                //        //绑定数据
                //        DDLWHName.DataSource = tmp;
                //        DDLWHName.DataValueField = "WHName";
                //        DDLWHName.DataTextField = "WHCode";
                //        DDLWHName.DataBind();
                //        DDLWHName.Items.Insert(0, new ListItem("不限", ""));
                //    }

                //}

            }
        }
        
        public string ABC()
        {
            string A = GridView1.Columns.Count.ToString();
            return  A;
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            {
                DateTime d1;
                if (!DateTime.TryParse(txtStartTime.Text.Trim(), out d1))
                {
                    lblMessage.Text = "日期格式错！";
                    return;
                }

                DateTime d2;
                if (!DateTime.TryParse(txtEndTime.Text.Trim(), out d2))
                {
                    lblMessage.Text = "日期格式错！";
                    return;
                }

                if (d1 > d2)
                {
                    lblMessage.Text = "起始日期大于结束日期！";
                    return;
                }

                GridView1.DataBind();
            }
        }

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            //暂时留着
        }

        protected void EntityDataSource1_QueryCreated(object sender, QueryCreatedEventArgs e)
        {
            //按时间查询
            DateTime startDate = DateTime.Parse(txtStartTime.Text);
            DateTime endDate = DateTime.Parse(txtEndTime.Text);
            endDate = endDate.AddDays(1);
            e.Query = e.Query.Cast<Gold.DAL.InventoryBook>().Where(o => o.ChangeTime >= startDate && o.ChangeTime < endDate);

            //按名称查询
            if (!string.IsNullOrEmpty(txtCargoName.Text.Trim()))
            {
                e.Query = e.Query.Cast<Gold.DAL.InventoryBook>().Where(o => o.CargoName == txtCargoName.Text.Trim());
                return;
            }

            //按编码查询
            if (!string.IsNullOrEmpty(txtCargoCode.Text.Trim()))
            {
                e.Query = e.Query.Cast<Gold.DAL.InventoryBook>().Where(o => o.CargoCode == txtCargoCode.Text.Trim());
                return;

            }

            ////订单状态
            //if (DDLWHName.SelectedIndex != 0)
            //{
            //    int status = Convert.ToInt32(DDLWHName.SelectedValue);
            //    e.Query = e.Query.Cast<??>().Where(o => o.?? == status);
            //}
        }



        ///   <summary>   
        ///   根据条件列合并GridView列中相同的行   
        ///   </summary>   
        ///   <param   name="GridView1">GridView对象</param>   
        ///   <param   name="cellNum">需要合并的列</param>
        ///   ///   <param   name="cellNum2">条件列(根据某条件列还合并)</param>
        ///   
        public static void GroupRows(GridView GridView1, int cellNum, int cellNum2)
        {
            int i = 0, rowSpanNum = 1;
            while (i < GridView1.Rows.Count - 1)
            {
                GridViewRow gvr = GridView1.Rows[i];
                for (++i; i < GridView1.Rows.Count; i++)
                {
                    GridViewRow gvrNext = GridView1.Rows[i];
                    if (gvr.Cells[cellNum].Text + gvr.Cells[cellNum2].Text == gvrNext.Cells[cellNum].Text + gvrNext.Cells[cellNum2].Text)
                    {
                        gvrNext.Cells[cellNum].Visible = false;
                        rowSpanNum++;
                    }
                    else
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                        rowSpanNum = 1;
                        break;
                    }
                    if (i == GridView1.Rows.Count - 1)
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                    }
                }
            }
        }

        }
    }
