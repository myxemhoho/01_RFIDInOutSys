<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadFile.aspx.cs" Inherits="Gold.Order.UploadFile" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="absmiddle" alt="" src="../Styles/images/loadingPic.gif" /></asp:Label>

    <ajaxToolkit:AjaxFileUpload ID="AjaxFileUpload1" runat="server" Padding-Bottom="4"
            Padding-Left="2" Padding-Right="1" Padding-Top="4" ThrobberID="myThrobber" 
            OnUploadComplete="AjaxFileUpload1_OnUploadComplete" 
            MaximumNumberOfFiles="10"
            AllowedFileTypes="xls,xlsx" />
</asp:Content>
