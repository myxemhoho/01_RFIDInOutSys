using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using System.Data;

namespace Gold.SystemSetting
{
    public partial class Positions : System.Web.UI.Page
    {
        private TextBox _txtName;
        private TextBox _txtDesc;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //添加记录
        protected void lbtnInsert_Click(object sender, EventArgs e)
        {
            TextBox txtName = GridView1.FooterRow.FindControl("txtName") as TextBox;
            TextBox txtDesc = GridView1.FooterRow.FindControl("txtDescription") as TextBox;

            using (var context = new Gold.DAL.GoldEntities())
            {
                var tmp = context.CreateObject<Position>();
                tmp.PositionName = txtName.Text;
                tmp.Description = txtDesc.Text;
                context.AddToPosition(tmp);
                context.SaveChanges();
                GridView1.DataBind();
            }

        }

        //添加记录
        protected void lbtnSave_Click(object sender, EventArgs e)
        {
            //TextBox txtName = GridView1.FooterRow.FindControl("txtName") as TextBox;
            //TextBox txtDesc = GridView1.FooterRow.FindControl("txtDescription") as TextBox;

            using (var context = new Gold.DAL.GoldEntities())
            {
                var tmp = context.CreateObject<Position>();
                tmp.PositionName = _txtName.Text;
                tmp.Description = _txtDesc.Text;
                context.AddToPosition(tmp);
                context.SaveChanges();
                GridView1.DataBind();
            }

        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                _txtName = (TextBox)e.Row.FindControl("txtName2");
                _txtDesc = (TextBox)e.Row.FindControl("txtDescption2");
            }
        }

    }
}