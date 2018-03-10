<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PackageStatisticList.aspx.cs" Inherits="Gold.Query.PackageStatisticList" %>

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

        //清除label的Text
        function clearLabelText(lblID) {
            document.getElementById(lblID).innerText = "";
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
            <table width="500px">
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            仓库:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_WHCode" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td> 
                    <td class="tdRight">
                        <label class="commonLabel">
                            包装:</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_PackageName" runat="server" Width="110px">
                        </asp:DropDownList>
                    </td>
                    <td align="right" >
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                          OnClientClick="showWaitDiv('divWait');"  OnClick="btnQuery_Click" Text="查询" />
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
                <asp:Label ID="lblCheckMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
                <asp:Button ID="btnDelete" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="删除选中项" OnClick="btnDelete_Click" />
            </div>
        </div>
        <div>
            <asp:EntityDataSource ID="EntityDataSource_gv" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableDelete="True" EnableFlattening="False"
                EntitySetName="PackageStatistic" AutoGenerateWhereClause="false"
                Where="it.WHCode like '%'+@WHCode+'%' and it.PackageName like '%'+@PackageName+'%'">
                <WhereParameters>
                    <asp:ControlParameter ControlID="DropDownList_PackageName" Name="PackageName" PropertyName="SelectedItem.Text" DefaultValue="%"
                        Type="String" />
                    <asp:ControlParameter ControlID="DropDownList_WHCode" Name="WHCode" PropertyName="SelectedValue" DefaultValue="%"
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
            <asp:GridView ID="gv_BinList" runat="server" AllowPaging="false" ShowHeader="false" PageSize="15" AllowSorting="True"  CssClass="linetable" 
                Width="1140px" AutoGenerateColumns="False" DataKeyNames="PSID" DataSourceID="EntityDataSource_gv"
                OnPageIndexChanging="gv_BinList_PageIndexChanging" OnRowCommand="gv_BinList_RowCommand"
                OnRowDeleted="gv_BinList_RowDeleted" OnDataBound="gv_BinList_DataBound"
                OnRowDataBound="gv_BinList_RowDataBound">
                <%--OnRowUpdating="gv_BinList_RowUpdating" OnRowUpdated="gv_BinList_RowUpdated"--%>
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <EmptyDataTemplate>
                    <table class="GridViewEmpty_Table" width="100%">
                        <%--<tr class="GridViewEmpty_RowHeader">
                            <td>序号</td>
                            <td>仓库编码</td>
                            <td>仓库名称</td>
                            <td>包装名称</td>
                            <td>总数量</td>
                            <td>箱号(起)</td>
                            <td>箱号(至)</td>
                            <td>更新时间</td>
                            <td>备注</td>
                        </tr>--%>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="9">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="*">
                        <ItemStyle Width="80px"/>
                        <ItemTemplate>
                            <asp:CheckBox ID="gvChk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <input id="CheckAll" type="checkbox" onclick="selectAll(this);" />全选
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="序号" >
                        <ItemStyle HorizontalAlign="Center"  Width="40px"/>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>                                        
                    <asp:TemplateField HeaderText="PSID" SortExpression="PSID" Visible="false">                        
                        <ItemTemplate>
                            <asp:Label ID="lblRowPSID" runat="server" Text='<%# Bind("PSID") %>'></asp:Label>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WHCode" HeaderText="仓库编码" SortExpression="WHCode" ReadOnly="true" ItemStyle-Width="100px" HtmlEncode="true" ApplyFormatInEditMode="true" />
                    <asp:BoundField DataField="WHName" HeaderText="仓库名称" SortExpression="WHName" ReadOnly="true" ItemStyle-Width="120px" />
                    <asp:BoundField DataField="PackageId" HeaderText="包装编码" SortExpression="PackageId" ReadOnly="true" ItemStyle-Width="70px" Visible="false" />
                    <asp:BoundField DataField="PackageName" HeaderText="包装名称" SortExpression="PackageName" ReadOnly="true" ItemStyle-Width="120px" />
                    <asp:TemplateField HeaderText="总数量" SortExpression="PackageTotalCount" ItemStyle-Width="120px">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowPackageTotalCount" runat="server" Text='<%# Bind("PackageTotalCount") %>' Width="40px" MaxLength="10"></asp:TextBox>
                             <label class="commonSaveMsgLabel">
                                *</label>
                           <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowPackageTotalCount" ControlToValidate="tbxRowPackageTotalCount"
                                ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                runat="server" ErrorMessage="总数量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowPackageTotalCount" runat="server" Text='<%# Bind("PackageTotalCount") %>'></asp:Label>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="箱号(起)" SortExpression="PackageNoStart" ItemStyle-Width="100px">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowPackageNoStart" runat="server" Text='<%# Bind("PackageNoStart") %>' Width="40px" MaxLength="32"></asp:TextBox>                            
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowPackageNoStart" runat="server" Text='<%# Bind("PackageNoStart") %>'></asp:Label>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="箱号(止)" SortExpression="PackageNoEnd" ItemStyle-Width="100px">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowPackageNoEnd" runat="server" Text='<%# Bind("PackageNoEnd") %>' Width="40px" MaxLength="32"></asp:TextBox>                            
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowPackageNoEnd"  runat="server" Text='<%# Bind("PackageNoEnd") %>'></asp:Label>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="UpdateTime" HeaderText="更新时间" SortExpression="UpdateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ReadOnly="true" ItemStyle-Width="180px" />
                    <asp:TemplateField HeaderText="备注" SortExpression="Remark" ItemStyle-Width="180px">                    
                        <EditItemTemplate>                        
                            <asp:TextBox ID="tbxRowComment" runat="server" Text='<%# Bind("Remark") %>' Width="120px" MaxLength="100"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowComment" runat="server" Text='<%# Bind("Remark") %>'></asp:Label>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false" HeaderText="编辑" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup" />                            
                            <asp:LinkButton ID="btnNew" Text="删除" Visible="false" CommandName="Delete" CausesValidation="false"
                                runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <%--<asp:LinkButton ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" ValidationGroup="RowEditGroup" />--%>
                            <!--自定义更新按钮和更新事件-->
                            <asp:LinkButton ID="btnUpdate" Text="更新" CommandName="MyDefineUpdate" runat="server"
                                ValidationGroup="RowEditGroup" />
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取消" CommandName="Cancel" CausesValidation="false"
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
                新增包装统计信息</label>
        </div>
        <div class="commonQuery">
            <table style="width:95%;">
                <tr>
                    <td class="tdRight" width="60px">
                        <label class="commonLabel">
                            仓库：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListNewWHCode" runat="server" Width="120px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td> 
                    <td class="tdRight">
                        <label class="commonLabel">
                            包装：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListNewPackage" runat="server" Width="110px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            总数量：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewPackageTotalCount" runat="server" MaxLength="32"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxNewPackageTotalCount" ErrorMessage="包装数量不能为空！"
                            Display="Dynamic" ValidationGroup="BinNewGroup"></asp:RequiredFieldValidator>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            更新时间：</label>
                    </td>
                    <td>
                        <asp:Label ID="lblUpdateTime" runat="server" Text="--"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                                箱号(起)：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewPackageStart" runat="server" MaxLength="32"></asp:TextBox>
                    </td>
                    <td class="tdRight">
                        <label class="commonLabel">
                            箱号(至)：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewPackageEnd" runat="server" MaxLength="32"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdRight">
                        <label class="commonLabel">
                            备注：</label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbxNewRemark" runat="server" Width="99%" MaxLength="200" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnAddNew" runat="server" CssClass="ButtonImageStyle" Text="新增" ValidationGroup="BinNewGroup"
                CausesValidation="true" OnClick="btnAddNew_Click" />
            <asp:Button ID="btnClear" runat="server" CssClass="ButtonImageStyle" Text="清空" OnClick="btnClear_Click"
                CausesValidation="false" />
            <br />
            <asp:Label ID="lblAddMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        </div>
        <br />
    </div>
</asp:Content>
