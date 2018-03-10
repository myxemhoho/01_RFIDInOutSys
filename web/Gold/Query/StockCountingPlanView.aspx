<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StockCountingPlanView.aspx.cs" Inherits="Gold.Query.StockCountingPlanView" %>

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
                盘点计划单基本信息</label>
        </div>
        <div class="commonQuery">
            <table width="95%">
                <tr>
                    <td>
                        <label class="commonLabel">
                            盘点计划单号:</label>
                        <asp:Label runat="server" ID="lblSCPCodeShow" Text=""></asp:Label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            仓库:</label>
                        <asp:Label runat="server" ID="lblWHName" Text=""></asp:Label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            盘点类型:</label>
                        <asp:Label runat="server" ID="lblType" Text=""></asp:Label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            盘点状态:</label>
                        <asp:Label runat="server" ID="lblStatusShow" Text=""></asp:Label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            创建日期:</label>
                        <asp:Label runat="server" ID="lblCreateTime" Text=""></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    盘点结果及明细项</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
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
                                      <!--
                                      ,[DetailRowNumber]
      ,[CargoCode]
      ,[CargoName]
      ,[CargoModel]
      ,[CargoSpec]
      ,[CargoUnits]
      ,[BinCode]
      ,[NumPlan]
      ,[NumActual]
      ,[PeriodInNum]
      ,[PeriodOutNum]
      ,[NumDifference]
      ,[CountingEndTime]
      ,[CountingStartTime]
      ,[Status]
      ,[ActorID]
      ,[ActorName]
                                -->
            <asp:GridView ID="gv_CargoList" Width="1330px" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="3" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList_PageIndexChanging" OnSorting="gv_CargoList_Sorting"
                sortExpression="SCPCode" sortDirection="ASC" OnDataBound="gv_CargoList_DataBound"
                OnRowDataBound="gv_CargoList_RowDataBound" OnRowCommand="gv_CargoList_RowCommand">
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
                            <td>
                                行号
                            </td>
                            <td>
                                商品名称
                            </td>
                            <td>
                                型号
                            </td>
                            <td>
                                规格
                            </td>
                            <td>
                                单位
                            </td>
                            <td>
                                发行年份
                            </td>
                            <td>
                                期初数量
                            </td>
                            <td>
                                收入数量
                            </td>
                            <td>
                                发出数量
                            </td>
                            <td>
                                结存数量
                            </td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="10">
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
                    <asp:TemplateField HeaderText="盘点计划单号" SortExpression="SCPCode" Visible="false">
                        <ItemStyle Width="110px" />
                        <ItemTemplate>
                            <asp:Label ID="lblSCPCode" runat="server" Text='<%# Eval("SCPCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="行号" SortExpression="DetailRowNumber"  ItemStyle-Width="40px">                        
                        <ItemTemplate>
                            <asp:Label ID="lblDetailRowNumber" runat="server" Text='<%# Eval("DetailRowNumber") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="状态" SortExpression="Status">
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# GetStatusName(Eval("Status").ToString()) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="160px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ReadOnly="true" ItemStyle-Width="300px">
                    </asp:BoundField>
                    <asp:BoundField DataField="CargoType" HeaderText="商品类别" Visible="false" SortExpression="CargoType"
                        ReadOnly="true" ItemStyle-Width="60px">
                    </asp:BoundField>
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ReadOnly="true" ItemStyle-Width="80px">
                    </asp:BoundField>
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ReadOnly="true" ItemStyle-Width="100px">
                    </asp:BoundField>
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ReadOnly="true" ItemStyle-Width="40px">
                    </asp:BoundField>
                    <asp:BoundField DataField="BinCode" HeaderText="层位" SortExpression="BinCode" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" />
                    <asp:TemplateField HeaderText="账面数量" SortExpression="NumPlan" ItemStyle-Width="50px"  ItemStyle-HorizontalAlign="Right">                        
                        <ItemTemplate>
                            <asp:Label ID="lblNumPlan" runat="server" Text='<%# Eval("NumPlan") %>' />
                        </ItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="盘点期间入库数量" SortExpression="PeriodInNum" ItemStyle-Width="70px"  ItemStyle-HorizontalAlign="Right">                        
                        <ItemTemplate>
                            <asp:Label ID="lblPeriodInNum" runat="server" Text='<%# Eval("PeriodInNum") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>                      
                    <asp:TemplateField HeaderText="盘点期间出库数量" SortExpression="PeriodOutNum"  ItemStyle-Width="70px"  ItemStyle-HorizontalAlign="Right">                        
                        <ItemTemplate>
                            <asp:Label ID="lblPeriodOutNum" runat="server" Text='<%# Eval("PeriodOutNum") %>' />
                        </ItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="实际盘点数量" SortExpression="NumActual" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right">                        
                        <ItemTemplate>
                            <asp:Label ID="lblNumActual" runat="server" Text='<%# Eval("NumActual") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:TemplateField HeaderText="差额" SortExpression="NumDifference"  ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right">                        
                        <ItemTemplate>
                            <asp:Label ID="lblNumDifference" runat="server" Text='<%# Eval("NumDifference") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>                     
                    <asp:BoundField DataField="ActorName" HeaderText="盘点人" SortExpression="ActorName"
                        ItemStyle-Width="50px" />
                    <asp:BoundField DataField="CountingEndTime" HeaderText="结束时间" SortExpression="CountingEndTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
                        ItemStyle-Width="90px" />
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
        </div>
        <br />
    </div>
    <asp:Button ID="btnFinish" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
        Text="结束盘点任务" OnClick="btnFinish_Click" />
    <asp:Button ID="btnReturn" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
        Text="返回盘点列表页" PostBackUrl="~/Query/StockCountingPlanList.aspx" />
    <br />
    <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
</asp:Content>
