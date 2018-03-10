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
    public partial class StockCountingPlanEdit : System.Web.UI.Page
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

                TreeView_Scope.Attributes.Add("onclick", "OnTreeNodeChecked()");

                if (EditType == "1")
                {
                    lblTitle.Text = "盘点计划单新增";
                    tbxSCPCode.Text = "自动新增";
                    LoadInfo("-1");//新增
                }
                else
                {
                    lblTitle.Text = "盘点计划单编辑";
                    LoadInfo(EditID);
                }
            }
        }

        private void LoadInfo(string ItemID)
        {
            using (GoldEntities context = new GoldEntities())
            {
                //绑定盘点计划单状态
                List<NameValueModel> ListBinType = Gold.Utility.EnumData.GetEnumsList(typeof(Gold.Utility.EnumData.SCPTypeEnum));
                DropDownList_SCPType.Items.Clear();
                DropDownList_SCPType.DataTextField = "Name";
                DropDownList_SCPType.DataValueField = "Value";
                DropDownList_SCPType.DataSource = ListBinType;
                DropDownList_SCPType.DataBind();
                DropDownList_SCPType.Items.Insert(0, new ListItem("", ""));

                //绑定仓库
                var result = (from r in context.WareHouse select new { r.WHCode, r.WHName }).OrderBy(r => r.WHCode).ToList();
                DropDownList_WHCode.Items.Clear();
                DropDownList_WHCode.DataTextField = "WHName";
                DropDownList_WHCode.DataValueField = "WHCode";
                DropDownList_WHCode.DataSource = result;
                DropDownList_WHCode.DataBind();
                DropDownList_WHCode.Items.Insert(0, new ListItem("", ""));

                string defaultWHCode = WebConfigHelper.Instance.GetAppSettingValue("DefaultWHCode");
                DropDownList_WHCode.SelectedIndex = DropDownList_WHCode.Items.IndexOf(DropDownList_WHCode.Items.FindByValue(defaultWHCode));
                DropDownList_WHCode.Enabled = false;//目前只支持对地王26库的盘点，故仓库选项禁用

                //货架名称 例如A架
                var BinCodeFirstLetterList = (from r in context.StorageBin orderby r.BinCode.Substring(0, 1) select new { BinShort = r.BinCode.Substring(0, 1) }).Distinct();

                //层位名
                var BinCodeTotalCodeList = (from r in context.StorageBin orderby r.BinCode select r.BinCode);

                //层中小区域名及每个小区域数量
                var CargoTagBinCodeAndNumberList = (from r in context.CargoTag orderby r.BinCode group r by r.BinCode into x select new { BinCode = x.Key, Number = x.Sum(r => r.Number) });

                //编辑状态下进入此页面时
                if (ItemID != "-1")
                {
                    StockCountingPlan existModel = (from r in context.StockCountingPlan where r.SCPCode == ItemID select r).FirstOrDefault();

                    if (existModel != null)
                    {
                        List<StockCountingDetail> detailList = (from r in context.StockCountingDetail where r.SCPCode == existModel.SCPCode select r).ToList<StockCountingDetail>();
                        tbxSCPCode.Text = existModel.SCPCode;
                        //设置选中项
                        DropDownList_SCPType.SelectedIndex = DropDownList_SCPType.Items.IndexOf(DropDownList_SCPType.Items.FindByValue(existModel.SCPType.Value.ToString()));

                        tbxComment.Text = existModel.Comment;

                        //将数据导入到TreeView中
                        foreach (var binCodeFirstLetter in BinCodeFirstLetterList)
                        {
                            TreeNode FirstLevelNode = new TreeNode(binCodeFirstLetter.BinShort, binCodeFirstLetter.BinShort);
                            FirstLevelNode.Collapse();
                            TreeView_Scope.Nodes.Add(FirstLevelNode);

                            var TempBinCodeTotalCodeList = (from r in BinCodeTotalCodeList where r.StartsWith(binCodeFirstLetter.BinShort) select r);
                            foreach (var binCodeTotalCode in TempBinCodeTotalCodeList)
                            {
                                TreeNode SecondLevelNode = new TreeNode(binCodeTotalCode, binCodeTotalCode);
                                FirstLevelNode.ChildNodes.Add(SecondLevelNode);

                                var TempCargoTagBinCodeAndNumberList = (from r in CargoTagBinCodeAndNumberList where r.BinCode.StartsWith(binCodeTotalCode) select r);
                                foreach (var temp in TempCargoTagBinCodeAndNumberList)
                                {
                                    string ShowName = temp.BinCode + "&nbsp;&nbsp;[现存" + (temp.Number == null ? "0" : temp.Number.Value.ToString()).PadLeft(5, ' ') + "枚]";

                                    TreeNode ThirdLevelNode = new TreeNode(ShowName, temp.BinCode);
                                    SecondLevelNode.ChildNodes.Add(ThirdLevelNode);

                                    //判断是否在计划单中，若在计划单中则选中
                                    var existInThePlan = (from r in detailList where r.BinCode == temp.BinCode select r).FirstOrDefault();
                                    if (existInThePlan != null)
                                    {
                                        ThirdLevelNode.Checked = true;
                                        if (ThirdLevelNode.Parent != null)//如果节点上级不为空则展开该节点
                                        {
                                            ThirdLevelNode.Parent.Expand();
                                            if (ThirdLevelNode.Parent.Parent != null)
                                                ThirdLevelNode.Parent.Parent.Expand();
                                        }
                                    }


                                }


                            }

                        }
                    }
                }
                else
                {
                    //将数据导入到TreeView中
                    foreach (var binCodeFirstLetter in BinCodeFirstLetterList)
                    {
                        TreeNode FirstLevelNode = new TreeNode(binCodeFirstLetter.BinShort, binCodeFirstLetter.BinShort);
                        FirstLevelNode.Collapse();

                        var TempBinCodeTotalCodeList = (from r in BinCodeTotalCodeList where r.StartsWith(binCodeFirstLetter.BinShort) select r);
                        foreach (var binCodeTotalCode in TempBinCodeTotalCodeList)
                        {
                            TreeNode SecondLevelNode = new TreeNode(binCodeTotalCode, binCodeTotalCode);

                            var TempCargoTagBinCodeAndNumberList = (from r in CargoTagBinCodeAndNumberList where r.BinCode.StartsWith(binCodeTotalCode) select r);
                            foreach (var temp in TempCargoTagBinCodeAndNumberList)
                            {
                                string ShowName = temp.BinCode + "&nbsp;&nbsp;[现存" + (temp.Number == null ? "0" : temp.Number.Value.ToString()).PadLeft(5, ' ') + "枚]";

                                TreeNode ThirdLevelNode = new TreeNode(ShowName, temp.BinCode);
                                SecondLevelNode.ChildNodes.Add(ThirdLevelNode);
                            }

                            FirstLevelNode.ChildNodes.Add(SecondLevelNode);
                        }
                        TreeView_Scope.Nodes.Add(FirstLevelNode);
                    }
                }
            }
        }

        /// <summary>
        /// 获取新增界面中的数据
        /// </summary>
        /// <param name="msg">异常消息</param>
        /// <returns></returns>
        private bool GetNewModel(out StockCountingPlan newModel, out List<StockCountingDetail> detailList, out string msg)
        {
            msg = "";
            newModel = null;
            detailList = new List<StockCountingDetail>();
            try
            {
                newModel = new StockCountingPlan();

                newModel.SCPCode = DAL.KeyGenerator.Instance.GetStockCountingPlanKey();//获取自动生成序列盘点计划单号


                if (DropDownList_SCPType.SelectedItem == null || DropDownList_WHCode.SelectedItem == null
                    || DropDownList_SCPType.SelectedItem.Text.Trim() == "" || DropDownList_WHCode.SelectedItem.Text.Trim() == "")
                {
                    msg = "盘点类型和盘点仓库不能为空，请选择！";
                    return false;
                }

                newModel.SCPType = int.Parse(DropDownList_SCPType.Text == "" ? null : DropDownList_SCPType.SelectedItem.Value.Trim());
                newModel.SCPStatus = (int)EnumData.SCPStatusEnum.Initial;
                newModel.WHCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                newModel.WHName = DropDownList_WHCode.SelectedItem.Text;
                if (Session["UserInfo"] != null)
                {
                    Users LoginUser = Session["UserInfo"] as Users;
                    newModel.CreatorID = LoginUser.UserId;
                    newModel.CreatorName = LoginUser.UserName;

                }
                newModel.CreateDate = DateTime.Now;
                newModel.Comment = tbxComment.Text.Trim();

                List<string> BinCodeList = GetTreeViewCheckedNode();

                if (BinCodeList.Count == 0)
                {
                    msg = "必须选择盘点层位范围！";
                    return false;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    var result = (from r in context.CargoTag where BinCodeList.Contains(r.BinCode) && r.CargoCode != null & r.CargoCode != "" select r);
                    int rowNumber = 1;
                    foreach (var temp in result)
                    {
                        StockCountingDetail newObj = new StockCountingDetail();
                        newObj.SCPCode = newModel.SCPCode;
                        newObj.DetailRowNumber = rowNumber.ToString().PadLeft(3, '0');
                        rowNumber++;
                        newObj.CargoCode = temp.CargoCode;
                        newObj.CargoName = temp.Cargos.CargoName;
                        newObj.CargoModel = temp.Cargos.CargoModel;
                        newObj.CargoSpec = temp.Cargos.CargoSpec;
                        newObj.CargoUnits = temp.Cargos.CargoUnits;
                        newObj.BinCode = temp.BinCode;
                        newObj.NumPlan = temp.Number;//商品标签表中存储的商品数量
                        newObj.NumActual = null;
                        newObj.PeriodInNum = null;
                        newObj.PeriodOutNum = null;
                        newObj.NumDifference = null;
                        newObj.CountingEndTime = null;
                        newObj.CountingStartTime = newModel.CreateDate;//盘点开始时间即为创建时间
                        newObj.Status = (int)EnumData.SCDetailStatusEnum.Uncompleted;
                        newObj.ActorID = null;
                        newObj.ActorName = null;

                        detailList.Add(newObj);
                    }
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
        /// <param name="updateModel">传入此方法之前应先查询此对象所有值</param>
        /// <param name="detailList">当前所有选中的层位对应的明细</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GetUpdateModel(ref StockCountingPlan updateModel, out List<StockCountingDetail> detailList, out string msg)
        {
            msg = "";
            detailList = new List<StockCountingDetail>();
            try
            {
                //updateModel.SCPCode = DAL.KeyGenerator.Instance.GetStockCountingPlanKey();//获取自动生成序列盘点计划单号
                updateModel.SCPCode = tbxSCPCode.Text.Trim();

                if (DropDownList_SCPType.SelectedItem == null || DropDownList_WHCode.SelectedItem == null
                    || DropDownList_SCPType.SelectedItem.Text.Trim() == "" || DropDownList_WHCode.SelectedItem.Text.Trim() == "")
                {
                    msg = "盘点类型和盘点仓库不能为空，请选择！";
                    return false;
                }

                updateModel.SCPType = int.Parse(DropDownList_SCPType.Text == "" ? null : DropDownList_SCPType.SelectedItem.Value.Trim());

                updateModel.WHCode = DropDownList_WHCode.SelectedItem.Value.ToString();
                updateModel.WHName = DropDownList_WHCode.SelectedItem.Text;
                if (Session["UserInfo"] != null)
                {
                    Users LoginUser = Session["UserInfo"] as Users;
                    updateModel.CreatorID = LoginUser.UserId;
                    updateModel.CreatorName = LoginUser.UserName;

                }
                updateModel.CreateDate = DateTime.Now;
                updateModel.Comment = tbxComment.Text.Trim();

                List<string> BinCodeList = GetTreeViewCheckedNode();

                if (BinCodeList.Count == 0)
                {
                    msg = "必须选择盘点层位范围！";
                    return false;
                }

                using (GoldEntities context = new GoldEntities())
                {
                    var result = (from r in context.CargoTag where BinCodeList.Contains(r.BinCode) && r.CargoCode != null && r.CargoCode != "" select r);

                    //获取最大值
                    string scpCode = updateModel.SCPCode;
                    List<string> rowNumberList = (from r in context.StockCountingDetail where r.SCPCode == scpCode select r.DetailRowNumber).ToList<string>();
                    List<int> rowNumberIntList = rowNumberList.ConvertAll<int>(ConvertStringToInt);
                    int maxRowNumber = rowNumberIntList.Count == 0 ? 0 : rowNumberIntList.Max();

                    foreach (var temp in result)
                    {
                        maxRowNumber++;

                        StockCountingDetail newObj = new StockCountingDetail();
                        newObj.SCPCode = updateModel.SCPCode;
                        newObj.DetailRowNumber = maxRowNumber.ToString().PadLeft(3, '0'); ;//因为此处明细行是作为新增数据处理，所以设置行号
                        newObj.CargoCode = temp.CargoCode;
                        newObj.CargoName = temp.Cargos.CargoName;
                        newObj.CargoModel = temp.Cargos.CargoModel;
                        newObj.CargoSpec = temp.Cargos.CargoSpec;
                        newObj.CargoUnits = temp.Cargos.CargoUnits;
                        newObj.BinCode = temp.BinCode;
                        newObj.NumPlan = temp.Number;//商品标签表中存储的商品数量
                        newObj.NumActual = null;
                        newObj.PeriodInNum = null;
                        newObj.PeriodOutNum = null;
                        newObj.NumDifference = null;
                        newObj.CountingEndTime = null;
                        newObj.CountingStartTime = updateModel.CreateDate;//DateTime.Now;//盘点开始时间即为创建时间
                        newObj.Status = (int)EnumData.SCDetailStatusEnum.Uncompleted;
                        newObj.ActorID = null;
                        newObj.ActorName = null;

                        detailList.Add(newObj);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }

        }

        int ConvertStringToInt(string str)
        {
            int ret = -1;
            if (int.TryParse(str, out ret) == false)
                ret = -1;

            return ret;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblSaveMsg.Text = "";
                string outMsg = "";
                if (ViewState["EditType"].ToString() == "1")//新增Insert
                {
                    StockCountingPlan newModel = new StockCountingPlan();
                    List<StockCountingDetail> ListDetail = new List<StockCountingDetail>();

                    bool getNewOK = GetNewModel(out newModel, out ListDetail, out outMsg);
                    if (getNewOK == false)
                    {
                        lblSaveMsg.Text = "新增失败！[" + outMsg + "]";
                        return;
                    }
                    using (GoldEntities context = new GoldEntities())
                    {
                        //新增计划表
                        context.StockCountingPlan.AddObject(newModel);

                        //新增盘点表
                        foreach (StockCountingDetail sd in ListDetail)
                        {
                            context.StockCountingDetail.AddObject(sd);
                        }

                        int AffectRowsCount = context.SaveChanges();
                        lblSaveMsg.Text = "保存成功![影响行数" + AffectRowsCount.ToString() + "]";
                        btnSave.Enabled = false;
                        btnSave.CssClass = "ButtonImageStyleEnableFalse";
                    }
                }
                else//更新Update 
                {

                    //更新时，先删除全部明细表的行，再新增所有本次选中的层位对应的明细行
                    using (GoldEntities context = new GoldEntities())
                    {
                        string EditID = ViewState["EditID"].ToString();

                        List<StockCountingDetail> OldListDetail = new List<StockCountingDetail>();
                        List<StockCountingDetail> ListDetail = new List<StockCountingDetail>();

                        StockCountingPlan updateModel = (from r in context.StockCountingPlan where r.SCPCode == EditID select r).FirstOrDefault();
                        if (updateModel != null)
                        {
                            if (updateModel.SCPStatus.Value != (int)EnumData.SCPStatusEnum.Initial)
                            {
                                lblSaveMsg.Text = "更新失败！[该盘点计划单不是初始状态，不能进行修改]";
                                return;
                            }

                            bool getUpdateOK = GetUpdateModel(ref updateModel, out ListDetail, out outMsg);
                            if (getUpdateOK == false)
                            {
                                lblSaveMsg.Text = "更新失败！[" + outMsg + "]";
                                return;
                            }

                            OldListDetail = (from r in context.StockCountingDetail where r.SCPCode == updateModel.SCPCode select r).ToList<StockCountingDetail>();

                            //先删除旧有的明细行
                            foreach (StockCountingDetail del in OldListDetail)
                            {
                                context.StockCountingDetail.DeleteObject(del);
                            }

                            //再新增本次选中的明细行
                            foreach (StockCountingDetail add in ListDetail)
                            {
                                context.StockCountingDetail.AddObject(add);
                            }

                            int AffectRowsCount = context.SaveChanges();
                            lblSaveMsg.Text = "保存成功![影响行数" + AffectRowsCount.ToString() + "]";
                            btnSave.Enabled = false;
                            btnSave.CssClass = "ButtonImageStyleEnableFalse";
                        }
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

            //lblSaveMsg.Text = "";
        }

        protected void TreeView_Scope_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            //TreeNode senderNode = sender as TreeNode;
            //foreach (TreeNode n in senderNode.ChildNodes)
            //{
            //    n.Checked = senderNode.Checked;
            //}
        }

        /// <summary>
        /// 获取页面中选中的CheckBox
        /// </summary>
        /// <returns></returns>
        List<string> GetTreeViewCheckedNode()
        {
            List<string> retBinCode = new List<string>();
            foreach (TreeNode FirstLevelNode in TreeView_Scope.Nodes)
            {
                foreach (TreeNode SecondLevelNode in FirstLevelNode.ChildNodes)
                {
                    foreach (TreeNode ThirdLevelNode in SecondLevelNode.ChildNodes)
                    {
                        if (ThirdLevelNode.Checked)
                        {
                            retBinCode.Add(ThirdLevelNode.Value);
                        }
                    }
                }
            }
            return retBinCode;
        }
    }
}