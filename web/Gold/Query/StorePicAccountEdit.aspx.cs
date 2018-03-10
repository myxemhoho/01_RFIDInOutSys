using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gold.DAL;
using Gold.Utility;

namespace Gold.Query
{
    public partial class StorePicAccountEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GoodsSelect1.PostBack += this.GoodsSelect1_PostBack;

            // 当前页面是编辑还是新增； 1为新增 2为编辑,3为复制新增            
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
                    lblTitle.Text = "存提记录新增";
                    tbxCargoCode.Enabled = true;
                    LoadInfo("-1");//新增
                }
                else
                {
                    lblTitle.Text = "存提记录编辑";
                    tbxCargoCode.Enabled = false;
                    LoadInfo(EditID);
                }
            }
        }

        private void LoadInfo(string ItemID)
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定存提状态
                List<NameValueModel> ListPickOrStore = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.PickOrStore));
                DropDownList_StorePickType.Items.Clear();
                DropDownList_StorePickType.DataTextField = "Name";
                DropDownList_StorePickType.DataValueField = "Value";
                DropDownList_StorePickType.DataSource = ListPickOrStore;
                DropDownList_StorePickType.DataBind();
                DropDownList_StorePickType.Items.Insert(0, new ListItem("", ""));

                //绑定包装
                var result = (from r in context.Packages select new { r.PackageId, r.PackageName }).OrderBy(r => r.PackageName).ToList();
                DropDownList_PackageName.Items.Clear();
                DropDownList_PackageName.DataTextField = "PackageName";
                DropDownList_PackageName.DataValueField = "PackageId";
                DropDownList_PackageName.DataSource = result;
                DropDownList_PackageName.DataBind();
                DropDownList_PackageName.Items.Insert(0, new ListItem("", ""));

                //绑定仓库
                var resultWH = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                DropDownList_WHCode.Items.Clear();
                DropDownList_WHCode.DataTextField = "WHName";
                DropDownList_WHCode.DataValueField = "WHCode";
                DropDownList_WHCode.DataSource = resultWH;
                DropDownList_WHCode.DataBind();
                DropDownList_WHCode.Items.Insert(0, new ListItem("", ""));


                //编辑状态下进入此页面时
                if (ItemID != "-1")
                {
                    tbxCargoCode.Enabled = false;
                    tbxCargoName.Enabled = false;
                    btnSelectCargo1.Enabled = false;
                    btnSelectCargo2.Enabled = false;
                    DropDownList_WHCode.Enabled = false;
                    DropDownList_StorePickType.Enabled = false;
                    btnAccountNumberOld.Visible = false;
                    btnComputeNewNumber.Visible = false;
                    tbxStorePickNumber.Enabled = false;
                    tbxAccountNumberNew.Enabled = false;
                    tbxFactCheckNumberNew.Enabled = false;
                    tbxIsProfitOrLostNew.Enabled = false;

                    int ItemIDInt = int.Parse(ItemID);

                    StorePickAccount existModel = (from r in context.StorePickAccount where r.StockPickAccountID == ItemIDInt select r).FirstOrDefault();
                    if (existModel != null)
                    {

                        string storePickType = existModel.StorePickType;
                        DropDownList_StorePickType.SelectedIndex = DropDownList_StorePickType.Items.IndexOf(DropDownList_StorePickType.Items.FindByValue(storePickType));
                        tbxStorePickNumber.Text = existModel.StorePickNumber.ToString();
                        DropDownList_WHCode.SelectedIndex = DropDownList_WHCode.Items.IndexOf(DropDownList_WHCode.Items.FindByValue(existModel.WHCode));
                        tbxCargoCode.Text = existModel.CargoCode;
                        tbxCargoName.Text = existModel.CargoName;

                        tbxStorePickNumber.Text = existModel.StorePickNumber.ToString();
                        tbxAccountNumberOld.Text = existModel.AccountNumber.ToString();
                        tbxFactCheckNumberOld.Text = existModel.FactCheckNumber.ToString();
                        tbxIsProfitOrLostOld.Text = existModel.IsProfitOrLoss.ToString();
                        //编辑时数据一致
                        tbxAccountNumberNew.Text = existModel.AccountNumber.ToString();
                        tbxFactCheckNumberNew.Text = existModel.FactCheckNumber.ToString();
                        tbxIsProfitOrLostNew.Text = existModel.IsProfitOrLoss.ToString();
                        
                        DropDownList_PackageName.SelectedIndex = DropDownList_PackageName.Items.IndexOf(DropDownList_PackageName.Items.FindByText(existModel.PackageName));
                        tbxPackageCount.Text = existModel.PackageCount.Value.ToString();
                        tbxStandardCountEachBag.Text = existModel.StandardCountEachBag.Value.ToString();
                        tbxPackageNoStart.Text = existModel.PackageNoStart;
                        tbxPackageNoEnd.Text = existModel.PackageNoEnd;
                        tbxStoreDescription.Text = existModel.StoreDescription;
                        tbxRemark.Text = existModel.Remark;
                        tbxPackageLockNo.Text = existModel.PackageLockNo;
                        tbxPackageShareNo.Text = existModel.PackageShareNo;
                        tbxRecordDetail.Text = existModel.RecordDetail;
                        lblRecordTime.Text = existModel.RecordTime.ToString();
                        lblRecordMonth.Text = existModel.RecordMonth;


                    }
                }
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out StorePickAccount newModel, out string msg)
        {
            msg = "";
            newModel = null;
            try
            {
                newModel = new StorePickAccount();
                newModel.RecordTime = DateTime.Now;
                newModel.RecordMonth = DateTime.Now.ToString("yyyy-MM");

                if (DropDownList_WHCode.SelectedIndex == 0 || DropDownList_StorePickType.SelectedIndex == 0)
                {
                    msg = "仓库和存提类型不能为空，请选择！";
                    return false;
                }
                newModel.WHCode = DropDownList_WHCode.SelectedItem.Value;
                newModel.StorePickType = DropDownList_StorePickType.SelectedItem.Value;

                string cargoCode = tbxCargoCode.Text.Trim();
                using (DAL.GoldEntities context = new GoldEntities())
                {
                    Cargos cargoExist = context.Cargos.Where(r => r.CargoCode == cargoCode).FirstOrDefault<Cargos>();
                    if (cargoExist == null)
                    {
                        msg = "商品编码错误，请重新选择商品编码";
                        return false;
                    }
                    else
                    {
                        newModel.CargoCode = cargoExist.CargoCode;
                        newModel.CargoName = cargoExist.CargoName;
                        newModel.CargoModel = cargoExist.CargoModel;
                        newModel.CargoSpec = cargoExist.CargoSpec;
                        newModel.CargoUnits = cargoExist.CargoUnits;
                        newModel.ReleaseYear = cargoExist.ProduceYear;
                    }
                }

                double AccountNumber = 0, FactCheckNumber = 0,StorePickNumber=0;
                int IsProfitOrLoss = 0, PackageCount = 0, StandardCountEachBag = 0;

                double.TryParse(tbxStorePickNumber.Text.Trim(), out StorePickNumber);
                double.TryParse(tbxAccountNumberNew.Text.Trim(), out AccountNumber);
                double.TryParse(tbxFactCheckNumberNew.Text.Trim(), out FactCheckNumber);
                int.TryParse(tbxIsProfitOrLostNew.Text.Trim(), out IsProfitOrLoss);
                int.TryParse(tbxPackageCount.Text.Trim(), out PackageCount);
                int.TryParse(tbxStandardCountEachBag.Text.Trim(), out StandardCountEachBag);

                newModel.StorePickNumber = StorePickNumber;
                newModel.AccountNumber = AccountNumber;
                newModel.FactCheckNumber = FactCheckNumber;
                newModel.IsProfitOrLoss = IsProfitOrLoss;

                if (DropDownList_PackageName.SelectedIndex != 0)
                {
                    newModel.PackageId = int.Parse(DropDownList_PackageName.SelectedItem.Value.ToString());
                    newModel.PackageName = DropDownList_PackageName.SelectedItem.Text;
                }
                else
                {
                    newModel.PackageId = 0;
                    newModel.PackageName = "";
                }

                newModel.PackageCount = PackageCount;
                newModel.StandardCountEachBag = StandardCountEachBag;
                newModel.PackageNoStart = tbxPackageNoStart.Text;
                newModel.PackageNoEnd = tbxPackageNoEnd.Text;
                newModel.StoreDescription = tbxStoreDescription.Text;
                newModel.Remark = tbxRemark.Text;
                newModel.PackageLockNo = tbxPackageLockNo.Text;
                newModel.PackageShareNo = tbxPackageShareNo.Text;
                newModel.RecordDetail = tbxRecordDetail.Text;
                newModel.BadRate = tbxBadRate.Text;


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
        private bool GetUpdateModel(ref StorePickAccount updateModel, out string msg)
        {
            msg = "";

            try
            {
                //updateModel.RecordTime = DateTime.Now;
                //updateModel.RecordMonth = DateTime.Now.ToString("yyyy-MM");
                updateModel.Remark += DateTime.Now.ToString("yyyy-MM-dd") + "编辑1次 ";

                if (DropDownList_WHCode.SelectedIndex == 0 || DropDownList_StorePickType.SelectedIndex == 0)
                {
                    msg = "仓库和存提类型不能为空，请选择！";
                    return false;
                }
                updateModel.WHCode = DropDownList_WHCode.SelectedItem.Value;
                updateModel.StorePickType = DropDownList_StorePickType.SelectedItem.Value;

                string cargoCode = tbxCargoCode.Text.Trim();
                using (DAL.GoldEntities context = new GoldEntities())
                {
                    Cargos cargoExist = context.Cargos.Where(r => r.CargoCode == cargoCode).FirstOrDefault<Cargos>();
                    if (cargoExist == null)
                    {
                        msg = "商品编码错误，请重新选择商品编码";
                        return false;
                    }
                    else
                    {
                        updateModel.CargoCode = cargoExist.CargoCode;
                        updateModel.CargoName = cargoExist.CargoName;
                        updateModel.CargoModel = cargoExist.CargoModel;
                        updateModel.CargoSpec = cargoExist.CargoSpec;
                        updateModel.CargoUnits = cargoExist.CargoUnits;
                        updateModel.ReleaseYear = cargoExist.ProduceYear;
                    }
                }

                double AccountNumber = 0, FactCheckNumber = 0, StorePickNumber = 0;
                int IsProfitOrLoss = 0, PackageCount = 0, StandardCountEachBag = 0;

                double.TryParse(tbxStorePickNumber.Text.Trim(), out StorePickNumber);
                double.TryParse(tbxAccountNumberNew.Text.Trim(), out AccountNumber);
                double.TryParse(tbxFactCheckNumberNew.Text.Trim(), out FactCheckNumber);
                int.TryParse(tbxIsProfitOrLostNew.Text.Trim(), out IsProfitOrLoss);
                int.TryParse(tbxPackageCount.Text.Trim(), out PackageCount);
                int.TryParse(tbxStandardCountEachBag.Text.Trim(), out StandardCountEachBag);

                updateModel.StorePickNumber = StorePickNumber;
                updateModel.AccountNumber = AccountNumber;
                updateModel.FactCheckNumber = FactCheckNumber;
                updateModel.IsProfitOrLoss = IsProfitOrLoss;

                if (DropDownList_PackageName.SelectedIndex != 0)
                {
                    updateModel.PackageId = int.Parse(DropDownList_PackageName.SelectedItem.Value.ToString());
                    updateModel.PackageName = DropDownList_PackageName.SelectedItem.Text;
                }
                else
                {
                    updateModel.PackageId = 0;
                    updateModel.PackageName = "";
                }

                updateModel.PackageCount = PackageCount;
                updateModel.StandardCountEachBag = StandardCountEachBag;
                updateModel.PackageNoStart = tbxPackageNoStart.Text;
                updateModel.PackageNoEnd = tbxPackageNoEnd.Text;
                updateModel.StoreDescription = tbxStoreDescription.Text;
                updateModel.Remark = tbxRemark.Text;
                updateModel.PackageLockNo = tbxPackageLockNo.Text;
                updateModel.PackageShareNo = tbxPackageShareNo.Text;
                updateModel.RecordDetail = tbxRecordDetail.Text;
                updateModel.BadRate = tbxBadRate.Text;

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
                    DAL.StorePickAccount newModel = new StorePickAccount();
                    bool getNewOK = GetNewModel(out newModel, out outMsg);
                    if (getNewOK == false)
                    {
                        lblSaveMsg.Text = "新增失败！[" + outMsg + "]";
                        return;
                    }
                    using (GoldEntities context = new GoldEntities())
                    {
                        context.StorePickAccount.AddObject(newModel);
                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功";
                        btnSave.Enabled = false;
                        btnSave.CssClass = "ButtonImageStyleEnableFalse";
                    }
                }
                else//更新Update 
                {
                    using (GoldEntities context = new GoldEntities())
                    {
                        int EditID = int.Parse(ViewState["EditID"].ToString());
                        DAL.StorePickAccount updateModel = (from r in context.StorePickAccount where r.StockPickAccountID == EditID select r).FirstOrDefault();
                        bool getUpdateOK = GetUpdateModel(ref updateModel, out outMsg);
                        if (getUpdateOK == false)
                        {
                            lblSaveMsg.Text = "更新失败！[" + outMsg + "]";
                            return;
                        }
                        //context.WareHouse.Attach(updateModel);
                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功";
                        btnSave.Enabled = false;
                        btnSave.CssClass = "ButtonImageStyleEnableFalse";
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
            //tbxCargoCode.Text = "";
            //tbxCargoName.Text = "";
            //tbxUnits.Text = "";
            //tbxYear.Text = "";
            //tbxPrice1.Text = "";
            //tbxPrice2.Text = "";
            //tbxPrice3.Text = "";
            //tbxPrice4.Text = "";
            //tbxPrice5.Text = "";
            //tbxComment.Text = "";

            lblSaveMsg.Text = "";
        }

        #region 弹出框商品选择

        protected void SelectCargo_Click(object sender, EventArgs e)
        {
            string cargoCode = tbxCargoCode.Text.Trim();
            string cargoName = tbxCargoName.Text.Trim();
            string[] cargoCondition = new string[] { cargoCode, cargoName };
            GoodsSelect1.CargoQueryCondition = cargoCondition;
            GoodsSelect1.DataBindForQuery();
            this.popWindow.Show();
        }

        protected void btnClosePop_Click(object sender, EventArgs e)
        {
            this.popWindow.Hide();
            return;
        }

        //商品选择返回值
        protected void GoodsSelect1_GetCargoSelect(object sender, EventArgs e)
        {
            if (GoodsSelect1.ListSelectedCargo.Count > 1)
            {
                //ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('只能选择一个商品，请重新选择！');</script>");
                //Response.Write("<script type='text/javascript'>alert('只能选择一个商品');</script>");
                //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dddd", "alert('只能选择一个商品，请重新选择！');", true);

                //以上方式只能用在普通页面，对于Ajax页面使用以下函数
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "alertfun", "alert('只能选择一个商品，请重新选择');", true);

                this.popWindow.Show();
                return;
            }

            this.popWindow.Hide();
            for (int i = 0; i < GoodsSelect1.ListSelectedCargo.Count; i++)
            {
                string[] goodSelect = GoodsSelect1.ListSelectedCargo[i];
                if (i == 0)
                {
                    string cargoCode = goodSelect[0];

                    tbxCargoCode.Text = goodSelect[0];//商品编码
                    tbxCargoName.Text = goodSelect[1];//商品名称

                    break;
                }
                //else
                //{
                //    //根据选择的商品个数，新增行项目
                //    StockIn si;
                //    if (ViewState["StockIn"] != null)
                //        si = (StockIn)ViewState["StockIn"];
                //    else
                //        si = new StockIn();

                //    UpdateStockIn(ref si);

                //    StockDetail sd = new StockDetail();
                //    sd.CargoCode = goodSelect[0];//商品编码
                //    sd.CargoName = goodSelect[1];//商品编码
                //    sd.CargoModel = goodSelect[2];//型 号
                //    sd.CargoSpec = goodSelect[3];//规格
                //    sd.CargoUnits = goodSelect[4];//单位
                //    //sd.CargoStatus = Convert.ToInt32(goodSelect[5]);//状态
                //    sd.ReleaseYear = goodSelect[5];//发行年份
                //    si.StockDetail.Add(sd);

                //    ViewState["StockIn"] = si;
                //    GridView1.DataBind();
                //}
            }
        }

        protected void GoodsSelect1_PostBack(object sender, EventArgs e)
        {
            if (!GoodsSelect1.ShowPop)
            {
                this.popWindow.Show();
                return;
            }
        }
        #endregion

        /// <summary>
        /// 根据仓库和商品获取原账面数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAccountNumberOld_Click(object sender, EventArgs e)
        {
            if (DropDownList_WHCode.SelectedIndex == 0 || tbxCargoCode.Text == "")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "btnAccountNumberOld_Click_WH", "alert('请先选择仓库和商品！');", true);
                return;
            }
            if (tbxCargoCode.Text.Trim() != "")
            {
                using (GoldEntities context = new GoldEntities())
                {
                    Cargos existModel = (from r in context.Cargos where r.CargoCode == tbxCargoCode.Text.Trim() select r).FirstOrDefault();
                    if (existModel == null)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "btnAccountNumberOld_Click_Cargos", "alert('商品编码[" + tbxCargoCode.Text.Trim() + "]在系统中不存在，请先选择正确的商品！');", true);
                        return;
                    }
                }
            }
            string whCode = DropDownList_WHCode.SelectedItem.Value.ToString();
            string cargoCode = tbxCargoCode.Text.Trim();


            using (GoldEntities context = new GoldEntities())
            {
                //获取之前的余额
                StorePickAccount lastExistModel = null;
                if (ViewState["EditType"].ToString() == "1")//新增
                {
                    lastExistModel = (from r in context.StorePickAccount
                                      orderby r.StockPickAccountID descending
                                      where r.WHCode == whCode && r.CargoCode == cargoCode
                                      select r).FirstOrDefault();
                }
                else //编辑
                {
                    int storePickAccountID = int.Parse(ViewState["EditID"].ToString());
                    lastExistModel = (from r in context.StorePickAccount
                                      orderby r.StockPickAccountID descending
                                      where r.WHCode == whCode && r.CargoCode == cargoCode && r.StockPickAccountID != storePickAccountID
                                      select r).FirstOrDefault();
                }

                if (lastExistModel == null)
                {
                    tbxAccountNumberOld.Text = "0";
                    tbxFactCheckNumberOld.Text = "0";
                    tbxIsProfitOrLostOld.Text = "0";
                }
                else
                {
                    tbxAccountNumberOld.Text = lastExistModel.AccountNumber.ToString();
                    tbxFactCheckNumberOld.Text = lastExistModel.FactCheckNumber.ToString();
                    tbxIsProfitOrLostOld.Text = lastExistModel.IsProfitOrLoss.ToString();
                }
            }

        }

        /// <summary>
        /// 根据原账面数量和本次存提数量计算新的账面数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnComputeNewNumber_Click(object sender, EventArgs e)
        {
            if (tbxAccountNumberOld.Text.Trim() == "" || DropDownList_StorePickType.SelectedIndex == 0 || tbxStorePickNumber.Text.Trim() == "")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "btnComputeNewNumber_Click1", @"alert('请检查一下内容是否完成：\n点击“获取原账面数”按钮获取原账面数量 \n选择存提类型 \n填写存提数量！');", true);
                return;
            }
            double StorePickNumber = 0;
            if (double.TryParse(tbxStorePickNumber.Text, out StorePickNumber) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "btnComputeNewNumber_Click2", "alert('本次存提数量只能为数字！');", true);
                return;
            }

            double AccountNumberOld = 0, FactCheckNumberOld = 0;
            int IsProfitOrLossOld = 0;
            double AccountNumberNew = 0, FactCheckNumberNew = 0;
            int IsProfitOrLossNew = 0;

            double.TryParse(tbxAccountNumberOld.Text.Trim(), out AccountNumberOld);
            double.TryParse(tbxFactCheckNumberOld.Text.Trim(), out FactCheckNumberOld);
            int.TryParse(tbxIsProfitOrLostOld.Text.Trim(), out IsProfitOrLossOld);

            if (DropDownList_StorePickType.SelectedItem.Value == ((int)EnumData.PickOrStore.Pick).ToString())
            {
                AccountNumberNew = AccountNumberOld - StorePickNumber;
                FactCheckNumberNew = FactCheckNumberOld - StorePickNumber;
                IsProfitOrLossNew = (int)(FactCheckNumberNew - AccountNumberNew);
            }
            else if (DropDownList_StorePickType.SelectedItem.Value == ((int)EnumData.PickOrStore.Store).ToString())
            {
                AccountNumberNew = AccountNumberOld + StorePickNumber;
                FactCheckNumberNew = FactCheckNumberOld + StorePickNumber;
                IsProfitOrLossNew = (int)(FactCheckNumberNew - AccountNumberNew);
            }

            tbxAccountNumberNew.Text = AccountNumberNew.ToString();
            tbxFactCheckNumberNew.Text = FactCheckNumberNew.ToString();
            tbxIsProfitOrLostNew.Text = IsProfitOrLossNew.ToString();
        }



    }
}