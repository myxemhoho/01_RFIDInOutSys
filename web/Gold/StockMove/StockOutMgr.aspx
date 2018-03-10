<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StockOutMgr.aspx.cs" Inherits="Gold.StockMove.StockOutMgr" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/Silverlight.js"></script>
    <script type="text/javascript">
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

        function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
            var theTable = document.getElementById(gridViewClientID);
            var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
            var i;
            var j = 0; //checkbox的列索引

            if (theTable == undefined)
                return;

            for (i = 0; i < theTable.rows.length; i++) {
                var objCheckBox = theTable.rows[i].cells[j].firstChild;
                if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
            }
        }        
    </script>
    <script  type="text/javascript">
        function SetRadioOut(nowRadio) 
        {
            var myForm, objRadio;
            myForm = document.forms[0];
            for (var i = 0; i < myForm.length; i++) 
            {
                if (myForm.elements[i].type == "radio") {
                    objRadio = myForm.elements[i];
                    if (objRadio != nowRadio && objRadio.name.indexOf("grdOutInfo") > -1 && objRadio.name.indexOf("radSelectOut") > -1) {
                        if (objRadio.checked) {
                            objRadio.checked = false;
                        }
                    } 
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
                    <td style="width: 70px; text-align: right; ">
                        出库单号：
                     
                    </td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtSOCode" runat="server" MaxLength="32" Width="90px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        订单号：
                     
                    </td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtFromOrderNo" runat="server" Width="90px" />
                    </td>
                    <td style="width: 100px; text-align: right">
                        单据状态：                     
                    </td>
                    <td style="width: 100px">
                        <asp:DropDownList ID="ddlSOStatus" runat="server"  Width="100%" AppendDataBoundItems="True"
                            DataSourceID="edsSOStatus" DataTextField="Name" 
                            DataValueField="Code" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsSOStatus" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />
                            </WhereParameters>
                        </asp:EntityDataSource>
                    </td>
                    <td style="width: 100px; text-align: right">
                        出库时间：                     
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
                    <td style="text-align: right; width:70px;">
                        单据类型：                     
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSOType" runat="server"  Width="100" AppendDataBoundItems="True"
                            DataSourceID="edsSOType" DataTextField="Name"
                            DataValueField="Code" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsSOType" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="SOType" />
                            </WhereParameters>
                        </asp:EntityDataSource>
                    </td>
                    <td style="text-align: right; width:100px;">
                        库管员：                     
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStoreKeeper" runat="server" Width="100%" 
                            DataSourceID="edsStoreKeeper" DataTextField="Name" 
                            DataValueField="Name" AppendDataBoundItems="True" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
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
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
                            </WhereParameters>
                        </asp:EntityDataSource>
                    </td>
                    <td style="text-align: right; width:100px;">
                        发货仓库：                     
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWHCode" runat="server" Width="187px" AppendDataBoundItems="True"
                          DataSourceID="edsWHCode" DataTextField="WHName"  DataValueField="WHCode">
                            <asp:ListItem>不限</asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsWHCode" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="WareHouse" >
                        </asp:EntityDataSource>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                           OnClientClick="showWaitDiv('divWait');" Text="查 询" onclick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="box">
        <h2>
            <span>订单列表</span>
            <asp:LinkButton ID="lbtStockCancel" runat="server" Text="单据撤销" 
                OnClientClick="javascript:return confirm('确定对所选单据进行撤销(冲销)吗？\n(每次只能选择一个单据进行撤销)');" 
                onclick="lbtStockCancel_Click" />
            <asp:LinkButton ID="lbtnSummit" runat="server" Text="多单提交至备货" OnClick="lbtnSummit_Click" />            
            <asp:LinkButton ID="lbtnNewOut" runat="server"  Text="新建出库单" onclick="lbtnNewOut_Click" /> 
            <asp:LinkButton ID="btnNCDataImport" runat="server" Text="NC数据导入" OnClick="btnNCDataImport_Click" />           
            <asp:LinkButton ID="lbtnImport" runat="server" Text="导入其他出库单..." />
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
            <asp:GridView ID="grdOutInfo" runat="server" AutoGenerateColumns="False" Width="2400px"
                DataKeyNames="SOCode" DataSourceID="EntityDataSource1" CssClass="linetable"
                AllowPaging="false" ShowHeader="false" sortExpression="SOCode" sortDirection="ASC" 
                OnSorting="grdOutInfo_Sorting" AllowSorting="True"  
                OnPageIndexChanging="grdOutInfo_PageIndexChanging"  onrowcommand="grdOutInfo_RowCommand"
                OnDataBound="grdOutInfo_DataBound"  onrowdatabound="grdOutInfo_RowDataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table">
                        <%--<tr class="GridViewEmpty_RowHeader">
                            <td>出库单号</td>
                            <td>出库日期</td>
                            <td>出库单状态</td>
                            <td>出库类型</td>
                            <td>来源类型</td>
                            <td>源用友订单号</td>
                            <td>收发类别</td>
                            <td>销售部门</td>
                            <td>业务员</td>
                            <td>客户名称</td>
                            <td>仓库名称</td>
                            <td>库管员</td>
                            
                            <td>仓库签名</td>
                            <td>业务类型</td>
                                                        
                            
                            <td>制单人</td>
                            <td>总数量</td>
                            <td>总价</td>
                            <td>最后修改时间</td>
                            <td>RFID处理人工号</td>
                            <td>RFID处理人姓名</td>
                            <td>RFID处理时间</td>
                           
                            <td>备注</td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="30">无数据</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="选择" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%--                            <asp:RadioButton ID="radSelectOut" runat="server" Text="" Visible ='<%# GetButtonVisible(Eval("SOStatus"),Eval("SOType"))%>'/>  --%>
                            <asp:CheckBox ID="chkSelect" runat="server" Text="" />
                        </ItemTemplate>
                    </asp:TemplateField>                   
                    <asp:TemplateField HeaderText="出库单号" SortExpression="SOCode" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnSOCode" runat="server" Text='<%# Bind("SOCode") %>' ForeColor='<%# GetForeColor(Eval("SOCode"))%>' OnClick="lbtnSOCode_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>                  
                    <asp:TemplateField HeaderText="出库日期" SortExpression="SODate" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate><asp:Label ID="lblSODate" runat="server"  Text='<%# Bind("SODate","{0:yyyy-MM-dd}") %>'></asp:Label></ItemTemplate>                    
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="出库单状态" SortExpression="SOStatus" ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetSOStatus(Eval("SOStatus"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="出库类型" SortExpression="SOType"  ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetSOType(Eval("SOType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="来源类型" SortExpression="FromType" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <%# GetFromType(Eval("FromType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="用友参考单号" SortExpression="FromUCOrderNo" ItemStyle-Width="130px"
                         ItemStyle-HorizontalAlign="Right">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' Visible='<%# GetTextBoxVisible(Eval("FromType"))==false?true:false%> ' MaxLength="50" Width="80px"></asp:TextBox> 
                            <asp:TextBox ID="tbxFromBillNo" runat="server" Text='<%# Bind("FromBillNo") %>' Visible='<%# GetTextBoxVisible(Eval("FromType"))%>'  MaxLength="50" Width="80px"></asp:TextBox>   
                            <asp:LinkButton ID="btnUpdate" Text="更新" CommandName="MyDefineUpdate" runat="server" OnClientClick="showWaitDiv('divWait');"
                                ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取消" CommandName="Cancel" CausesValidation="false" OnClientClick="showWaitDiv('divWait');"
                                runat="server" />                       
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblFromBillNo" runat="server"  Text='<%# Bind("FromBillNo") %>'></asp:Label>
                            <asp:Label ID="lblFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>'></asp:Label>
                            <asp:LinkButton ID="btnEdit" Text="编辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup" OnClientClick="showWaitDiv('divWait');"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="收发类别" SortExpression="OutCategory" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <%# GetOutCategory(Eval("OutCategory"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <%--<asp:BoundField DataField="FromOrderNo" HeaderText="源RFID订单号" ItemStyle-VerticalAlign="Top"
                        SortExpression="FromOrderNo" />--%>
                    <asp:TemplateField HeaderText="销售部门" SortExpression="SellDepartmentName" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblSellDepartmentName" runat="server"  Text='<%# Bind("SellDepartmentName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="业务员" SortExpression="BussinessMan" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblBussinessMan" runat="server"  Text='<%# Bind("BussinessMan") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="客户名称" SortExpression="CustomerName" >
                        <ItemTemplate>
                            <asp:Label ID="lblCustomerName" runat="server"  Text='<%# Bind("CustomerName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                    
                    <asp:TemplateField HeaderText="仓库名称" SortExpression="WHName" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="WHName" runat="server"  Text='<%# Bind("WHName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="库管员" SortExpression="StorageMan" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblStorageMan" runat="server"  Text='<%# Bind("StorageMan") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="仓库签名" SortExpression="WarehouseSign" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="WarehouseSign" runat="server"  Text='<%# Bind("WarehouseSign") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:BoundField DataField="WHCode" HeaderText="仓库编码" ItemStyle-VerticalAlign="Top"
                        SortExpression="WHCode" />--%>
                    <asp:TemplateField HeaderText="业务类型" SortExpression="BusinessType" ItemStyle-Width="80px">
                        <ItemTemplate>                           
                            <%# GetOutBusinessType(Eval("BusinessType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="领导" SortExpression="LeaderSign" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="LeaderSign" runat="server"  Text='<%# Bind("LeaderSign") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>                    
                    <%--<asp:TemplateField HeaderText="财务" SortExpression="AccountSign" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="AccountSign" runat="server"  Text='<%# Bind("AccountSign") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="经办人" SortExpression="BusinessSign" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="BusinessSign" runat="server"  Text='<%# Bind("BusinessSign") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="制单人" SortExpression="EditorSign" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="EditorSign" runat="server"  Text='<%# Bind("EditorSign") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="总数量" SortExpression="TotalNumber" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="TotalNumber" runat="server"  Text='<%# Bind("TotalNumber") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="总价" SortExpression="TotalMoney" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="TotalMoney" runat="server"  Text='<%# Bind("TotalMoney") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="最后修改时间" SortExpression="LastModifyTime" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="LastModifyTime" runat="server"  Text='<%# Bind("LastModifyTime") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="RFID处理人工号" SortExpression="RFIDActorID" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="RFIDActorID" runat="server"  Text='<%# Bind("RFIDActorID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="RFID处理人姓名" SortExpression="RFIDActorName" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="RFIDActorName" runat="server"  Text='<%# Bind("RFIDActorName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="RFID处理时间" SortExpression="RFIDActorTime" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="RFIDActorTime" runat="server"  Text='<%# Bind("RFIDActorTime") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="源RFID订单类型" SortExpression="FromOrderType" ItemStyle-Width="110px">
                        <ItemTemplate>
                            <%# GetFromOrderType(Eval("FromOrderType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="源RFID订单号" SortExpression="FromOrderNo" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="FromOrderNo" runat="server"  Text='<%# Bind("FromOrderNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="源Excel出库单号" SortExpression="FromBillNo" ItemStyle-Width="110px">
                        <ItemTemplate>
                            <asp:Label ID="lblFromBillNo" runat="server"  Text='<%# Bind("FromBillNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                   
                   <asp:TemplateField HeaderText="源Excel出库单类型" SortExpression="FromBillType" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <%# GetSOType(Eval("FromBillType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>--%>                    
                    <asp:TemplateField HeaderText="备注" SortExpression="Comment" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label ID="lblComment" runat="server"  Text='<%# Bind("Comment") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: left">
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
            <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableFlattening="False" 
                EntitySetName="VSelectAllOutCancleBillForOutMgr" onquerycreated="EntityDataSource1_QueryCreated" >
            </asp:EntityDataSource>
        </div>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
       
    </div>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopup">
        <div class="modalPopupWrapper">
            <div id="pnlDragTitle" class="modalHeader">
                <span>其他出库单文件导入</span>
                <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="OK" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <div id="silverlightControlHost" class="uploadControl">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="../ClientBin/SLFileUpload.xap" />
                        <param name="initParams" value="UploadPage=../Upload/FileUpload.ashx,FileType=其他出库单" />
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

