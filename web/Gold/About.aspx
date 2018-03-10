<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="Gold.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>

    <div>
        <asp:SiteMapDataSource runat="Server" ID="SiteMapDataSource1" />        
        <asp:GridView ID="gridNavigationLinks" runat="server" 
          DataSourceID="SiteMapDataSource1" AutoGenerateColumns="false">
          <Columns>
            <asp:TemplateField>
              <ItemTemplate>
                <a href='<%# Eval("Url") %>'><%# Eval("Title") %></a>
                <br/>
                <%# Eval("Description") %>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
    </asp:GridView>
    </div>
</asp:Content>
