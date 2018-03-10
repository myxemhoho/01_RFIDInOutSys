<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="WareHouseList.aspx.cs" Inherits="Gold.BaseInfoSetting.WareHouseList" %>



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

        //gridViewClientID是GridView的客户端ID
        //allCheckBoxObj 是触发全选的那个CheckBox对象，调用时使用This
        function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
            //方法1只适合在IE中使用
            //            var theTable = document.getElementById(gridViewClientID);
            //            var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
            //            var i;
            //            var j = 0; //checkbox的列索引

            //            if (theTable == undefined)
            //                return;
            //                
            //            for (i = 0; i < theTable.rows.length; i++) {
            //                var objCheckBox = theTable.rows[i].cells[j].firstChild;
            //                if (objCheckBox.checked != null) 
            //                    objCheckBox.checked = obj.checked;
            //            }

            //方法2 在IE，Chrome，FireFox均通用
            var grid = document.getElementById(gridViewClientID); //获取
            var theChkCheckAll = allCheckBoxObj; //触发全选的CheckBox
            var elements = grid.getElementsByTagName("input");
            var j = 0;
            for (j = 0; j < elements.length; j++) {
                if (elements[j] != null && elements[j].type == "checkbox") {
                    var checkedValue = theChkCheckAll.checked;
                    elements[j].checked = checkedValue;
                }
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
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                查询条件</label>
        </div>
        <div class="commonQuery">
            <asp:Label ID="lblName" CssClass="commonLabel" runat="server" Text="仓库名称："></asp:Label>
            <asp:TextBox ID="tbxWHName" runat="server"></asp:TextBox>
            <span style="width: 100px;"></span>
            <asp:Label ID="lblCode" CssClass="commonLabel" runat="server" Text="仓库代码："></asp:Label>
            <asp:TextBox ID="tbxWHCode" runat="server"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
              OnClientClick="showWaitDiv('divWait');" OnClick="btnQuery_Click" Text="查询" />
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
                <asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                <asp:Button ID="btnNCDataImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC数据导入" OnClick="btnNCDataImport_Click" OnClientClick="return AskConfirm();"/>
                <asp:Button ID="btnShowAddPage" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="新增仓库" OnClick="btnShowAddPage_Click" />
                <asp:Button ID="btnShowEditPage" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="编辑仓库" OnClick="btnShowEditPage_Click" />
                <asp:Button ID="btnDelete" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="删除仓库" OnClick="btnDelete_Click" />
                <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server"
                    TargetControlID="btnDelete" ConfirmText="确定要删除吗？">
                </ajaxToolkit:ConfirmButtonExtender>
            </div>
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
            <asp:GridView ID="gv_WareHouseList" runat="server" AllowPaging="True" AllowSorting="true"  CssClass="linetable" Width="100%" 
                PageSize="10" AutoGenerateColumns="False" OnPageIndexChanging="gv_WareHouseList_PageIndexChanging"
                OnSorting="gv_WareHouseList_Sorting" sortExpression="WHCode" sortDirection="ASC"
                OnDataBound="gv_WareHouseList_DataBound">
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
                                仓库编码
                             
                            </td>
                            <td>
                                仓库名称
                             
                            </td>
                            <td>
                                仓库类型
                             
                            </td>
                            <td>
                                仓库启用状态
                             
                            </td>
                            <td>
                                仓库地址
                             
                            </td>
                            <td>
                                仓库联系电话
                             
                            </td>
                            <td>
                                仓库描述
                             
                            </td>
                            <td>
                                仓库名称
                             
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
                    <asp:TemplateField>
                        <ItemStyle Width="5%" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="javascript:selectAllCheckBox('<%=gv_WareHouseList.ClientID %>',this);" />
                            全选
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="仓库编码" SortExpression="WHCode">
                        <ItemStyle Width="20%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWHCode" runat="server" Text='<%# Eval("WHCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHName" ItemStyle-Width="25%" />
                    <asp:TemplateField HeaderText="仓库类型" SortExpression="WHType">
                        <ItemStyle Width="10%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWHType" runat="server" Text='<%# Eval("WHType").ToString()=="1"?"实体仓库":"虚拟仓库" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="仓库启用状态" SortExpression="Enabled">
                        <ItemStyle Width="10%" />
                        <ItemTemplate>
                            <asp:Label ID="lblEnabled" runat="server" Text='<%# Eval("Enabled").ToString().ToLower()=="true"?"已启用":"未启用" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:BoundField DataField="WHType" HeaderText="仓库类型" SortExpression="WHType" />
                    <asp:BoundField DataField="Enabled" HeaderText="仓库启用状态" SortExpression="Enabled" />--%>
                    <asp:BoundField DataField="Address" HeaderText="仓库地址" SortExpression="Address" ItemStyle-Width="10%" />
                    <asp:BoundField DataField="Phone" HeaderText="仓库联系电话" SortExpression="Phone" ItemStyle-Width="10%" />
                    <asp:BoundField DataField="Comment" HeaderText="仓库描述" SortExpression="Comment" ItemStyle-Width="10%" />
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: right">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />
                                页
                                    共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />
                                页
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
                                    CommandName="Page" Text="GO" />
                                <!-- here set the CommandArgument of the Go Button to '-1' as the flag -->
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
            </asp:GridView>
            <br />
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
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
    </div>
</asp:Content>
