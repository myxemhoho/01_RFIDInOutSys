<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StockInMgr.aspx.cs" Inherits="Gold.StockMove.StockInMgr" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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

        function SetRadio(nowRadio) {
            var myForm, objRadio;
            myForm = document.forms[0];
            for (var i = 0; i < myForm.length; i++) {
                if (myForm.elements[i].type == "radio") {
                    objRadio = myForm.elements[i];
                    if (objRadio != nowRadio && objRadio.name.indexOf("GridView1") > -1 && objRadio.name.indexOf("radSelect") > -1) {
                        if (objRadio.checked) {
                            objRadio.checked = false;
                        }
                    }
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <h2>
            <span>查询条件</span></h2>
        <div class="boxContent">
            <table>
                <tr>
                    <td style="text-align: right; width:70px;" >
                        入库单号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtSICode" runat="server" Width="90px" />
                    </td>
                    <td style="text-align: right; width:100px;">
                        订单编号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtOrderCode" runat="server" MaxLength="32" Width="90px" />
                    </td>
                    <td style="width: 70px; text-align: right; width:100px;">
                        单据状态：
                    </td>
                    <td style="width: 100px">
                        <asp:DropDownList ID="ddlSIStatus" runat="server"  Width="100" AppendDataBoundItems="True"
                            DataSourceID="edsSIStatus" DataTextField="Name" 
                            DataValueField="Code" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsSIStatus" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="SIStatus" />
                            </WhereParameters>
                        </asp:EntityDataSource>
                    </td> 
                    <td style="width: 70px; text-align: right; width:100px;">
                        入库时间：
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
                        <asp:DropDownList ID="ddlSIType" runat="server"  Width="100" AppendDataBoundItems="True"
                            DataSourceID="edsSIType" DataTextField="Name"
                            DataValueField="Code" >
                            <asp:ListItem Text="不限" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:EntityDataSource ID="edsSIType" runat="server" 
                            ConnectionString="name=GoldEntities" 
                            DefaultContainerName="GoldEntities" EnableFlattening="False" 
                            EntitySetName="DataDict" Where="it.Category=@Category">
                            <WhereParameters>
                                <asp:Parameter Type="String" Name="Category" DefaultValue="SIType" />
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
                        收货仓库：
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
            <asp:LinkButton ID="lbtnNewIn" runat="server"  Text="新建入库单" onclick="lbtnNewIn_Click" />  
            <asp:LinkButton ID="btnNCDataImport" runat="server" Text="NC数据导入" OnClick="btnNCDataImport_Click" />  
            <asp:LinkButton ID="lbtnImport" runat="server" Text="导入其他入库单..." />    
            <asp:LinkButton ID="lbtnTransferImport" runat="server" Text="导入调拨入库单..." />
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
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="2100px"
                DataKeyNames="SICode" DataSourceID="EntityDataSource1"  
                AllowPaging="false" ShowHeader="false" CssClass="linetable" sortExpression="SIDate" sortDirection="DESC"                 
                OnSorting="GridView1_Sorting" AllowSorting="True" 
                OnPageIndexChanging="GridView1_PageIndexChanging" 
                OnDataBound="GridView1_DataBound" onrowdatabound="GridView1_RowDataBound" 
                onrowcommand="GridView1_RowCommand">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />                
                <EmptyDataTemplate>
                <table class="GridViewEmpty_Table">
                    <%--<tr class="GridViewEmpty_RowHeader">
                        <td>入库单号</td>
                        <td>入库日期</td>
                        <td>入库单状态</td>
                        <td>入库类型</td>
                        <td>来源类行</td> 
                        <td>用友参考单号</td>
                        <td>收发类别</td>                       
                        <td>仓库编码</td>
                        <td>仓库名称</td>
                        <td>供应单位</td>
                       
                        <td>业务员</td>
                        <td>库管员</td>
                       
                        <td>合计数量</td>
                        <td>最后修改时间</td>
                        <td>RFID处理人工号</td>
                        <td>RFID处理人姓名</td>
                        <td>RFID处理时间</td>
                       
                        <td>备注</td>             
                        </tr>--%>
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="24">
                            无数据
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="选择" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                           <%-- <asp:RadioButton ID="radSelect" runat="server" Text="" Visible ='<%# GetButtonVisible(Eval("SIStatus"),Eval("SIType"))%>'/>                   --%>
                           <asp:CheckBox ID="chkSelect" runat="server" Text="" />
                        </ItemTemplate>                        
                    </asp:TemplateField>                     
                    <asp:TemplateField HeaderText="入库单号" SortExpression="SICode" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">                       
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnSICode" runat="server" Text='<%# Bind("SICode") %>' ForeColor='<%# GetForeColor(Eval("SICode"))%>' OnClick="lbtnSICode_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:TemplateField HeaderText="入库日期" SortExpression="SIDate" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblSIDate" runat="server" Text='<%# Bind("SIDate", "{0:yyyy-MM-dd}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                  
                                      
                    <asp:TemplateField HeaderText="入库单状态" SortExpression="SIStatus" ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetSIStatus(Eval("SIStatus"))%>                            
                        </ItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="入库类型" SortExpression="SIType" ItemStyle-Width="70px">
                        <ItemTemplate>
                            <%# GetSIType(Eval("SIType"))%>                            
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
                    <asp:TemplateField HeaderText="收发类别" SortExpression="InCategory" ItemStyle-Width="110px">
                        <ItemTemplate>
                            <%# GetInCategory(Eval("InCategory"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="仓库编码" SortExpression="WHCode" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblWHCode" runat="server"  Text='<%# Bind("WHCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="仓库名称" SortExpression="WHName" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <asp:Label ID="lblWHName" runat="server"  Text='<%# Bind("WHName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="供应单位" SortExpression="Supplier" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label ID="lblSupplier" runat="server"  Text='<%# Bind("Supplier") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="业务部门" SortExpression="BusinessDepartmentName" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblBusinessDepartmentName" runat="server"  Text='<%# Bind("BusinessDepartmentName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="业务员" SortExpression="Operator" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblOperator" runat="server"  Text='<%# Bind("Operator") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="库管员" SortExpression="StoreKeeper" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblStoreKeeper" runat="server"  Text='<%# Bind("StoreKeeper") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="验收员" SortExpression="Checker" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lbChecker" runat="server"  Text='<%# Bind("Checker") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="合计数量" SortExpression="TotalNumber" ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalNumber" runat="server"  Text='<%# Bind("TotalNumber") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="最后修改时间" SortExpression="LastModifyTime" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblLastModifyTime" runat="server"  Text='<%# Bind("LastModifyTime") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                   
                    <asp:TemplateField HeaderText="RFID处理人工号" SortExpression="RFIDActorID" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblRFIDActorID" runat="server"  Text='<%# Bind("RFIDActorID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="RFID处理人姓名" SortExpression="RFIDActorName" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblRFIDActorName" runat="server"  Text='<%# Bind("RFIDActorName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="RFID处理时间" SortExpression="RFIDActorTime" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="lblRFIDActorTime" runat="server"  Text='<%# Bind("RFIDActorTime") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="源RFID订单类型" SortExpression="FromOrderType" ItemStyle-Width="110px">
                        <ItemTemplate>
                            <%# GetFromOrderType(Eval("FromOrderType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="源RFID订单号" SortExpression="FromOrderNo" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblFromOrderNo" runat="server"  Text='<%# Bind("FromOrderNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="源Excel入库单号" SortExpression="FromBillNo" ItemStyle-Width="110px">
                        <ItemTemplate>
                            <asp:Label ID="lblFromBillNo" runat="server"  Text='<%# Bind("FromBillNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                   
                   <asp:TemplateField HeaderText="源Excel入库单类型" SortExpression="FromBillType" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <%# GetSIType(Eval("FromBillType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>                  --%>  
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
            </asp:GridView>
            <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableFlattening="False" 
                EntitySetName="VSelectAllInCancleBillForInMgr" 
                onquerycreated="EntityDataSource1_QueryCreated" >
            </asp:EntityDataSource>
        </div>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>       
    </div>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopup">
        <div class="modalPopupWrapper">
            <div id="pnlDragTitle" class="modalHeader">
                <span>其他入库单文件导入</span>
                <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="OK" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <div id="silverlightControlHost" class="uploadControl">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="../ClientBin/SLFileUpload.xap" />
                        <param name="initParams" value="UploadPage=../Upload/FileUpload.ashx,FileType=其他入库单" />
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

    <%--调拨入库单--%>
    <asp:Panel ID="pnlPopWindowForTransfer" runat="server" Style="display: none" CssClass="modalPopup">
        <div class="modalPopupWrapper">
            <div id="Div1" class="modalHeader">
                <span>调拨入库单导入</span>
                <asp:Button ID="Button1" runat="server" CssClass="ClosePopButton" Text="OK" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <div id="Div2" class="uploadControl">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="../ClientBin/SLFileUpload.xap" />
                        <param name="initParams" value="UploadPage=../Upload/FileUpload.ashx,FileType=调拨入库单" />
                        <param name="onError" value="onSilverlightError" />
                        <param name="background" value="white" />
                        <param name="minRuntimeVersion" value="5.0.61118.0" />
                        <param name="autoUpgrade" value="true" />
                        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration: none">
                            <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="获取 Microsoft Silverlight"
                                style="border-style: none" />
                        </a>
                    </object>
                    <iframe id="_sl_historyFrameForTransfer" style="visibility: hidden; height: 0px; width: 0px;
                        border: 0px"></iframe>
                </div>
            </div>
         </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="popWindowForTransfer" runat="server" TargetControlID="lbtnTransferImport"
        PopupControlID="pnlPopWindowForTransfer" BackgroundCssClass="modalBackground" DropShadow="true"
        PopupDragHandleControlID="pnlDragTitle">
    </ajaxToolkit:ModalPopupExtender>
</asp:Content>
