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
    public partial class CargoEdit : System.Web.UI.Page
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
                    lblTitle.Text = "商品信息新增";
                    tbxCargoCode.Enabled = true;
                    LoadInfo("-1");//新增
                }
                else
                {
                    lblTitle.Text = "商品信息编辑";
                    tbxCargoCode.Enabled = false;
                    LoadInfo(EditID);
                }
            }
        }

        private void LoadInfo(string ItemID)
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

                //绑定可售状态
                List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.CargoSaleStatus));
                DropDownList_SaleStatus.Items.Clear();
                DropDownList_SaleStatus.DataTextField = "Name";
                DropDownList_SaleStatus.DataValueField = "Value";
                DropDownList_SaleStatus.DataSource = ListBinType;
                DropDownList_SaleStatus.DataBind();


                //编辑状态下进入此页面时
                if (ItemID != "-1")
                {
                    Cargos existModel = (from r in context.Cargos where r.CargoCode == ItemID select r).FirstOrDefault();
                    if (existModel != null)
                    {
                        tbxCargoCode.Text = existModel.CargoCode;
                        tbxCargoName.Text = existModel.CargoName;

                        //设置选中项
                        DropDownList_CargoModel.SelectedIndex = DropDownList_CargoModel.Items.IndexOf(DropDownList_CargoModel.Items.FindByText(existModel.CargoModel.Trim()));
                        DropDownList_CargoSpec.SelectedIndex = DropDownList_CargoSpec.Items.IndexOf(DropDownList_CargoSpec.Items.FindByText(existModel.CargoSpec.Trim()));

                        tbxUnits.Text = existModel.CargoUnits;
                        tbxYear.Text = existModel.ProduceYear;

                        DropDownList_SaleStatus.SelectedIndex = DropDownList_SaleStatus.Items.IndexOf(DropDownList_SaleStatus.Items.FindByValue(existModel.SaleStatus == null ? "" : existModel.SaleStatus.Value.ToString()));
                        tbxPrice1.Text = existModel.Price1 == null ? "" : existModel.Price1.Value.ToString();
                        tbxPrice2.Text = existModel.Price2 == null ? "" : existModel.Price2.Value.ToString();
                        tbxPrice3.Text = existModel.Price3 == null ? "" : existModel.Price3.Value.ToString();
                        tbxPrice4.Text = existModel.Price4 == null ? "" : existModel.Price4.Value.ToString();
                        tbxPrice5.Text = existModel.Price5 == null ? "" : existModel.Price5.Value.ToString();

                        tbxComment.Text = existModel.Comment;
                    }
                }
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out Cargos newModel, out string msg)
        {
            msg = "";
            newModel = null;
            try
            {
                newModel = new Cargos();
                string cargoCode = tbxCargoCode.Text.Trim();
                newModel.CargoCode = cargoCode;
                newModel.CargoName = tbxCargoName.Text.Trim();
                newModel.CargoType = 1;//该字段暂不用，设定为默认值1

                if (DropDownList_CargoModel.SelectedItem == null || DropDownList_CargoSpec.SelectedItem == null
                    || DropDownList_CargoModel.SelectedItem.Text.Trim() == "" || DropDownList_CargoSpec.SelectedItem.Text.Trim() == "")
                {
                    msg = "商品型号和规格不能为空，请选择！";
                    return false;
                }

                newModel.CargoModel = DropDownList_CargoModel.SelectedItem.Text.Trim();
                newModel.CargoSpec = DropDownList_CargoSpec.SelectedItem.Text.Trim();
                newModel.CargoUnits = tbxUnits.Text.Trim();
                newModel.ProduceYear = tbxYear.Text.Trim();
                newModel.SaleStatus = int.Parse(DropDownList_SaleStatus.Text.Trim() == "" ? null : DropDownList_SaleStatus.Text.Trim());

                decimal price1 = 0, price2 = 0, price3 = 0, price4 = 0, price5 = 0;

                decimal.TryParse(tbxPrice1.Text.Trim(), out price1);
                decimal.TryParse(tbxPrice2.Text.Trim(), out price2);
                decimal.TryParse(tbxPrice3.Text.Trim(), out price3);
                decimal.TryParse(tbxPrice4.Text.Trim(), out price4);
                decimal.TryParse(tbxPrice5.Text.Trim(), out price5);

                newModel.Price1 = price1;
                newModel.Price2 = price2;
                newModel.Price3 = price3;
                newModel.Price4 = price4;
                newModel.Price5 = price5;

                newModel.Comment = tbxComment.Text.Trim();

                using (GoldEntities context = new GoldEntities())
                {
                    var sameCode = (from r in context.Cargos where r.CargoCode == cargoCode select r).ToList();
                    if (sameCode != null && sameCode.Count > 0)
                        msg += "系统中已经存在编号为[" + cargoCode + "]的商品信息,请重填编号";

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
        private bool GetUpdateModel(ref Cargos updateModel, out string msg)
        {
            msg = "";

            try
            {
                string cargoCode = tbxCargoCode.Text.Trim();
                updateModel.CargoCode = cargoCode;
                updateModel.CargoName = tbxCargoName.Text.Trim();
                updateModel.CargoType = 1;//该字段暂不用，设定为默认值1

                if (DropDownList_CargoModel.SelectedItem == null || DropDownList_CargoSpec.SelectedItem == null
                    || DropDownList_CargoModel.SelectedItem.Text.Trim() == "" || DropDownList_CargoSpec.SelectedItem.Text.Trim() == "")
                {
                    msg = "商品型号和规格不能为空，请选择！";
                    return false;
                }

                updateModel.CargoModel = DropDownList_CargoModel.SelectedItem.Text.Trim();
                updateModel.CargoSpec = DropDownList_CargoSpec.SelectedItem.Text.Trim();

                updateModel.CargoUnits = tbxUnits.Text.Trim();
                updateModel.ProduceYear = tbxYear.Text.Trim();
                updateModel.SaleStatus = int.Parse(DropDownList_SaleStatus.Text.Trim() == "" ? "0" : DropDownList_SaleStatus.Text.Trim());

                decimal price1 = 0, price2 = 0, price3 = 0, price4 = 0, price5 = 0;

                decimal.TryParse(tbxPrice1.Text.Trim(), out price1);
                decimal.TryParse(tbxPrice2.Text.Trim(), out price2);
                decimal.TryParse(tbxPrice3.Text.Trim(), out price3);
                decimal.TryParse(tbxPrice4.Text.Trim(), out price4);
                decimal.TryParse(tbxPrice5.Text.Trim(), out price5);

                updateModel.Price1 = price1;
                updateModel.Price2 = price2;
                updateModel.Price3 = price3;
                updateModel.Price4 = price4;
                updateModel.Price5 = price5;

                updateModel.Comment = tbxComment.Text.Trim();

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
                    DAL.Cargos newModel = new Cargos();
                    bool getNewOK = GetNewModel(out newModel, out outMsg);
                    if (getNewOK == false)
                    {
                        lblSaveMsg.Text = "新增失败！[" + outMsg + "]";
                        return;
                    }
                    using (GoldEntities context = new GoldEntities())
                    {
                        context.Cargos.AddObject(newModel);
                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功";
                    }
                }
                else//更新Update 
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        string EditID = ViewState["EditID"].ToString();
                        DAL.Cargos updateModel = (from r in context.Cargos where r.CargoCode == EditID select r).FirstOrDefault();
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
            tbxCargoCode.Text = "";
            tbxCargoName.Text = "";
            tbxUnits.Text = "";
            tbxYear.Text = "";
            tbxPrice1.Text = "";
            tbxPrice2.Text = "";
            tbxPrice3.Text = "";
            tbxPrice4.Text = "";
            tbxPrice5.Text = "";
            tbxComment.Text = "";

            lblSaveMsg.Text = "";
        }
    }
}