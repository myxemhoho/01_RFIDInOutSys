<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StockCountingPlanList.aspx.cs" Inherits="Gold.Query.StockCountingPlanList" %>

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
                    <td class="tdRight" width="100px">
                        <label class="commonLabel">
                            盘点计划单号:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxSCPCode" runat="server"></asp:TextBox>
                    </td>                     
                    <td class="tdRight">
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="180px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight"><label class="commonLabel">
                            盘点计划单类型:</label></td>
                    <td><asp:DropDownList ID="DropDownList_Type" runat="server" Width="110px">
                        </asp:DropDownList></td>
                    <td></td>         
                </tr>
                <tr>              
                    <td class="tdRight">
                        <label class="commonLabel">
                            盘点计划单状态:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_SCPStatus" runat="server" Width="150px">
                        </asp:DropDownList>
                    </td>                    
                    <td class="tdRight">
                        <label class="commonLabel">
                            查询日期:</label>
                    </td>
                    <td colspan="3">
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
                    <td align="right" >
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                           OnClientClick="showWaitDiv('divWait');" OnClick="btnQuery_Click" Text="查询" />
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
                    Text="新增盘点计划单" PostBackUrl="~/Query/StockCountingPlanEdit.aspx?EditType=1&EditID=0" />
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
                                <!--[SCPCode]
      ,[SCPType]
      ,[SCPStatus]
      ,[WHCode]
      ,[WHName]
      ,[CreatorID]
      ,[CreatorName]
      ,[CreateDate]
      ,[FinishPersonID]
      ,[FinishPersonName]
      ,[FinishDate]
      ,[Comment]
      ,[Reserve1]
      ,[Reserve2]-->

            <asp:GridView ID="gv_CargoList" Width="1140px" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="15" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList_PageIndexChanging" OnSorting="gv_CargoList_Sorting"
                sortExpression="SCPCode" sortDirection="DESC" 
                OnDataBound="gv_CargoList_DataBound" 
                onrowdatabound="gv_CargoList_RowDataBound" 
                onrowcommand="gv_CargoList_RowCommand">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table" style="width:100%;">
                        <%--<tr class="GridViewEmpty_RowHeader">
                            <td>盘点计划单号</td>
                            <td>类型</td>
                            <td>状态</td>
                            <td>仓库</td>
                            <td>盘点层位</td>
                            <td>创建人</td>
                            <td>创建时间</td>
                            <td>结束人</td>
                            <td>结束时间</td>
                            <td>备注</td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="10" >
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
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="盘点计划单号" SortExpression="SCPCode">
                        <ItemStyle Width="110px" />
                        <ItemTemplate>
                            <asp:Label ID="lblSCPCode" runat="server" Text='<%# Eval("SCPCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="类型" SortExpression="SCPCode">
                        <ItemStyle Width="40px" />
                        <ItemTemplate>
                            <asp:Label ID="lblSCPType" runat="server" Text='<%# Eval("SCPType").ToString()=="0"?"粗盘":"细盘" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="状态" SortExpression="SCPStatus">
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:Label ID="lblSCPStatus" runat="server" Text='<%# GetSCPStatusName(Eval("SCPStatus").ToString()) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHName" HeaderText="仓库" SortExpression="WHName"
                        ItemStyle-Width="100px" /> 
                        <asp:BoundField DataField="Reserve1" HeaderText="盘点层位" SortExpression="Reserve1"
                        ItemStyle-Width="280px" /> 
                     <asp:BoundField DataField="CreatorName" HeaderText="创建人" SortExpression="CreatorName"
                        ItemStyle-Width="50px" />                   
                    <asp:BoundField DataField="CreateDate" HeaderText="创建时间" SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
                        ItemStyle-Width="100px" />
                    <asp:BoundField DataField="FinishPersonName" HeaderText="结束人" SortExpression="FinishPersonName"
                        ItemStyle-Width="50px" />
                    <asp:BoundField DataField="FinishDate" HeaderText="结束时间" SortExpression="FinishDate" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
                        ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" SortExpression="Comment"
                        ItemStyle-Width="100px" />
                    <asp:TemplateField HeaderText="查看/结束盘点" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnViewDetail" Text="查看/结束盘点" CommandName="ViewDetail" runat="server"   ValidationGroup="RowEditGroup" />                            
                        </ItemTemplate>                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="编辑" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编辑" CommandName="MyEdit"  CausesValidation="false"  runat="server" />
                        </ItemTemplate>                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="删除" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDel" Text="删除" CommandName="MyDelete" CausesValidation="false"  runat="server" OnClientClick="javascript:return confirm('确定要删除吗？');" />
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
            </div>                    
        
        <br />
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
    </div>
</asp:Content>
