<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CargoEdit.aspx.cs" Inherits="Gold.BaseInfoSetting.CargoEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <asp:Label ID="lblTitle" runat="server" Text="仓库信息编辑"></asp:Label>
        </div>
        <div class="commonQuery">
            <table>
                <tr>
                    <td>
                        <label class="commonLabel">
                            商品编码：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxWHName" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxCargoName" ErrorMessage="商品名称不能为空！" Display="Dynamic"
                            ValidationGroup="WHNewGroup"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            商品型号：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="200px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品规格：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="200px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            计量单位：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxUnits" runat="server" Width="200px" MaxLength="100"></asp:TextBox><label
                            class="commonSaveMsgLabel">
                            *</label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxUnits" ErrorMessage="计量单位不能为空！" Display="Dynamic"
                            ValidationGroup="WHNewGroup"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <label class="commonLabel">
                            发行年份：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxYear" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            可售状态：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_SaleStatus" runat="server" Width="200px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            批发价：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPrice1" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            会员价（金卡）：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPrice2" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                    <td>                        
                        <label class="commonLabel">
                            会员价（银卡）：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPrice3" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>                        
                        <label class="commonLabel">
                            会员价（白金卡）：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPrice4" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                    <td>                        
                        <label class="commonLabel">
                            参考售价：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPrice5" runat="server" Width="200px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：</label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbxComment" runat="server" Width="638px" MaxLength="100" 
                            Height="16px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="lblSaveMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnSave" runat="server" CausesValidation="true" ValidationGroup="WHNewGroup"
                CssClass="ButtonImageStyle" Text="保存" OnClick="btnSave_Click" />
            <asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" Text="清空" CausesValidation="false"
                OnClick="btnClear_Click" />
            <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" Text="返回"
                CausesValidation="false" PostBackUrl="~/BaseInfoSetting/CargoList.aspx" />
        </div>
    </div>
</asp:Content>
