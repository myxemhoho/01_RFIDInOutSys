<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="WareHouseEdit.aspx.cs" Inherits="Gold.BaseInfoSetting.WareHouseEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <asp:Label ID="lblTitle" runat="server" Text="仓库信息编辑"></asp:Label>
        </div>
        <div class="commonQuery">
            <label class="commonLabel">
                仓库代码：</label>
            <asp:TextBox ID="tbxWHCode" runat="server" Width="300px" MaxLength="30"></asp:TextBox>
            <label class="commonSaveMsgLabel">*</label>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxWHCode" CssClass="commonSaveMsgLabel"
                runat="server" ControlToValidate="tbxWHCode" ErrorMessage="仓库编号不能为空！" Display="Dynamic" ValidationGroup="WHNewGroup"></asp:RequiredFieldValidator>
            <br />
            <label class="commonLabel">
                仓库名称：</label>
            <asp:TextBox ID="tbxWHName" runat="server" Width="300px" MaxLength="50"></asp:TextBox>
            <label class="commonSaveMsgLabel">*</label>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxWHName" CssClass="commonSaveMsgLabel"
                runat="server" ControlToValidate="tbxWHName" ErrorMessage="仓库名称不能为空！" Display="Dynamic" ValidationGroup="WHNewGroup"></asp:RequiredFieldValidator>                
            <br />
            <label class="commonLabel">
                仓库类型：</label>
            <asp:DropDownList ID="DropDownList_WHType" runat="server">
                <asp:ListItem Value="1">实体仓库</asp:ListItem>
                <asp:ListItem Value="2">虚拟仓库</asp:ListItem>
            </asp:DropDownList>
            <label class="commonSaveMsgLabel">*</label>
            <br />
            <label class="commonLabel">
                启用状态：</label>
            <asp:DropDownList ID="DropDownList_Enabled" runat="server">
                <asp:ListItem Value="True">已启用</asp:ListItem>
                <asp:ListItem Value="False">未启用</asp:ListItem>
            </asp:DropDownList>
            <label class="commonSaveMsgLabel">*</label>
            <br />
            <label class="commonLabel">
                仓库地址：</label>
            <asp:TextBox ID="tbxAddress" runat="server" Width="500px" MaxLength="100"></asp:TextBox>
            <br />
            <label class="commonLabel">
                仓库电话：</label>
            <asp:TextBox ID="tbxPhone" runat="server" Width="500px" MaxLength="50"></asp:TextBox>
            <br />
            <label class="commonLabel">
                备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：</label>
            <asp:TextBox ID="tbxComment" runat="server" Width="500px" MaxLength="100"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblSaveMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnSave" runat="server" CausesValidation="true"  ValidationGroup="WHNewGroup" CssClass="ButtonImageStyle"
                Text="保存" onclick="btnSave_Click" />
                <asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" 
                Text="清空"  CausesValidation="false" onclick="btnClear_Click" />
                <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" 
                Text="返回"  CausesValidation="false" PostBackUrl="~/BaseInfoSetting/WareHouseList.aspx" />
            
        </div>
    </div>
</asp:Content>
