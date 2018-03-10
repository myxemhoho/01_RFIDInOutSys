<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SaleAllocationQueryRpt.aspx.cs" Inherits="Gold.SaleCargoSetting.SaleAllocationQueryRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" >
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                可售商品查询报表</label>
        </div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(集合)" WaitMessageFont-Names="Verdana" 
            WaitMessageFont-Size="14pt" Width="100%" Height="600px" CssClass="reportCss">
            <LocalReport ReportPath="Reports\Report_SaleAllocation.rdlc">
            </LocalReport>
        </rsweb:ReportViewer>

        <br />
        <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" Text="返回"
                CausesValidation="false" PostBackUrl="~/SaleCargoSetting/SaleAllocationQuery.aspx" />
                <br />
         <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
         
</div> 
</asp:Content> 
