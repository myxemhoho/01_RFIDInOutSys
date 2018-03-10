<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaleAllocationQuery.aspx.cs" Inherits="Gold.SaleCargoSetting.SaleAllocationQuery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .GridViewHeaderStyle_暂时不用
        {
            /*下面三行是关键样式 固定表头 ie中可用*/
            position: relative;
            top: expression(gridviewContainer.scrollTop-1);
            left: expression(gridviewContainer.scrollLeft-1); /*下面几行仅是控制样式*/
            background-color: #C0C0C0;
            font-family: 微软雅黑;
            font-size: 10pt;
            color: #9B6D2C; /*WhiteSmoke*/
            height: 22px;
        }
    </style>
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

        function IsNumber(input) {
            var re = /^[0-9]+.?[0-9]*$/;   //判断字符串是否为数字     //判断正整数 /^[1-9]+[0-9]*]*$/

            if (!re.test(input))
                return false;
            else
                return true;
        }

        //检查输入的分配量是否为数字且是否各部门分配量超过结存数量
        function NumberConfirm(rowIndex) {
            var t = document.getElementById("<%=gv_SaleAllocationList.ClientID%>")
            var jiecunColIndex = 9;
            var qudaoColIndex = 10;
            var xiaoshouColIndex = 11;
            var lingshouColIndex = 12;
            var dianziColIndex = 13;
            var gongyongColIndex = 14;
            var jidongColIndex = 15;


            //从第一行开始遍历（第0行是表头）
            //for (i = 1; i < t.rows.length; i++) 
            //{
            //if (i = rowIndex+1) 
            //{
            var i = rowIndex + 1;
            var errorMsg = "";
            //alert(t.rows[i].cells[cellNum].innerHTML)

            var jiecunHtml = t.rows[i].cells[jiecunColIndex].innerHTML
            jiecunTextBox = t.rows[i].cells[jiecunColIndex].getElementsByTagName("INPUT")[0]
            var jiecunNumber = jiecunTextBox == undefined ? "" : jiecunTextBox.value;
            if (!IsNumber(jiecunNumber)) //(!isNaN(jiecunNumber))//如果不是数字则提示
                errorMsg += "结存数量不能为空且只能为数字.\r";

            var qudaoHtml = t.rows[i].cells[qudaoColIndex].innerHTML
            qudaoTextBox = t.rows[i].cells[qudaoColIndex].getElementsByTagName("INPUT")[0]
            var qudaoNumber = qudaoTextBox == undefined ? "" : qudaoTextBox.value;
            if (!IsNumber(qudaoNumber))//(!isNaN(qudaoNumber))//如果不是数字则提示
                errorMsg += "渠道部的分配量不能为空且只能为数字.\r";

            var xiaoshouHtml = t.rows[i].cells[xiaoshouColIndex].innerHTML
            xiaoshouTextBox = t.rows[i].cells[xiaoshouColIndex].getElementsByTagName("INPUT")[0]
            var xiaoshouNumber = xiaoshouTextBox == undefined ? "" : xiaoshouTextBox.value;
            if (!IsNumber(xiaoshouNumber))//(!isNaN(xiaoshouNumber))//如果不是数字则提示
                errorMsg += "销售部的分配量不能为空且只能为数字.\r";

            var lingshouHtml = t.rows[i].cells[lingshouColIndex].innerHTML
            lingshouTextBox = t.rows[i].cells[lingshouColIndex].getElementsByTagName("INPUT")[0]
            var lingshouNumber = lingshouTextBox == undefined ? "" : lingshouTextBox.value;
            if (!IsNumber(lingshouNumber))// (!isNaN(lingshouNumber))//如果不是数字则提示
                errorMsg += "零售中心的分配量不能为空且只能为数字.\r";

            var dianziHtml = t.rows[i].cells[dianziColIndex].innerHTML
            dianziTextBox = t.rows[i].cells[dianziColIndex].getElementsByTagName("INPUT")[0]
            var dianziNumber = dianziTextBox == undefined ? "" : dianziTextBox.value;
            if (!IsNumber(dianziNumber))//(!isNaN(dianziNumber))//如果不是数字则提示
                errorMsg += "电子商务的分配量不能为空且只能为数字.\r";

            var gongyongHtml = t.rows[i].cells[gongyongColIndex].innerHTML
            gongyongTextBox = t.rows[i].cells[gongyongColIndex].getElementsByTagName("INPUT")[0]
            var gongyongNumber = gongyongTextBox == undefined ? "" : gongyongTextBox.value;
            if (!IsNumber(gongyongNumber))//(!isNaN(gongyongNumber))//如果不是数字则提示
                errorMsg += "公用部门的分配量不能为空且只能为数字.\r";

            var jidongHtml = t.rows[i].cells[jidongColIndex].innerHTML
            jidongTextBox = t.rows[i].cells[jidongColIndex].getElementsByTagName("INPUT")[0]
            var jidongNumber = jidongTextBox == undefined ? "" : jidongTextBox.value;
            if (!IsNumber(jidongNumber))//(!isNaN(jidongNumber))//如果不是数字则提示
                errorMsg += "机动量不能为空且只能为数字.\r";

            var DeptSum = parseFloat(qudaoNumber) + parseFloat(xiaoshouNumber) +
                            parseFloat(lingshouNumber) + parseFloat(dianziNumber) + parseFloat(gongyongNumber) + parseFloat(jidongNumber);

            if (errorMsg != "") {
                errorMsg += "\r请重新输入各部门分配量！";
                alert(errorMsg);
                return false;
            }

            if (DeptSum > jiecunNumber) {
                return confirm("各部门分配量之和为" + DeptSum + ".\r结存数量为" + jiecunNumber + ".\r\r机动量与各部门分配量之和已经超过结存数量！\r是否继续保存此数据？");
            }
            else {
                return true;
            }
            //}
            //}
            //return false;
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
    <div class="commonTitle">
        <label>
            可售商品信息公告</label>
    </div>
    <div class="commonQuery">
        <table width="99%">
            <tr>
                <td width="128px">
                    <asp:Image ID="Image1" ImageUrl="~/Styles/images/news.gif" runat="server" />
                </td>
                <td colspan="3">
                    <div runat="server" id="divNews" style="border: solid 1px #999; min-height: 128px;
                        background-color: WhiteSmoke;">
                    </div>
                    <%--<asp:TextBox ID="tbxNews" runat="server" TextMode="MultiLine" ReadOnly="true" 
                            Width="99%" BorderStyle="None" Height="118px" BackColor="#fff7ea"></asp:TextBox>--%>
                </td>
            </tr>
        </table>
        <br />
        <asp:Timer ID="Timer_News" runat="server" Enabled="true" Interval="120000" OnTick="Timer_News_Tick">
            <!--每120秒更新一次数据-->
        </asp:Timer>
    </div>
    <hr />
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                可售商品查询条件</label>
        </div>
        <div class="commonQuery">
            <table width="880px">
                <tr>
                    <td class="tdRight" width="70px">
                        <label class="commonLabel">
                            商品编码:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdRight" width="70px">
                        <label class="commonLabel">
                            商品名称:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoName" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdRight" width="70px">
                        <label class="commonLabel">
                            是否可售:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_SaleStatus" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            OnClientClick="showWaitDiv('divWait');" OnClick="btnQuery_Click" Text="查询" />
                    </td>
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品规格:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="150px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品型号:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="150px">
                        </asp:DropDownList>
                    </td>
                    <td>
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
                    可售商品查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
                <asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                <asp:Button ID="btnGoToPrintAndExportPage" runat="server" CausesValidation="false"
                    OnClientClick="showWaitDiv('divWait');" CssClass="ButtonImageStyle" Text="打印/导出" OnClick="btnGoToPrintAndExportPage_Click" />
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
                            <td>                                
                                <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument="CargoName" 
                                    onclick="LinkButtonHeader_Click">商品名称</asp:LinkButton>
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
                                批发价
                            </td>
                            <td>
                                参考售价
                            </td>
                            <td>
                                结存数量
                            </td>
                            <td>
                                渠道部分配量
                            </td>
                            <td>
                                销售部分配量
                            </td>
                            <td>
                                零售中心分配量
                            </td>
                            <td>
                                电子商务分配量
                            </td>
                            <td>
                                公用部门分配量
                            </td>
                            <td>
                                机动量
                            </td>
                        </tr>
                    </table>--%>
        </div>
        <div style="width: 99%; height: 500px;" class="divScroll" id="gridviewContainer"
            onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;">
            <asp:GridView ID="gv_SaleAllocationList" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="True" CssClass="linetable" Width="1560px" AutoGenerateColumns="False"
                DataKeyNames="CargoCode" OnPageIndexChanging="gv_SaleAllocationList_PageIndexChanging"
                OnDataBound="gv_SaleAllocationList_DataBound" sortDirection="ASC" sortExpression="CargoCode"
                OnSorting="gv_SaleAllocationList_Sorting">
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
                                批发价
                            </td>
                            <td>
                                参考售价
                            </td>
                            <td>
                                结存数量
                            </td>
                            <td>
                                渠道部分配量
                            </td>
                            <td>
                                销售部分配量
                            </td>
                            <td>
                                零售中心分配量
                            </td>
                            <td>
                                电子商务分配量
                            </td>
                            <td>
                                公用部门分配量
                            </td>
                            <td>
                                机动量
                            </td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="15">
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
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="160px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ReadOnly="true" ItemStyle-Width="300px"></asp:BoundField>
                    <asp:BoundField DataField="CargoType" HeaderText="商品类别" Visible="false" SortExpression="CargoType"
                        ReadOnly="true" ItemStyle-Width="60px"></asp:BoundField>
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ReadOnly="true" ItemStyle-Width="80px"></asp:BoundField>
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ReadOnly="true" ItemStyle-Width="100px"></asp:BoundField>
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ReadOnly="true" ItemStyle-Width="80px"></asp:BoundField>
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" SortExpression="ProduceYear"
                        ReadOnly="true" ItemStyle-Width="100px"></asp:BoundField>
                    <asp:BoundField DataField="Price1" HeaderText="批发价" SortExpression="Price1" ItemStyle-Width="80px"
                        ReadOnly="true" ItemStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Price5" HeaderText="参考售价" SortExpression="Price5" ItemStyle-Width="80px"
                        ReadOnly="true" ItemStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="结存数量" SortExpression="Total" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Label ID="lblRowTotal" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("Total") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="渠道部    分配量" SortExpression="DeptChannelAllocation"
                        ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptChannelAllocation" runat="server" Text='<%# Eval("DeptChannelAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="销售部    分配量" SortExpression="DeptSaleAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptSaleAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptSaleAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="零售中心分配量" SortExpression="DeptRetailAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowComment" runat="server" Text='<%# Eval("DeptRetailAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="电子商务分配量" SortExpression="DeptECAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptECAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptECAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="公用部门分配量" SortExpression="DeptPublicAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptPublicAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptPublicAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="机动量" SortExpression="Variation" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowVariation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("Variation") %>'></asp:Label>
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
            <br />
        </div>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
    </div>
        <!--下面的代码是微软提供为解决UpdatePanel在IE中慢的代码-->
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
