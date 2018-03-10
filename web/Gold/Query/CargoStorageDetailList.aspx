<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="CargoStorageDetailList.aspx.cs" Inherits="Gold.Query.CargoStorageDetailList" %>

<%@ Register TagPrefix="uc" TagName="GoodsSelect" Src="~/Controls/GoodsSelect.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
    <style type="text/css">
        .btnQueryCss
        {
            padding-left: 32px;
            padding-top: 2px;
            padding-bottom: 2px;
            background: url(../Styles/images/finder.png) no-repeat left top; /* 图片路径*/
            background-color: WhiteSmoke;
            border: solid 1px #999; /*去掉边框*/
            color: Black; /*为空间字体设置颜色*/
            cursor: hand;
            font-family: 微软雅黑; /*margin-left: 20px;*/
        }
        
        .tdRight
        {
            /*background-color: #C0C0C0;*/
            background-color: #E0E0E0;
            text-align: right;
            width: 90px;
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
        <div class="commonQuery" style=" margin-left:2px;">
            <table width="1140px" class="linetable">
                <tr>
                    <td valign="middle" class="tdRight">
                        <label class="commonLabel">
                            查询方式:</label>
                    </td>
                    <td width="130px">
                        <asp:RadioButton ID="RadioButton_ByPrecise" Text="精确查询" GroupName="GroupRadio" runat="server"
                            AutoPostBack="True" OnCheckedChanged="RadioButton_ByPrecise_CheckedChanged" OnClick="showWaitDiv('divWait');" />
                        <br />
                        <asp:RadioButton ID="RadioButton_ByLike" Text="模糊查询" GroupName="GroupRadio" runat="server"
                            AutoPostBack="True" OnCheckedChanged="RadioButton_ByLike_CheckedChanged" OnClick="showWaitDiv('divWait');" />
                    </td>
                    <td class="tdRight" >
                        <label class="commonLabel">
                            商品编码:</label>
                    </td>
                    <td valign="top" width="430px">
                        <div style=" display:block; margin:0px;">
                            <div style=" display:block; float:left; margin:0px; width:280px;">
                                <asp:TextBox ID="tbxCargoCode" runat="server" Width="98%" Height="64px" Style="border: solid 1px #7F9DB9;"
                                    TextMode="MultiLine"></asp:TextBox>
                            </div>
                            <div style=" display:block; float:left;margin:0px;width:100px;">
                                <asp:Button runat="server" ID="BtnAddSelectCargo" OnClick="BtnAddSelectCargo_Click"
                                    OnClientClick="showWaitDiv('divWait');" CssClass="btnQueryCss" Width="100px"
                                    Height="32px" Text="累加查询" />
                                    <asp:Button runat="server" ID="BtnSelectCargo" OnClick="BtnSelectCargo_Click" OnClientClick="showWaitDiv('divWait');"
                                    CssClass="btnQueryCss" Width="100px" Height="32px" Text="重新查询" />                                
                                
                            </div>
                        </div>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品名称:</label>
                    </td>
                    <td valign="top" width="310px">
                        <asp:TextBox ID="tbxCargoName" runat="server" Width="300px" Height="64px" Style="border: solid 1px #7F9DB9;"
                            TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品规格:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>                    
                    <td class="tdRight">
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="160px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            区位:
                        </label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxBinCode" runat="server" Width="120px"></asp:TextBox>
                    </td>
                    </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            商品型号:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight" width="120px">
                        <label class="commonLabel">
                            RFID仓库/区位一致性:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_StoreEqual" runat="server" Width="160px">
                            <asp:ListItem Text="" Value="-1"></asp:ListItem>
                            <asp:ListItem Text="不一致" Value="0"></asp:ListItem>
                            <asp:ListItem Text="一致" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="tdRight" width="110px">
                        <label class="commonLabel">
                            RFID/用友存量一致性:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_NCEqual" runat="server" Width="120px">
                            <asp:ListItem Text="" Value="-1"></asp:ListItem>
                            <asp:ListItem Text="不一致" Value="0"></asp:ListItem>
                            <asp:ListItem Text="一致" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding-top: 15px;">
                <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    OnClientClick="showWaitDiv('divWait');" OnClick="btnQuery_Click" Text="查询" />&nbsp;
                    <asp:CheckBox ID="chkQueryNC" runat="server" Text="同时查询用友库存" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnStartAlarm" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    OnClientClick="showWaitDiv('divWait');" OnClick="btnStartAlarm_Click" Text="对查询结果亮灯" />&nbsp;
                <asp:Button ID="btnStopAlarm" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    OnClientClick="showWaitDiv('divWait');" OnClick="btnStopAlarm_Click" Text="对查询结果关灯" />
            </div>
            <br />
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
                <asp:Button ID="btnGoToPrintAndExportPage" runat="server" CausesValidation="false"
                    OnClientClick="showWaitDiv('divWait');" CssClass="ButtonImageStyle" Text="打印/导出"
                    OnClick="btnGoToPrintAndExportPage_Click" />
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
        <div id="divWait" style="display: none; z-index:99999;">
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
            <asp:GridView ID="gv_CargoList" Width="1155px" runat="server" AllowPaging="false"
                ShowFooter="true" ShowHeader="false" AllowSorting="true" CssClass="linetable"
                PageSize="10" AutoGenerateColumns="False" OnPageIndexChanging="gv_CargoList_PageIndexChanging"
                OnSorting="gv_CargoList_Sorting" sortExpression="CargoCode" sortDirection="ASC"
                OnDataBound="gv_CargoList_DataBound" OnRowDataBound="gv_CargoList_RowDataBound"
                OnRowCommand="gv_CargoList_RowCommand">
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
                                仓库编码
                            </td>
                            <td>
                                仓库名称
                            </td>
                            <td>
                                仓库存量
                            </td>
                            <td>
                                区位名称
                            </td>
                            <td>
                                区位存量
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
                    <asp:TemplateField HeaderText="序号" InsertVisible="False" ItemStyle-Width="40px">
                        <ItemStyle HorizontalAlign="Center"/>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoCode" HeaderText="商品编码" SortExpression="CargoCode"
                        ItemStyle-Width="150px" />
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ItemStyle-Width="280px" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ItemStyle-Width="60px" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ItemStyle-Width="40px" />
                    <%--<asp:BoundField DataField="WHCode" HeaderText="仓库编码" SortExpression="WHCode" ReadOnly="true" Visible="false"
                        ItemStyle-Width="70px"></asp:BoundField>--%>
                    <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHName" ReadOnly="true"
                        ItemStyle-Width="90px"></asp:BoundField>
                     <asp:BoundField DataField="NCCargoStockCount" HeaderText="用友存量" SortExpression="NCCargoStockCount"
                        ItemStyle-HorizontalAlign="Right" ReadOnly="true" ItemStyle-Width="70px"></asp:BoundField>
                    <asp:BoundField DataField="CargoStockCount" HeaderText="RFID存量" SortExpression="CargoStockCount"
                        ItemStyle-HorizontalAlign="Right" ReadOnly="true" ItemStyle-Width="70px"></asp:BoundField>
                    <asp:BoundField DataField="BinCode" HeaderText="区位" SortExpression="BinCode" ReadOnly="true"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Width="60px"></asp:BoundField>
                    <asp:BoundField DataField="Number" HeaderText="区位存量" SortExpression="Number" ItemStyle-HorizontalAlign="Right"
                        ReadOnly="true" ItemStyle-Width="65px"></asp:BoundField>
                    <asp:TemplateField HeaderText="标签" SortExpression="BinTagStatus" ItemStyle-Width="50px"
                        ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblRowBinTagStatus" Visible="false" runat="server" Text='<%# Bind("BinTagStatus") %>'></asp:Label>
                            <asp:Label ID="lblRowBinTagStatusName" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="标签报警测试" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="lblRowBinTagID" Visible="false" runat="server" Text='<%# Bind("BinTagID") %>'></asp:Label>
                            <asp:Label ID="lblCargoCode" Visible="false" runat="server" Text='<%# Bind("CargoCode") %>'></asp:Label>
                            <asp:Label ID="lblWHCode" Visible="false" runat="server" Text='<%# Bind("WHCode") %>'></asp:Label>
                            <asp:Label ID="lblBinCode" Visible="false" runat="server" Text='<%# Bind("BinCode") %>'></asp:Label>
                            <asp:Label ID="lblNumber" Visible="false" runat="server" Text='<%# Bind("Number") %>'></asp:Label>
                            <asp:Button ID="btnRowTagTestAlarmStart" Text="启动" CssClass="ButtonSmallImageStyle"
                                CommandName="TagAlarmStart" CausesValidation="false" runat="server" />
                            <asp:Button ID="btnRowTagTestAlarmStop" Text="停止" CssClass="ButtonSmallImageStyle"
                                CommandName="TagAlarmStop" CausesValidation="false" runat="server" />
                            <asp:Label ID="lblRowTagTestShortMsg" CssClass="commonSaveMsgLabel" runat="server"
                                Text=""></asp:Label>
                            <div id='<%# "waitDiv_X"+
                            Eval("CargoCode").ToString()+
                            (Eval("WHCode")==null?"":Eval("WHCode").ToString())+
                            (Eval("BinCode")==null?"":Eval("BinCode").ToString())+
                            (Eval("Number")==null?"":Eval("Number").ToString()) %>' style="display: none;">
                                <img src="../Styles/images/uploading.gif" />
                                <label>
                                    执行中……</label></div>
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
        </div>
        <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupCargo">
            <div class="modalPopupWrapperCargo">
                <div id="pnlDragTitle" class="modalHeader">
                    <span>选择商品</span>
                    <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="关  闭" OnClientClick="showWaitDiv('divWait');"
                        OnClick="btnClosePop_Click" />
                </div>
                <div class="modalBody">
                    <uc:GoodsSelect ID="GoodsSelect1" runat="server" OnGetCargoSelect="GoodsSelect1_GetCargoSelect" />
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton ID="LinkButton1" runat="server" Width="0" Height="0"></asp:LinkButton>
        <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="LinkButton1"
            PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
            PopupDragHandleControlID="pnlDragTitle">
        </ajaxToolkit:ModalPopupExtender>



        <!--下面是类似于MessageBox的功能，使用下面的控件需要在每个页面引用MsgBoxJScript.js及样式文件-->
        <asp:LinkButton runat="server" ID="showModalPopupServerOperatorButton_Msg" Visible="false"
            Text="shown via server in code behind" OnClick="showModalPopupServerOperatorButton_Click" />
        <a id="showModalPopupClientButton_Msg" href="#" style="display:none;">on the client in script</a>
        <asp:Button runat="server" ID="hiddenTargetControlForModalPopup_Msg" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="programmaticModalPopup_Msg" BehaviorID="programmaticModalPopupBehavior_Msg"
            TargetControlID="hiddenTargetControlForModalPopup_Msg" PopupControlID="programmaticPopup_Msg"
            BackgroundCssClass="modalBackground_Msg" DropShadow="True" PopupDragHandleControlID="programmaticPopupDragHandle_Msg"
            RepositionMode="RepositionOnWindowScroll">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel runat="server" CssClass="modalPopup_Msg" ID="programmaticPopup_Msg" Style="display: none;">
            <asp:Panel runat="Server" ID="programmaticPopupDragHandle_Msg" CssClass="programmaticPopupDragHandle_Msg_Css">
                <div style="float: left;">
                    系统提示</div>
                <div style="float: right;">
                    <input type="button" id="hideModalPopupViaClientButton_Msg" class="ClosePopButton_Msg"
                        value="关闭" />
                    <!--<a id="hideModalPopupViaClientButton_Msg" href="#" class="ClosePopButton" >关闭</a>-->
                </div>
            </asp:Panel>
            <div style="padding: 10px;">
                <asp:Label ID="lblMessageContent" runat="server" Text=""></asp:Label>
                <br />
                <div id="divMessageDetail" runat="server">
                <hr />
                <span>详细信息</span>
                <%--<asp:Label ID="lblMessageDetailTitle" runat="server" Text="详细信息"></asp:Label>--%>
                <br />
                <asp:Label ID="lblMessageContentException" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </asp:Panel>
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
