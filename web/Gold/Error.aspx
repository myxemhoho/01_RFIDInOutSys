﻿<%@ Page Title="出错啦！~" Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Gold.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>出错啦~~！！</title>
    <link href="Styles/Site.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="Error">
        <div style="height:150px"></div>
        <asp:Label ID="Label1" runat="server" Text="Label" CssClass="ErrorMsg"></asp:Label>
    </div>
    </form>
</body>
</html>