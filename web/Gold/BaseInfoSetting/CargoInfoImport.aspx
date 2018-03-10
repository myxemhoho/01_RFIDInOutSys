<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CargoInfoImport.aspx.cs" Inherits="Gold.BaseInfoSetting.CargoInfoImport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        //js动态添加fileupload控件
        function addFile() {
            var str = '<INPUT type="file" runat="server" size="50" NAME="File"><br />'
            document.getElementById('MyFile').insertAdjacentHTML("beforeEnd", str)
        }

        function showWaitDiv(divName) {            
            document.getElementById(divName).style.display = "block";
        }
        function hiddenWaitDiv(divName) {
            document.getElementById(divName).style.display = "none";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commonPage">
        <div class="commonTitle">
            <asp:Label ID="lblTitle" runat="server" Text="商品信息导入"></asp:Label>
        </div>
        <div class="commonQuery">
            <div style="font-family: 微软雅黑; height: 105px; font-size: 9pt; font-weight: bold; border: solid 1px black;
                padding: 10px;">
                <div style="width: 48%; float: left;">
                    <span style="font-size: 11pt; text-decoration:underline;">单文件导入操作说明:</span>
                    <br />
                    1.导入一个Excel文件数据时，首先点击“浏览”选择一个包含商品信息的Excel文件（仅限小于4M的Excel文件）<br />
                    2.点击“导入前预览”，Excel数据将加载到预览窗口。<br />
                    3.在预览窗口核对数据无误后点击“导入到系统”,系统进行导入并提示导入结果。<br />
                </div>
                <div style="width: 48%; float: right;">
                    <span style="font-size: 11pt; text-decoration:underline;">多文件导入操作说明:</span>
                    <br />
                    1.导入多个Excel文件数据时，首先点击“增加”按钮增加多个文件上传控件。<br />
                    2.然后对每个“浏览”按钮点击选择一个包含商品信息的Excel文件（仅限小于4M的Excel文件）。<br />
                    3.点击“导入前预览”，Excel数据将加载到预览窗口。<br />
                    4.在预览窗口核对数据无误后点击“导入到系统”,系统进行导入并提示导入结果。<br />
                </div>
            </div>
            <div>
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div style=" color:Black; font-weight:bold; font-size:12pt;"><img src="../Styles/images/siteMapPath.png" alt="" />&nbsp;&nbsp;1.选择文件<hr /></div>
                        </div>
                        <input type="button" class="ButtonImageStyle" value="增加" onclick="addFile()" />
                        <input onclick="this.form.reset()" class="ButtonImageStyle" type="button" value="重置" />
                        <div id="MyFile" style="width: 800px;">
                            <input type="file" runat="server" size="50" name="File" /></div>
                        <asp:Button runat="server" Text="导入前预览" CssClass="ButtonImageStyle" ID="btnImportPreview" OnClientClick="showWaitDiv('waitDiv1');"
                            OnClick="btnImportPreview_Click"></asp:Button>
                            <div id="waitDiv1" style=" display:none;"><img src="../Styles/images/uploading.gif" /><label>正在执行中,请稍候……</label></div>
                            <!--注意必须将btnImportPreview放入UpdatePanel中且设置PostBackTrigger-->
                        <br />
                        <br />
                        <asp:Label ID="strStatus" runat="server" Font-Names="宋体" Font-Bold="True" Font-Size="9pt"
                            Width="100%" BorderStyle="None" BorderColor="White"></asp:Label>
                        <br />
                        <asp:Label ID="lblPreviewResult" runat="server" CssClass="commonSaveMsgLabel" Text=""></asp:Label>
                        <div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnImportPreview" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <br />
                <div style=" color:Black; font-weight:bold; font-size:12pt;"><img src="../Styles/images/siteMapPath.png" alt="" />&nbsp;&nbsp;2.导入前预览<hr /></div>
            </div>
            <div style="width:98%; height:600px; padding:5px 5px 20px 5px; overflow:auto;  border:solid 1px black;">
            <asp:GridView ID="gv_CargoList_Preview" runat="server" AllowPaging="false" AllowSorting="false"
                CssClass="linetable" AutoGenerateColumns="False">
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
                                备注
                            </td>
                        </tr>
                        <tr class="GridViewEmpty_RowData">
                            <td colspan="7">
                                无数据
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" Width="50px" />
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品编码" SortExpression="CargoCode">
                        <ItemStyle Width="180px" />
                        <ItemTemplate>
                            <asp:Label ID="lblCargoCode" runat="server" Text='<%# Eval("CargoCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CargoName" HeaderText="商品名称" ItemStyle-Width="300px" />
                    <asp:BoundField DataField="CargoModel" HeaderText="型号" ItemStyle-Width="80px" />
                    <asp:BoundField DataField="CargoSpec" HeaderText="规格" ItemStyle-Width="80px" />
                    <asp:BoundField DataField="CargoUnits" HeaderText="单位" ItemStyle-Width="60px" />
                    <asp:BoundField DataField="ProduceYear" HeaderText="发行年份" ItemStyle-Width="40px" />
                    <asp:BoundField DataField="Comment" HeaderText="备注" ItemStyle-Width="300px" />
                </Columns>
                <PagerTemplate>
                </PagerTemplate>
            </asp:GridView>
            </div>
            <div>
                <br />
                <br />
                <div style=" color:Black; font-weight:bold; font-size:12pt;"><img src="../Styles/images/siteMapPath.png" alt="" />&nbsp;&nbsp;3.导入到系统<hr /></div>
            </div>
            <asp:Button runat="server" Text="导入到系统" CssClass="ButtonImageStyle" CausesValidation="false" OnClientClick="showWaitDiv('waitdiv2');"
                ID="btnImportToSystem" OnClick="btnImportToSystem_Click"></asp:Button>
            <asp:Button ID="Button1" runat="server" CssClass="ButtonImageStyle" Text="返回" CausesValidation="false"
                PostBackUrl="~/BaseInfoSetting/CargoList.aspx" />
            <div id="waitdiv2" style=" display:none;"><img src="../Styles/images/uploading.gif" /><label>正在执行中，请稍候……</label></div>
            <br />
            <asp:Label ID="lblSaveMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <br />
        </div>
    </div>
</asp:Content>
