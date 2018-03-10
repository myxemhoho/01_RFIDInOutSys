<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StockOutCancel.aspx.cs" Inherits="Gold.StockMove.StockOutCancel" %>
<%@ Register TagPrefix="ucBinCode" TagName="BinCodeSelect" Src="~/Controls/BinCodeSelect.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript" type="text/javascript">
        function selectAll(obj) {
            var theTable = document.getElementById("<%=grdStockDetailEdit.ClientID%>");  //obj..parentElement.parentElement.parentElement;
            var nodeList = theTable.getElementsByTagName("input");
            for (var i = 0; i < nodeList.length; i++) {
                var node = nodeList[i];
                if (node.type == "checkbox") {
                    node.checked = obj.checked;
                }
            }
        }
    </script>    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ObjectDataSource ID="odsStockCancel" runat="server" 
        SelectMethod="SelectStockCancel"          
        TypeName="Gold.StockMove.StockOutCancel" 
        onobjectcreating="odsStockCancle_ObjectCreating" 
        onobjectdisposing="odsStockCancle_ObjectDisposing" 
    />
    <asp:ObjectDataSource ID="odsStockDetail" runat="server" 
        SelectMethod="SelectStockDetail" 
        TypeName="Gold.StockMove.StockOutCancel" 
        onobjectcreating="odsStockDetail_ObjectCreating" 
        onobjectdisposing="odsStockDetail_ObjectDisposing"         
    />
    <div class="box">
        <h2>
            <asp:Label runat="server" ID="lblTitle" Text="抬头信息" CssClass="boxTitle" />
        </h2>
        <div class="boxContent">
            <div class="orderHeader">
                <asp:FormView ID="FormView1" runat="server" DataKeyNames="SCCode" DefaultMode="ReadOnly"  
                DataSourceID="odsStockCancel" ondatabound="FormView1_DataBound" >
                <ItemTemplate>
                    <table class="tableForm">
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                撤销单号：</td>
                            <td style="width: 120px;">
                                <asp:Label ID="ctlSCCode" runat="server" Text='<%# Bind("SCCode") %>' />
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销单类型：</td>
                            <td style="width: 120px;">
                              <asp:DropDownList ID="ctlSCType" runat="server" Enabled="false" Width="100" 
                                    DataSourceID="edsSCType" DataTextField="Name" 
                                    DataValueField="Code" AppendDataBoundItems="True" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCType" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SCType" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                                
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销状态：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlSCStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="true"
                                    DataSourceID="edsSCStatus" DataTextField="Name" SelectedValue='<%# Bind("SCStatus") %>' 
                                    DataValueField="Code" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCStatus" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category  AND it.Code!='3' AND it.Code !='4' ">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />                                           
                                    </WhereParameters>
                                </asp:EntityDataSource> 
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销日期：</td>
                            <td style="width: 210px;" >
                                <asp:Label ID="ctlSCDate" runat="server" Text='<%# Bind("SCDate", "{0:yyyy-MM-dd}") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                被冲销单号：</td>
                            <td style="width: 120px;">
                                <asp:TextBox ID="ctlCancelBillCode" runat="server" Enabled="false" Text='<%# Bind("CancelBillCode") %>' />
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                被冲销程度：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlCancelLevel" runat="server" DataSourceID="edsCancelLevel" 
                                    DataTextField="Name" DataValueField="Code" Enabled="false" AppendDataBoundItems="True"
                                    SelectedValue='<%# Bind("CancelLevel") %>' 
                                    Width="100">
                                <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsCancelLevel" runat="server" 
                                    ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                                    EnableFlattening="False" EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Name="Category" Type="String" DefaultValue="CancelLevel" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                仓  库：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlWHName" runat="server" Width="100" AppendDataBoundItems="True"
                                    DataSourceID="edsInWHName" DataTextField="WHName" Enabled="false"
                                    DataValueField="WHCode">
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="WareHouse" >                            
                                </asp:EntityDataSource>   
                                <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                备  注：</td>
                            <td style="width: 210px;" >
                                <asp:Label ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />                                    
                            </td>
                        </tr>                                                  
                    </table>
                </ItemTemplate>
                <EditItemTemplate>
                    <table class="tableForm">
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                撤销单号：</td>
                            <td style="width: 120px;">
                                <asp:Label ID="ctlSCCode" runat="server" Text='<%# Bind("SCCode") %>' />
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销单类型：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlSCType" runat="server" Enabled="false" Width="100" 
                                    DataSourceID="edsSCType" DataTextField="Name"  SelectedValue='<%# Bind("SCType") %>'
                                    DataValueField="Code" AppendDataBoundItems="True" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCType" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SCType" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销状态：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlSCStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="true"
                                    DataSourceID="edsSCStatus" DataTextField="Name" SelectedValue='<%# Bind("SCStatus") %>' 
                                    DataValueField="Code" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCStatus" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category AND it.Code!='3' AND it.Code !='4' ">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />                                           
                                    </WhereParameters>
                                </asp:EntityDataSource> 
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销日期：</td>
                            <td style="width: 210px;" >                                   
                                <asp:TextBox Width="100px" ID="ctlSCDate" runat="server" Text='<%# Bind("SCDate", "{0:yyyy-MM-dd}") %>' />
                                <ajaxToolkit:CalendarExtender ID="ctlSCDate_CalendarExtender" runat="server"
                                    TargetControlID="ctlSCDate" >
                                </ajaxToolkit:CalendarExtender>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                被冲销单号：</td>
                            <td style="width: 120px;">
                                <asp:TextBox ID="ctlCancelBillCode" runat="server" Enabled="false" Text='<%# Bind("CancelBillCode") %>' />
                            </td>
                            <%--<td style="width: 100px;" class="tdKey">
                                被冲销程度：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlCancelLevel" runat="server" DataSourceID="edsCancelLevel" 
                                    DataTextField="Name" DataValueField="Code"  AppendDataBoundItems="True"
                                    SelectedValue='<%# Bind("CancelLevel") %>' 
                                    Width="100">
                                <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsCancelLevel" runat="server" 
                                    ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                                    EnableFlattening="False" EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Name="Category" Type="String" DefaultValue="CancelLevel" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                            </td>--%>
                            <td style="width: 100px;" class="tdKey">
                                仓  库：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlWHName" runat="server" Width="100" AppendDataBoundItems="True"
                                    DataSourceID="edsInWHName" DataTextField="WHName" Enabled="false"
                                    DataValueField="WHCode">
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="WareHouse" >                            
                                </asp:EntityDataSource>   
                                <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                备  注：</td>
                            <td colspan="3" style="width: 210px;" >                                  
                                <asp:TextBox Width="200px" MaxLength="100" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />                               
                            </td>
                        </tr>                                                  
                    </table>                    
                </EditItemTemplate>
                <InsertItemTemplate>
                    <table class="tableForm">
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                撤销单号：</td>
                            <td style="width: 120px;">
                                <asp:Label ID="ctlSCCode" runat="server" Text='<%# Bind("SCCode") %>' />
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销单类型：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlSCType" runat="server" Enabled="false" Width="100" 
                                    DataSourceID="edsSCType" DataTextField="Name"  SelectedValue='<%# Bind("SCType") %>'
                                    DataValueField="Code" AppendDataBoundItems="True" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCType" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SCType" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销状态：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlSCStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="true"
                                    DataSourceID="edsSCStatus" DataTextField="Name" SelectedValue='<%# Bind("SCStatus") %>' 
                                    DataValueField="Code" >
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsSCStatus" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="DataDict" Where="it.Category=@Category AND it.Code!='3' AND it.Code !='4' ">
                                    <WhereParameters>
                                        <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />                                           
                                    </WhereParameters>
                                </asp:EntityDataSource> 
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                撤销日期：</td>
                            <td style="width: 210px;" >                                   
                                <asp:TextBox Width="100px" ID="ctlSCDate" runat="server" Text='<%# Bind("SCDate", "{0:yyyy-MM-dd}") %>' />
                                <ajaxToolkit:CalendarExtender ID="ctlSCDate_CalendarExtender" runat="server"
                                    TargetControlID="ctlSCDate" >
                                </ajaxToolkit:CalendarExtender>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="tdKey">
                                被冲销单号：</td>
                            <td style="width: 120px;">
                                <asp:TextBox ID="ctlCancelBillCode" runat="server" Enabled="false" Text='<%# Bind("CancelBillCode") %>' />
                            </td>
                            <%--<td style="width: 100px;" class="tdKey">
                                被冲销程度：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlCancelLevel" runat="server" DataSourceID="edsCancelLevel" 
                                    DataTextField="Name" DataValueField="Code" AppendDataBoundItems="True"
                                    SelectedValue='<%# Bind("CancelLevel") %>' 
                                    Width="100">
                                <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:EntityDataSource ID="edsCancelLevel" runat="server" 
                                    ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                                    EnableFlattening="False" EntitySetName="DataDict" Where="it.Category=@Category">
                                    <WhereParameters>
                                        <asp:Parameter Name="Category" Type="String" DefaultValue="CancelLevel" />
                                    </WhereParameters>
                                </asp:EntityDataSource>
                            </td>--%>
                            <td style="width: 100px;" class="tdKey">
                                仓  库：</td>
                            <td style="width: 120px;">
                                <asp:DropDownList ID="ctlWHName" runat="server" Width="100" AppendDataBoundItems="True"
                                    DataSourceID="edsInWHName" DataTextField="WHName" Enabled="false"
                                    DataValueField="WHCode">
                                    <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                    ConnectionString="name=GoldEntities" 
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="WareHouse" >                            
                                </asp:EntityDataSource>   
                                <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                            </td>
                            <td style="width: 100px;" class="tdKey">
                                备  注：</td>
                            <td colspan="3" style="width: 210px;" >                                  
                                <asp:TextBox Width="200px" MaxLength="100" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />                               
                            </td>
                        </tr>                                                  
                    </table>
                </InsertItemTemplate>
            </asp:FormView>
            </div>
        </div>
    </div>
    <div class="box">
        <h2>
            <span>商品列表</span>
            <asp:LinkButton ID="lbtnDeleteRow" runat="server" Text="删除所选商品" 
                onclick="lbtnDeleteRow_Click" OnClientClick="javascript:return confirm('确认要删除选中商品吗？');" />          
        </h2>
        <div class="boxContent">
            <div class="orderItem" style="overflow-x: scroll; min-height:400px;border:solid 1px #999;">
                <%--列表行项目编辑--%>
                <asp:GridView ID="grdStockDetailEdit" runat="server" AutoGenerateColumns="False" 
                    Width="1700px" DataSourceID="odsStockDetail" 
                    DataKeyNames="BillCode,BillRowNumber" onrowcommand="grdStockDetailEdit_RowCommand" RowStyle-HorizontalAlign="Center"  CssClass="linetable">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="50px">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked="true" Text="" />
                            </ItemTemplate>
                            <HeaderTemplate>
                                全选<input id="CheckAll" type="checkbox" onclick="selectAll(this);" checked="checked" />
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BillRowNumber" HeaderText="序号" 
                            SortExpression="BillRowNumber" ReadOnly="True" />
                        <asp:BoundField DataField="CargoCode" HeaderText="商品编码" ItemStyle-Width="170px"
                            SortExpression="CargoCode" />
                        <asp:BoundField DataField="CargoName" HeaderText="商品名称" ItemStyle-Width="300px"
                            SortExpression="CargoName" />                    
                        <asp:BoundField DataField="CargoSpec" HeaderText="规 格" 
                            SortExpression="CargoSpec" />
                        <asp:BoundField DataField="CargoModel" HeaderText="型 号" 
                            SortExpression="CargoModel" />                        
                        <asp:BoundField DataField="CargoUnits" HeaderText="单 位" 
                            SortExpression="CargoUnits" />
                         <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" ItemStyle-Width="70px"
                            SortExpression="NumOriginalPlan" />                          
                        <asp:TemplateField HeaderText="应发数量" SortExpression="NumCurrentPlan" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtNumCurrentPlan" Width="80px" runat="server" Text='<%# Bind("NumCurrentPlan") %>'></asp:TextBox>                                
                            </ItemTemplate>                           
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumActual" HeaderText="实发数量" ItemStyle-Width="70px"
                            SortExpression="NumActual" />                          
                        <asp:TemplateField HeaderText="发货区位" SortExpression="BinCode" ItemStyle-Width="110px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtBinCode" Width="50px" runat="server" Text='<%# Bind("BinCode") %>'></asp:TextBox>
                                <asp:Button runat="server" ID="SelectBinCode" Text="..." CommandName="BinCodeSelect" />                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" 
                            SortExpression="UCOrderNo" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" 
                            SortExpression="Comment" />
                        <asp:TemplateField HeaderText="商品状态" SortExpression="CargoStatus" ItemStyle-Width="40px">
                            <ItemTemplate><asp:Label runat="server" ID="lblCargoStatus" Text='<%# Eval("CargoStatus")==null?"":Eval("CargoStatus").ToString()=="0"?"未完成":"已完成" %>'></asp:Label></ItemTemplate>
                         </asp:TemplateField>
                        <asp:BoundField DataField="ReleaseYear" HeaderText="商品发行年份" 
                            SortExpression="ReleaseYear" />
                        <asp:BoundField DataField="InOutWHCode" HeaderText="发货仓库编码" 
                            SortExpression="InOutWHCode" />  
                        <asp:BoundField DataField="InOutWHName" HeaderText="发货仓库" SortExpression="InOutWHName" />
                        <asp:BoundField DataField="BillCode" HeaderText="BillCode" ReadOnly="True" 
                            SortExpression="BillCode" Visible="False" />
                        <asp:BoundField DataField="BillType" HeaderText="BillType" 
                            SortExpression="BillType" Visible="False" />
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="NumCurrentPlan" 
                            SortExpression="NumCurrentPlan" Visible="False" />
                        <asp:BoundField DataField="RowTotalMoney" HeaderText="RowTotalMoney" 
                            SortExpression="RowTotalMoney" Visible="False" />
                        <asp:BoundField DataField="RFIDOrderNo" HeaderText="RFIDOrderNo" 
                            SortExpression="RFIDOrderNo" Visible="False" />
                    </Columns>
                </asp:GridView>
                <%--列表行项目查看--%>
                <asp:GridView ID="grdStockDetail" runat="server" AutoGenerateColumns="False" 
                    Width="1500px" DataSourceID="odsStockDetail"  
                    DataKeyNames="BillCode,BillRowNumber"  RowStyle-HorizontalAlign="Center"  CssClass="linetable">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <Columns>
                        <asp:BoundField DataField="BillRowNumber" HeaderText="序号" 
                            SortExpression="BillRowNumber" ReadOnly="True" />
                        <asp:BoundField DataField="CargoCode" HeaderText="商品编码" 
                            SortExpression="CargoCode" />
                        <asp:BoundField DataField="CargoName" HeaderText="商品名称" 
                            SortExpression="CargoName" />
                        <asp:BoundField DataField="CargoSpec" HeaderText="规 格" 
                            SortExpression="CargoSpec" />
                        <asp:BoundField DataField="CargoModel" HeaderText="型 号" 
                            SortExpression="CargoModel" />                        
                        <asp:BoundField DataField="CargoUnits" HeaderText="单 位" 
                            SortExpression="CargoUnits" />
                        <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" 
                            SortExpression="NumOriginalPlan" /> 
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="应发数量" 
                            SortExpression="NumCurrentPlan" />                                                   
                        <asp:BoundField DataField="NumActual" HeaderText="实发数量" 
                            SortExpression="NumActual" />                        
                        <asp:BoundField DataField="BinCode" HeaderText="发货区位" 
                            SortExpression="BinCode" />
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" 
                            SortExpression="UCOrderNo" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" 
                            SortExpression="Comment" />
                        <asp:TemplateField HeaderText="商品状态" SortExpression="CargoStatus" ItemStyle-Width="40px">
                            <ItemTemplate><asp:Label runat="server" ID="lblCargoStatus" Text='<%# Eval("CargoStatus")==null?"":Eval("CargoStatus").ToString()=="0"?"未完成":"已完成" %>'></asp:Label></ItemTemplate>
                         </asp:TemplateField>
                        <asp:BoundField DataField="ReleaseYear" HeaderText="商品发行年份" 
                            SortExpression="ReleaseYear" />
                        <asp:BoundField DataField="InOutWHCode" HeaderText="发货仓库编码" 
                            SortExpression="InOutWHCode" />
                        <asp:BoundField DataField="InOutWHName" HeaderText="发货仓库" SortExpression="InOutWHName" />
                        <asp:BoundField DataField="BillCode" HeaderText="BillCode" ReadOnly="True" 
                            SortExpression="BillCode" Visible="False" />
                        <asp:BoundField DataField="BillType" HeaderText="BillType" 
                            SortExpression="BillType" Visible="False" />
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="NumCurrentPlan" 
                            SortExpression="NumCurrentPlan" Visible="False" />
                        <asp:BoundField DataField="RowTotalMoney" HeaderText="RowTotalMoney" 
                            SortExpression="RowTotalMoney" Visible="False" />
                        <asp:BoundField DataField="RFIDOrderNo" HeaderText="RFIDOrderNo" 
                            SortExpression="RFIDOrderNo" Visible="False" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>        
    </div>
    <hr />
    <div class="orderFooter">
        <asp:Button ID="btnSubmit" runat="server" Text="提交至备货" CssClass="ButtonImageStyle" 
          onclick="btnSubmit_Click" OnClientClick="javascript:return confirm('确定提交吗？');" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnEdit" runat="server" Text="编 辑" CssClass="ButtonImageStyle" 
          onclick="btnEdit_Click" Visible="false" />
        <asp:Button ID="btnSave" runat="server" Text="保 存" CssClass="ButtonImageStyle" 
          onclick="btnSave_Click" />      
        <asp:Button ID="btnUpdate" runat="server" Text="更 新" CssClass="ButtonImageStyle" 
          onclick="btnUpdate_Click" Visible="false" />        
        <asp:Button ID="btnCancel" runat="server" Text="取 消" CssClass="ButtonImageStyle" 
          onclick="btnCancel_Click" Visible="false" OnClientClick="javascript:return confirm('确认放弃当前操作吗？');" />
        <asp:Button ID="btnReturn" runat="server" Text="返回出库单列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturn_Click" Visible="false" />
        <br />  
        
        <%--设置编辑提示框--%>       
        <asp:Button runat="server" ID="hiddenTargetControlForModalPopup" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="programmaticModalPopup" BehaviorID="programmaticModalPopupBehavior"
            TargetControlID="hiddenTargetControlForModalPopup" PopupControlID="programmaticPopup"
            BackgroundCssClass="modalBackground" DropShadow="True" PopupDragHandleControlID="programmaticPopupDragHandle"
            RepositionMode="RepositionOnWindowScroll">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" Style="display: none;
            width: 300px; height:120px;">
            <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move; background-color: #DDDDDD;
                border: solid 1px Gray; color: Black; text-align: center;">
                提示
            </asp:Panel>
            <br />
            此订单
            <asp:Label runat="server" ID="lblEditorID" Font-Bold="true" Text='<%# Eval("EditorID") %>'></asp:Label>
            <asp:Label runat="server" ID="lblEditorName" Font-Bold="true" Text='<%# Eval("EditorName") %>'/>
            正在编辑。<br/>是否继续生成<asp:Label runat="server" ID="lblInOrOut" />?<br/>
            <asp:LinkButton runat="server" ID="hideModalPopupViaServer" Text="确定"
                OnClick="hideModalPopupViaServer_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;                   
            <asp:LinkButton runat="server" ID="hideClose" Text="取消" OnClick="hideClose_Click" />
            <br />
        </asp:Panel>        
    </div>
    <div>
       <asp:Label runat="server" ID="lblMessage" CssClass="commonSaveMsgLabel" Text="" ForeColor="Red"/>
    </div>    
    <asp:LinkButton ID="lbGoods" runat="server" width="0" Height="0"></asp:LinkButton>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupBinCode">
        <div class="modalPopupWrapperBinCode">
            <div id="pnlDragTitle" class="modalHeader">
                <span>选择层位</span>
                <asp:Button ID="btnClosePop" runat="server" Text="关闭" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <ucBinCode:BinCodeSelect ID="BinCodeSelect1" runat="server" OnGetBinCodeSelect="BinCodeSelect1_OnGetBinCodeSelect"/>
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="LinkButton1"
        PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
        PopupDragHandleControlID="pnlDragTitle">
    </ajaxToolkit:ModalPopupExtender>
    <asp:LinkButton ID="LinkButton1" runat="server" width="0" Height="0"></asp:LinkButton>


</asp:Content>

