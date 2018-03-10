using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Gold.DAL;
using Gold.Utility;
using System.Data;
using System.Data.SqlClient;

namespace Gold.SaleCargoSetting
{
    public partial class SaleAllocationSet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();

                GridViewBind();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_SaleAllocationList);//生成固定表头
        }

        #region 查询

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GridViewBind();
        }

        void GridViewBind()
        {
            try
            {
                //lblGridViewMsg.Text = "";

                string cargoCode = tbxCargoCode.Text.Trim();
                string cargoName = tbxCargoName.Text.Trim();
                string modelName = DropDownList_CargoModel.SelectedItem.Text.Trim();
                string specName = DropDownList_CargoSpec.SelectedItem.Text.Trim();


                System.Diagnostics.Trace.WriteLine("proc start" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss:ffff"));
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    List<DAL.proc_WebSelectEachDeptSaleAllocation_Result> resultList = context.proc_WebSelectEachDeptSaleAllocation().ToList<DAL.proc_WebSelectEachDeptSaleAllocation_Result>();


                    var queryResult = (from r in resultList
                                       where r.CargoCode.Contains(cargoCode)
                                       && r.CargoName.Contains(cargoName)
                                       && r.CargoModel.Contains(modelName)
                                       && r.CargoSpec.Contains(specName)
                                       select r).ToList();

                    string sortExpression = gv_SaleAllocationList.Attributes["sortExpression"];
                    SortDirection sortDirection = gv_SaleAllocationList.Attributes["sortDirection"] == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

                    if (sortDirection == SortDirection.Ascending)
                        queryResult = queryResult.OrderBy(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();
                    else
                        queryResult = queryResult.OrderByDescending(r => r.GetType().GetProperty(sortExpression).GetValue(r, null)).ToList();

                    gv_SaleAllocationList.PageSize = WebConfigHelper.Instance.GetDefaultPageSize();
                    gv_SaleAllocationList.DataSource = queryResult;
                    gv_SaleAllocationList.DataBind();
                }
                System.Diagnostics.Trace.WriteLine("proc end" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss:ffff"));
#if entityframework
                
                string sortExpression = gv_SaleAllocationList.Attributes["sortExpression"];
                string sortDirection = gv_SaleAllocationList.Attributes["sortDirection"];


                DataTable dtBind = SqlHelper.ExecuteDataTable("proc_WebSelectEachDeptSaleAllocation", System.Data.CommandType.StoredProcedure, null);
                // 根据GridView排序数据列及排序方向设置显示的默认数据视图
                if ((!string.IsNullOrEmpty(sortExpression)) && (!string.IsNullOrEmpty(sortDirection)))
                {
                    dtBind.DefaultView.Sort = string.Format("{0} {1}", sortExpression, sortDirection);
                }
                gv_SaleAllocationList.DataSource = dtBind.DefaultView.ToTable();
                gv_SaleAllocationList.DataBind();

                
#endif
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

        #endregion

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SaleAllocationList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            try
            {

                GridView theGrid = sender as GridView;   // refer to the GridView
                int newPageIndex = 0;

                if (-2 == e.NewPageIndex)
                { // when click the "GO" Button

                    TextBox txtNewPageIndex = null;

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


                GridViewBind();//根据新页索引重新绑定数据

            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！ gv_SaleAllocationList_PageIndexChanging" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SaleAllocationList_DataBound(object sender, EventArgs e)
        {
            try
            {
                string sjs = "GetResultFromServer();";
                ScriptManager.RegisterClientScriptBlock(this.gv_SaleAllocationList, this.GetType(), "", sjs, true);

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
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "查询出现异常！ gv_SaleAllocationList_DataBound" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        #endregion
        
        #region 编辑

        /// <summary>
        /// 行命令触发前先清空界面消息提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SaleAllocationList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView senderGrid = sender as GridView;
            //更新或删除前对消息Label清空
            if (e.CommandName == "Edit" || e.CommandName == "Update" || e.CommandName == "Delete")
            {
                //lblGridViewMsg.Text = "";
                ClearTipMsgLabel();//清除提示信息
            }
            else if (e.CommandName == "MyDefineUpdate")
            {
                lblGridViewMsg.Text = "";
                if (senderGrid.EditIndex != -1)
                {
                    string editBinCode = senderGrid.DataKeys[senderGrid.EditIndex].Value.ToString();
                    GridViewRow editRow = senderGrid.Rows[senderGrid.EditIndex]; //GridView较DataGrid提供了更多的API，获取分页块可以使用BottomPagerRow 或者TopPagerRow，当然还增加了HeaderRow和FooterRow
                    if ((editRow.RowType == DataControlRowType.DataRow))
                    {
                        string saveMsg="";
                        if (SaveAllocate(editRow, out saveMsg))
                        {
                            senderGrid.EditIndex = -1;
                            lblGridViewMsg.Text = "保存成功！[" + saveMsg + "]";
                            DAL.CommonConvert.ShowMessageBox(this.Page,lblGridViewMsg.Text.Replace("\r\n",""));
                            GridViewBind();
                            
                        }
                        else 
                        {
                            lblGridViewMsg.Text = "保存失败！[" + saveMsg + "]";
                            DAL.CommonConvert.ShowMessageBox(this.Page, lblGridViewMsg.Text.Replace("\r\n", ""));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 在行数据绑定完成后绑定编辑界面的DropDownList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SaleAllocationList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView theGrid = sender as GridView;

            //将数据行中的层位类型和标签状态代码转换成名称显示
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
               

                ////给Button加上客户端javascript函数

                LinkButton btnUpdate = e.Row.FindControl("btnUpdate") as LinkButton;
                if (btnUpdate != null)
                    btnUpdate.OnClientClick = "return NumberConfirm(" + e.Row.RowIndex.ToString() + ");";
            }
        }



        #endregion

        //保存分配量
        bool SaveAllocate(GridViewRow editRow, out string saveMsg)
        {
            System.Text.StringBuilder checkMsg = new System.Text.StringBuilder();
            System.Text.StringBuilder LogMsg = new System.Text.StringBuilder();


            //商品编码
            Label lblCargoCode = editRow.FindControl("lblCargoCode") as Label;
            if (lblCargoCode == null)
            {
                checkMsg.Append("未能获取商品编码");
                checkMsg.Append(Environment.NewLine);
            }

            //结存数量
            TextBox tbxRowTotal = editRow.FindControl("tbxRowTotal") as TextBox;
            if (tbxRowTotal == null)
            {
                checkMsg.Append("未能获取结存数量.");
                checkMsg.Append(Environment.NewLine);
            }

            //渠道部分配量
            TextBox tbxRowDeptChannelAllocation = editRow.FindControl("tbxRowDeptChannelAllocation") as TextBox;
            if (tbxRowDeptChannelAllocation == null)
            {
                checkMsg.Append("未能获取渠道部分配量.");
                checkMsg.Append(Environment.NewLine);
            }

            //销售部分配量
            TextBox tbxRowDeptSaleAllocation = editRow.FindControl("tbxRowDeptSaleAllocation") as TextBox;
            if (tbxRowDeptSaleAllocation == null)
            {
                checkMsg.Append("未能获取销售部分配量.");
                checkMsg.Append(Environment.NewLine);
            }

            //零售中心分配量
            TextBox tbxRowDeptRetailAllocation = editRow.FindControl("tbxRowDeptRetailAllocation") as TextBox;
            if (tbxRowDeptRetailAllocation == null)
            {
                checkMsg.Append("未能获取零售中心分配量.");
                checkMsg.Append(Environment.NewLine);
            }

            //电子商务分配量
            TextBox tbxRowDeptECAllocation = editRow.FindControl("tbxRowDeptECAllocation") as TextBox;
            if (tbxRowDeptECAllocation == null)
            {
                checkMsg.Append("未能获取电子商务分配量.");
                checkMsg.Append(Environment.NewLine);
            }

            //公用部门分配量
            TextBox tbxRowDeptPublicAllocation = editRow.FindControl("tbxRowDeptPublicAllocation") as TextBox;
            if (tbxRowDeptPublicAllocation == null)
            {
                checkMsg.Append("未能获取公用部门分配量.");
                checkMsg.Append(Environment.NewLine);
            }

            //机动量
            TextBox tbxRowVariation = editRow.FindControl("tbxRowVariation") as TextBox;
            if (tbxRowVariation == null)
            {
                checkMsg.Append("未能获取机动量.");
                checkMsg.Append(Environment.NewLine);
            }

            if (checkMsg.Length > 0)
            {
                saveMsg = checkMsg.ToString();
                return false;
            }


            try
            {
                DAL.GoldEntities context = new GoldEntities();

                string cargoCode = lblCargoCode.Text;
                LogMsg.Append("商品编码为");
                LogMsg.Append(lblCargoCode.Text);

                Cargos UpdateCargos = (from r in context.Cargos where r.CargoCode == lblCargoCode.Text.Trim() select r).FirstOrDefault<Cargos>();
                LogMsg.Append(" 原商品结存数量为");
                LogMsg.Append(UpdateCargos.Total.ToString());
                UpdateCargos.Total = int.Parse(tbxRowTotal.Text);
                LogMsg.Append("。更新后商品结存数量为");
                LogMsg.Append(UpdateCargos.Total.ToString());

                LogMsg.Append(" 原商品机动量为");
                LogMsg.Append(UpdateCargos.Variation.ToString());
                UpdateCargos.Variation = int.Parse(tbxRowVariation.Text);
                LogMsg.Append("。更新后商品机动量为");
                LogMsg.Append(UpdateCargos.Variation.ToString());

                //新增或更新渠道部分配量
                string DeptChannelNo = "20103";
                SetNewOrUpdateModel(DeptChannelNo, cargoCode, int.Parse(tbxRowDeptChannelAllocation.Text), ref LogMsg, ref context);
                

                //新增或更新销售部分配量
                string DeptSaleNo = "20102";
                SetNewOrUpdateModel(DeptSaleNo, cargoCode, int.Parse(tbxRowDeptSaleAllocation.Text), ref LogMsg, ref context);
                

                //新增或更新零售中心部分配量
                string DeptRetailNo = "20105";//零售中心这里对应营业部
                SetNewOrUpdateModel(DeptRetailNo, cargoCode, int.Parse(tbxRowDeptRetailAllocation.Text), ref LogMsg, ref context);
                

                //新增或更新电子商务分配量
                string DeptECNo = "20110";
                SetNewOrUpdateModel(DeptECNo, cargoCode, int.Parse(tbxRowDeptECAllocation.Text), ref LogMsg, ref context);
                

                //新增或更新公用部门分配量
                string DeptPublicNo = "99999";
                SetNewOrUpdateModel(DeptPublicNo, cargoCode, int.Parse(tbxRowDeptPublicAllocation.Text), ref LogMsg, ref context);


                //以上新增和更新使用隐式事务
                int affectRows = context.SaveChanges();
                saveMsg = "影响行数：" + affectRows.ToString();

                context.Dispose();

                return affectRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                saveMsg = Utility.LogHelper.GetExceptionMsg(ex);

                return false;
            }
        }

        //对每个部门保存分配量
        void SetNewOrUpdateModel(string deptNo, string cargoCode, int newAllocationNumber, ref System.Text.StringBuilder LogMsg, ref DAL.GoldEntities context)
        {
            //新增或更新渠道部分配量

            SaleAllocation existDeptChannel = (from r in context.SaleAllocation where r.DepartmentCode == deptNo && r.CargoCode == cargoCode select r).FirstOrDefault<SaleAllocation>();
            if (existDeptChannel != null)//更新
            {
                LogMsg.Append(" 部门[");
                LogMsg.Append(deptNo);
                LogMsg.Append("]分配量由");
                LogMsg.Append(existDeptChannel.Allocation.ToString());
                LogMsg.Append("更改为");
                LogMsg.Append(newAllocationNumber.ToString());

                existDeptChannel.Allocation = newAllocationNumber;
            }
            else //新增
            {
                LogMsg.Append(" 部门[");
                LogMsg.Append(deptNo);
                LogMsg.Append("]分配量首次设置为");
                LogMsg.Append(newAllocationNumber.ToString());

                SaleAllocation newAllocation = new SaleAllocation();
                newAllocation.Allocation = newAllocationNumber;
                newAllocation.CargoCode = cargoCode;
                newAllocation.DepartmentCode = deptNo;

                context.SaleAllocation.AddObject(newAllocation);
            }
        }

        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定型号
                var allModelList = (from r in context.Models orderby r.ModelName select new { r.ModelId, r.ModelName }).ToList();
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

                ////绑定可售状态
                //List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.CargoSaleStatus));
                //DropDownList_SaleStatus.Items.Clear();
                //DropDownList_SaleStatus.DataTextField = "Name";
                //DropDownList_SaleStatus.DataValueField = "Value";
                //DropDownList_SaleStatus.DataSource = ListBinType;
                //DropDownList_SaleStatus.DataBind();
                //DropDownList_SaleStatus.Items.Insert(0, new ListItem("", ""));
            }
        }

        void ClearTipMsgLabel()
        {
            //lblCheckMsg.Text = "";
            //lblAddMsg.Text = "";
            lblGridViewMsg.Text = "";
        }

        protected void gv_SaleAllocationList_Sorting(object sender, GridViewSortEventArgs e)
        {
            //保存sortExpression和sortDirection。
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";
            if (sortExpression.Equals(gv_SaleAllocationList.Attributes["sortExpression"]) && "ASC".Equals(gv_SaleAllocationList.Attributes["sortDirection"]))
            {
                sortDirection = "DESC";
            }
            gv_SaleAllocationList.Attributes.Add("sortExpression", sortExpression);
            gv_SaleAllocationList.Attributes.Add("sortDirection", sortDirection);

            GridViewBind();
        }

        protected void gv_SaleAllocationList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gv_SaleAllocationList.EditIndex = e.NewEditIndex;
            GridViewBind();
        }

        protected void gv_SaleAllocationList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gv_SaleAllocationList.EditIndex = -1;
            GridViewBind();
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
                gv_SaleAllocationList.Sort(sortExpression, gv_SaleAllocationList.SortDirection);
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