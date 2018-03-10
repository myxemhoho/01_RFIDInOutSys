<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Departments.aspx.cs" Inherits="Gold.SystemSetting.Departments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />    
    <script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">


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
            <asp:TreeView ID="TreeView1" runat="server" ImageSet="Simple" SkipLinkText="" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="0px"
                    NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                    VerticalPadding="0px" />
            </asp:TreeView>
            <hr />
            <asp:Button ID="btnNCDataImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC数据导入" OnClick="btnNCDataImport_Click" OnClientClick="return AskConfirm();"/>
        </fieldset>
    </div>
    <div class="rightList">
        <fieldset>
            <legend>部门详情</legend>
            <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Department"
                EnableInsert="True" EnableUpdate="True" Where="it.DepartmentCode=@DeptCode" OnInserted="EntityDataSource1_Inserted"
                OnUpdated="EntityDataSource1_Updated">
                <WhereParameters>
                    <asp:ControlParameter ControlID="TreeView1" Name="DeptCode" Type="String" PropertyName="SelectedNode.Value" />
                </WhereParameters>
            </asp:EntityDataSource>
            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="DepartmentCode"  CssClass="linetable"
                DataSourceID="EntityDataSource1" OnDataBound="DetailsView1_DataBound">
                <Fields>
                    <asp:TemplateField HeaderText="部门编码">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DepartmentCode") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("DepartmentCode") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DepartmentCode") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CodeRequired" runat="server" ControlToValidate="TextBox1"
                                CssClass="failureNotification" ToolTip="部门编码不能为空！" ValidationGroup="CreateDepartmentValidationGroup">必须输入</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ControlStyle Width="180px" />
                        <HeaderStyle HorizontalAlign="Center" Width="100px" />
                        <ItemStyle Height="30px" HorizontalAlign="Left" Width="400px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="部门名称" SortExpression="DepartmentName">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("DepartmentName") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DepartmentName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CodeRequired" runat="server" ControlToValidate="TextBox1"
                                CssClass="failureNotification" ErrorMessage="必须填写部门编码" ToolTip="部门编码不能为空！" ValidationGroup="UpdateDepartmentValidationGroup">必须输入</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DepartmentName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="TextBox2"
                                CssClass="failureNotification" ErrorMessage="必须填写部门名称" ToolTip="部门名称不能为空！" ValidationGroup="CreateDepartmentValidationGroup">必须输入</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ControlStyle Width="180px" />
                        <HeaderStyle HorizontalAlign="Center" Width="100px" />
                        <ItemStyle Height="30px" HorizontalAlign="Left" Width="200px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ManagerID" HeaderText="部门主管" SortExpression="ManagerID">
                    <ControlStyle Width="180px" />
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" Width="200px" Height="30px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="上级部门" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Width="200px" ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:DropDownList ID="DepartmentsDropDownList" runat="server" Width="180px" DataTextField="DepartmentName"
                                DataValueField="DepartmentCode" SelectedValue='<%# Bind("Parent") %>' OnInit="DepartmentsDropDownList_Init"
                                DataSourceID="edsDepartment">
                            </asp:DropDownList>
                            <asp:EntityDataSource ID="edsDepartment" runat="server" ConnectionString="name=GoldEntities"
                                DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="Department"
                                Select="it.[DepartmentCode], it.[DepartmentName]">
                            </asp:EntityDataSource>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Left" Height="30px" Width="200px"></ItemStyle>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Description" HeaderText="备注" SortExpression="Description">
                    <ControlStyle Width="180px" />
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" Width="200px" Height="30px" />
                    </asp:BoundField>
                    <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100%"
                        ItemStyle-Height="30px">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" Text="编 辑" CommandName="Edit" runat="server" CssClass="ButtonImageStyle"/>
                            <br />
                            <br />
                            <asp:Button ID="btnNew" Text="新建子部门" CommandName="New" runat="server" CssClass="ButtonImageStyle"/>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Button ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" CssClass="ButtonImageStyle" ValidationGroup="UpdateDepartmentValidationGroup" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" Text="取 消" CommandName="Cancel" runat="server" CssClass="ButtonImageStyle" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInsert" Text="保 存" CommandName="Insert" runat="server" CssClass="ButtonImageStyle" ValidationGroup="CreateDepartmentValidationGroup" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" Text="取 消" CommandName="Cancel" runat="server" CssClass="ButtonImageStyle" />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Height="30px" Width="200px"></ItemStyle>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <hr />
            <label>部门名称：</label>
            <asp:Label ID="lblCurrentDeptName" runat="server" Text=""></asp:Label>
            <label>&nbsp;&nbsp;&nbsp;&nbsp;部门编码：</label>
            <asp:Label ID="lblCurrentDeptCode" runat="server" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDelDept" Text="删除此部门" runat="server" 
                CssClass="ButtonImageStyle" OnClientClick="return confirm('确定要删除此部门吗？');" onclick="btnDelDept_Click"/>
        </fieldset>
    </div>

    <div id="divWait" style="display: none;">
            <div align="right">
                <img src="../Styles/images/close.png" class="divWait_Close"
                    onclick="hiddenWaitDiv('divWait');" /></div>
            <div>
                <img src="../Styles/images/uploading.gif" /><label>执行中,请稍候……</label>
                <br /></div>
        </div>

    <div>
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
    </div>
</asp:Content>
