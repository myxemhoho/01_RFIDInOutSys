using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

using Gold.NCInvoke;
using System.Data;

namespace Gold.BaseInfoSetting
{
    public partial class CargoList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();

                //要在GridView标记中加入自定义的标记sortExpression和sortDirection,例如 sortExpression="WHCode" sortDirection="ASC"
                GridViewBind();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_CargoList);//生成固定表头
        }

        void GridViewBind()
        {
            try
            {
                lblGridViewMsg.Text = "";

                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();

                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    var queryResult = (from r in context.Cargos
                                       where r.CargoCode.Contains(cargoCode) && r.CargoName.Contains(cargoName) && r.CargoModel.Contains(modelName) && r.CargoSpec.Contains(specName)
                                       select r).ToList();

                    string sortExpression = gv_CargoList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_CargoList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    gv_CargoList.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_CargoList.DataSource = queryResult;
                    gv_CargoList.DataBind();
                }
            }
            catch (Exception ex) 
            {
                lblGridViewMsg.Text = "查询出现异常！" + Utility.LogHelper.GetExceptionMsg(ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        protected void gv_CargoList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView theGrid = sender as GridView;   // refer to the GridView
            int newPageIndex = 0;

            if (-2 == e.NewPageIndex)
            { // when click the "GO" Button

                TextBox txtNewPageIndex = null;
                //GridViewRow pagerRow = theGrid.Controls[0].Controls[theGrid.Controls[0].Controls.Count - 1] as GridViewRow; // refer to PagerTemplate
                GridViewRow pagerRow = theGrid.BottomPagerRow; //GridView较DataGrid提供了更多的API，获取分页块可以使用BottomPagerRow 或者TopPagerRow，当然还增加了HeaderRow和FooterRow
                
                if (null != pagerRow)
                {
                    txtNewPageIndex = pagerRow.FindControl("txtNewPageIndex") as TextBox;    // refer to the TextBox with the NewPageIndex value
                }

                if (null != txtNewPageIndex)
                {
                    newPageIndex = int.Parse(txtNewPageIndex.Text) - 1; // get the NewPageIndex
                }
            }
            else
            {   // when click the first, last, previous and next Button
                newPageIndex = e.NewPageIndex;
            }

            // check to prevent form the NewPageIndex out of the range
            newPageIndex = newPageIndex < 0 ? 0 : newPageIndex;
            newPageIndex = newPageIndex >= theGrid.PageCount ? theGrid.PageCount - 1 : newPageIndex;

            // specify the NewPageIndex
            theGrid.PageIndex = newPageIndex;

            // rebind the control
            // in this case of retrieving the data using the xxxDataSoucr control,
            // just do nothing, because the asp.net engine binds the data automatically

            GridViewBind();
        }


        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GridViewBind();
        }

        protected void gv_CargoList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_CargoList.Attributes["sortExpression"]) && "ASC".Equals(gv_CargoList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_CargoList.Attributes.Add("sortExpression", sortExpression);
            gv_CargoList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void btnShowAddPage_Click(object sender, EventArgs e)
        {
            //QueryString中EditType 1为新增 2为编辑
            //QueryString中EditID 0表示不编辑
            string EditID = "0";
            System.Text.StringBuilder url = new System.Text.StringBuilder("~/BaseInfoSetting/CargoEdit.aspx?EditType=1");
            url.Append("&EditID=");
            url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=1";
            Response.Redirect(url.ToString());
        }

        protected void btnShowEditPage_Click(object sender, EventArgs e)
        {   //QueryString中EditType 1为新增 2为编辑
            //QueryString中EditID 0表示不编辑 1表示编辑
            string EditID = "0";

            lblCheckMsg.Text = "";
            lblGridViewMsg.Text = "";

            List<string> checkedItem = GetCheckItemID();
            if (checkedItem.Count == 0)
            {
                lblCheckMsg.Text = "请在列表中选择一条记录进行编辑！";
                DAL.CommonConvert.ShowMessageBox(this.Page, "请在列表中选择一条记录进行编辑！");
                return;
            }
            else if (checkedItem.Count > 1)
            {
                lblCheckMsg.Text = "只能选择列表中的一条记录进行编辑！";
                DAL.CommonConvert.ShowMessageBox(this.Page, "只能选择列表中的一条记录进行编辑！");
                return;
            }
            else
            {
                EditID = checkedItem[0].Trim();
            }

            System.Text.StringBuilder url = new System.Text.StringBuilder("~/BaseInfoSetting/CargoEdit.aspx?EditType=2");
            url.Append("&EditID=");
            url.Append(EditID); //"~/BaseInfoSetting/WareHouseEdit.aspx?EditType=2";
            Response.Redirect(url.ToString());
        }

        private List<string> GetCheckItemID()
        {
            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < gv_CargoList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_CargoList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = (currentRow.FindControl("lblCargoCode") as Label).Text.Trim();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;
                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                lblCheckMsg.Text = "";
                List<string> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    lblCheckMsg.Text = "请先选中待删除的项";
                    DAL.CommonConvert.ShowMessageBox(this.Page, "请先选中待删除的项");
                    return;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    foreach (string delCode in checkedDelList)
                    {
                        DAL.Cargos delObject = (from r in context.Cargos where r.CargoCode.Equals(delCode) select r).FirstOrDefault();

                        context.DeleteObject(delObject);
                    }
                    int delRow = context.SaveChanges();
                    if (delRow > 0)
                    {
                        lblCheckMsg.Text = "删除成功！[已删除" + delRow.ToString() + "项]";
                        DAL.CommonConvert.ShowMessageBox(this.Page,"删除成功！[已删除" + delRow.ToString() + "项]");
                    }
                    else
                    {
                        lblCheckMsg.Text = "删除失败！";
                        DAL.CommonConvert.ShowMessageBox(this.Page,"删除失败！");
                    }

                    GridViewBind();//删除后重新绑定数据
                }
            }
            catch (Exception ex) 
            {
                lblCheckMsg.Text = "删除失败！";
                DAL.CommonConvert.ShowMessageBox(this.Page, "删除失败！");
                lblGridViewMsg.Text="删除出现异常！详细信息："+Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        protected void gv_CargoList_DataBound(object sender, EventArgs e)
        {
            //设置自定义分页的显示状态
            GridView theGrid = sender as GridView;
            //if (theGrid.Rows.Count > theGrid.PageSize)
            //{
                int newPageIndex = theGrid.PageIndex;

                GridViewRow pagerRowShow = theGrid.BottomPagerRow;
                if (pagerRowShow == null)
                    return;
                LinkButton btnFirst = pagerRowShow.FindControl("btnFirst") as LinkButton;
                LinkButton btnPrev = pagerRowShow.FindControl("btnPrev") as LinkButton;
                LinkButton btnNext = pagerRowShow.FindControl("btnNext") as LinkButton;
                LinkButton btnLast = pagerRowShow.FindControl("btnLast") as LinkButton;


                if (newPageIndex >= theGrid.PageCount - 1)
                {
                    btnLast.Enabled = false;
                    btnNext.Enabled = false;
                }
                else
                {
                    btnLast.Enabled = true;
                    btnNext.Enabled = true;
                }

                if (newPageIndex <= 0)
                {
                    btnFirst.Enabled = false;
                    btnPrev.Enabled = false;
                }
                else
                {
                    btnFirst.Enabled = true;
                    btnPrev.Enabled = true;
                }
            //}
        }

        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定型号
                var allModelList = (from r in context.Models orderby r.ModelName select new { r.ModelId,r.ModelName}).ToList();
                DropDownList_CargoModel.Items.Clear();
                DropDownList_CargoModel.DataTextField = "ModelName";
                DropDownList_CargoModel.DataValueField = "ModelID";
                DropDownList_CargoModel.DataSource = allModelList;
                DropDownList_CargoModel.DataBind();
                DropDownList_CargoModel.Items.Insert(0, new ListItem("", ""));

                //绑定规格
                var allSpecList = (from r in context.Specifications orderby r.SpecName select new { r.SpecId, r.SpecName }).ToList();
                DropDownList_CargoSpec.Items.Clear();
                DropDownList_CargoSpec.DataTextField = "SpecName";
                DropDownList_CargoSpec.DataValueField = "SpecId";
                DropDownList_CargoSpec.DataSource = allSpecList;
                DropDownList_CargoSpec.DataBind();
                DropDownList_CargoSpec.Items.Insert(0, new ListItem("", ""));
            }
        }

        #region 固定GridView表头

        /// <summary>
        /// 生成固定表头
        /// </summary>
        /// <param name="gv_SaleAllocationList"></param>
        void DisplayFixHeader(GridView gv_SaleAllocationList)
        {
            Table table = new Table();
            table.CssClass = "GridViewFixHeader_Table";
            table.Width = new Unit(gv_SaleAllocationList.Width.Value);

            TableRow tr = new TableRow();
            tr.CssClass = "GridViewHeaderStyle";
            int i = 0;
            foreach (DataControlField cell in gv_SaleAllocationList.Columns)
            {
                if (cell.Visible == true)
                {
                    TableCell td = new TableCell();
                    td.Style.Add("text-align", "left");
                    if (cell.ItemStyle.Width.Value != (double)0)
                    {
                        td.Width = (int)Math.Round(cell.ItemStyle.Width.Value);
                    }

                    if (!string.IsNullOrEmpty(cell.SortExpression))
                    {
                        LinkButton lbtn = new LinkButton();
                        lbtn.CommandArgument = cell.SortExpression;
                        lbtn.Text = cell.HeaderText;
                        lbtn.Click += new EventHandler(LinkButtonHeader_Click);
                        lbtn.ID = "LinkButton" + (i++).ToString();
                        //lbtn.OnClientClick = "divWait('divWait');";//js提示
                        lbtn.Attributes.Add("onclick", "showWaitDiv('divWait');");//js提示

                        td.Controls.Add(lbtn);


                    }
                    else
                    {
                        td.Text = cell.HeaderText;
                    }
                    if (string.IsNullOrEmpty(cell.SortExpression) && cell.HeaderText == "*")
                    {

                        CheckBox chk = new CheckBox();
                        chk.ID = "chkAllCheck" + (i++).ToString();
                        chk.Text = "全选";
                        //chk.AutoPostBack = true;
                        //chk.CheckedChanged += new EventHandler(CheckBox_CheckAll_CheckedChanged);
                        chk.Attributes.Add("onclick", "javascript:selectAllCheckBox('" + gv_SaleAllocationList.ClientID + "',this);");

                        td.Controls.Add(chk);

                    }

                    tr.Cells.Add(td);
                }
            }
            table.Rows.Add(tr);

            divHeader.Controls.Clear();
            divHeader.Controls.Add(table);

            //gv_SaleAllocationList.ShowHeader = false;//这里设置无效，要在标记中设置

        }

        //自定义排序
        protected void LinkButtonHeader_Click(object sender, EventArgs e)
        {
            LinkButton lBtn = sender as LinkButton;
            if (lBtn != null)
            {
                string sortExpression = lBtn.CommandArgument;//获取排序字段，进行排序
                SortDirection sortDirection = gv_CargoList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_CargoList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_CargoList.Sort(sortExpression, newSortDirection);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        #region 仿MessageBox

        /// <summary>
        /// 显示简单消息(例如"保存成功")
        /// </summary>
        /// <param name="msg">提示信息</param>
        void ShowMessageBox(string msg)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = false;//当只显示简单信息时，“详细信息栏”不显示
            lblMessageContentException.Visible = false;
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 显示提示信息和详细的异常信息（例如“保存失败！”,"详细信息：XXX"）
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="ex">异常信息</param>
        void ShowMessageBox(string msg, Exception ex)
        {
            lblMessageContent.Text = msg;

            divMessageDetail.Visible = true;//当显示异常信息时，“详细信息栏”进行显示
            lblMessageContentException.Visible = true;
            string exMsg = Utility.LogHelper.GetExceptionMsg(ex);
            lblMessageContentException.Text = exMsg;

            this.programmaticModalPopup_Msg.Show();//弹出div
        }

        /// <summary>
        /// 在服务端关闭提示信息(此方法一般不要用，关闭时用弹出div的javascript关闭会性能更好)
        /// </summary>
        void CloseMessageBox()
        {
            lblMessageContent.Text = "";
            lblMessageContentException.Text = "";

            this.programmaticModalPopup_Msg.Hide();//隐藏div
        }


        protected void showModalPopupServerOperatorButton_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Show();
        }
        protected void hideModalPopupViaServer_Click(object sender, EventArgs e)
        {
            this.programmaticModalPopup_Msg.Hide();
        }

        #endregion

        #region 用友NC数据导入
        protected void btnNCDataImport_Click(object sender, EventArgs e)
        {
            try
            {
                #region 查询数据

                string typeArgs = "CargoInfo";
                string conditionArgs = string.Empty;
                DataTable dt = null;
                string queryMsg = "";
                string saveMsg = "";

                BasicInfoInvoke BasicInfoInvokeObj = BasicInfoInvokeFactory.CreateInstance(typeArgs, conditionArgs);

                if (BasicInfoInvokeObj.GetNCDataJoinRFID(out dt, out queryMsg) == false)
                {
                    ShowMessageBox("查询用友系统信息失败！详细信息：" + queryMsg);
                    return;
                }
                else
                {
                    bool result = BasicInfoInvokeObj.SaveToRFID(dt, out saveMsg);
                    ShowMessageBox(result == true ? "数据导入成功！" : "数据导入失败！", new Exception(saveMsg));
                }

                #endregion

                GridViewBind();//数据导入成功后重新绑定数据
            }
            catch (Exception ex)
            {
                ShowMessageBox("数据导入失败！", ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        protected void btnNCPriceImport_Click(object sender, EventArgs e)
        {
            try
            {
                #region 查询数据

                string typeArgs = "CargoPrice";
                string conditionArgs = string.Empty;
                DataTable dt = null;
                string queryMsg = "";
                string saveMsg = "";

                BasicInfoInvoke BasicInfoInvokeObj = BasicInfoInvokeFactory.CreateInstance(typeArgs, conditionArgs);

                if (BasicInfoInvokeObj.GetNCDataJoinRFID(out dt, out queryMsg) == false)
                {
                    ShowMessageBox("查询用友系统信息失败！详细信息：" + queryMsg);
                    return;
                }
                else
                {
                    bool result = BasicInfoInvokeObj.SaveToRFID(dt, out saveMsg);
                    ShowMessageBox(result == true ? "数据导入成功！" : "数据导入失败！", new Exception(saveMsg));
                }

                #endregion

                GridViewBind();//数据导入成功后重新绑定数据
            }
            catch (Exception ex)
            {
                ShowMessageBox("数据导入失败！", ex);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion

        

        #region 解决ViewState过于庞大的问题
        protected override object LoadPageStateFromPersistenceMedium()
        {
            string viewStateID = (string)((Pair)base.LoadPageStateFromPersistenceMedium()).Second;
            string stateStr = (string)Cache[viewStateID];
            if (stateStr == null)
            {
                string fn = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, @"App_Data/ViewState/" + viewStateID);
                stateStr = System.IO.File.ReadAllText(fn);
            }
            return new ObjectStateFormatter().Deserialize(stateStr);
        }

        protected override void SavePageStateToPersistenceMedium(object state)
        {
            string value = new ObjectStateFormatter().Serialize(state);
            string viewStateID = (DateTime.Now.Ticks + (long)this.GetHashCode()).ToString(); //产生离散的id号码
            string fn = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, @"App_Data/ViewState/" + viewStateID);
            //ThreadPool.QueueUserWorkItem(File.WriteAllText(fn, value));
            System.IO.File.WriteAllText(fn, value);
            Cache.Insert(viewStateID, value);
            base.SavePageStateToPersistenceMedium(viewStateID);
        }
        #endregion
    }
}