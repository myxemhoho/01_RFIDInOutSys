<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StockOutReg.aspx.cs" Inherits="Gold.StockMove.StockOutReg" %>
 

<%@ Register TagPrefix="ucBinCode" TagName="BinCodeSelect" Src="~/Controls/BinCodeSelect.ascx" %>
<%@ Register TagPrefix="ucGoods" TagName="GoodsSelect" Src="~/Controls/GoodsSelect.ascx" %>

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
    <asp:ObjectDataSource ID="odsStockOut" runat="server" 
        SelectMethod="SelectStockOut"  
        TypeName="Gold.StockMove.StockOutReg" 
        onobjectcreating="odsStockOut_ObjectCreating" 
        onobjectdisposing="odsStockOut_ObjectDisposing" 
    />
    <asp:ObjectDataSource ID="odsStockDetail" runat="server" 
        SelectMethod="SelectStockDetail" 
        TypeName="Gold.StockMove.StockOutReg" 
        onobjectcreating="odsStockDetail_ObjectCreating" 
        onobjectdisposing="odsStockDetail_ObjectDisposing" 
        SortParameterName="BillRowNumber"
    />
    <div class="box">
        <h2>
            <asp:Label runat="server" ID="lblTitle" Text="抬头信息" CssClass="boxTitle" />
        </h2>
        <div class="boxContent">
            <div class="orderHeader">
                <asp:FormView ID="FormView1" runat="server" DataKeyNames="SOCode" DefaultMode="ReadOnly"  
                    DataSourceID="odsStockOut" ondatabound="FormView1_DataBound" >
                    <ItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlSOCode" Width="100%" runat="server" Text='<%# Bind("SOCode") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlSOType" runat="server" Enabled="false" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsSOType" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>    
                                    <asp:TextBox ID="txtSOType" runat="server" Text='<%# Bind("SOType") %>' Visible="false"></asp:TextBox>                                
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    单据状态：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlSOStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsSOStatus" DataTextField="Name" SelectedValue='<%# Bind("SOStatus") %>' 
                                        DataValueField="Code" >
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    出库日期：</td>
                                <td style="width: 220px;">
                                    <asp:Label ID="ctlSODate" runat="server" Text='<%# Bind("SODate", "{0:yyyy-MM-dd}") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlFromType" runat="server"  Enabled="false" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsFromType" DataTextField="Name" SelectedValue='<%# Bind("FromType") %>' 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsFromType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="FromType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>                                    
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    部门：</td>
                                <td style="width: 120px;">                                    
                                    <%--<asp:DropDownList ID="ctlSellDepartmentName" runat="server" Enabled="false" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsDepartmentName" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsDepartmentName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SellDepartmentName" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>--%>
                                    <asp:Label ID="lblSellDepartmentName" Width="100%" runat="server" Text='<%# Bind("SellDepartmentName") %>' />
                                    <asp:TextBox ID="txtSellDepartmentName" runat="server" Text='<%# Bind("SellDepartmentName") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    客户名称：</td>
                                <td style="width: 220px;" >
                                    <asp:Label ID="ctlCustomerName" runat="server" Text='<%# Bind("CustomerName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    业务类型：</td>
                                <td style="width: 120px;" >
                                    <asp:DropDownList ID="ctlBusinessType" runat="server"  Width="100px" Enabled="false" AppendDataBoundItems="True"
                                        DataSourceID="edsBusinessType" DataTextField="Name"
                                        DataValueField="Code">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsBusinessType" runat="server"
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OBusinessType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtBusinessType_Selected" runat="server" Text='<%# Bind("BusinessType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 150px;">
                                    <%--<asp:Label ID="ctlBussinessMan" Width="100%" runat="server" Text='<%# Bind("BussinessMan") %>' />--%>
                                    <%--<asp:DropDownList ID="ctlBussinessMan" runat="server" Enabled="false" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name"
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>--%>
                                    <asp:Label ID="lblBussinessMan" Width="100%" runat="server" Text='<%# Bind("BussinessMan") %>' />
                                    <asp:TextBox ID="txtBussinessMan" runat="server" Text='<%# Bind("BussinessMan") %>' Visible="false"></asp:TextBox>
                                </td>                                                     
                                <td style="width: 120px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">
                                    <%--<asp:DropDownList ID="ctlStorageMan" runat="server" Width="100px" Enabled="false"
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName" 
                                        DataValueField="UserName" AppendDataBoundItems="True">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">                                       
                                    </asp:EntityDataSource>--%>
                                    <asp:Label ID="lblStorageMan" runat="server" Text='<%# Bind("StorageMan") %>'></asp:Label>
                                    <asp:TextBox ID="txtStorageMan" runat="server" Text='<%# Bind("StorageMan") %>' Visible="false"></asp:TextBox>
                                </td> 
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 220px;" >                                   
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200" AppendDataBoundItems="True"
                                      DataSourceID="edsInWHName" DataTextField="WHName"  DataValueField="WHCode" Enabled="false" 
                                     >
                                      <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                      </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="WareHouse" >                            
                                    </asp:EntityDataSource>   
                                    <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                                </td>           
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    合计数量：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlTotalNumber" runat="server" Text='<%# Bind("TotalNumber") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlOutCategory" runat="server" Enabled="false" Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsOutCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOutCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OutCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtOutCategory" runat="server" Text='<%# Bind("OutCategory") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    源Excel出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlFromBillNo" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备 注：</td>
                                <td colspan="1" style="width:220px;" >
                                    <asp:Label ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlSOCode" runat="server" Text='<%# Bind("SOCode") %>' Enabled="false" />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlSOType" runat="server"  Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsSOType" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                    <asp:TextBox ID="txtSOType" runat="server" Text='<%# Bind("SOType") %>' Visible="false"></asp:TextBox>                                
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    单据状态：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlSOStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsSOStatus" DataTextField="Name" SelectedValue='<%# Bind("SOStatus") %>' 
                                        DataValueField="Code" >

                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    出库日期：</td>
                                <td style="width: 220px;" >
                                    <asp:TextBox Width="200px" ID="ctlSODate" runat="server" Text='<%# Bind("SODate", "{0:yyyy-MM-dd}") %>' />
                                    <ajaxToolkit:CalendarExtender ID="ctlSODate_CalendarExtender" runat="server"
                                        TargetControlID="ctlSODate" >
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 150px;">
                                    <%--<asp:DropDownList Width="100px" ID="ctlFromType" runat="server"/>--%>
                                    <asp:DropDownList ID="ctlFromType" runat="server" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsFromType" DataTextField="Name" SelectedValue='<%# Bind("FromType") %>' 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsFromType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="FromType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                    <%--<asp:TextBox Width="100px" ID="ctlFromType" runat="server" Text='<%# Bind("FromType") %>' />  --%>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    部门：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlSellDepartmentName" runat="server" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsDepartmentName" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsDepartmentName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SellDepartmentName" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtSellDepartmentName" runat="server" Text='<%# Bind("SellDepartmentName") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    客户名称：</td>
                                <td style="width: 220px;" >
                                    <asp:TextBox Width="200px" ID="ctlCustomerName" runat="server" Text='<%# Bind("CustomerName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    业务类型：</td>
                                <td style="width: 120px;" >
                                    <asp:DropDownList ID="ctlBusinessType" runat="server" Width="100px" AppendDataBoundItems="True"
                                        DataSourceID="edsBusinessType" DataTextField="Name"
                                        DataValueField="Code">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsBusinessType" runat="server"
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OBusinessType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtBusinessType_Selected" runat="server" Text='<%# Bind("BusinessType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlBussinessMan" runat="server" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtBussinessMan" runat="server" Text='<%# Bind("BussinessMan") %>' Visible="false"></asp:TextBox>
                                </td>                                
                                <td style="width: 120px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">                                    
                                    <asp:DropDownList ID="ctlStorageMan" runat="server" Width="100px" 
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName" 
                                        DataValueField="UserName" AppendDataBoundItems="True" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">                                       
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtStorageMan" runat="server" Text='<%# Bind("StorageMan") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 220px;">
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200" AppendDataBoundItems="True"
                                      DataSourceID="edsInWHName" DataTextField="WHName" 
                                      DataValueField="WHCode">
                                      <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                      </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="WareHouse" >                            
                                    </asp:EntityDataSource>   
                                    <label class="commonSaveMsgLabel">*</label>
                                    <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                                </td>                                
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    合计数量：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlTotalNumber" runat="server" Text='<%# Bind("TotalNumber") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlOutCategory" runat="server" Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsOutCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOutCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OutCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtOutCategory" runat="server" Text='<%# Bind("OutCategory") %>' Visible="false"></asp:TextBox>
                                    <label class="commonSaveMsgLabel">*</label>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    源Excel出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox ID="ctlFromBillNo" Width="100px" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备 注：</td>
                                <td colspan="1" style="width: 220px;">
                                    <asp:TextBox Width="200px" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlSOCode" runat="server" Text='<%# Bind("SOCode") %>' Enabled="false"/>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlSOType" runat="server"  Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsSOType" DataTextField="Name"
                                        DataValueField="Code">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                    <asp:TextBox ID="txtSOType" runat="server" Text='<%# Bind("SOType") %>' Visible="false"></asp:TextBox>                                
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    单据状态：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlSOStatus" runat="server" Enabled="false" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsSOStatus" DataTextField="Name" SelectedValue='<%# Bind("SOStatus") %>' 
                                        DataValueField="Code" >
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSOStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SOStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    出库日期：</td>
                                <td style="width: 220px;" >
                                    <asp:TextBox Width="200px" ID="ctlSODate" runat="server" Text='<%# Bind("SODate", "{0:yyyy-MM-dd}") %>' />
                                    <ajaxToolkit:CalendarExtender ID="ctlSODate_CalendarExtender" runat="server"
                                        TargetControlID="ctlSODate" >
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlFromType" runat="server"   Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsFromType" DataTextField="Name" SelectedValue='<%# Bind("FromType") %>' 
                                        DataValueField="Code">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsFromType" runat="server"
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="FromType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    部门：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlSellDepartmentName" runat="server" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsDepartmentName" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsDepartmentName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SellDepartmentName" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtSellDepartmentName" runat="server" Text='<%# Bind("SellDepartmentName") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    客户名称：</td>
                                <td style="width: 220px;" >
                                    <asp:TextBox Width="200px" ID="ctlCustomerName" runat="server" Text='<%# Bind("CustomerName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    业务类型：</td>
                                <td style="width: 120px;" >
                                    <asp:DropDownList ID="ctlBusinessType" runat="server"   Width="100px" AppendDataBoundItems="True"
                                        DataSourceID="edsBusinessType" DataTextField="Name" 
                                        DataValueField="Code">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsBusinessType" runat="server"
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OBusinessType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtBusinessType_Selected" runat="server" Text='<%# Bind("BusinessType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlBussinessMan" runat="server" Width="130" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name"
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Operator" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtBussinessMan" runat="server" Text='<%# Bind("BussinessMan") %>' Visible="false"></asp:TextBox>
                                </td>                                
                                <td style="width: 120px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlStorageMan" runat="server" Width="100px"
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName" 
                                        DataValueField="UserName" AppendDataBoundItems="True">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">                                       
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtStorageMan" runat="server" Text='<%# Bind("StorageMan") %>' Visible="false"></asp:TextBox>
                                </td>  
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 220px;">
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200" AppendDataBoundItems="True"
                                      DataSourceID="edsInWHName" DataTextField="WHName"
                                      DataValueField="WHCode">
                                      <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                      </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInWHName" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="WareHouse" >                            
                                    </asp:EntityDataSource>  
                                    <label class="commonSaveMsgLabel">*</label> 
                                    <asp:TextBox ID="txtWHName" runat="server" Text='<%# Bind("WHCode") %>' Visible="false"></asp:TextBox>
                                </td>                              
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    合计数量：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlTotalNumber" runat="server" Text='<%# Bind("TotalNumber") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ctlOutCategory" runat="server"  Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsOutCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOutCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="OutCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtOutCategory" runat="server" Text='<%# Bind("OutCategory") %>' Visible="false"></asp:TextBox>
                                    <label class="commonSaveMsgLabel">*</label>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    源Excel出库单号：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox ID="ctlFromBillNo" Width="100px" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备 注：</td>
                                <td colspan="1" style="width: 220px;">
                                    <asp:TextBox Width="200px" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
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
            <asp:LinkButton ID="lbtnAddRow" runat="server" Text="增加商品" 
                onclick="lbtnAddRow_Click" />
        </h2>
        <div class="boxContent">
            <div class="orderItem" style="overflow-x: scroll;min-height:400px;border:solid 1px #999;">
                <%--列表行项目编辑--%>
                <asp:GridView ID="grdStockDetailEdit" runat="server" AutoGenerateColumns="False" 
                    Width="1700px" DataSourceID="odsStockDetail" 
                    DataKeyNames="BillCode,BillRowNumber" onrowcommand="grdStockDetailEdit_RowCommand" 
                    RowStyle-HorizontalAlign="Center"  CssClass="linetable" 
                    onrowdatabound="grdStockDetailEdit_RowDataBound">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <PagerStyle CssClass="GridViewPagerStyle" />
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
                        <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode" ItemStyle-Width="170px">
                            <ItemTemplate>                                
                                <asp:TextBox ID="ctlCargoCode" Width="120px" runat="server" Text='<%# Bind("CargoCode") %>' Visible='<%# GetButtonVisible()%>'></asp:TextBox>
                                <asp:Button runat="server" ID="SelectCargo" Text="..." CommandName="GoodsSelect" Visible='<%# GetButtonVisible()%>' />
                                <asp:Label runat="server" ID="lblCargoCode" Text='<%# Bind("CargoCode") %>' Visible ='<%# GetButtonVisible()==true?false:true %>' />
                            </ItemTemplate>
                            <%--<ItemTemplate>
                               <asp:Label ID="lblCargoCode1" runat="server" Text='<%# Bind("CargoCode") %>' Visible='<%# GetButtonVisible()==true?false:true%>'></asp:Label>
                            </ItemTemplate>--%>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="商品名称" SortExpression="CargoName" ItemStyle-Width="300px">
                            <ItemTemplate>
                                <asp:TextBox ID="ctlCargoName" runat="server" Width="250px" Text='<%# Bind("CargoName") %>' Visible ='<%# GetButtonVisible()%>'></asp:TextBox>
                                <asp:Button runat="server" ID="SelectCargoByName" Text="..." CommandName="GoodsSelect" Visible='<%# GetButtonVisible()%>' />
                                <asp:Label runat="server" ID="lblCargoName" Text='<%# Bind("CargoName") %>' Visible ='<%# GetButtonVisible()==true?false:true %>' />
                            </ItemTemplate>                           
                        </asp:TemplateField>                      
                        <asp:BoundField DataField="CargoSpec" HeaderText="规 格" 
                            SortExpression="CargoSpec" />
                        <asp:BoundField DataField="CargoModel" HeaderText="型 号" 
                            SortExpression="CargoModel" />                        
                        <asp:BoundField DataField="CargoUnits" HeaderText="单 位" 
                            SortExpression="CargoUnits" />                         
                         <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" ItemStyle-Width="75px"
                            SortExpression="NumOriginalPlan" />                          
                        <asp:TemplateField HeaderText="应发数量" SortExpression="NumCurrentPlan" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtNumCurrentPlan" Width="80px" runat="server" Text='<%# Bind("NumCurrentPlan") %>'></asp:TextBox>                                
                            </ItemTemplate>                           
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumActual" HeaderText="实发数量" ItemStyle-Width="75px" SortExpression="NumActual" />
                        <%--<asp:TemplateField HeaderText="实发数量" SortExpression="NumActual" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:TextBox ID="TextBox2" Width="80px" runat="server" Text='<%# Bind("NumOriginalPlan") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>--%>                         
                        <asp:TemplateField HeaderText="发货区位" SortExpression="BinCode" ItemStyle-Width="110px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtBinCode" Width="50px" runat="server" Text='<%# Bind("BinCode") %>'></asp:TextBox>
                                <asp:Button runat="server" ID="SelectBinCode" Text="..." CommandName="BinCodeSelect" />
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" ForeColor="Red" ControlToValidate="txtBinCode"
                                                runat="server" ErrorMessage="请选择发货层位！"></asp:RequiredFieldValidator>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" 
                            SortExpression="UCOrderNo" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" 
                            SortExpression="Comment" />
                         <asp:TemplateField HeaderText="商品状态" SortExpression="CargoStatus" ItemStyle-Width="60px">
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
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" ItemStyle-Width="100px" 
                            SortExpression="UCOrderNo" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" 
                            SortExpression="Comment" />
                         <asp:TemplateField HeaderText="状态" SortExpression="CargoStatus" ItemStyle-Width="60px">
                            <ItemTemplate><asp:Label runat="server" ID="lblCargoStatus" Text='<%# Eval("CargoStatus")==null?"":Eval("CargoStatus").ToString()=="0"?"未完成":"已完成" %>'></asp:Label></ItemTemplate>
                         </asp:TemplateField>
                        <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" 
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
        <asp:Button ID="btnReset" runat="server" Text="重 置" CssClass="ButtonImageStyle" 
          onclick="btnReset_Click" />
        <asp:Button ID="btnUpdate" runat="server" Text="更 新" CssClass="ButtonImageStyle" 
          onclick="btnUpdate_Click" Visible="false" />        
        <asp:Button ID="btnCancel" runat="server" Text="取 消" CssClass="ButtonImageStyle" 
          onclick="btnCancel_Click" Visible="false" OnClientClick="javascript:return confirm('确认放弃当前操作吗？');" />
        <asp:Button ID="btnReturn" runat="server" Text="返回出库单列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturn_Click" Visible="false" />        
        <asp:Button ID="btnReturnSales" runat="server" Text="返回销售列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturnSales_Click" Visible="false" />
        <asp:Button ID="btnReturnShift" runat="server" Text="返回转库列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturnShift_Click" Visible="false" />
        <asp:Button ID="btnReturnTransfer" runat="server" Text="返回调拨列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturnTransfer_Click" Visible="false" />
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
    <asp:Panel ID="pnlPopWindowGoods" runat="server" Style="display: none" CssClass="modalPopupCargo">
        <div class="modalPopupWrapperCargo">
            <div id="pnlDragTitleGoods" class="modalHeader">
                <span>选择商品</span>
                <asp:Button ID="btnCloseGoods" runat="server" CssClass="ClosePopButton" Text="关闭" OnClick="btnCloseGoods_Click" />
            </div>
            <div class="modalBody">
                <ucGoods:GoodsSelect ID="GoodsSelect1" runat="server" OnGetCargoSelect="GoodsSelect1_GetCargoSelect"  />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="popWindowGoods" runat="server" TargetControlID="lbGoods"
        PopupControlID="pnlPopWindowGoods" BackgroundCssClass="modalBackground" DropShadow="true"
        PopupDragHandleControlID="pnlDragTitleGoods">
    </ajaxToolkit:ModalPopupExtender>
    <asp:LinkButton ID="lbGoods" runat="server" width="0" Height="0"></asp:LinkButton>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupBinCode">
        <div class="modalPopupWrapperBinCode">
            <div id="pnlDragTitle" class="modalHeader">
                <span>选择层位</span>
                <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="关闭" OnClick="btnClosePop_Click" />
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
