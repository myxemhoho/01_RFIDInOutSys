<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TransferOrderView.aspx.cs" Inherits="Gold.Order.TransferOrderView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
        <h2><asp:Label runat="server" ID="lblTitle" Text="" CssClass="boxTitle" /></h2>
        <div class="boxContent">
            <div class="orderHeader">
                <asp:FormView ID="fvTransferOrder" runat="server" DataKeyNames="OrderCode" ondatabound="fvTransferOrder_DataBound" >
                    <ItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 140px;" class="tdKey">用友NC系统订单号：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("NCOrderCode") %>' />                                   
                                </td>
                                <td style="width: 130px;" class="tdKey">订单日期：</td>
                                <td style="width: 90px;">
                                    <asp:Label ID="OrderDateLabel" runat="server" Text='<%# Bind("OrderDate", "{0:yyyy-MM-dd}") %>'/>
                                    <%--<asp:Label ID="OrderDateLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"OrderDate","{0:yyyy-mm-dd}") %>'/>--%>
                                    <%--<asp:Label ID="OrderDateLabel" runat="server" Text='<%# TransTimeToShortDater(Eval("OrderDate")) %>' />--%>
                                </td>
                                <td style="width: 100px;" class="tdKey">订单状态：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="VersionLabel" runat="server" Text='<%# GetOrderStatus(Eval("OrderStatus"))%> ' />
                                </td>
                                <td style="width: 70px;" class="tdKey">供货单位：</td>
                                <td style="width: 200px;">
                                    <asp:Label ID="CompanyLabel" runat="server" Text='<%# Bind("Supplier") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 140px;" class="tdKey">调出公司：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="lblOutCompany" runat="server" Text='<%# Bind("OutCompany") %>' />
                                </td>
                                <td style="width: 130px;" class="tdKey">调出仓库：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="lblOutWHName" runat="server" Text='<%# Bind("OutWHName") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">调入公司：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="lblInCompany" runat="server" Text='<%# Bind("InCompany") %>' />
                                </td>                                
                                <td style="width: 70px;" class="tdKey">调入仓库：</td>
                                <td style="width:200px;">
                                    <asp:Label ID="lblInWHName" runat="server" Text='<%# Bind("InWHName") %>' />
                                </td>
                            </tr>
                            <tr>                                 
                                <td style="width: 140px;" class="tdKey">合计数量：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ReceiverLabel" runat="server" Text='<%# Bind("TotalNumber") %>' />
                                </td>
                                <td style="width: 130px;" class="tdKey">验收员：</td>
                                <td style="width: 90px;">
                                    <asp:Label ID="OrderStatusLabel" runat="server" Text='<%# Bind("Checker") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">最后修改时间：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="CurrencyTypeLabel" runat="server" Text='<%# Bind("LastModifyTime") %>' />
                                </td>
                                <td style="width: 70px;" class="tdKey">备注：</td>
                                <td style="width: 200px;">
                                    <asp:Label ID="PreparerLabel" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                                
                            </tr>
                            <tr>
                                <td style="width: 140px;" class="tdKey">RFID处理人：</td>
                                <td >
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("RFIDActorID") %>' />
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("RFIDActorName") %>' /> 
                                </td>
                                 <td style="width: 130px;" class="tdKey">RFID处理时间：</td>
                                <td colspan="5" style="width: 200px;">
                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("RFIDActorTime") %>' />
                                </td>                                                                                        
                            </tr>
                        </table>

                    </ItemTemplate>
                </asp:FormView>
            </div>
            <hr />
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
                <asp:GridView ID="grdTransferOrderDetail" runat="server" AutoGenerateColumns="False" Width="1730px"
                    DataKeyNames="OrderCode,DetailRowNumber" RowStyle-HorizontalAlign="Center"
                    CssClass="linetable" AllowPaging="false" ShowHeader="false" sortExpression="CargoName" sortDirection="ASC" 
                    OnSorting="grdTransferOrderDetail_Sorting" AllowSorting="True" 
                    OnPageIndexChanging="grdTransferOrderDetail_PageIndexChanging" 
                    OnDataBound="grdTransferOrderDetail_DataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                <table class="GridViewEmpty_Table">
                    <%--<tr class="GridViewEmpty_RowHeader">
                   <td>行号</td>
                        <td>用友NC系统订单号</td>
                        <td>商品编码</td>
                        <td>商品名称</td>
                        <td>型号</td>
                        <td>规格</td>
                        <td>单位</td>
                        <td>发行年份</td>
                        <td>行状态</td>
                        <td>订单数量</td>
                        <td>应收数量</td>
                        <td>实收数量</td>
                        <td>调入仓库</td>
                        <td>调出仓库</td>
                        <td>备注</td>                       
                    </tr>--%>
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="14">
                            无数据
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
                    <Columns>                        
                        <asp:BoundField DataField="DetailRowNumber" HeaderText="行号" ReadOnly="True" ItemStyle-Width="50"/>
                        <asp:BoundField DataField="NCOrderCode" HeaderText="用友NC系统订单号" ItemStyle-Width="120"/>
                        <asp:BoundField DataField="CargoCode" HeaderText="商品编码" SortExpression="CargoCode"  ItemStyle-Width="160"/>
                        <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"  ItemStyle-Width="350"/>
                        <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"  ItemStyle-Width="80"/>
                        <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"  ItemStyle-Width="100"/>
                        <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"  ItemStyle-Width="40"/>
                        <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份"  ItemStyle-Width="70"/>
                        <asp:TemplateField HeaderText="行状态" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <%# GetOrderStatus(Eval("DetailRowStatus"))%>                            
                            </ItemTemplate>                        
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" ItemStyle-Width="80px"/>
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="应收数量" ItemStyle-Width="80px"/>
                        <asp:BoundField DataField="NumActual" HeaderText="实收数量" ItemStyle-Width="80px"/>
                        <asp:BoundField DataField="InWHName" HeaderText="调入仓库" ItemStyle-Width="120px"/>
                        <asp:BoundField DataField="OutWHName" HeaderText="调出仓库" ItemStyle-Width="120px"/>
                        <asp:BoundField DataField="Comment" HeaderText="备注" ItemStyle-Width="200px"/>                                              
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
            </div>
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
            <hr />
            <div class="orderFooter">
                <asp:Button ID="btnGenStockIn" runat="server" CausesValidation="false" 
                    CssClass="ButtonImageStyle" Text=" 生成入库单 " onclick="btnGenStockIn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:Button ID="btnGenStockOut" runat="server" CausesValidation="false" 
                 CssClass="ButtonImageStyle" Text="生成出库单" onclick="btnGenStockOut_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReturn" runat="server" CausesValidation="false" 
                    Text="返回订单列表" CssClass="ButtonMidWidthImageStyle" onclick="btnReturn_Click" />

                <%--设置编辑提示框--%>               
                <asp:Button runat="server" ID="hiddenTargetControlForModalPopup" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender runat="server" ID="programmaticModalPopup" BehaviorID="programmaticModalPopupBehavior"
                    TargetControlID="hiddenTargetControlForModalPopup" PopupControlID="programmaticPopup"
                    BackgroundCssClass="modalBackground" DropShadow="True" PopupDragHandleControlID="programmaticPopupDragHandle"
                    RepositionMode="RepositionOnWindowScroll">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" Style="display: none;
                    width: 300px; height:120px;">
                    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move; background-color: #DDDDDD;
                        border: solid 1px Gray; color: Black; text-align: center;">
                        提示
                    </asp:Panel>
                    <br />
                    此订单
                    <asp:Label runat="server" ID="lblEditorID" Font-Bold="true" Text='<%# Eval("EditorID") %>'></asp:Label>
                    <asp:Label runat="server" ID="lblEditorName" Font-Bold="true" Text='<%# Eval("EditorName") %>'/>
                    正在编辑。<br/>是否继续生成<asp:Label runat="server" ID="lblInOrOut" />?<br/>
                    <asp:LinkButton runat="server" ID="hideModalPopupViaServer" Text="确定"
                        OnClick="hideModalPopupViaServer_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;                   
                    <asp:LinkButton runat="server" ID="hideClose" Text="取消" OnClick="hideClose_Click" />
                    <br />
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
