<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InOutCollectListRpt.aspx.cs" Inherits="Gold.Query.InOutCollectListRpt" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" >
    <div class="commonPage">
        <div class="commonTitle">
            <label>
                库存商品收发汇总报表</label>
        </div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(集合)" WaitMessageFont-Names="Verdana" 
            WaitMessageFont-Size="14pt" Width="100%" Height="600px" 
            CssClass="reportCss">
            <LocalReport ReportPath="Reports\Report_InOutCollectByCargo.rdlc">
            </LocalReport>
        </rsweb:ReportViewer>
        <br />
        <asp:Button ID="btnGoToList" runat="server" CssClass="ButtonImageStyle" Text="返回"
                CausesValidation="false" PostBackUrl="~/Query/InOutCollectList.aspx" />
                <br />
         <asp:Label ID="lblGridViewMsg" CssClass="commonSaveMsgLabel" runat="server" Text=""></asp:Label>
         
</div> 
</asp:Content> 
