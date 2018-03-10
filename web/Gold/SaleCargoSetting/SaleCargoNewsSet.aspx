<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaleCargoNewsSet.aspx.cs" Inherits="Gold.SaleCargoSetting.SaleCargoNewsSet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                可售商品信息发布</label>
        </div>
        <div class="commonQuery">
            <table width="95%">
                <tr>
                    <td width="80px">
                        <label class="commonLabel">
                            标题:</label><label class="commonSaveMsgLabel">
                                *</label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbxTitle" runat="server" Width="99%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td  width="80px" style="vertical-align: top;">
                        <label class="commonLabel">
                            内容:</label><label class="commonSaveMsgLabel">
                                *</label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbxContent" runat="server" TextMode="MultiLine" Width="99%" Height="125px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td  width="80px">
                        <label class="commonLabel">
                            发布人:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxEditor" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                    <td>
                        <label class="commonLabel">
                            发布时间:</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxDate" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="3">
                    <br />
                        <asp:Button ID="btnSave" runat="server" CausesValidation="false" CssClass="ButtonMidWidthImageStyle"
                            Text="发布" OnClick="btnSave_Click" />
                        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <br />
        </div>
        <div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    历史信息记录</label>
            </div>
            <div class="CommonTitle_InnerdivRight">
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
        <asp:GridView ID="gv_CargoList" runat="server" AllowPaging="false" AllowSorting="True" ShowHeader="false" Width="1140px"
            CssClass="linetable" PageSize="20" AutoGenerateColumns="False" OnPageIndexChanging="gv_CargoList_PageIndexChanging"
            OnSorting="gv_CargoList_Sorting" sortExpression="NewsID" sortDirection="ASC"
            OnDataBound="gv_CargoList_DataBound">
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
            <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <RowStyle CssClass="GridViewRowStyle" />
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <PagerStyle CssClass="GridViewPagerStyle" />
            <EmptyDataTemplate>
                <table class="GridViewEmpty_Table">                    
                    <tr class="GridViewEmpty_RowData">
                        <td colspan="12">
                            无数据
                        </td>
                    </tr>
                </table>
                <!--
                    dt.Columns.Add("NewsID", typeof(string));//消息ID
            dt.Columns.Add("NewsCreateDate", typeof(DateTime));//消息发布时间
            dt.Columns.Add("NewsTitle", typeof(string));//消息标题
            dt.Columns.Add("NewsContent", typeof(string));//消息内容
            dt.Columns.Add("EditorID", typeof(string));//消息发布人ID
            dt.Columns.Add("EditorName", typeof(string));//消息发布人姓名
                    
                    -->
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField HeaderText="序号" InsertVisible="False">
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%#Container.DataItemIndex+1%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NewsID" HeaderText="NewsID" Visible="false"></asp:BoundField>
                <asp:BoundField DataField="NewsCreateDate" HeaderText="发布时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="140px">
                </asp:BoundField>
                <asp:BoundField DataField="NewsTitle" HeaderText="信息标题" ItemStyle-Width="200px">
                </asp:BoundField>
                <asp:BoundField DataField="NewsContent" HeaderText="信息内容" ItemStyle-Width="640px">
                </asp:BoundField>
                <asp:BoundField DataField="EditorID" HeaderText="发布人ID" ItemStyle-Width="60px"></asp:BoundField>
                <asp:BoundField DataField="EditorName" HeaderText="发布人" ItemStyle-Width="60px">
                </asp:BoundField>
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
