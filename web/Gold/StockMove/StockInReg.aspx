<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StockInReg.aspx.cs" Inherits="Gold.StockMove.StockInReg" %>

<%@ Register TagPrefix="uc" TagName="GoodsSelect" Src="~/Controls/GoodsSelect.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript" type="text/javascript">
        function selectAll(obj) {
            var theTable = document.getElementById("<%=GridView1.ClientID%>");  //obj..parentElement.parentElement.parentElement;
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
    <asp:ObjectDataSource ID="odsStockIn" runat="server" 
        SelectMethod="SelectStockIn"  
        TypeName="Gold.StockMove.StockInReg" 
        onobjectcreating="odsStockIn_ObjectCreating" 
        onobjectdisposing="odsStockIn_ObjectDisposing" 
    />
    <asp:ObjectDataSource ID="odsStockDetail" runat="server" 
        SelectMethod="SelectStockDetail" 
        TypeName="Gold.StockMove.StockInReg" 
        onobjectcreating="odsStockDetail_ObjectCreating" 
        onobjectdisposing="odsStockDetail_ObjectDisposing" 
    />
    <asp:EntityDataSource ID="edsWarehouse" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" EntitySetName="WareHouse"
        Select="it.[WHCode], it.[WHName]">
    </asp:EntityDataSource>
    <asp:EntityDataSource ID="edsBinCode" runat="server" ConnectionString="name=GoldEntities"
        DefaultContainerName="GoldEntities" EnableFlattening="False" 
        EntitySetName="CargoTag" 
        Select="it.BinCode" Where="it.CargoCode is null"
        >
    </asp:EntityDataSource>
    <div class="box">
        <h2>
            <asp:Label runat="server" ID="lblTitle" Text="抬头信息" CssClass="boxTitle" />
        </h2>
        <div class="boxContent">
            <div class="orderHeader">
                <asp:FormView ID="FormView1" runat="server" DataKeyNames="SICode" DefaultMode="ReadOnly"  
                    DataSourceID="odsStockIn" ondatabound="FormView1_DataBound" >
                    <ItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    入库单号：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="ctlSICode" runat="server" Text='<%# Bind("SICode") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlSIType" runat="server" Enabled="false" Width="120" 
                                        DataSourceID="edsSIType" DataTextField="Name"
                                        DataValueField="Code" AppendDataBoundItems="True" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtSIType" runat="server" Text='<%# Bind("SIType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    入库日期：</td>
                                <td style="width: 120px;">
                                    <asp:Label ID="ctlSIDate" runat="server" Text='<%# Bind("SIDate", "{0:yyyy-MM-dd}") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 210px;" >
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200px" AppendDataBoundItems="True"
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
                            </tr>
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 140px;">
                                    <asp:Label ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlFromType" runat="server" DataSourceID="edsFromType" 
                                        DataTextField="Name" DataValueField="Code" Enabled="false" AppendDataBoundItems="True"
                                        SelectedValue='<%# Bind("FromType") %>' 
                                        Width="120px">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsFromType" runat="server" 
                                        ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                                        EnableFlattening="False" EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter DefaultValue="FromType" Name="Category" Type="String" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 120px;">
                                    <%--<asp:Label ID="ctlOperator" runat="server" Text='<%# Bind("Operator") %>' />--%>
                                    <%--<asp:DropDownList ID="ctlOperator" runat="server" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name" 
                                        DataValueField="Name" Enabled="false">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Buyer" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>--%>
                                    <asp:Label ID="lblOperator" runat="server" Text='<%# Bind("Operator") %>'></asp:Label>
                                    <asp:TextBox ID="txOperator" runat="server" Text='<%# Bind("Operator") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    供货单位：</td>
                                <td style="width: 210px;" >
                                    <asp:Label ID="lblSupplier" runat="server" Text='<%# Bind("Supplier") %>' />
                                    <%--<asp:DropDownList ID="ctlSupplier" runat="server" Width="200px" 
                                        DataSourceID="edsSupplier" DataTextField="Name" Enabled="false"
                                        DataValueField="Name" AppendDataBoundItems="True">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSupplier" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Supplier" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>--%>
                                    <asp:TextBox ID="txtSupplier" runat="server" Text='<%# Bind("Supplier") %>' Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlInCategory" runat="server" Enabled="false" Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsInCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="InCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtInCategory" runat="server" Text='<%# Bind("InCategory") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单状态：</td>
                                <td style="width: 140px;" >
                                    <asp:DropDownList ID="ctlSIStatus" runat="server" Enabled="false" Width="120px" AppendDataBoundItems="true"
                                        DataSourceID="edsSIStatus" DataTextField="Name" SelectedValue='<%# Bind("SIStatus") %>' 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">
                                    <%--<asp:DropDownList ID="ctlStoreKeeper" runat="server" Width="100px" 
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName" Enabled="false"
                                        DataValueField="UserName" AppendDataBoundItems="True" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">
                                    </asp:EntityDataSource>--%>
                                    <asp:Label ID="lblStoreKeeper" runat="server" Text='<%# Bind("StoreKeeper") %>' ></asp:Label>
                                   <asp:TextBox ID="txtStoreKeeper" runat="server" Text='<%# Bind("StoreKeeper") %>' Visible="false" ></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备注：</td>
                                <td style="width: 210px;" >
                                    <asp:Label ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                            </tr>                               
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    源Excel入库单号：</td>
                                <td style="width: 140px;">
                                     <asp:Label ID="ctlFromBillNo" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td colspan="6"></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    入库单号：</td>
                                <td style="width: 140px;">
                                    <asp:TextBox Width="130px" MaxLength="20" ID="ctlSICode" runat="server" Text='<%# Bind("SICode") %>' Enabled="false" />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlSIType" runat="server"  Width="120px" AppendDataBoundItems="True"
                                        DataSourceID="edsSIType" DataTextField="Name"
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                    <asp:TextBox ID="txtSIType" runat="server" Text='<%# Bind("SIType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    入库日期：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlSIDate" runat="server" Text='<%# Bind("SIDate", "{0:yyyy-MM-dd}") %>' />
                                    <ajaxToolkit:CalendarExtender ID="ctlSIDate_CalendarExtender" runat="server"
                                        TargetControlID="ctlSIDate" >
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 210px;" >
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200px" AppendDataBoundItems="True"
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
                                <td style="width: 120px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 140px;">
                                    <asp:TextBox Width="130px" MaxLength="20" ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlFromType" runat="server"   Width="120" AppendDataBoundItems="True"
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
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlOperator" runat="server" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Buyer" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txOperator" runat="server" Text='<%# Bind("Operator") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    供货单位：</td>
                                <td style="width: 210px;" >
                                    <asp:DropDownList ID="ctlSupplier" runat="server" Width="200px" 
                                        DataSourceID="edsSupplier" DataTextField="Name" 
                                        DataValueField="Name" AppendDataBoundItems="True" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSupplier" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category  and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Supplier" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtSupplier" runat="server" Text='<%# Bind("Supplier") %>' Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlInCategory" runat="server" Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsInCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="InCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtInCategory" runat="server" Text='<%# Bind("InCategory") %>' Visible="false"></asp:TextBox>
                                    <label class="commonSaveMsgLabel">*</label>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单状态：</td>
                                <td style="width: 140px;" >
                                    <asp:DropDownList ID="ctlSIStatus" runat="server" Enabled="false" Width="120" AppendDataBoundItems="true"
                                        DataSourceID="edsSIStatus" DataTextField="Name" SelectedValue='<%# Bind("SIStatus") %>' 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 120px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlStoreKeeper" runat="server" Width="100px" 
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName"
                                        DataValueField="UserName" AppendDataBoundItems="True" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtStoreKeeper" runat="server" Text='<%# Bind("StoreKeeper") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备注：</td>
                                <td style="width: 210px;" >
                                    <asp:TextBox Width="200px" MaxLength="100" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    源Excel入库单号：</td>
                                <td style="width: 140px;">
                                     <asp:TextBox ID="ctlFromBillNo" Width="130px" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td colspan="6"></td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <table class="tableForm">
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    入库单号：</td>
                                <td style="width: 140px;">
                                    <asp:TextBox Width="130px" MaxLength="20" ID="ctlSICode" runat="server" Text='<%# Bind("SICode") %>' Enabled="false" />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    单据类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlSIType" runat="server"  Width="120" AppendDataBoundItems="True"
                                        DataSourceID="edsSIType" DataTextField="Name"
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIType" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIType" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <label class="commonSaveMsgLabel">*</label>
                                     <asp:TextBox ID="txtSIType" runat="server" Text='<%# Bind("SIType") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    入库日期：</td>
                                <td style="width: 120px;">
                                    <asp:TextBox Width="100px" ID="ctlSIDate" runat="server"  Text='<%# Bind("SIDate", "{0:yyyy-MM-dd}") %>' />
                                    <ajaxToolkit:CalendarExtender ID="ctlSIDate_CalendarExtender" runat="server"
                                        TargetControlID="ctlSIDate" >
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    仓 库：</td>
                                <td style="width: 210px;" >
                                    <asp:DropDownList ID="ctlWHName" runat="server" Width="200px" AppendDataBoundItems="True"
                                      DataSourceID="edsInWHName" DataTextField="WHName" DataValueField="WHCode">
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
                                <td style="width: 120px;" class="tdKey">
                                    用友参考单号：</td>
                                <td style="width: 140px;">
                                    <asp:TextBox Width="130px" MaxLength="20" ID="ctlFromUCOrderNo" runat="server" Text='<%# Bind("FromUCOrderNo") %>' />
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单类型：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlFromType" runat="server"   Width="120" AppendDataBoundItems="True"
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
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    业务员：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlOperator" runat="server" Width="100" AppendDataBoundItems="True"
                                        DataSourceID="edsOperator" DataTextField="Name" 
                                        DataValueField="Name" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsOperator" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Buyer" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txOperator" runat="server" Text='<%# Bind("Operator") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    供货单位：</td>
                                <td style="width: 210px;" >
                                    <asp:DropDownList ID="ctlSupplier" runat="server" Width="200px" 
                                        DataSourceID="edsSupplier" DataTextField="Name" 
                                        DataValueField="Name" AppendDataBoundItems="True">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSupplier" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category and it.Enabled = true">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="Supplier" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtSupplier" runat="server" Text='<%# Bind("Supplier") %>' Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px;" class="tdKey">
                                    收发类别：</td>
                                <td style="width: 140px;">
                                    <asp:DropDownList ID="ctlInCategory" runat="server" Width="130px" AppendDataBoundItems="true"
                                        DataSourceID="edsInCategory" DataTextField="Name" 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsInCategory" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="InCategory" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtInCategory" runat="server" Text='<%# Bind("InCategory") %>' Visible="false"></asp:TextBox>
                                    <label class="commonSaveMsgLabel">*</label>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    订单状态：</td>
                                <td style="width: 140px;" >
                                    <asp:DropDownList ID="ctlSIStatus" runat="server" Enabled="false" Width="120" AppendDataBoundItems="true"
                                        DataSourceID="edsSIStatus" DataTextField="Name" SelectedValue='<%# Bind("SIStatus") %>' 
                                        DataValueField="Code" >
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsSIStatus" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="DataDict" Where="it.Category=@Category">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="Category" DefaultValue="SIStatus" />
                                        </WhereParameters>
                                    </asp:EntityDataSource>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    库管员：</td>
                                <td style="width: 120px;">
                                    <asp:DropDownList ID="ctlStoreKeeper" runat="server" Width="100px" 
                                        DataSourceID="edsStoreKeeper" DataTextField="UserName" 
                                        DataValueField="UserName" AppendDataBoundItems="True">
                                        <asp:ListItem Text="不限" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsStoreKeeper" runat="server" 
                                        ConnectionString="name=GoldEntities" 
                                        DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                        EntitySetName="Users">
                                    </asp:EntityDataSource>
                                    <asp:TextBox ID="txtStoreKeeper" runat="server" Text='<%# Bind("StoreKeeper") %>' Visible="false"></asp:TextBox>
                                </td>
                                <td style="width: 100px;" class="tdKey">
                                    备注：</td>
                                <td style="width: 210px;" >
                                    <asp:TextBox Width="200px" MaxLength="100" ID="ctlComment" runat="server" Text='<%# Bind("Comment") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px;" class="tdKey">
                                    源Excel入库单号：</td>
                                <td style="width: 140px;">
                                     <asp:TextBox ID="ctlFromBillNo" Width="130px" runat="server" Text='<%# Bind("FromBillNo") %>' />
                                </td>
                                <td colspan="6"></td>
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
                onclick="lbtnDeleteRow_Click" OnClientClick="javascript:return confirm('确认要删除选中商品吗？');"/>
            <asp:LinkButton ID="lbtnAddRow" runat="server" Text="增加商品" 
                onclick="lbtnAddRow_Click" />
        </h2>
        <div class="boxContent">
            <div class="orderItem" style="overflow-x: scroll; min-height:400px;border:solid 1px #999;">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    Width="1400px" DataSourceID="odsStockDetail" 
                    DataKeyNames="BillCode,BillRowNumber" 
                    onrowcommand="GridView1_RowCommand"
                    RowStyle-HorizontalAlign="Center"  CssClass="linetable" 
                    onrowdatabound="GridView1_RowDataBound">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="60px">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked="true" Text="" />
                            </ItemTemplate>
                            <HeaderTemplate>
                                全选<input id="CheckAll" type="checkbox" onclick="selectAll(this);" checked="checked" />
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BillRowNumber" HeaderText="序号" 
                            SortExpression="BillRowNumber" ReadOnly="True" ItemStyle-Width="40px" />
                        <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <asp:TextBox ID="ctlCargoCode" runat="server" Text='<%# Bind("CargoCode") %>' Visible ='<%# GetButtonVisible()%>' Width="120px" ></asp:TextBox>
                                <asp:Button runat="server" ID="SelectCargo" Text="..." CommandName="GoodsSelect" Visible ='<%# GetButtonVisible()%>'  />
                                <asp:Label runat="server" ID="lblCargoCode" Text='<%# Bind("CargoCode") %>' Visible ='<%# GetButtonVisible()==true?false:true %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="商品名称" SortExpression="CargoName" ItemStyle-Width="250px">
                            <ItemTemplate>
                                <asp:TextBox ID="ctlCargoName" runat="server" Text='<%# Bind("CargoName") %>' Visible ='<%# GetButtonVisible()%>' Width="180px"></asp:TextBox>
                                <asp:Button runat="server" ID="SelectCargoByName" Text="..." CommandName="GoodsSelect" Visible ='<%# GetButtonVisible()%>'  />
                                <asp:Label runat="server" ID="lblCargoName" Text='<%# Bind("CargoName") %>' Visible ='<%# GetButtonVisible()==true?false:true %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CargoModel" HeaderText="型 号" HtmlEncode="false"  ItemStyle-Width="60px"
                            SortExpression="CargoModel" />
                        <asp:BoundField DataField="CargoSpec" HeaderText="规 格" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="CargoSpec" />
                        <asp:BoundField DataField="CargoUnits" HeaderText="单位" HtmlEncode="false" ItemStyle-Width="40px"
                            SortExpression="CargoUnits" />                       
                        <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" HtmlEncode="false" ItemStyle-Width="75px"
                            SortExpression="NumOriginalPlan" />
                        <asp:TemplateField HeaderText="应收数量" SortExpression="NumCurrentPlan" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtNumCurrentPlan" Width="60px" runat="server" Text='<%# Bind("NumCurrentPlan") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumActual" HeaderText="实收数量" HtmlEncode="false" ItemStyle-Width="75px"
                            SortExpression="NumActual" />                        
                        <asp:TemplateField HeaderText="仓库" SortExpression="InOutWHCode">
                            <ItemTemplate>
                                <asp:DropDownList ID="ctlInOutWHCode" runat="server" Width="110px" SelectedValue='<%# Bind("InOutWHCode") %>'
                                     DataSourceID="edsWarehouse" DataTextField="WHName" DataValueField="WHCode"
                                      AppendDataBoundItems="true" OnDataBound="ctlInOutWHCode_DataBound">
                                    <asp:ListItem Text="--选择--" Value="" Selected="True" />
                                </asp:DropDownList>
                                <%--<asp:RequiredFieldValidator ID="rfvInOutWHCode" ForeColor="Red" ControlToValidate="ctlInOutWHCode"
                                                runat="server" ErrorMessage="请选择仓库！"></asp:RequiredFieldValidator>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="区位" SortExpression="BinCode">
                            <ItemTemplate>
                                <asp:DropDownList ID="ctlBinCode" runat="server" Width="90px"  DataTextField="BinCode" DataValueField="BinCode"
                                      AppendDataBoundItems="true" >
                                    <asp:ListItem Text="--选择--" Value="" Selected="True" />
                                </asp:DropDownList>
                                <%----%>
                                <asp:TextBox ID="tbxBinCode_Selected" runat="server" Text='<%# Bind("BinCode") %>' Visible="false"></asp:TextBox>
                                <%--<asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=GoldEntities"
                                    DefaultContainerName="GoldEntities" EnableFlattening="False" 
                                    EntitySetName="CargoTag" 
                                    Select="it.BinCode"                                    
                                    Where="it.CargoCode=@CargoCode or it.CargoCode==null">
                                        <WhereParameters>
                                            <asp:Parameter Type="String" Name="CargoCode" DefaultValue='<%# Eval("CargoCode") %>' />
                                        </WhereParameters>
                                </asp:EntityDataSource>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" HtmlEncode="false" ItemStyle-Width="100px"
                            SortExpression="UCOrderNo" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="Comment" />
                        <asp:TemplateField HeaderText="商品状态" SortExpression="CargoStatus" ItemStyle-Width="60px">
                            <ItemTemplate><asp:Label runat="server" ID="lblCargoStatus" Text='<%# Eval("CargoStatus")==null?"":Eval("CargoStatus").ToString()=="0"?"未完成":"已完成" %>'></asp:Label></ItemTemplate>
                         </asp:TemplateField>
                        <asp:BoundField DataField="ReleaseYear" HeaderText="商品发行年份" HtmlEncode="false" ItemStyle-Width="75px"
                            SortExpression="ReleaseYear" />
                        <asp:BoundField DataField="BillCode" HeaderText="BillCode" ReadOnly="True" HtmlEncode="false"  
                            SortExpression="BillCode" Visible="False" />
                        <asp:BoundField DataField="BillType" HeaderText="BillType" HtmlEncode="false" 
                            SortExpression="BillType" Visible="False" />
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="NumCurrentPlan" HtmlEncode="false" 
                            SortExpression="NumCurrentPlan" Visible="False" />
                        <asp:BoundField DataField="RowTotalMoney" HeaderText="RowTotalMoney" HtmlEncode="false" 
                            SortExpression="RowTotalMoney" Visible="False" />
                        <asp:BoundField DataField="RFIDOrderNo" HeaderText="RFIDOrderNo" HtmlEncode="false" 
                            SortExpression="RFIDOrderNo" Visible="False" />
                        <asp:TemplateField  HeaderText="BillRowNumber" 
                            SortExpression="BillRowNumber" Visible="False">
                            <ItemTemplate>
                                <asp:Label ID="lblBillRowNumber" runat="server" Text='<%# Eval("BillRowNumber") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
                    Width="1600px" DataSourceID="odsStockDetail" 
                    DataKeyNames="BillCode,BillRowNumber" RowStyle-HorizontalAlign="Center"  CssClass="linetable">
                    <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
                    <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
                    <RowStyle CssClass="GridViewRowStyle" />
                    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                    <Columns>
                        <asp:BoundField DataField="BillRowNumber" HeaderText="序号"  ItemStyle-Width="40px" 
                            SortExpression="BillRowNumber" ReadOnly="True" />
                        <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="TextBox1" runat="server" Text='<%# Bind("CargoCode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CargoName" HeaderText="名 称" HtmlEncode="false" ItemStyle-Width="250px"
                            SortExpression="CargoName" />
                        <asp:BoundField DataField="CargoModel" HeaderText="型 号" HtmlEncode="false"  ItemStyle-Width="60px"
                            SortExpression="CargoModel" />
                        <asp:BoundField DataField="CargoSpec" HeaderText="规 格" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="CargoSpec" />
                        <asp:BoundField DataField="CargoUnits" HeaderText="单位" HtmlEncode="false" ItemStyle-Width="40px"
                            SortExpression="CargoUnits" />                        
                        <asp:BoundField DataField="NumOriginalPlan" HeaderText="订单数量" HtmlEncode="false" ItemStyle-Width="75px"
                            SortExpression="NumOriginalPlan" />
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="应收数量" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="NumCurrentPlan" />
                        <asp:BoundField DataField="NumActual" HeaderText="实收数量" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="NumActual" />
                        <%--<asp:TemplateField HeaderText="实收数量" SortExpression="NumActual">
                            <ItemTemplate>
                                <asp:Label ID="TextBox2" runat="server" Text='<%# Bind("NumActual") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:BoundField DataField="InOutWHName" HeaderText="仓库" HtmlEncode="false" ItemStyle-Width="110px"
                            SortExpression="InOutWHName" />
                        <asp:BoundField DataField="BinCode" HeaderText="区位" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="BinCode" />
                        <asp:BoundField DataField="UCOrderNo" HeaderText="来源单号" HtmlEncode="false" ItemStyle-Width="100px"
                            SortExpression="UCOrderNo" />
                        <asp:BoundField DataField="Comment" HeaderText="备注" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="Comment" />
                        <asp:TemplateField HeaderText="状态" SortExpression="CargoStatus" ItemStyle-Width="60px">
                            <ItemTemplate><asp:Label runat="server" ID="lblCargoStatus" Text='<%# Eval("CargoStatus")==null?"":Eval("CargoStatus").ToString()=="0"?"未完成":"已完成" %>'></asp:Label></ItemTemplate>
                         </asp:TemplateField>                        
                        <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" HtmlEncode="false" ItemStyle-Width="80px"
                            SortExpression="ReleaseYear" />
                        <asp:BoundField DataField="BillCode" HeaderText="BillCode" ReadOnly="True" HtmlEncode="false"  
                            SortExpression="BillCode" Visible="False" />
                        <asp:BoundField DataField="BillType" HeaderText="BillType" HtmlEncode="false" 
                            SortExpression="BillType" Visible="False" />
                        <asp:BoundField DataField="NumCurrentPlan" HeaderText="NumCurrentPlan" HtmlEncode="false" 
                            SortExpression="NumCurrentPlan" Visible="False" />
                        <asp:BoundField DataField="RowTotalMoney" HeaderText="RowTotalMoney" HtmlEncode="false" 
                            SortExpression="RowTotalMoney" Visible="False" />
                        <asp:BoundField DataField="RFIDOrderNo" HeaderText="RFIDOrderNo" HtmlEncode="false" 
                            SortExpression="RFIDOrderNo" Visible="False" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
    <hr />
    <div class="orderFooter">
        <asp:Button ID="btnSubmit" runat="server" Text="提交至备货" CssClass="ButtonImageStyle" 
                    onclick="btnSubmit_Click" Visible="false" OnClientClick="javascript:return confirm('确定提交吗？');" />
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
        <asp:Button ID="btnReturn" runat="server" Text="返回入库单列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturn_Click" Visible="false" />
        <asp:Button ID="btnReturnPurchase" runat="server" Text="返回采购列表" CssClass="ButtonMidWidthImageStyle" 
          onclick="btnReturnPurchase_Click" Visible="false" />
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
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopupCargo">
        <div class="modalPopupWrapperCargo">
            <div id="pnlDragTitle" class="modalHeader">
                <span>选择商品</span>
                <asp:Button ID="btnClosePop" runat="server" CssClass="ClosePopButton" Text="关  闭" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <uc:GoodsSelect ID="GoodsSelect1" runat="server" OnGetCargoSelect="GoodsSelect1_GetCargoSelect"  />
            </div>
        </div>
    </asp:Panel>

    <%--<asp:LinkButton ID="lblttt" runat="server"  Text="sss" />--%>
    
    <asp:LinkButton ID="LinkButton1" runat="server" width="0" Height="0"></asp:LinkButton>
    <ajaxToolkit:ModalPopupExtender ID="popWindow" runat="server" TargetControlID="LinkButton1"
        PopupControlID="pnlPopWindow" BackgroundCssClass="modalBackground" DropShadow="true"
        PopupDragHandleControlID="pnlDragTitle">
    </ajaxToolkit:ModalPopupExtender>




</asp:Content>
