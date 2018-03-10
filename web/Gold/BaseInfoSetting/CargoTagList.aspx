<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation ="false"  AutoEventWireup="true"  CodeBehind="CargoTagList.aspx.cs" Inherits="Gold.BaseInfoSetting.CargoTagList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%--<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />--%>
    <script language="javascript" type="text/javascript">
        

        //显示等待滚动图片
        function showWaitDiv(divName) {
            document.getElementById(divName).style.display = "block";
        }
        //隐藏等待滚动图片
        function hiddenWaitDiv(divName) {
            document.getElementById(divName).style.display = "none";
        }
    </script>
    <style type="text/css">
        .tableCellWidth td
        {
            width: 230px;
            text-align: left;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                查询条件</label>
        </div>
        <div class="commonQuery">
            <table class="tableCellWidth">
                <tr>
                    <td>
                        <label>
                            商品编码:</label>
                        <asp:TextBox ID="tbxCargoCode" runat="server" Width="150px"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品名称:</label>
                        <asp:TextBox ID="tbxCargoName" runat="server" Width="150px"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品规格:</label>
                        <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <label class="commonLabel">
                            商品型号:</label>
                        <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            层位名称:</label>
                        <asp:TextBox ID="tbxCargoTagBinCode" runat="server" Width="150px"></asp:TextBox>
                        <%--<asp:DropDownList ID="DropDownList_StorageBin" runat="server" Width="155px">
                        </asp:DropDownList>--%>
                    </td>
                    <td>
                        <label class="commonLabel">
                            存储状态:</label>
                        <asp:DropDownList ID="DropDownList_StorageState" runat="server" Width="120px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="已存商品" Value="1"></asp:ListItem>
                            <asp:ListItem Text="未存商品" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <%--<label class="commonLabel">
                            所属仓库:</label>
                        <asp:DropDownList ID="DropDownList_WareHouse" runat="server" Width="155px" OnSelectedIndexChanged="DropDownList_WareHouse_SelectedIndexChanged">
                        </asp:DropDownList>--%>
                    </td>
                    <td style="text-align: center;">
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                          OnClientClick="showWaitDiv('divWait');"   OnClick="btnQuery_Click" Text="查询" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:PostBackTrigger ControlID="lbtnExport" />
            </Triggers>
            <ContentTemplate>
                <div class="commonTitle">
                    <div class="CommonTitle_InnerdivLeft">
                        <label>
                            查询结果</label>
                    </div>
                    <div class="CommonTitle_InnerdivRight">
                        <asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                        <asp:LinkButton ID="lbtnExport" runat="server" Text="导出Excel" OnClick="lbtnExport_Click" />
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
                <asp:GridView ID="gv_CargoTagList" runat="server" AllowPaging="false" Width="1140px" ShowHeader="false"
                    AllowSorting="true" CssClass="linetable" PageSize="20" AutoGenerateColumns="False"
                    OnPageIndexChanging="gv_CargoTagList_PageIndexChanging" OnSorting="gv_CargoTagList_Sorting"
                    sortExpression="CargoCode" sortDirection="ASC" 
                    OnDataBound="gv_CargoTagList_DataBound" 
                    onrowdatabound="gv_CargoTagList_RowDataBound">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <PagerStyle CssClass="GridViewPagerStyle" />
                    <EmptyDataTemplate>
                        <table class="GridViewEmpty_Table">
                            <%--<tr class="GridViewEmpty_RowHeader">
                                <td>
                                    仓库
                                </td>
                                <td>
                                    层位
                                </td>
                                <td>
                                    商品电子标签编码
                                </td>
                                <td>
                                    商品名称
                                </td>
                                <td>
                                    商品编码
                                </td>
                                <td>
                                    数量
                                </td>
                                <td>
                                    单位
                                </td>
                                <td>
                                    型号
                                </td>
                                <td>
                                    规格
                                </td>
                                <td>
                                    备注
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
                        <asp:TemplateField HeaderText="序号" InsertVisible="False">
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <%#Container.DataItemIndex+1%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="WHName" HeaderText="仓库" SortExpression="WHName" ItemStyle-Width="100px" />--%>
                        <asp:BoundField DataField="BinCode" HeaderText="区位编码" SortExpression="BinCode" ItemStyle-Width="70px" />
                        <%--注意 这里的层位是指CargoTag的BinCode--%>
                        <asp:BoundField DataField="TagCode" HeaderText="商品电子标签码" SortExpression="TagCode"
                            ItemStyle-Width="160px" />
                        <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                            ItemStyle-Width="350px" />
                        <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                            <ItemStyle Width="180px" />
                            <ItemTemplate>
                                <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Number" HeaderText="数量" SortExpression="Number" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                            ItemStyle-Width="40px" />
                        <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                            ItemStyle-Width="80px" />
                        <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                            ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" ItemStyle-Width="80px" />
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
            </ContentTemplate>
        </asp:UpdatePanel>
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
