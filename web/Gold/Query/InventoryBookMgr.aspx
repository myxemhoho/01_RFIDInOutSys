<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryBookMgr.aspx.cs" Inherits="Gold.Account.InventoryBook" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <h2>
            <span>查询条件</span>&nbsp;&nbsp;
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
        </h2>
        <div class="boxContent">
            <table>
                <tr>
                    <td style="width: 70px; text-align: right">
                        商品名称：</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtCargoName" runat="server" MaxLength="32" Width="90px" />
                    </td>
                    <td style="width: 70px; text-align: right">
                        商品编码：
                     
                    </td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtCargoCode" runat="server" MaxLength="32" Width="90px" />
                    </td>

                    <td style="width: 70px; text-align: right">
                        台帐日期：
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartTime" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server"
                            TargetControlID="txtStartTime" >
                        </ajaxToolkit:CalendarExtender>
                        至
                         
                        <asp:TextBox ID="txtEndTime" runat="server" MaxLength="32" Width="80px" />
                        <ajaxToolkit:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Enabled="True"
                            TargetControlID="txtEndTime">
                        </ajaxToolkit:CalendarExtender>
                    </td>
                    <td>仓库：</td>
                    <td>
                        <asp:DropDownList ID="DDLWHName" runat="server" MaxLength="32" Width="90px" 
                            DataSourceID="EntityDataSource2" DataTextField="WHName" 
                            DataValueField="WHName" >
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            Text="查 询" onclick="btnQuery_Click" Height="23px" Width="91px" />
                    </td>
                               
                </tr>
                
            </table>
        </div>
    </div>
    <div class="box">
        <h2>
            <span>订单列表</span>
            </h2>
        <div class="boxContent" style="overflow-x: scroll;height:300px;">
            <asp:GridView ID="GridView1" runat="server" Width="1300px"
                AutoGenerateColumns="False" DataKeyNames="TableAccountID,CargoCode" ShowFooter="True" 
                DataSourceID="EntityDataSource1">
                <Columns>
                               
                    <asp:TemplateField FooterText="留空" HeaderText="台账ID" 
                        SortExpression="TableAccountID">
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("TableAccountID") %>'></asp:Label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("TableAccountID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="商品名称" SortExpression="CargoName">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CargoName") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("CargoName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="发行年份" 
                        SortExpression="ReleaseYear">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ReleaseYear") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("ReleaseYear") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="商品规格" SortExpression="CargoSpec">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("CargoSpec") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("CargoSpec") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="商品型号" 
                        SortExpression="CargoModel">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("CargoModel") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("CargoModel") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="商品单位" 
                        SortExpression="CargoUnits">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("CargoUnits") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("CargoUnits") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="台账时间" 
                        SortExpression="ChangeTime">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("ChangeTime") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("ChangeTime") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText='<% = ABC()%>' HeaderText="RFID订单号" 
                        SortExpression="OrderCode">
                        <EditItemTemplate> 
                            <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("OrderCode") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("OrderCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="留空" HeaderText="备注" SortExpression="Remark">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("Remark") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("Remark") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="1" HeaderText="期初数量" 
                        SortExpression="NumOriginal">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("NumOriginal") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("NumOriginal") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="2" HeaderText="收入数量" SortExpression="NumAdd">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("NumAdd") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("NumAdd") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="3" HeaderText="发出数量" SortExpression="NumDel">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("NumDel") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label12" runat="server" Text='<%# Bind("NumDel") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="4" HeaderText="结存数量" SortExpression="NumCurrent">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox12" runat="server" Text='<%# Bind("NumCurrent") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label13" runat="server" Text='<%# Bind("NumCurrent") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField FooterText="5" HeaderText="记账人姓名" 
                        SortExpression="ChangePerson">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("ChangePerson") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label14" runat="server" Text='<%# Bind("ChangePerson") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                        <asp:TemplateField><FooterTemplate><% = ABC()%></FooterTemplate></asp:TemplateField>


                    <%--<asp:BoundField DataField="CargoCode" HeaderText="商品编码" ReadOnly="True" 
                        SortExpression="CargoCode" />
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" 
                        SortExpression="CargoName" />
                    <asp:BoundField DataField="CargoModel" HeaderText="商品型号" 
                        SortExpression="CargoModel" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="商品规格" 
                        SortExpression="CargoSpec" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="商品单位" 
                        SortExpression="CargoUnits" />
                    <asp:BoundField DataField="CargoStatus" HeaderText="商品状态" 
                        SortExpression="CargoStatus" />
                    <asp:BoundField DataField="ReleaseYear" HeaderText="发行年份" 
                        SortExpression="ReleaseYear" />
                    <asp:BoundField DataField="OrderCode" HeaderText="RFID订单号" 
                        SortExpression="OrderCode" />
                    <asp:BoundField DataField="UCOrderCode" HeaderText="用友订单号" 
                        SortExpression="UCOrderCode" />
                    <asp:BoundField DataField="StockBillNo" HeaderText="出入库单号" 
                        SortExpression="StockBillNo" />
                    <asp:BoundField DataField="StockBillType" HeaderText="出入库单据类型" 
                        SortExpression="StockBillType" />
                    <asp:BoundField DataField="WHCode" HeaderText="仓库编码" 
                        SortExpression="WHCode" />
                    <asp:BoundField DataField="ChangeType" HeaderText="变更类型" 
                        SortExpression="ChangeType" />
                    <asp:BoundField DataField="ChangePersonID" HeaderText="记账人ID" 
                        SortExpression="ChangePersonID" />
                    <asp:BoundField DataField="ChangePerson" HeaderText="记账人姓名" 
                        SortExpression="ChangePerson" />
                    <asp:BoundField DataField="NumOriginal" HeaderText="期初数量" 
                        SortExpression="NumOriginal" />
                    <asp:BoundField DataField="NumAdd" HeaderText="收入数量" 
                        SortExpression="NumAdd" />
                    <asp:BoundField DataField="NumDel" HeaderText="发出数量" 
                        SortExpression="NumDel" />
                    <asp:BoundField DataField="NumCurrent" HeaderText="结存数量" 
                        SortExpression="NumCurrent" />
                    <asp:BoundField DataField="Remark" HeaderText="备注" 
                        SortExpression="Remark" />                                      
                    <asp:BoundField DataField="Reserve1" HeaderText="预留1" 
                        SortExpression="Reserve1" />--%>

                </Columns>
            </asp:GridView>   
            <div>
            <table>

                          <td style="width: 70px; text-align: right">
                                     统计：</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="32" Width="90px"> 

                        
                  </asp:TextBox>
                    </td>
              </table>
            
            </div>       


<asp:EntityDataSource ID="EntityDataSource1" runat="server" 
                ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                EnableFlattening="False" EntitySetName="InventoryBook" 
                onquerycreated="EntityDataSource1_QueryCreated">
            </asp:EntityDataSource>
                        <asp:EntityDataSource ID="EntityDataSource2" runat="server" 
                            ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" 
                            EnableFlattening="False" EntitySetName="WareHouse" Select="it.[WHName]">
                        </asp:EntityDataSource>
        </div>
        
    </div>
    <asp:Panel ID="pnlPopWindow" runat="server" Style="display: none" CssClass="modalPopup">
        <div class="modalPopupWrapper">
            <div id="pnlDragTitle" class="modalHeader">
                <span>采购订单文件导入</span>
                <asp:Button ID="btnClosePop" runat="server" Text="OK" OnClick="btnClosePop_Click" />
            </div>
            <div class="modalBody">
                <div id="silverlightControlHost" class="uploadControl">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="../ClientBin/SLFileUpload.xap" />
                        <param name="initParams" value="UploadPage=../Upload/FileUpload.ashx,FileType=采购订单" />
                        <param name="onError" value="onSilverlightError" />
                        <param name="background" value="white" />
                        <param name="minRuntimeVersion" value="5.0.61118.0" />
                        <param name="autoUpgrade" value="true" />
                      
                        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration: none">
                        <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="获取 Microsoft Silverlight"
                                style="border-style: none" />
                        </a>
                    </object>
                    <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px;
                        border: 0px"></iframe>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
