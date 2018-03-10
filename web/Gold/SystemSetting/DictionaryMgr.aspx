<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DictionaryMgr.aspx.cs" Inherits="Gold.SystemSetting.DictionaryMgr" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="leftQuery">
    <fieldset>
        <legend>字典列表</legend>
        <asp:TreeView ID="TreeView1" runat="server" ImageSet="Arrows" >
            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
            <ParentNodeStyle Font-Bold="False" />
            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" 
                HorizontalPadding="0px" VerticalPadding="0px" />
        </asp:TreeView>
        <hr />
        <table>
            <tr>
                <td colspan="2">新增字典项：</td>
            </tr>
            <tr>
                <td>代码：</td>
                <td>
                    <asp:TextBox ID="txtRootCode" runat="server" Width="100px" MaxLength="32" />
                </td>
            </tr>
            <tr>
                <td>显示名称：</td>
                <td>
                    <asp:TextBox ID="txtRootName" runat="server" Width="100px" MaxLength="100" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height:30px">
                    <asp:Button ID="btnSaveRoot" runat="server" onclick="btnSaveRoot_Click" Text=" 保 存 " CssClass="ButtonImageStyle" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                <asp:Label ID="lblrootMessage" runat="server" ForeColor="Red" Text="" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>

<div class="rightList" style="width:800px;">
    <fieldset>
        <legend>值定义</legend>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="linetable"
            DataKeyNames="Id" DataSourceID="EntityDataSource1" 
            onrowdatabound="GridView1_RowDataBound" 
            onrowcommand="GridView1_RowCommand" >
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
            <Columns>
                <%--<asp:BoundField DataField="Code" HeaderText="代码" 
                    SortExpression="Code">
                <ItemStyle HorizontalAlign="Center" Width="80px" />
                <ControlStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="Name" HeaderText="名称" SortExpression="Name">
                <ItemStyle HorizontalAlign="Center" Width="200px" />
                </asp:BoundField>--%>
                <asp:TemplateField HeaderText="代码" SortExpression="Code" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblCode" runat="server" Text='<%# Eval("Code") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowCode" runat="server" Text='<%# Bind("Code") %>' MaxLength="32" Width="150px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                <asp:TemplateField HeaderText="名称" SortExpression="Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="300px">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowName" runat="server" Text='<%# Bind("Name") %>' MaxLength="100" Width="300px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                <asp:BoundField DataField="Order" HeaderText="显示顺序" SortExpression="Order">
                <ItemStyle HorizontalAlign="Center" Width="60px" />
                <ControlStyle Width="60px" />
                </asp:BoundField>
                <asp:CheckBoxField DataField="Enabled" HeaderText="启用" SortExpression="Enabled">
                <ItemStyle HorizontalAlign="Center" Width="50px" />
                <ControlStyle Width="50px" />
                </asp:CheckBoxField>
                <%--<asp:CommandField ShowEditButton="true" EditText="编辑" 
                 UpdateText="更新" CancelText="取消"
                 ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px" />--%>
                 <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编 辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup" />                            
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" ValidationGroup="RowEditGroup" />   
                            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server" TargetControlID="btnUpdate" ConfirmText="修改用友接口程序配置参数URL将会导致网站重启，所有网站用户会被强制退出，（建议在网站无其他人使用时进行此项操作），确定要继续进行修改吗？  " Enabled="false">
                            </ajaxToolkit:ConfirmButtonExtender>                         
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取 消" CommandName="Cancel" CausesValidation="false"
                                runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                没有数据
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:EntityDataSource ID="EntityDataSource1" runat="server" 
            ConnectionString="name=GoldEntities" 
            DefaultContainerName="GoldEntities" EnableFlattening="False" 
            EnableUpdate="True" EntitySetName="DataDict"
            Where="it.Category=@Code" onupdated="EntityDataSource1_Updated" 
            onupdating="EntityDataSource1_Updating" >
            <WhereParameters>
                <asp:ControlParameter ControlID="TreeView1" Name="Code" Type="String" PropertyName="SelectedNode.Value" />
            </WhereParameters>
        </asp:EntityDataSource>
    </fieldset>

    <fieldset>
        <legend>新增值定义</legend>
        <table>
            <tr>
                <td>代码：</td>
                <td>
                    <asp:TextBox ID="txtValueCode" runat="server" MaxLength="100" Width="200px" Text="" />
                </td>
            </tr>
            <tr>
                <td>名称：</td>
                <td>
                    <asp:TextBox ID="txtValueName" runat="server" MaxLength="100" Width="200px" Text="" />
                </td>
            </tr>
            <tr>
                <td>显示顺序：</td>
                <td>
                    <asp:TextBox ID="txtValueOrder" runat="server" MaxLength="100" Width="200px" Text="" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height:30px">
                    <asp:Button ID="btnSaveValue" runat="server" Text=" 保 存 " CssClass="ButtonImageStyle"  
                        onclick="btnSaveValue_Click" CausesValidation="False" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                <asp:Label ID="lblMessage2" runat="server" ForeColor="Red" Text="" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>

<br />
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
