<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryBookQtyGroup.aspx.cs" Inherits="Gold.Query.InventoryBookQtyGroup" EnableEventValidation="false"%>
<%@ Register TagPrefix="uc" TagName="GoodsSelectSingle" Src="~/Controls/GoodsSelectSingle.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script src="../Scripts/MsgBoxJScript.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">

    //显示等待滚动图片
    function showWaitDiv(divName) {
        document.getElementById(divName).style.display = "block";
    }
    //隐藏等待滚动图片
    function hiddenWaitDiv(divName) {
        document.getElementById(divName).style.display = "none";
    }

    var styleToSelect;
    function onOk() {
        $get('Paragraph1').className = styleToSelect;
    }

    // Add click handlers for buttons to show and hide modal popup on pageLoad
    function pageLoad() {
        $addHandler($get("showModalPopupClientButton_Msg"), 'click', showModalPopupViaClient);
        $addHandler($get("hideModalPopupViaClientButton_Msg"), 'click', hideModalPopupViaClient);
//        $addHandler($get("btnOpenPopClient"), 'click', showModalPopupViaClient_Goods);
//        $addHandler($get("btnClosePopClient"), 'click', hideModalPopupViaClient_Goods);
    }

    function showModalPopupViaClient(ev) {
        ev.preventDefault();
        var modalPopupBehavior = $find('programmaticModalPopupBehavior_Msg');
        modalPopupBehavior.show();
    }

    function hideModalPopupViaClient(ev) {
        ev.preventDefault();
        var modalPopupBehavior = $find('programmaticModalPopupBehavior_Msg');
        modalPopupBehavior.hide();
    }

    function showModalPopupViaClient_Goods(ev) {
        ev.preventDefault();
        var modalPopupBehavior = $find('popWindow_Goods');
        modalPopupBehavior.show();
    }

    function hideModalPopupViaClient_Goods(ev) {
        ev.preventDefault();
        var modalPopupBehavior = $find('popWindow_Goods');
        modalPopupBehavior.hide();
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="lbtnExport" />                    
                    <%--<asp:PostBackTrigger ControlID="btnSelectCargo2" />
                    <asp:PostBackTrigger ControlID="btnClosePop" />--%>
                    <%--<asp:PostBackTrigger ControlID="popWindow" />--%>
                </Triggers>
                <ContentTemplate>

    <%--<asp:EntityDataSource ID="edsWarehouse" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="WareHouse"
        Select="it.[WHCode], it.[WHName]">
    </asp:EntityDataSource>

    <asp:EntityDataSource ID="edsInventoryBook" runat="server" 
        ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
        EnableFlattening="False" EntitySetName="InventoryBook" OrderBy="it.WHCode,it.CargoCode,it.ChangeTime,it.TableAccountID"
        onquerycreated="edsInventoryBook_QueryCreated">
    </asp:EntityDataSource>--%>

    <div class="box">
        <h2>
            <span>查询条件</span>&nbsp;&nbsp;
        </h2>
        <div class="boxContent">
         <table>
                <tr>
                    <td style="width: 70px; text-align: right">
                        商品名称：</td>
                    <td > <%--style="width:200px;"--%>
                        <asp:TextBox ID="txtCargoName" runat="server" MaxLength="100" Width="187px" />
                        <asp:Button runat="server" ID="btnSelectCargo2" Text="..." OnClick="SelectCargo_Click" Width="30" OnClientClick="showWaitDiv('divWait');" />
                        <%--<input type="button" value="query" id="btnOpenPopClient" />--%>
                    </td>
                    <td style="width: 70px; text-align: right">
                        商品编码：
                    </td>
                    <td style="width: 130px">
                        <asp:TextBox ID="txtCargoCode" runat="server" MaxLength="32" Width="150px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        台帐日期：
                    </td>
                    <td style="width: 210px;">
                        <asp:TextBox ID="txtStartDate" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server"
                            TargetControlID="txtStartDate" >
                        </ajaxToolkit:CalendarExtender>
                        至
                        <asp:TextBox ID="txtEndDate" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" Enabled="True"
                            TargetControlID="txtEndDate">
                        </ajaxToolkit:CalendarExtender>
                    </td>
                    <td>
                        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red" />
                    </td>
                </tr>
                <tr>
                    
                    <td style="width: 70px; text-align: right">
                        商品型号：</td>
                    <td >
                        <asp:TextBox ID="txtCargoModel" runat="server" MaxLength="32" Width="187px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        商品规格：
                    </td>
                    <td style="width: 130px">
                        <asp:TextBox ID="txtCargoSpec" runat="server" MaxLength="32" Width="150px" />
                    </td>
                    <td style="width: 70px; text-align: right">仓库：</td>
                    <td style="width: 160px">
                        <%--<asp:DropDownList ID="drpWHCode" runat="server" Width="192px"
                            DataSourceID="edsWarehouse" DataTextField="WHName" DataValueField="WHCode"
                            AppendDataBoundItems="true" >
                        </asp:DropDownList>--%>
                        <asp:DropDownList ID="drpWHCode" runat="server" Width="192px">
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="Button1" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                           OnClientClick="showWaitDiv('divWait');" Text="查 询"  Height="21px" Width="81px" onclick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="commonPage">
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
            <asp:LinkButton ID="lbtnExport" runat="server" Text="导出Excel" 
                onclick="lbtnExport_Click" />        
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
        <div style="width: 99%; height: 500px; overflow-y:scroll;" class="divScroll" id="gridviewContainer"
            onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;"><!--DataSourceID="edsInventoryBook"-->
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                HorizontalAlign="Center" AllowPaging="false" ShowHeader="false"
                DataKeyNames="TableAccountID,CargoCode"  
                CssClass="linetable" Width="1300px" sortExpression="CargoCode" sortDirection="ASC"
                 ShowFooter="true" onrowdatabound="GridView1_RowDataBound" 
                ondatabound="GridView1_DataBound" onsorting="GridView1_Sorting">
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
                <td>仓库编码</td>
                <td>商品名称</td>
                <td>商品编码</td>
                <td>型号</td>
                <td>规格</td>
                <td>日期</td>
                <td>计量单位</td>
                <td>发行年份</td>
                <td>用友单据号</td>
                <td>摘要</td>
                <td>期初数量</td>
                <td>收入数量</td>
                <td>发出数量</td>
                <td>结余数量</td>
                <td>记账人</td>
                </tr>--%>
                 <tr class ="GridViewEmpty_RowData">
                 <td colspan="14" >无数据</td>
                 </tr>
                </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="序号" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" >
                      <ItemTemplate>
                         <%#  Container.DataItemIndex+1 %> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHCode" HeaderText="仓库" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px" 
                        SortExpression="WHCode" />
                    <asp:BoundField DataField="WHName" HeaderText="仓库名" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px" 
                        SortExpression="WHName" />
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" 
                        SortExpression="CargoName" />
                    <asp:BoundField DataField="CargoCode" HeaderText="商品编码" ReadOnly="True" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center"
                        SortExpression="CargoCode" ItemStyle-Width="140px" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                        SortExpression="CargoModel" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px" 
                        SortExpression="CargoSpec" />
                    <asp:BoundField DataField="ChangeTime" HeaderText="日期" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="90px" 
                        SortExpression="ChangeTime" />
                    <asp:BoundField DataField="JoinNCOrder" HeaderText="用友单据号" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="90px" 
                        SortExpression="JoinNCOrder" />
                     <asp:BoundField DataField="StockBillNo" HeaderText="RFID单据" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="90px" 
                        SortExpression="ReleaseYear" />
                     <asp:BoundField DataField="CargoUnits" HeaderText="单位" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                        SortExpression="CargoUnits" /> 
                    <asp:BoundField DataField="NumOriginal" HeaderText="期初数量" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="65px" 
                        SortExpression="NumOriginal" />
                    <asp:BoundField DataField="NumAdd" HeaderText="收入数量" SortExpression="NumAdd" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="65px"  />
                    <asp:BoundField DataField="NumDel" HeaderText="发出数量" SortExpression="NumDel" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="65px"  />
                    <asp:BoundField DataField="NumCurrent" HeaderText="结余数量" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="65px" 
                        SortExpression="NumCurrent" />
                    <asp:BoundField DataField="ChangePerson" HeaderText="记账人" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="70px" 
                        SortExpression="ChangePerson" />
                        <asp:BoundField DataField="Remark" HeaderText="摘要" SortExpression="Remark" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="90px"  />
                </Columns>
            </asp:GridView>


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

        <!--商品单选框-->

        <a id="btnOpenPopClientTest" href="#" style="display:none;">on the client in script</a>
        <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupCargo">
            <div class="modalPopupWrapperCargo">
                <div id="pnlDragTitle" class="modalHeader">
                    <span>选择商品</span>
                    <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="关  闭" OnClientClick="showWaitDiv('divWait');"
                        OnClick="btnClosePop_Click" />
                        <%--<input type="button" id="btnClosePopClient" class="ClosePopButton_Msg"
                        value="关闭client" />--%>
                </div>
                <div class="modalBody">
                    <uc:GoodsSelectSingle ID="GoodsSelect1" runat="server" OnGetCargoSelect="GoodsSelect1_GetCargoSelect" />
                </div>
            </div>
        </asp:Panel>
        <asp:LinkButton ID="LinkButton1NULL" runat="server" Width="0" Height="0"></asp:LinkButton>            
        <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="LinkButton1NULL" BehaviorID="popWindow_Goods"
            PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
            PopupDragHandleControlID="pnlDragTitle">
        </ajaxToolkit:ModalPopupExtender>

            </div>


     </div>
    

        </ContentTemplate>
    </asp:UpdatePanel>
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
