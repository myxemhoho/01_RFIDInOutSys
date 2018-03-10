<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BinCodeSelect.ascx.cs" Inherits="Gold.Controls.BinCodeSelect" %>
<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
<script language="javascript" type="text/javascript">
    function selectAll2(obj) {
        var theTable = document.getElementById("<%=grdBinCode.ClientID%>");  //obj..parentElement.parentElement.parentElement;
       
            var nodeList = theTable.getElementsByTagName("input");
            for (var i = 0; i < nodeList.length; i++) {
                var node = nodeList[i];
                if (node.type == "checkbox") {
                    node.checked = obj.checked;
                }
            }
            
    }
    </script>

  <div  style="border:solid 5px #6495ED">
    <%--<div class="commonTitle">
        <label> 查询条件</label>
    </div>--%>   
        <table style="width:100%">
        <tr>
            <td style="text-align:right; width:100px;">仓库编码：</td>
            <td style="width:120px;"><asp:TextBox ID="txtWHCode" runat="server" MaxLength="32" Width="100px" /></td>
            <td style="text-align:right; width:100px;">层位编码：</td>
            <td style="width:120px;"><asp:TextBox ID="txtBinCode" runat="server" MaxLength="32" Width="120px" /></td>
            <td>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnQuery" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                    Text="查 询" onclick="btnQuery_Click" />
            </td>
        </tr>
    </table>
   
     <%--<div class="commonTitle">
            <div class="CommonTitle_InnerdivLeft">
                <label>
                    查询结果</label>
            </div>            
    </div>--%>
    <hr />
    <div class="boxContent" style="overflow-x: scroll; height:420px;">
        <asp:GridView runat="server" ID="grdBinCode" AllowPaging="True" PageSize="10" 
            AutoGenerateColumns="False" DataKeyNames="BinCode" Width="100%"  OnDataBound="grdBinCode_DataBound"  
            DataSourceID="edsBinCode" onrowcommand="grdBinCode_RowCommand" 
            CssClass="linetable"   OnPageIndexChanging="grdBinCode_PageIndexChanging"          >
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" Height="40px" />
            <SortedAscendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <SortedDescendingHeaderStyle CssClass="GridViewHeaderSortStyle" />
            <RowStyle CssClass="GridViewRowStyle" />
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <Columns>
                <%--<asp:TemplateField HeaderText="序号">
                    <ItemTemplate>
                            <%# Container.DataItemIndex+1 %>                            
                        </ItemTemplate>
                </asp:TemplateField>  --%>
                <asp:TemplateField ItemStyle-Width="60px">
                        <ItemTemplate>
                            <asp:CheckBox ID="chbSelect" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                                全选<input id="CheckAll" type="checkbox" onclick="selectAll2(this);" />
                        </HeaderTemplate>
                    </asp:TemplateField>             
                <%--<asp:TemplateField HeaderText="层位编码" SortExpression="BinCode">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnBinCode" runat="server" Text='<%# Bind("BinCode") %>' CommandName="BinCodeSelect" ></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField DataField="BinCode" HeaderText="层位编码" HtmlEncode="false" 
                    SortExpression="BinCode"/>
                <asp:BoundField DataField="BinName" HeaderText="层位名称" HtmlEncode="false" 
                    SortExpression="BinName" />                
                <asp:BoundField DataField="Number" HeaderText="商品数量"  HtmlEncode="false" 
                    SortExpression="Number" />                
                <asp:BoundField DataField="CargoUnits" HeaderText="单位"  HtmlEncode="false" 
                    SortExpression="CargoUnits" />
                <asp:BoundField DataField="WHCode" HeaderText="仓库编码" HtmlEncode="false"  
                    SortExpression="WHCode" />
                <asp:BoundField DataField="WHName" HeaderText="仓库名称"  HtmlEncode="false" 
                    SortExpression="WHName" />                                
                <%--<asp:BoundField DataField="Comment" HeaderText="备注"  HtmlEncode="false" 
                    SortExpression="Comment" />--%>                                   
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
        <asp:EntityDataSource ID="edsBinCode" runat="server"
         ConnectionString="name=GoldEntities" DefaultContainerName="GoldEntities" EntitySetName="VBinCode" 
         onquerycreated="edsBinCode_QueryCreated" onselecting="edsBinCode_Selecting" 
        >
        </asp:EntityDataSource>
        <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
        <br />
        <div>
           <table style="text-align:center; width:100%;">
                <tr>            
                    <td>
                        <asp:Button ID="btnOk" runat="server" CausesValidation="false" CssClass="ButtonImageStyle"
                            Text="确 定" onclick="btnOK_Click" />
                    </td>
                </tr>
            </table> 
        </div>       
    </div> 
</div>


