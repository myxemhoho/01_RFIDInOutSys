<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StorePicAccountEdit.aspx.cs" Inherits="Gold.Query.StorePicAccountEdit" %>

<%@ Register TagPrefix="uc" TagName="GoodsSelect" Src="~/Controls/GoodsSelect.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript" type="text/javascript">
    function selectAll(obj) {
        var theTable = obj.parentElement.parentElement.parentElement;
        var i;
        var j = obj.parentElement.cellIndex;

        for (i = 0; i < theTable.rows.length; i++) {
            var objCheckBox = theTable.rows[i].cells[j].firstChild;
            if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
        }
    }

    function IsNumber(input) {

        var feiFu = /^\d+(\.\d+)?$/;
        var feiZheng = /^((-\d+(\.\d+)?)|(0+(\.0+)?))$/;

        //非负浮点数（正浮点数 + 0）：^\d+(\.\d+)?$
        //非正浮点数（负浮点数 + 0） ^((-\d+(\.\d+)?)|(0+(\.0+)?))$

        if (!feiFu.test(input)&&!feiZheng.test(input))//即不是正浮点数也不是负浮点数
            return false;
        else
            return true;
    }

    //检查输入是否为数字
    function NumberConfirm() {

        var errorMsg = "";
        var MainContent = "MainContent_";

        var tbxStorePickNumber = document.getElementById(MainContent + "tbxStorePickNumber");
        var tbxStorePickNumberValue = tbxStorePickNumber == undefined ? "" : tbxStorePickNumber.value;
        if (tbxStorePickNumberValue==""|| !IsNumber(tbxStorePickNumberValue))
            errorMsg += "本次存提数量不能为空且只能为数字！\n";

        var tbxAccountNumberNew = document.getElementById(MainContent + "tbxAccountNumberNew");
        var tbxAccountNumberNewValue = tbxAccountNumberNew == undefined ? "" : tbxAccountNumberNew.value;
        if (tbxAccountNumberNewValue==""|| !IsNumber(tbxAccountNumberNewValue))
            errorMsg += "本次存提后账面数量不能为空且只能为数字！\n";

        var tbxIsProfitOrLostNew = document.getElementById(MainContent + "tbxIsProfitOrLostNew");
        var tbxIsProfitOrLostNewValue = tbxIsProfitOrLostNew == undefined ? "" : tbxIsProfitOrLostNew.value;
        if (tbxIsProfitOrLostNewValue==""|| !IsNumber(tbxIsProfitOrLostNewValue))
            errorMsg += "本次存提后盈亏不能为空且只能为数字！\n";

        var tbxRecordDetail = document.getElementById(MainContent + "tbxRecordDetail");
        var tbxRecordDetailValue = tbxRecordDetail == undefined ? "" : tbxRecordDetail.value;
        if (tbxRecordDetailValue=="")
            errorMsg += "收发情况不能为空！\n";

        var tbxPackageCount = document.getElementById(MainContent + "tbxPackageCount");
        var tbxPackageCountValue = tbxPackageCount == undefined ? "" : tbxPackageCount.value;
        if (tbxPackageCountValue!=""&&!IsNumber(tbxPackageCountValue))
            errorMsg += "件数只能为数字！\n";

             var tbxStandardCountEachBag = document.getElementById(MainContent + "tbxStandardCountEachBag");
        var tbxStandardCountEachBagValue = tbxStandardCountEachBag == undefined ? "" : tbxStandardCountEachBag.value;
        if (tbxStandardCountEachBagValue!=""&&!IsNumber(tbxStandardCountEachBagValue))
            errorMsg += "“枚/箱”只能为数字！\n";

        if (errorMsg != "") {            
            alert(errorMsg);
            return false;
        }
        else {
            return true;
        }
    }



    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <asp:Label ID="lblTitle" runat="server" Text="存提记录编辑"></asp:Label>
        </div>
        <div class="commonQuery">
            <table>
                <tr>
                    <td>
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="120px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品编码：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server" Width="150px" MaxLength="50"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:Button runat="server" ID="btnSelectCargo1" Text="..." OnClick="SelectCargo_Click" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxCargoCode" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxCargoCode" ErrorMessage="商品编号不能为空！" Display="Dynamic"
                            ValidationGroup="WHNewGroup"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品名称：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoName" runat="server" Width="200px" MaxLength="100"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:Button runat="server" ID="btnSelectCargo2" Text="..." OnClick="SelectCargo_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            原账面数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxAccountNumberOld" runat="server" Width="100px" MaxLength="100"
                            Enabled="false"></asp:TextBox>
                        <asp:Button ID="btnAccountNumberOld" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                            Text="获取原账面数" OnClick="btnAccountNumberOld_Click" />
                    </td>
                    <td>
                        <label class="commonLabel">
                            原盘点实物数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxFactCheckNumberOld" runat="server" Width="100px" MaxLength="100"
                            Enabled="false"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            原盈亏数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxIsProfitOrLostOld" runat="server" Width="100px" MaxLength="100"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            存/提:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_StorePickType" runat="server" Width="120px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            本次存提数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxStorePickNumber" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:Button ID="btnComputeNewNumber" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                            Text="计算存提后余数" OnClick="btnComputeNewNumber_Click" />
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            本次存提后账面数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxAccountNumberNew" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            本次存提后盘点实物数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxFactCheckNumberNew" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            本次存提后盈亏数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxIsProfitOrLostNew" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            收发情况：</label>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbxRecordDetail" runat="server" Width="100%" Height="80px" TextMode="MultiLine"
                            MaxLength="100"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            包装:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_PackageName" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            件数：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageCount" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            (仅限数字)</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            枚/箱：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxStandardCountEachBag" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            (仅限数字)</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            箱号(起)：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageNoStart" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            箱号(至)：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageNoEnd" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            共用箱号：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageShareNo" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            箱锁号：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageLockNo" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            不良品率：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxBadRate" runat="server" Width="100px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            存放明细：</label>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbxStoreDescription" runat="server" Width="100%" MaxLength="100"
                            TextMode="MultiLine" Height="60px"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：</label>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbxRemark" runat="server" Width="100%" MaxLength="100" TextMode="MultiLine"
                            Height="60px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            存提登记时间：</label>
                    </td>
                    <td>
                        <asp:Label ID="lblRecordTime" runat="server" Width="100px" MaxLength="100" ></asp:Label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            收发月份：</label>
                    </td>
                    <td>
                        <asp:Label ID="lblRecordMonth" runat="server" Width="100px" MaxLength="100" ></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="lblSaveMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnSave" runat="server" CausesValidation="true" ValidationGroup="WHNewGroup"
                CssClass="ButtonImageStyle" Text="保存" OnClick="btnSave_Click" OnClientClick="return NumberConfirm();" />
            <%--<asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" Text="清空" CausesValidation="false"
                OnClick="btnClear_Click" />--%>
            <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" Text="返回"
                CausesValidation="false" PostBackUrl="~/Query/StorePicAccountList.aspx" />
        </div>
        <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupCargo">
            <div class="modalPopupWrapperCargo">
                <div id="pnlDragTitle" class="modalHeader">
                    <span>选择商品</span>
                    <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="关闭" OnClick="btnClosePop_Click" />
                </div>
                <div class="modalBody">
                    <uc:GoodsSelect ID="GoodsSelect1" runat="server" OnGetCargoSelect="GoodsSelect1_GetCargoSelect" />
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton ID="LinkButton1" runat="server" Width="0" Height="0"></asp:LinkButton>
        <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="LinkButton1"
            PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
            PopupDragHandleControlID="pnlDragTitle">
        </ajaxToolkit:ModalPopupExtender>
    </div>
</asp:Content>
