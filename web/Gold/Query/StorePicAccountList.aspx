<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StorePicAccountList.aspx.cs" Inherits="Gold.Query.StorePicAccountList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                查询条件</label>
        </div>
        <div class="commonQuery">
            <table width="95%">
                <tr>
                    <td>
                        <label class="commonLabel">
                            商品编码:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品名称:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoName" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            共用箱号:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxPackageShareNo" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            查询日期:</label>
                    </td>
                    <td >
                        <asp:TextBox ID="txtStartTime" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" TargetControlID="txtStartTime">
                        </ajaxToolkit:CalendarExtender>
                        <label class="commonLabel">
                            至:</label>
                        <asp:TextBox ID="txtEndTime" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Enabled="True"
                            TargetControlID="txtEndTime">
                        </ajaxToolkit:CalendarExtender>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            存/提:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_StorePickType" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            盈亏状态:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_IsProfitOrLoss" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            包装:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_PackageName" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            查看方式:</label>
                    </td>
                    <td >
                        <asp:RadioButton ID="RadioButton_ByCargo" Text="按每天存提流水查看" GroupName="GroupRadio" 
                            runat="server" AutoPostBack="True"  OnClick="showWaitDiv('divWait');"
                            oncheckedchanged="RadioButton_ByCargo_CheckedChanged" />
                        <br />
                        <asp:RadioButton ID="RadioButton_ByWHCode" Text="按月份存提汇总查看" GroupName="GroupRadio" 
                            runat="server" AutoPostBack="True"  OnClick="showWaitDiv('divWait');"
                            oncheckedchanged="RadioButton_ByWHCode_CheckedChanged" />
                    </td>
                    <td>
                    </td>
                    <td align="right" >
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            OnClick="btnQuery_Click" Text="查询" OnClientClick="showWaitDiv('divWait');"/>
                    </td>
                </tr>
            </table>
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
                <asp:Label ID="lblDisplayPattern" Text="" runat="server" CssClass="commonSaveMsgLabel"></asp:Label>
            </div>
            <div class="CommonTitle_InnerdivRight">
            <asp:Button ID="btnShowAddPage" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    Text="新增存提记录" PostBackUrl="~/Query/StorePicAccountEdit.aspx?EditType=1&EditID=0" />
           <asp:Button ID="btnShowEditPage" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    Text="编辑存提记录" OnClick="btnShowEditPage_Click" />
                <asp:Button ID="btnGoToPrintAndExportPage" runat="server" 
                    CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="打印/导出" onclick="btnGoToPrintAndExportPage_Click" />
            </div>
        </div>
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
                <img src="../Styles/images/close.png" class="divWait_Close" onclick="hiddenWaitDiv('divWait');" /></div>
            <div>
                <img src="../Styles/images/uploading.gif" /><label>执行中,请稍候……</label>
                <br />
            </div>
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
            <asp:GridView ID="gv_CargoList" Width="2020px" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="15" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList_PageIndexChanging" OnSorting="gv_CargoList_Sorting"
                sortExpression="StockPickAccountID" sortDirection="DESC" 
                OnDataBound="gv_CargoList_DataBound" onrowdatabound="gv_CargoList_RowDataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table">
                        <%--<tr class="GridViewEmpty_RowHeader">
                            <td>序号</td>
                            <td>存提登记时间</td>
                            <td>存/提</td>
                            <td>仓库编码</td>
                            <td>仓库名称</td>
                            <td>商品编码</td>
                            <td>商品名称</td>
                            <td>存提数量</td>
                            <td>账面数量</td>
                            <td>盘点实物数量</td>
                            <td>盘盈/盘亏</td>
                            <td>包装</td>
                            <td>件数</td>
                            <td>枚/箱</td>
                            <td>箱号起</td>
                            <td>箱号至</td>
                            <td>存放明细</td>
                            <td>备注</td>
                            <td>箱锁号</td>
                            <td>共用箱号</td>
                            <td>收发情况</td>
                            <td>不良品率</td>
                            <td>收发月份</td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="23">
                                无数据
                            </td>
                        </tr>
                        <!--
                        [StockPickAccountID]
      ,[RecordTime]
      ,[StorePickType]
      ,[StorePickNumber]
      ,[WHCode]
      ,[CargoCode]
      ,[CargoName]
      ,[CargoModel]
      ,[CargoSpec]
      ,[CargoUnits]
      ,[ReleaseYear]
      ,[AccountNumber]
      ,[FactCheckNumber]
      ,[IsProfitOrLoss]
      ,[PackageId]
      ,[PackageName]
      ,[PackageCount]
      ,[StandardCountEachBag]
      ,[PackageNoStart]
      ,[PackageNoEnd]
      ,[StoreDescription]
      ,[Remark]
      ,[PackageLockNo]
      ,[PackageShareNo]
      ,[RecordDetail]
      ,[RecordMonth]
      ,[BadRate]
                        -->
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="30px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <%--<input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选--%>
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="40px"/>
                        <HeaderStyle HorizontalAlign="Center"  />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="RecordTime" HeaderText="存提登记时间" SortExpression="RecordTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="160px" />
                    <asp:TemplateField HeaderText="存/提" SortExpression="StorePickType" ItemStyle-Width="50px">
                        <ItemTemplate>
                            <asp:Label ID="lblStorePickType" runat="server" Text='<%# Eval("StorePickType").ToString()=="1"?"提货":"存货" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="150px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHCode" HeaderText="仓库编码" SortExpression="WHCode"
                        ItemStyle-Width="80px" />
                        <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHCode"
                        ItemStyle-Width="120px" />
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="280px" />
                    <asp:BoundField DataField="StorePickNumber" HeaderText="存提数量" SortExpression="StorePickNumber" ItemStyle-HorizontalAlign="Right" 
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="AccountNumber" HeaderText="账面数量" SortExpression="AccountNumber" ItemStyle-HorizontalAlign="Right" 
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="FactCheckNumber" HeaderText="盘点实物数量" SortExpression="FactCheckNumber" ItemStyle-HorizontalAlign="Right" 
                        ItemStyle-Width="60px" />
                    <asp:BoundField DataField="IsProfitOrLoss" HeaderText="+盘盈/-盘亏" SortExpression="IsProfitOrLoss" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageName" HeaderText="包装" SortExpression="PackageName" ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageCount" HeaderText="件数" SortExpression="PackageCount" ItemStyle-Width="40px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="StandardCountEachBag" HeaderText="枚/箱" SortExpression="StandardCountEachBag"
                        ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageNoStart" HeaderText="箱号(起)" SortExpression="PackageNoStart" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageNoEnd" HeaderText="箱号(至)" SortExpression="PackageNoEnd" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="StoreDescription" HeaderText="存放明细" SortExpression="StoreDescription" ItemStyle-Width="120px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Remark" HeaderText="备注" SortExpression="Remark" ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageLockNo" HeaderText="箱锁号#" SortExpression="PackageLockNo" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageShareNo" HeaderText="共用箱号" SortExpression="PackageShareNo" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="RecordDetail" HeaderText="收发情况" SortExpression="RecordDetail" ItemStyle-Width="150px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="RecordMonth" HeaderText="收发月份" SortExpression="RecordMonth" ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="BadRate" HeaderText="不良品率" SortExpression="BadRate" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="StockPickAccountID" HeaderText="流水号" SortExpression="StockPickAccountID" ItemStyle-Width="60px"/>
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
            <asp:GridView ID="gv_CargoList2" Width="1900px" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="15" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList2_PageIndexChanging" OnSorting="gv_CargoList2_Sorting"
                sortExpression="RecordMonth" sortDirection="DESC" OnDataBound="gv_CargoList2_DataBound"
                OnRowDataBound="gv_CargoList2_RowDataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table">
                        <%--<tr class="GridViewEmpty_RowHeader">
                            <td>序号</td>
                            <td>收发月份</td>
                            <td>存/提</td>
                            <td>仓库编码</td>
                            <td>仓库名称</td>
                            <td>商品编码</td>
                            <td>商品名称</td>
                            <td>存提数量</td>
                            <td>账面数量</td>
                            <td>盘点实物数量</td>
                            <td>盘盈/盘亏</td>
                            <td>包装</td>
                            <td>件数</td>
                            <td>枚/箱</td>
                            <td>箱号起</td>
                            <td>箱号至</td>
                            <td>存放明细</td>
                            <td>备注</td>
                            <td>箱锁号</td>
                            <td>共用箱号</td>
                            <td>收发情况</td>
                            <td>不良品率</td>
                            <td>存提登记时间</td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="23">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <%--<asp:TemplateField>
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="40px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="RecordMonth" HeaderText="收发月份" SortExpression="RecordMonth" ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:TemplateField HeaderText="存/提" SortExpression="StorePickType" ItemStyle-Width="50px">
                        <ItemTemplate>
                            <asp:Label ID="lblStorePickType" runat="server" Text='<%# Eval("StorePickType").ToString()=="1"?"提货":"存货" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHCode" HeaderText="仓库编码" SortExpression="WHCode"
                        ItemStyle-Width="80px" />
                        <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHCode"
                        ItemStyle-Width="120px" />                    
                    <asp:BoundField DataField="CargoCode" HeaderText="商品编码" SortExpression="CargoCode"
                        ItemStyle-Width="150px" />
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="280px" />
                    <asp:BoundField DataField="StorePickNumber" HeaderText="存提数量" SortExpression="StorePickNumber"
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="AccountNumber" HeaderText="账面数量" SortExpression="AccountNumber"
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="FactCheckNumber" HeaderText="盘点实物数量" SortExpression="FactCheckNumber"
                        ItemStyle-Width="60px" />
                    <asp:BoundField DataField="IsProfitOrLoss" HeaderText="+盘盈/-盘亏"  ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageName" HeaderText="包装" ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageCount" HeaderText="件数" ItemStyle-Width="40px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="StandardCountEachBag" HeaderText="枚/箱" 
                        ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageNoStart" HeaderText="箱号(起)" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageNoEnd" HeaderText="箱号(至)"  ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="StoreDescription" HeaderText="存放明细"  ItemStyle-Width="120px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Remark" HeaderText="备注"  ItemStyle-Width="80px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageLockNo" HeaderText="箱锁号#"  ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="PackageShareNo" HeaderText="共用箱号"  ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="RecordDetail" HeaderText="收发情况"  ItemStyle-Width="150px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="BadRate" HeaderText="不良品率"  ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="RecordTime" HeaderText="存提登记时间"  ItemStyle-Width="150px"  DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"/>
                        <asp:BoundField DataField="StockPickAccountID" HeaderText="StockPickAccountID"  Visible="false" />
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: left">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex2"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />
                                页
                                    共<asp:Label ID="lblPageCount2" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />
                                页
                                </span>
                                <asp:LinkButton ID="btnFirst2" runat="server" CausesValidation="False" CommandArgument="First"
                                    CommandName="Page" Text="首页" />
                                <asp:LinkButton ID="btnPrev2" runat="server" CausesValidation="False" CommandArgument="Prev"
                                    CommandName="Page" Text="上一页" />
                                <asp:LinkButton ID="btnNext2" runat="server" CausesValidation="False" CommandArgument="Next"
                                    CommandName="Page" Text="下一页" />
                                <asp:LinkButton ID="btnLast2" runat="server" CausesValidation="False" CommandArgument="Last"
                                    CommandName="Page" Text="尾页" />
                                <asp:TextBox ID="txtNewPageIndex2" runat="server" Width="20px" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />
                                <asp:LinkButton ID="btnGo2" runat="server" CausesValidation="False" CommandArgument="-1"
                                    CommandName="Page" Text="GO" />
                                <!-- here set the CommandArgument of the Go Button to '-1' as the flag -->
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
            </asp:GridView>
        </div>
        <br />
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
    </div>
</asp:Content>
