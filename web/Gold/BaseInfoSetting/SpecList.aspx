<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SpecList.aspx.cs" Inherits="Gold.BaseInfoSetting.SpecList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
    <style type="text/css">
        
    </style>
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

        //在服务器控件上记录当前鼠标在GridView内点击的X,Y坐标值
        function RecordPostion(obj) {
            var div1 = obj;
            var sx = document.getElementById('MainContent_dvscrollX');
            var sy = document.getElementById('MainContent_dvscrollY');

            var header = document.getElementById("MainContent_divHeader"); //自定义GridView标题栏

            var divScrollTop = div1.scrollTop;
            var divScrollLeft = div1.scrollLeft;

            sy.value = divScrollTop
            sx.value = divScrollLeft

            header.scrollLeft = divScrollLeft; //设置标题栏横向坐标
        }

        //从服务器控件中获取PostBack前鼠标GridView内点击的X,Y坐标值
        function GetResultFromServer() {
            try {
                var sx = document.getElementById('MainContent_dvscrollX');
                var sy = document.getElementById('MainContent_dvscrollY');
                var header = document.getElementById("MainContent_divHeader"); //自定义GridView标题栏

                var syValue = sy.value;
                var sxValue = sx.value;

                document.getElementById('gridviewContainer').scrollTop = syValue;
                document.getElementById('gridviewContainer').scrollLeft = sxValue;


                header.scrollLeft = sxValue; //设置标题栏横向坐标
            } catch (e) {
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                查询条件</label>
        </div>
        <div class="commonQuery">
            <asp:Label ID="lblName" CssClass="commonLabel" runat="server" Text="规格名称："></asp:Label>
            <asp:TextBox ID="tbxName" runat="server"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                OnClick="btnQuery_Click" Text="查询" />
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">                
                <asp:Button ID="btnNCDataImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC数据导入" OnClick="btnNCDataImport_Click" OnClientClick="return AskConfirm();"/>
            </div>
        </div>
        <div>
            <asp:EntityDataSource ID="EntityDataSource_gv" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableDelete="True" EnableFlattening="False"
                EnableInsert="True" EnableUpdate="True" EntitySetName="Specifications" AutoGenerateWhereClause="false"
                Where="it.SpecName like '%'+@SpecName+'%'">
                <WhereParameters>
                    <asp:ControlParameter ControlID="tbxName" Name="SpecName" PropertyName="Text" DefaultValue="%"
                        Type="String" />
                </WhereParameters>
            </asp:EntityDataSource>
             <!--
             固定GridView标题的方法
             1.GridView外用带滚动条的div包着，并设置GridView的宽度
             2.另外做一个静态Table放在div中，此div宽度与包含GridView的div的宽度一致
             3.设置静态Table的宽度与GridView的宽度一致
             4.在包含GridVidw的div中写javascript onscroll="divHeader.scrollLeft=this.scrollLeft"
             5.在静态Table中要排序的列单元格中防止LinkButton用来排序，并在后台代码中写排序方法
             7.设置GridView标记中的ShowHeader=false
             -->
        <div id="divWait" style="display: none;">
            <div align="right">
                <img src="../Styles/images/close.png" class="divWait_Close"
                    onclick="hiddenWaitDiv('divWait');" /></div>
            <div>
                <img src="../Styles/images/uploading.gif" /><label>执行中,请稍候……</label>
                <br /></div>
        </div>
        <div id="divHeader" runat="server" style="width: 99%; overflow-x: hidden; overflow-y: scroll;"
            class="divFixHeader">
            <%--<table class="GridViewEmpty_Table" style=" width:1500px;">
                        <tr class="GridViewHeaderStyle">
                            <td>
                                序号
                            </td>
                            <td>
                                <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument="CargoCode" 
                                    onclick="LinkButtonHeader_Click">商品编码</asp:LinkButton>
                            </td>
                        </tr>
                    </table>--%>           
        </div>

        <!--设置点击GridView内控件PostBack后让GridView内行数据滚动到点击前坐标位置步骤：
        1.添加两个服务端隐藏域控件dvscrollX,dvscrollY
        2.添加JS函数RecordPostion和GetResultFromServer，注意这两个个函数中的相关控件ID要和页面中的一致
        3.在包含GridView的div中添加onscroll="j a v ascript:RecordPostion(this);"JS函数调用
        4.在后台CS代码文件中GridView_DataBound事件函数中添加调用JS的代码 string sjs = "GetResultFromServer();";  ScriptManager.RegisterClientScriptBlock(this.gv_CargoList, this.GetType(), "", sjs, true);
        -->
        <asp:HiddenField ID="dvscrollX" runat="server" />
        <asp:HiddenField ID="dvscrollY" runat="server" />
        <div style="width: 99%; height: 500px;" class="divScroll" id="gridviewContainer"
            onscroll="javascript:RecordPostion(this); "><!--onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;-->
            <asp:GridView ID="gv_SpecList" runat="server" AllowPaging="false" ShowHeader="false" PageSize="15" AllowSorting="True" Width="650px"
                CssClass="linetable" AutoGenerateColumns="False" DataKeyNames="SpecId" DataSourceID="EntityDataSource_gv"
                OnPageIndexChanging="gv_SpecList_PageIndexChanging" OnRowCommand="gv_SpecList_RowCommand"
                OnRowDeleted="gv_SpecList_RowDeleted" OnRowUpdated="gv_SpecList_RowUpdated" OnRowUpdating="gv_SpecList_RowUpdating"
                OnDataBound="gv_SpecList_DataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                     <table class="GridViewEmpty_Table">
                        <tr class="GridViewEmpty_RowHeader">
                            <td>
                                规格名称
                            </td>
                            <td>
                                备注
                            </td>
                        </tr>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="8">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="50px"/>
                        <HeaderStyle HorizontalAlign="Center"  />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SpecId" HeaderText="规格编号" SortExpression="SpecId" Visible="false"
                        ReadOnly="True" />
                    <asp:TemplateField HeaderText="规格名称" SortExpression="SpecName" ItemStyle-Width="300px"
                        ItemStyle-VerticalAlign="Top">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowSpecName" runat="server" Text='<%# Bind("SpecName") %>'></asp:TextBox>
                            <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowSpecName" ControlToValidate="tbxRowSpecName"
                                ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                runat="server" ErrorMessage="规格名称不能为空"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator_tbxRowSpecName" ControlToValidate="tbxRowSpecName"
                                ValidationGroup="RowEditGroup" runat="server" CssClass="commonSaveMsgLabel" ErrorMessage="长度过长"
                                Display="Dynamic" OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowSpecName" runat="server" Text='<%# Bind("SpecName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="规格备注" SortExpression="Comment" ItemStyle-Width="200px"
                        ItemStyle-VerticalAlign="Top">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowComment" runat="server" Text='<%# Bind("Comment") %>'></asp:TextBox>
                            <br />
                            <asp:CustomValidator ID="CustomValidator_tbxRowComment" ControlToValidate="tbxRowComment"
                                ValidationGroup="RowEditGroup" runat="server" CssClass="commonSaveMsgLabel" ErrorMessage="长度过长"
                                Display="Dynamic" OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowComment" runat="server" Text='<%# Bind("Comment") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编 辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnNew" Text="删 除" CommandName="Delete" CausesValidation="false"
                                runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取 消" CommandName="Cancel" CausesValidation="false"
                                runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: right">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />页
                                    &nbsp; 共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />页
                                </span>
                                <asp:LinkButton ID="btnFirst" runat="server" CausesValidation="False" CommandArgument="First"
                                    CommandName="Page" Text="首页" />
                                <asp:LinkButton ID="btnPrev" runat="server" CausesValidation="False" CommandArgument="Prev"
                                    CommandName="Page" Text="上一页" />
                                <asp:LinkButton ID="btnNext" runat="server" CausesValidation="False" CommandArgument="Next"
                                    CommandName="Page" Text="下一页" />
                                <asp:LinkButton ID="btnLast" runat="server" CausesValidation="False" CommandArgument="Last"
                                    CommandName="Page" Text="尾页" />
                                <asp:TextBox ID="txtNewPageIndex" runat="server" Width="20px" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />
                                <asp:LinkButton ID="btnGo" runat="server" CausesValidation="False" CommandArgument="-1"
                                    CommandName="Page" Text="GO" /><!-- here set the CommandArgument of the Go Button to '-1' as the flag -->
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
            </asp:GridView>
            </div>
            <br />
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
        <br />
        <div class="commonTitle">
            <label>
                新增规格信息</label>
        </div>
        <div class="commonQuery">
            <label class="commonLabel">
                规格名称：</label>
            <asp:TextBox ID="tbxNewSpecName" runat="server" OnTextChanged="tbxNewSpecName_TextChanged"></asp:TextBox>
            <label class="commonSaveMsgLabel">
                *</label>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="commonSaveMsgLabel"
                runat="server" ControlToValidate="tbxNewSpecName" ErrorMessage="规格名称不能为空！" Display="Dynamic"
                ValidationGroup="SpecNewGroup"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="CustomValidator_tbxNewSpecName" ControlToValidate="tbxNewSpecName"
                ValidationGroup="SpecNewGroup" runat="server" CssClass="commonSaveMsgLabel" ErrorMessage="长度过长"
                Display="Dynamic" OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
            <span style="width: 100px;"></span>
            <label class="commonLabel">
                规格备注：</label>
            <asp:TextBox ID="tbxNewComment" runat="server"></asp:TextBox>
            <asp:CustomValidator ID="CustomValidator_tbxNewComment" ControlToValidate="tbxNewComment"
                ValidationGroup="SpecNewGroup" CssClass="commonSaveMsgLabel" runat="server" ErrorMessage="长度过长"
                Display="Dynamic" OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
            <asp:Button ID="btnAddNew" runat="server" CssClass="ButtonImageStyle" Text="新增" ValidationGroup="SpecNewGroup"
                CausesValidation="true" OnClick="btnAddNew_Click" />
            <asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" Text="清空" OnClick="btnClear_Click"
                CausesValidation="false" />
            <asp:Label ID="lblAddMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
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
    </div>
</asp:Content>
