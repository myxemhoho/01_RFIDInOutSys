using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

using Gold.Utility;

namespace Gold.BaseInfoSetting
{
    public partial class StorageBinList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNewPanelDropDownList();
                gv_BinList.PageSize = Utility.WebConfigHelper.Instance.GetDefaultPageSize();
            }

            //这里要注意，一定要放在if (!this.IsPostBack)之外
            DisplayFixHeader(this.gv_BinList);//生成固定表头
        }

        #region 查询

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuery_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 分页

        /// <summary>
        /// 分页导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

            gv_BinList.DataBind();
        }





        /// <summary>
        /// GridView数据绑定完成，设置分页控件状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_DataBound(object sender, EventArgs e)
        {
            string sjs = "GetResultFromServer();";
            ScriptManager.RegisterClientScriptBlock(this.gv_BinList, this.GetType(), "", sjs, true);

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

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
        }

        #endregion



        #region 编辑

        /// <summary>
        /// 行命令触发前先清空界面消息提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    try
                    {
                        using (GoldEntities context = new GoldEntities())
                        {
                            StorageBin updateModel = (from r in context.StorageBin where r.BinCode == editBinCode select r).FirstOrDefault();
                            string msg = "";
                            if (GetUpdateModel(ref updateModel, senderGrid.Rows[senderGrid.EditIndex], out msg) == false)
                            {
                                lblGridViewMsg.Text = msg;
                                return;
                            }
                            else
                            {
                                int result = context.SaveChanges();
                                if (result > 0)
                                {
                                    lblGridViewMsg.Text = "更新成功！";
                                    senderGrid.EditIndex = -1;//取消编辑状态
                                }
                                else
                                    lblGridViewMsg.Text = "更新失败（影响行数为0）";

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblGridViewMsg.Text = "更新失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
                    }
                }
            }
            else
            {
                if (e.CommandName == "TagAlarmStart" || e.CommandName == "TagAlarmStop")
                {


                    string shortMsg = "";
                    string detailMsg = "";
                    string binTagID = "";
                    bool result = false;

                    GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                    Label lblRowBinTagID = drv == null ? null : drv.FindControl("lblRowBinTagID") as Label;
                    Label lblRowTagTestShortMsg = drv == null ? null : drv.FindControl("lblRowTagTestShortMsg") as Label;//非编辑状态下 显示标签编码的控件
                    TextBox tbxRowBinTagID = drv == null ? null : drv.FindControl("tbxRowBinTagID") as TextBox;//编辑状态下 显示标签编码的控件

                    //先清空提示
                    lblRowTagTestShortMsg.Text = "";
                    lblGridViewMsg.Text = "";


                    if (lblRowBinTagID == null && tbxRowBinTagID == null)
                    {
                        lblGridViewMsg.Text = "获取列表中层位标签标签信息失败！";
                        shortMsg = lblGridViewMsg.Text;
                        return;
                    }
                    else
                    {
                        if (senderGrid.EditIndex == -1 && lblRowBinTagID != null)//非编辑态下获取标签编码
                            binTagID = lblRowBinTagID.Text.Trim();
                        else if (senderGrid.EditIndex != -1 && tbxRowBinTagID != null)//编辑态下获取标签编码
                            binTagID = tbxRowBinTagID.Text.Trim();
                    }

                    if (e.CommandName == "TagAlarmStart")
                    {
                        result = StartOrStopAlarm(binTagID, true, out shortMsg, out detailMsg);
                    }
                    else if (e.CommandName == "TagAlarmStop")
                    {
                        result = StartOrStopAlarm(binTagID, false, out shortMsg, out detailMsg);
                    }

                    lblRowTagTestShortMsg.Text = shortMsg;
                    lblGridViewMsg.Text = detailMsg;

                    gv_BinList.DataBind();//亮灯和关灯后手持机服务会改标签数据库字段状态，所以这里刷新数据源
                }
            }
        }


        /// <summary>
        /// 标签报警测试
        /// </summary>
        /// <param name="binTagID">标签编码</param>
        /// <param name="IsStartAlarm">true-开始报警，false-停止报警</param>
        /// <param name="shortMsg">调用报警函数简短消息提示</param>
        /// <param name="detailMsg">调用报警函数详细消息提示</param>
        /// <returns></returns>
        private bool StartOrStopAlarm(string binTagID, bool IsStartAlarm, out string shortMsg, out string detailMsg)
        {
            try
            {
                int BinTagLightAlartCount = 3;//层位标签报警测试时亮灯次数
                int BinTagSoundAlartCount = 3;//层位标签报警测试时鸣音次数
                bool ServiceResult = false;
                if (string.IsNullOrEmpty(binTagID))
                {
                    shortMsg = "标签编码不能为空";
                    detailMsg = "用于报警测试的标签编码不能为空！请编辑标签编码后再进行报警测试！";
                }

                ////从配置文件读取亮灯次数
                //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("BinTagLightAlartCount"))
                //{
                //    int temp = 0;
                //    string configValue = System.Configuration.ConfigurationManager.AppSettings["BinTagLightAlartCount"].ToString();
                //    if (int.TryParse(configValue, out temp))
                //        BinTagLightAlartCount = temp;
                //}

                ////从配置文件读取鸣笛次数
                //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("BinTagSoundAlartCount"))
                //{
                //    int temp = 0;
                //    string configValue = System.Configuration.ConfigurationManager.AppSettings["BinTagSoundAlartCount"].ToString();
                //    if (int.TryParse(configValue, out temp))
                //        BinTagSoundAlartCount = temp;
                //}

                //从数据库数据字典读取亮点次数
                BinTagLightAlartCount = DAL.DbCommonMethod.GetTagFirstLightCount();

                //从数据库数据字典读取鸣音次数
                BinTagSoundAlartCount = DAL.DbCommonMethod.GetTagFirstSoundCount();

                ServiceReference_DeviceService.DeviceServiceClient client = new ServiceReference_DeviceService.DeviceServiceClient();
                client.Open();
                if (IsStartAlarm)
                {
                    //调用服务端标签报警函数
                    ServiceResult = client.TagControl(binTagID, BinTagLightAlartCount, BinTagSoundAlartCount);
                }
                else
                {
                    //调用服务端标签报警函数
                    ServiceResult = client.TagControl(binTagID, 0, 0);
                }
                client.Close();

                if (ServiceResult)
                {
                    shortMsg = IsStartAlarm ? "报警已开启" : "报警已停止";
                    detailMsg = "标签[" + binTagID + "]已" + (IsStartAlarm ? "开启报警！" : "停止报警");
                    return true;
                }
                else
                {
                    shortMsg = IsStartAlarm ? "报警开启失败" : "报警停止失败";
                    detailMsg = "标签[" + binTagID + "]" + (IsStartAlarm ? "报警开启失败！" : "报警停止失败！") + "。服务端返回False。";
                    return false;
                }

            }
            catch (Exception ex)
            {
                shortMsg = IsStartAlarm ? "报警开启失败" : "报警停止失败";
                detailMsg = "标签[" + binTagID + "]" + (IsStartAlarm ? "报警开启失败！" : "报警停止失败！") + "。详细原因：" + LogHelper.GetExceptionMsg(ex);
                return false;
            }
        }

        /// <summary>
        /// 在行数据绑定完成后绑定编辑界面的DropDownList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView theGrid = sender as GridView;

            //将数据行中的层位类型和标签状态代码转换成名称显示
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblRowBinType = e.Row.FindControl("lblRowBinType") as Label;
                Label lblRowBinTypeName = e.Row.FindControl("lblRowBinTypeName") as Label;
                if (lblRowBinType != null && lblRowBinTypeName != null)
                {
                    lblRowBinTypeName.Text = GetBinTypeName(lblRowBinType.Text.Trim());
                }

                Label lblRowBinTagStatus = e.Row.FindControl("lblRowBinTagStatus") as Label;
                Label lblRowBinTagStatusName = e.Row.FindControl("lblRowBinTagStatusName") as Label;
                if (lblRowBinTagStatus != null && lblRowBinTagStatusName != null)
                {
                    lblRowBinTagStatusName.Text = GetBinTagStatusName(lblRowBinTagStatus.Text.Trim());
                }

                //给Button加上客户端javascript函数
                Label lblBinCode = e.Row.FindControl("lblBinCode") as Label;
                Button btnRowTagTestAlarmStart = e.Row.FindControl("btnRowTagTestAlarmStart") as Button;
                Button btnRowTagTestAlarmStop = e.Row.FindControl("btnRowTagTestAlarmStop") as Button;
                Label lblRowTagTestShortMsg = e.Row.FindControl("lblRowTagTestShortMsg") as Label;
                string divID = "waitDiv_" + lblBinCode.Text.Trim();
                btnRowTagTestAlarmStart.OnClientClick = "showWaitDiv('" + divID + "');clearLabelText('" + lblRowTagTestShortMsg.ClientID + "');";//使用js显示进度并清除单元格中的消息提示文字
                btnRowTagTestAlarmStop.OnClientClick = "showWaitDiv('" + divID + "');clearLabelText('" + lblRowTagTestShortMsg.ClientID + "');";
            }



            if (theGrid.EditIndex != -1)
            {
                if ((e.Row.RowIndex == theGrid.EditIndex) &&
                    (e.Row.RowType == DataControlRowType.DataRow) &&
                    (e.Row.RowState.HasFlag(DataControlRowState.Edit) &&
                    (e.Row.DataItem != null)))
                {
                    //GridViewRow editRow = theGrid.Rows[theGrid.EditIndex]; //GridView较DataGrid提供了更多的API，获取分页块可以使用BottomPagerRow 或者TopPagerRow，当然还增加了HeaderRow和FooterRow

                    DropDownList dropdownList_EditBinTagStatus = null;
                    DropDownList dropdownList_EditBinType = null;
                    DropDownList dropdownList_EditWareHouse = null;

                    Label lblRowBinTypeEdit = null;
                    Label lblRowWareHouseCodeEdit = null;
                    Label lblRowBinTagStatusEdit = null;
                    GridViewRow editRow = e.Row;


                    if (null != editRow)
                    {
                        dropdownList_EditBinType = editRow.FindControl("dropdownList_binType") as DropDownList;
                        dropdownList_EditWareHouse = editRow.FindControl("dropdownList_WareHouse") as DropDownList;
                        dropdownList_EditBinTagStatus = editRow.FindControl("dropdownList_BinTagStatus") as DropDownList;

                        lblRowBinTypeEdit = editRow.FindControl("lblRowBinTypeEdit") as Label;
                        lblRowWareHouseCodeEdit = editRow.FindControl("lblRowWareHouseCodeEdit") as Label;
                        lblRowBinTagStatusEdit = editRow.FindControl("lblRowBinTagStatusEdit") as Label;
                    }


                    if (dropdownList_EditBinType != null)
                    {
                        //绑定层位类型
                        List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.BinTypeEnum));
                        dropdownList_EditBinType.Items.Clear();
                        dropdownList_EditBinType.DataTextField = "Name";
                        dropdownList_EditBinType.DataValueField = "Value";
                        dropdownList_EditBinType.DataSource = ListBinType;
                        dropdownList_EditBinType.DataBind();

                        //设置选中项
                        if (lblRowBinTypeEdit != null)
                        {
                            dropdownList_EditBinType.SelectedIndex = dropdownList_EditBinType.Items.IndexOf(dropdownList_EditBinType.Items.FindByValue(lblRowBinTypeEdit.Text.Trim()));
                        }
                    }

                    if (dropdownList_EditBinTagStatus != null)
                    {
                        //绑定标签状态
                        List<NameValueModel> ListBinTagStatus = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.BinTagStatusEnum));
                        dropdownList_EditBinTagStatus.Items.Clear();
                        dropdownList_EditBinTagStatus.DataTextField = "Name";
                        dropdownList_EditBinTagStatus.DataValueField = "Value";
                        dropdownList_EditBinTagStatus.DataSource = ListBinTagStatus;
                        dropdownList_EditBinTagStatus.DataBind();

                        //设置选中项
                        if (lblRowBinTagStatusEdit != null)
                        {
                            dropdownList_EditBinTagStatus.SelectedIndex = dropdownList_EditBinTagStatus.Items.IndexOf(dropdownList_EditBinTagStatus.Items.FindByValue(lblRowBinTagStatusEdit.Text.Trim()));
                        }
                    }

                    if (dropdownList_EditWareHouse != null)
                    {
                        using (GoldEntities context = new GoldEntities())
                        {
                            var result = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                            dropdownList_EditWareHouse.Items.Clear();
                            dropdownList_EditWareHouse.DataTextField = "WHName";
                            dropdownList_EditWareHouse.DataValueField = "WHCode";
                            dropdownList_EditWareHouse.DataSource = result;
                            dropdownList_EditWareHouse.DataBind();
                        }

                        //设置选中项
                        if (lblRowWareHouseCodeEdit != null)
                        {
                            dropdownList_EditWareHouse.SelectedIndex = dropdownList_EditWareHouse.Items.IndexOf(dropdownList_EditWareHouse.Items.FindByValue(lblRowWareHouseCodeEdit.Text));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetUpdateModel(ref StorageBin updateModel, GridViewRow editRow, out string msg)
        {
            msg = "";
            try
            {
                if (null == editRow)
                {
                    msg = "获取的编辑行为空！请重试！";
                    return false;
                }

                DropDownList dropdownList_EditBinTagStatus = null;
                DropDownList dropdownList_EditBinType = null;
                DropDownList dropdownList_EditWareHouse = null;
                TextBox tbxRowBinName = null;
                TextBox tbxRowBinTagID = null;
                TextBox tbxRowComment = null;

                dropdownList_EditBinType = editRow.FindControl("dropdownList_binType") as DropDownList;
                dropdownList_EditWareHouse = editRow.FindControl("dropdownList_WareHouse") as DropDownList;
                dropdownList_EditBinTagStatus = editRow.FindControl("dropdownList_BinTagStatus") as DropDownList;
                tbxRowBinName = editRow.FindControl("tbxRowBinName") as TextBox;
                tbxRowBinTagID = editRow.FindControl("tbxRowBinTagID") as TextBox;
                tbxRowComment = editRow.FindControl("tbxRowComment") as TextBox;


                if (string.IsNullOrEmpty(dropdownList_EditBinType.SelectedItem.Value) ||
                    string.IsNullOrEmpty(dropdownList_EditBinTagStatus.SelectedItem.Value) ||
                    string.IsNullOrEmpty(dropdownList_EditWareHouse.SelectedItem.Value))
                {
                    msg = "层位类型、标签状态、所属仓库 均为必填项！请填写！";
                    return false;
                }
                string binName = tbxRowBinName.Text.Trim();
                string binCode = updateModel.BinCode;

                int binType = 1;
                int binTagStatus = 1;

                //updateModel.BinCode = binCode;
                updateModel.BinName = binName;
                int.TryParse(dropdownList_EditBinType.SelectedItem.Value.ToString(), out binType);
                int.TryParse(dropdownList_EditBinTagStatus.SelectedItem.Value.ToString(), out binTagStatus);
                updateModel.BinType = binType;
                updateModel.BinTagStatus = binTagStatus;
                updateModel.WareHouse = dropdownList_EditWareHouse.SelectedItem.Value.ToString();
                updateModel.BinTagID = tbxRowBinTagID.Text.Trim();
                updateModel.Comment = tbxRowComment.Text.Trim();

                string whCode = updateModel.WareHouse;

                using (GoldEntities context = new GoldEntities())
                {
                    var sameName = (from r in context.StorageBin where (r.BinName == binName && r.BinCode != binCode && r.WareHouse == whCode) select r).ToList();
                    if (sameName != null && sameName.Count > 0)
                        msg += "系统内仓库[" + whCode + "]中已经存在名称为[" + binName + "]的层位信息,请重填名称!<br />";

                    string newBinTagID = updateModel.BinTagID;
                    StorageBin existTagIDBin = (from r in context.StorageBin where (r.BinTagID == newBinTagID && r.BinCode != binCode && r.WareHouse == whCode) select r).FirstOrDefault();
                    if (existTagIDBin != null)
                        msg += "电子标签[" + updateModel.BinTagID + "]已经在仓库[" + whCode + "]层位[" + existTagIDBin.BinCode + "]被注册过了！请重新填写未注册的标签编号！<br />";


                    if (msg.Length > 0)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }

        }


        #endregion

        #region 行删除

        /// <summary>
        /// 行删除完成后进行界面提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_BinList_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.AffectedRows > 0)
                lblGridViewMsg.Text = "删除成功！";
            else
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(e.Exception) + "]";
        }

        #endregion

        #region 多项删除

        /// <summary>
        /// 获取已选中行
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckItemID()
        {
            List<string> CheckIDList = new List<string>();
            for (int i = 0; i < gv_BinList.Rows.Count; i++)
            {
                GridViewRow currentRow = gv_BinList.Rows[i];
                if (currentRow != null)
                {
                    string itemID = (currentRow.FindControl("lblBinCode") as Label).Text.Trim();
                    CheckBox currentCbx = currentRow.FindControl("gvChk") as CheckBox;

                    if (currentCbx.Checked && !CheckIDList.Contains(itemID))
                    {
                        CheckIDList.Add(itemID);
                    }
                }
            }
            return CheckIDList;
        }

        /// <summary>
        /// 多项删除按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ClearTipMsgLabel();//清除提示信息

                List<string> checkedDelList = GetCheckItemID();
                if (checkedDelList.Count <= 0)
                {
                    lblCheckMsg.Text = "请先选中待删除的项";
                    return;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    foreach (string delCode in checkedDelList)
                    {
                        DAL.StorageBin delObject = (from r in context.StorageBin where r.BinCode.Equals(delCode) select r).FirstOrDefault();

                        context.DeleteObject(delObject);
                    }
                    int delRow = context.SaveChanges();
                    if (delRow > 0)
                        lblCheckMsg.Text = "删除成功！[已删除" + delRow.ToString() + "项]";
                    else
                        lblCheckMsg.Text = "删除失败！";

                    //GridViewBind();//删除后重新绑定数据
                    gv_BinList.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblGridViewMsg.Text = "删除失败[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
            }
        }
        #endregion

        #region 新增

        /// <summary>
        /// 加载新增Panel中的dropdownlist数据
        /// </summary>
        void LoadNewPanelDropDownList()
        {
            //绑定层位类型
            List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.BinTypeEnum));
            dropdownList_NewBinType.Items.Clear();
            dropdownList_NewBinType.DataTextField = "Name";
            dropdownList_NewBinType.DataValueField = "Value";
            dropdownList_NewBinType.DataSource = ListBinType;
            dropdownList_NewBinType.DataBind();

            //绑定标签状态
            List<NameValueModel> ListBinTagStatus = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.BinTagStatusEnum));
            dropdownList_NewBinTagStatus.Items.Clear();
            dropdownList_NewBinTagStatus.DataTextField = "Name";
            dropdownList_NewBinTagStatus.DataValueField = "Value";
            dropdownList_NewBinTagStatus.DataSource = ListBinTagStatus;
            dropdownList_NewBinTagStatus.DataBind();

            using (GoldEntities context = new GoldEntities())
            {
                var result = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                dropdownList_NewWareHouse.Items.Clear();
                dropdownList_NewWareHouse.DataTextField = "WHName";
                dropdownList_NewWareHouse.DataValueField = "WHCode";
                dropdownList_NewWareHouse.DataSource = result;
                dropdownList_NewWareHouse.DataBind();
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out StorageBin newModel, out string msg)
        {
            msg = "";
            newModel = null;
            try
            {
                if (string.IsNullOrEmpty(dropdownList_NewBinType.SelectedItem.Value) ||
                    string.IsNullOrEmpty(dropdownList_NewBinTagStatus.SelectedItem.Value) ||
                    string.IsNullOrEmpty(dropdownList_NewWareHouse.SelectedItem.Value))
                {
                    msg = "层位类型、标签状态、所属仓库 均为必填项！请填写！";
                    return false;
                }

                newModel = new StorageBin();
                string binCode = tbxNewBinCode.Text.Trim();
                string binName = tbxNewBinName.Text.Trim();
                int binType = 1;
                int binTagStatus = 1;

                newModel.BinCode = binCode;
                newModel.BinName = binName;
                int.TryParse(dropdownList_NewBinType.SelectedItem.Value.ToString(), out binType);
                int.TryParse(dropdownList_NewBinTagStatus.SelectedItem.Value.ToString(), out binTagStatus);
                newModel.BinType = binType;
                newModel.BinTagStatus = binTagStatus;
                newModel.WareHouse = dropdownList_NewWareHouse.SelectedItem.Value.ToString();
                newModel.BinTagID = tbxNewBinTagID.Text.Trim();
                newModel.Comment = tbxNewComment.Text.Trim();

                using (GoldEntities context = new GoldEntities())
                {
                    var sameCode = (from r in context.StorageBin where r.BinCode == binCode select r).ToList();
                    if (sameCode != null && sameCode.Count > 0)
                        msg += "系统中已经存在编号为[" + binCode + "]的层位信息,请重填编号!<br />";
                    var sameName = (from r in context.StorageBin where r.BinName == binName select r).ToList();
                    if (sameName != null && sameName.Count > 0)
                        msg += "系统中已经存在名称为[" + binName + "]的层位信息,请重填名称!<br />";

                    string newBinTagID = newModel.BinTagID;
                    StorageBin existTagIDBin = (from r in context.StorageBin where r.BinTagID == newBinTagID select r).FirstOrDefault();
                    if (existTagIDBin != null)
                        msg += "电子标签[" + newModel.BinTagID + "]已经在层位[" + existTagIDBin.BinCode + "]被注册过了！请重新填写未注册的标签编号！<br />";


                    if (msg.Length > 0)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }

        }

        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid == false)
                return;

            //lblAddMsg.Text = "";
            ClearTipMsgLabel();//清除提示信息
            try
            {
                StorageBin newModel = null;
                string msg = "";
                if (GetNewModel(out newModel, out msg) == false)
                {
                    lblAddMsg.Text = msg;
                    return;
                }
                using (DAL.GoldEntities context = new DAL.GoldEntities())
                {
                    context.StorageBin.AddObject(newModel);
                    context.SaveChanges();
                    lblAddMsg.Text = "保存成功";

                    gv_BinList.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblAddMsg.Text = "保存失败！详细信息：" + Utility.LogHelper.GetExceptionMsg(ex);
            }
        }

        /// <summary>
        /// 清空新增栏控件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbxNewBinCode.Text = "";
            tbxNewBinName.Text = "";
            tbxNewComment.Text = "";
            tbxNewBinTagID.Text = "";
            lblAddMsg.Text = "";
        }

        #endregion




        /// <summary>
        /// 将GridView行中标签类型码转换为标签类型名称
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        string GetBinTypeName(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
                return "";
            try
            {
                EnumData.BinTypeEnum currentEnum = (EnumData.BinTypeEnum)Enum.Parse(typeof(EnumData.BinTypeEnum), enumValue);
                return Gold.Utility.EnumData.GetEnumDesc(currentEnum);
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetBinTypeName", ex);
                return "";
            }
        }

        /// <summary>
        /// 将GridView行中标签类型码转换为标签类型名称
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        string GetBinTagStatusName(string enumValue)
        {
            try
            {
                EnumData.BinTagStatusEnum currentEnum = (EnumData.BinTagStatusEnum)Enum.Parse(typeof(EnumData.BinTagStatusEnum), enumValue);
                return Gold.Utility.EnumData.GetEnumDesc(currentEnum);
            }
            catch (Exception ex)
            {
                Utility.LogHelper.WriteLog(LogHelper.LogLevel.Error, "GetBinTagStatusName", ex);
                return "";
            }
        }

        void ClearTipMsgLabel()
        {
            lblCheckMsg.Text = "";
            lblAddMsg.Text = "";
            lblGridViewMsg.Text = "";
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
                SortDirection sortDirection = gv_BinList.SortDirection;
                SortDirection newSortDirection;
                switch (gv_BinList.SortDirection)
                {
                    case SortDirection.Ascending: newSortDirection = SortDirection.Descending; break;//取反
                    case SortDirection.Descending: newSortDirection = SortDirection.Ascending; break;//取反
                    default: newSortDirection = SortDirection.Ascending; break;
                }
                gv_BinList.Sort(sortExpression, newSortDirection);
                gv_BinList.DataBind();//因使用的是数据源控件，所以要重新绑定
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), "hiddenWaitDiv('divWait');", true);//js提示
            }
        }

        #endregion
    }
}