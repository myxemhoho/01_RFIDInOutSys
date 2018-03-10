<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesOrderFromNC.aspx.cs" Inherits="Gold.Order.SalesOrderFromNC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
//        function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
//            var theTable = document.getElementById(gridViewClientID);
//            var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
//            var i;
//            var j = 0; //checkbox的列索引

//            if (theTable == undefined)
//                return;

//            for (i = 0; i < theTable.rows.length; i++) {
//                var objCheckBox = theTable.rows[i].cells[j].firstChild;
//                if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
//            }
        //        }

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">  
  <div class="box">
        <h2>
            <span>查询条件</span></h2>
        <div class="boxContent">
            <table>               
                <tr>
                    <td style="width: 100px; text-align: right">
                        订单时间：
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartTime" runat="server" MaxLength="32" Width="80px" />
                        至
                        <asp:TextBox ID="txtEndTime" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server"
                            TargetControlID="txtStartTime" >
                        </ajaxToolkit:CalendarExtender>
                        <ajaxToolkit:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Enabled="True"
                            TargetControlID="txtEndTime">
                        </ajaxToolkit:CalendarExtender>
                        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red" />
                    </td>
                    <td style="text-align: right; width:100px;">
                        是否已导入：
                    </td>
                    <td>                       
                       <asp:DropDownList ID="ddlisAlreadyStatus" runat="server" Width="80px">
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                            <asp:ListItem Text="未导入" Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="已导入" Value="1"></asp:ListItem>                            
                        </asp:DropDownList>
                         
                    </td>
                    
                    <td style="text-align: right;">
                        &nbsp;</td>
                    <td> 
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                          OnClientClick="showWaitDiv('divWait');"  Text="查 询" onclick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div> 
  <div class="box">
        <%--<h2>
            <span>订单列表</span>
            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="false"
               Text="删除订单" OnClick="btnDelete_Click" />
            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server"
                TargetControlID="btnDelete" ConfirmText="确定要删除吗？">
            </ajaxToolkit:ConfirmButtonExtender>
            <asp:LinkButton ID="lbtnImport" runat="server" Text="导入订单..." />            
        </h2>--%>
        <!--<div class="boxContent" style="overflow-x: scroll;overflow-x:auto; overflow-y:auto; border:solid 1px #999; min-height:500px;" >-->
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
        <div style="width: 99%; height: 500px;" class="divScroll" id="gridviewContainer"
            onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="2000px"
                DataKeyNames="OrderCode" 
                AllowPaging="false" ShowHeader="false" RowStyle-HorizontalAlign="Center"                
                CssClass="linetable"              
                sortExpression="OrderCode" sortDirection="ASC" 
                OnSorting="GridView1_Sorting" AllowSorting="True" 
                OnPageIndexChanging="GridView1_PageIndexChanging" 
                OnDataBound="GridView1_DataBound">   
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                <table class="GridViewEmpty_Table">
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="25">
                            无数据
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>               
                <Columns>   
                    <asp:TemplateField HeaderText="*">
                        <ItemStyle Width="60px"/>
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>                                       
                    <asp:BoundField DataField="NCOrderCode" HeaderText="用友订单号" SortExpression="NCOrderCode" ItemStyle-Width="120px" />                                   
                    <%--<asp:BoundField DataField="NCOrderCode" HeaderText="NC订单编号" 
                        SortExpression="NCOrderCode" />--%>
                    <%--<asp:TemplateField HeaderText="订单状态" SortExpression="OrderStatus" ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetOrderStatus(Eval("OrderStatus"))%>   
                            <asp:TextBox runat="server" ID="txtOrderCode" Visible="false" Text='<%# Eval("OrderCode") %>'/>
                          <asp:TextBox runat="server" ID="txtOrderStatus" Visible="false" Text='<%# Eval("OrderStatus") %>' />                         
                        </ItemTemplate>                        
                    </asp:TemplateField>   --%>                  
                    <asp:BoundField DataField="SalesDate" HeaderText="销售日期" ItemStyle-Width="90px"
                        SortExpression="SalesDate" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="SellDepartmentName" HeaderText="销售部门" ItemStyle-Width="80px"
                        SortExpression="SellDepartmentName" />
                    <asp:BoundField DataField="Operator" HeaderText="业务员" ItemStyle-Width="70px"
                        SortExpression="Operator" />
                    <asp:BoundField DataField="ClientName" HeaderText="客户名称" ItemStyle-Width="200px"
                        SortExpression="ClientName" />
                    <%--<asp:BoundField DataField="MemberCard" HeaderText="会员卡号" 
                        SortExpression="MemberCard" />--%>
                    <asp:BoundField DataField="DeliveryAddr" HeaderText="收货地址" ItemStyle-Width="220px"
                        SortExpression="DeliveryAddr" />
                    <asp:BoundField DataField="BusinessType" HeaderText="业务类型" ItemStyle-Width="90px"
                        SortExpression="BusinessType" />
                    <asp:BoundField DataField="ContactPerson" HeaderText="联系人" ItemStyle-Width="80px"
                        SortExpression="ContactPerson" />
                    <asp:BoundField DataField="ContactPhoneNo" HeaderText="联系电话" ItemStyle-Width="100px"
                        SortExpression="ContactPhoneNo" />
                    <asp:BoundField DataField="Preparer" HeaderText="制单人" ItemStyle-Width="80px"
                        SortExpression="Preparer" />
                    <asp:BoundField DataField="TotalNumber" HeaderText="合计数量" ItemStyle-Width="70px"
                        SortExpression="TotalNumber" />
                    <asp:BoundField DataField="TotalMoney" HeaderText="合计金额" ItemStyle-Width="70px"
                        SortExpression="TotalMoney" />
                    <asp:BoundField DataField="DiscountMoney" HeaderText="合计折扣金额" ItemStyle-Width="70px"
                        SortExpression="DiscountMoney" />
                    <%--<asp:BoundField DataField="LeaderSign" HeaderText="领导签名" 
                        SortExpression="LeaderSign" />--%>
                    <asp:BoundField DataField="WarehouseSign" HeaderText="仓库签名" ItemStyle-Width="70px"
                        SortExpression="WarehouseSign" />
                   <asp:BoundField DataField="AccountSign" HeaderText="财务签名" ItemStyle-Width="70px"
                        SortExpression="AccountSign" />
                     <%--<asp:BoundField DataField="BusinessSign" HeaderText="业务签名" 
                        SortExpression="BusinessSign" />
                    <asp:BoundField DataField="EditorSign" HeaderText="制单人签名"
                        SortExpression="EditorSign" /> --%>
                    <asp:BoundField DataField="LastModifyTime" HeaderText="最后修改时间" ItemStyle-Width="140px"
                        SortExpression="LastModifyTime" />
                    <%--<asp:BoundField DataField="RFIDActorID" HeaderText="RFID处理人ID" ItemStyle-Width="80px"
                        SortExpression="RFIDActorID" />
                    <asp:BoundField DataField="RFIDActorName" HeaderText="RFIDA处理人姓名" ItemStyle-Width="90px"
                        SortExpression="RFIDActorName" />
                    <asp:BoundField DataField="RFIDActorTime" HeaderText="RFID处理时间" ItemStyle-Width="140px"
                        SortExpression="RFIDActorTime" />--%>
                    <asp:BoundField DataField="Comment" HeaderText="备注" ItemStyle-Width="240px"
                        SortExpression="Comment" />
                    <%--<asp:BoundField DataField="Reserve1" HeaderText="预留1" 
                        SortExpression="Reserve1" />
                    <asp:BoundField DataField="Reserve2" HeaderText="预留2" 
                        SortExpression="Reserve2" />--%>
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: left">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />页
                                    共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />页
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
                <RowStyle HorizontalAlign="Center"></RowStyle>
            </asp:GridView>
            <%--<asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableFlattening="False" 
                EntitySetName="PurchaseOrder" onquerycreated="EntityDataSource1_QueryCreated" >
            </asp:EntityDataSource>--%>
        </div>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        <div class="orderFooter"> 
            <br />               
            <asp:Button ID="btnSubmit" runat="server" CausesValidation="false" 
                Text="提交" CssClass="ButtonImageStyle" onclick="btnSubmit_Click" OnClientClick="javascript:return confirm('确定要将用友数据导入到RFID系统中吗？');"/>               
            <asp:Button ID="btnReturn" runat="server" CausesValidation="false" Visible="true"
                   Text="返回订单列表" CssClass="ButtonMidWidthImageStyle" onclick="btnReturn_Click" />    
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

