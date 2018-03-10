<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserMgr.aspx.cs" Inherits="Gold.SystemSetting.UserMgr" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
<script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
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

        //显示等待滚动图片
        function showWaitDiv(divName) {
            document.getElementById(divName).style.display = "block";
        }
        //隐藏等待滚动图片
        function hiddenWaitDiv(divName) {
            document.getElementById(divName).style.display = "none";
        }

        function AskConfirm() {
            if (confirm('确定要将用友数据导入到RFID系统中吗？') == true) {
                showWaitDiv('divWait');
                return true;
            }
            else
                return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="leftQuery">
        <fieldset>
            <legend>组织机构</legend>
            <asp:TreeView ID="TreeView1" runat="server" ImageSet="Simple" SkipLinkText="" 
                onselectednodechanged="TreeView1_SelectedNodeChanged" >
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px"
                    NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                    VerticalPadding="0px" />
            </asp:TreeView>
        </fieldset>
    </div>

    <div class="rightList">
        <fieldset>
        <legend>用户详情</legend>
                <asp:FormView ID="FormView2" runat="server" DefaultMode="ReadOnly" DataSourceID="EntityDataSource2"
        OnItemUpdated="FormView2_ItemUpdated" OnItemCommand="FormView2_ItemCommand" OnItemInserting="FormView2_ItemInserting"
        OnItemInserted="FormView2_ItemInserted" onitemupdating="FormView2_ItemUpdating" >
        <EditItemTemplate>
            <table >
                <tr style="height: 30px">
                    <td style="width: 40px;">
                        工号：
                    </td>
                    <td style="width: 150px;">
                        <asp:TextBox ID="txtUserCode" runat="server" Text='<%# Bind("UserId") %>'  Width="130px" Enabled="false" />
                    </td>
                    <td style="width: 40px">
                        姓名：
                    </td>
                    <td style="width: 150px">
                        <asp:TextBox ID="txtUserName" runat="server" Text='<%# Bind("UserName") %>'  Width="130px"  />
                    </td>
                    <td style="width: 40px">
                        状态：
                    </td>
                    <td>
                        <asp:CheckBox ID="cboxStatus" runat="server" Checked='<%# Bind("Enabled") %>'  />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td>
                        部门：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpDepartment" runat="server" Width="135px" SelectedValue='<%# Bind("DepartmentCode") %>'
                             DataSourceID="edsDepartment" DataTextField="DepartmentName" DataValueField="DepartmentCode" />
                        <asp:EntityDataSource ID="edsDepartment" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Department"
                             Select="it.[DepartmentCode], it.[DepartmentName]">
                        </asp:EntityDataSource>
                    </td>
                    <td>
                        岗位：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpPosition" runat="server" Width="135px" SelectedValue='<%# Bind("PositionId") %>'
                             DataSourceID="edsPosition" DataTextField="PositionName" DataValueField="PositionId" />
                        <asp:EntityDataSource ID="edsPosition" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Position"
                             Select="it.[PositionId], it.[PositionName]">
                        </asp:EntityDataSource>
                    </td>
                    <td style="width: 40px">
                        角色：
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoles" runat="server" Text='' Width="200px" ReadOnly="true" OnLoad="txtRoles_Load" />
                        <ajaxToolkit:PopupControlExtender ID="PopupControlExtender2" runat="server"
                            TargetControlID="txtRoles" PopupControlID="pnlRoles" Position="Bottom" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td colspan="6">
                        <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                            Text="更新" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="取消" />
                    </td>
                </tr>
            </table>
            <table>

                <tr style="height: 30px">
                    <td colspan="4">

                    </td>
                </tr>
            </table>
        </EditItemTemplate>
        <InsertItemTemplate>
            <table >
                <tr style="height: 30px">
                    <td style="width: 40px;">
                        工号：
                    </td>
                    <td style="width: 150px;">
                        <asp:TextBox ID="txtUserCode" runat="server" Text='<%# Bind("UserId") %>'  Width="130px" />
                    </td>
                    <td style="width: 40px">
                        姓名：
                    </td>
                    <td style="width: 150px">
                        <asp:TextBox ID="txtUserName" runat="server" Text='<%# Bind("UserName") %>'  Width="130px" />
                    </td>
                    <td style="width: 40px">
                        状态：
                    </td>
                    <td>
                        <asp:CheckBox ID="cboxStatus" runat="server" Checked='<%# Bind("Enabled") %>' />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td>
                        部门：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpDepartment" runat="server" Width="135px" SelectedValue='<%# Bind("DepartmentCode") %>'
                             DataSourceID="edsDepartment" DataTextField="DepartmentName" DataValueField="DepartmentCode"
                             OnInit="drpDepartment_Init"  />
                        <asp:EntityDataSource ID="edsDepartment" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Department"
                             Select="it.[DepartmentCode], it.[DepartmentName]">
                        </asp:EntityDataSource>
                    </td>
                    <td>
                        岗位：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpPosition" runat="server" Width="135px" SelectedValue='<%# Bind("PositionId") %>'
                             DataSourceID="edsPosition" DataTextField="PositionName" DataValueField="PositionId" />
                        <asp:EntityDataSource ID="edsPosition" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Position"
                             Select="it.[PositionId], it.[PositionName]">
                        </asp:EntityDataSource>
                    </td>
                    <td style="width: 40px">
                        角色：
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoles" runat="server" Text='' Width="200px" ReadOnly="true" OnLoad="txtRoles_Load" />
                        <ajaxToolkit:PopupControlExtender ID="PopupControlExtender2" runat="server"
                            TargetControlID="txtRoles" PopupControlID="pnlRoles" Position="Bottom" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td colspan="6">
                        <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                            Text="插入" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="取消" />
                    </td>
                </tr>
            </table>
        </InsertItemTemplate>
        <ItemTemplate>
            <table >
                <tr style="height: 30px">
                    <td style="width: 40px;">
                        工号：
                    </td>
                    <td style="width: 150px;">
                        <asp:TextBox ID="txtUserCode" runat="server" Text='<%# Bind("UserId") %>'  Width="130px" Enabled="false" />
                    </td>
                    <td style="width: 40px">
                        姓名：
                    </td>
                    <td style="width: 150px">
                        <asp:TextBox ID="txtUserName" runat="server" Text='<%# Bind("UserName") %>'  Width="130px"  Enabled="false" />
                    </td>
                    <td style="width: 40px">
                        状态：
                    </td>
                    <td>
                        <asp:CheckBox ID="cboxStatus" runat="server" Checked='<%# Bind("Enabled") %>'  Enabled="false" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td>
                        部门：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpDepartment" runat="server" Width="135px" SelectedValue='<%# Bind("DepartmentCode") %>'
                             DataSourceID="edsDepartment" DataTextField="DepartmentName" DataValueField="DepartmentCode"  Enabled="false" />
                        <asp:EntityDataSource ID="edsDepartment" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Department"
                             Select="it.[DepartmentCode], it.[DepartmentName]">
                        </asp:EntityDataSource>
                    </td>
                    <td>
                        岗位：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpPosition" runat="server" Width="135px" SelectedValue='<%# Bind("PositionId") %>'
                             DataSourceID="edsPosition" DataTextField="PositionName" DataValueField="PositionId" Enabled="false" />
                        <asp:EntityDataSource ID="edsPosition" runat="server" ConnectionString="name=GoldEntities"
                             DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Position"
                             Select="it.[PositionId], it.[PositionName]">
                        </asp:EntityDataSource>
                    </td>
                    <td style="width: 40px">
                        角色：
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoles" runat="server" Text='' Enabled="false" Width="200px" OnLoad="txtRoles_Load" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td colspan="6">
                        <asp:LinkButton ID="EditButton" runat="server" CausesValidation="True" CommandName="Edit"
                            Text="编辑" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="CreateButton" runat="server" CausesValidation="false" CommandName="Create"
                            Text="新建用户" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <EmptyDataTemplate>
        <table >
                <tr style="height: 30px">
                    <td style="width: 40px;">
                        工号：
                    </td>
                    <td style="width: 150px;">
                        <asp:TextBox ID="txtUserCode" runat="server" Text='' Enabled="false" Width="130px" />
                    </td>
                    <td style="width: 40px">
                        姓名：
                    </td>
                    <td style="width: 150px">
                        <asp:TextBox ID="txtUserName" runat="server" Text='' Enabled="false" Width="130px" />
                    </td>
                    <td style="width: 40px">
                        状态：
                    </td>
                    <td>
                        <asp:CheckBox ID="cboxStatus" runat="server" Enabled="false" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td>
                        部门：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpDepartment" runat="server"  Enabled="false" Width="135px" />
                    </td>
                    <td>
                        岗位：
                    </td>
                    <td>
                        <asp:DropDownList ID="drpPosition" runat="server"  Enabled="false" Width="135px" />
                    </td>
                    <td style="width: 40px">
                        角色：
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoles" runat="server" Text='' Enabled="false" Width="200px" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td colspan="6">
                        <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="false" CommandName="Create"
                            Text="新建用户" />
                    </td>
                </tr>
            </table>
        </EmptyDataTemplate>
    </asp:FormView>
    <asp:EntityDataSource ID="EntityDataSource2" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" EnableInsert="True"
        EnableUpdate="True" EntitySetName="Users" Where="it.UserId=@UserId">
        <WhereParameters>
            <asp:ControlParameter ControlID="GridView1" Type="String" Name="UserId" PropertyName="SelectedValue" />
        </WhereParameters>
    </asp:EntityDataSource>
        </fieldset>
        <br />
        <asp:Button ID="btnNCDataImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC数据导入" OnClick="btnNCDataImport_Click" OnClientClick="return AskConfirm();"/>
        <hr />

        <fieldset>
            <legend>用户列表</legend>
<%--    <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Users"
        Include="Department,Position" EntityTypeFilter="Users" Where="it.DepartmentCode=@DeptCode" Select="" >
        <WhereParameters>
            <asp:ControlParameter ControlID="TreeView1" Type="String" Name="DeptCode" PropertyName="SelectedValue" /> 
        </WhereParameters>
    </asp:EntityDataSource>--%>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="UserId" CssClass="linetable"
         onrowcommand="GridView1_RowCommand" sortExpression="UserId" sortDirection="ASC"  AllowSorting="true"
                onsorting="GridView1_Sorting"><%--DataSourceID="EntityDataSource1"--%>
        <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="工 号" ReadOnly="True" SortExpression="UserId" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" />
            <asp:TemplateField HeaderText="姓 名" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" SortExpression="UserName">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnUserName" runat="server" CommandName="Select" CommandArgument='<%# Eval("UserId") %>'
                        Text='<%# Eval("UserName") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="所属部门" SortExpression="DepartmentCode">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblDepartment" Text='<%# Bind("Department.DepartmentName") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="150px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="岗 位" SortExpression="PositionId">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblPosition" Text='<%# Bind("Position.PositionName") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="150px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="状 态" SortExpression="Enabled">
                <ItemTemplate>
                    <asp:CheckBox ID="cboxStatus" runat="server" Checked='<%# Bind("Enabled") %>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="150px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="重置密码" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnResetPassword" runat="server" CommandName="ResetPassword" CommandArgument='<%# Eval("UserId") %>'
                        Text="重置密码" OnClientClick="javascript:return confirm('确定要对此用户重置密码？');" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="删除用户" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDeleteUser" runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("UserId") %>'
                        Text="删除用户" OnClientClick="javascript:return confirm('确定要删除此用户吗？');" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

        </fieldset>
    </div>



        <asp:Panel ID="pnlRoles" runat="server" CssClass="popupControl">
            <div style="border: 1px outset white; width: 160px">
                <asp:CheckBoxList ID="cboxListRoles" runat="server" AutoPostBack="True" />
            </div>
        </asp:Panel>

        <div id="divWait" style="display: none;">
            <div align="right">
                <img src="../Styles/images/close.png" class="divWait_Close"
                    onclick="hiddenWaitDiv('divWait');" /></div>
            <div>
                <img src="../Styles/images/uploading.gif" /><label>执行中,请稍候……</label>
                <br /></div>
        </div>


            <!--下面是类似于MessageBox的功能，使用下面的控件需要在每个页面引用MsgBoxJScript.js及样式文件-->
        <asp:LinkButton runat="server" ID="showModalPopupServerOperatorButton_Msg" Visible="false"
            Text="shown via server in code behind" OnClick="showModalPopupServerOperatorButton_Click" />
        <a id="showModalPopupClientButton_Msg" href="#" style="display:none;">on the client in script</a>
        <asp:Button runat="server" ID="hiddenTargetControlForModalPopup_Msg" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="programmaticModalPopup_Msg" BehaviorID="programmaticModalPopupBehavior_Msg"
            TargetControlID="hiddenTargetControlForModalPopup_Msg" PopupControlID="programmaticPopup_Msg"
            BackgroundCssClass="modalBackground_Msg" DropShadow="True" PopupDragHandleControlID="programmaticPopupDragHandle_Msg"
            RepositionMode="RepositionOnWindowScroll">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel runat="server" CssClass="modalPopup_Msg" ID="programmaticPopup_Msg" Style="display: none;">
            <asp:Panel runat="Server" ID="programmaticPopupDragHandle_Msg" CssClass="programmaticPopupDragHandle_Msg_Css">
                <div style="float: left;">
                    系统提示</div>
                <div style="float: right;">
                    <input type="button" id="hideModalPopupViaClientButton_Msg" class="ClosePopButton_Msg"
                        value="关闭" />
                    <!--<a id="hideModalPopupViaClientButton_Msg" href="#" class="ClosePopButton" >关闭</a>-->
                </div>
            </asp:Panel>
            <div style="padding: 10px;">
                <asp:Label ID="lblMessageContent" runat="server" Text=""></asp:Label>
                <br />
                <div id="divMessageDetail" runat="server">
                <hr />
                <span>详细信息</span>
                <%--<asp:Label ID="lblMessageDetailTitle" runat="server" Text="详细信息"></asp:Label>--%>
                <br />
                <asp:Label ID="lblMessageContentException" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </asp:Panel>
</asp:Content>
