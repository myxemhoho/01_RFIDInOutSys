<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CargoSafeInventoryList.aspx.cs" Inherits="Gold.Query.CargoSafeInventoryList" %>
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

        function IsNumber(input) {
            var re = /^[0-9]+.?[0-9]*$/;   //判断字符串是否为数字     //判断正整数 /^[1-9]+[0-9]*]*$/

            if (!re.test(input))
                return false;
            else
                return true;
        }

        function IsCheckedOK(tbxID) {
            var tbx = document.getElementById(tbxID);
            if (tbx != null) {
                if (tbx.value == "") {
                    alert("设置的安全库存量不能为空，且只能为数字，请重新填写！");
                    return false;
                 }
                if (tbx.value != "") {
                    if (IsNumber(tbx.value) == false) {
                        alert("设置的安全库存量只能为数字，请重新填写！");
                        return false;
                     }
                 }
            }
            return true;
        }

//        function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
//            var theTable = document.getElementById(gridViewClientID);
//            var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
//            var i;
//            var j = 0; //checkbox的列索引

//            if (theTable == undefined)
//                return;

//            for (i = 0; i < theTable.rows.length; i++) {
//                var objCheckBox = theTable.rows[i].cells[j].firstChild;
//                if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
//            }
        //        }

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
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" >
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                查询条件</label>
        </div>
        <div class="commonQuery">
            <table width="800px">
                <tr>
                    <td class="tdRight" width="70px">
                        <label class="commonLabel">
                            商品编码:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoCode" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品名称:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxCargoName" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="150px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            OnClick="btnQuery_Click" Text="查询" OnClientClick="showWaitDiv('divWait');"/>
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
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
            <%--<asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>--%>
                <%--<asp:Button ID="btnGoToPrintAndExportPage" runat="server" 
                    CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="打印/导出" onclick="btnGoToPrintAndExportPage_Click" />--%>
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
    
        <asp:GridView ID="gv_SaleAllocationList" runat="server" AllowPaging="false" ShowHeader="false" 
                 PageSize="5" AllowSorting="True"
            CssClass="linetable" Width="1080px" AutoGenerateColumns="False" DataKeyNames="CargoCode"
            OnPageIndexChanging="gv_SaleAllocationList_PageIndexChanging" 
            OnDataBound="gv_SaleAllocationList_DataBound"  sortDirection="ASC"
            sortExpression="CargoCode" OnSorting="gv_SaleAllocationList_Sorting" 
                 >
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
            <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <RowStyle CssClass="GridViewRowStyle" />
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <PagerStyle CssClass="GridViewPagerStyle" />
            <EmptyDataTemplate>
                <table class="GridViewEmpty_Table" style="width:100%;">
                    <%--<tr class="GridViewEmpty_RowHeader">
                        <td>商品编码</td>
                        <td>商品名称</td>
                        <td>型号</td>
                        <td>规格</td>
                        <td>单位</td>
                        <td>发行年份</td>
                        <td>仓库编码</td>
                        <td>仓库名</td>
                        <td>安全库存量</td>
                    </tr>--%>
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="15">
                            无数据
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <Columns>
            <asp:TemplateField HeaderText="*">
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>
                <asp:TemplateField HeaderText="序号" InsertVisible="False">
                    <ItemStyle HorizontalAlign="Center" Width="40px"  />
                    <HeaderStyle HorizontalAlign="Center"/>
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
                <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"  ReadOnly="true" ItemStyle-Width="300px" >                    
                </asp:BoundField>
                <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"  ReadOnly="true" ItemStyle-Width="80px" >                    
                </asp:BoundField>
                <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"  ReadOnly="true" ItemStyle-Width="80px" >                    
                </asp:BoundField>
                <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"  ReadOnly="true" ItemStyle-Width="60px" >                    
                </asp:BoundField>
                <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" SortExpression="ProduceYear"  ReadOnly="true" ItemStyle-Width="80px" >                    
                </asp:BoundField>   
                <asp:BoundField DataField="WHCode" HeaderText="仓库编码" SortExpression="WHCode"  ReadOnly="true" ItemStyle-Width="80px" >    </asp:BoundField>                    
                <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHName"  ReadOnly="true" ItemStyle-Width="100px" >                    
                </asp:BoundField>             
                <asp:TemplateField HeaderText="安全库存量" SortExpression="SafeInventory" ItemStyle-Width="100px" ItemStyle-VerticalAlign="Top" >
                    <ItemTemplate>
                        <asp:Label ID="lblRowSafeInventory" style="word-break:break-all;word-wrap:break-word;" runat="server" Text='<%# Eval("SafeInventory") %>'></asp:Label>
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
         <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
         <br />
         <div class="commonTitle">
            <label>
                对选中商品设置安全库存量</label>
        </div>
        <div class="commonQuery">
            <label class="commonLabel">
                设置本页面内选中商品在["<span style=" font-weight:bold; color:Blue;"><asp:Label ID="lblWHName" runat="server" Text="--"></asp:Label></span>"]：</label>
            <asp:TextBox ID="tbxNewSafeNumber" runat="server" MaxLength="50"></asp:TextBox>
            <label class="commonSaveMsgLabel">
                (* 必填项,只能填数字)</label>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="commonSaveMsgLabel"
                runat="server" ControlToValidate="tbxNewSafeNumber" ErrorMessage="安全库存量不能为空，且只能为数字！" Display="Dynamic"
                ValidationGroup="ModelNewGroup"></asp:RequiredFieldValidator>
                <br />
            <asp:Button ID="btnAddNew" runat="server" CssClass="ButtonImageStyle" OnClientClick="return IsCheckedOK('MainContent_tbxNewSafeNumber');" Text="保存" ValidationGroup="ModelNewGroup"
                CausesValidation="true" OnClick="btnAddNew_Click" />
                <br />
            <asp:Label ID="lblAddMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
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
