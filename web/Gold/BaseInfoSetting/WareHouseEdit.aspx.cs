using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;

namespace Gold.BaseInfoSetting
{
    public partial class WareHouseEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 当前页面是编辑还是新增； 1为新增 2为编辑            
            string EditType = "1";

            // 由管理页面传来的待更新的记录ID            
            string EditID = "0";

            if (!this.IsPostBack)
            {
                if (Request.QueryString.Count > 0)
                {
                    EditType = Request.QueryString["EditType"].ToString().Trim();
                    EditID = Request.QueryString["EditID"].ToString().Trim();

                }
                ViewState["EditType"] = EditType;
                ViewState["EditID"] = EditID;

                if (EditType == "1")
                {
                    lblTitle.Text = "仓库信息新增";
                    tbxWHCode.Enabled = true;
                }
                else
                {
                    lblTitle.Text = "仓库信息编辑";
                    tbxWHCode.Enabled = false;
                    LoadInfo(EditID);
                }
            }
        }

        private void LoadInfo(string ItemID)
        {
            using (GoldEntities context = new GoldEntities())
            {
                WareHouse existModel = (from r in context.WareHouse where r.WHCode == ItemID select r).FirstOrDefault();
                if (existModel != null)
                {
                    tbxWHCode.Text = existModel.WHCode;
                    tbxWHName.Text = existModel.WHName;
                    DropDownList_WHType.SelectedIndex = DropDownList_WHType.Items.IndexOf(DropDownList_WHType.Items.FindByValue(existModel.WHType.Value.ToString()));
                    DropDownList_Enabled.SelectedIndex = DropDownList_Enabled.Items.IndexOf(DropDownList_Enabled.Items.FindByValue(existModel.Enabled.Value.ToString()));
                    tbxAddress.Text = existModel.Address;
                    tbxPhone.Text = existModel.Phone;
                    tbxComment.Text = existModel.Comment;
                }
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out WareHouse newModel, out string msg)
        {
            msg = "";
            newModel = null;
            try
            {
                newModel = new WareHouse();
                string whCode = tbxWHCode.Text.Trim();
                string whName = tbxWHName.Text.Trim();
                int whType = 0;
                bool whEnabled = false;


                newModel.WHCode = whCode;
                newModel.WHName = whName;
                int.TryParse(DropDownList_WHType.SelectedItem.Value.ToString(), out whType);
                bool.TryParse(DropDownList_Enabled.SelectedItem.Value.ToString(), out whEnabled);
                newModel.WHType = whType;
                newModel.Enabled = whEnabled;
                newModel.Address = tbxAddress.Text.Trim();
                newModel.Phone = tbxPhone.Text.Trim();
                newModel.Comment = tbxComment.Text.Trim();

                using (GoldEntities context = new GoldEntities())
                {
                    var sameCode = (from r in context.WareHouse where r.WHCode == whCode select r).ToList();
                    if (sameCode != null && sameCode.Count > 0)
                        msg += "系统中已经存在编号为[" + whCode + "]的仓库信息,请重填编号";
                    var sameName = (from r in context.WareHouse where r.WHName == whName select r).ToList();
                    if (sameName != null && sameName.Count > 0)
                        msg += "系统中已经存在名称为[" + whName + "]的仓库信息,请重填名称";

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
        /// 根据界面数据填充更新的对象
        /// </summary>
        /// <param name="updateModel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GetUpdateModel(ref WareHouse updateModel, out string msg)
        {
            msg = "";

            try
            {
                string whCode = tbxWHCode.Text.Trim();
                string whName = tbxWHName.Text.Trim();
                int whType = 0;
                bool whEnabled = false;

                updateModel.WHName = whName;
                int.TryParse(DropDownList_WHType.SelectedItem.Value.ToString(), out whType);
                bool.TryParse(DropDownList_Enabled.SelectedItem.Value.ToString(), out whEnabled);
                updateModel.WHType = whType;
                updateModel.Enabled = whEnabled;
                updateModel.Address = tbxAddress.Text.Trim();
                updateModel.Phone = tbxPhone.Text.Trim();
                updateModel.Comment = tbxComment.Text.Trim();

                using (GoldEntities context = new GoldEntities())
                {//查询新仓库名是否与现有其他仓库记录冲突
                    var sameName = (from r in context.WareHouse where (r.WHName == whName && r.WHCode != whCode) select r).ToList();
                    if (sameName != null && sameName.Count > 0)
                        msg += "系统中已经存在名称为[" + whName + "]的仓库信息,请重填名称";

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblSaveMsg.Text = "";
                string outMsg = "";
                if (ViewState["EditType"].ToString() == "1")//新增Insert
                {
                    DAL.WareHouse newModel = new WareHouse();
                    bool getNewOK = GetNewModel(out newModel, out outMsg);
                    if (getNewOK == false)
                    {
                        lblSaveMsg.Text = "新增失败！[" + outMsg + "]";
                        return;
                    }
                    using (GoldEntities context = new GoldEntities())
                    {
                        context.WareHouse.AddObject(newModel);
                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功";
                    }
                }
                else//更新Update 
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        string EditID = ViewState["EditID"].ToString();
                        DAL.WareHouse updateModel = (from r in context.WareHouse where r.WHCode == EditID select r).FirstOrDefault();
                        bool getUpdateOK = GetUpdateModel(ref updateModel, out outMsg);
                        if (getUpdateOK == false)
                        {
                            lblSaveMsg.Text = "更新失败！[" + outMsg + "]";
                            return;
                        }
                        //context.WareHouse.Attach(updateModel);
                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功";
                    }
                }
            }
            catch (Exception ex)
            {
                lblSaveMsg.Text = "更新失败！[" + Utility.LogHelper.GetExceptionMsg(ex) + "]";
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbxWHCode.Text = "";
            tbxWHName.Text = "";
            tbxAddress.Text = "";
            tbxPhone.Text = "";
            tbxComment.Text = "";

            lblSaveMsg.Text = "";
        }
    }
}