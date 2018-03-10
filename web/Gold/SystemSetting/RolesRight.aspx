<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RolesRight.aspx.cs" Inherits="Gold.SystemSetting.RolesRight" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        /*GridView被选中行的样式*/
        .GridViewSelectedRowStyle
        {
            background-color: #99CCFF; /*SteelBlue;*/ /*font-weight: bold;*/
            font-family: 微软雅黑;
            color: Black;
            font-weight: bold;
        }
    </style>
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
    <div class="leftQuery">
        <fieldset>
            <legend>角色列表</legend>
            <div style="padding-left: 15px;">
                <asp:GridView runat="server" CellPadding="0" ID="RolesGrid" GridLines="None" CellSpacing="2"
                    AutoGenerateColumns="false" ShowHeader="false" BorderStyle="None" OnRowDeleting="RolesGrid_RowDeleting"
                    OnRowCommand="RolesGrid_RowCommand">
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <Columns>
                        <asp:TemplateField HeaderText="Roles">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnRoleName" runat="server" CommandName="Select" CommandArgument='<%# Container.DataItem.ToString() %>'
                                    Text='<%# Container.DataItem.ToString() %>' />
                                <label>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnDel" runat="server" CommandName="Delete" CommandArgument='<%# Container.DataItem.ToString() %>'
                                    Text="删除" OnClientClick="javascript:return confirm('确定要删除吗？');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <br />
            <asp:Label ID="lblDelMsg" ForeColor="red" runat="server" Text="" />
            <hr />
            <label>
            添加角色</label>
            <asp:TextBox ID="txtRoleName" runat="server" Width="120px" MaxLength="32" />
            <asp:Button Text="创 建" ID="btnCreate" runat="server" OnClick="btnCreate_OnClick"
                CssClass="ButtonImageStyle" />
            <br />
            <asp:Label ID="lblMessage" ForeColor="red" runat="server" Text="" />
            <br />
        </fieldset>
    </div>
    <div class="rightList" >
        <fieldset >
            <legend>[<asp:Label ID="lblRoleName" ForeColor="red" runat="server" Text="" />
                ]角色拥有的权限</legend>
            <asp:TreeView ID="TreeView1" runat="server" ShowCheckBoxes="All">
            </asp:TreeView>
            <br />
            <asp:Button Text="保 存" ID="btnSave" runat="server" OnClick="btnSave_OnClick" CssClass="ButtonImageStyle" />
            <br />
            <asp:Label ID="lblSaveMsg" ForeColor="red" runat="server" Text="" />
        </fieldset>
    </div>
</asp:Content>
