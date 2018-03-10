using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gold
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] set1 = new string[] { "A", "B", "C" };
            string[] set2 = new string[] {  "B", "C", "D" };

            string[] set3 = set1.Except(set2).ToArray();
            string[] set4 = set2.Except(set1).ToArray();
        }
    }
}
