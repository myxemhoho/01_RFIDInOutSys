<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaleCargoSet.aspx.cs" Inherits="Gold.SaleCargoSetting.SaleCargoSet"
    ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .GridViewHeaderStyle_暂不用
        {
            /*下面两行是关键样式 固定表头 ie中可用*/
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

        //在服务器控件上记录当前鼠标在GridView内点击的X,Y坐标值
        function RecordPostion(obj) {
            var div1 = obj;
            var sx = document.getElementById('MainContent_dvscrollX');
            var sy = document.getElementById('MainContent_dvscrollY');

            var header = document.getElementById("MainContent_divHeader"); //自定义GridView标题栏

            var divScrollTop = div1.scrollTop;
            var divScrollLeft = div1.scrollLeft;

            sy.value = divScrollTop
            sx.value = divScrollLeft

            header.scrollLeft = divScrollLeft; //设置标题栏横向坐标
        }

        //从服务器控件中获取PostBack前鼠标GridView内点击的X,Y坐标值
        function GetResultFromServer() {
            try {
                var sx = document.getElementById('MainContent_dvscrollX');
                var sy = document.getElementById('MainContent_dvscrollY');
                var header = document.getElementById("MainContent_divHeader"); //自定义GridView标题栏

                var syValue = sy.value;
                var sxValue = sx.value;

                document.getElementById('gridviewContainer').scrollTop = syValue;
                document.getElementById('gridviewContainer').scrollLeft = sxValue;


                header.scrollLeft = sxValue; //设置标题栏横向坐标
            } catch (e) {
            }
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
                            是否可售:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_SaleStatus" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            OnClick="btnQuery_Click" Text="查询" OnClientClick="showWaitDiv('divWait');" />
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
            </div>
        </div>
        <!--
        <div onclick="showWaitDiv('divWait');">
            开启测试</div>
        <div onclick="hiddenWaitDiv('divWait');">
            关闭测试</div>
            -->
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
            <%--<table class="GridViewFixHeader_Table" style="width: 1140px;">
                <tr class="GridViewHeaderStyle">
                    <td style="width: 40px; text-align: left;">
                        序号
                     
                    </td>
                    <td style="width: 200px; text-align: left;">
                        <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument="CargoCode" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >商品编码</asp:LinkButton>
                    </td>
                    <td style="width: 480px; text-align: left;">
                        <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument="CargoName" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >商品名称</asp:LinkButton>
                    </td>
                    <td style="width: 80px; text-align: left;">
                        <asp:LinkButton ID="LinkButton3" runat="server" CommandArgument="CargoModel" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >型号</asp:LinkButton>
                    </td>
                    <td style="width: 140px; text-align: left;">
                        <asp:LinkButton ID="LinkButton4" runat="server" CommandArgument="CargoSpec" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >规格</asp:LinkButton>
                    </td>
                    <td style="width: 40px; text-align: left;">
                        <asp:LinkButton ID="LinkButton5" runat="server" CommandArgument="CargoUnits" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >单位</asp:LinkButton>
                    </td>
                    <td style="width: 60px; text-align: left;">
                        <asp:LinkButton ID="LinkButton6" runat="server" CommandArgument="ProduceYear" 
                                    OnClientClick="showWaitDiv('divWait');" OnClick="LinkButtonHeader_Click" >发行年份</asp:LinkButton>
                    </td>
                    <td style="width: 160px; text-align: left;">
                        <asp:LinkButton ID="LinkButton7" runat="server" CommandArgument="SaleStatus" 
                                    OnClientClick="showWaitDiv('divWait')" OnClick="LinkButtonHeader_Click" >可售状态设置</asp:LinkButton>
                        
                        &nbsp;<label>全选</label><input type="checkbox" id="chkInputSelectAll" onclick="selectAllCheckBox('MainContent_gv_CargoList',this);"/>
                    </td>
                </tr>
            </table>--%>
        </div>

        <!--设置点击GridView内控件PostBack后让GridView内行数据滚动到点击前坐标位置步骤：
        1.添加两个服务端隐藏域控件dvscrollX,dvscrollY
        2.添加JS函数RecordPostion和GetResultFromServer，注意这两个个函数中的相关控件ID要和页面中的一致
        3.在包含GridView的div中添加onscroll="j a v ascript:RecordPostion(this);"JS函数调用
        4.在后台CS代码文件中GridView_DataBound事件函数中添加调用JS的代码 string sjs = "GetResultFromServer();";  ScriptManager.RegisterClientScriptBlock(this.gv_CargoList, this.GetType(), "", sjs, true);
        -->
        <asp:HiddenField ID="dvscrollX" runat="server" />
        <asp:HiddenField ID="dvscrollY" runat="server" />
        <div style="width: 99%; height: 500px;" class="divScroll" id="gridviewContainer"
            onscroll="javascript:RecordPostion(this); "><!--onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;-->
            <asp:GridView ID="gv_CargoList" Width="1140px" runat="server" AllowPaging="false"
                AllowSorting="True" ShowHeader="false" CssClass="linetable" PageSize="20" AutoGenerateColumns="False"
                OnPageIndexChanging="gv_CargoList_PageIndexChanging" OnSorting="gv_CargoList_Sorting"
                sortExpression="CargoCode" sortDirection="ASC" OnDataBound="gv_CargoList_DataBound">
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
                                可售状态
                            </td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="12">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <%--<asp:TemplateField Visible="false">
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False" ItemStyle-Width="40px">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="480px"></asp:BoundField>
                    <asp:BoundField DataField="CargoType" HeaderText="商品类别" Visible="false" SortExpression="CargoType"
                        ItemStyle-Width="60px"></asp:BoundField>
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ItemStyle-Width="80px"></asp:BoundField>
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ItemStyle-Width="140px"></asp:BoundField>
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ItemStyle-Width="40px"></asp:BoundField>
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" SortExpression="ProduceYear"
                        ItemStyle-Width="60px"></asp:BoundField>
                    <%--<asp:BoundField DataField="SaleStatus" HeaderText="可售状态" 
                        SortExpression="SaleStatus" ItemStyle-Width="100px"
                        ItemStyle-HorizontalAlign="Right" >                        
<ItemStyle HorizontalAlign="Right" Width="100px"></ItemStyle>
                    </asp:BoundField>--%>
                    <asp:TemplateField HeaderText="可售状态设置" SortExpression="SaleStatus" ItemStyle-Width="160px">
                        <HeaderTemplate>
                            <asp:LinkButton runat="server" ID="lBtnSaleStatus" Text="可售状态" OnClick="lBtnSaleStatus_Click"></asp:LinkButton>
                            <asp:CheckBox ID="CheckBox_CheckAll" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckAll_CheckedChanged"
                                Text="全选" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSaleStatus" runat="server" Checked='<%# Eval("SaleStatus")==null?false: (Eval("SaleStatus").ToString()=="1"?true:false) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: left">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />
                                    页 共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />
                                    页 </span>
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
            <br />
        </div>
        <br />
        <asp:Button ID="btnSave" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
            Text="保存设置" OnClick="btnSave_Click" OnClientClick="showWaitDiv('divWait');" />
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
