<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaleAllocationSet.aspx.cs" Inherits="Gold.SaleCargoSetting.SaleAllocationSet" %>

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

        //检查输入的分配量是否为数字且是否各部门分配量超过结存数量
        function NumberConfirm(rowIndex) {
            var t = document.getElementById("<%=gv_SaleAllocationList.ClientID%>")
            var jiecunColIndex = 4; //9;
            var qudaoColIndex = 5; //10;
            var xiaoshouColIndex = 6; //11;
            var lingshouColIndex = 7//12;
            var dianziColIndex = 8; //13;
            var gongyongColIndex = 9; // ;14;
            var jidongColIndex = 10; // 15;


            //从第一行开始遍历（第0行是表头）
            //for (i = 1; i < t.rows.length; i++) 
            //{
            //if (i = rowIndex+1) 
            //{
            var i = rowIndex; // + 1;//这里去掉加1，因为表头被去掉了
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

//        //测试导致UpdatePanel变慢的原因
//        function AlertTime(typecode)
//        {
//            var myDate = new Date();
//            var totalms= myDate.getTime();
//            var h= myDate.getHours(); //获取当前小时数(0-23)   
//            var m= myDate.getMinutes(); //获取当前分钟数(0-59)   
//            var s= myDate.getSeconds(); //获取当前秒数(0-59)   
//            var ms= myDate.getMilliseconds(); //获取当前毫秒数(0-999)
//            var timeName =  h + ":" + m + ":" + s + ":" + ms;
//                        
//            if (typecode == 1) {
//                document.getElementById("MainContent_TextBox1").value = timeName;
//                document.getElementById("MainContent_TextBox3").value = totalms;
//            }
//            else
//            {
//                document.getElementById("MainContent_TextBox2").value = timeName;
//                document.getElementById("MainContent_TextBox4").value = totalms;

//                var start = document.getElementById("MainContent_TextBox3").value;
//                var stop = document.getElementById("MainContent_TextBox4").value;
//                document.getElementById("MainContent_TextBox5").value = stop - start;
//            }
//            //alert(timeName);
        //        }

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
            <table width="700px">
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
                    <td>
                    </td>
                    <td>
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
                    <%--<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>--%>
                    </td>
                    <td>
                    <%--<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>--%>
                    </td>
                    <td>
                    <%--<asp:TextBox ID="TextBox5" runat="server"></asp:TextBox>--%>
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
            onscroll="javascript:RecordPostion(this); "><!--onscroll="MainContent_divHeader.scrollLeft=this.scrollLeft;-->
            <asp:GridView ID="gv_SaleAllocationList" runat="server" AllowPaging="false" ShowHeader="false"
                AllowSorting="True" CssClass="linetable" Width="1440px" AutoGenerateColumns="False"
                DataKeyNames="CargoCode" OnPageIndexChanging="gv_SaleAllocationList_PageIndexChanging"
                OnRowCommand="gv_SaleAllocationList_RowCommand" OnDataBound="gv_SaleAllocationList_DataBound"
                OnRowDataBound="gv_SaleAllocationList_RowDataBound" sortDirection="ASC" sortExpression="CargoCode"
                OnSorting="gv_SaleAllocationList_Sorting" OnRowEditing="gv_SaleAllocationList_RowEditing"
                OnRowCancelingEdit="gv_SaleAllocationList_RowCancelingEdit">
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
                                渠道部 分配量
                            </td>
                            <td>
                                销售部 分配量
                            </td>
                            <td>
                                零售中心 分配量
                            </td>
                            <td>
                                电子商务 分配量
                            </td>
                            <td>
                                公用部门 分配量
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
                        <ItemStyle HorizontalAlign="Center" Width="35px" />
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="130px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" SortExpression="CargoName"
                        ReadOnly="true" ItemStyle-Width="290px"></asp:BoundField>
                    <asp:BoundField DataField="CargoType" HeaderText="商品类别" Visible="false" SortExpression="CargoType"
                        ReadOnly="true" ItemStyle-Width="60px"></asp:BoundField>
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" SortExpression="CargoUnits"
                        ReadOnly="true" ItemStyle-Width="35px"></asp:BoundField>
                    <asp:TemplateField HeaderText="结存数量" SortExpression="Total" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowTotal" runat="server" Text='<%# Eval("Total") %>' MaxLength="32"
                                Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowTotal" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("Total") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="渠道部    分配量" SortExpression="DeptChannelAllocation"
                        ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowDeptChannelAllocation" runat="server" Text='<%# Eval("DeptChannelAllocation") %>'
                                MaxLength="50" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowDeptChannelAllocation"
                                ControlToValidate="tbxRowDeptChannelAllocation" ValidationGroup="RowEditGroup"
                                Display="Dynamic" CssClass="commonSaveMsgLabel" runat="server" ErrorMessage="渠道部分配量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptChannelAllocation" runat="server" Text='<%# Eval("DeptChannelAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="销售部    分配量" SortExpression="DeptSaleAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowDeptSaleAllocation" runat="server" Text='<%# Eval("DeptSaleAllocation") %>'
                                MaxLength="32" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowDeptSaleAllocation"
                                ControlToValidate="tbxRowDeptSaleAllocation" ValidationGroup="RowEditGroup" Display="Dynamic"
                                CssClass="commonSaveMsgLabel" runat="server" ErrorMessage="销售部分配量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptSaleAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptSaleAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="零售中心分配量" SortExpression="DeptRetailAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowDeptRetailAllocation" runat="server" Text='<%# Eval("DeptRetailAllocation") %>'
                                MaxLength="100" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowDeptRetailAllocation"
                                ControlToValidate="tbxRowDeptRetailAllocation" ValidationGroup="RowEditGroup"
                                Display="Dynamic" CssClass="commonSaveMsgLabel" runat="server" ErrorMessage="零售中心分配量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowComment" runat="server" Text='<%# Eval("DeptRetailAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="电子商务分配量" SortExpression="DeptECAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowDeptECAllocation" runat="server" Text='<%# Eval("DeptECAllocation") %>'
                                MaxLength="32" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowDeptECAllocation" ControlToValidate="tbxRowDeptECAllocation"
                                ValidationGroup="RowEditGroup" Display="Dynamic" CssClass="commonSaveMsgLabel"
                                runat="server" ErrorMessage="电子商务分配量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptECAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptECAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="公用部门分配量" SortExpression="DeptPublicAllocation" ItemStyle-Width="70px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowDeptPublicAllocation" runat="server" Text='<%# Eval("DeptPublicAllocation") %>'
                                MaxLength="32" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_tbxRowDeptPublicAllocation"
                                ControlToValidate="tbxRowDeptPublicAllocation" ValidationGroup="RowEditGroup"
                                Display="Dynamic" CssClass="commonSaveMsgLabel" runat="server" ErrorMessage="公用部门分配量不能为空"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowDeptPublicAllocation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("DeptPublicAllocation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="机动量" SortExpression="Variation" ItemStyle-Width="60px"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle">
                        <EditItemTemplate>
                            <asp:TextBox ID="tbxRowVariation" runat="server" Text='<%# Eval("Variation") %>'
                                MaxLength="32" Width="30px"></asp:TextBox>
                            <br />
                            <label class="commonSaveMsgLabel">
                                *</label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblRowVariation" Style="word-break: break-all; word-wrap: break-word;"
                                runat="server" Text='<%# Eval("Variation") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="分配量编辑" ShowHeader="false" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Width="80px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" Text="编辑" CommandName="Edit" runat="server" ValidationGroup="RowEditGroup"
                                OnClientClick="showWaitDiv('divWait');" />
                            <asp:LinkButton ID="btnNew" Text="删除" Visible="false" CommandName="Delete" CausesValidation="false"
                                OnClientClick="showWaitDiv('divWait');" runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <%--<asp:LinkButton ID="btnUpdate" Text="更 新" CommandName="Update" runat="server" ValidationGroup="RowEditGroup" />--%>
                            <!--自定义更新按钮和更新事件-->
                            <asp:LinkButton ID="btnUpdate" Text="更新" CommandName="MyDefineUpdate" runat="server"
                                ValidationGroup="RowEditGroup" OnClientClick="showWaitDiv('divWait'); return NumberConfirm('0','0')" />
                            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender_btnUpdate" Enabled="false"
                                TargetControlID="btnUpdate" runat="server" ConfirmText="确定要更新吗？">
                            </ajaxToolkit:ConfirmButtonExtender>
                            &nbsp;
                            <asp:LinkButton ID="btnCancel" Text="取消" CommandName="Cancel" CausesValidation="false"
                                runat="server" OnClientClick="showWaitDiv('divWait');" />
                            <asp:Label runat="server" ID="lblUpdateRowMsg" Text=""></asp:Label>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" SortExpression="CargoModel"
                        ReadOnly="true" ItemStyle-Width="50px"></asp:BoundField>
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" SortExpression="CargoSpec"
                        ReadOnly="true" ItemStyle-Width="60px"></asp:BoundField>
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" SortExpression="ProduceYear"
                        ReadOnly="true" ItemStyle-Width="80px"></asp:BoundField>
                    <asp:BoundField DataField="Price1" HeaderText="批发价" SortExpression="Price1" ItemStyle-Width="80px"
                        ReadOnly="true" ItemStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Price5" HeaderText="参考售价" SortExpression="Price5" ItemStyle-Width="80px"
                        ReadOnly="true" ItemStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
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
