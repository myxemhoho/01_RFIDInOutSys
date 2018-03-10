<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StockCountingPlanEdit.aspx.cs" Inherits="Gold.Query.StockCountingPlanEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">

    //JS实现treeview中选中父节点，子节点也选中,如果子节点全部选中，自动选中父节点
    //仅支持TreeView
    //调用方法 TreeView.Attributes.Add("onclick", "OnTreeNodeChecked()"); 或者 OnClick="OnTreeNodeChecked();"

    function OnTreeNodeChecked() {
        var ele = event.srcElement;
        if (ele.type == 'checkbox') {
            var childrenDivID = ele.id.replace('CheckBox', 'Nodes');
            var div = document.getElementById(childrenDivID);
            if (div != null) {
                var checkBoxs = div.getElementsByTagName('INPUT');
                for (var i = 0; i < checkBoxs.length; i++) {
                    if (checkBoxs[i].type == 'checkbox')
                        checkBoxs[i].checked = ele.checked;
                }
            }
            OnTreeNodeChildChecked(ele);

        }
    }

    function OnTreeNodeChildChecked(ele) {
        //递归处理
        if (ele == null) {
            ele = event.srcElement;
        }
        var parentDiv = ele.parentElement; //.parentElement.parentElement.parentElement.parentElement;

        var parentChkBox = document.getElementById(parentDiv.id.replace('Nodes', 'CheckBox'));
        if (parentChkBox != null) {
            var ChildsChkAll = true;
            var Boxs = parentDiv.getElementsByTagName('INPUT');
            for (var i = 0; i < Boxs.length; i++) {
                if (Boxs[i].type == 'checkbox' && Boxs[i].checked == false) {
                    ChildsChkAll = false;
                }
            }
            parentChkBox.checked = ChildsChkAll;
            OnTreeNodeChildChecked(parentChkBox);
        }
    }
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <asp:Label ID="lblTitle" runat="server" Text="盘点计划单编辑"></asp:Label>
        </div>
        <div class="commonQuery">
            <table>
                <tr>
                    <td>
                        <label class="commonLabel">
                            盘点计划单号:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxSCPCode" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="150px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            盘点计划单类型:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_SCPType" runat="server" Width="110px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <label class="commonLabel">
                            盘点层位：</label>
                    </td>
                    <td colspan="3">
                        <asp:TreeView ID="TreeView_Scope" ShowCheckBoxes="All" runat="server"                             
                            ontreenodecheckchanged="TreeView_Scope_TreeNodeCheckChanged">
                        </asp:TreeView>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：</label>
                    </td>
                    <td >
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
                OnClick="btnClear_Click" Visible="False" />
            <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" Text="返回"
                CausesValidation="false" PostBackUrl="~/Query/StockCountingPlanList.aspx" />
        </div>
    </div>
</asp:Content>
