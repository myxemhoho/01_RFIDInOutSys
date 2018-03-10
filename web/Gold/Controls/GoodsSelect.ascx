<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoodsSelect.ascx.cs"
    Inherits="Gold.Controls.GoodsSelect" %>
<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    /*等待提示框*/
    #divWaitUCCargo
    {
        width: 100px;
        height: 20px;
        padding-top: 0px;
        text-align: center;
        font-size: 10pt;
        color: #6495ED;
        vertical-align: middle;
        border: solid 2px #6495ED;
        background-color: White;
        z-index: 999;
        display: none;
    }
</style>
<script language="javascript" type="text/javascript">
    function selectAll1(obj) {
        var theTable = document.getElementById("<%=gvGoods.ClientID%>");  //obj..parentElement.parentElement.parentElement;

        var nodeList = theTable.getElementsByTagName("input");
        for (var i = 0; i < nodeList.length; i++) {
            var node = nodeList[i];
            if (node.type == "checkbox") {
                node.checked = obj.checked;
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

    //清除label的Text
    function clearLabelText(lblID) {
        document.getElementById(lblID).innerText = "";
    }

//    function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
//        var theTable = document.getElementById(gridViewClientID);
//        var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
//        var i;
//        var j = 0; //checkbox的列索引

//        if (theTable == undefined)
//            return;

//        for (i = 0; i < theTable.rows.length; i++) {
//            var objCheckBox = theTable.rows[i].cells[j].firstChild;
//            if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
//        }
    //    }

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
<div style="border: solid 5px #6495ED"><!--四周边框5px 和天蓝色-->
    <table style="width:950px; padding:0px; margin:0px; ">
        <tr>
            <td style="text-align: left; width: 110px;">
                请输入条件查询！
            </td>
            <td style="text-align: right; width: 90px;">
                商品编码：
            </td>
            <td style="width: 120px;">
                <asp:TextBox ID="txtCode" runat="server" MaxLength="32" Width="120px" />
            </td>
            <td style="text-align: right; width: 90px;">
                商品名称：
            </td>
            <td style="width: 160px;">
                <asp:TextBox ID="txtName" runat="server" MaxLength="32" Width="160px" />
            </td>
            <td style="width: 110px;">
                &nbsp;<asp:CheckBox ID="chkQueryAll" runat="server" Text="查询全部商品" />&nbsp;
            </td>
            <td style="width: 100px;">
                &nbsp;
                <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    OnClientClick="showWaitDiv('divWaitUCCargo');" Text="查 询" OnClick="btnQuery_Click" />
            </td>
            <td style="width: 110px;">
                <div id="divWaitUCCargo" style="display: none; vertical-align: middle;">
                    <div style="display: inline; margin: 0px; padding: 0px;">
                        <img src="../Styles/images/uploading.gif" align="absmiddle" /><label>执行中…</label>
                        &nbsp;<%--<label onclick="hiddenWaitDiv('divWaitUCCargo');">X</label>--%><img
                            src="../Styles/images/close.png" align="absmiddle" class="divWait_Close" onclick="hiddenWaitDiv('divWaitUCCargo');" /></div>
                </div>
            </td>
        </tr>
    </table>
    <hr />
    <%--<div class="boxContent" style="overflow-x: scroll; height:650px;">--%>
    <!--
             固定GridView标题的方法
             1.GridView外用带滚动条的div包着，并设置GridView的宽度
             2.另外做一个静态Table放在div中，此div宽度与包含GridView的div的宽度一致
             3.设置静态Table的宽度与GridView的宽度一致
             4.在包含GridVidw的div中写javascript onscroll="divHeader.scrollLeft=this.scrollLeft"
             5.在静态Table中要排序的列单元格中防止LinkButton用来排序，并在后台代码中写排序方法
             7.设置GridView标记中的ShowHeader=false
             -->
    <div style="padding: 0px 5px 0px 5px;">
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
        <div style="width: 99%; height: 405px;" class="divScroll" id="gridviewContainer"
            onscroll="MainContent_GoodsSelect1_divHeader.scrollLeft=this.scrollLeft;">
            <!--DataSourceID="edsCargos"-->
            <asp:GridView runat="server" ID="gvGoods" AllowPaging="false" ShowHeader="false"
                PageSize="20" OnPageIndexChanging="gvGoods_PageIndexChanging" Width="1930px"
                AutoGenerateColumns="False" DataKeyNames="CargoCode" OnDataBound="gvGoods_DataBound"
                sortExpression="CargoCode" sortDirection="ASC" OnRowCommand="gvGoods_RowCommand"
                OnSorting="gvGoods_Sorting" CssClass="linetable">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table">
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="12">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <%--<PagerSettings LastPageText="最后一页" FirstPageText="第一页" NextPageText="下一页" Visible="true" Mode="NumericFirstLast" />--%>
                <%-- <PagerTemplate >
                <
                </PagerTemplate>--%>
                <Columns>
                    <asp:TemplateField ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderText="*">
                        <ItemTemplate>
                            <asp:CheckBox ID="chbSelect" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            全选<input id="CheckAll" type="checkbox" onclick="selectAll1(this);" />
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoCode" HeaderText="商品编码" HtmlEncode="false" ItemStyle-Width="180px"
                        SortExpression="CargoCode" />
                    <asp:BoundField DataField="CargoName" HeaderText="名称" HtmlEncode="false" ItemStyle-Width="480px"
                        SortExpression="CargoName" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" HtmlEncode="false" ItemStyle-Width="60px"
                        SortExpression="CargoModel" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" HtmlEncode="false" ItemStyle-Width="80px"
                        SortExpression="CargoSpec" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" HtmlEncode="false" ItemStyle-Width="40px"
                        SortExpression="CargoUnits" />
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" HtmlEncode="false" ItemStyle-Width="60px"
                        SortExpression="ProduceYear" />
                    <asp:BoundField DataField="Price1" HeaderText="批发价" HtmlEncode="false" ItemStyle-Width="100px"
                        SortExpression="Price1" />
                    <asp:BoundField DataField="Price2" HeaderText="金卡会员价" HtmlEncode="false" ItemStyle-Width="100px"
                        SortExpression="Price2" />
                    <asp:BoundField DataField="Price3" HeaderText="银卡会员价" HtmlEncode="false" ItemStyle-Width="100px"
                        SortExpression="Price3" />
                    <asp:BoundField DataField="Price4" HeaderText="白金卡会员价" HtmlEncode="false" ItemStyle-Width="100px"
                        SortExpression="Price4" />
                    <asp:BoundField DataField="Price5" HeaderText="参考售价" HtmlEncode="false" ItemStyle-Width="100px"
                        SortExpression="Price5" />
                    <asp:BoundField DataField="Comment" HeaderText="备注" HtmlEncode="false" ItemStyle-Width="480px"
                        SortExpression="Comment" />
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
    </div>
    <%--<asp:EntityDataSource ID="edsCargos" runat="server" 
            ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
            EnableFlattening="False" EntitySetName="Cargos" 
            onquerycreated="edsCargos_QueryCreated" >
        </asp:EntityDataSource>--%>
    <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
    
    <div style="text-align: center;">
        <table style="text-align: center; width: 100%;">
            <tr>
                <td>
                    <asp:Button ID="btnOk" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                        Text="确 定" OnClick="btnOK_Click" />
                </td>
            </tr>
        </table>
    </div>
</div>
