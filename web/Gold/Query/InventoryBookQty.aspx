<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" 
    AutoEventWireup="true" CodeBehind="InventoryBookQty.aspx.cs" Inherits="Gold.Query.InventoryBookQty" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

<script type="text/javascript">
    var cal1;

    function pageLoad() {
        cal1 = $find("calendar1");
        modifyCalDelegates(cal1);
    }

    function modifyCalDelegates(cal) {
        //we need to modify the original delegate of the month cell.
        cal._cell$delegates = {
            mouseover: Function.createDelegate(cal, cal._cell_onmouseover),
            mouseout: Function.createDelegate(cal, cal._cell_onmouseout),

            click: Function.createDelegate(cal, function (e) {
                /// <summary>
                /// Handles the click event of a cell
                /// </summary>
                /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

                e.stopPropagation();
                e.preventDefault();

                if (!cal._enabled) return;

                var target = e.target;
                var visibleDate = cal._getEffectiveVisibleDate();
                Sys.UI.DomElement.removeCssClass(target.parentNode, "ajax__calendar_hover");
                switch (target.mode) {
                    case "prev":
                    case "next":
                        cal._switchMonth(target.date);
                        break;
                    case "title":
                        switch (cal._mode) {
                            case "days": cal._switchMode("months"); break;
                            case "months": cal._switchMode("years"); break;
                        }
                        break;
                    case "month":
                        //if the mode is month, then stop switching to day mode.
                        if (target.month == visibleDate.getMonth()) {
                            //this._switchMode("days");
                        } else {
                            cal._visibleDate = target.date;
                            //this._switchMode("days");
                        }
                        cal.set_selectedDate(target.date);
                        cal._switchMonth(target.date);
                        cal._blur.post(true);
                        cal.raiseDateSelectionChanged();
                        break;
                    case "year":
                        if (target.date.getFullYear() == visibleDate.getFullYear()) {
                            cal._switchMode("months");
                        } else {
                            cal._visibleDate = target.date;
                            cal._switchMode("months");
                        }
                        break;

                    //                case "day": 
                    //                    this.set_selectedDate(target.date); 
                    //                    this._switchMonth(target.date); 
                    //                    this._blur.post(true); 
                    //                    this.raiseDateSelectionChanged(); 
                    //                    break; 
                    case "today":
                        cal.set_selectedDate(target.date);
                        cal._switchMonth(target.date);
                        cal._blur.post(true);
                        cal.raiseDateSelectionChanged();
                        break;
                }
            })
        }
    }

    function onCalendarShown(sender, args) {
        //set the default mode to month
        sender._switchMode("months", true);
        changeCellHandlers(cal1);
    }

    function changeCellHandlers(cal) {
        if (cal._monthsBody) {
            //remove the old handler of each month body.
            for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                var row = cal._monthsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    $common.removeHandlers(row.cells[j].firstChild, cal._cell$delegates);
                }
            }
            //add the new handler of each month body.
            for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                var row = cal._monthsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    $addHandlers(row.cells[j].firstChild, cal._cell$delegates);
                }
            }
        }
    }

    function onCalendarHidden(sender, args) {
        if (sender.get_selectedDate()) {
//            if (cal1.get_selectedDate() && cal2.get_selectedDate() && cal1.get_selectedDate() > cal2.get_selectedDate()) {
//                alert('The "From" Date should smaller than the "To" Date, please reselect!');
//                sender.show();
//                return;
//            }
            //get the final date
            var finalDate = new Date(sender.get_selectedDate());
            var selectedMonth = finalDate.getMonth();
            finalDate.setDate(1);
//            if (sender == cal2) {
//                // set the calender2's default date as the last day
//                finalDate.setMonth(selectedMonth + 1);
//                finalDate = new Date(finalDate - 1);
//            }
            //set the date to the TextBox
            sender.get_element().value = finalDate.format(sender._format);
        }
    }
    
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="lbtnExport" />
                </Triggers>
                <ContentTemplate>
            
    <asp:EntityDataSource ID="edsWarehouse" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="WareHouse"
        Select="it.[WHCode], it.[WHName]">
    </asp:EntityDataSource>

    <asp:EntityDataSource ID="edsInventoryBook" runat="server" 
        ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
        EnableFlattening="False" EntitySetName="InventoryBook" 
        onquerycreated="edsInventoryBook_QueryCreated">
    </asp:EntityDataSource>

    <div class="box">
        <h2>
            <span>查询条件</span>&nbsp;&nbsp;
        </h2>
        <div class="boxContent">
            <table>
                <tr>
                    <td style="width: 70px; text-align: right">
                        商品名称：</td>
                    <td style="width:200px;" >
                        <asp:TextBox ID="txtCargoName" runat="server" MaxLength="100" Width="187px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        商品编码：
                    </td>
                    <td style="width: 130px">
                        <asp:TextBox ID="txtCargoCode" runat="server" MaxLength="32" Width="150px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        台帐月份：
                    </td>
                    <td style="width: 100px;">
                        <asp:TextBox ID="txtDate" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM"
                            TargetControlID="txtDate" DefaultView="Months" BehaviorID="calendar1"
                            OnClientShown="onCalendarShown" OnClientHidden="onCalendarHidden"  >
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
                        <asp:DropDownList ID="drpWHCode" runat="server" Width="155px"
                            DataSourceID="edsWarehouse" DataTextField="WHName" DataValueField="WHCode"
                            AppendDataBoundItems="true" >
                                <asp:ListItem Text="----不限----" Value="" Selected="True" />
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="Button1" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            Text="查 询"  Height="23px" Width="91px" onclick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div class="box">
        <h2>
            <span>查询结果</span>
            <asp:LinkButton ID="lbtnExport" runat="server" Text="导出Excel" 
                onclick="lbtnExport_Click" /> 
        </h2>
        <div class="boxContent" style="overflow-x: scroll;height:400px; border:1px solid #999;">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center" 
                 Width="99%" DataKeyNames="TableAccountID,CargoCode" DataSourceID="edsInventoryBook" CssClass="linetable"
                ShowFooter="true" onrowdatabound="GridView1_RowDataBound">
                <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                <RowStyle CssClass="GridViewRowStyle" />
                <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                <PagerStyle CssClass="GridViewPagerStyle" />
                <FooterStyle CssClass="GridViewRowStyle" />
                <EmptyDataTemplate>
                <table class="GridViewEmpty_Table">
                <tr class="GridViewEmpty_RowHeader">
                <td>序号</td>
                <td>商品名称</td>
                <td>商品编码</td>
                <td>型号</td>
                <td>规格</td>
                <td>期初数量</td>
                <td>本月收入</td>
                <td>本月发出</td>
                <td>结余数量</td>
                </tr>
                <tr class="GridViewEmpty_RowData">
                <td colspan="29">
                无数据
                </td>
                </tr>
                </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="序号" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px">
                      <ItemTemplate>
                         <%#  Container.DataItemIndex+1 %> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="280px"
                        SortExpression="CargoName" />
                    <asp:BoundField DataField="CargoCode" HeaderText="商品编码" ReadOnly="True" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center"
                        SortExpression="CargoCode" ItemStyle-Width="150px" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px"
                        SortExpression="CargoModel" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px"
                        SortExpression="CargoSpec" />
                    <%--<asp:BoundField DataField="ChangeTime" HeaderText="日期" ItemStyle-HorizontalAlign="Center" 
                        SortExpression="ChangeTime" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="计量单位" ItemStyle-HorizontalAlign="Center" 
                        SortExpression="CargoUnits" />
                    <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" ItemStyle-HorizontalAlign="Center" 
                        SortExpression="ReleaseYear" />
                    <asp:BoundField DataField="UCOrderCode" HeaderText="用友单据号" ItemStyle-HorizontalAlign="Center" 
                        SortExpression="UCOrderCode" />
                    <asp:BoundField DataField="Remark" HeaderText="摘要" SortExpression="Remark" ItemStyle-HorizontalAlign="Center" />--%>
                    <asp:BoundField DataField="NumOriginal" HeaderText="期初数量" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px"
                        SortExpression="NumOriginal" />
                    <asp:BoundField DataField="NumAdd" HeaderText="本月收入" SortExpression="NumAdd" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px" />
                    <asp:BoundField DataField="NumDel" HeaderText="本月发出" SortExpression="NumDel" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px" />
                    <asp:BoundField DataField="NumCurrent" HeaderText="结余数量" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px"
                        SortExpression="NumCurrent" />
          <%--          <asp:BoundField DataField="ChangePerson" HeaderText="记账人" ItemStyle-HorizontalAlign="Center" 
                        SortExpression="ChangePerson" />--%>
                </Columns>
            </asp:GridView>
        </div>
    </div>


        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
