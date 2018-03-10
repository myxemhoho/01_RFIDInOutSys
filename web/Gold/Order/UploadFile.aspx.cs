using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Gold.Order
{
    public partial class UploadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void AjaxFileUpload1_OnUploadComplete(object sender, AjaxFileUploadEventArgs file)
        {
            // User can save file to File System, database or in session state
            if (file.ContentType.Contains("xls") || file.ContentType.Contains("xlsx"))
            {
                Session["fileContentType_" + file.FileId] = file.ContentType;
                Session["fileContents_" + file.FileId] = file.GetContents();
            }

            // Set PostedUrl to preview the uploaded file.         
            file.PostedUrl = string.Format("?preview=1&fileId={0}", file.FileId);
        }

    }
}