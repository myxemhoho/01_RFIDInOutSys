<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CargoList.aspx.cs" Inherits="Gold.BaseInfoSetting.CargoList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
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

        function selectAllCheckBox(gridViewClientID, allCheckBoxObj) {
            var theTable = document.getElementById(gridViewClientID);
            var obj = allCheckBoxObj;  //document.getElementById(allCheckBoxObj);
            var i;
            var j = 0; //checkbox的列索引

            if (theTable == undefined)
                return;

            for (i = 0; i < theTable.rows.length; i++) {
                var objCheckBox = theTable.rows[i].cells[j].firstChild;
                if (objCheckBox.checked != null) objCheckBox.checked = obj.checked;
            }
        }

        function AskConfirm() {
            if (confirm('确定要将用友数据导入到RFID系统中吗？') == true) {
                showWaitDiv('divWait');
                return true;
            }
            else
                return false;
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
            <%--<asp:Label ID="lblCode" CssClass="commonLabel" runat="server" Text="商品代码："></asp:Label>--%>
            <label class="commonLabel">商品编码:</label>
            <asp:TextBox ID="tbxCargoCode" runat="server"></asp:TextBox>
            &nbsp;&nbsp;
            <%--<asp:Label ID="lblName" CssClass="commonLabel" runat="server" Text="商品名称："></asp:Label>--%>
            <label class="commonLabel" >商品名称:</label>
            <asp:TextBox ID="tbxCargoName" runat="server"></asp:TextBox>
            &nbsp;&nbsp;
            <label class="commonLabel">商品规格:</label>
            <asp:DropDownList ID="DropDownList_CargoSpec" runat="server" Width="120px">
            </asp:DropDownList>
            &nbsp;&nbsp;
            <label class="commonLabel">商品型号:</label>
            <asp:DropDownList ID="DropDownList_CargoModel" runat="server" Width="120px">
            </asp:DropDownList>
            &nbsp;&nbsp;
            <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
               OnClientClick="showWaitDiv('divWait');" OnClick="btnQuery_Click" Text="查询" />
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
                <asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                <asp:Button ID="btnNCDataImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC商品导入" OnClick="btnNCDataImport_Click" OnClientClick="return AskConfirm();"/>
                    <asp:Button ID="btnNCPriceImport" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="NC价格导入" OnClick="btnNCPriceImport_Click" OnClientClick="return AskConfirm();"/>
                <asp:Button ID="Button2" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    Text="导入商品信息" PostBackUrl="~/BaseInfoSetting/CargoInfoImport.aspx" />
                <asp:Button ID="Button1" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                    Text="导入商品价格" PostBackUrl="~/BaseInfoSetting/CargoPriceImport.aspx" />
                <asp:Button ID="btnShowAddPage" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="新增商品" OnClick="btnShowAddPage_Click" />
                <asp:Button ID="btnShowEditPage" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="编辑商品" OnClick="btnShowEditPage_Click" />
                <asp:Button ID="btnDelete" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="删除商品" OnClick="btnDelete_Click" />
                <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server"
                    TargetControlID="btnDelete" ConfirmText="确定要删除吗？">
                </ajaxToolkit:ConfirmButtonExtender>
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
            <asp:GridView ID="gv_CargoList" Width="1640px" runat="server" AllowPaging="false" ShowHeader="false" AllowSorting="true"  CssClass="linetable" 
                PageSize="20" AutoGenerateColumns="False" OnPageIndexChanging="gv_CargoList_PageIndexChanging"
                OnSorting="gv_CargoList_Sorting" sortExpression="CargoCode" sortDirection="ASC"
                OnDataBound="gv_CargoList_DataBound">
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
                                会员价（金卡）
                            </td>
                            <td>
                                会员价（银卡）
                            </td>
                            <td>
                                会员价（白金卡）
                            </td>
                            <td>
                                参考售价
                            </td>
                            <td>
                                备注
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
                    <asp:TemplateField HeaderText="*">
                        <ItemStyle Width="60px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="javascript:selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>   
                    <asp:TemplateField HeaderText="序号"  InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="50px"/>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>                 
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="180px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName" ItemStyle-Width="300px" />                    
                    <asp:BoundField DataField="CargoType" HeaderText="商品类别" Visible="false" SortExpression="CargoType" ItemStyle-Width="60px"  />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"  ItemStyle-Width="80px"  />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec" ItemStyle-Width="120px" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits" ItemStyle-Width="60px" />
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" SortExpression="ProduceYear" ItemStyle-Width="60px" />
                    <asp:BoundField DataField="Comment" HeaderText="备注" SortExpression="Comment" ItemStyle-Width="300px" />
                    <asp:BoundField DataField="Price1" HeaderText="批发价" SortExpression="Price1" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Price2" HeaderText="会员价（金卡）" SortExpression="Price2" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Price3" HeaderText="会员价（银卡）" SortExpression="Price3" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Price4" HeaderText="会员价（白金卡）" SortExpression="Price4" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Price5" HeaderText="参考售价" SortExpression="Price5" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                    
                    
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align:left">
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
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>

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
