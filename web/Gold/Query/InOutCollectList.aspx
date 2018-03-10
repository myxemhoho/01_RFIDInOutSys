<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="InOutCollectList.aspx.cs" Inherits="Gold.Query.InOutCollectList" %>

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
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品编码:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server"></asp:TextBox>
                    </td>                    
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品规格:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="120px">
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
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品名称:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoName" runat="server"></asp:TextBox>
                    </td>                    
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品型号:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>                    
                    <td class="tdRight">
                    <label class="commonLabel">
                            汇总方式:</label>
                    </td>
                    <td colspan="4">
                        <asp:RadioButton ID="RadioButton_ByCargo" Text="按商品汇总" GroupName="GroupRadio" 
                            runat="server" AutoPostBack="True" OnClick="showWaitDiv('divWait');"
                            oncheckedchanged="RadioButton_ByCargo_CheckedChanged" />
                        <asp:RadioButton ID="RadioButton_ByWHCode" Text="按仓库汇总" GroupName="GroupRadio" 
                            runat="server" AutoPostBack="True" 
                            oncheckedchanged="RadioButton_ByWHCode_CheckedChanged" OnClick="showWaitDiv('divWait');"/>
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
                
                <asp:Button ID="btnGoToPrintAndExportPage" runat="server" 
                    CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="打印/导出" onclick="btnGoToPrintAndExportPage_Click" OnClientClick="showWaitDiv('divWait');"/>
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
            <asp:GridView ID="gv_CargoList" Width="1140px" runat="server" AllowPaging="false" ShowFooter="true" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="15" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList_PageIndexChanging" OnSorting="gv_CargoList_Sorting"
                sortExpression="CargoCode" sortDirection="ASC" 
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
                            <td>商品编码</td>
                            <td>商品名称</td>
                            <td>发行年份</td>
                            <td>型号</td>
                            <td>规格</td>
                            <td>单位</td>
                            <td>期初数量</td>
                            <td>收入数量</td>
                            <td>发出数量</td>
                            <td>结存数量</td>
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
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="170px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="380px" /> 
                     <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" SortExpression="ReleaseYear"
                        ItemStyle-Width="60px" />                   
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ItemStyle-Width="100px" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ItemStyle-Width="60px" />                    
                    <asp:BoundField DataField="FirstOrignialNum" HeaderText="期初数量" SortExpression="FirstOrignialNum" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumAdd" HeaderText="收入数量" SortExpression="SumNumAdd" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumDel" HeaderText="发出数量" SortExpression="SumNumDel" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumCurrent" HeaderText="结存数量" SortExpression="SumNumCurrent"
                        ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right" />
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
            
            <asp:GridView ID="gv_CargoList2" Width="1140px" runat="server" AllowPaging="false" ShowFooter="true" ShowHeader="false"
                AllowSorting="true" CssClass="linetable" PageSize="15" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList2_PageIndexChanging" OnSorting="gv_CargoList2_Sorting"
                sortExpression="CargoCode" sortDirection="ASC" OnDataBound="gv_CargoList2_DataBound"
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
                            <td>
                                商品编码
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
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="170px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode2" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="330px" />
                    <asp:BoundField DataField="WHName" HeaderText="仓库" SortExpression="WHName" ItemStyle-Width="100px" />
                    <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" SortExpression="ReleaseYear"
                        ItemStyle-Width="70px" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ItemStyle-Width="100px" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ItemStyle-Width="50px" />
                    <asp:BoundField DataField="FirstOrignialNum" HeaderText="期初数量" SortExpression="FirstOrignialNum"
                        ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumAdd" HeaderText="收入数量" SortExpression="SumNumAdd"
                        ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumDel" HeaderText="发出数量" SortExpression="SumNumDel"
                        ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="SumNumCurrent" HeaderText="结存数量" SortExpression="SumNumCurrent"
                        ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Right" />
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: left">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex2"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />页
                                    共<asp:Label ID="lblPageCount2" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />页
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

        <!--下面的代码是微软提供为解决UpdatePanel在IE中慢的代码
    Add the JavaScript below immediately before the closing </body> element of the page experiencing the delay.
    -->
    <script language="javascript" type="text/javascript">

        function disposeTree(sender, args) {
            var elements = args.get_panelsUpdating();
            for (var i = elements.length - 1; i >= 0; i--) {
                var element = elements[i];
                var allnodes = element.getElementsByTagName('*'),
                length = allnodes.length;
                var nodes = new Array(length)
                for (var k = 0; k < length; k++) {
                    nodes[k] = allnodes[k];
                }
                for (var j = 0, l = nodes.length; j < l; j++) {
                    var node = nodes[j];
                    if (node.nodeType === 1) {
                        if (node.dispose && typeof (node.dispose) === "function") {
                            node.dispose();
                        }
                        else if (node.control && typeof (node.control.dispose) === "function") {
                            node.control.dispose();
                        }

                        var behaviors = node._behaviors;
                        if (behaviors) {
                            behaviors = Array.apply(null, behaviors);
                            for (var k = behaviors.length - 1; k >= 0; k--) {
                                behaviors[k].dispose();
                            }
                        }
                    }
                }
                element.innerHTML = "";
            }
        }


        Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(disposeTree);

</script>
</asp:Content>
