<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PackagesList.aspx.cs" Inherits="Gold.BaseInfoSetting.PackagesList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
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
                查询条件</label>
        </div>
        <div class="commonQuery">
            <asp:Label ID="lblName" CssClass="commonLabel" runat="server" Text="包装名称："></asp:Label>
            <asp:TextBox ID="tbxName" runat="server"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                OnClick="btnQuery_Click" Text="查询" />
        </div>
        <div class="commonTitle">
            <label>
                查询结果</label>
        </div>
        <div>
            <asp:EntityDataSource ID="EntityDataSource_gv" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableDelete="True" EnableFlattening="False"
                EnableInsert="True" EnableUpdate="True" EntitySetName="Packages" AutoGenerateWhereClause="false"
                Where="it.PackageName like '%'+@PackageName+'%'">
                <WhereParameters>
                    <asp:ControlParameter ControlID="tbxName" Name="PackageName" PropertyName="Text" DefaultValue="%"
                        Type="String" />
                </WhereParameters>
            </asp:EntityDataSource>
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
            <asp:GridView ID="gv_PackageList" runat="server" AllowPaging="false" ShowHeader="false" PageSize="15" AllowSorting="True" Width="650px"
                CssClass="linetable" AutoGenerateColumns="False" DataKeyNames="PackageId" DataSourceID="EntityDataSource_gv"
                OnPageIndexChanging="gv_PackageList_PageIndexChanging" OnRowCommand="gv_PackageList_RowCommand"
                OnRowDeleted="gv_PackageList_RowDeleted" OnRowUpdated="gv_PackageList_RowUpdated"
                OnRowUpdating="gv_PackageList_RowUpdating" OnDataBound="gv_PackageList_DataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table">
                        <tr class="GridViewEmpty_RowHeader">
                            <td>
                                包装名称
                            </td>
                            <td>
                                备注
                            </td>
                        </tr>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="2">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PackageId" HeaderText="包装编号" SortExpression="PackageId" Visible="false"
                        ReadOnly="True" />
                    <asp:TemplateField HeaderText="包装名称" SortExpression="PackageName" ItemStyle-Width="300px"
                        ItemStyle-VerticalAlign="Top">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowPackageName" runat="server" Text='<%# Bind("PackageName") %>'
                                MaxLength="50"></asp:TextBox>
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowPackageName" ControlToValidate="tbxRowPackageName"
                                ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                runat="server" ErrorMessage="包装名称不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowPackageName" runat="server" Text='<%# Bind("PackageName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="备注" SortExpression="Comment" ItemStyle-Width="200px"
                        ItemStyle-VerticalAlign="Top">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowComment" runat="server" Text='<%# Bind("Comment") %>' MaxLength="100"></asp:TextBox>
                            <label class="commonSaveMsgLabel">
                                *</label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowComment" runat="server" Text='<%# Bind("Comment") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编 辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnNew" Text="删 除" CommandName="Delete" CausesValidation="false"
                                runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取 消" CommandName="Cancel" CausesValidation="false"
                                runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <table width="100%">
                        <tr>
                            <td style="text-align: right">
                                <span class="GridViewPager_PageNumberAndCountLabel">第<asp:Label ID="lblPageIndex"
                                    runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1   %>' />页
                                    &nbsp; 共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount   %>' />页
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
            <br />
            <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
        <br />
        <div class="commonTitle">
            <label>
                新增包装信息</label>
        </div>
        <div class="commonQuery">
            <label class="commonLabel">
                包装名称：</label>
            <asp:TextBox ID="tbxNewPackageName" runat="server" MaxLength="50"></asp:TextBox>
            <label class="commonSaveMsgLabel">
                *</label>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="commonSaveMsgLabel"
                runat="server" ControlToValidate="tbxNewPackageName" ErrorMessage="型号名称不能为空！" Display="Dynamic"
                ValidationGroup="PackageNewGroup"></asp:RequiredFieldValidator>
            <span style="width: 100px;">&nbsp;</span>
            <label class="commonLabel">
                型号备注：</label>
            <asp:TextBox ID="tbxNewComment" runat="server" MaxLength="100"></asp:TextBox>
            <asp:Button ID="btnAddNew" runat="server" CssClass="ButtonImageStyle" Text="新增" ValidationGroup="PackageNewGroup"
                CausesValidation="true" OnClick="btnAddNew_Click" />
            <asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" Text="清空" OnClick="btnClear_Click"
                CausesValidation="false" />
            <asp:Label ID="lblAddMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
        <br />
    </div>
</asp:Content>
