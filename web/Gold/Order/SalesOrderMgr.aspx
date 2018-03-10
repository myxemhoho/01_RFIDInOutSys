<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SalesOrderMgr.aspx.cs" Inherits="Gold.Order.SalesOrderMgr" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/Silverlight.js"></script>
    <script type="text/javascript">
        function selectAll(obj) {
            var theTable = obj.parentElement.parentElement.parentElement;
            var i;
            var j = obj.parentElement.cellIndex;

            for (i = 0; i < theTable.rows.length; i++) {
                var objCheckBox = theTable.rows[i].cells[j].firstChild;
                if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
            }
        }
        
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Silverlight 应用程序中未处理的错误 " + appSource + "\n";

            errMsg += "代码: " + iErrorCode + "    \n";
            errMsg += "类别: " + errorType + "       \n";
            errMsg += "消息: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "文件: " + args.xamlFile + "     \n";
                errMsg += "行: " + args.lineNumber + "     \n";
                errMsg += "位置: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "行: " + args.lineNumber + "     \n";
                    errMsg += "位置: " + args.charPosition + "     \n";
                }
                errMsg += "方法名称: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        //显示等待滚动图片
        function showWaitDiv(divName) {
            document.getElementById(divName).style.display = "block";
        }
        //隐藏等待滚动图片
        function hiddenWaitDiv(divName) {
            document.getElementById(divName).style.display = "none";
        }

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <h2>
            <span>查询条件</span></h2>
        <div class="boxContent">
            <table>
                <tr>
                    <td style="width: 70px; text-align: right">
                        订单号：
                    </td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtOrderCode" runat="server" MaxLength="20" Width="90px" />
                    </td>
                    <td style="width: 100px; text-align: right">
                        销售部门：
                    </td>
                    <td style="width: 100px">                       
                       <asp:DropDownList ID="ddlSellDepartment" runat="server" Width="100%" 
                            DataSourceID="edsSellDepartment" DataTextField="Name" 
                            DataValueField="Name" AppendDataBoundItems="True" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsSellDepartment" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="SellDepartmentName" />
                            </WhereParameters>
                        </asp:EntityDataSource> 
                    </td>                    
                    <td style="width: 100px; text-align: right">
                        销售日期：
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
                </tr>
                <tr>
                    <td style="text-align: right; width:70px">
                        订单状态：
                    </td>
                    <td>                        
                        <asp:DropDownList ID="ddlOrderStatus" runat="server" Width="100%" 
                            DataSourceID="edsOrderStatus" DataTextField="Name" 
                            DataValueField="Code" AppendDataBoundItems="True" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsOrderStatus" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="OrderStatus" />
                            </WhereParameters>
                        </asp:EntityDataSource>    
                    </td>
                    <td style="text-align: right; width:100px;">
                        业务员：
                    </td>
                    <td>                      
                       <asp:DropDownList ID="ddlOperator" runat="server" Width="100%" 
                            DataSourceID="edsOperator" DataTextField="Name" 
                            DataValueField="Name" AppendDataBoundItems="True" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsOperator" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
                            </WhereParameters>
                        </asp:EntityDataSource>
                    </td>                    
                    <td style="text-align: right; width:100px;">
                        客户名称：
                    </td>
                    <td>
                        <asp:TextBox ID="txtClientName" runat="server" MaxLength="50" Width="187px" />
                        <ajaxToolkit:AutoCompleteExtender  runat="server" ID="aceClientName" Enabled="true"
                            TargetControlID="txtClientName" ServiceMethod="GetClientName"  CompletionInterval="300"
                            MinimumPrefixLength ="1" CompletionSetCount ="10" UseContextKey="True" >
                        </ajaxToolkit:AutoCompleteExtender>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                          OnClientClick="showWaitDiv('divWait');"  Text="查 询" onclick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="box">
        <h2>
            <span>订单列表</span>
            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="false"
               Text="删除订单" OnClick="btnDelete_Click" />
            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server"
                TargetControlID="btnDelete" ConfirmText="确定要删除吗？">
            </ajaxToolkit:ConfirmButtonExtender>
            <asp:LinkButton ID="btnNCDataImport" runat="server" Text="NC数据导入" OnClick="btnNCDataImport_Click" />
            <asp:LinkButton ID="lbtnImport" runat="server" Text="导入订单..." />
        </h2>
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
             <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="2300px"
                DataKeyNames="OrderCode" DataSourceID="EntityDataSource1" 
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
                    <%--<tr class="GridViewEmpty_RowHeader">
                        
                        <td>订单编码</td>
                       
                        <td>订单状态</td>
                        <td>销售日期</td>
                        <td>销售部门</td>
                        <td>业务员</td>
                        <td>客户名称</td>

                        <td>收货地址</td>
                        <td>业务类型</td>
                        <td>联系人</td>
                        <td>联系电话</td>
                        <td>制单人</td>
                        <td>合计数量</td>
                        <td>合计金额</td>
                        <td>合计折扣金额</td>

                        <td>仓库签名</td>
                        <td>财务签名</td>

                        <td>最后修改时间</td>
                        <td>RFID处理人ID</td>
                        <td>RFID处理人姓名</td>
                        <td>RFID处理时间</td>
                        <td>备注</td>
                        <td>预留1</td>
                        <td>预留2</td>

                    </tr>--%>
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="29">
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
                    <asp:HyperLinkField DataNavigateUrlFields="OrderCode"  SortExpression="OrderCode"
                        DataNavigateUrlFormatString="SalesOrderView.aspx?orderCode={0}" 
                        DataTextField="OrderCode" HeaderText="订单编码"  Target="_blank">
                    <ItemStyle Width="120px" />
                    </asp:HyperLinkField>                   
                    <%--<asp:BoundField DataField="NCOrderCode" HeaderText="NC订单编号" 
                        SortExpression="NCOrderCode" />--%>
                    <asp:TemplateField HeaderText="订单状态" SortExpression="OrderStatus" ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetOrderStatus(Eval("OrderStatus"))%>   
                            <asp:TextBox runat="server" ID="txtOrderCode" Visible="false" Text='<%# Eval("OrderCode") %>'/>
                          <asp:TextBox runat="server" ID="txtOrderStatus" Visible="false" Text='<%# Eval("OrderStatus") %>' />                         
                        </ItemTemplate>                        
                    </asp:TemplateField>                     
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
                    <asp:BoundField DataField="RFIDActorID" HeaderText="RFID处理人ID" ItemStyle-Width="80px"
                        SortExpression="RFIDActorID" />
                    <asp:BoundField DataField="RFIDActorName" HeaderText="RFIDA处理人姓名" ItemStyle-Width="90px"
                        SortExpression="RFIDActorName" />
                    <asp:BoundField DataField="RFIDActorTime" HeaderText="RFID处理时间" ItemStyle-Width="140px"
                        SortExpression="RFIDActorTime" />
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
            <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableFlattening="False" 
                EntitySetName="SalesOrder" 
                onquerycreated="EntityDataSource1_QueryCreated" >
            </asp:EntityDataSource>
        </div>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
    </div>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopup">
        <div class="modalPopupWrapper">
            <div id="pnlDragTitle" class="modalHeader">
                <span>销售订单文件导入</span>
                <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="OK" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <div id="silverlightControlHost" class="uploadControl">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="../ClientBin/SLFileUpload.xap" />
                        <param name="initParams" value="UploadPage=../Upload/FileUpload.ashx,FileType=销售订单" />
                        <param name="onError" value="onSilverlightError" />
                        <param name="background" value="white" />
                        <param name="minRuntimeVersion" value="5.0.61118.0" />
                        <param name="autoUpgrade" value="true" />
                        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration: none">
                            <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="获取 Microsoft Silverlight"
                                style="border-style: none" />
                        </a>
                    </object>
                    <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px;
                        border: 0px"></iframe>
                </div>
            </div>
         </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="lbtnImport"
        PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
        PopupDragHandleControlID="pnlDragTitle">
    </ajaxToolkit:ModalPopupExtender>
</asp:Content>
