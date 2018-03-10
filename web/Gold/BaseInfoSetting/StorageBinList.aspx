<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StorageBinList.aspx.cs" Inherits="Gold.BaseInfoSetting.StorageBinList" %>

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
            <asp:Label ID="Label1" CssClass="commonLabel" runat="server" Text="层位编码："></asp:Label>
            <asp:TextBox ID="tbxCode" runat="server"></asp:TextBox>
            <asp:Label ID="lblName" CssClass="commonLabel" runat="server" Text="层位名称："></asp:Label>
            <asp:TextBox ID="tbxName" runat="server"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                OnClick="btnQuery_Click" Text="查询" />
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
                <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnDelete" runat="server"
                    TargetControlID="btnDelete" ConfirmText="确定要删除吗？">
                </ajaxToolkit:ConfirmButtonExtender>
            </div>
        </div>
        <div>
            <asp:EntityDataSource ID="EntityDataSource_gv" runat="server" ConnectionString="name=GoldEntities"
                DefaultContainerName="GoldEntities" EnableDelete="True" EnableFlattening="False"
                EntitySetName="StorageBin" AutoGenerateWhereClause="false" Include="WareHouse1"
                Where="it.BinCode like '%'+@BinCode+'%' and it.BinName like '%'+@BinName+'%'">
                <WhereParameters>
                    <asp:ControlParameter ControlID="tbxName" Name="BinName" PropertyName="Text" DefaultValue="%"
                        Type="String" />
                    <asp:ControlParameter ControlID="tbxCode" Name="BinCode" PropertyName="Text" DefaultValue="%"
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
                onscroll="javascript:RecordPostion(this);">
                <!--onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;-->
                <asp:GridView ID="gv_BinList" runat="server" AllowPaging="false" ShowHeader="false"
                    PageSize="15" AllowSorting="True" CssClass="linetable" Width="1100px" AutoGenerateColumns="False"
                    DataKeyNames="BinCode" DataSourceID="EntityDataSource_gv" OnPageIndexChanging="gv_BinList_PageIndexChanging"
                    OnRowCommand="gv_BinList_RowCommand" OnRowDeleted="gv_BinList_RowDeleted" OnDataBound="gv_BinList_DataBound"
                    OnRowDataBound="gv_BinList_RowDataBound">
                    <%--OnRowUpdating="gv_BinList_RowUpdating" OnRowUpdated="gv_BinList_RowUpdated"--%>
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <PagerStyle CssClass="GridViewPagerStyle" />
                    <EmptyDataTemplate>
                        <table class="GridViewEmpty_Table">
                            <%--<tr class="GridViewEmpty_RowHeader">
                            <td>层位编码</td>
                            <td>层位名称</td>
                            <td>层位类型</td>
                            <td>所属仓库</td>
                            <td>层位电子标签编码</td>
                            <td>标签状态</td>
                            <td>层位备注</td>
                            <td>标签测试按钮</td>
                        </tr>--%>
                            <tr class="GridViewEmpty_RowData">
                                <td colspan="8">
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
                        <asp:TemplateField HeaderText="层位编号" SortExpression="BinCode" ItemStyle-Width="60px">
                            <ItemTemplate>
                                <asp:Label ID="lblBinCode" runat="server" Text='<%# Eval("BinCode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="层位名称" SortExpression="BinName" ItemStyle-Width="80px"
                            ItemStyle-VerticalAlign="Top">
                            <EditItemTemplate>
                                <asp:TextBox ID="tbxRowBinName" runat="server" Text='<%# Bind("BinName") %>' MaxLength="50"
                                    Width="50px"></asp:TextBox>
                                <br />
                                <label class="commonSaveMsgLabel">
                                    *</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowBinName" ControlToValidate="tbxRowBinName"
                                    ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                    runat="server" ErrorMessage="层位名称不能为空"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowBinName" runat="server" Text='<%# Bind("BinName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="层位类型" SortExpression="BinType" ItemStyle-Width="120px"
                            ItemStyle-VerticalAlign="Top">
                            <EditItemTemplate>
                                <asp:Label ID="lblRowBinTypeEdit" Visible="false" runat="server" Text='<%# Bind("BinType") %>'></asp:Label>
                                <asp:DropDownList ID="dropdownList_binType" runat="server" Width="90px">
                                    <%--<asp:ListItem Value="1" Text="实体层位"></asp:ListItem>
                                <asp:ListItem Value="0" Text="虚拟层位"></asp:ListItem>--%>
                                </asp:DropDownList>
                                <br />
                                <label class="commonSaveMsgLabel">
                                    *</label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowBinType" Visible="false" runat="server" Text='<%# Bind("BinType") %>'></asp:Label>
                                <asp:Label ID="lblRowBinTypeName" runat="server" Text=""></asp:Label><!--用于显示层位类型名称-->
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="所属仓库" SortExpression="WareHouse" ItemStyle-Width="100px"
                            ItemStyle-VerticalAlign="Top">
                            <EditItemTemplate>
                                <asp:Label ID="lblRowWareHouseCodeEdit" Visible="false" runat="server" Text='<%# Bind("WareHouse") %>'></asp:Label>
                                <asp:DropDownList ID="dropdownList_WareHouse" runat="server" Width="80px">
                                </asp:DropDownList>
                                <br />
                                <label class="commonSaveMsgLabel">
                                    *</label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowWareHouseName" runat="server" Text='<%# Bind("WareHouse1.WHName") %>'></asp:Label>
                                <asp:Label ID="lblRowWareHouseCode" Visible="false" runat="server" Text='<%# Bind("WareHouse") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="层位电子标签编码" SortExpression="BinTagID" ItemStyle-Width="120px"
                            ItemStyle-VerticalAlign="Top">
                            <EditItemTemplate>
                                <asp:TextBox ID="tbxRowBinTagID" runat="server" Text='<%# Bind("BinTagID") %>' MaxLength="32"
                                    Width="100px"></asp:TextBox>
                                <br />
                                <label class="commonSaveMsgLabel">
                                    *</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowBinTagID" ControlToValidate="tbxRowBinTagID"
                                    ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                    runat="server" ErrorMessage="层位电子标签编码不能为空"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowBinTagID" Style="word-break: break-all; word-wrap: break-word;"
                                    runat="server" Text='<%# Bind("BinTagID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="标签状态" SortExpression="BinTagStatus" ItemStyle-Width="80px">
                            <EditItemTemplate>
                                <asp:Label ID="lblRowBinTagStatusEdit" Visible="false" runat="server" Text='<%# Bind("BinTagStatus") %>'></asp:Label>
                                <asp:DropDownList ID="dropdownList_BinTagStatus" runat="server" Width="70px">
                                    <%--<asp:ListItem Value="1" Text="正常"></asp:ListItem>
                                <asp:ListItem Value="0" Text="异常"></asp:ListItem>--%>
                                </asp:DropDownList>
                                <br />
                                <label class="commonSaveMsgLabel">
                                    *</label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowBinTagStatus" Visible="false" runat="server" Text='<%# Bind("BinTagStatus") %>'></asp:Label>
                                <asp:Label ID="lblRowBinTagStatusName" runat="server" Text=""></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="层位备注" SortExpression="Comment" ItemStyle-Width="100px"
                            ItemStyle-VerticalAlign="Top">
                            <EditItemTemplate>
                                <asp:TextBox ID="tbxRowComment" runat="server" Text='<%# Bind("Comment") %>' MaxLength="100"
                                    Width="80px"></asp:TextBox>
                                <label class="commonSaveMsgLabel">
                                    *</label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRowComment" runat="server" Text='<%# Bind("Comment") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px">
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
                        <asp:TemplateField HeaderText="标签测试" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="210px">
                            <ItemTemplate>
                                <asp:Button ID="btnRowTagTestAlarmStart" Text="报警测试" CssClass="ButtonImageStyle"
                                    CommandName="TagAlarmStart" CausesValidation="false" runat="server" />
                                <asp:Button ID="btnRowTagTestAlarmStop" Text="停止报警" CssClass="ButtonImageStyle" CommandName="TagAlarmStop"
                                    CausesValidation="false" runat="server" />
                                <asp:Label ID="lblRowTagTestShortMsg" CssClass="commonSaveMsgLabel" runat="server"
                                    Text=""></asp:Label>
                                <div id='<%# "waitDiv_"+ Eval("BinCode").ToString() %>' style="display: none;">
                                    <img src="../Styles/images/uploading.gif" /><label>执行中……</label></div>
                            </ItemTemplate>
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
                新增层位信息</label>
        </div>
        <div class="commonQuery">
            <table>
                <tr>
                    <td>
                        <label class="commonLabel">
                            层位编号：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewBinCode" runat="server" MaxLength="20"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxNewBinCode" ErrorMessage="层位编号不能为空！" Display="Dynamic"
                            ValidationGroup="BinNewGroup"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <label class="commonLabel">
                            层位名称：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewBinName" runat="server" MaxLength="50"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxNewBinName" ErrorMessage="层位名称不能为空！" Display="Dynamic"
                            ValidationGroup="BinNewGroup"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            层位类型：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="dropdownList_NewBinType" runat="server" Width="150px">
                            <asp:ListItem Value="1" Text="实体层位"></asp:ListItem>
                            <asp:ListItem Value="0" Text="虚拟层位"></asp:ListItem>
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                    <td>
                        <label class="commonLabel">
                            所属仓库：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="dropdownList_NewWareHouse" runat="server" Width="150px">
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            层位电子标签编码：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewBinTagID" runat="server" MaxLength="32"></asp:TextBox>
                        <label class="commonSaveMsgLabel">
                            *</label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="commonSaveMsgLabel"
                            runat="server" ControlToValidate="tbxNewBinTagID" ErrorMessage="层位电子标签编码不能为空！"
                            Display="Dynamic" ValidationGroup="BinNewGroup"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <label class="commonLabel">
                            层位电子标签状态：</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="dropdownList_NewBinTagStatus" runat="server" Width="150px">
                            <asp:ListItem Value="1" Text="正常"></asp:ListItem>
                            <asp:ListItem Value="0" Text="异常"></asp:ListItem>
                        </asp:DropDownList>
                        <label class="commonSaveMsgLabel">
                            *</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="commonLabel">
                            层位备注：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbxNewComment" runat="server" MaxLength="100"></asp:TextBox>
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
